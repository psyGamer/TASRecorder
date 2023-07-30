using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FFmpeg;
using YamlDotNet.Serialization;

namespace Celeste.Mod.TASRecorder;

public class TASRecorderModuleSettings : EverestModuleSettings {
    public int FPS { get; set; } = 60;

    public int VideoResolution { get; set; } = 5;
    [YamlIgnore]
    public int VideoWidth => TASRecorderMenu.RESOLUTIONS[VideoResolution].Item1;
    [YamlIgnore]
    public int VideoHeight => TASRecorderMenu.RESOLUTIONS[VideoResolution].Item2;

    public int VideoBitrate { get; set; } = 6500000;
    public int AudioBitrate { get; set; } = 128000;

    public int VideoCodecOverwrite { get; set; } = -1;
    public int AudioCodecOverwrite { get; set; } = -1;

    public string H264Preset { get; set; } = "faster";

    public string ContainerType { get; set; } = "mp4";

    public bool RecordingIndicator { get; set; } = true;
    public RecordingTimeIndicator RecordingTime { get; set; } = RecordingTimeIndicator.RegularFrames;
    public bool RecordingProgrees { get; set; } = true;
}

public enum RecordingTimeIndicator {
    NoTime, Regular, RegularFrames
}
