using Celeste.Mod.TASRecorder.Util;

namespace Celeste.Mod.TASRecorder;

internal static class RecordingManager {

    private static Encoder _encoder = null;
    private static bool _recording = false;

    public static Encoder Encoder => _encoder;
    public static bool Recording => _recording;

    public static bool RecordingVideo => _recording && _encoder!.HasVideo;
    public static bool RecordingAudio => _recording && _encoder!.HasAudio;

    public const int NoEstimate = -1;
    public static int CurrentFrameCount = 0;
    public static int DurationEstimate = NoEstimate;

    public static void StartRecording(string fileName = null) {
        _encoder = new Encoder(fileName);
        _recording = true;

        if (!Encoder.HasVideo && !Encoder.HasAudio) {
            Log.Warn("Encoder has neither video nor audio! Aborting recording");
            _recording = false;
            _encoder = null;
            return;
        }

        CurrentFrameCount = 0;
        DurationEstimate = TASRecorderAPI.NoEstimate;

        RecordingRenderer.Start();
        TASRecorderMenu.OnStateChanged();

        if (Encoder.HasAudio) AudioCapture.StartRecording();

        Log.Info($"Started recording! Saving to {Encoder.FilePath}");
    }

    public static void StopRecording() {
        if (!Recording) return;
        _recording = false;

        if (Encoder.HasAudio) AudioCapture.StopRecording();

        _encoder.End();
        _encoder = null;

        // Can't use properties directly, since they have a recording check and it already stopped
        TASRecorderModule.Settings._speed = 1.0f;
        if (TASRecorderModule.Settings.resetSfxMuteState) {
            Audio.BusMuted(Buses.GAMEPLAY, false);
            Audio.BusMuted(Buses.UI, false);
            Audio.BusMuted(Buses.STINGS, false);
            TASRecorderModule.Settings.resetSfxMuteState = false;
        }

        TASRecorderMenu.OnStateChanged();

        Log.Info("Stopped recording!");
    }
}
