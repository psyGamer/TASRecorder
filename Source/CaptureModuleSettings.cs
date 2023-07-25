using System;
using YamlDotNet.Serialization;

namespace Celeste.Mod.Capture;

public class CaptureModuleSettings : EverestModuleSettings {

    private static readonly int[] FRAME_RATES = { 24, 30, 60 };
    private static readonly int DEFAULT_FRAME_RATE_IDX = Array.IndexOf(FRAME_RATES, 60);
    private static readonly int[] VIDEO_BITRATES = CreateIntRange(1000000, 10000000, 500000);
    private static readonly int DEFAULT_VIDEO_BITRATE_IDX = Array.IndexOf(VIDEO_BITRATES, 6500000);
    private static readonly int[] AUDIO_BITRATES = CreateIntRange(32000, 512000, 16000);
    private static readonly int DEFAULT_AUDIO_BITRATE_IDX = Array.IndexOf(AUDIO_BITRATES, 128000);
    private static readonly Tuple<int, int>[] RESOLUTIONS = {
        Tuple.Create(320 * 1, 180 * 1),
        Tuple.Create(320 * 2, 180 * 2),
        Tuple.Create(320 * 3, 180 * 3),
        Tuple.Create(320 * 4, 180 * 4),
        Tuple.Create(320 * 5, 180 * 5),
        Tuple.Create(320 * 6, 180 * 6),
    };
    private static readonly int DEFAULT_RESOLUTION_IDX = 5; // 1920x1080

    public int FPS { get; set; } = 60;

    public int VideoResolution = 5;
    [YamlIgnore]
    public int VideoWidth => RESOLUTIONS[VideoResolution].Item1;
    [YamlIgnore]
    public int VideoHeight => RESOLUTIONS[VideoResolution].Item2;

    public int VideoBitrate { get; set; } = 6500000;
    public int AudioBitrate { get; set; } = 128000;

    public string VideoCodec { get; set; } = null;
    public string AudioCodec { get; set; } = null;

    public string ContainerType { get; set; } = "mp4";

    internal void CreateSettingsMenu(TextMenu menu, bool inGame) {
        menu.Add(new TextMenu.Slider("fps", (i) => $"{FRAME_RATES[i]}", 0, FRAME_RATES.Length - 1, DEFAULT_FRAME_RATE_IDX)
                 .Change(i => FPS = FRAME_RATES[i]));
        menu.Add(new TextMenu.Slider("resolution", (i) => $"{RESOLUTIONS[i].Item1}x{RESOLUTIONS[i].Item2}", 0, RESOLUTIONS.Length - 1, DEFAULT_RESOLUTION_IDX)
                 .Change(i => VideoResolution = i));

        menu.Add(new TextMenu.Slider("video_bitrate", (i) => $"{VIDEO_BITRATES[i] / 1000} kb/s", 0, VIDEO_BITRATES.Length - 1, DEFAULT_VIDEO_BITRATE_IDX)
                 .Change(i => VideoBitrate = VIDEO_BITRATES[i]));
        menu.Add(new TextMenu.Slider("audio_bitrate", (i) => $"{AUDIO_BITRATES[i] / 1000} kb/s", 0, AUDIO_BITRATES.Length - 1, DEFAULT_AUDIO_BITRATE_IDX)
                 .Change(i => AudioBitrate = AUDIO_BITRATES[i]));
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
