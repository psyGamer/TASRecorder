using Celeste.Mod.TASRecorder.Util;
using FFmpeg;
using System;
using System.Runtime.InteropServices;
using static FFmpeg.FFmpeg;

namespace Celeste.Mod.TASRecorder;

internal unsafe struct OutputStream {
    public AVStream* Stream;
    public AVCodecContext* CodecCtx;

    public AVFrame* InFrame; // For writing data into it
    public AVFrame* OutFrame; //  For writing to a file

    public AVPacket* Packet;

    public SwsContext* SwsCtx;
    public SwrContext* SwrCtx;

    public long VideoPTS;
    public int SamplesCount;
    public int FrameSamplePos;
}

public unsafe class FFmpegLibraryEncoder : Encoder {
    // Input data format from the VideoCapture
    private const AVPixelFormat INPUT_PIX_FMT = AVPixelFormat.AV_PIX_FMT_RGBA;

    uint AudioDataChannels;
    uint AudioDataSamples;
    uint AudioDataSize;
    uint AudioDataBufferSize;

    OutputStream VideoStream;
    OutputStream AudioStream;
    readonly AVFormatContext* FormatCtx;

    public FFmpegLibraryEncoder(string? fileName = null) : base(fileName) {
        AudioDataChannels = 0;
        AudioDataSamples = 0;
        AudioDataSize = 0;
        AudioDataBufferSize = 0;

        FormatCtx = null;
        VideoStream = default;
        AudioStream = default;

        fixed (AVFormatContext** pFmtCtx = &FormatCtx) {
            AvCheck(avformat_alloc_output_context2(pFmtCtx, null, null!, FilePath), "Failed allocating format context");
        }

        var videoCodecID = FormatCtx->oformat->video_codec;
        var audioCodecID = FormatCtx->oformat->audio_codec;

        if (TASRecorderModule.Settings.VideoCodecOverwrite != TASRecorderMenu.NoOverwriteId) {
            videoCodecID = (AVCodecID) TASRecorderModule.Settings.VideoCodecOverwrite;
        }
        if (TASRecorderModule.Settings.AudioCodecOverwrite != TASRecorderMenu.NoOverwriteId) {
            audioCodecID = (AVCodecID) TASRecorderModule.Settings.AudioCodecOverwrite;
        }

        AVCodec* videoCodec;
        if (TASRecorderModule.Settings.VideoCodecOverwrite == TASRecorderMenu.H264RgbId) {
            Log.Info("Using video codec: H.264 RGB");
            videoCodec = avcodec_find_encoder_by_name("libx264rgb");
            videoCodecID = videoCodec->id;
        } else {
            Log.Info($"Using video codec: {videoCodecID}");
            videoCodec = avcodec_find_encoder(videoCodecID);
        }
        if (videoCodec == null && videoCodecID != (int)AVCodecID.AV_CODEC_ID_NONE) {
            Log.Error("Failed to create video codec!");
        }

        Log.Info($"Using audio codec: {audioCodecID}");
        AVCodec* audioCodec = avcodec_find_encoder(audioCodecID);
        if (audioCodec == null && audioCodecID != (int)AVCodecID.AV_CODEC_ID_NONE) {
            Log.Error("Failed to create audio codec!");
        }

        HasVideo = videoCodec != null;
        HasAudio = audioCodec != null;

        if (!HasVideo && !HasAudio) return;

        if (HasVideo) AddStream(videoCodec, ref VideoStream, videoCodecID);
        if (HasAudio) AddStream(audioCodec, ref AudioStream, audioCodecID);

        if (HasVideo) OpenVideo(videoCodec);
        if (HasAudio) OpenAudio(audioCodec);

        if ((FormatCtx->oformat->flags & AVFMT_NOFILE) == 0)
            AvCheck(avio_open(&FormatCtx->pb, FilePath, AVIO_FLAG_WRITE), "Failed opening output file");
        AvCheck(avformat_write_header(FormatCtx, null), "Failed writing header to output file");
    }

    public override void End() {
        // Flush the encoders
        if (HasVideo) WriteFrame(ref VideoStream, null);
        if (HasAudio) WriteFrame(ref AudioStream, null);

        AvCheck(av_write_trailer(FormatCtx), "Failed writing stream trailer");

        if (HasVideo) CloseStream(ref VideoStream);
        if (HasAudio) CloseStream(ref AudioStream);

        if ((FormatCtx->oformat->flags & AVFMT_NOFILE) == 0)
            AvCheck(avio_closep(&FormatCtx->pb), "Failed closing file");

        avformat_free_context(FormatCtx);

        RecordingManager.MarkEncoderFinished();
    }

