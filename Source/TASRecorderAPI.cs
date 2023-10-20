using System;
using Celeste.Mod.TASRecorder.Util;

namespace Celeste.Mod.TASRecorder;

/// <summary>
/// Stable API for interfacing with TAS Recorder.
/// </summary>
public static class TASRecorderAPI {

    /// <summary>
    /// Starts a recording.
    /// If <see cref="IsRecording"/> is true or <see cref="IsFFmpegInstalled"/> is false, this shouldn't be called.
    /// <param name="fileName">The file name of the recording. If null, it's generated from "dd-MM-yyyy_HH-mm-ss"</param>
    /// </summary>
    public static void StartRecording(string fileName = null) {
        if (!IsFFmpegInstalled()) {
            Log.Warn("Tried to start recording, without having FFmpeg installed");
            return;
        }
        if (IsRecording()) {
            Log.Warn("Tried to start recording, while recording");
            return;
        }

        try {
            TASRecorderModule.StartRecording(fileName);
        } catch (Exception ex) {
            Log.Error("Failed to start recording!");
            Log.Exception(ex);
        }
    }

    /// <summary>
    /// Stops a recording which was previously started.
    /// If <see cref="IsRecording"/> or <see cref="IsFFmpegInstalled"/> is false, this shouldn't be called.
    /// </summary>
    public static void StopRecording() {
        if (!IsFFmpegInstalled()) {
            Log.Warn("Tried to stop recording, without having FFmpeg installed");
            return;
        }
        if (!IsRecording()) {
            Log.Warn("Tried to stop recording, while not recording");
            return;
        }

        try {
            TASRecorderModule.StopRecording();
        } catch (Exception ex) {
            Log.Error("Failed to stop recording!");
            Log.Exception(ex);
        }
    }

    /// <summary>
    /// Indicates that there isn't a known end time.
    /// </summary>
    public static int NoEstimate => -1;

    /// <summary>
    /// Sets the estimated amount of total frames.
    /// This is used for the progress bar, but doesn't actually interact with the recording.
    /// If <see cref="IsRecording"/> or <see cref="IsFFmpegInstalled"/> is false, this shouldn't be called.
    /// <param name="frames">The total amount of frames, excluding loading times. If set to <see cref="NoEstimate"/>, there's isn't a progress bar.</param>
    /// </summary>
    public static void SetDurationEstimate(int frames) {
        if (!IsFFmpegInstalled()) {
            Log.Warn("Tried to set recording duration estimate, without having FFmpeg installed");
            return;
        }
        if (!IsRecording()) {
            Log.Warn("Tried to set recording duration estimate, while not recording");
            return;
        }

        TASRecorderModule.SetEstimate(frames);
    }

    /// <summary>
    /// Weather TAS Recorder is currently recording.
    /// </summary>
    public static bool IsRecording() => TASRecorderModule.Recording;

    /// <summary>
    /// Weather TAS Recorder is could properly load FFmpeg.
    /// </summary>
    public static bool IsFFmpegInstalled() => FFmpegLoader.Installed;
}
