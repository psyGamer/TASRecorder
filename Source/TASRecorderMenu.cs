using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FFmpeg;

namespace Celeste.Mod.TASRecorder;

internal record MenuEntry {
    public TextMenu.Item Item;
    public string DescriptionText;
    public Color DescriptionColor;

    public static implicit operator MenuEntry(TextMenu.Item item) => new() {
        Item = item,
        DescriptionText = string.Empty,
        DescriptionColor = Color.Transparent,
    };
}
internal static class MenuEntryExtensions {
    public static MenuEntry WithDescription(this TextMenu.Item item, string text, Color? color = null) => new() {
        Item = item,
        DescriptionText = text,
        DescriptionColor = color ?? Color.Gray,
    };
}

public static class TASRecorderMenu {
    private static TASRecorderModuleSettings Settings => TASRecorderModule.Settings;

    internal static void CreateSettingsMenu(TextMenu menu) {
        menu.AddAll(new MenuEntry[] {
            CreateSlider(nameof(TASRecorderModuleSettings.FPS),
                         new[] { 24, 30, 60 }, fps => $"{fps} FPS",
                         disableWhileRecording: true),
            CreateSlider(nameof(TASRecorderModuleSettings.VideoResolution),
                         CreateIntRange(1, 6), res => $"{res * Celeste.GameWidth}x{res * Celeste.GameHeight}",
                         disableWhileRecording: true),
            CreateSlider(nameof(TASRecorderModuleSettings.VideoBitrate),
                         CreateIntRange(1000000, 10000000, 500000), rate => $"{rate / 1000} kb/s",
                         disableWhileRecording: true)
                .WithDescription("VideoBitrate_DESC".GetDialog()),
            CreateSlider(nameof(TASRecorderModuleSettings.AudioBitrate),
                         CreateIntRange(32000, 512000, 16000), rate => $"{rate / 1000} kb/s",
                         disableWhileRecording: true)
                .WithDescription("AudioBitrate_DESC".GetDialog()),

            CreateSubMenu("CODEC_SETTINGS", new MenuEntry[] {
                CreateSlider(nameof(TASRecorderModuleSettings.ContainerType),
                             new[] { "mp4", "mkv", "mov", "webm" },
                             disableWhileRecording: true),
                CreateSlider(nameof(TASRecorderModuleSettings.VideoCodecOverwrite),
                             new[] { -1, // No overwrite
                                (int)AVCodecID.AV_CODEC_ID_NONE,
                                (int)AVCodecID.AV_CODEC_ID_H264,
                                (int)AVCodecID.AV_CODEC_ID_AV1,
                                (int)AVCodecID.AV_CODEC_ID_VP9,
                                (int)AVCodecID.AV_CODEC_ID_VP8, },
                             disableWhileRecording: true)
                    .WithDescription("VideoCodecOverwrite_DESC".GetDialog(), Color.Yellow),
                CreateSlider(nameof(TASRecorderModuleSettings.AudioCodecOverwrite),
                             new[] { -1, // No overwrite
                                (int)AVCodecID.AV_CODEC_ID_NONE,
                                (int)AVCodecID.AV_CODEC_ID_AAC,
                                (int)AVCodecID.AV_CODEC_ID_MP3,
                                (int)AVCodecID.AV_CODEC_ID_OPUS,
                                (int)AVCodecID.AV_CODEC_ID_VORBIS, },
                             disableWhileRecording: true)
                    .WithDescription("AudioCodecOverwrite_DESC".GetDialog(), Color.Yellow),
                CreateSlider(nameof(TASRecorderModuleSettings.H264Preset),
                             new[] { "veryslow", "slower", "slow", "medium", "fast", "faster", "veryfast", "superfast", "ultrafast" },
                             disableWhileRecording: true)
                    .WithDescription("H264Preset_DESC".GetDialog()),
            }),

            CreateSubMenu("RECORDING_BANNER", new MenuEntry[] {
                CreateOnOff(nameof(TASRecorderModuleSettings.RecordingIndicator), Settings.RecordingIndicator),
                CreateSlider(nameof(Settings.RecordingTime),
                             new[] { RecordingTimeIndicator.NoTime, RecordingTimeIndicator.Regular, RecordingTimeIndicator.RegularFrames }),
                CreateOnOff(nameof(TASRecorderModuleSettings.RecordingProgrees), Settings.RecordingProgrees),
            }),
        });
    }

