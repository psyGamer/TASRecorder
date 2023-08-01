using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FFmpeg;
using YamlDotNet.Serialization;

namespace Celeste.Mod.TASRecorder;

public class TASRecorderModuleSettings : EverestModuleSettings {
    private float _fps = 60;
    public float FPS {
        get => _fps;
        set {
            _fps = value;
            TASRecorderModule.Encoder?.RefreshSettings();
        }
    }

    // Only intended for TAS with: Set, TASRecorder.Speed, 2.0
    // Reset on recording stop and not supported while not recording
    internal float _speed = 1.0f;
    public float Speed {
        get => _speed;
        set {
            if (!TASRecorderModule.Recording) return;
            _speed = value;
            TASRecorderModule.Encoder?.RefreshSettings();
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

    // Avoid potentially unmuting something we shouldn't, when not using this feature
    internal bool resetSfxMuteState = false;
    // Intended for TAS, since sped up gameplay can be loud
    // It needs to be in the settings to be able to do: Set, TASRecorder.MuteSFX, true
    // Reset on recording stop and not supported while not recording
    [YamlIgnore]
    public bool MuteSFX {
        get => Audio.BusMuted(Buses.GAMEPLAY, null);
        set {
            if (!TASRecorderModule.Recording) return;
            Audio.BusMuted(Buses.GAMEPLAY, value);
            Audio.BusMuted(Buses.UI, value);
            Audio.BusMuted(Buses.STINGS, value); // What is stings? It seems to be only used in Audio.PauseGameplaySfx...
            resetSfxMuteState = true;
        }
    }
}

public enum RecordingTimeIndicator {
    NoTime, Regular, RegularFrames
}
