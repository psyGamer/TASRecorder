using System;
using System.Runtime.InteropServices;
using FFmpeg.Util;

namespace FFmpeg;

public static unsafe partial class FFmpeg {
    /// <summary>Get the AVCodecID for the given codec tag tag. If no codec id is found returns AV_CODEC_ID_NONE.</summary>
    /// <param name="tags">list of supported codec_id-codec_tag pairs, as stored in AVInputFormat.codec_tag and AVOutputFormat.codec_tag</param>
    /// <param name="tag">codec tag to match to a codec ID</param>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVCodecID av_codec_get_id(AVCodecTag** @tags, uint @tag);

    /// <summary>Get the codec tag for the given codec id id. If no codec tag is found returns 0.</summary>
    /// <param name="tags">list of supported codec_id-codec_tag pairs, as stored in AVInputFormat.codec_tag and AVOutputFormat.codec_tag</param>
    /// <param name="id">codec ID to match to a codec tag</param>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint av_codec_get_tag(AVCodecTag** @tags, AVCodecID @id);

    /// <summary>Get the codec tag for the given codec id.</summary>
    /// <param name="tags">list of supported codec_id - codec_tag pairs, as stored in AVInputFormat.codec_tag and AVOutputFormat.codec_tag</param>
    /// <param name="id">codec id that should be searched for in the list</param>
    /// <param name="tag">A pointer to the found tag</param>
    /// <returns>0 if id was not found in tags, &gt; 0 if it was found</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_codec_get_tag2(AVCodecTag** @tags, AVCodecID @id, uint* @tag);

    /// <summary>Iterate over all registered demuxers.</summary>
    /// <param name="opaque">a pointer where libavformat will store the iteration state. Must point to NULL to start the iteration.</param>
    /// <returns>the next registered demuxer or NULL when the iteration is finished</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVInputFormat* av_demuxer_iterate(void** @opaque);

