using Celeste.Mod.TASRecorder.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NativeDialog = NativeFileDialogSharp.Dialog;

namespace Celeste.Mod.TASRecorder;

internal record MenuEntry {
    private static readonly Color DefaultDescriptionColor = Color.Gray;

    public required Func<(string Text, Color Color)>? DescriptionProvider;
    public readonly List<Func<bool>> EnableConditions = [];

    public required TextMenu.Item Item;
    public TextMenuExt.EaseInSubHeaderExt? DescriptionItem;

    private bool isSelected = false;

    public MenuEntry WithDescription(string text, Color? color = null) {
        DescriptionProvider = () => (text, color ?? DefaultDescriptionColor);
        return this;
    }
    public MenuEntry WithDynamicDescription(Func<string> text, Color? color = null) {
        DescriptionProvider = () => (text(), color ?? DefaultDescriptionColor);
        return this;
    }

    public MenuEntry WithCondition(Func<bool> condition) {
        EnableConditions.Add(condition);
        return this;
    }

    public static implicit operator MenuEntry(TextMenu.Item item) => new() {
        Item = item, DescriptionProvider = null,
    };

    public void OnEnter() {
        isSelected = true;
        UpdateDescription();
    }
    public void OnExit() {
        isSelected = false;
        UpdateDescription();
    }

    public void UpdateDescription() {
        if (DescriptionItem == null) return;

        if (!isSelected) {
            DescriptionItem.FadeVisible = false;
            return;
        }

        if (DescriptionProvider != null) {
            (string descText, Color descColor) = DescriptionProvider();
            if (string.IsNullOrWhiteSpace(descText)) {
                DescriptionItem.FadeVisible = false;
            } else {
                DescriptionItem.Title = descText;
                DescriptionItem.TextColor = descColor;
                DescriptionItem.FadeVisible = true;
            }
        }
    }
}

public static class TASRecorderMenu {
    public const string NoCodecOverride = "";
    public const string CodecOverride_None = "none";

    private static TASRecorderModuleSettings Settings => TASRecorderModule.Settings;

    private static readonly List<MenuEntry> AllEntries = [];

    internal static void OnStateChanged() {
        foreach (var entry in AllEntries) {
            // Disable if any condition fails
            entry.Item.Disabled = entry.EnableConditions.Count > 0 && entry.EnableConditions.Any(cond => !cond());
            entry.UpdateDescription();
        }
    }

    private static bool NotRecording() => !RecordingManager.Recording;

    private static bool NotRecordingOrLibraryEncoder() =>
        !RecordingManager.Recording || RecordingManager.Encoder is FFmpegLibraryEncoder;

    public static bool UsingH264() => Settings.VideoCodecOverride is ("libx264" or "libx264rgb") ||
                                      Settings is { VideoCodecOverride: "", ContainerType: "mp4" or "mkv" or "mov" } ||
                                      Settings.HardwareAccelerationType != HWAccelType.None;

