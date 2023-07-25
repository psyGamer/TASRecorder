using FFmpeg;
using System;
using YamlDotNet.Serialization;

namespace Celeste.Mod.Capture;

public class CaptureModuleSettings : EverestModuleSettings {

    private static readonly int[] FRAME_RATES = { 24, 30, 60 };
    private static readonly int[] VIDEO_BITRATES = CreateIntRange(1000000, 10000000, 500000);
    private static readonly int[] AUDIO_BITRATES = CreateIntRange(32000, 512000, 16000);
    private static readonly Tuple<int, int>[] RESOLUTIONS = {
        Tuple.Create(320 * 1, 180 * 1),
        Tuple.Create(320 * 2, 180 * 2),
        Tuple.Create(320 * 3, 180 * 3),
        Tuple.Create(320 * 4, 180 * 4),
        Tuple.Create(320 * 5, 180 * 5),
        Tuple.Create(320 * 6, 180 * 6),
    };
    private static readonly (string, string)[] CONTAINER_TYPES = {
        ("mp4", "MPEG-4 (.mp4)"),
        ("mkv", "Matroska (.mkv)"),
        ("mov", "QuickTime (.mov)"),
        ("webm", "WebM (.webm)"),
    };
    private static readonly (int, string)[] VIDEO_CODECS = {
        (-1, "Default"),
        ((int)AVCodecID.AV_CODEC_ID_NONE, "No Video"),
        ((int)AVCodecID.AV_CODEC_ID_H264, "H.264"),
        ((int)AVCodecID.AV_CODEC_ID_AV1, "AV1"),
        ((int)AVCodecID.AV_CODEC_ID_VP9, "VP9"),
        ((int)AVCodecID.AV_CODEC_ID_VP8, "VP8"),
    };
    private static readonly (int, string)[] AUDIO_CODECS = {
        (-1, "Default"),
        ((int)AVCodecID.AV_CODEC_ID_NONE, "No Audio"),
        ((int)AVCodecID.AV_CODEC_ID_AAC, "AAC"),
        ((int)AVCodecID.AV_CODEC_ID_MP3, "MP3"),
        ((int)AVCodecID.AV_CODEC_ID_OPUS, "Opus"),
        ((int)AVCodecID.AV_CODEC_ID_VORBIS, "Vorbis"),
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

    public string ContainerType { get; set; } = "mp4";

    internal void CreateSettingsMenu(TextMenu menu) {
        menu.Add(new TextMenu.Slider("fps", (i) => $"{FRAME_RATES[i]}", 0, FRAME_RATES.Length - 1, Array.IndexOf(FRAME_RATES, FPS))
                 .Change(i => FPS = FRAME_RATES[i]));
        menu.Add(new TextMenu.Slider("resolution", (i) => $"{RESOLUTIONS[i].Item1}x{RESOLUTIONS[i].Item2}", 0, RESOLUTIONS.Length - 1, VideoResolution)
                 .Change(i => VideoResolution = i));

        menu.Add(new TextMenu.Slider("video_bitrate", (i) => $"{VIDEO_BITRATES[i] / 1000} kb/s", 0, VIDEO_BITRATES.Length - 1, Array.IndexOf(VIDEO_BITRATES, VideoBitrate))
                 .Change(i => VideoBitrate = VIDEO_BITRATES[i]));
        menu.Add(new TextMenu.Slider("audio_bitrate", (i) => $"{AUDIO_BITRATES[i] / 1000} kb/s", 0, AUDIO_BITRATES.Length - 1, Array.IndexOf(AUDIO_BITRATES, AudioBitrate))
                 .Change(i => AudioBitrate = AUDIO_BITRATES[i]));

        menu.Add(new TextMenu.Slider("container_type", (i) => $"{CONTAINER_TYPES[i].Item2}", 0, CONTAINER_TYPES.Length - 1, Array.IndexOf(CONTAINER_TYPES, ContainerType))
                 .Change(i => ContainerType = CONTAINER_TYPES[i].Item1));

        menu.Add(new TextMenu.Slider("video_codec_overwrite", (i) => $"{VIDEO_CODECS[i].Item2}", 0, VIDEO_CODECS.Length - 1, Array.FindIndex(VIDEO_CODECS, c => c.Item1 == VideoCodecOverwrite))
                 .Change(i => VideoCodecOverwrite = VIDEO_CODECS[i].Item1));
        menu.Add(new TextMenu.Slider("audio_codec_overwrite", (i) => $"{AUDIO_CODECS[i].Item2}", 0, AUDIO_CODECS.Length - 1, Array.FindIndex(AUDIO_CODECS, c => c.Item1 == AudioCodecOverwrite))
                 .Change(i => AudioCodecOverwrite = AUDIO_CODECS[i].Item1));
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
