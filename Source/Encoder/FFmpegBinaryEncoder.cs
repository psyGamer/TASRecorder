using Celeste.Mod.TASRecorder.Util;
using FFmpeg;
using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Pipes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using BindingFlags = System.Reflection.BindingFlags;

namespace Celeste.Mod.TASRecorder;

internal class VideoPipeSource : IPipeSource {
    private readonly FFmpegBinaryEncoder Encoder;

    public VideoPipeSource(FFmpegBinaryEncoder encoder) {
        Encoder = encoder;
    }

    public string GetStreamArguments() => $"-f rawvideo -r {TASRecorderModule.Settings.FPS} -pix_fmt {FFmpegBinaryEncoder.INPUT_PIX_FMT} -s {TASRecorderModule.Settings.VideoWidth}x{TASRecorderModule.Settings.VideoHeight}";

    public async Task WriteAsync(Stream stream, CancellationToken token) {
        while (!Encoder.Finished || Encoder.VideoQueue.Count != 0) {
            if (!Encoder.VideoQueue.TryDequeue(out byte[]? frame)) continue;

            await stream.WriteAsync(frame, 0, frame.Length, token);
            Encoder.AvailableVideoBuffers.Push(frame);
        }
    }
}

internal class AudioPipeSource : IPipeSource {
    private readonly FFmpegBinaryEncoder Encoder;

    public AudioPipeSource(FFmpegBinaryEncoder encoder) {
        Encoder = encoder;
    }

    public string GetStreamArguments() => $"-f {FFmpegBinaryEncoder.INPUT_SAMPLE_FMT} -ar {TASRecorder.Encoder.AUDIO_SAMPLE_RATE} -ac {TASRecorder.Encoder.AUDIO_CHANNEL_COUNT}";

    public async Task WriteAsync(Stream stream, CancellationToken token) {
        while (!Encoder.Finished || Encoder.AudioQueue.Count != 0) {
            if (!Encoder.AudioQueue.TryDequeue(out byte[]? frame)) continue;

            await stream.WriteAsync(frame, 0, frame.Length, token);
            Encoder.AvailableAudioBuffers.Push(frame);
        }
    }
}

public unsafe class FFmpegBinaryEncoder : Encoder {
    // Input data format from the VideoCapture
    internal const string INPUT_PIX_FMT = "rgba";
    // Input data format from the AudioCapture
    internal const string INPUT_SAMPLE_FMT = "f32le";
    private const int BYTES_PER_PIXEL = 4;

    private GCHandle VideoHandle;
    private byte[] VideoBuffer = null!;
    private GCHandle AudioHandle;
    private byte[] AudioBuffer = null!;

    internal bool Finished = false; // Indicates that no new frames will be generated.
    internal readonly Stack<byte[]> AvailableVideoBuffers = new();
    internal readonly Queue<byte[]> VideoQueue = new();
    internal readonly Stack<byte[]> AvailableAudioBuffers = new();
    internal readonly Queue<byte[]> AudioQueue = new();

    private readonly Task<bool> Task;

    public FFmpegBinaryEncoder(string? fileName = null) : base(fileName) {
        VideoRowStride = TASRecorderModule.Settings.VideoWidth * BYTES_PER_PIXEL;
        HasVideo = TASRecorderModule.Settings.VideoCodecOverwrite != (int)AVCodecID.AV_CODEC_ID_NONE;
        HasAudio = TASRecorderModule.Settings.AudioCodecOverwrite != (int)AVCodecID.AV_CODEC_ID_NONE;

        // Thanks for making the constructor private...
        var c_ctor = typeof(FFMpegArguments).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, Array.Empty<Type>()) ??
                     throw new Exception("FFMpegArguments doesnt contain constructor???");
        FFMpegArguments args = (FFMpegArguments)c_ctor.Invoke(null);

        if (HasVideo) {
            args.AddPipeInput(new VideoPipeSource(this), options => {
                if (TASRecorderModule.Settings.HardwareAccelerationType == HWAccelType.QSV) {
                    options.WithHardwareAcceleration(HardwareAccelerationDevice.QSV);
                    options.WithCustomArgument("-hwaccel_output_format qsv");
                } else if (TASRecorderModule.Settings.HardwareAccelerationType == HWAccelType.NVENC) {
                    options.WithHardwareAcceleration(HardwareAccelerationDevice.CUDA);
                }
            });
        }

