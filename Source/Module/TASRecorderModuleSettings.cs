using Celeste.Mod.TASRecorder.Util;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;

namespace Celeste.Mod.TASRecorder;

public class TASRecorderModuleSettings : EverestModuleSettings {
    private const string UnknownVersion = "unknown";
    public string Version { get; set; } = UnknownVersion;

    private int _fps = 60;
    public int FPS {
        get => _fps;
        set {
            if (RecordingManager.Recording && RecordingManager.Encoder is not FFmpegLibraryEncoder) {
                Log.Warn("Tried to change the FPS while recording without using the FFmpeg Library Encoder");
                return;
            }

            _fps = Math.Clamp(value, 1, 60);
            if (RecordingManager.Encoder is FFmpegLibraryEncoder ffmpeg) ffmpeg.RefreshSettings();
        }
    }

    // Only intended for TAS with: "Set, TASRecorder.Speed, 2.0"
    // Reset on recording stop and not supported while not recording
    internal float _speed = 1.0f;
    [YamlIgnore]
    public float Speed {
        get => _speed;
        set {
            if (IsCelesteTASRestoringSettings())
                return;
            if (!RecordingManager.Recording) {
                Log.Warn("Tried to \"Set, TASRecorder.Speed, ...\" while not recording ");
                return;
            }
            _speed = Math.Clamp(value, 0.1f, 30.0f);
            if (RecordingManager.Encoder is FFmpegLibraryEncoder ffmpeg) {
                ffmpeg.RefreshSettings();
            } else if (RecordingManager.Recording) {
                Log.Warn("Tried to change the Speed while recording without using the FFmpeg Library Encoder");
            }
        }
    }

    // Setting it to a positive value will start a black-fade which takes for "value" seconds to switch from transparent to fully opaque
    // A negative value will do the same, exact the other way around.
    [YamlIgnore]
    public float BlackFade {
        get => float.NaN;
        set {
            if (IsCelesteTASRestoringSettings())
                return;
            if (!RecordingManager.Recording) {
                Log.Warn("Tried to \"Set, TASRecorder.BlackFade, ...\" while not recording");
                return;
            }

            switch (Math.Sign(value)) {
                case 1:
                    VideoCapture.BlackFadeStart = 0.0f;
                    VideoCapture.BlackFadeEnd = Math.Abs(value);
                    break;
                case -1:
                    VideoCapture.BlackFadeStart = Math.Abs(value);
                    VideoCapture.BlackFadeEnd = 0.0f;
                    break;
            }
        }
    }

    [YamlIgnore]
    public string BlackFadeText {
        get => VideoCapture.BlackFadeText;
        set {
            if (IsCelesteTASRestoringSettings())
                return;
            if (!RecordingManager.Recording) {
                Log.Warn("Tried to \"Set, TASRecorder.BlackFadeText, ...\" while not recording");
                return;
            }
            VideoCapture.BlackFadeText = value;
        }
    }

    [YamlIgnore]
    public Vector2 BlackFadeTextPosition {
        get => VideoCapture.BlackFadeTextPosition;
        set {
            if (IsCelesteTASRestoringSettings())
                return;
            if (!RecordingManager.Recording) {
                Log.Warn("Tried to \"Set, TASRecorder.BlackFadeTextPosition, ...\" while not recording");
                return;
            }
            VideoCapture.BlackFadeTextPosition = value;
        }
    }

    [YamlIgnore]
    public float BlackFadeTextScale {
        get => VideoCapture.BlackFadeTextScale;
        set {
            if (IsCelesteTASRestoringSettings())
                return;
            if (!RecordingManager.Recording) {
                Log.Warn("Tried to \"Set, TASRecorder.BlackFadeTextScale, ...\" while not recording");
                return;
            }
            VideoCapture.BlackFadeTextScale = value;
        }
    }

    [YamlIgnore]
    public Color BlackFadeTextColor {
        get => VideoCapture.BlackFadeTextColor;
        set {
            if (IsCelesteTASRestoringSettings())
                return;
            if (!RecordingManager.Recording) {
                Log.Warn("Tried to \"Set, TASRecorder.BlackFadeTextColor, ...\" while not recording");
                return;
            }
            Log.Verbose($"Color: {value}");
            VideoCapture.BlackFadeTextColor = value;
        }
    }

