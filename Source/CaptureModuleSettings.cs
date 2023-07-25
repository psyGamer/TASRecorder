using System;
using Microsoft.Xna.Framework;
using FFmpeg;
using YamlDotNet.Serialization;

namespace Celeste.Mod.Capture;

public class CaptureModuleSettings : EverestModuleSettings {

    private static readonly int[] FRAME_RATES = { 24, 30, 60 };
    private static readonly int[] VIDEO_BITRATES = CreateIntRange(1000000, 10000000, 500000);
    private static readonly int[] AUDIO_BITRATES = CreateIntRange(32000, 512000, 16000);
    private static readonly (int, int)[] RESOLUTIONS = {
        (320 * 1, 180 * 1), // 320x180
        (320 * 2, 180 * 2), // 640x360
        (320 * 3, 180 * 3), // 960x540
        (320 * 4, 180 * 4), // 1280x720
        (320 * 5, 180 * 5), // 1600x900
        (320 * 6, 180 * 6), // 1920x1080
    };
    private static readonly string[] CONTAINER_TYPES = { "mp4", "mkv", "mov", "webm" };
    private static readonly int[] VIDEO_CODECS = {
        -1,
        (int)AVCodecID.AV_CODEC_ID_NONE,
        (int)AVCodecID.AV_CODEC_ID_H264,
        (int)AVCodecID.AV_CODEC_ID_AV1,
        (int)AVCodecID.AV_CODEC_ID_VP9,
        (int)AVCodecID.AV_CODEC_ID_VP8,
    };
    private static readonly int[] AUDIO_CODECS = {
        -1,
        (int)AVCodecID.AV_CODEC_ID_NONE,
        (int)AVCodecID.AV_CODEC_ID_AAC,
        (int)AVCodecID.AV_CODEC_ID_MP3,
        (int)AVCodecID.AV_CODEC_ID_OPUS,
        (int)AVCodecID.AV_CODEC_ID_VORBIS,
    };
    private static readonly string[] H264_PRESETS = {
        "veryslow", "slower", "slow", "medium", "fast", "faster", "veryfast", "superfast", "ultrafast"
    };

    public int FPS { get; set; } = 60;

    public int VideoResolution = 5;
    [YamlIgnore]
    public int VideoWidth => RESOLUTIONS[VideoResolution].Item1;
    [YamlIgnore]
    public int VideoHeight => RESOLUTIONS[VideoResolution].Item2;

    public int VideoBitrate { get; set; } = 6500000;
    public int AudioBitrate { get; set; } = 128000;

    public int VideoCodecOverwrite { get; set; } = -1;
    public int AudioCodecOverwrite { get; set; } = -1;

    public string H264Preset { get; set; } = "faster";

    public string ContainerType { get; set; } = "mp4";

    private TextMenu.Item _h264Preset;

