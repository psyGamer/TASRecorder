using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FFmpeg;
using YamlDotNet.Serialization;

namespace Celeste.Mod.TASRecorder;

public class TASRecorderModuleSettings : EverestModuleSettings {
    private int _fps = 60;
    public int FPS {
        get => _fps;
        set {
            if (TASRecorderModule.Recording) return;
            _fps = value;
        }
    }

    private int _videoResolution = 6;
    public int VideoResolution {
        get => _videoResolution;
        set {
            if (TASRecorderModule.Recording) return;
            _videoResolution = value;
        }
    }
    [YamlIgnore]
    public int VideoWidth => Celeste.GameWidth * VideoResolution;
    [YamlIgnore]
    public int VideoHeight => Celeste.GameHeight * VideoResolution;

     private int _videoBitrate = 6500000;
    private int _audioBitrate = 128000;
    public int VideoBitrate {
        get => _videoBitrate;
        set {
            if (TASRecorderModule.Recording) return;
            _videoBitrate = value;
        }
    }
    public int AudioBitrate {
        get => _audioBitrate;
        set {
            if (TASRecorderModule.Recording) return;
            _audioBitrate = value;
        }
    }

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
