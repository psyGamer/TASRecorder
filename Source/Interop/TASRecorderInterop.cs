using System;
// ReSharper disable MemberCanBePrivate.Global

namespace Celeste.Mod.TASRecorder.Interop;

[Obsolete("Use TASRecorderAPI instead")]
public static class TASRecorderInterop {

    public static void StartRecording(string fileName = null) => TASRecorderAPI.StartRecording(fileName);
    public static void StopRecording() => TASRecorderAPI.StopRecording();

    public static void RecordFrames(int frames, string fileName = null) {
        TASRecorderAPI.StartRecording(fileName);
        TASRecorderAPI.SetDurationEstimate(frames);
    }

    public static bool IsRecording() => TASRecorderAPI.IsRecording();
    public static bool IsFFmpegInstalled() => TASRecorderAPI.IsFFmpegInstalled();
}