    public void RefreshSettings() {
        if (HasVideo) {
            var ctx = VideoStream.CodecCtx;
            ctx->framerate = av_make_q((int) (TASRecorderModule.Settings.FPS / TASRecorderModule.Settings.Speed * 1000.0f), 1000);
        }
    }

    public override void PrepareVideo(int width, int height) {
        ref var outStream = ref VideoStream;
        var ctx = outStream.CodecCtx;

        if (outStream.InFrame->width != width ||
            outStream.InFrame->height != height) {
            fixed (AVFrame** pFrame = &outStream.InFrame) {
                av_frame_free(pFrame);
            }
            outStream.InFrame = AllocateVideoFrame(INPUT_PIX_FMT, width, height);

            sws_freeContext(outStream.SwsCtx);
            outStream.SwsCtx = sws_getContext(
                width, height, INPUT_PIX_FMT,
                ctx->width, ctx->height, ctx->pix_fmt,
                SWS_BICUBIC, null, null, null);

            VideoRowStride = outStream.InFrame->linesize[0];
        }

        VideoData = outStream.InFrame->data[0];
    }
    public override void PrepareAudio(uint channelCount, uint sampleCount) {
        AudioDataSize = Math.Max(channelCount, AUDIO_CHANNEL_COUNT) * sampleCount * (uint) Marshal.SizeOf<float>();
        AudioDataSamples = sampleCount;
        AudioDataChannels = channelCount;

        if (AudioDataBufferSize < AudioDataSize) {
            AudioDataBufferSize = AudioDataSize * 2; // Avoid having to reallocate soon
            AudioData = (byte*) NativeMemory.Realloc(AudioData, AudioDataBufferSize);
        }
    }

    public override void FinishVideo() {
        ref var outStream = ref VideoStream;
        var ctx = outStream.CodecCtx;

        AvCheck(av_frame_make_writable(outStream.InFrame), "Failed making frame writable");
        AvCheck(sws_scale(outStream.SwsCtx, outStream.InFrame->data, outStream.InFrame->linesize, 0, outStream.InFrame->height,
                                    outStream.OutFrame->data, outStream.OutFrame->linesize),
                "Failed resampling video");

        outStream.OutFrame->duration = (long) (ctx->time_base.den / ctx->time_base.num / ctx->framerate.num * ctx->framerate.den / TASRecorderModule.Settings.Speed);
        outStream.VideoPTS += outStream.OutFrame->duration;
        outStream.OutFrame->pts = outStream.VideoPTS;

        WriteFrame(ref outStream, outStream.OutFrame);
    }
    public override void FinishAudio() {
        ref var outStream = ref AudioStream;
        var ctx = outStream.CodecCtx;

        float* srcData = (float*) AudioData;
        float* dstData = (float*) outStream.InFrame->data[0];

        for (int s = 0; s < AudioDataSamples; s++) {
            for (int c = 0; c < AudioDataChannels && c < ctx->ch_layout.nb_channels; c++) {
                dstData[outStream.FrameSamplePos * ctx->ch_layout.nb_channels + c] = srcData[s * AudioDataChannels + c];
            }

            outStream.FrameSamplePos++;
            if (outStream.FrameSamplePos >= outStream.InFrame->nb_samples) {
                // This frame is completely filled with data
                int dstSampleCount = (int) av_rescale_rnd(swr_get_delay(outStream.SwrCtx, ctx->sample_rate) + outStream.InFrame->nb_samples,
                                                                       ctx->sample_rate, ctx->sample_rate, AVRounding.AV_ROUND_UP);

                AvCheck(av_frame_make_writable(outStream.OutFrame), "Failed making frame writable");

                fixed (byte** pSrc = outStream.InFrame->data.ToArray(), pDst = outStream.OutFrame->data.ToArray()) {
                    AvCheck(swr_convert(outStream.SwrCtx, pDst, dstSampleCount,
                                                          pSrc, outStream.InFrame->nb_samples),
                            "Failed resampling audio data");
                }

                outStream.OutFrame->pts = av_rescale_q(outStream.SamplesCount, av_make_q(1, ctx->sample_rate), ctx->time_base);
                outStream.SamplesCount += outStream.OutFrame->nb_samples;
                outStream.FrameSamplePos = 0;

                WriteFrame(ref outStream, outStream.OutFrame);
            }
        }
    }