    /// <summary>Returns The AV_DISPOSITION_* flag corresponding to disp or a negative error code if disp does not correspond to a known stream disposition.</summary>
    /// <returns>The AV_DISPOSITION_* flag corresponding to disp or a negative error code if disp does not correspond to a known stream disposition.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_disposition_from_string(
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @disp);

    /// <summary>Returns The string description corresponding to the lowest set bit in disposition. NULL when the lowest set bit does not correspond to a known disposition or when disposition is 0.</summary>
    /// <param name="disposition">a combination of AV_DISPOSITION_* values</param>
    /// <returns>The string description corresponding to the lowest set bit in disposition. NULL when the lowest set bit does not correspond to a known disposition or when disposition is 0.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public static extern string av_disposition_to_string(int @disposition);

    /// <summary>Print detailed information about the input or output format, such as duration, bitrate, streams, container, programs, metadata, side data, codec and time base.</summary>
    /// <param name="ic">the context to analyze</param>
    /// <param name="index">index of the stream to dump information about</param>
    /// <param name="url">the URL to print, such as source or destination file</param>
    /// <param name="is_output">Select whether the specified context is an input(0) or output(1)</param>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_dump_format(AVFormatContext* @ic, int @index,
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @url, int @is_output);

    /// <summary>Check whether filename actually is a numbered sequence generator.</summary>
    /// <param name="filename">possible numbered sequence string</param>
    /// <returns>1 if a valid numbered sequence string, 0 otherwise</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_filename_number_test(
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @filename);

    /// <summary>Find the &quot;best&quot; stream in the file. The best stream is determined according to various heuristics as the most likely to be what the user expects. If the decoder parameter is non-NULL, av_find_best_stream will find the default decoder for the stream&apos;s codec; streams for which no decoder can be found are ignored.</summary>
    /// <param name="ic">media file handle</param>
    /// <param name="type">stream type: video, audio, subtitles, etc.</param>
    /// <param name="wanted_stream_nb">user-requested stream number, or -1 for automatic selection</param>
    /// <param name="related_stream">try to find a stream related (eg. in the same program) to this one, or -1 if none</param>
    /// <param name="decoder_ret">if non-NULL, returns the decoder for the selected stream</param>
    /// <param name="flags">flags; none are currently defined</param>
    /// <returns>the non-negative stream number in case of success, AVERROR_STREAM_NOT_FOUND if no stream with the requested type could be found, AVERROR_DECODER_NOT_FOUND if streams were found but no decoder</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_find_best_stream(AVFormatContext* @ic, AVMediaType @type, int @wanted_stream_nb, int @related_stream, AVCodec** @decoder_ret, int @flags);

    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_find_default_stream_index(AVFormatContext* @s);

    /// <summary>Find AVInputFormat based on the short name of the input format.</summary>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVInputFormat* av_find_input_format(
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @short_name);

    /// <summary>Find the programs which belong to a given stream.</summary>
    /// <param name="ic">media file handle</param>
    /// <param name="last">the last found program, the search will start after this program, or from the beginning if it is NULL</param>
    /// <param name="s">stream index</param>
    /// <returns>the next program which belongs to s, NULL if no program is found or the last program is not among the programs of ic.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVProgram* av_find_program_from_stream(AVFormatContext* @ic, AVProgram* @last, int @s);

    /// <summary>Returns the method used to set ctx-&gt;duration.</summary>
    /// <returns>AVFMT_DURATION_FROM_PTS, AVFMT_DURATION_FROM_STREAM, or AVFMT_DURATION_FROM_BITRATE.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVDurationEstimationMethod av_fmt_ctx_get_duration_estimation_method(AVFormatContext* @ctx);

    /// <summary>This function will cause global side data to be injected in the next packet of each stream as well as after any subsequent seek.</summary>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_format_inject_global_side_data(AVFormatContext* @s);

    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_get_frame_filename(byte* @buf, int @buf_size,
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @path, int @number);

    /// <summary>Return in &apos;buf&apos; the path with &apos;%d&apos; replaced by a number.</summary>
    /// <param name="buf">destination buffer</param>
    /// <param name="buf_size">destination buffer size</param>
    /// <param name="path">numbered sequence string</param>
    /// <param name="number">frame number</param>
    /// <param name="flags">AV_FRAME_FILENAME_FLAGS_*</param>
    /// <returns>0 if OK, -1 on format error</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_get_frame_filename2(byte* @buf, int @buf_size,
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @path, int @number, int @flags);

    /// <summary>Get timing information for the data currently output. The exact meaning of &quot;currently output&quot; depends on the format. It is mostly relevant for devices that have an internal buffer and/or work in real time.</summary>
    /// <param name="s">media file handle</param>
    /// <param name="stream">stream in the media file</param>
    /// <param name="dts">DTS of the last packet output for the stream, in stream time_base units</param>
    /// <param name="wall">absolute time when that packet whas output, in microsecond</param>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_get_output_timestamp(AVFormatContext* @s, int @stream, long* @dts, long* @wall);

    /// <summary>Allocate and read the payload of a packet and initialize its fields with default values.</summary>
    /// <param name="s">associated IO context</param>
    /// <param name="pkt">packet</param>
    /// <param name="size">desired payload size</param>
    /// <returns>&gt;0 (read size) if OK, AVERROR_xxx otherwise</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_get_packet(AVIOContext* @s, AVPacket* @pkt, int @size);

    /// <summary>Guess the codec ID based upon muxer and filename.</summary>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVCodecID av_guess_codec(AVOutputFormat* @fmt,
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @short_name,
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @filename,
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @mime_type, AVMediaType @type);

    /// <summary>Return the output format in the list of registered output formats which best matches the provided parameters, or return NULL if there is no match.</summary>
    /// <param name="short_name">if non-NULL checks if short_name matches with the names of the registered formats</param>
    /// <param name="filename">if non-NULL checks if filename terminates with the extensions of the registered formats</param>
    /// <param name="mime_type">if non-NULL checks if mime_type matches with the MIME type of the registered formats</param>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVOutputFormat* av_guess_format(
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @short_name,
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @filename,
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @mime_type);

    /// <summary>Guess the frame rate, based on both the container and codec information.</summary>
    /// <param name="ctx">the format context which the stream is part of</param>
    /// <param name="stream">the stream which the frame is part of</param>
    /// <param name="frame">the frame for which the frame rate should be determined, may be NULL</param>
    /// <returns>the guessed (valid) frame rate, 0/1 if no idea</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVRational av_guess_frame_rate(AVFormatContext* @ctx, AVStream* @stream, AVFrame* @frame);

    /// <summary>Guess the sample aspect ratio of a frame, based on both the stream and the frame aspect ratio.</summary>
    /// <param name="format">the format context which the stream is part of</param>
    /// <param name="stream">the stream which the frame is part of</param>
    /// <param name="frame">the frame with the aspect ratio to be determined</param>
    /// <returns>the guessed (valid) sample_aspect_ratio, 0/1 if no idea</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVRational av_guess_sample_aspect_ratio(AVFormatContext* @format, AVStream* @stream, AVFrame* @frame);

    /// <summary>Send a nice hexadecimal dump of a buffer to the specified file stream.</summary>
    /// <param name="f">The file stream pointer where the dump should be sent to.</param>
    /// <param name="buf">buffer</param>
    /// <param name="size">buffer size</param>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_hex_dump(_iobuf* @f, byte* @buf, int @size);

    /// <summary>Send a nice hexadecimal dump of a buffer to the log.</summary>
    /// <param name="avcl">A pointer to an arbitrary struct of which the first field is a pointer to an AVClass struct.</param>
    /// <param name="level">The importance level of the message, lower values signifying higher importance.</param>
    /// <param name="buf">buffer</param>
    /// <param name="size">buffer size</param>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_hex_dump_log(void* @avcl, int @level, byte* @buf, int @size);

    /// <summary>Get the index for a specific timestamp.</summary>
    /// <param name="st">stream that the timestamp belongs to</param>
    /// <param name="timestamp">timestamp to retrieve the index for</param>
    /// <param name="flags">if AVSEEK_FLAG_BACKWARD then the returned index will correspond to the timestamp which is &lt; = the requested one, if backward is 0, then it will be &gt;= if AVSEEK_FLAG_ANY seek to any frame, only keyframes otherwise</param>
    /// <returns>&lt; 0 if no such timestamp could be found</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_index_search_timestamp(AVStream* @st, long @timestamp, int @flags);

    /// <summary>Write a packet to an output media file ensuring correct interleaving.</summary>
    /// <param name="s">media file handle</param>
    /// <param name="pkt">The packet containing the data to be written.  If the packet is reference-counted, this function will take ownership of this reference and unreference it later when it sees fit. If the packet is not reference-counted, libavformat will make a copy. The returned packet will be blank (as if returned from av_packet_alloc()), even on error.  This parameter can be NULL (at any time, not just at the end), to flush the interleaving queues.  Packet&apos;s &quot;stream_index&quot; field must be set to the index of the corresponding stream in &quot;s-&gt;streams&quot;.  The timestamps ( &quot;pts&quot;, &quot;dts&quot;) must be set to correct values in the stream&apos;s timebase (unless the output format is flagged with the AVFMT_NOTIMESTAMPS flag, then they can be set to AV_NOPTS_VALUE). The dts for subsequent packets in one stream must be strictly increasing (unless the output format is flagged with the AVFMT_TS_NONSTRICT, then they merely have to be nondecreasing).  &quot;duration&quot; should also be set if known.</param>
    /// <returns>0 on success, a negative AVERROR on error.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_interleaved_write_frame(AVFormatContext* @s, AVPacket* @pkt);

    /// <summary>Write an uncoded frame to an output media file.</summary>
    /// <returns>&gt;=0 for success, a negative code on error</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_interleaved_write_uncoded_frame(AVFormatContext* @s, int @stream_index, AVFrame* @frame);

    /// <summary>Iterate over all registered muxers.</summary>
    /// <param name="opaque">a pointer where libavformat will store the iteration state. Must point to NULL to start the iteration.</param>
    /// <returns>the next registered muxer or NULL when the iteration is finished</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVOutputFormat* av_muxer_iterate(void** @opaque);

    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVProgram* av_new_program(AVFormatContext* @s, int @id);

    /// <summary>Send a nice dump of a packet to the log.</summary>
    /// <param name="avcl">A pointer to an arbitrary struct of which the first field is a pointer to an AVClass struct.</param>
    /// <param name="level">The importance level of the message, lower values signifying higher importance.</param>
    /// <param name="pkt">packet to dump</param>
    /// <param name="dump_payload">True if the payload must be displayed, too.</param>
    /// <param name="st">AVStream that the packet belongs to</param>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_pkt_dump_log2(void* @avcl, int @level, AVPacket* @pkt, int @dump_payload, AVStream* @st);

    /// <summary>Send a nice dump of a packet to the specified file stream.</summary>
    /// <param name="f">The file stream pointer where the dump should be sent to.</param>
    /// <param name="pkt">packet to dump</param>
    /// <param name="dump_payload">True if the payload must be displayed, too.</param>
    /// <param name="st">AVStream that the packet belongs to</param>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_pkt_dump2(_iobuf* @f, AVPacket* @pkt, int @dump_payload, AVStream* @st);

    /// <summary>Like av_probe_input_buffer2() but returns 0 on success</summary>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_probe_input_buffer(AVIOContext* @pb, AVInputFormat** @fmt,
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @url, void* @logctx, uint @offset, uint @max_probe_size);

    /// <summary>Probe a bytestream to determine the input format. Each time a probe returns with a score that is too low, the probe buffer size is increased and another attempt is made. When the maximum probe size is reached, the input format with the highest score is returned.</summary>
    /// <param name="pb">the bytestream to probe</param>
    /// <param name="fmt">the input format is put here</param>
    /// <param name="url">the url of the stream</param>
    /// <param name="logctx">the log context</param>
    /// <param name="offset">the offset within the bytestream to probe from</param>
    /// <param name="max_probe_size">the maximum probe buffer size (zero for default)</param>
    /// <returns>the score in case of success, a negative value corresponding to an the maximal score is AVPROBE_SCORE_MAX AVERROR code otherwise</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_probe_input_buffer2(AVIOContext* @pb, AVInputFormat** @fmt,
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @url, void* @logctx, uint @offset, uint @max_probe_size);

    /// <summary>Guess the file format.</summary>
    /// <param name="pd">data to be probed</param>
    /// <param name="is_opened">Whether the file is already opened; determines whether demuxers with or without AVFMT_NOFILE are probed.</param>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVInputFormat* av_probe_input_format(AVProbeData* @pd, int @is_opened);

    /// <summary>Guess the file format.</summary>
    /// <param name="pd">data to be probed</param>
    /// <param name="is_opened">Whether the file is already opened; determines whether demuxers with or without AVFMT_NOFILE are probed.</param>
    /// <param name="score_max">A probe score larger that this is required to accept a detection, the variable is set to the actual detection score afterwards. If the score is &lt; = AVPROBE_SCORE_MAX / 4 it is recommended to retry with a larger probe buffer.</param>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVInputFormat* av_probe_input_format2(AVProbeData* @pd, int @is_opened, int* @score_max);

    /// <summary>Guess the file format.</summary>
    /// <param name="is_opened">Whether the file is already opened; determines whether demuxers with or without AVFMT_NOFILE are probed.</param>
    /// <param name="score_ret">The score of the best detection.</param>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVInputFormat* av_probe_input_format3(AVProbeData* @pd, int @is_opened, int* @score_ret);

    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_program_add_stream_index(AVFormatContext* @ac, int @progid, uint @idx);

    /// <summary>Return the next frame of a stream. This function returns what is stored in the file, and does not validate that what is there are valid frames for the decoder. It will split what is stored in the file into frames and return one for each call. It will not omit invalid data between valid frames so as to give the decoder the maximum information possible for decoding.</summary>
    /// <returns>0 if OK, &lt; 0 on error or end of file. On error, pkt will be blank (as if it came from av_packet_alloc()).</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_read_frame(AVFormatContext* @s, AVPacket* @pkt);

    /// <summary>Pause a network-based stream (e.g. RTSP stream).</summary>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_read_pause(AVFormatContext* @s);

    /// <summary>Start playing a network-based stream (e.g. RTSP stream) at the current position.</summary>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_read_play(AVFormatContext* @s);

    /// <summary>Generate an SDP for an RTP session.</summary>
    /// <param name="ac">array of AVFormatContexts describing the RTP streams. If the array is composed by only one context, such context can contain multiple AVStreams (one AVStream per RTP stream). Otherwise, all the contexts in the array (an AVCodecContext per RTP stream) must contain only one AVStream.</param>
    /// <param name="n_files">number of AVCodecContexts contained in ac</param>
    /// <param name="buf">buffer where the SDP will be stored (must be allocated by the caller)</param>
    /// <param name="size">the size of the buffer</param>
    /// <returns>0 if OK, AVERROR_xxx on error</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_sdp_create(AVFormatContext** @ac, int @n_files, byte* @buf, int @size);

    /// <summary>Seek to the keyframe at timestamp. &apos;timestamp&apos; in &apos;stream_index&apos;.</summary>
    /// <param name="s">media file handle</param>
    /// <param name="stream_index">If stream_index is (-1), a default stream is selected, and timestamp is automatically converted from AV_TIME_BASE units to the stream specific time_base.</param>
    /// <param name="timestamp">Timestamp in AVStream.time_base units or, if no stream is specified, in AV_TIME_BASE units.</param>
    /// <param name="flags">flags which select direction and seeking mode</param>
    /// <returns>&gt;= 0 on success</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_seek_frame(AVFormatContext* @s, int @stream_index, long @timestamp, int @flags);

    /// <summary>Wrap an existing array as stream side data.</summary>
    /// <param name="st">stream</param>
    /// <param name="type">side information type</param>
    /// <param name="data">the side data array. It must be allocated with the av_malloc() family of functions. The ownership of the data is transferred to st.</param>
    /// <param name="size">side information size</param>
    /// <returns>zero on success, a negative AVERROR code on failure. On failure, the stream is unchanged and the data remains owned by the caller.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_stream_add_side_data(AVStream* @st, AVPacketSideDataType @type, byte* @data, ulong @size);

    /// <summary>Get the AVClass for AVStream. It can be used in combination with AV_OPT_SEARCH_FAKE_OBJ for examining options.</summary>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVClass* av_stream_get_class();

    /// <summary>Get the internal codec timebase from a stream.</summary>
    /// <param name="st">input stream to extract the timebase from</param>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVRational av_stream_get_codec_timebase(AVStream* @st);

    /// <summary>Returns the pts of the last muxed packet + its duration</summary>
    [Obsolete()]
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern long av_stream_get_end_pts(AVStream* @st);

    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVCodecParserContext* av_stream_get_parser(AVStream* @s);

    /// <summary>Get side information from stream.</summary>
    /// <param name="stream">stream</param>
    /// <param name="type">desired side information type</param>
    /// <param name="size">If supplied, *size will be set to the size of the side data or to zero if the desired side data is not present.</param>
    /// <returns>pointer to data if present or NULL otherwise</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern byte* av_stream_get_side_data(AVStream* @stream, AVPacketSideDataType @type, ulong* @size);

    /// <summary>Allocate new information from stream.</summary>
    /// <param name="stream">stream</param>
    /// <param name="type">desired side information type</param>
    /// <param name="size">side information size</param>
    /// <returns>pointer to fresh allocated data or NULL otherwise</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern byte* av_stream_new_side_data(AVStream* @stream, AVPacketSideDataType @type, ulong @size);

    /// <summary>Split a URL string into components.</summary>
    /// <param name="proto">the buffer for the protocol</param>
    /// <param name="proto_size">the size of the proto buffer</param>
    /// <param name="authorization">the buffer for the authorization</param>
    /// <param name="authorization_size">the size of the authorization buffer</param>
    /// <param name="hostname">the buffer for the host name</param>
    /// <param name="hostname_size">the size of the hostname buffer</param>
    /// <param name="port_ptr">a pointer to store the port number in</param>
    /// <param name="path">the buffer for the path</param>
    /// <param name="path_size">the size of the path buffer</param>
    /// <param name="url">the URL to split</param>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_url_split(byte* @proto, int @proto_size, byte* @authorization, int @authorization_size, byte* @hostname, int @hostname_size, int* @port_ptr, byte* @path, int @path_size,
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @url);

    /// <summary>Write a packet to an output media file.</summary>
    /// <param name="s">media file handle</param>
    /// <param name="pkt">The packet containing the data to be written. Note that unlike av_interleaved_write_frame(), this function does not take ownership of the packet passed to it (though some muxers may make an internal reference to the input packet).  This parameter can be NULL (at any time, not just at the end), in order to immediately flush data buffered within the muxer, for muxers that buffer up data internally before writing it to the output.  Packet&apos;s &quot;stream_index&quot; field must be set to the index of the corresponding stream in &quot;s-&gt;streams&quot;.  The timestamps ( &quot;pts&quot;, &quot;dts&quot;) must be set to correct values in the stream&apos;s timebase (unless the output format is flagged with the AVFMT_NOTIMESTAMPS flag, then they can be set to AV_NOPTS_VALUE). The dts for subsequent packets passed to this function must be strictly increasing when compared in their respective timebases (unless the output format is flagged with the AVFMT_TS_NONSTRICT, then they merely have to be nondecreasing). &quot;duration&quot;) should also be set if known.</param>
    /// <returns>&lt; 0 on error, = 0 if OK, 1 if flushed and there is no more data to flush</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_write_frame(AVFormatContext* @s, AVPacket* @pkt);

    /// <summary>Write the stream trailer to an output media file and free the file private data.</summary>
    /// <param name="s">media file handle</param>
    /// <returns>0 if OK, AVERROR_xxx on error</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_write_trailer(AVFormatContext* @s);

    /// <summary>Write an uncoded frame to an output media file.</summary>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_write_uncoded_frame(AVFormatContext* @s, int @stream_index, AVFrame* @frame);

    /// <summary>Test whether a muxer supports uncoded frame.</summary>
    /// <returns>&gt;=0 if an uncoded frame can be written to that muxer and stream,  &lt; 0 if not</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_write_uncoded_frame_query(AVFormatContext* @s, int @stream_index);

    /// <summary>Allocate an AVFormatContext. avformat_free_context() can be used to free the context and everything allocated by the framework within it.</summary>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVFormatContext* avformat_alloc_context();

    /// <summary>Allocate an AVFormatContext for an output format. avformat_free_context() can be used to free the context and everything allocated by the framework within it.</summary>
    /// <param name="ctx">pointee is set to the created format context, or to NULL in case of failure</param>
    /// <param name="oformat">format to use for allocating the context, if NULL format_name and filename are used instead</param>
    /// <param name="format_name">the name of output format to use for allocating the context, if NULL filename is used instead</param>
    /// <param name="filename">the name of the filename to use for allocating the context, may be NULL</param>
    /// <returns>&gt;= 0 in case of success, a negative AVERROR code in case of failure</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avformat_alloc_output_context2(AVFormatContext** @ctx, AVOutputFormat* @oformat,
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @format_name,
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @filename);

    /// <summary>Close an opened input AVFormatContext. Free it and all its contents and set *s to NULL.</summary>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern void avformat_close_input(AVFormatContext** @s);

    /// <summary>Return the libavformat build-time configuration.</summary>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public static extern string avformat_configuration();

    /// <summary>Read packets of a media file to get stream information. This is useful for file formats with no headers such as MPEG. This function also computes the real framerate in case of MPEG-2 repeat frame mode. The logical file position is not changed by this function; examined packets may be buffered for later processing.</summary>
    /// <param name="ic">media file handle</param>
    /// <param name="options">If non-NULL, an ic.nb_streams long array of pointers to dictionaries, where i-th member contains options for codec corresponding to i-th stream. On return each dictionary will be filled with options that were not found.</param>
    /// <returns>&gt;=0 if OK, AVERROR_xxx on error</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avformat_find_stream_info(AVFormatContext* @ic, AVDictionary** @options);

    /// <summary>Discard all internally buffered data. This can be useful when dealing with discontinuities in the byte stream. Generally works only with formats that can resync. This includes headerless formats like MPEG-TS/TS but should also work with NUT, Ogg and in a limited way AVI for example.</summary>
    /// <param name="s">media file handle</param>
    /// <returns>&gt;=0 on success, error code otherwise</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avformat_flush(AVFormatContext* @s);

    /// <summary>Free an AVFormatContext and all its streams.</summary>
    /// <param name="s">context to free</param>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern void avformat_free_context(AVFormatContext* @s);

    /// <summary>Get the AVClass for AVFormatContext. It can be used in combination with AV_OPT_SEARCH_FAKE_OBJ for examining options.</summary>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVClass* avformat_get_class();

    /// <summary>Returns the table mapping MOV FourCCs for audio to AVCodecID.</summary>
    /// <returns>the table mapping MOV FourCCs for audio to AVCodecID.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVCodecTag* avformat_get_mov_audio_tags();

    /// <summary>Returns the table mapping MOV FourCCs for video to libavcodec AVCodecID.</summary>
    /// <returns>the table mapping MOV FourCCs for video to libavcodec AVCodecID.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVCodecTag* avformat_get_mov_video_tags();

    /// <summary>Returns the table mapping RIFF FourCCs for audio to AVCodecID.</summary>
    /// <returns>the table mapping RIFF FourCCs for audio to AVCodecID.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVCodecTag* avformat_get_riff_audio_tags();

    /// <summary>@{ Get the tables mapping RIFF FourCCs to libavcodec AVCodecIDs. The tables are meant to be passed to av_codec_get_id()/av_codec_get_tag() as in the following code:</summary>
    /// <returns>the table mapping RIFF FourCCs for video to libavcodec AVCodecID.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVCodecTag* avformat_get_riff_video_tags();

    /// <summary>Get the index entry count for the given AVStream.</summary>
    /// <param name="st">stream</param>
    /// <returns>the number of index entries in the stream</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avformat_index_get_entries_count(AVStream* @st);

    /// <summary>Get the AVIndexEntry corresponding to the given index.</summary>
    /// <param name="st">Stream containing the requested AVIndexEntry.</param>
    /// <param name="idx">The desired index.</param>
    /// <returns>A pointer to the requested AVIndexEntry if it exists, NULL otherwise.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVIndexEntry* avformat_index_get_entry(AVStream* @st, int @idx);

    /// <summary>Get the AVIndexEntry corresponding to the given timestamp.</summary>
    /// <param name="st">Stream containing the requested AVIndexEntry.</param>
    /// <param name="wanted_timestamp">Timestamp to retrieve the index entry for.</param>
    /// <param name="flags">If AVSEEK_FLAG_BACKWARD then the returned entry will correspond to the timestamp which is &lt; = the requested one, if backward is 0, then it will be &gt;= if AVSEEK_FLAG_ANY seek to any frame, only keyframes otherwise.</param>
    /// <returns>A pointer to the requested AVIndexEntry if it exists, NULL otherwise.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVIndexEntry* avformat_index_get_entry_from_timestamp(AVStream* @st, long @wanted_timestamp, int @flags);

    /// <summary>Allocate the stream private data and initialize the codec, but do not write the header. May optionally be used before avformat_write_header() to initialize stream parameters before actually writing the header. If using this function, do not pass the same options to avformat_write_header().</summary>
    /// <param name="s">Media file handle, must be allocated with avformat_alloc_context(). Its &quot;oformat&quot; field must be set to the desired output format; Its &quot;pb&quot; field must be set to an already opened ::AVIOContext.</param>
    /// <param name="options">An ::AVDictionary filled with AVFormatContext and muxer-private options. On return this parameter will be destroyed and replaced with a dict containing options that were not found. May be NULL.</param>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avformat_init_output(AVFormatContext* @s, AVDictionary** @options);

    /// <summary>Return the libavformat license.</summary>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public static extern string avformat_license();

    /// <summary>Check if the stream st contained in s is matched by the stream specifier spec.</summary>
    /// <returns>&gt;0 if st is matched by spec; 0  if st is not matched by spec; AVERROR code if spec is invalid</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avformat_match_stream_specifier(AVFormatContext* @s, AVStream* @st,
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @spec);

    /// <summary>Undo the initialization done by avformat_network_init. Call it only once for each time you called avformat_network_init.</summary>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avformat_network_deinit();

    /// <summary>Do global initialization of network libraries. This is optional, and not recommended anymore.</summary>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avformat_network_init();

    /// <summary>Add a new stream to a media file.</summary>
    /// <param name="s">media file handle</param>
    /// <param name="c">unused, does nothing</param>
    /// <returns>newly created stream or NULL on error.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVStream* avformat_new_stream(AVFormatContext* @s, AVCodec* @c);

    /// <summary>Open an input stream and read the header. The codecs are not opened. The stream must be closed with avformat_close_input().</summary>
    /// <param name="ps">Pointer to user-supplied AVFormatContext (allocated by avformat_alloc_context). May be a pointer to NULL, in which case an AVFormatContext is allocated by this function and written into ps. Note that a user-supplied AVFormatContext will be freed on failure.</param>
    /// <param name="url">URL of the stream to open.</param>
    /// <param name="fmt">If non-NULL, this parameter forces a specific input format. Otherwise the format is autodetected.</param>
    /// <param name="options">A dictionary filled with AVFormatContext and demuxer-private options. On return this parameter will be destroyed and replaced with a dict containing options that were not found. May be NULL.</param>
    /// <returns>0 on success, a negative AVERROR on failure.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avformat_open_input(AVFormatContext** @ps,
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @url, AVInputFormat* @fmt, AVDictionary** @options);

    /// <summary>Test if the given container can store a codec.</summary>
    /// <param name="ofmt">container to check for compatibility</param>
    /// <param name="codec_id">codec to potentially store in container</param>
    /// <param name="std_compliance">standards compliance level, one of FF_COMPLIANCE_*</param>
    /// <returns>1 if codec with ID codec_id can be stored in ofmt, 0 if it cannot. A negative number if this information is not available.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avformat_query_codec(AVOutputFormat* @ofmt, AVCodecID @codec_id, int @std_compliance);

    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avformat_queue_attached_pictures(AVFormatContext* @s);

    /// <summary>Seek to timestamp ts. Seeking will be done so that the point from which all active streams can be presented successfully will be closest to ts and within min/max_ts. Active streams are all streams that have AVStream.discard &lt; AVDISCARD_ALL.</summary>
    /// <param name="s">media file handle</param>
    /// <param name="stream_index">index of the stream which is used as time base reference</param>
    /// <param name="min_ts">smallest acceptable timestamp</param>
    /// <param name="ts">target timestamp</param>
    /// <param name="max_ts">largest acceptable timestamp</param>
    /// <param name="flags">flags</param>
    /// <returns>&gt;=0 on success, error code otherwise</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avformat_seek_file(AVFormatContext* @s, int @stream_index, long @min_ts, long @ts, long @max_ts, int @flags);

    /// <summary>Transfer internal timing information from one stream to another.</summary>
    /// <param name="ofmt">target output format for ost</param>
    /// <param name="ost">output stream which needs timings copy and adjustments</param>
    /// <param name="ist">reference input stream to copy timings from</param>
    /// <param name="copy_tb">define from where the stream codec timebase needs to be imported</param>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avformat_transfer_internal_stream_timing_info(AVOutputFormat* @ofmt, AVStream* @ost, AVStream* @ist, AVTimebaseSource @copy_tb);

    /// <summary>Return the LIBAVFORMAT_VERSION_INT constant.</summary>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint avformat_version();

    /// <summary>Allocate the stream private data and write the stream header to an output media file.</summary>
    /// <param name="s">Media file handle, must be allocated with avformat_alloc_context(). Its &quot;oformat&quot; field must be set to the desired output format; Its &quot;pb&quot; field must be set to an already opened ::AVIOContext.</param>
    /// <param name="options">An ::AVDictionary filled with AVFormatContext and muxer-private options. On return this parameter will be destroyed and replaced with a dict containing options that were not found. May be NULL.</param>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avformat_write_header(AVFormatContext* @s, AVDictionary** @options);

    /// <summary>Accept and allocate a client context on a server context.</summary>
    /// <param name="s">the server context</param>
    /// <param name="c">the client context, must be unallocated</param>
    /// <returns>&gt;= 0 on success or a negative value corresponding to an AVERROR on failure</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avio_accept(AVIOContext* @s, AVIOContext** @c);

    /// <summary>Allocate and initialize an AVIOContext for buffered I/O. It must be later freed with avio_context_free().</summary>
    /// <param name="buffer">Memory block for input/output operations via AVIOContext. The buffer must be allocated with av_malloc() and friends. It may be freed and replaced with a new buffer by libavformat. AVIOContext.buffer holds the buffer currently in use, which must be later freed with av_free().</param>
    /// <param name="buffer_size">The buffer size is very important for performance. For protocols with fixed blocksize it should be set to this blocksize. For others a typical size is a cache page, e.g. 4kb.</param>
    /// <param name="write_flag">Set to 1 if the buffer should be writable, 0 otherwise.</param>
    /// <param name="opaque">An opaque pointer to user-specific data.</param>
    /// <param name="read_packet">A function for refilling the buffer, may be NULL. For stream protocols, must never return 0 but rather a proper AVERROR code.</param>
    /// <param name="write_packet">A function for writing the buffer contents, may be NULL. The function may not change the input buffers content.</param>
    /// <param name="seek">A function for seeking to specified byte position, may be NULL.</param>
    /// <returns>Allocated AVIOContext or NULL on failure.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVIOContext* avio_alloc_context(byte* @buffer, int @buffer_size, int @write_flag, void* @opaque, avio_alloc_context_read_packet_func @read_packet, avio_alloc_context_write_packet_func @write_packet, avio_alloc_context_seek_func @seek);

    /// <summary>Return AVIO_FLAG_* access flags corresponding to the access permissions of the resource in url, or a negative value corresponding to an AVERROR code in case of failure. The returned access flags are masked by the value in flags.</summary>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avio_check(
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @url, int @flags);

    /// <summary>Close the resource accessed by the AVIOContext s and free it. This function can only be used if s was opened by avio_open().</summary>
    /// <returns>0 on success, an AVERROR &lt; 0 on error.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avio_close(AVIOContext* @s);

    /// <summary>Close directory.</summary>
    /// <param name="s">directory read context.</param>
    /// <returns>&gt;=0 on success or negative on error.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avio_close_dir(AVIODirContext** @s);

    /// <summary>Return the written size and a pointer to the buffer. The buffer must be freed with av_free(). Padding of AV_INPUT_BUFFER_PADDING_SIZE is added to the buffer.</summary>
    /// <param name="s">IO context</param>
    /// <param name="pbuffer">pointer to a byte buffer</param>
    /// <returns>the length of the byte buffer</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avio_close_dyn_buf(AVIOContext* @s, byte** @pbuffer);

    /// <summary>Close the resource accessed by the AVIOContext *s, free it and set the pointer pointing to it to NULL. This function can only be used if s was opened by avio_open().</summary>
    /// <returns>0 on success, an AVERROR &lt; 0 on error.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avio_closep(AVIOContext** @s);

    /// <summary>Free the supplied IO context and everything associated with it.</summary>
    /// <param name="s">Double pointer to the IO context. This function will write NULL into s.</param>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern void avio_context_free(AVIOContext** @s);

    /// <summary>Iterate through names of available protocols.</summary>
    /// <param name="opaque">A private pointer representing current protocol. It must be a pointer to NULL on first iteration and will be updated by successive calls to avio_enum_protocols.</param>
    /// <param name="output">If set to 1, iterate over output protocols, otherwise over input protocols.</param>
    /// <returns>A static string containing the name of current protocol or NULL</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public static extern string avio_enum_protocols(void** @opaque, int @output);

    /// <summary>Similar to feof() but also returns nonzero on read errors.</summary>
    /// <returns>non zero if and only if at end of file or a read error happened when reading.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avio_feof(AVIOContext* @s);

    /// <summary>Return the name of the protocol that will handle the passed URL.</summary>
    /// <returns>Name of the protocol or NULL.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public static extern string avio_find_protocol_name(
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @url);

    /// <summary>Force flushing of buffered data.</summary>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern void avio_flush(AVIOContext* @s);

    /// <summary>Free entry allocated by avio_read_dir().</summary>
    /// <param name="entry">entry to be freed.</param>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern void avio_free_directory_entry(AVIODirEntry** @entry);

    /// <summary>Return the written size and a pointer to the buffer. The AVIOContext stream is left intact. The buffer must NOT be freed. No padding is added to the buffer.</summary>
    /// <param name="s">IO context</param>
    /// <param name="pbuffer">pointer to a byte buffer</param>
    /// <returns>the length of the byte buffer</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avio_get_dyn_buf(AVIOContext* @s, byte** @pbuffer);

    /// <summary>Read a string from pb into buf. The reading will terminate when either a NULL character was encountered, maxlen bytes have been read, or nothing more can be read from pb. The result is guaranteed to be NULL-terminated, it will be truncated if buf is too small. Note that the string is not interpreted or validated in any way, it might get truncated in the middle of a sequence for multi-byte encodings.</summary>
    /// <returns>number of bytes read (is always &lt; = maxlen). If reading ends on EOF or error, the return value will be one more than bytes actually read.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avio_get_str(AVIOContext* @pb, int @maxlen, byte* @buf, int @buflen);

    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avio_get_str16be(AVIOContext* @pb, int @maxlen, byte* @buf, int @buflen);

    /// <summary>Read a UTF-16 string from pb and convert it to UTF-8. The reading will terminate when either a null or invalid character was encountered or maxlen bytes have been read.</summary>
    /// <returns>number of bytes read (is always &lt; = maxlen)</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avio_get_str16le(AVIOContext* @pb, int @maxlen, byte* @buf, int @buflen);

    /// <summary>Perform one step of the protocol handshake to accept a new client. This function must be called on a client returned by avio_accept() before using it as a read/write context. It is separate from avio_accept() because it may block. A step of the handshake is defined by places where the application may decide to change the proceedings. For example, on a protocol with a request header and a reply header, each one can constitute a step because the application may use the parameters from the request to change parameters in the reply; or each individual chunk of the request can constitute a step. If the handshake is already finished, avio_handshake() does nothing and returns 0 immediately.</summary>
    /// <param name="c">the client context to perform the handshake on</param>
    /// <returns>0   on a complete and successful handshake &gt; 0 if the handshake progressed, but is not complete  &lt; 0 for an AVERROR code</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avio_handshake(AVIOContext* @c);

    /// <summary>Create and initialize a AVIOContext for accessing the resource indicated by url.</summary>
    /// <param name="s">Used to return the pointer to the created AVIOContext. In case of failure the pointed to value is set to NULL.</param>
    /// <param name="url">resource to access</param>
    /// <param name="flags">flags which control how the resource indicated by url is to be opened</param>
    /// <returns>&gt;= 0 in case of success, a negative value corresponding to an AVERROR code in case of failure</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avio_open(AVIOContext** @s,
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @url, int @flags);

    /// <summary>Open directory for reading.</summary>
    /// <param name="s">directory read context. Pointer to a NULL pointer must be passed.</param>
    /// <param name="url">directory to be listed.</param>
    /// <param name="options">A dictionary filled with protocol-private options. On return this parameter will be destroyed and replaced with a dictionary containing options that were not found. May be NULL.</param>
    /// <returns>&gt;=0 on success or negative on error.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avio_open_dir(AVIODirContext** @s,
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @url, AVDictionary** @options);

    /// <summary>Open a write only memory stream.</summary>
    /// <param name="s">new IO context</param>
    /// <returns>zero if no error.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avio_open_dyn_buf(AVIOContext** @s);

    /// <summary>Create and initialize a AVIOContext for accessing the resource indicated by url.</summary>
    /// <param name="s">Used to return the pointer to the created AVIOContext. In case of failure the pointed to value is set to NULL.</param>
    /// <param name="url">resource to access</param>
    /// <param name="flags">flags which control how the resource indicated by url is to be opened</param>
    /// <param name="int_cb">an interrupt callback to be used at the protocols level</param>
    /// <param name="options">A dictionary filled with protocol-private options. On return this parameter will be destroyed and replaced with a dict containing options that were not found. May be NULL.</param>
    /// <returns>&gt;= 0 in case of success, a negative value corresponding to an AVERROR code in case of failure</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avio_open2(AVIOContext** @s,
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @url, int @flags, AVIOInterruptCB* @int_cb, AVDictionary** @options);

    /// <summary>Pause and resume playing - only meaningful if using a network streaming protocol (e.g. MMS).</summary>
    /// <param name="h">IO context from which to call the read_pause function pointer</param>
    /// <param name="pause">1 for pause, 0 for resume</param>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avio_pause(AVIOContext* @h, int @pause);

    /// <summary>Write a NULL terminated array of strings to the context. Usually you don&apos;t need to use this function directly but its macro wrapper, avio_print.</summary>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern void avio_print_string_array(AVIOContext* @s, byte*[] @strings);

    /// <summary>Writes a formatted string to the context.</summary>
    /// <returns>number of bytes written, &lt; 0 on error.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avio_printf(AVIOContext* @s,
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @fmt);

    /// <summary>Get AVClass by names of available protocols.</summary>
    /// <returns>A AVClass of input protocol name or NULL</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVClass* avio_protocol_get_class(
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name);

    /// <summary>Write a NULL-terminated string.</summary>
    /// <returns>number of bytes written.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avio_put_str(AVIOContext* @s,
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @str);

    /// <summary>Convert an UTF-8 string to UTF-16BE and write it.</summary>
    /// <param name="s">the AVIOContext</param>
    /// <param name="str">NULL-terminated UTF-8 string</param>
    /// <returns>number of bytes written.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avio_put_str16be(AVIOContext* @s,
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @str);

    /// <summary>Convert an UTF-8 string to UTF-16LE and write it.</summary>
    /// <param name="s">the AVIOContext</param>
    /// <param name="str">NULL-terminated UTF-8 string</param>
    /// <returns>number of bytes written.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avio_put_str16le(AVIOContext* @s,
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @str);

    /// <summary>@{</summary>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avio_r8(AVIOContext* @s);

    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint avio_rb16(AVIOContext* @s);

    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint avio_rb24(AVIOContext* @s);

    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint avio_rb32(AVIOContext* @s);

    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern ulong avio_rb64(AVIOContext* @s);

    /// <summary>Read size bytes from AVIOContext into buf.</summary>
    /// <returns>number of bytes read or AVERROR</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avio_read(AVIOContext* @s, byte* @buf, int @size);

    /// <summary>Get next directory entry.</summary>
    /// <param name="s">directory read context.</param>
    /// <param name="next">next entry or NULL when no more entries.</param>
    /// <returns>&gt;=0 on success or negative on error. End of list is not considered an error.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avio_read_dir(AVIODirContext* @s, AVIODirEntry** @next);

    /// <summary>Read size bytes from AVIOContext into buf. Unlike avio_read(), this is allowed to read fewer bytes than requested. The missing bytes can be read in the next call. This always tries to read at least 1 byte. Useful to reduce latency in certain cases.</summary>
    /// <returns>number of bytes read or AVERROR</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avio_read_partial(AVIOContext* @s, byte* @buf, int @size);

    /// <summary>Read contents of h into print buffer, up to max_size bytes, or up to EOF.</summary>
    /// <returns>0 for success (max_size bytes read or EOF reached), negative error code otherwise</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avio_read_to_bprint(AVIOContext* @h, AVBPrint* @pb, ulong @max_size);

    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint avio_rl16(AVIOContext* @s);

    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint avio_rl24(AVIOContext* @s);

    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint avio_rl32(AVIOContext* @s);

    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern ulong avio_rl64(AVIOContext* @s);

    /// <summary>fseek() equivalent for AVIOContext.</summary>
    /// <returns>new position or AVERROR.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern long avio_seek(AVIOContext* @s, long @offset, int @whence);

    /// <summary>Seek to a given timestamp relative to some component stream. Only meaningful if using a network streaming protocol (e.g. MMS.).</summary>
    /// <param name="h">IO context from which to call the seek function pointers</param>
    /// <param name="stream_index">The stream index that the timestamp is relative to. If stream_index is (-1) the timestamp should be in AV_TIME_BASE units from the beginning of the presentation. If a stream_index &gt;= 0 is used and the protocol does not support seeking based on component streams, the call will fail.</param>
    /// <param name="timestamp">timestamp in AVStream.time_base units or if there is no stream specified then in AV_TIME_BASE units.</param>
    /// <param name="flags">Optional combination of AVSEEK_FLAG_BACKWARD, AVSEEK_FLAG_BYTE and AVSEEK_FLAG_ANY. The protocol may silently ignore AVSEEK_FLAG_BACKWARD and AVSEEK_FLAG_ANY, but AVSEEK_FLAG_BYTE will fail if used and not supported.</param>
    /// <returns>&gt;= 0 on success</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern long avio_seek_time(AVIOContext* @h, int @stream_index, long @timestamp, int @flags);

    /// <summary>Get the filesize.</summary>
    /// <returns>filesize or AVERROR</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern long avio_size(AVIOContext* @s);

    /// <summary>Skip given number of bytes forward</summary>
    /// <returns>new position or AVERROR.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern long avio_skip(AVIOContext* @s, long @offset);

    /// <summary>Writes a formatted string to the context taking a va_list.</summary>
    /// <returns>number of bytes written, &lt; 0 on error.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avio_vprintf(AVIOContext* @s,
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @fmt, byte* @ap);

    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern void avio_w8(AVIOContext* @s, int @b);

    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern void avio_wb16(AVIOContext* @s, uint @val);

    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern void avio_wb24(AVIOContext* @s, uint @val);

    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern void avio_wb32(AVIOContext* @s, uint @val);

    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern void avio_wb64(AVIOContext* @s, ulong @val);

    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern void avio_wl16(AVIOContext* @s, uint @val);

    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern void avio_wl24(AVIOContext* @s, uint @val);

    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern void avio_wl32(AVIOContext* @s, uint @val);

    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern void avio_wl64(AVIOContext* @s, ulong @val);

    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern void avio_write(AVIOContext* @s, byte* @buf, int @size);

    /// <summary>Mark the written bytestream as a specific type.</summary>
    /// <param name="s">the AVIOContext</param>
    /// <param name="time">the stream time the current bytestream pos corresponds to (in AV_TIME_BASE units), or AV_NOPTS_VALUE if unknown or not applicable</param>
    /// <param name="type">the kind of data written starting at the current pos</param>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern void avio_write_marker(AVIOContext* @s, long @time, AVIODataMarkerType @type);
}
