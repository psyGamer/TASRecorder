using System;
using System.IO;

namespace Celeste.Mod.TASRecorder;

public abstract class Encoder {
    // Celeste only works well with this value
    public const int AUDIO_SAMPLE_RATE = 48000;

    public readonly string FilePath;

    public unsafe byte* VideoData;
    public unsafe byte* AudioData;

    public int VideoRowStride;

    public bool HasVideo { get; protected init; }
    public bool HasAudio { get; protected init; }

    protected unsafe Encoder(string? fileName = null) {
        string name = (fileName ?? $"{DateTime.Now:dd-MM-yyyy_HH-mm-ss}") + $".{TASRecorderModule.Settings.ContainerType}";
        FilePath = $"{TASRecorderModule.Settings.OutputDirectory}/{name}";

        if (!Directory.Exists(TASRecorderModule.Settings.OutputDirectory)) {
            Directory.CreateDirectory(TASRecorderModule.Settings.OutputDirectory);
        }

        VideoData = null;
        VideoRowStride = 0;

        AudioData = null;
    }

    public abstract void End();

    public abstract void PrepareVideo(int width, int height);
    public abstract void PrepareAudio(uint channelCount, uint sampleCount);

    public abstract void FinishVideo();
    public abstract void FinishAudio();
}
