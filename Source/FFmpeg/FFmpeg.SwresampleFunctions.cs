#pragma warning disable

using FFmpeg.Util;
using System;
using System.Runtime.InteropServices;

namespace FFmpeg;

public static unsafe partial class FFmpeg {
    /// <summary>Allocate SwrContext.</summary>
    /// <returns>NULL on error, allocated context otherwise</returns>
    [DllImport("swresample", CallingConvention = CallingConvention.Cdecl)]
    public static extern SwrContext* swr_alloc();

    /// <summary>Allocate SwrContext if needed and set/reset common parameters.</summary>
    /// <param name="s">existing Swr context if available, or NULL if not</param>
    /// <param name="out_ch_layout">output channel layout (AV_CH_LAYOUT_*)</param>
    /// <param name="out_sample_fmt">output sample format (AV_SAMPLE_FMT_*).</param>
    /// <param name="out_sample_rate">output sample rate (frequency in Hz)</param>
    /// <param name="in_ch_layout">input channel layout (AV_CH_LAYOUT_*)</param>
    /// <param name="in_sample_fmt">input sample format (AV_SAMPLE_FMT_*).</param>
    /// <param name="in_sample_rate">input sample rate (frequency in Hz)</param>
    /// <param name="log_offset">logging level offset</param>
    /// <param name="log_ctx">parent logging context, can be NULL</param>
    /// <returns>NULL on error, allocated context otherwise</returns>
    [Obsolete("use ")]
    [DllImport("swresample", CallingConvention = CallingConvention.Cdecl)]
    public static extern SwrContext* swr_alloc_set_opts(SwrContext* @s, long @out_ch_layout, AVSampleFormat @out_sample_fmt, int @out_sample_rate, long @in_ch_layout, AVSampleFormat @in_sample_fmt, int @in_sample_rate, int @log_offset, void* @log_ctx);

    /// <summary>Allocate SwrContext if needed and set/reset common parameters.</summary>
    /// <param name="ps">Pointer to an existing Swr context if available, or to NULL if not. On success, *ps will be set to the allocated context.</param>
    /// <param name="out_ch_layout">output channel layout (e.g. AV_CHANNEL_LAYOUT_*)</param>
    /// <param name="out_sample_fmt">output sample format (AV_SAMPLE_FMT_*).</param>
    /// <param name="out_sample_rate">output sample rate (frequency in Hz)</param>
    /// <param name="in_ch_layout">input channel layout (e.g. AV_CHANNEL_LAYOUT_*)</param>
    /// <param name="in_sample_fmt">input sample format (AV_SAMPLE_FMT_*).</param>
    /// <param name="in_sample_rate">input sample rate (frequency in Hz)</param>
    /// <param name="log_offset">logging level offset</param>
    /// <param name="log_ctx">parent logging context, can be NULL</param>
    /// <returns>0 on success, a negative AVERROR code on error. On error, the Swr context is freed and *ps set to NULL.</returns>
    [DllImport("swresample", CallingConvention = CallingConvention.Cdecl)]
    public static extern int swr_alloc_set_opts2(SwrContext** @ps, AVChannelLayout* @out_ch_layout, AVSampleFormat @out_sample_fmt, int @out_sample_rate, AVChannelLayout* @in_ch_layout, AVSampleFormat @in_sample_fmt, int @in_sample_rate, int @log_offset, void* @log_ctx);

    /// <summary>Generate a channel mixing matrix.</summary>
    /// <param name="in_layout">input channel layout</param>
    /// <param name="out_layout">output channel layout</param>
    /// <param name="center_mix_level">mix level for the center channel</param>
    /// <param name="surround_mix_level">mix level for the surround channel(s)</param>
    /// <param name="lfe_mix_level">mix level for the low-frequency effects channel</param>
    /// <param name="rematrix_maxval">if 1.0, coefficients will be normalized to prevent overflow. if INT_MAX, coefficients will not be normalized.</param>
    /// <param name="matrix">mixing coefficients; matrix[i + stride * o] is the weight of input channel i in output channel o.</param>
    /// <param name="stride">distance between adjacent input channels in the matrix array</param>
    /// <param name="matrix_encoding">matrixed stereo downmix mode (e.g. dplii)</param>
    /// <param name="log_ctx">parent logging context, can be NULL</param>
    /// <returns>0 on success, negative AVERROR code on failure</returns>
    [Obsolete("use ")]
    [DllImport("swresample", CallingConvention = CallingConvention.Cdecl)]
    public static extern int swr_build_matrix(ulong @in_layout, ulong @out_layout, double @center_mix_level, double @surround_mix_level, double @lfe_mix_level, double @rematrix_maxval, double @rematrix_volume, double* @matrix, int @stride, AVMatrixEncoding @matrix_encoding, void* @log_ctx);

