using System;
using System.Threading;
using Microsoft.Xna.Framework;
using MonoMod;
using MonoMod.RuntimeDetour;

namespace Celeste.Mod.Capture;

/* Ensures everything stays in sync.
 * Audio capture is usually
 */
public static class Timings {

    private static Hook hook_Game_Tick;

    public static void Load() {
        hook_Game_Tick = new Hook(
            typeof(Game).GetMethod("Tick"),
            on_Game_Tick
        );
    }

    public static void Unload() {
        hook_Game_Tick.Dispose();
    }

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
        }
    }
}