        var processor = args.OutputToFile(FilePath, overwrite: true, options => {
            if (HasVideo) {
                options.WithCustomArgument($"-b:v {TASRecorderModule.Settings.VideoBitrate}");
                switch (TASRecorderModule.Settings.HardwareAccelerationType)
                {
                    case HWAccelType.QSV:
                        options.WithVideoCodec("h264_qsv");
                        break;
                    case HWAccelType.NVENC:
                        string preset = TASRecorderModule.Settings.H264Preset switch {
                            // Thanks NVIDIA...
                            "veryfast" or "superfast" or "ultrafast" => "p1",
                            "faster" => "p2",
                            "fast" => "p3",
                            "medium" => "p4",
                            "slow" => "p5",
                            "slower" => "p6",
                            "veryslow" => "p7",
                            _ => throw new ArgumentOutOfRangeException()
                        };
                        options.WithVideoCodec("h264_nvenc");
                        options.WithCustomArgument($"-preset:v {preset}");
                        options.WithCustomArgument("-tune:v hq");
                        options.WithCustomArgument("-rc:v vbr");
                        options.WithCustomArgument($"-cq:v {TASRecorderModule.Settings.H264Quality}");
                        break;
                    case HWAccelType.AMF:
                        options.WithVideoCodec("h264_amf");
                        break;
                    case HWAccelType.VideoToolbox:
                        options.WithVideoCodec("h264_videotoolbox");
                        break;
                    case HWAccelType.None:
                    default:
                        if (TASRecorderModule.Settings.VideoCodecOverwrite != TASRecorderMenu.NoOverwriteId) {
                            options.WithVideoCodec(AVCodecIDToName(TASRecorderModule.Settings.VideoCodecOverwrite));
                        }
                        if (TASRecorderMenu.UsingH264()) {
                            options.WithCustomArgument($"-preset {TASRecorderModule.Settings.H264Preset}");
                            options.WithCustomArgument($"-crf {TASRecorderModule.Settings.H264Quality}");
                        }
                        break;
                }
            }

            if (HasAudio) {
                options.WithCustomArgument($"-b:a {TASRecorderModule.Settings.AudioBitrate}");
                if (TASRecorderModule.Settings.AudioCodecOverwrite != TASRecorderMenu.NoOverwriteId) {
                    options.WithAudioCodec(AVCodecIDToName(TASRecorderModule.Settings.AudioCodecOverwrite));
                }
            }
        });
        processor.NotifyOnError(stderr => Logger.Log(LogLevel.Error, $"{Log.TAG}/FFmpeg", stderr));
        processor.NotifyOnOutput(stdout => Logger.Log(LogLevel.Info, $"{Log.TAG}/FFmpeg" ,stdout));
        processor.NotifyOnProgress(progress => Log.Verbose($"Progress: {progress}"));

        Log.Info($"Invoking FFmpeg with following arguments: \"{processor.Arguments}\"");
        Task = processor.ProcessAsynchronously(throwOnError: true, new FFOptions { BinaryFolder = FFmpegLoader.BinaryPath });
    }

    public override void End() {
        Finished = true;
        Log.Info("Waiting for FFmpeg to finish...");
        Task.Wait();
    }

    public override void PrepareVideo(int width, int height) {
        CheckTask();

        if (width != TASRecorderModule.Settings.VideoWidth || height != TASRecorderModule.Settings.VideoHeight)
            throw new Exception($"Invalid recording resolution! Excepted {TASRecorderModule.Settings.VideoWidth}x{TASRecorderModule.Settings.VideoHeight}, got {width}x{height}");

        if (AvailableVideoBuffers.TryPop(out byte[]? buffer)) {
            VideoBuffer = buffer;
            VideoHandle = GCHandle.Alloc(VideoBuffer, GCHandleType.Pinned);
            VideoData = (byte*)VideoHandle.AddrOfPinnedObject();
            return;
        }

        VideoBuffer = new byte[width * height * BYTES_PER_PIXEL];
        VideoHandle = GCHandle.Alloc(VideoBuffer, GCHandleType.Pinned);
        VideoData = (byte*)VideoHandle.AddrOfPinnedObject();
    }

    public override void PrepareAudio(uint channelCount, uint sampleCount) {
        CheckTask();

        if (channelCount != AUDIO_CHANNEL_COUNT)
            throw new Exception($"Invalid recording channel count! Excepted {AUDIO_CHANNEL_COUNT}, got {channelCount}");

        if (AvailableAudioBuffers.TryPop(out byte[]? buffer)) {
            AudioBuffer = buffer;
            AudioHandle = GCHandle.Alloc(AudioBuffer, GCHandleType.Pinned);
            AudioData = (byte*)AudioHandle.AddrOfPinnedObject();
            return;
        }

        AudioBuffer = new byte[channelCount * sampleCount * (uint) Marshal.SizeOf<float>()];
        AudioHandle = GCHandle.Alloc(AudioBuffer, GCHandleType.Pinned);
        AudioData = (byte*)AudioHandle.AddrOfPinnedObject();
    }

    public override void FinishVideo() {
        CheckTask();

        VideoQueue.Enqueue(VideoBuffer);
        VideoData = null;
        VideoHandle.Free();
    }

    public override void FinishAudio() {
        CheckTask();

        AudioQueue.Enqueue(AudioBuffer);
        AudioData = null;
        AudioHandle.Free();
    }

    private static string AVCodecIDToName(int codecID) => codecID switch {
        // Video
        (int)AVCodecID.AV_CODEC_ID_H264 => "libx264",
        TASRecorderMenu.H264RgbId => "libx264rgb",
        (int)AVCodecID.AV_CODEC_ID_AV1 => "av1",
        (int)AVCodecID.AV_CODEC_ID_VP9 => "vp9",
        (int)AVCodecID.AV_CODEC_ID_VP8 => "vp8",
        // Audio
        (int)AVCodecID.AV_CODEC_ID_AAC => "acc",
        (int)AVCodecID.AV_CODEC_ID_MP3 => "mp3",
        (int)AVCodecID.AV_CODEC_ID_FLAC => "flac",
        (int)AVCodecID.AV_CODEC_ID_OPUS => "opus",
        (int)AVCodecID.AV_CODEC_ID_VORBIS => "vorbis",

        _ => throw new ArgumentOutOfRangeException(nameof(codecID), codecID, null)
    };

    private void CheckTask() {
        if (Task.Exception is not { } ex) return;

        Log.Error("FFmpeg binary process failed!");
        Log.Exception(ex);
        RecordingManager.StopRecording();
    }
}
