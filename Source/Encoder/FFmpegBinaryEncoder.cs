using Celeste.Mod.TASRecorder.Util;
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
            Log.Info($"Video: {Encoder.VideoQueue.Count}");

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
    private byte[] VideoBuffer;
    private GCHandle AudioHandle;
    private byte[] AudioBuffer;

    internal bool Finished = false; // Indicates that no new frames will be generated.
    internal readonly Stack<byte[]> AvailableVideoBuffers = new();
    internal readonly Queue<byte[]> VideoQueue = new();
    internal readonly Stack<byte[]> AvailableAudioBuffers = new();
    internal readonly Queue<byte[]> AudioQueue = new();

    private readonly Task<bool> Task;

    public FFmpegBinaryEncoder(string? fileName = null) : base(fileName) {
        VideoRowStride = TASRecorderModule.Settings.VideoWidth * BYTES_PER_PIXEL;
        HasVideo = true;
        HasAudio = true;

        var args = FFMpegArguments
            .FromPipeInput(new VideoPipeSource(this), options => {
                options.WithVideoBitrate(TASRecorderModule.Settings.VideoBitrate);
                if (TASRecorderModule.Settings.HardwareAccelerationType == HWAccelType.QSV) {
                    options.WithHardwareAcceleration(HardwareAccelerationDevice.QSV);
                } else if (TASRecorderModule.Settings.HardwareAccelerationType == HWAccelType.NVENC) {
                    options.WithHardwareAcceleration(HardwareAccelerationDevice.CUDA);
                }
            })
            .AddPipeInput(new AudioPipeSource(this), options => {
                options.WithAudioBitrate(TASRecorderModule.Settings.AudioBitrate);
                if (TASRecorderModule.Settings.HardwareAccelerationType == HWAccelType.QSV) {
                    options.WithHardwareAcceleration(HardwareAccelerationDevice.QSV);
                } else if (TASRecorderModule.Settings.HardwareAccelerationType == HWAccelType.NVENC) {
                    options.WithHardwareAcceleration(HardwareAccelerationDevice.CUDA);
                }
            })
            .OutputToFile(FilePath, overwrite: true, options => {
                if (TASRecorderModule.Settings.HardwareAccelerationType == HWAccelType.QSV) {
                    options.WithVideoCodec("h264_qsv");
                } else if (TASRecorderModule.Settings.HardwareAccelerationType == HWAccelType.NVENC) {
                    options.WithVideoCodec("h264_nvenc");
                } else if (TASRecorderModule.Settings.HardwareAccelerationType == HWAccelType.AMF) {
                    options.WithVideoCodec("h264_amf");
                } else if (TASRecorderModule.Settings.HardwareAccelerationType == HWAccelType.VideoToolbox) {
                    options.WithVideoCodec("h264_videotoolbox");
                }
            });
        Log.Info($"Invoking FFmpeg with following arguments: \"{args.Arguments}\"");
        Task = args.ProcessAsynchronously();
    }

    public override void End() {
        Finished = true;
        Log.Info("Waiting for FFmpeg to finish...");
        Task.Wait();
    }

    public override void PrepareVideo(int width, int height) {
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
        VideoQueue.Enqueue(VideoBuffer);
        VideoData = null;
        VideoHandle.Free();
    }

    public override void FinishAudio() {
        AudioQueue.Enqueue(AudioBuffer);
        AudioData = null;
        AudioHandle.Free();
    }
}