    // ref is not allowed inside lambdas, which is required for setting the value.
    // This could be cached, however it's only invoked on menu creation, so it's fine.
    private static TextMenu.Item CreateOnOff(string settingName, bool disableWhileRecording = false) {
        var prop = typeof(TASRecorderModuleSettings).GetProperty(settingName);
        if (prop.PropertyType != typeof(bool)) throw new ArgumentException($"The setting {settingName} is not of type bool");

        return new TextMenu.OnOff(settingName.GetDialog(), (bool)prop.GetValue(Settings))
            .Change(b => prop.SetValue(Settings, b));
    }

    private static TextMenu.Item CreateSlider<T>(string settingName, T[] options, Func<T, string> toString = null, bool disableWhileRecording = false) {
        var prop = typeof(TASRecorderModuleSettings).GetProperty(settingName);
        if (prop.PropertyType != typeof(T)) throw new ArgumentException($"The setting {settingName} is not of type {nameof(T)}");

        return new TextMenu.Slider(settingName.GetDialog(), i => {
            if (toString == null) {
                // Replace '-' with 'N', because '-' inside dialogs breaks.
                return $"{settingName}_{options[i].ToString().Replace('-', 'N')}".GetDialog();
            }
            return toString(options[i]);
        }, min: 0, max: options.Length - 1, Array.FindIndex(options, x => EqualityComparer<T>.Default.Equals(x, (T)prop.GetValue(Settings))))
            .Change(i => prop.SetValue(Settings, options[i]));
    }

    private static readonly TextMenu fakeMenu = new();
    private static TextMenu.Item CreateSubMenu(string dialogText, MenuEntry[] entires) {
        var subMenu = new TextMenuExt.SubMenu(dialogText.GetDialog(), enterOnSelect: false);

        foreach (var entry in entires) {
            subMenu.Add(entry.Item);

            if (string.IsNullOrWhiteSpace(entry.DescriptionText)) continue;

            var desc = new TextMenuExt.EaseInSubHeaderExt(entry.DescriptionText, false, fakeMenu) {
                TextColor = entry.DescriptionColor,
                HeightExtra = 0f
            };
            subMenu.Add(desc);

            entry.Item.OnEnter += () => desc.FadeVisible = true;
            entry.Item.OnLeave += () => desc.FadeVisible = false;
        }

        return subMenu;
    }

    private static int[] CreateIntRange(int min, int max, int step = 1) {
        int length = (int) Math.Ceiling((max - min) / (float)step) + 1;
        int[] values = new int[length];

        for (int i = 0, val = min; i < length; i++, val += step) {
            values[i] = val;
        }

        return values;
    }

    internal static void AddAll(this TextMenu menu, IEnumerable<MenuEntry> entires) {
        foreach (var entry in entires) {
            menu.Add(entry.Item);

            if (string.IsNullOrWhiteSpace(entry.DescriptionText)) continue;

            var desc = new TextMenuExt.EaseInSubHeaderExt(entry.DescriptionText, false, menu) {
                TextColor = entry.DescriptionColor,
                HeightExtra = 0f
            };
            menu.Add(desc);

            entry.Item.OnEnter += () => desc.FadeVisible = true;
            entry.Item.OnLeave += () => desc.FadeVisible = false;
        }
    }

    public static string GetDialog(this string text) => Dialog.Clean($"TAS_RECORDER_{text}");
}
