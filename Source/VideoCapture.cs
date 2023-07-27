using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using MonoMod.RuntimeDetour;

namespace Celeste.Mod.TASRecorder;

public static class VideoCapture {

    private static Hook hook_Game_Tick;
    private static Hook hook_GraphicsDevice_SetRenderTarget;

    internal static void Load() {
        hook_Game_Tick = new Hook(
            typeof(Game).GetMethod("Tick"),
            on_Game_Tick
        );
        hook_GraphicsDevice_SetRenderTarget = new Hook(
            typeof(GraphicsDevice).GetMethod("SetRenderTarget", new Type[] { typeof(RenderTarget2D) }),
            on_GraphicsDevice_SetRenderTarget
        );
    }
    internal static void Unload() {
        hook_Game_Tick.Dispose();
        hook_GraphicsDevice_SetRenderTarget.Dispose();
    }

    // Hijacks SetRenderTarget(null) calls to point to our captureTarget instead of the back buffer.
    private static bool hijackBackBuffer = false;
    private static RenderTarget2D captureTarget;

    private unsafe static void CaptureFrame() {
        int width = captureTarget.Width;
        int height = captureTarget.Height;

        Color[] buffer = new Color[width * height];
        captureTarget.GetData(buffer);

        TASRecorderModule.Encoder.PrepareVideo(width, height);
        fixed (Color* srcData = buffer) {
            int srcRowStride = width * sizeof(Color);
            int dstRowStride = TASRecorderModule.Encoder.VideoRowStride;

            byte* src = (byte*)srcData;
            byte* dst = TASRecorderModule.Encoder.VideoData;

            for (int i = 0; i < height; i++) {
                NativeMemory.Clear(dst, (nuint)dstRowStride);
                NativeMemory.Copy(src, dst, (nuint)srcRowStride);
                src += srcRowStride;
                dst += dstRowStride;
            }
        }
        TASRecorderModule.Encoder.FinishVideo();
    }

    // Taken from Engine.UpdateView(), but without depending on presentation parameters
    private static void UpdateEngineView(int width, int height) {
        if (width / (float)Engine.Width > height / (float)Engine.Height)
        {
            Engine.ViewWidth = (int)(height / (float)Engine.Height * (float)Engine.Width);
            Engine.ViewHeight = (int)height;
        }
        else
        {
            Engine.ViewWidth = (int)width;
            Engine.ViewHeight = (int)(width / (float)Engine.Width * (float)Engine.Height);
        }
        float ratio = (float)Engine.ViewHeight / (float)Engine.ViewWidth;
        Engine.ViewWidth -= Engine.ViewPadding * 2;
        Engine.ViewHeight -= (int)(ratio * (float)Engine.ViewPadding * 2f);
        Engine.ScreenMatrix = Matrix.CreateScale((float)Engine.ViewWidth / (float)Engine.Width);
        Viewport viewport = default;
        viewport.X = (int)(width / 2f - (float)(Engine.ViewWidth / 2));
        viewport.Y = (int)(height / 2f - (float)(Engine.ViewHeight / 2));
        viewport.Width = Engine.ViewWidth;
        viewport.Height = Engine.ViewHeight;
        viewport.MinDepth = 0f;
        viewport.MaxDepth = 1f;
        Engine.Viewport = viewport;
    }

    // We need to use a modified version of the main game loop to avoid skipping frames
    private delegate void orig_Game_Tick(Game self);
    [SuppressMessage("Microsoft.CodeAnalysis", "IDE1006")]
    private static void on_Game_Tick(orig_Game_Tick orig, Game self) {
        if (!TASRecorderModule.Recording || !TASRecorderModule.Encoder.HasVideo) {
            orig(self);
            return;
        }

        Syncing.SyncWithAudio();

        FNAPlatform.PollEvents(self, ref self.currentAdapter, self.textInputControlDown, ref self.textInputSuppress);

        self.gameTime.ElapsedGameTime = self.TargetElapsedTime;
        self.gameTime.TotalGameTime = self.TargetElapsedTime;

        var device = Celeste.Instance.GraphicsDevice;
        if (captureTarget == null || device.Viewport.Width != captureTarget.Width || device.Viewport.Height != captureTarget.Height) {
            captureTarget?.Dispose();
            captureTarget = new RenderTarget2D(device, TASRecorderModule.Settings.VideoWidth, TASRecorderModule.Settings.VideoHeight, mipMap: false, device.PresentationParameters.BackBufferFormat, DepthFormat.None);
        }

        self.Update(self.gameTime);
        RecordingRenderer.Update();

        if (self.BeginDraw()) {
            int oldWidth = Engine.ViewWidth;
            int oldHeight = Engine.ViewHeight;
            var oldMatrix = Engine.ScreenMatrix;
            var oldViewport = Engine.Viewport;
            UpdateEngineView(captureTarget.Width, captureTarget.Height);
            hijackBackBuffer = true;

            self.Draw(self.gameTime);

            hijackBackBuffer = false;
            Engine.ViewWidth = oldWidth;
            Engine.ViewHeight = oldHeight;
            Engine.ScreenMatrix = oldMatrix;
            Engine.Viewport = oldViewport;

            if (TASRecorderModule.Recording)
                CaptureFrame(); // Recording might have stopped, in the mean time

            // Render our capture to the screen
            var matrix = Matrix.CreateScale(Engine.ViewWidth / (float)captureTarget.Width);

            device.SetRenderTarget(null);
            device.Clear(Color.Black);
            device.Viewport = Engine.Viewport;
            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null, matrix);
            Draw.SpriteBatch.Draw(captureTarget, Vector2.Zero, Color.White);
            Draw.SpriteBatch.End();

            RecordingRenderer.Render();

            self.EndDraw();
        }
    }

    private delegate void orig_GraphicsDevice_SetRenderTarget(GraphicsDevice self, RenderTarget2D target);
    [SuppressMessage("Microsoft.CodeAnalysis", "IDE1006")]
    private static void on_GraphicsDevice_SetRenderTarget(orig_GraphicsDevice_SetRenderTarget orig, GraphicsDevice self, RenderTarget2D target) {
        // Redirect the backbuffer to our render target, to capture the frame.
        if (hijackBackBuffer && target is null) {
            orig(self, captureTarget);
            return;
        }

        orig(self, target);
    }
}
