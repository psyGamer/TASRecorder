using System;
using System.IO;
using System.Runtime.InteropServices;
using FFmpeg;
using static FFmpeg.ffmpeg;

namespace Celeste.Mod.Capture;

internal unsafe struct OutputStream {
    public AVStream* Stream;
    public AVCodecContext* CodecCtx;

    public AVFrame* InFrame; // For writing data into it
    public AVFrame* OutFrame; //  For writing to a file

    public AVPacket* Packet;

    public SwsContext* SwsCtx;
    public SwrContext* SwrCtx;

    public int FrameCount;
    public int SamplesCount;
    public int FrameSamplePos;
}

public unsafe class Encoder {
    public const string RECORDING_DIRECTORY = "CaptureRecordings";

    // Input data format from the VideoCapture
    public const AVPixelFormat INPUT_PIX_FMT = AVPixelFormat.AV_PIX_FMT_RGBA;
    // Celeste only works well with this value
    public const int AUDIO_SAMPLE_RATE = 48000;
    // We only record in stereo, since there's no point in going higher.
    public const int AUDIO_CHANNEL_COUNT = 2;

    string FilePath;

    public byte* VideoData;
    public int VideoRowStride;

    public byte* AudioData;
    uint AudioDataChannels;
    uint AudioDataSamples;
    uint AudioDataSize;
    uint AudioDataBufferSize;

    public bool HasVideo;
    public bool HasAudio;
    OutputStream VideoStream;
    OutputStream AudioStream;
    AVFormatContext* FormatCtx;

    public Encoder() {
        this.VideoData = null;
        this.VideoRowStride = 0;

        this.AudioData = null;
        this.AudioDataChannels = 0;
        this.AudioDataSamples = 0;
        this.AudioDataSize = 0;
        this.AudioDataBufferSize = 0;

        this.FormatCtx = null;
        this.VideoStream = default(OutputStream);
        this.AudioStream = default(OutputStream);

        var now = DateTime.Now;
        this.FilePath = $"{RECORDING_DIRECTORY}/{now.ToString("dd-MM-yyyy_HH-mm-ss")}.{CaptureModule.Settings.ContainerType}";

        if (!Directory.Exists(RECORDING_DIRECTORY)) {
            Directory.CreateDirectory(RECORDING_DIRECTORY);
        }

        fixed(AVFormatContext** pFmtCtx = &this.FormatCtx) {
            AvCheck(avformat_alloc_output_context2(pFmtCtx, null, null, this.FilePath), "Failed allocating format context");
        }

        AVCodecID videoCodecID = this.FormatCtx->oformat->video_codec;
        AVCodecID audioCodecID = this.FormatCtx->oformat->audio_codec;

        AVCodec* videoCodec = null;
        if (CaptureModule.Settings.VideoCodecOverwrite != -1) {
            videoCodec = avcodec_find_encoder((AVCodecID) CaptureModule.Settings.VideoCodecOverwrite);
        } else if (videoCodecID != AVCodecID.AV_CODEC_ID_NONE) {
            videoCodec = avcodec_find_encoder(videoCodecID);
        }

        AVCodec* audioCodec = null;
        if (CaptureModule.Settings.AudioCodecOverwrite != -1) {
            videoCodec = avcodec_find_encoder((AVCodecID) CaptureModule.Settings.AudioCodecOverwrite);
        } else if (audioCodecID != AVCodecID.AV_CODEC_ID_NONE) {
            audioCodec = avcodec_find_encoder(audioCodecID);
        }

        this.HasVideo = videoCodec != null;
        this.HasAudio = audioCodec != null;

        if (!HasVideo && !HasAudio) return;

        if (this.HasVideo) AddStream(videoCodec, ref this.VideoStream, videoCodecID);
        if (this.HasAudio) AddStream(audioCodec, ref this.AudioStream, audioCodecID);

        if (this.HasVideo) OpenVideo(videoCodec);
        if (this.HasAudio) OpenAudio(audioCodec);

        if ((this.FormatCtx->oformat->flags & AVFMT_NOFILE) == 0)
            AvCheck(avio_open(&this.FormatCtx->pb, this.FilePath, AVIO_FLAG_WRITE), "Failed opening output file");
        AvCheck(avformat_write_header(this.FormatCtx, null), "Failed writing header to output file");
    }

    public void End() {
        // Flush the encoders
        if (this.HasVideo) WriteFrame(ref this.VideoStream, null);
        if (this.HasAudio) WriteFrame(ref this.AudioStream, null);

        av_write_trailer(this.FormatCtx);

        if (this.HasVideo) CloseStream(ref this.VideoStream);
        if (this.HasAudio) CloseStream(ref this.AudioStream);

        if ((this.FormatCtx->oformat->flags & AVFMT_NOFILE) == 0)
            avio_closep(&this.FormatCtx->pb);

        avformat_free_context(this.FormatCtx);
    }

