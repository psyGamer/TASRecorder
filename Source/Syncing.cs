namespace Celeste.Mod.Capture;

internal static class Syncing {

    private static bool videoDone = false;
    private static bool audioDone = false;

    // Required to avoid race conditions
    private static bool videoContinued = false;
    private static bool audioContinued = false;

    // Only really useful for debugging
    private static bool disableVideoCapture = false;
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
}