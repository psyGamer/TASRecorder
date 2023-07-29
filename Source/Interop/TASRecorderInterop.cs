using System;

namespace Celeste.Mod.TASRecorder.Interop;

public static class TASRecorderInterop {

    public static void StartRecording(string fileName = null) {
        if (IsRecording()) return;

        try {
            TASRecorderModule.StartRecording(-1, fileName);
        } catch (Exception) { }
    }
    public static void StopRecording() {
        if (!IsRecording()) return;

        try {
            TASRecorderModule.StopRecording();
        } catch (Exception) { }
    }

    public static void RecordFrames(int frames, string fileName = null) {
        try {
            TASRecorderModule.StartRecording(frames, fileName);
        } catch (Exception) { }
    }

    public static bool IsRecording() {
        return TASRecorderModule.Recording;
    }

    public static bool IsFFmpegInstalled() {
        try {
            _ = FFmpeg.DynamicallyLinkedBindings.avutil_version();
            _ = FFmpeg.DynamicallyLinkedBindings.avformat_version();
            _ = FFmpeg.DynamicallyLinkedBindings.avcodec_version();
            _ = FFmpeg.DynamicallyLinkedBindings.swresample_version();
            _ = FFmpeg.DynamicallyLinkedBindings.swscale_version();

            return true;
        } catch (Exception) {
            return false;
        }
    }
}
