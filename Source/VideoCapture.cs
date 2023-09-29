using Celeste.Mod.TASRecorder.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using MonoMod.RuntimeDetour;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Celeste.Mod.TASRecorder;

public static class VideoCapture {

    private static Hook hook_Game_Tick;
    private static Hook hook_Game_Update;
    private static Hook hook_GraphicsDevice_SetRenderTarget;

    internal static void Load() {
        hook_Game_Tick = new Hook(
            typeof(Game).GetMethod("Tick"),
            On_Game_Tick
        );
        hook_Game_Update = new Hook(
            typeof(Game).GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance),
            On_Game_Update
        );
        hook_GraphicsDevice_SetRenderTarget = new Hook(
            typeof(GraphicsDevice).GetMethod("SetRenderTarget", new[] { typeof(RenderTarget2D) }),
            On_GraphicsDevice_SetRenderTarget
        );
        On.Monocle.Engine.RenderCore += On_Engine_RenderCore;
    }
    internal static void Unload() {
        hook_Game_Tick?.Dispose();
        hook_Game_Update?.Dispose();
        hook_GraphicsDevice_SetRenderTarget?.Dispose();
        On.Monocle.Engine.RenderCore -= On_Engine_RenderCore;
    }

    internal static int CurrentFrameCount = 0;
    internal static int TargetFrameCount = -1;

    private static TimeSpan RecordingDeltaTime => TASRecorderModule.Settings.FPS switch {
        60 => TimeSpan.FromTicks(166667L),
        30 => TimeSpan.FromTicks(333334L),
        24 => TimeSpan.FromTicks(416667L),
        _ => TimeSpan.FromSeconds(1.0f / TASRecorderModule.Settings.FPS),
    } * TASRecorderModule.Settings.Speed;

    // Hijacks SetRenderTarget(null) calls to point to our captureTarget instead of the back buffer.
    private static bool hijackBackBuffer = false;
    private static RenderTarget2D captureTarget;
    private static bool tickHookActive = false;

    private static int oldWidth;
    private static int oldHeight;
    private static Matrix oldMatrix;
    private static Viewport oldViewport;
    private static bool updateHappened;

    private static unsafe void CaptureFrame() {
        Log.Debug("Starting frame capture");
        int width = captureTarget.Width;
        int height = captureTarget.Height;

        Color[] buffer = new Color[width * height];
        captureTarget.GetData(buffer);

        TASRecorderModule.Encoder.PrepareVideo(width, height);
        fixed (Color* srcData = buffer) {
            int srcRowStride = width * sizeof(Color);
            int dstRowStride = TASRecorderModule.Encoder.VideoRowStride;

            byte* src = (byte*) srcData;
            byte* dst = TASRecorderModule.Encoder.VideoData;

            for (int i = 0; i < height; i++) {
                NativeMemory.Clear(dst, (nuint) dstRowStride);
                NativeMemory.Copy(src, dst, (nuint) srcRowStride);
                src += srcRowStride;
                dst += dstRowStride;
            }
        }
        TASRecorderModule.Encoder.FinishVideo();
        Log.Debug("Successfully captured frame");
    }

    // Taken from Engine.UpdateView(), but without depending on presentation parameters
    private static void UpdateEngineView(int width, int height) {
        if (width / (float) Engine.Width > height / (float) Engine.Height) {
            Engine.ViewWidth = (int) (height / (float) Engine.Height * Engine.Width);
            Engine.ViewHeight = height;
        } else {
            Engine.ViewWidth = width;
            Engine.ViewHeight = (int) (width / (float) Engine.Width * Engine.Height);
        }
        float ratio = Engine.ViewHeight / (float) Engine.ViewWidth;
        Engine.ViewWidth -= Engine.ViewPadding * 2;
        Engine.ViewHeight -= (int) (ratio * Engine.ViewPadding * 2f);
        Engine.ScreenMatrix = Matrix.CreateScale(Engine.ViewWidth / (float) Engine.Width);
        Viewport viewport = default;
        viewport.X = (int) (width / 2f - Engine.ViewWidth / 2);
        viewport.Y = (int) (height / 2f - Engine.ViewHeight / 2);
        viewport.Width = Engine.ViewWidth;
        viewport.Height = Engine.ViewHeight;
        viewport.MinDepth = 0f;
        viewport.MaxDepth = 1f;
        Engine.Viewport = viewport;
    }

    // We need to use a modified version of the main game loop to avoid skipping frames
    private delegate void orig_Game_Tick(Game self);
    private static void On_Game_Tick(orig_Game_Tick orig, Game self) {
        if (!TASRecorderModule.Recording || !TASRecorderModule.Encoder.HasVideo) {
            updateHappened = false;
            orig(self);

            // We started recording on this frame.
            if (TASRecorderModule.Recording) {
                // The first half of this is inside on_Game_Update
                hijackBackBuffer = false;
                Engine.ViewWidth = oldWidth;
                Engine.ViewHeight = oldHeight;
                Engine.ScreenMatrix = oldMatrix;
                Engine.Viewport = oldViewport;

                CaptureFrame();

                // Don't rerender to the screen or display the indicator, because drawing already ended.
            }

            return;
        }

        if (TargetFrameCount != -1 && CurrentFrameCount >= TargetFrameCount) {
            TASRecorderModule.StopRecording();
            orig(self);
            return;
        }

        Syncing.SyncWithAudio();

        FNAPlatform.PollEvents(self, ref self.currentAdapter, self.textInputControlDown, ref self.textInputSuppress);

        self.accumulatedElapsedTime += RecordingDeltaTime;

        if (self.accumulatedElapsedTime < self.TargetElapsedTime) return;

        var device = Celeste.Instance.GraphicsDevice;
        if (captureTarget == null || captureTarget.Width != TASRecorderModule.Settings.VideoWidth || captureTarget.Height != TASRecorderModule.Settings.VideoHeight) {
            captureTarget?.Dispose();
            captureTarget = new RenderTarget2D(device, TASRecorderModule.Settings.VideoWidth, TASRecorderModule.Settings.VideoHeight, mipMap: false, device.PresentationParameters.BackBufferFormat, DepthFormat.None);
        }

        self.gameTime.ElapsedGameTime = self.TargetElapsedTime;
        self.gameTime.TotalGameTime = self.TargetElapsedTime;

        while (self.accumulatedElapsedTime >= self.TargetElapsedTime) {
            self.accumulatedElapsedTime -= self.TargetElapsedTime;

            // Avoid recording frames twice
            tickHookActive = true;
            self.Update(self.gameTime);
            tickHookActive = false;
            RecordingRenderer.Update();

            CurrentFrameCount++;
        }

        if (self.BeginDraw()) {
            oldWidth = Engine.ViewWidth;
            oldHeight = Engine.ViewHeight;
            oldMatrix = Engine.ScreenMatrix;
            oldViewport = Engine.Viewport;
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
            var matrix = Matrix.CreateScale(Engine.ViewWidth / (float) captureTarget.Width);

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

    // After this update, Render would be called from orig_Tick. That means we would miss the first frame.
    // If the game is currently lagging, more than 1 frame could be skipped.
    private delegate void orig_Game_Update(Game self, GameTime gameTime);
    private static void On_Game_Update(orig_Game_Update orig, Game self, GameTime gameTime) {
        if (updateHappened && !tickHookActive && TASRecorderModule.Recording) {
            // We are currently lagging. Don't update to avoid skipping frames.
            return;
        }

        orig(self, gameTime);

        // Maybe the recording started outside of an update.
        // This ensures we get at least 1 update
        updateHappened = true;

        // For some reason, when recording with CelesteTAS, the first frame is missing the level
        // when recording from here, but not when recording from the original Tick
        if (!tickHookActive && TASRecorderModule.Recording) {
            var device = Celeste.Instance.GraphicsDevice;
            if (captureTarget == null || captureTarget.Width != TASRecorderModule.Settings.VideoWidth || captureTarget.Height != TASRecorderModule.Settings.VideoHeight) {
                captureTarget?.Dispose();
                captureTarget = new RenderTarget2D(device, TASRecorderModule.Settings.VideoWidth, TASRecorderModule.Settings.VideoHeight, mipMap: false, device.PresentationParameters.BackBufferFormat, DepthFormat.None);
            }

            oldWidth = Engine.ViewWidth;
            oldHeight = Engine.ViewHeight;
            oldMatrix = Engine.ScreenMatrix;
            oldViewport = Engine.Viewport;
            UpdateEngineView(captureTarget.Width, captureTarget.Height);
            hijackBackBuffer = true;
        }
    }

    private static void On_Engine_RenderCore(On.Monocle.Engine.orig_RenderCore orig, Engine self) {
        orig(self);

        // Render the banner fadeout after the FNA main loop hook is disabled
        if (RecordingRenderer.ShouldUpdate() && (!TASRecorderModule.Recording || !TASRecorderModule.Encoder.HasVideo)) {
            RecordingRenderer.Update();
            RecordingRenderer.Render();
        }
    }

    private delegate void orig_GraphicsDevice_SetRenderTarget(GraphicsDevice self, RenderTarget2D target);
    private static void On_GraphicsDevice_SetRenderTarget(orig_GraphicsDevice_SetRenderTarget orig, GraphicsDevice self, RenderTarget2D target) {
        // Redirect the backbuffer to our render target, to capture the frame.
        if (hijackBackBuffer && target is null) {
            orig(self, captureTarget);
            return;
        }

        orig(self, target);
    }
}