    /// <summary>Generate a channel mixing matrix.</summary>
    /// <param name="in_layout">input channel layout</param>
    /// <param name="out_layout">output channel layout</param>
    /// <param name="center_mix_level">mix level for the center channel</param>
    /// <param name="surround_mix_level">mix level for the surround channel(s)</param>
    /// <param name="lfe_mix_level">mix level for the low-frequency effects channel</param>
    /// <param name="matrix">mixing coefficients; matrix[i + stride * o] is the weight of input channel i in output channel o.</param>
    /// <param name="stride">distance between adjacent input channels in the matrix array</param>
    /// <param name="matrix_encoding">matrixed stereo downmix mode (e.g. dplii)</param>
    /// <returns>0 on success, negative AVERROR code on failure</returns>
    [DllImport("swresample", CallingConvention = CallingConvention.Cdecl)]
    public static extern int swr_build_matrix2(AVChannelLayout* @in_layout, AVChannelLayout* @out_layout, double @center_mix_level, double @surround_mix_level, double @lfe_mix_level, double @maxval, double @rematrix_volume, double* @matrix, long @stride, AVMatrixEncoding @matrix_encoding, void* @log_context);

    /// <summary>Closes the context so that swr_is_initialized() returns 0.</summary>
    /// <param name="s">Swr context to be closed</param>
    [DllImport("swresample", CallingConvention = CallingConvention.Cdecl)]
    public static extern void swr_close(SwrContext* @s);

    /// <summary>Configure or reconfigure the SwrContext using the information provided by the AVFrames.</summary>
    /// <param name="swr">audio resample context</param>
    /// <param name="out">output AVFrame</param>
    /// <param name="in">input AVFrame</param>
    /// <returns>0 on success, AVERROR on failure.</returns>
    [DllImport("swresample", CallingConvention = CallingConvention.Cdecl)]
    public static extern int swr_config_frame(SwrContext* @swr, AVFrame* @out, AVFrame* @in);

    /// <summary>Convert audio.</summary>
    /// <param name="s">allocated Swr context, with parameters set</param>
    /// <param name="out">output buffers, only the first one need be set in case of packed audio</param>
    /// <param name="out_count">amount of space available for output in samples per channel</param>
    /// <param name="in">input buffers, only the first one need to be set in case of packed audio</param>
    /// <param name="in_count">number of input samples available in one channel</param>
    /// <returns>number of samples output per channel, negative value on error</returns>
    [DllImport("swresample", CallingConvention = CallingConvention.Cdecl)]
    public static extern int swr_convert(SwrContext* @s, byte** @out, int @out_count, byte** @in, int @in_count);

    /// <summary>Convert the samples in the input AVFrame and write them to the output AVFrame.</summary>
    /// <param name="swr">audio resample context</param>
    /// <param name="output">output AVFrame</param>
    /// <param name="input">input AVFrame</param>
    /// <returns>0 on success, AVERROR on failure or nonmatching configuration.</returns>
    [DllImport("swresample", CallingConvention = CallingConvention.Cdecl)]
    public static extern int swr_convert_frame(SwrContext* @swr, AVFrame* @output, AVFrame* @input);

    /// <summary>Drops the specified number of output samples.</summary>
    /// <param name="s">allocated Swr context</param>
    /// <param name="count">number of samples to be dropped</param>
    /// <returns>&gt;= 0 on success, or a negative AVERROR code on failure</returns>
    [DllImport("swresample", CallingConvention = CallingConvention.Cdecl)]
    public static extern int swr_drop_output(SwrContext* @s, int @count);

    /// <summary>Free the given SwrContext and set the pointer to NULL.</summary>
    /// <param name="s">a pointer to a pointer to Swr context</param>
    [DllImport("swresample", CallingConvention = CallingConvention.Cdecl)]
    public static extern void swr_free(SwrContext** @s);

