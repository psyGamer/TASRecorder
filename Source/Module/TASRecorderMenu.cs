using Celeste.Mod.TASRecorder.Util;
using FFmpeg;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NativeDialog = NativeFileDialogSharp.Dialog;

namespace Celeste.Mod.TASRecorder;

internal record MenuEntry {
    private static readonly Color DefaultDescriptionColor = Color.Gray;

    public required TextMenu.Item Item;
    public required string DescriptionText;
    public Color DescriptionColor = DefaultDescriptionColor;
    public readonly List<Func<bool>> EnableConditions = new();

    public MenuEntry WithDescription(string text, Color? color = null) {
        DescriptionText = text;
        DescriptionColor = color ?? DefaultDescriptionColor;
        return this;
    }
    public MenuEntry WithCondition(Func<bool> condition) {
        EnableConditions.Add(condition);
        return this;
    }

    public static implicit operator MenuEntry(TextMenu.Item item) => new() {
        Item = item,
        DescriptionText = string.Empty,
    };
}

public static class TASRecorderMenu {
    public const int NoOverwriteId = -1;
    public const int H264RgbId = -2;

    private static TASRecorderModuleSettings Settings => TASRecorderModule.Settings;

    private static readonly List<MenuEntry> AllEntries = new();
    internal static void OnStateChanged() {
        foreach (var entry in AllEntries) {
            // Disable if any condition fails
            entry.Item.Disabled = entry.EnableConditions.Count > 0 && entry.EnableConditions.Any(cond => !cond());
        }
    }

    private static bool NotRecording() => !RecordingManager.Recording;
    private static bool NotRecordingOrLibraryEncoder() => !RecordingManager.Recording || RecordingManager.Encoder is FFmpegLibraryEncoder;
    public static bool UsingH264() => Settings.VideoCodecOverwrite == (int)AVCodecID.AV_CODEC_ID_H264 ||
                                       Settings.VideoCodecOverwrite == H264RgbId ||
                                       Settings is { VideoCodecOverwrite: -1, ContainerType: "mp4" or "mkv" or "mov" } ||
                                       Settings.HardwareAccelerationType != HWAccelType.None;

    internal static void CreateSettingsMenu(TextMenu menu) {
        AllEntries.Clear();

        menu.AddAll(new[] {
            CreateSlider(nameof(TASRecorderModuleSettings.FPS),
                         new[] { 24, 30, 60 }, fps => $"{fps} FPS")
                .WithCondition(NotRecordingOrLibraryEncoder),
            CreateSlider(nameof(TASRecorderModuleSettings.VideoResolution),
                         CreateIntRange(1, 6), res => $"{res * Celeste.GameWidth}x{res * Celeste.GameHeight}")
                .WithCondition(NotRecordingOrLibraryEncoder),
            CreateSlider(nameof(TASRecorderModuleSettings.VideoBitrate),
                         CreateIntRange(1000000, 10000000, 500000), rate => $"{rate / 1000} kb/s")
                .WithDescription("VideoBitrate_DESC".GetDialog())
                .WithCondition(NotRecordingOrLibraryEncoder),
            CreateSlider(nameof(TASRecorderModuleSettings.AudioBitrate),
                         CreateIntRange(32000, 512000, 16000), rate => $"{rate / 1000} kb/s")
                .WithDescription("AudioBitrate_DESC".GetDialog())
                .WithCondition(NotRecordingOrLibraryEncoder),
            CreateFolderSelection(nameof(TASRecorderModuleSettings.OutputDirectory)),
            CreateSlider(nameof(TASRecorderModuleSettings.EncoderType),
#if DEBUG
                new[] { EncoderType.FFmpegBinary, EncoderType.FFmpegLibrary, EncoderType.Null })
#else
                new[] { EncoderType.FFmpegBinary, EncoderType.FFmpegLibrary })
#endif
                .WithDescription("EncoderType_DESC".GetDialog())
                .WithCondition(NotRecording),
            CreateSlider(nameof(TASRecorderModuleSettings.HardwareAccelerationType),
                new[] { HWAccelType.None, HWAccelType.QSV, HWAccelType.NVENC, HWAccelType.AMF, HWAccelType.VideoToolbox })
                .WithDescription("HardwareAccelerationType_DESC".GetDialog())
                .WithCondition(NotRecording)
                .WithCondition(() => Settings.EncoderType == EncoderType.FFmpegBinary),

            CreateSubMenu("CODEC_SETTINGS", new[] {
                CreateSlider(nameof(TASRecorderModuleSettings.ContainerType),
                             new[] { "mp4", "mkv", "mov", "webm" })
                    .WithCondition(NotRecording),
                CreateSlider(nameof(TASRecorderModuleSettings.VideoCodecOverwrite),
                             new[] {
                                 NoOverwriteId,
                                (int)AVCodecID.AV_CODEC_ID_NONE,
                                (int)AVCodecID.AV_CODEC_ID_H264,
                                 H264RgbId,
                                (int)AVCodecID.AV_CODEC_ID_AV1,
                                (int)AVCodecID.AV_CODEC_ID_VP9,
                                (int)AVCodecID.AV_CODEC_ID_VP8,
                             })
                    .WithDescription("VideoCodecOverwrite_DESC".GetDialog())
                    .WithCondition(NotRecording)
                    .WithCondition(() => Settings.EncoderType != EncoderType.FFmpegBinary || Settings.HardwareAccelerationType == HWAccelType.None),
                CreateSlider(nameof(TASRecorderModuleSettings.AudioCodecOverwrite),
                             new[] {
                                 NoOverwriteId,
                                (int)AVCodecID.AV_CODEC_ID_NONE,
                                (int)AVCodecID.AV_CODEC_ID_AAC,
                                (int)AVCodecID.AV_CODEC_ID_MP3,
                                (int)AVCodecID.AV_CODEC_ID_FLAC,
                                (int)AVCodecID.AV_CODEC_ID_OPUS,
                                (int)AVCodecID.AV_CODEC_ID_VORBIS,
                             })
                    .WithDescription("AudioCodecOverwrite_DESC".GetDialog())
                    .WithCondition(NotRecording),
                CreateSlider(nameof(TASRecorderModuleSettings.H264Preset),
                             new[] { "veryslow", "slower", "slow", "medium", "fast", "faster", "veryfast", "superfast", "ultrafast" })
                    .WithDescription("H264Preset_DESC".GetDialog())
                    .WithCondition(UsingH264)
                    .WithCondition(NotRecording),
                CreateSlider(nameof(TASRecorderModuleSettings.H264Quality),
                        CreateIntRange(0, 51), crf => Dialog.Has($"TAS_RECORDER_H264Quality_{crf}")
                            ? $"H264Quality_{crf}".GetDialog()
                            : crf.ToString())
                    .WithDescription("H264Quality_DESC".GetDialog())
                    .WithCondition(UsingH264)
                    .WithCondition(NotRecording),
            }),

            CreateSubMenu("RECORDING_BANNER", new[] {
                CreateOnOff(nameof(TASRecorderModuleSettings.RecordingIndicator)),
                CreateSlider(nameof(Settings.RecordingTime),
                             new[] { RecordingTimeIndicator.NoTime, RecordingTimeIndicator.Regular, RecordingTimeIndicator.RegularFrames }),
                CreateOnOff(nameof(TASRecorderModuleSettings.RecordingProgress)),
            }),
        });

