using System;
using System.IO;

namespace Celeste.Mod.TASRecorder;

public abstract class Encoder {
    // Celeste only works well with this value
    public const int AUDIO_SAMPLE_RATE = 48000;

    public readonly string FilePath;

    public unsafe byte* VideoData;
    public int VideoRowStride;

    public unsafe byte* AudioData;

    public bool HasVideo { get; protected set; }
    public bool HasAudio { get; protected set; }

    protected Encoder(string? fileName = null) {
        string name = (fileName ?? $"{DateTime.Now:dd-MM-yyyy_HH-mm-ss}") + $".{TASRecorderModule.Settings.ContainerType}";
        FilePath = $"{TASRecorderModule.Settings.OutputDirectory}/{name}";

        if (!Directory.Exists(TASRecorderModule.Settings.OutputDirectory)) {
            Directory.CreateDirectory(TASRecorderModule.Settings.OutputDirectory);
        }
    }

    public abstract void End();

    public abstract void PrepareVideo(int width, int height);
    public abstract void PrepareAudio(uint channelCount, uint sampleCount);

    public abstract void FinishVideo();
    public abstract void FinishAudio();
}