    private int _videoResolution = 6;
    public int VideoResolution {
        get => _videoResolution;
        set {
            if (RecordingManager.Recording && RecordingManager.Encoder is not FFmpegLibraryEncoder) {
                Log.Warn("Tried to change the video resolution while recording without using the FFmpeg Library Encoder");
                return;
            }

            _videoResolution = Math.Clamp(value, 1, 6);
            if (RecordingManager.Encoder is FFmpegLibraryEncoder ffmpeg) ffmpeg.RefreshSettings();
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
            if (RecordingManager.Recording && RecordingManager.Encoder is not FFmpegLibraryEncoder) {
                Log.Warn("Tried to change the video bitrate while recording without using the FFmpeg Library Encoder");
                return;
            }

            _videoBitrate = Math.Max(value, 100000);
            if (RecordingManager.Encoder is FFmpegLibraryEncoder ffmpeg) ffmpeg.RefreshSettings();
        }
    }
    public int AudioBitrate {
        get => _audioBitrate;
        set {
            if (RecordingManager.Recording && RecordingManager.Encoder is not FFmpegLibraryEncoder) {
                Log.Warn("Tried to change the audio bitrate while recording without using the FFmpeg Library Encoder");
                return;
            }

            _audioBitrate = Math.Max(value, 8000);
            if (RecordingManager.Encoder is FFmpegLibraryEncoder ffmpeg) ffmpeg.RefreshSettings();
        }
    }

    public string OutputDirectory { get; set; } = Path.Combine(Everest.PathGame, "TAS-Recordings");

    public EncoderType EncoderType { get; set; } = EncoderType.FFmpegBinary;
    public HWAccelType HardwareAccelerationType { get; set; } = HWAccelType.None;

    public int VideoCodecOverwrite { get; set; } = -1;
    public int AudioCodecOverwrite { get; set; } = -1;

    public string H264Preset { get; set; } = "faster";
    public int H264Quality { get; set; } = 21;

    public string FFmpegBinaryEncoderOutputOverwrite { get; set; } = "";

    public string ContainerType { get; set; } = "mp4";

    public bool RecordingIndicator { get; set; } = true;
    public RecordingTimeIndicator RecordingTime { get; set; } = RecordingTimeIndicator.RegularFrames;
    public bool RecordingProgress { get; set; } = true;

    // Avoid potentially unmuting something we shouldn't, when not using this feature
    internal bool resetSfxMuteState = false;
    // Intended for TAS, since sped up gameplay can be loud
    // It needs to be in the settings to be able to do: Set, TASRecorder.MuteSFX, true
    // Reset on recording stop and not supported while not recording
    [YamlIgnore]
    public bool MuteSFX {
        get => Audio.BusMuted(Buses.GAMEPLAY, null);
        set {
            if (IsCelesteTASRestoringSettings())
                return;
            if (!RecordingManager.Recording) {
                Log.Warn("Tried to \"Set, TASRecorder.MuteSFX, ...\" while not recording");
                return;
            }
            Audio.BusMuted(Buses.GAMEPLAY, value);
            Audio.BusMuted(Buses.UI, value);
            Audio.BusMuted(Buses.STINGS, value); // What is stings? It seems to be only used in Audio.PauseGameplaySfx...
            resetSfxMuteState = true;
        }
    }

    // Small hack to avoid warnings when CelesteTAS tries to restore the settings
    private static bool IsCelesteTASRestoringSettings() => Environment.StackTrace.Contains("TAS.EverestInterop.RestoreSettings.TryRestore()");

    public bool IsAtLeastVersion(int major, int minor, int patch) {
        string prevVersion = Version;

        // Update version in settings
        var version = TASRecorderModule.Instance.Metadata.Version;
        Version = $"{version.Major}.{version.Minor}.{version.Build}";
        TASRecorderModule.Instance.SaveSettings();

        if (prevVersion == UnknownVersion) return false;
        string[] parts = prevVersion.Split(".").ToArray();

        if (parts.Length != 3) {
            Log.Error($"Invalid version: {prevVersion}");
            return false;
        }

        int currMajor = int.Parse(parts[0]);
        int currMinor = int.Parse(parts[1]);
        int currPatch = int.Parse(parts[2]);

        return currMajor >= major && currMinor >= minor && currPatch >= patch;
    }
}

public enum RecordingTimeIndicator {
    NoTime, Regular, RegularFrames
}

public enum EncoderType {
    FFmpegLibrary, FFmpegBinary, Null
}
public enum HWAccelType {
    None, QSV, NVENC, AMF, VideoToolbox
}