        // Apply all conditions on menu creation
        OnStateChanged();
    }

    // ref is not allowed inside lambdas, which is required for setting the value.
    // This could be cached, however it's only invoked on menu creation, so it's fine.
    private static MenuEntry CreateOnOff(string settingName) {
        var prop = typeof(TASRecorderModuleSettings).GetProperty(settingName)!;
        if (prop.PropertyType != typeof(bool)) throw new ArgumentException($"The setting {settingName} is not of type bool");

        return new TextMenu.OnOff(settingName.GetDialog(), (bool) prop.GetValue(Settings)!)
            .Change(b => {
                prop.SetValue(Settings, b);
                OnStateChanged();
            });
    }

    private static MenuEntry CreateSlider<T>(string settingName, T[] options, Func<T, string>? toString = null) where T: notnull {
        var prop = typeof(TASRecorderModuleSettings).GetProperty(settingName)!;
        if (prop.PropertyType != typeof(T)) throw new ArgumentException($"The setting {settingName} is not of type {nameof(T)}");

        return new TextMenu.Slider(settingName.GetDialog(), i => {
            if (toString == null) {
                // Replace '-' with 'N', because '-' inside dialogs breaks.
                return $"{settingName}_{options[i].ToString()!.Replace('-', 'N')}".GetDialog();
            }
            return toString(options[i]);
        }, min: 0, max: options.Length - 1, Array.FindIndex(options, x => EqualityComparer<T>.Default.Equals(x, (T) prop.GetValue(Settings)!)))
            .Change(i => {
                prop.SetValue(Settings, options[i]);
                OnStateChanged();
            });
    }

    private static MenuEntry CreateFolderSelection(string settingName) {
        var prop = typeof(TASRecorderModuleSettings).GetProperty(settingName)!;
        if (prop.PropertyType != typeof(string)) throw new ArgumentException($"The setting {settingName} is not of type string");

        string path = (string) prop.GetValue(Settings)!;
        if (path.StartsWith(Everest.PathGame)) {
            path = path.Remove(0, Everest.PathGame.Length);
            if (path.StartsWith("/")) path = path.Remove(0, 1); // We don't want to create a root
        }

        var button = new ValueButton(settingName.GetDialog(), path);
        button.Pressed(() => Task.Run(() => {
            var result = NativeDialog.FolderPicker(Everest.PathGame);
            if (result.IsCancelled) return;
            if (result.IsError) {
                Log.Error("Failed selecting folder!");
                Log.Error(result.ErrorMessage);
                return;
            }

            string? newPath = null;
            if (result.Path.StartsWith(Everest.PathGame)) {
                newPath = button.Value.Remove(0, Everest.PathGame.Length);
                if (newPath.StartsWith("/"))
                    newPath = button.Value.Remove(0, 1); // We don't want to create a root
            }

            prop.SetValue(Settings, newPath ?? result.Path);
            button.Value = newPath ?? result.Path;

            OnStateChanged();
        }));

        return button;
    }

    private static readonly TextMenu fakeMenu = new();
    private static MenuEntry CreateSubMenu(string dialogText, MenuEntry[] entries) {
        var subMenu = new TextMenuExt.SubMenu(dialogText.GetDialog(), enterOnSelect: false);

        foreach (var entry in entries) {
            AllEntries.Add(entry);

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

    private static void AddAll(this TextMenu menu, IEnumerable<MenuEntry> entries) {
        foreach (var entry in entries) {
            AllEntries.Add(entry);

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

    private static int[] CreateIntRange(int min, int max, int step = 1) {
        int length = (int) Math.Ceiling((max - min) / (float) step) + 1;
        int[] values = new int[length];

        for (int i = 0, val = min; i < length; i++, val += step) {
            values[i] = val;
        }

        return values;
    }

    private static string GetDialog(this string text) => Dialog.Clean($"TAS_RECORDER_{text}");
}
