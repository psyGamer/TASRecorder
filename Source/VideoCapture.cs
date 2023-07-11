using System;
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

    private static void captureFrame() {
        var device = Celeste.Instance.GraphicsDevice;

        int w = device.Viewport.Width;
        int h = device.Viewport.Height;

        Color[] buffer = new Color[w * h];
        device.GetBackBufferData(device.Viewport.Bounds, buffer, 0, buffer.Length);
    }

    // We need to use a modified version of the main game loop to avoid skipping frames
    private delegate void orig_Game_Tick(Game self);
    private static void on_Game_Tick(orig_Game_Tick orig, Game self) {
        if (!CaptureModule.Recording) {
            orig(self);
            return;
        }

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