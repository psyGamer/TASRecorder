using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using MonoMod.RuntimeDetour;

namespace Celeste.Mod.Capture;

public static class VideoCapture {

    internal static Hook hook_Game_Tick;
    internal static void Load() {
        hook_Game_Tick = new Hook(
            typeof(Game).GetMethod("Tick"),
            on_Game_Tick
        );
    }
    internal static void Unload() {
        hook_Game_Tick.Dispose();
    }

    private unsafe static void captureFrame() {
        var device = Celeste.Instance.GraphicsDevice;

        int width = device.Viewport.Width;
        int height = device.Viewport.Height;

        Color[] buffer = new Color[width * height];
        device.GetBackBufferData(device.Viewport.Bounds, buffer, 0, buffer.Length);

        if (!CaptureModule.Recording) return;

        CaptureModule.Encoder.PrepareVideo(width, height);
        fixed (Color* srcData = buffer) {
            int srcRowStride = width * sizeof(Color);
            int dstRowStride = CaptureModule.Encoder.VideoRowStride;

            byte* src = (byte*)srcData;
            byte* dst = CaptureModule.Encoder.VideoData;

            for (int i = 0; i < height; i++) {
                NativeMemory.Clear(dst, (nuint)dstRowStride);
                NativeMemory.Copy(src, dst, (nuint)srcRowStride);
                src += srcRowStride;
                dst += dstRowStride;
            }
        }
        CaptureModule.Encoder.FinishVideo();
    }

    // We need to use a modified version of the main game loop to avoid skipping frames
    private delegate void orig_Game_Tick(Game self);
    private static void on_Game_Tick(orig_Game_Tick orig, Game self) {
        if (!CaptureModule.Recording) {
            orig(self);
            return;
        }

        Syncing.SyncWithAudio();

        FNAPlatform.PollEvents(self, ref self.currentAdapter, self.textInputControlDown, ref self.textInputSuppress);

        self.gameTime.ElapsedGameTime = self.TargetElapsedTime;
        self.gameTime.TotalGameTime = self.TargetElapsedTime;

        self.Update(self.gameTime);
        if (self.BeginDraw()) {
            self.Draw(self.gameTime);
            self.EndDraw();

            captureFrame();
        }
    }
}