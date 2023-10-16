namespace Celeste.Mod.TASRecorder;

internal static class Syncing {

    private static bool videoDone = false;
    private static bool audioDone = false;

    // Required to avoid race conditions
    private static bool videoContinued = false;
    private static bool audioContinued = false;

    // Spin lock until the next frame
    public static void SyncWithAudio() {
        if (!TASRecorderModule.Encoder?.HasAudio ?? false) return;

        videoDone = true;
        while (TASRecorderModule.Recording && !audioDone) { }

        videoContinued = true;
        while (TASRecorderModule.Recording && !audioContinued) { }
        audioContinued = false;
        videoDone = false;
    }

    public static void SyncWithVideo() {
        if (!TASRecorderModule.Encoder?.HasVideo ?? false) return;

        audioDone = true;
        while (TASRecorderModule.Recording && !videoDone) { }

        audioContinued = true;
        while (TASRecorderModule.Recording && !videoContinued) { }
        videoContinued = false;
        audioDone = false;
    }
}