    /// <summary>Get the AVClass for SwrContext. It can be used in combination with AV_OPT_SEARCH_FAKE_OBJ for examining options.</summary>
    /// <returns>the AVClass of SwrContext</returns>
    [DllImport("swresample", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVClass* swr_get_class();

    /// <summary>Gets the delay the next input sample will experience relative to the next output sample.</summary>
    /// <param name="s">swr context</param>
    /// <param name="base">timebase in which the returned delay will be:</param>
    [DllImport("swresample", CallingConvention = CallingConvention.Cdecl)]
    public static extern long swr_get_delay(SwrContext* @s, long @base);

    /// <summary>Find an upper bound on the number of samples that the next swr_convert call will output, if called with in_samples of input samples. This depends on the internal state, and anything changing the internal state (like further swr_convert() calls) will may change the number of samples swr_get_out_samples() returns for the same number of input samples.</summary>
    /// <param name="in_samples">number of input samples.</param>
    [DllImport("swresample", CallingConvention = CallingConvention.Cdecl)]
    public static extern int swr_get_out_samples(SwrContext* @s, int @in_samples);

    /// <summary>Initialize context after user parameters have been set.</summary>
    /// <param name="s">Swr context to initialize</param>
    /// <returns>AVERROR error code in case of failure.</returns>
    [DllImport("swresample", CallingConvention = CallingConvention.Cdecl)]
    public static extern int swr_init(SwrContext* @s);

    /// <summary>Injects the specified number of silence samples.</summary>
    /// <param name="s">allocated Swr context</param>
    /// <param name="count">number of samples to be dropped</param>
    /// <returns>&gt;= 0 on success, or a negative AVERROR code on failure</returns>
    [DllImport("swresample", CallingConvention = CallingConvention.Cdecl)]
    public static extern int swr_inject_silence(SwrContext* @s, int @count);

    /// <summary>Check whether an swr context has been initialized or not.</summary>
    /// <param name="s">Swr context to check</param>
    /// <returns>positive if it has been initialized, 0 if not initialized</returns>
    [DllImport("swresample", CallingConvention = CallingConvention.Cdecl)]
    public static extern int swr_is_initialized(SwrContext* @s);

    /// <summary>Convert the next timestamp from input to output timestamps are in 1/(in_sample_rate * out_sample_rate) units.</summary>
    /// <param name="s">initialized Swr context</param>
    /// <param name="pts">timestamp for the next input sample, INT64_MIN if unknown</param>
    /// <returns>the output timestamp for the next output sample</returns>
    [DllImport("swresample", CallingConvention = CallingConvention.Cdecl)]
    public static extern long swr_next_pts(SwrContext* @s, long @pts);

    /// <summary>Set a customized input channel mapping.</summary>
    /// <param name="s">allocated Swr context, not yet initialized</param>
    /// <param name="channel_map">customized input channel mapping (array of channel indexes, -1 for a muted channel)</param>
    /// <returns>&gt;= 0 on success, or AVERROR error code in case of failure.</returns>
    [DllImport("swresample", CallingConvention = CallingConvention.Cdecl)]
    public static extern int swr_set_channel_mapping(SwrContext* @s, int* @channel_map);

    /// <summary>Activate resampling compensation (&quot;soft&quot; compensation). This function is internally called when needed in swr_next_pts().</summary>
    /// <param name="s">allocated Swr context. If it is not initialized, or SWR_FLAG_RESAMPLE is not set, swr_init() is called with the flag set.</param>
    /// <param name="sample_delta">delta in PTS per sample</param>
    /// <param name="compensation_distance">number of samples to compensate for</param>
    /// <returns>&gt;= 0 on success, AVERROR error codes if:</returns>
    [DllImport("swresample", CallingConvention = CallingConvention.Cdecl)]
    public static extern int swr_set_compensation(SwrContext* @s, int @sample_delta, int @compensation_distance);

    /// <summary>Set a customized remix matrix.</summary>
    /// <param name="s">allocated Swr context, not yet initialized</param>
    /// <param name="matrix">remix coefficients; matrix[i + stride * o] is the weight of input channel i in output channel o</param>
    /// <param name="stride">offset between lines of the matrix</param>
    /// <returns>&gt;= 0 on success, or AVERROR error code in case of failure.</returns>
    [DllImport("swresample", CallingConvention = CallingConvention.Cdecl)]
    public static extern int swr_set_matrix(SwrContext* @s, double* @matrix, int @stride);

    /// <summary>Return the swr build-time configuration.</summary>
    [DllImport("swresample", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public static extern string swresample_configuration();

    /// <summary>Return the swr license.</summary>
    [DllImport("swresample", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public static extern string swresample_license();

    /// <summary>Return the LIBSWRESAMPLE_VERSION_INT constant.</summary>
    [DllImport("swresample", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint swresample_version();
}
