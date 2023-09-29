using System;
// ReSharper disable MemberCanBePrivate.Global

namespace Celeste.Mod.TASRecorder.Interop;

public static class TASRecorderInterop {

    public static void StartRecording(string fileName = null) {
        if (IsRecording() || !IsFFmpegInstalled()) return;

        try {
            TASRecorderModule.StartRecording(-1, fileName);
        } catch (Exception) {
            // ignored
        }
    }
    public static void StopRecording() {
        if (!IsRecording() || !IsFFmpegInstalled()) return;

        try {
            TASRecorderModule.StopRecording();
        } catch (Exception) {
            // ignored
        }
    }

    public static void RecordFrames(int frames, string fileName = null) {
        if (IsRecording() || !IsFFmpegInstalled()) return;

        try {
            TASRecorderModule.StartRecording(frames, fileName);
        } catch (Exception) {
            // ignored
        }
    }

    public static bool IsRecording() => TASRecorderModule.Recording;
    public static bool IsFFmpegInstalled() => FFmpegLoader.Installed;
}