    internal void CreateSettingsMenu(TextMenu menu) {
        menu.Add(new TextMenu.Slider("FPS".GetDialogText(), i => $"{FRAME_RATES[i]}", 0, FRAME_RATES.Length - 1, Array.IndexOf(FRAME_RATES, FPS))
                 .Change(i => FPS = FRAME_RATES[i]));
        menu.Add(new TextMenu.Slider("RESOLUTION".GetDialogText(), i => $"{RESOLUTIONS[i].Item1}x{RESOLUTIONS[i].Item2}", 0, RESOLUTIONS.Length - 1, VideoResolution)
                 .Change(i => VideoResolution = i));

        menu.AddWithDescription(new TextMenu.Slider("VIDEO_BITRATE".GetDialogText(), i => $"{VIDEO_BITRATES[i] / 1000} kb/s", 0, VIDEO_BITRATES.Length - 1, Array.IndexOf(VIDEO_BITRATES, VideoBitrate))
                 .Change(i => VideoBitrate = VIDEO_BITRATES[i]), "VIDEO_BITRATE_DESC".GetDialogText());
        menu.AddWithDescription(new TextMenu.Slider("AUDIO_BITRATE".GetDialogText(), i => $"{AUDIO_BITRATES[i] / 1000} kb/s", 0, AUDIO_BITRATES.Length - 1, Array.IndexOf(AUDIO_BITRATES, AudioBitrate))
                 .Change(i => AudioBitrate = AUDIO_BITRATES[i]), "AUDIO_BITRATE_DESC".GetDialogText());

        menu.Add(new TextMenu.Slider("CONTAINER_TYPE".GetDialogText(), i => $"CONTAINER_TYPE_{CONTAINER_TYPES[i]}".GetDialogText(), 0, CONTAINER_TYPES.Length - 1, Array.IndexOf(CONTAINER_TYPES, ContainerType))
                 .Change(i => {
                    ContainerType = CONTAINER_TYPES[i];
                    _h264Preset.Disabled = !(VideoCodecOverwrite == (int)AVCodecID.AV_CODEC_ID_H264 || VideoCodecOverwrite == -1 && ContainerType is "mp4" or "mkv" or "mov");
                }));

        menu.AddWithDescription(new TextMenu.Slider("VIDEO_CODEC_OVERWRITE".GetDialogText(), i => $"VIDEO_CODEC_OVERWRITE_{VIDEO_CODECS[i]}".Replace("-1", "default").GetDialogText(), 0, VIDEO_CODECS.Length - 1, Array.IndexOf(VIDEO_CODECS, VideoCodecOverwrite))
                 .Change(i => {
                    VideoCodecOverwrite = VIDEO_CODECS[i];
                    _h264Preset.Disabled = !(VideoCodecOverwrite == (int)AVCodecID.AV_CODEC_ID_H264 || VideoCodecOverwrite == -1 && ContainerType is "mp4" or "mkv" or "mov");
                 }), "VIDEO_CODEC_OVERWRITE_DESC".GetDialogText(), Color.Orange);
        menu.AddWithDescription(new TextMenu.Slider("AUDIO_CODEC_OVERWRITE".GetDialogText(), i => $"AUDIO_CODEC_OVERWRITE_{AUDIO_CODECS[i]}".Replace("-1", "default").GetDialogText(), 0, AUDIO_CODECS.Length - 1, Array.IndexOf(AUDIO_CODECS, AudioCodecOverwrite))
                 .Change(i => AudioCodecOverwrite = AUDIO_CODECS[i]), "AUDIO_CODEC_OVERWRITE_DESC".GetDialogText(), Color.Orange);

        _h264Preset = new TextMenu.Slider("H264_PRESET".GetDialogText(), i => $"H264_PRESET_{H264_PRESETS[i]}".GetDialogText(), 0, H264_PRESETS.Length - 1, Array.IndexOf(H264_PRESETS, H264Preset))
                  .Change(i => H264Preset = H264_PRESETS[i]);
        _h264Preset.Disabled = !(VideoCodecOverwrite == (int)AVCodecID.AV_CODEC_ID_H264 || VideoCodecOverwrite == -1 && ContainerType is "mp4" or "mkv" or "mov");
        menu.AddWithDescription(_h264Preset, "H264_PRESET_DESC".GetDialogText());
    }

    private static int[] CreateIntRange(int min, int max, int step) {
        int length = (int) Math.Ceiling((max - min) / (float)step) + 1;
        int[] values = new int[length];

        for (int i = 0, val = min; i < length; i++, val += step) {
            values[i] = val;
        }

        return values;
    }
}

internal static class Extensions {
    public static void AddWithDescription(this TextMenu menu, TextMenu.Item menuItem, string description, Color? color = null) {
        TextMenuExt.EaseInSubHeaderExt descriptionText = new(description, false, menu) {
            TextColor = color ?? Color.Gray,
            HeightExtra = 0f
        };

        menu.Add(menuItem);
        menu.Add(descriptionText);

        menuItem.OnEnter += () => descriptionText.FadeVisible = true;
        menuItem.OnLeave += () => descriptionText.FadeVisible = false;
    }

    public static string GetDialogText(this string text) => Dialog.Clean($"TAS_RECORDER_{text}");
}
