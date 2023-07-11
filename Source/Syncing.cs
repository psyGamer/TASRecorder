using System;
using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour;

namespace Celeste.Mod.Capture;

internal static class Syncing {

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

    private static bool videoDone = false;
    private static bool audioDone = false;

    // Required to avoid race conditions
    private static bool videoContinued = false;
    private static bool audioContinued = false;

    // Only really useful for debugging
    private static bool disableVideoCapture = true;
    private static bool disableAudioCapture = false;

    // Spin lock untin the next frame
    public static void SyncWithAudio() {
        if (disableAudioCapture) return;

        videoDone = true;
        while(CaptureModule.Recording && !audioDone) {}

        videoContinued = true;
        while (CaptureModule.Recording && !audioContinued) {}
        audioContinued = false;
        videoDone = false;
    }
    public static void SyncWithVideo() {
        if (disableVideoCapture) return;

        audioDone = true;
        while(CaptureModule.Recording && !videoDone) {}

        audioContinued = true;
        while (CaptureModule.Recording && !videoContinued) {}
        videoContinued = false;
        audioDone = false;
    }

    // We need to use a modified version of the main game loop to avoid skipping frames
    internal delegate void orig_Game_Tick(Game self);
    internal static void on_Game_Tick(orig_Game_Tick orig, Game self) {
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