    private void AddStream(AVCodec* codec, ref OutputStream outStream, AVCodecID codecID) {
        outStream.Packet = av_packet_alloc();
        outStream.Stream = avformat_new_stream(FormatCtx, null);
        outStream.Stream->id = (int) FormatCtx->nb_streams - 1;
        outStream.CodecCtx = avcodec_alloc_context3(codec);

        var ctx = outStream.CodecCtx;

        switch (codec->type) {
            case AVMediaType.AVMEDIA_TYPE_VIDEO:
                ctx->codec_id = codecID;
                ctx->bit_rate = TASRecorderModule.Settings.VideoBitrate;
                ctx->width = TASRecorderModule.Settings.VideoWidth;
                ctx->height = TASRecorderModule.Settings.VideoHeight;
                ctx->time_base = av_make_q(1, 60 * 10000);
                ctx->framerate = av_make_q((int) (TASRecorderModule.Settings.FPS / TASRecorderModule.Settings.Speed * 1000.0f), 1000);
                ctx->gop_size = 12;
                ctx->pix_fmt = codec->pix_fmts != null ? codec->pix_fmts[0] : AVPixelFormat.AV_PIX_FMT_YUV420P;

                outStream.Stream->time_base = ctx->time_base;
                break;
            case AVMediaType.AVMEDIA_TYPE_AUDIO:
                ctx->sample_fmt = codec->sample_fmts != null ? codec->sample_fmts[0] : AVSampleFormat.AV_SAMPLE_FMT_FLTP;
                ctx->bit_rate = TASRecorderModule.Settings.AudioBitrate;
                ctx->sample_rate = AUDIO_SAMPLE_RATE;

                fixed (AVChannelLayout* pCh = &AV_CHANNEL_LAYOUT_STEREO) {
                    AvCheck(av_channel_layout_copy(&ctx->ch_layout, pCh), "Failed copying channel layout");
                }
                outStream.Stream->time_base = av_make_q(1, ctx->sample_rate);
                break;
        }

        if ((FormatCtx->oformat->flags & AVFMT_GLOBALHEADER) != 0) {
            ctx->flags |= AV_CODEC_FLAG_GLOBAL_HEADER;
        }
    }

    private void CloseStream(ref OutputStream outStream) {
        fixed (AVCodecContext** pCodecCtx = &outStream.CodecCtx) {
            avcodec_free_context(pCodecCtx);
        }
        fixed (AVFrame** pFrame = &outStream.InFrame) {
            av_frame_free(pFrame);
        }
        fixed (AVFrame** pFrame = &outStream.OutFrame) {
            av_frame_free(pFrame);
        }
        fixed (AVPacket** pPacket = &outStream.Packet) {
            av_packet_free(pPacket);
        }
    }

    private void OpenVideo(AVCodec* codec) {
        ref var outStream = ref VideoStream;
        var ctx = outStream.CodecCtx;

        if (codec->id == AVCodecID.AV_CODEC_ID_H264) {
            AVDictionary* options = null;
            AvCheck(av_dict_set(&options, "preset", TASRecorderModule.Settings.H264Preset, 0), "Failed setting H.264 preset");
            AvCheck(av_dict_set(&options, "crf", TASRecorderModule.Settings.H264Quality.ToString(), 0), "Failed setting H.264 quality");

            AvCheck(avcodec_open2(ctx, codec, &options), $"Could not open video codec with H.264 present: {TASRecorderModule.Settings.H264Preset} | CRF: {TASRecorderModule.Settings.H264Quality}");
        } else {
            AvCheck(avcodec_open2(ctx, codec, null), "Could not open video codec");
        }

        outStream.InFrame = AllocateVideoFrame(INPUT_PIX_FMT, ctx->width, ctx->height);
        outStream.OutFrame = AllocateVideoFrame(ctx->pix_fmt, ctx->width, ctx->height);
        outStream.FrameSamplePos = 0;
        VideoRowStride = outStream.InFrame->linesize[0];

        outStream.SwsCtx = sws_getContext(ctx->width, ctx->height, INPUT_PIX_FMT,
                                          ctx->width, ctx->height, ctx->pix_fmt,
                                          SWS_BICUBIC, null, null, null);

        AvCheck(avcodec_parameters_from_context(outStream.Stream->codecpar, ctx), "Failed copying parameters from context");
    }

