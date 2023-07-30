using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FFmpeg;
using Celeste.Mod.TASRecorder;

namespace Celeste.Mod.TASRecorder;

internal record MenuEntry {
    public TextMenu.Item Item;
    public List<(string, Color)> Descriptions;

    public MenuEntry AddDescription(string text, Color? color = null) {
        Descriptions.Add((text, color ?? Color.Gray));
        return this;
    }

    public static implicit operator MenuEntry(TextMenu.Item item) => new() {
        Item = item,
        Descriptions = new(),
    };
}

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
        menu.AddAll(new MenuEntry[] {
            new TextMenu.Button("hi"),
            CreateSlider(nameof(TASRecorderModuleSettings.FPS),
                         new[] { 24, 30, 60 }, fps => $"{fps} FPS",
                         disableWhileRecording: true),
            CreateSlider(nameof(TASRecorderModuleSettings.VideoResolution),
                         CreateIntRange(1, 6), res => $"{res * Celeste.GameWidth}x{res * Celeste.GameHeight}",
                         disableWhileRecording: true),
            CreateSlider(nameof(TASRecorderModuleSettings.VideoBitrate),
                         CreateIntRange(1000000, 10000000, 500000), rate => $"{rate / 1000} kb/s",
                         disableWhileRecording: true),
            CreateSlider(nameof(TASRecorderModuleSettings.AudioBitrate),
                         CreateIntRange(32000, 512000, 16000), rate => $"{rate / 1000} kb/s",
                         disableWhileRecording: true),

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
                             disableWhileRecording: true),
                CreateSlider(nameof(TASRecorderModuleSettings.AudioCodecOverwrite),
                             new[] { -1, // No overwrite
                                (int)AVCodecID.AV_CODEC_ID_NONE,
                                (int)AVCodecID.AV_CODEC_ID_AAC,
                                (int)AVCodecID.AV_CODEC_ID_MP3,
                                (int)AVCodecID.AV_CODEC_ID_OPUS,
                                (int)AVCodecID.AV_CODEC_ID_VORBIS, },
                             disableWhileRecording: true),
                CreateSlider(nameof(TASRecorderModuleSettings.H264Preset),
                             new[] { "veryslow", "slower", "slow", "medium", "fast", "faster", "veryfast", "superfast", "ultrafast" },
                             disableWhileRecording: true),
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

            if (entry.Descriptions.Count > 0) {
                var descriptions = entry.Descriptions.Select(line => {
                    var (text, color) = line;
                    // The containingMenu is only used for ItemSpacing
                    return new TextMenuExt.EaseInSubHeaderExt(text, false, fakeMenu) {
                        TextColor = color,
                        HeightExtra = 0f
                    };
                });

                foreach (var desc in descriptions) {
                    subMenu.Add(desc);
                }

                entry.Item.OnEnter += () => {
                    foreach (var desc in descriptions) {
                        desc.FadeVisible = true;
                    }
                };
                entry.Item.OnLeave += () => {
                    foreach (var desc in descriptions) {
                        desc.FadeVisible = false;
                    }
                };
            }
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

    internal static void AddAll(this TextMenu menu, IEnumerable<MenuEntry> menuItems) {
        foreach (var item in menuItems) {
            menu.Add(item.Item);
        }
    }

    public static string GetDialog(this string text) => Dialog.Clean($"TAS_RECORDER_{text}");
}