    internal static void CreateSettingsMenu(TextMenu menu) {
        AllEntries.Clear();

        menu.AddAll([
            CreateSlider(nameof(TASRecorderModuleSettings.FPS),
                    [24, 30, 60, 120], fps => $"{fps} FPS")
                .WithDynamicDescription(() => Settings.FPS > 60 ? "FPS_DESC".GetDialog() : string.Empty)
                .WithCondition(NotRecordingOrLibraryEncoder),
            CreateSlider(nameof(TASRecorderModuleSettings.VideoResolution),
                    [1, 2, 3, 4, 5, 6, 8, 12], res => $"{res * Celeste.GameWidth}x{res * Celeste.GameHeight}")
                .WithDynamicDescription(() => Settings.VideoResolution > 6 ? "VideoResolution_DESC".GetDialog() : string.Empty)
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
                    [EncoderType.FFmpegBinary, EncoderType.FFmpegLibrary, EncoderType.None])
#else
                    [EncoderType.FFmpegBinary, EncoderType.FFmpegLibrary])
#endif
                .WithDescription("EncoderType_DESC".GetDialog())
                .WithCondition(NotRecording),
            CreateSlider(nameof(TASRecorderModuleSettings.HardwareAccelerationType),
                    [HWAccelType.None, HWAccelType.QSV, HWAccelType.NVENC, HWAccelType.AMF, HWAccelType.VideoToolbox])
                .WithDescription("HardwareAccelerationType_DESC".GetDialog())
                .WithCondition(NotRecording)
                .WithCondition(() => Settings.EncoderType == EncoderType.FFmpegBinary),

            CreateSubMenu("CODEC_SETTINGS", [
                CreateSlider(nameof(TASRecorderModuleSettings.ContainerType),
                        ["mp4", "mkv", "mov", "webm"])
                    .WithCondition(NotRecording),
                CreateSlider(nameof(TASRecorderModuleSettings.VideoCodecOverride),
                    [
                        NoCodecOverride,
                        CodecOverride_None,
                        "libx264",
                        "libx264rgb",
                        "av1",
                        "vp9",
                        "vp8",
                    ])
                    .WithDescription("VideoCodecOverride_DESC".GetDialog())
                    .WithCondition(NotRecording)
                    .WithCondition(() =>
                        Settings.EncoderType != EncoderType.FFmpegBinary ||
                        Settings.HardwareAccelerationType == HWAccelType.None),
                CreateSlider(nameof(TASRecorderModuleSettings.AudioCodecOverride),
                    [
                        NoCodecOverride,
                        CodecOverride_None,
                        "aac",
                        "mp3",
                        "flac",
                        "opus",
                        "vorbis",
                    ])
                    .WithDescription("AudioCodecOverride_DESC".GetDialog())
                    .WithCondition(NotRecording),
                CreateSlider(nameof(TASRecorderModuleSettings.H264Preset),
                    [
                        "veryslow", "slower", "slow", "medium", "fast", "faster", "veryfast", "superfast", "ultrafast"
                    ])
                    .WithDescription("H264Preset_DESC".GetDialog())
                    .WithCondition(UsingH264)
                    .WithCondition(NotRecording),
                CreateSlider(nameof(TASRecorderModuleSettings.H264Quality),
                        CreateIntRange(0, 51), crf => Dialog.Has($"TAS_RECORDER_H264Quality_{crf}")
                            ? $"H264Quality_{crf}".GetDialog()
                            : crf.ToString())
                    .WithDescription("H264Quality_DESC".GetDialog())
                    .WithCondition(UsingH264)
                    .WithCondition(NotRecording)
            ]),

            CreateSubMenu("RECORDING_BANNER", [
                CreateOnOff(nameof(TASRecorderModuleSettings.RecordingIndicator)),
                CreateSlider(nameof(Settings.RecordingTime),
                [
                    RecordingTimeIndicator.NoTime, RecordingTimeIndicator.Regular, RecordingTimeIndicator.RegularFrames
                ]),
                CreateOnOff(nameof(TASRecorderModuleSettings.RecordingProgress))
            ])
        ]);

        // Apply all conditions on menu creation
        OnStateChanged();
    }

    // ref is not allowed inside lambdas, which is required for setting the value.
    // This could be cached, however it's only invoked on menu creation, so it's fine.
    private static MenuEntry CreateOnOff(string settingName) {
        var prop = typeof(TASRecorderModuleSettings).GetProperty(settingName)!;
        if (prop.PropertyType != typeof(bool))
            throw new ArgumentException($"The setting {settingName} is not of type bool");

        return new TextMenu.OnOff(settingName.GetDialog(), (bool) prop.GetValue(Settings)!)
            .Change(b => {
                prop.SetValue(Settings, b);
                OnStateChanged();
            });
    }

    private static MenuEntry CreateSlider<T>(string settingName, T[] options, Func<T, string>? toString = null)
        where T : notnull {
        var prop = typeof(TASRecorderModuleSettings).GetProperty(settingName)!;
        if (prop.PropertyType != typeof(T))
            throw new ArgumentException($"The setting {settingName} is not of type {nameof(T)}");

        return new TextMenu.Slider(settingName.GetDialog(), i => {
                    if (toString == null) {
                        // Replace '-' with 'N', because '-' inside dialogs breaks.
                        return $"{settingName}_{options[i].ToString()!.Replace('-', 'N')}".GetDialog();
                    }

                    return toString(options[i]);
                }, min: 0, max: options.Length - 1,
                Array.FindIndex(options, x => EqualityComparer<T>.Default.Equals(x, (T) prop.GetValue(Settings)!)))
            .Change(i => {
                prop.SetValue(Settings, options[i]);
                OnStateChanged();
            });
    }

    private static MenuEntry CreateFolderSelection(string settingName) {
        var prop = typeof(TASRecorderModuleSettings).GetProperty(settingName)!;
        if (prop.PropertyType != typeof(string))
            throw new ArgumentException($"The setting {settingName} is not of type string");

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

            if (entry.DescriptionProvider == null) continue;
            (string descText, Color descColor) = entry.DescriptionProvider();

            entry.DescriptionItem = new TextMenuExt.EaseInSubHeaderExt(descText, false, fakeMenu) {
                TextColor = descColor, HeightExtra = 0f,
            };
            subMenu.Add(entry.DescriptionItem);

            entry.Item.OnEnter += entry.OnEnter;
            entry.Item.OnLeave += entry.OnExit;
            entry.UpdateDescription();
        }

        return subMenu;
    }

    private static void AddAll(this TextMenu menu, IEnumerable<MenuEntry> entries) {
        foreach (var entry in entries) {
            AllEntries.Add(entry);

            menu.Add(entry.Item);

            if (entry.DescriptionProvider == null) continue;
            (string descText, Color descColor) = entry.DescriptionProvider();

            entry.DescriptionItem = new TextMenuExt.EaseInSubHeaderExt(descText, false, fakeMenu) {
                TextColor = descColor, HeightExtra = 0f,
            };
            menu.Add(entry.DescriptionItem);

            entry.Item.OnEnter += entry.OnEnter;
            entry.Item.OnLeave += entry.OnExit;
            entry.UpdateDescription();
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