    private void OpenAudio(AVCodec* codec) {
        ref var outStream = ref AudioStream;
        var ctx = outStream.CodecCtx;

        AvCheck(avcodec_open2(ctx, codec, null), "Could not open audio codec");

        int sampleCount = ctx->frame_size;
        outStream.InFrame = AllocateAudioFrame(AVSampleFormat.AV_SAMPLE_FMT_FLT, &ctx->ch_layout, ctx->sample_rate, sampleCount);
        outStream.OutFrame = AllocateAudioFrame(ctx->sample_fmt, &ctx->ch_layout, ctx->sample_rate, sampleCount);
        outStream.VideoPTS = 0;

        outStream.SwrCtx = swr_alloc();
        AvCheck(av_opt_set_chlayout(outStream.SwrCtx, "in_chlayout", &ctx->ch_layout, 0), "Failed setting in_chlayout");
        AvCheck(av_opt_set_int(outStream.SwrCtx, "in_sample_rate", ctx->sample_rate, 0), "Failed setting in_sample_rate");
        AvCheck(av_opt_set_sample_fmt(outStream.SwrCtx, "in_sample_fmt", AVSampleFormat.AV_SAMPLE_FMT_FLT, 0), "Failed setting in_sample_fmt");
        AvCheck(av_opt_set_chlayout(outStream.SwrCtx, "out_chlayout", &ctx->ch_layout, 0), "Failed setting out_chlayout");
        AvCheck(av_opt_set_int(outStream.SwrCtx, "out_sample_rate", ctx->sample_rate, 0), "Failed setting out_sample_rate");
        AvCheck(av_opt_set_sample_fmt(outStream.SwrCtx, "out_sample_fmt", ctx->sample_fmt, 0), "Failed setting out_sample_fmt");
        AvCheck(swr_init(outStream.SwrCtx), "Failed initializing resampling context");

        AvCheck(avcodec_parameters_from_context(outStream.Stream->codecpar, ctx), "Failed copying parameters from context");
    }

    private AVFrame* AllocateVideoFrame(AVPixelFormat fmt, int width, int height) {
        var frame = av_frame_alloc();
        frame->format = (int) fmt;
        frame->width = width;
        frame->height = height;

        AvCheck(av_frame_get_buffer(frame, 0), "Failed allocating frame buffer");

        return frame;
    }

    private AVFrame* AllocateAudioFrame(AVSampleFormat fmt, AVChannelLayout* chLayout, int sampleRate, int sampleCount) {
        var frame = av_frame_alloc();
        frame->format = (int) fmt;
        frame->sample_rate = sampleRate;
        frame->nb_samples = sampleCount;

        AvCheck(av_channel_layout_copy(&frame->ch_layout, chLayout), "Failed copying channel layout");
        AvCheck(av_frame_get_buffer(frame, 0), "Failed allocating frame buffer");

        return frame;
    }

    private void WriteFrame(ref OutputStream outStream, AVFrame* frame) {
        AvCheck(avcodec_send_frame(outStream.CodecCtx, frame), "Error sending the frame to the encoder");

        // Read all the available output packets (in general there may be any number of them)
        while (true) {
            int result = avcodec_receive_packet(outStream.CodecCtx, outStream.Packet);

            if (result == AVERROR(EAGAIN) || result == AVERROR_EOF)
                return;

            AvCheck(result, "Failed encoding frame");

            if (frame != null)
                outStream.Packet->duration = frame->duration;

            av_packet_rescale_ts(outStream.Packet, outStream.CodecCtx->time_base, outStream.Stream->time_base);
            outStream.Packet->stream_index = outStream.Stream->index;
            AvCheck(av_interleaved_write_frame(FormatCtx, outStream.Packet), "Failed writing output packet");
        }
    }

    private void AvCheck(int errorCode, string errorMessage) {
        if (errorCode >= 0) return;

        byte[] buffer = new byte[256];
        nint ptr;
        fixed (byte* pBuffer = buffer) {
            if (av_strerror(errorCode, pBuffer, 256) < 0) {
                Log.Error($"FFmpeg Error - {errorMessage}: [{errorMessage}]");
                return;
            }

            ptr = (nint) pBuffer;
        }

        string? error = Marshal.PtrToStringUTF8(ptr);
        Log.Error($"FFmpeg Error - {errorMessage}: {error} [{errorCode}] ");
    }
}