    public void PrepareVideo(int width, int height) {
        ref OutputStream outStream = ref this.VideoStream;
        AVCodecContext* ctx = outStream.CodecCtx;

        if (outStream.InFrame->width != width ||
            outStream.InFrame->height != height)
        {
            fixed (AVFrame** pFrame = &outStream.InFrame) {
                av_frame_free(pFrame);
            }
            outStream.InFrame = AllocateVideoFrame(INPUT_PIX_FMT, width, height);

            sws_freeContext(outStream.SwsCtx);
            outStream.SwsCtx = sws_getContext(
                width, height, INPUT_PIX_FMT,
                ctx->width, ctx->height, ctx->pix_fmt,
                SWS_BICUBIC, null, null, null);

            this.VideoRowStride = outStream.InFrame->linesize[0];
        }

        this.VideoData = outStream.InFrame->data[0];
    }
    public void PrepareAudio(uint channelCount, uint sampleCount) {
        ref OutputStream outStream = ref this.AudioStream;
        AVCodecContext* ctx = outStream.CodecCtx;

        this.AudioDataSize = Math.Max(channelCount, AUDIO_CHANNEL_COUNT) * sampleCount * (uint)Marshal.SizeOf<float>();
        this.AudioDataSamples = sampleCount;
        this.AudioDataChannels = channelCount;

        if (this.AudioDataBufferSize < this.AudioDataSize)
        {
            this.AudioDataBufferSize = this.AudioDataSize * 2; // Avoid having to reallocate soon
            this.AudioData = (byte*)NativeMemory.Realloc(this.AudioData, this.AudioDataBufferSize);
        }
    }

