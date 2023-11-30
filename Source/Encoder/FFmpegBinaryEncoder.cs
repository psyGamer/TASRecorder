using Celeste.Mod.TASRecorder.Util;
using FFMpegCore;
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
    private FFmpegBinaryEncoder Encoder;

    public VideoPipeSource(FFmpegBinaryEncoder encoder) {
        Encoder = encoder;
    }

    public string GetStreamArguments() => $"-f rawvideo -r {TASRecorderModule.Settings.FPS} -pix_fmt {FFmpegBinaryEncoder.INPUT_PIX_FMT} -s {TASRecorderModule.Settings.VideoWidth}x{TASRecorderModule.Settings.VideoHeight}";

    public async Task WriteAsync(Stream stream, CancellationToken token) {
        while (!Encoder.Finished || Encoder.FrameQueue.Count != 0) {
            if (!Encoder.FrameQueue.TryDequeue(out byte[]? frame)) continue;

            await stream.WriteAsync(frame, 0, frame.Length, token);
            Encoder.AvailableBuffers.Push(frame);
        }
    }
}

public unsafe class FFmpegBinaryEncoder : Encoder {
    // Input data format from the VideoCapture
    internal const string INPUT_PIX_FMT = "rgba";
    private const int BYTES_PER_PIXEL = 4;

    private GCHandle VideoHandle;
    private byte[] VideoBuffer;

    internal bool Finished = false; // Indicates that no new frames will be generated.
    internal readonly Stack<byte[]> AvailableBuffers = new();
    internal readonly Queue<byte[]> FrameQueue = new();

    private readonly Task<bool> Task;

    public FFmpegBinaryEncoder(string? fileName = null) : base(fileName) {
        VideoRowStride = TASRecorderModule.Settings.VideoWidth * BYTES_PER_PIXEL;
        HasVideo = true;
        HasAudio = false;
        Task = FFMpegArguments
            .FromPipeInput(new VideoPipeSource(this))
            .OutputToFile(FilePath)
            .ProcessAsynchronously();
    }

    public override void End() {
        Finished = true;
        Task.Wait();
    }

    public override void PrepareVideo(int width, int height) {
        if (width != TASRecorderModule.Settings.VideoWidth || height != TASRecorderModule.Settings.VideoHeight)
            throw new Exception($"Invalid recording size! Excepted {TASRecorderModule.Settings.VideoWidth}x{TASRecorderModule.Settings.VideoHeight}, got {width}x{height}");

        if (AvailableBuffers.TryPop(out byte[]? buffer)) {
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
        throw new NotImplementedException();
    }

    public override void FinishVideo() {
        FrameQueue.Enqueue(VideoBuffer);
        VideoData = null;
        VideoHandle.Free();
    }

    public override void FinishAudio() {
        throw new NotImplementedException();
    }
}
