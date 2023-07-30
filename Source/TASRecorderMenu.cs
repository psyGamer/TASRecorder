using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FFmpeg;

namespace Celeste.Mod.TASRecorder;

public static class TASRecorderMenu {
    private static readonly int[] FRAME_RATES = { 24, 30, 60 };
    private static readonly int[] VIDEO_BITRATES = CreateIntRange(1000000, 10000000, 500000);
    private static readonly int[] AUDIO_BITRATES = CreateIntRange(32000, 512000, 16000);
    internal static readonly (int, int)[] RESOLUTIONS = {
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

    private static TASRecorderModuleSettings Settings => TASRecorderModule.Settings;

    private static readonly List<TextMenu.Item> captureSettings = new();
    private static TextMenu.Item _h264Preset;

    internal static void CreateSettingsMenu(TextMenu menu) {
        captureSettings.Clear();

        captureSettings.Add(new TextMenu.Slider("FPS".GetDialogText(), i => $"{FRAME_RATES[i]} FPS", 0, FRAME_RATES.Length - 1, Array.IndexOf(FRAME_RATES, Settings.FPS))
                 .Change(i => Settings.FPS = FRAME_RATES[i]));
        captureSettings.Add(new TextMenu.Slider("RESOLUTION".GetDialogText(), i => $"{RESOLUTIONS[i].Item1}x{RESOLUTIONS[i].Item2}", 0, RESOLUTIONS.Length - 1, Settings.VideoResolution)
                 .Change(i => Settings.VideoResolution = i));

        captureSettings.AddWithDescription(menu, new TextMenu.Slider("VIDEO_BITRATE".GetDialogText(), i => $"{VIDEO_BITRATES[i] / 1000} kb/s", 0, VIDEO_BITRATES.Length - 1, Array.IndexOf(VIDEO_BITRATES, Settings.VideoBitrate))
                 .Change(i => Settings.VideoBitrate = VIDEO_BITRATES[i]), "VIDEO_BITRATE_DESC".GetDialogText());
        captureSettings.AddWithDescription(menu, new TextMenu.Slider("AUDIO_BITRATE".GetDialogText(), i => $"{AUDIO_BITRATES[i] / 1000} kb/s", 0, AUDIO_BITRATES.Length - 1, Array.IndexOf(AUDIO_BITRATES, Settings.AudioBitrate))
                 .Change(i => Settings.AudioBitrate = AUDIO_BITRATES[i]), "AUDIO_BITRATE_DESC".GetDialogText());

        menu.AddAll(captureSettings);

        var codecSubMenu = new TextMenuExt.SubMenu("CODEC_SETTINGS".GetDialogText(), enterOnSelect: false);

        codecSubMenu.Add(new TextMenu.Slider("CONTAINER_TYPE".GetDialogText(), i => $"CONTAINER_TYPE_{CONTAINER_TYPES[i]}".GetDialogText(), 0, CONTAINER_TYPES.Length - 1, Array.IndexOf(CONTAINER_TYPES, Settings.ContainerType))
                 .Change(i => {
                    Settings.ContainerType = CONTAINER_TYPES[i];
                    _h264Preset.Disabled = !(Settings.VideoCodecOverwrite == (int)AVCodecID.AV_CODEC_ID_H264 || Settings.VideoCodecOverwrite == -1 && Settings.ContainerType is "mp4" or "mkv" or "mov");
                }));

        codecSubMenu.AddWithDescription(menu, new TextMenu.Slider("VIDEO_CODEC_OVERWRITE".GetDialogText(), i => $"VIDEO_CODEC_OVERWRITE_{VIDEO_CODECS[i]}".Replace("-1", "default").GetDialogText(), 0, VIDEO_CODECS.Length - 1, Array.IndexOf(VIDEO_CODECS, Settings.VideoCodecOverwrite))
                 .Change(i => {
                    Settings.VideoCodecOverwrite = VIDEO_CODECS[i];
                    _h264Preset.Disabled = !(Settings.VideoCodecOverwrite == (int)AVCodecID.AV_CODEC_ID_H264 || Settings.VideoCodecOverwrite == -1 && Settings.ContainerType is "mp4" or "mkv" or "mov");
                 }), "VIDEO_CODEC_OVERWRITE_DESC".GetDialogText(), Color.Orange);
        codecSubMenu.AddWithDescription(menu, new TextMenu.Slider("AUDIO_CODEC_OVERWRITE".GetDialogText(), i => $"AUDIO_CODEC_OVERWRITE_{AUDIO_CODECS[i]}".Replace("-1", "default").GetDialogText(), 0, AUDIO_CODECS.Length - 1, Array.IndexOf(AUDIO_CODECS, Settings.AudioCodecOverwrite))
                 .Change(i => Settings.AudioCodecOverwrite = AUDIO_CODECS[i]), "AUDIO_CODEC_OVERWRITE_DESC".GetDialogText(), Color.Orange);

        _h264Preset = new TextMenu.Slider("H264_PRESET".GetDialogText(), i => $"H264_PRESET_{H264_PRESETS[i]}".GetDialogText(), 0, H264_PRESETS.Length - 1, Array.IndexOf(H264_PRESETS, Settings.H264Preset))
                  .Change(i => Settings.H264Preset = H264_PRESETS[i]);
        _h264Preset.Disabled = !(Settings.VideoCodecOverwrite == (int)AVCodecID.AV_CODEC_ID_H264 || Settings.VideoCodecOverwrite == -1 && Settings.ContainerType is "mp4" or "mkv" or "mov");
        codecSubMenu.AddWithDescription(menu, _h264Preset, "H264_PRESET_DESC".GetDialogText());

        menu.Add(codecSubMenu);
        captureSettings.AddRange(codecSubMenu.Items);

        var recordingSubMenu = new TextMenuExt.SubMenu("RECORDING_BANNER".GetDialogText(), enterOnSelect: false);

        recordingSubMenu.Add(new TextMenu.OnOff("RECORDING_INDICATOR".GetDialogText(), Settings.RecordingIndicator)
                 .Change(b => Settings.RecordingIndicator = b));
        recordingSubMenu.Add(new TextMenu.Slider("RECORDING_TIME".GetDialogText(), i => $"RECORDING_TIME_{(RecordingTimeIndicator)i}".GetDialogText(), 0, Enum.GetValues(typeof(RecordingTimeIndicator)).Length - 1, (int)Settings.RecordingTime)
                .Change(i => Settings.RecordingTime = (RecordingTimeIndicator)i));
        recordingSubMenu.Add(new TextMenu.OnOff("RECORDING_PROGRESS".GetDialogText(), Settings.RecordingProgrees)
                 .Change(b => Settings.RecordingProgrees = b));

        menu.Add(recordingSubMenu);

        if (TASRecorderModule.Recording) {
            DisableMenu();
        }
    }

    internal static void EnableMenu() {
        foreach (var item in captureSettings) {
            item.Disabled = false;
        }
        if (_h264Preset != null)
            _h264Preset.Disabled = !(Settings.VideoCodecOverwrite == (int)AVCodecID.AV_CODEC_ID_H264 || Settings.VideoCodecOverwrite == -1 && Settings.ContainerType is "mp4" or "mkv" or "mov");
    }
    internal static void DisableMenu() {
        foreach (var item in captureSettings) {
            item.Disabled = true;
        }
        if (_h264Preset != null)
            _h264Preset.Disabled = true;
    }

    private static int[] CreateIntRange(int min, int max, int step) {
        int length = (int) Math.Ceiling((max - min) / (float)step) + 1;
        int[] values = new int[length];

        for (int i = 0, val = min; i < length; i++, val += step) {
            values[i] = val;
        }

        return values;
    }

    public static void AddAll(this TextMenu menu, List<TextMenu.Item> menuItems) {
        foreach (var item in menuItems) {
            menu.Add(item);
        }
    }

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
    public static void AddWithDescription(this TextMenuExt.SubMenu subMenu, TextMenu menu, TextMenu.Item menuItem, string description, Color? color = null) {
        TextMenuExt.EaseInSubHeaderExt descriptionText = new(description, false, menu) {
            TextColor = color ?? Color.Gray,
            HeightExtra = 0f
        };

        subMenu.Add(menuItem);
        subMenu.Add(descriptionText);

        menuItem.OnEnter += () => descriptionText.FadeVisible = true;
        menuItem.OnLeave += () => descriptionText.FadeVisible = false;
    }
    public static void AddWithDescription(this List<TextMenu.Item> items, TextMenu menu, TextMenu.Item menuItem, string description, Color? color = null) {
        TextMenuExt.EaseInSubHeaderExt descriptionText = new(description, false, menu) {
            TextColor = color ?? Color.Gray,
            HeightExtra = 0f
        };

        items.Add(menuItem);
        items.Add(descriptionText);

        menuItem.OnEnter += () => descriptionText.FadeVisible = true;
        menuItem.OnLeave += () => descriptionText.FadeVisible = false;
    }

    public static string GetDialogText(this string text) => Dialog.Clean($"TAS_RECORDER_{text}");
}