    public void FinishVideo() {
        ref OutputStream outStream = ref this.VideoStream;
        AVCodecContext* ctx = outStream.CodecCtx;

        AvCheck(av_frame_make_writable(outStream.InFrame), "Failed making frame writable");
        AvCheck(sws_scale(outStream.SwsCtx, outStream.InFrame->data,  outStream.InFrame->linesize, 0, outStream.InFrame->height,
                                    outStream.OutFrame->data, outStream.OutFrame->linesize),
                "Failed resampling video");

        outStream.OutFrame->duration = ctx->time_base.den / ctx->time_base.num / ctx->framerate.num * ctx->framerate.den;
        outStream.OutFrame->pts = outStream.FrameCount++ * outStream.OutFrame->duration;

        WriteFrame(ref outStream, outStream.OutFrame);
    }
    public void FinishAudio() {
        ref OutputStream outStream = ref this.AudioStream;
        AVCodecContext* ctx = outStream.CodecCtx;

        float* srcData = (float*)this.AudioData;
        float* dstData = (float*)outStream.InFrame->data[0];

        for (int s = 0; s < this.AudioDataSamples; s++)
        {
            for (int c = 0; c < this.AudioDataChannels && c < ctx->ch_layout.nb_channels; c++)
            {
                dstData[outStream.FrameSamplePos * ctx->ch_layout.nb_channels + c] = srcData[s * this.AudioDataChannels + c];
            }

            outStream.FrameSamplePos++;
            if (outStream.FrameSamplePos >= outStream.InFrame->nb_samples)
            {
                // This frame is completely filled with data
                int dstSampleCount = (int)av_rescale_rnd(swr_get_delay(outStream.SwrCtx, ctx->sample_rate) + outStream.InFrame->nb_samples,
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
        outStream.Stream = avformat_new_stream(this.FormatCtx, null);
        outStream.Stream->id = (int)this.FormatCtx->nb_streams - 1;
        outStream.CodecCtx = avcodec_alloc_context3(codec);

        AVCodecContext* ctx = outStream.CodecCtx;

        switch (codec->type)
        {
        case AVMediaType.AVMEDIA_TYPE_VIDEO:
            ctx->codec_id = codecID;
            ctx->bit_rate = CaptureModule.Settings.VideoBitrate;
            ctx->width = CaptureModule.Settings.VideoWidth;
            ctx->height = CaptureModule.Settings.VideoHeight;
            ctx->time_base = av_make_q(1, CaptureModule.Settings.FPS * 10000);
            ctx->framerate = av_make_q(CaptureModule.Settings.FPS, 1 );
            ctx->gop_size = 12;
            ctx->pix_fmt = codec->pix_fmts != null ? codec->pix_fmts[0] : AVPixelFormat.AV_PIX_FMT_YUV420P;

            outStream.Stream->time_base = ctx->time_base;
            break;
        case AVMediaType.AVMEDIA_TYPE_AUDIO:
            ctx->sample_fmt = codec->sample_fmts != null ? codec->sample_fmts[0] : AVSampleFormat.AV_SAMPLE_FMT_FLTP;
            ctx->bit_rate = CaptureModule.Settings.AudioBitrate;
            ctx->sample_rate = AUDIO_SAMPLE_RATE;

            fixed (AVChannelLayout* pCh = &AV_CHANNEL_LAYOUT_STEREO) {
                AvCheck(av_channel_layout_copy(&ctx->ch_layout, pCh), "Failed copying channel layout");
            }
            outStream.Stream->time_base = av_make_q(1, ctx->sample_rate);
            break;
        default:
            break;
        }

        if ((this.FormatCtx->oformat->flags & AVFMT_GLOBALHEADER) != 0) {
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
        ref OutputStream outStream = ref this.VideoStream;
        AVCodecContext* ctx = outStream.CodecCtx;

        if (codec->id == AVCodecID.AV_CODEC_ID_H264) {
            AVDictionary* options = null;
            av_dict_set(&options, "preset", CaptureModule.Settings.H264Preset, 0);

            AvCheck(avcodec_open2(ctx, codec, &options), "Could not open video codec");
        } else {
            AvCheck(avcodec_open2(ctx, codec, null), "Could not open video codec");
        }

        outStream.InFrame = AllocateVideoFrame(INPUT_PIX_FMT, ctx->width, ctx->height);
        outStream.OutFrame = AllocateVideoFrame(ctx->pix_fmt, ctx->width, ctx->height);
        outStream.FrameSamplePos = 0;
        this.VideoRowStride = outStream.InFrame->linesize[0];

        outStream.SwsCtx = sws_getContext(ctx->width, ctx->height, INPUT_PIX_FMT,
                                          ctx->width, ctx->height, ctx->pix_fmt,
                                          SWS_BICUBIC, null, null, null);

        AvCheck(avcodec_parameters_from_context(outStream.Stream->codecpar, ctx), "Failed copying parameters from context");
    }

    private void OpenAudio(AVCodec* codec) {
        ref OutputStream outStream = ref this.AudioStream;
        AVCodecContext* ctx = outStream.CodecCtx;

        // TODO: Codec options
        // i32 codecOptionsLength = settings_audio_codec_options_length();

        // if (codecOptionsLength > 0)
        // {
        //     codec_option_t* codecOptions = settings_audio_codec_options();
        //     AVDictionary* options = { 0 };
        //     for (i32 i = 0; i < codecOptionsLength; i++)
        //     {
        //         codec_option_t* option = &codecOptions[i];
        //         av_dict_set(&options, option->name, option->value, 0);
        //     }

        //     AVCHECK(avcodec_open2(ctx, codec, &options), "Could not open audio codec");
        // }
        // else
        // {
        AvCheck(avcodec_open2(ctx, codec, null), "Could not open audio codec");
        // }

        int sampleCount = ctx->frame_size;
        outStream.InFrame  = AllocateAudioFrame(AVSampleFormat.AV_SAMPLE_FMT_FLT, &ctx->ch_layout, ctx->sample_rate, sampleCount);
        outStream.OutFrame = AllocateAudioFrame(ctx->sample_fmt, &ctx->ch_layout, ctx->sample_rate, sampleCount);
        outStream.FrameCount = 0;

        outStream.SwrCtx = swr_alloc();
        av_opt_set_chlayout  (outStream.SwrCtx, "in_chlayout",     &ctx->ch_layout,    0);
        av_opt_set_int       (outStream.SwrCtx, "in_sample_rate",   ctx->sample_rate,  0);
        av_opt_set_sample_fmt(outStream.SwrCtx, "in_sample_fmt",    AVSampleFormat.AV_SAMPLE_FMT_FLT, 0);
        av_opt_set_chlayout  (outStream.SwrCtx, "out_chlayout",    &ctx->ch_layout,    0);
        av_opt_set_int       (outStream.SwrCtx, "out_sample_rate",  ctx->sample_rate,  0);
        av_opt_set_sample_fmt(outStream.SwrCtx, "out_sample_fmt",   ctx->sample_fmt,   0);
        AvCheck(swr_init(outStream.SwrCtx), "Failed initializing resampling context");

        AvCheck(avcodec_parameters_from_context(outStream.Stream->codecpar, ctx), "Failed copying parameters from context");
    }

    private AVFrame* AllocateVideoFrame(AVPixelFormat fmt, int width, int height) {
        AVFrame* frame = av_frame_alloc();
        frame->format = (int)fmt;
        frame->width = width;
        frame->height = height;

        AvCheck(av_frame_get_buffer(frame, 0), "Failed allocating frame buffer");

        return frame;
    }

    private AVFrame* AllocateAudioFrame(AVSampleFormat fmt, AVChannelLayout* chLayout, int sampleRate, int sampleCount) {
        AVFrame* frame = av_frame_alloc();
        frame->format = (int)fmt;
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
            AvCheck(av_interleaved_write_frame(this.FormatCtx, outStream.Packet), "Failed writing output packet");
        }
    }

    private void AvCheck(int errorCode, string errorMessage) {
        if (errorCode >= 0) return;

        byte[] buffer = new byte[256];
        nint ptr;
        fixed (byte* pBuffer = buffer) {
            av_strerror(errorCode, pBuffer, 256);
            ptr = (nint)pBuffer;
        }

        string error = Marshal.PtrToStringUTF8(ptr);
        Logger.Log(LogLevel.Error, CaptureModule.NAME, $"{errorMessage}: {error} [{errorCode}] ");
    }
}
