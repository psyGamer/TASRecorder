namespace Celeste.Mod.Capture;

internal static class Syncing {

    private static bool videoDone = false;
    private static bool audioDone = false;

    // Required to avoid race conditions
    private static bool videoContinued = false;
    private static bool audioContinued = false;

    // Spin lock untin the next frame
    public static void SyncWithAudio() {
        if (!CaptureModule.Encoder.HasAudio) return;

        videoDone = true;
        while(CaptureModule.Recording && !audioDone) {}

        videoContinued = true;
        while (CaptureModule.Recording && !audioContinued) {}
        audioContinued = false;
        videoDone = false;
    }

    public static void SyncWithVideo() {
        if (!CaptureModule.Encoder.HasVideo) return;

        audioDone = true;
        while(CaptureModule.Recording && !videoDone) {}

        audioContinued = true;
        while (CaptureModule.Recording && !videoContinued) {}
        videoContinued = false;
        audioDone = false;
    }
}
