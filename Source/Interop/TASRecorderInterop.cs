using System;
using static FFmpeg.FFmpeg;

namespace Celeste.Mod.TASRecorder.Interop;

public static class TASRecorderInterop {

    public static void StartRecording(string fileName = null) {
        if (IsRecording() || !IsFFmpegInstalled()) return;

        try {
            TASRecorderModule.StartRecording(-1, fileName);
        } catch (Exception) { }
    }
    public static void StopRecording() {
        if (!IsRecording() || !IsFFmpegInstalled()) return;

        try {
            TASRecorderModule.StopRecording();
        } catch (Exception) { }
    }

    public static void RecordFrames(int frames, string fileName = null) {
        if (IsRecording() || !IsFFmpegInstalled()) return;

        try {
            TASRecorderModule.StartRecording(frames, fileName);
        } catch (Exception) { }
    }

    public static bool IsRecording() {
        return TASRecorderModule.Recording;
    }

    public static bool IsFFmpegInstalled() {
        try {
            _ = avutil_version();
            _ = avformat_version();
            _ = avcodec_version();
            _ = swresample_version();
            _ = swscale_version();

            return true;
        } catch (Exception) {
            return false;
        }
    }
}
