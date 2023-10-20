namespace Celeste.Mod.TASRecorder;

internal static class Syncing {

    private static bool videoDone = false;
    private static bool audioDone = false;

    // Required to avoid race conditions
    private static bool videoContinued = false;
    private static bool audioContinued = false;

    // Spin lock until the next frame
    public static void SyncWithAudio() {
        if (!RecordingManager.RecordingAudio) return;

        videoDone = true;
        while (RecordingManager.Recording && !audioDone) { }

        videoContinued = true;
        while (RecordingManager.Recording && !audioContinued) { }
        audioContinued = false;
        videoDone = false;
    }

    public static void SyncWithVideo() {
        if (!RecordingManager.RecordingVideo) return;

        audioDone = true;
        while (RecordingManager.Recording && !videoDone) { }

        audioContinued = true;
        while (RecordingManager.Recording && !videoContinued) { }
        videoContinued = false;
        audioDone = false;
    }
}
