#pragma warning disable

using FFmpeg.Util;
using System;
using System.Runtime.InteropServices;

namespace FFmpeg;

public static unsafe partial class FFmpeg {
    /// <summary>Allocate a context for a given bitstream filter. The caller must fill in the context parameters as described in the documentation and then call av_bsf_init() before sending any data to the filter.</summary>
    /// <param name="filter">the filter for which to allocate an instance.</param>
    /// <param name="ctx">a pointer into which the pointer to the newly-allocated context will be written. It must be freed with av_bsf_free() after the filtering is done.</param>
    /// <returns>0 on success, a negative AVERROR code on failure</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_bsf_alloc(AVBitStreamFilter* @filter, AVBSFContext** @ctx);

    /// <summary>Reset the internal bitstream filter state. Should be called e.g. when seeking.</summary>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_bsf_flush(AVBSFContext* @ctx);

    /// <summary>Free a bitstream filter context and everything associated with it; write NULL into the supplied pointer.</summary>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_bsf_free(AVBSFContext** @ctx);

    /// <summary>Returns a bitstream filter with the specified name or NULL if no such bitstream filter exists.</summary>
    /// <returns>a bitstream filter with the specified name or NULL if no such bitstream filter exists.</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVBitStreamFilter* av_bsf_get_by_name(
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name);

    /// <summary>Get the AVClass for AVBSFContext. It can be used in combination with AV_OPT_SEARCH_FAKE_OBJ for examining options.</summary>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVClass* av_bsf_get_class();

    /// <summary>Get null/pass-through bitstream filter.</summary>
    /// <param name="bsf">Pointer to be set to new instance of pass-through bitstream filter</param>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_bsf_get_null_filter(AVBSFContext** @bsf);

    /// <summary>Prepare the filter for use, after all the parameters and options have been set.</summary>
    /// <param name="ctx">a AVBSFContext previously allocated with av_bsf_alloc()</param>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_bsf_init(AVBSFContext* @ctx);

    /// <summary>Iterate over all registered bitstream filters.</summary>
    /// <param name="opaque">a pointer where libavcodec will store the iteration state. Must point to NULL to start the iteration.</param>
    /// <returns>the next registered bitstream filter or NULL when the iteration is finished</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVBitStreamFilter* av_bsf_iterate(void** @opaque);

    /// <summary>Allocate empty list of bitstream filters. The list must be later freed by av_bsf_list_free() or finalized by av_bsf_list_finalize().</summary>
    /// <returns>Pointer to on success, NULL in case of failure</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVBSFList* av_bsf_list_alloc();

    /// <summary>Append bitstream filter to the list of bitstream filters.</summary>
    /// <param name="lst">List to append to</param>
    /// <param name="bsf">Filter context to be appended</param>
    /// <returns>&gt;=0 on success, negative AVERROR in case of failure</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_bsf_list_append(AVBSFList* @lst, AVBSFContext* @bsf);

    /// <summary>Construct new bitstream filter context given it&apos;s name and options and append it to the list of bitstream filters.</summary>
    /// <param name="lst">List to append to</param>
    /// <param name="bsf_name">Name of the bitstream filter</param>
    /// <param name="options">Options for the bitstream filter, can be set to NULL</param>
    /// <returns>&gt;=0 on success, negative AVERROR in case of failure</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_bsf_list_append2(AVBSFList* @lst,
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @bsf_name, AVDictionary** @options);

    /// <summary>Finalize list of bitstream filters.</summary>
    /// <param name="lst">Filter list structure to be transformed</param>
    /// <param name="bsf">Pointer to be set to newly created structure representing the chain of bitstream filters</param>
    /// <returns>&gt;=0 on success, negative AVERROR in case of failure</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_bsf_list_finalize(AVBSFList** @lst, AVBSFContext** @bsf);

    /// <summary>Free list of bitstream filters.</summary>
    /// <param name="lst">Pointer to pointer returned by av_bsf_list_alloc()</param>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_bsf_list_free(AVBSFList** @lst);

    /// <summary>Parse string describing list of bitstream filters and create single AVBSFContext describing the whole chain of bitstream filters. Resulting AVBSFContext can be treated as any other AVBSFContext freshly allocated by av_bsf_alloc().</summary>
    /// <param name="str">String describing chain of bitstream filters in format `bsf1[=opt1=val1:opt2=val2][,bsf2]`</param>
    /// <param name="bsf">Pointer to be set to newly created structure representing the chain of bitstream filters</param>
    /// <returns>&gt;=0 on success, negative AVERROR in case of failure</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_bsf_list_parse_str(
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @str, AVBSFContext** @bsf);

    /// <summary>Retrieve a filtered packet.</summary>
    /// <param name="ctx">an initialized AVBSFContext</param>
    /// <param name="pkt">this struct will be filled with the contents of the filtered packet. It is owned by the caller and must be freed using av_packet_unref() when it is no longer needed. This parameter should be &quot;clean&quot; (i.e. freshly allocated with av_packet_alloc() or unreffed with av_packet_unref()) when this function is called. If this function returns successfully, the contents of pkt will be completely overwritten by the returned data. On failure, pkt is not touched.</param>
    /// <returns>- 0 on success. - AVERROR(EAGAIN) if more packets need to be sent to the filter (using av_bsf_send_packet()) to get more output. - AVERROR_EOF if there will be no further output from the filter. - Another negative AVERROR value if an error occurs.</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_bsf_receive_packet(AVBSFContext* @ctx, AVPacket* @pkt);

    /// <summary>Submit a packet for filtering.</summary>
    /// <param name="ctx">an initialized AVBSFContext</param>
    /// <param name="pkt">the packet to filter. The bitstream filter will take ownership of the packet and reset the contents of pkt. pkt is not touched if an error occurs. If pkt is empty (i.e. NULL, or pkt-&gt;data is NULL and pkt-&gt;side_data_elems zero), it signals the end of the stream (i.e. no more non-empty packets will be sent; sending more empty packets does nothing) and will cause the filter to output any packets it may have buffered internally.</param>
    /// <returns>- 0 on success. - AVERROR(EAGAIN) if packets need to be retrieved from the filter (using av_bsf_receive_packet()) before new input can be consumed. - Another negative AVERROR value if an error occurs.</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_bsf_send_packet(AVBSFContext* @ctx, AVPacket* @pkt);

    /// <summary>Returns a non-zero number if codec is a decoder, zero otherwise</summary>
    /// <returns>a non-zero number if codec is a decoder, zero otherwise</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_codec_is_decoder(AVCodec* @codec);

    /// <summary>Returns a non-zero number if codec is an encoder, zero otherwise</summary>
    /// <returns>a non-zero number if codec is an encoder, zero otherwise</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_codec_is_encoder(AVCodec* @codec);

    /// <summary>Iterate over all registered codecs.</summary>
    /// <param name="opaque">a pointer where libavcodec will store the iteration state. Must point to NULL to start the iteration.</param>
    /// <returns>the next registered codec or NULL when the iteration is finished</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVCodec* av_codec_iterate(void** @opaque);

    /// <summary>Allocate a CPB properties structure and initialize its fields to default values.</summary>
    /// <param name="size">if non-NULL, the size of the allocated struct will be written here. This is useful for embedding it in side data.</param>
    /// <returns>the newly allocated struct or NULL on failure</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVCPBProperties* av_cpb_properties_alloc(ulong* @size);

    /// <summary>Allocate an AVD3D11VAContext.</summary>
    /// <returns>Newly-allocated AVD3D11VAContext or NULL on failure.</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVD3D11VAContext* av_d3d11va_alloc_context();

    /// <summary>Same behaviour av_fast_malloc but the buffer has additional AV_INPUT_BUFFER_PADDING_SIZE at the end which will always be 0.</summary>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_fast_padded_malloc(void* @ptr, uint* @size, ulong @min_size);

    /// <summary>Same behaviour av_fast_padded_malloc except that buffer will always be 0-initialized after call.</summary>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_fast_padded_mallocz(void* @ptr, uint* @size, ulong @min_size);

    /// <summary>Return audio frame duration.</summary>
    /// <param name="avctx">codec context</param>
    /// <param name="frame_bytes">size of the frame, or 0 if unknown</param>
    /// <returns>frame duration, in samples, if known. 0 if not able to determine.</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_get_audio_frame_duration(AVCodecContext* @avctx, int @frame_bytes);

    /// <summary>This function is the same as av_get_audio_frame_duration(), except it works with AVCodecParameters instead of an AVCodecContext.</summary>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_get_audio_frame_duration2(AVCodecParameters* @par, int @frame_bytes);

    /// <summary>Return codec bits per sample. Only return non-zero if the bits per sample is exactly correct, not an approximation.</summary>
    /// <param name="codec_id">the codec</param>
    /// <returns>Number of bits per sample or zero if unknown for the given codec.</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_get_exact_bits_per_sample(AVCodecID @codec_id);

    /// <summary>Return the PCM codec associated with a sample format.</summary>
    /// <param name="be">endianness, 0 for little, 1 for big, -1 (or anything else) for native</param>
    /// <returns>AV_CODEC_ID_PCM_* or AV_CODEC_ID_NONE</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVCodecID av_get_pcm_codec(AVSampleFormat @fmt, int @be);

    /// <summary>Return a name for the specified profile, if available.</summary>
    /// <param name="codec">the codec that is searched for the given profile</param>
    /// <param name="profile">the profile value for which a name is requested</param>
    /// <returns>A name for the profile if found, NULL otherwise.</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public static extern string av_get_profile_name(AVCodec* @codec, int @profile);

    /// <summary>Increase packet size, correctly zeroing padding</summary>
    /// <param name="pkt">packet</param>
    /// <param name="grow_by">number of bytes by which to increase the size of the packet</param>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_grow_packet(AVPacket* @pkt, int @grow_by);

    /// <summary>Initialize optional fields of a packet with default values.</summary>
    /// <param name="pkt">packet</param>
    [Obsolete("This function is deprecated. Once it's removed, sizeof(AVPacket) will not be a part of the ABI anymore.")]
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_init_packet(AVPacket* @pkt);

    /// <summary>Allocate the payload of a packet and initialize its fields with default values.</summary>
    /// <param name="pkt">packet</param>
    /// <param name="size">wanted payload size</param>
    /// <returns>0 if OK, AVERROR_xxx otherwise</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_new_packet(AVPacket* @pkt, int @size);

    /// <summary>Wrap an existing array as a packet side data.</summary>
    /// <param name="pkt">packet</param>
    /// <param name="type">side information type</param>
    /// <param name="data">the side data array. It must be allocated with the av_malloc() family of functions. The ownership of the data is transferred to pkt.</param>
    /// <param name="size">side information size</param>
    /// <returns>a non-negative number on success, a negative AVERROR code on failure. On failure, the packet is unchanged and the data remains owned by the caller.</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_packet_add_side_data(AVPacket* @pkt, AVPacketSideDataType @type, byte* @data, ulong @size);

    /// <summary>Allocate an AVPacket and set its fields to default values. The resulting struct must be freed using av_packet_free().</summary>
    /// <returns>An AVPacket filled with default values or NULL on failure.</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVPacket* av_packet_alloc();

    /// <summary>Create a new packet that references the same data as src.</summary>
    /// <returns>newly created AVPacket on success, NULL on error.</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVPacket* av_packet_clone(AVPacket* @src);

    /// <summary>Copy only &quot;properties&quot; fields from src to dst.</summary>
    /// <param name="dst">Destination packet</param>
    /// <param name="src">Source packet</param>
    /// <returns>0 on success AVERROR on failure.</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_packet_copy_props(AVPacket* @dst, AVPacket* @src);

    /// <summary>Free the packet, if the packet is reference counted, it will be unreferenced first.</summary>
    /// <param name="pkt">packet to be freed. The pointer will be set to NULL.</param>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_packet_free(AVPacket** @pkt);

    /// <summary>Convenience function to free all the side data stored. All the other fields stay untouched.</summary>
    /// <param name="pkt">packet</param>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_packet_free_side_data(AVPacket* @pkt);

    /// <summary>Initialize a reference-counted packet from av_malloc()ed data.</summary>
    /// <param name="pkt">packet to be initialized. This function will set the data, size, and buf fields, all others are left untouched.</param>
    /// <param name="data">Data allocated by av_malloc() to be used as packet data. If this function returns successfully, the data is owned by the underlying AVBuffer. The caller may not access the data through other means.</param>
    /// <param name="size">size of data in bytes, without the padding. I.e. the full buffer size is assumed to be size + AV_INPUT_BUFFER_PADDING_SIZE.</param>
    /// <returns>0 on success, a negative AVERROR on error</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_packet_from_data(AVPacket* @pkt, byte* @data, int @size);

    /// <summary>Get side information from packet.</summary>
    /// <param name="pkt">packet</param>
    /// <param name="type">desired side information type</param>
    /// <param name="size">If supplied, *size will be set to the size of the side data or to zero if the desired side data is not present.</param>
    /// <returns>pointer to data if present or NULL otherwise</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern byte* av_packet_get_side_data(AVPacket* @pkt, AVPacketSideDataType @type, ulong* @size);

    /// <summary>Ensure the data described by a given packet is reference counted.</summary>
    /// <param name="pkt">packet whose data should be made reference counted.</param>
    /// <returns>0 on success, a negative AVERROR on error. On failure, the packet is unchanged.</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_packet_make_refcounted(AVPacket* @pkt);

    /// <summary>Create a writable reference for the data described by a given packet, avoiding data copy if possible.</summary>
    /// <param name="pkt">Packet whose data should be made writable.</param>
    /// <returns>0 on success, a negative AVERROR on failure. On failure, the packet is unchanged.</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_packet_make_writable(AVPacket* @pkt);

    /// <summary>Move every field in src to dst and reset src.</summary>
    /// <param name="dst">Destination packet</param>
    /// <param name="src">Source packet, will be reset</param>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_packet_move_ref(AVPacket* @dst, AVPacket* @src);

    /// <summary>Allocate new information of a packet.</summary>
    /// <param name="pkt">packet</param>
    /// <param name="type">side information type</param>
    /// <param name="size">side information size</param>
    /// <returns>pointer to fresh allocated data or NULL otherwise</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern byte* av_packet_new_side_data(AVPacket* @pkt, AVPacketSideDataType @type, ulong @size);

    /// <summary>Pack a dictionary for use in side_data.</summary>
    /// <param name="dict">The dictionary to pack.</param>
    /// <param name="size">pointer to store the size of the returned data</param>
    /// <returns>pointer to data if successful, NULL otherwise</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern byte* av_packet_pack_dictionary(AVDictionary* @dict, ulong* @size);

    /// <summary>Setup a new reference to the data described by a given packet</summary>
    /// <param name="dst">Destination packet. Will be completely overwritten.</param>
    /// <param name="src">Source packet</param>
    /// <returns>0 on success, a negative AVERROR on error. On error, dst will be blank (as if returned by av_packet_alloc()).</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_packet_ref(AVPacket* @dst, AVPacket* @src);

    /// <summary>Convert valid timing fields (timestamps / durations) in a packet from one timebase to another. Timestamps with unknown values (AV_NOPTS_VALUE) will be ignored.</summary>
    /// <param name="pkt">packet on which the conversion will be performed</param>
    /// <param name="tb_src">source timebase, in which the timing fields in pkt are expressed</param>
    /// <param name="tb_dst">destination timebase, to which the timing fields will be converted</param>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_packet_rescale_ts(AVPacket* @pkt, AVRational @tb_src, AVRational @tb_dst);

    /// <summary>Shrink the already allocated side data buffer</summary>
    /// <param name="pkt">packet</param>
    /// <param name="type">side information type</param>
    /// <param name="size">new side information size</param>
    /// <returns>0 on success, &lt; 0 on failure</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_packet_shrink_side_data(AVPacket* @pkt, AVPacketSideDataType @type, ulong @size);

    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public static extern string av_packet_side_data_name(AVPacketSideDataType @type);

    /// <summary>Unpack a dictionary from side_data.</summary>
    /// <param name="data">data from side_data</param>
    /// <param name="size">size of the data</param>
    /// <param name="dict">the metadata storage dictionary</param>
    /// <returns>0 on success, &lt; 0 on failure</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_packet_unpack_dictionary(byte* @data, ulong @size, AVDictionary** @dict);

    /// <summary>Wipe the packet.</summary>
    /// <param name="pkt">The packet to be unreferenced.</param>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_packet_unref(AVPacket* @pkt);

    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_parser_close(AVCodecParserContext* @s);

    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVCodecParserContext* av_parser_init(int @codec_id);

    /// <summary>Iterate over all registered codec parsers.</summary>
    /// <param name="opaque">a pointer where libavcodec will store the iteration state. Must point to NULL to start the iteration.</param>
    /// <returns>the next registered codec parser or NULL when the iteration is finished</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVCodecParser* av_parser_iterate(void** @opaque);

    /// <summary>Parse a packet.</summary>
    /// <param name="s">parser context.</param>
    /// <param name="avctx">codec context.</param>
    /// <param name="poutbuf">set to pointer to parsed buffer or NULL if not yet finished.</param>
    /// <param name="poutbuf_size">set to size of parsed buffer or zero if not yet finished.</param>
    /// <param name="buf">input buffer.</param>
    /// <param name="buf_size">buffer size in bytes without the padding. I.e. the full buffer size is assumed to be buf_size + AV_INPUT_BUFFER_PADDING_SIZE. To signal EOF, this should be 0 (so that the last frame can be output).</param>
    /// <param name="pts">input presentation timestamp.</param>
    /// <param name="dts">input decoding timestamp.</param>
    /// <param name="pos">input byte position in stream.</param>
    /// <returns>the number of bytes of the input bitstream used.</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_parser_parse2(AVCodecParserContext* @s, AVCodecContext* @avctx, byte** @poutbuf, int* @poutbuf_size, byte* @buf, int @buf_size, long @pts, long @dts, long @pos);

    /// <summary>Reduce packet size, correctly zeroing padding</summary>
    /// <param name="pkt">packet</param>
    /// <param name="size">new size</param>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_shrink_packet(AVPacket* @pkt, int @size);

    /// <summary>Encode extradata length to a buffer. Used by xiph codecs.</summary>
    /// <param name="s">buffer to write to; must be at least (v/255+1) bytes long</param>
    /// <param name="v">size of extradata in bytes</param>
    /// <returns>number of bytes written to the buffer.</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint av_xiphlacing(byte* @s, uint @v);

    /// <summary>Modify width and height values so that they will result in a memory buffer that is acceptable for the codec if you do not use any horizontal padding.</summary>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern void avcodec_align_dimensions(AVCodecContext* @s, int* @width, int* @height);

    /// <summary>Modify width and height values so that they will result in a memory buffer that is acceptable for the codec if you also ensure that all line sizes are a multiple of the respective linesize_align[i].</summary>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern void avcodec_align_dimensions2(AVCodecContext* @s, int* @width, int* @height, ref int8 @linesize_align);

    /// <summary>Allocate an AVCodecContext and set its fields to default values. The resulting struct should be freed with avcodec_free_context().</summary>
    /// <param name="codec">if non-NULL, allocate private data and initialize defaults for the given codec. It is illegal to then call avcodec_open2() with a different codec. If NULL, then the codec-specific defaults won&apos;t be initialized, which may result in suboptimal default settings (this is important mainly for encoders, e.g. libx264).</param>
    /// <returns>An AVCodecContext filled with default values or NULL on failure.</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVCodecContext* avcodec_alloc_context3(AVCodec* @codec);

    /// <summary>Converts swscale x/y chroma position to AVChromaLocation.</summary>
    /// <param name="xpos">horizontal chroma sample position</param>
    /// <param name="ypos">vertical   chroma sample position</param>
    [Obsolete("Use av_chroma_location_pos_to_enum() instead.")]
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVChromaLocation avcodec_chroma_pos_to_enum(int @xpos, int @ypos);

    /// <summary>Close a given AVCodecContext and free all the data associated with it (but not the AVCodecContext itself).</summary>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avcodec_close(AVCodecContext* @avctx);

    /// <summary>Return the libavcodec build-time configuration.</summary>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public static extern string avcodec_configuration();

    /// <summary>Decode a subtitle message. Return a negative value on error, otherwise return the number of bytes used. If no subtitle could be decompressed, got_sub_ptr is zero. Otherwise, the subtitle is stored in *sub. Note that AV_CODEC_CAP_DR1 is not available for subtitle codecs. This is for simplicity, because the performance difference is expected to be negligible and reusing a get_buffer written for video codecs would probably perform badly due to a potentially very different allocation pattern.</summary>
    /// <param name="avctx">the codec context</param>
    /// <param name="sub">The preallocated AVSubtitle in which the decoded subtitle will be stored, must be freed with avsubtitle_free if *got_sub_ptr is set.</param>
    /// <param name="got_sub_ptr">Zero if no subtitle could be decompressed, otherwise, it is nonzero.</param>
    /// <param name="avpkt">The input AVPacket containing the input buffer.</param>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avcodec_decode_subtitle2(AVCodecContext* @avctx, AVSubtitle* @sub, int* @got_sub_ptr, AVPacket* @avpkt);

    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avcodec_default_execute(AVCodecContext* @c, avcodec_default_execute_func_func @func, void* @arg, int* @ret, int @count, int @size);

    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avcodec_default_execute2(AVCodecContext* @c, avcodec_default_execute2_func_func @func, void* @arg, int* @ret, int @count);

    /// <summary>The default callback for AVCodecContext.get_buffer2(). It is made public so it can be called by custom get_buffer2() implementations for decoders without AV_CODEC_CAP_DR1 set.</summary>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avcodec_default_get_buffer2(AVCodecContext* @s, AVFrame* @frame, int @flags);

    /// <summary>The default callback for AVCodecContext.get_encode_buffer(). It is made public so it can be called by custom get_encode_buffer() implementations for encoders without AV_CODEC_CAP_DR1 set.</summary>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avcodec_default_get_encode_buffer(AVCodecContext* @s, AVPacket* @pkt, int @flags);

    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVPixelFormat avcodec_default_get_format(AVCodecContext* @s, AVPixelFormat* @fmt);

    /// <summary>Returns descriptor for given codec ID or NULL if no descriptor exists.</summary>
    /// <returns>descriptor for given codec ID or NULL if no descriptor exists.</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVCodecDescriptor* avcodec_descriptor_get(AVCodecID @id);

    /// <summary>Returns codec descriptor with the given name or NULL if no such descriptor exists.</summary>
    /// <returns>codec descriptor with the given name or NULL if no such descriptor exists.</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVCodecDescriptor* avcodec_descriptor_get_by_name(
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name);

    /// <summary>Iterate over all codec descriptors known to libavcodec.</summary>
    /// <param name="prev">previous descriptor. NULL to get the first descriptor.</param>
    /// <returns>next descriptor or NULL after the last descriptor</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVCodecDescriptor* avcodec_descriptor_next(AVCodecDescriptor* @prev);

    /// <summary>@{</summary>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avcodec_encode_subtitle(AVCodecContext* @avctx, byte* @buf, int @buf_size, AVSubtitle* @sub);

    /// <summary>Converts AVChromaLocation to swscale x/y chroma position.</summary>
    /// <param name="xpos">horizontal chroma sample position</param>
    /// <param name="ypos">vertical   chroma sample position</param>
    [Obsolete("Use av_chroma_location_enum_to_pos() instead.")]
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avcodec_enum_to_chroma_pos(int* @xpos, int* @ypos, AVChromaLocation @pos);

    /// <summary>Fill AVFrame audio data and linesize pointers.</summary>
    /// <param name="frame">the AVFrame frame-&gt;nb_samples must be set prior to calling the function. This function fills in frame-&gt;data, frame-&gt;extended_data, frame-&gt;linesize[0].</param>
    /// <param name="nb_channels">channel count</param>
    /// <param name="sample_fmt">sample format</param>
    /// <param name="buf">buffer to use for frame data</param>
    /// <param name="buf_size">size of buffer</param>
    /// <param name="align">plane size sample alignment (0 = default)</param>
    /// <returns>&gt;=0 on success, negative error code on failure</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avcodec_fill_audio_frame(AVFrame* @frame, int @nb_channels, AVSampleFormat @sample_fmt, byte* @buf, int @buf_size, int @align);

    /// <summary>Find the best pixel format to convert to given a certain source pixel format. When converting from one pixel format to another, information loss may occur. For example, when converting from RGB24 to GRAY, the color information will be lost. Similarly, other losses occur when converting from some formats to other formats. avcodec_find_best_pix_fmt_of_2() searches which of the given pixel formats should be used to suffer the least amount of loss. The pixel formats from which it chooses one, are determined by the pix_fmt_list parameter.</summary>
    /// <param name="pix_fmt_list">AV_PIX_FMT_NONE terminated array of pixel formats to choose from</param>
    /// <param name="src_pix_fmt">source pixel format</param>
    /// <param name="has_alpha">Whether the source pixel format alpha channel is used.</param>
    /// <param name="loss_ptr">Combination of flags informing you what kind of losses will occur.</param>
    /// <returns>The best pixel format to convert to or -1 if none was found.</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVPixelFormat avcodec_find_best_pix_fmt_of_list(AVPixelFormat* @pix_fmt_list, AVPixelFormat @src_pix_fmt, int @has_alpha, int* @loss_ptr);

    /// <summary>Find a registered decoder with a matching codec ID.</summary>
    /// <param name="id">AVCodecID of the requested decoder</param>
    /// <returns>A decoder if one was found, NULL otherwise.</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVCodec* avcodec_find_decoder(AVCodecID @id);

    /// <summary>Find a registered decoder with the specified name.</summary>
    /// <param name="name">name of the requested decoder</param>
    /// <returns>A decoder if one was found, NULL otherwise.</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVCodec* avcodec_find_decoder_by_name(
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name);

    /// <summary>Find a registered encoder with a matching codec ID.</summary>
    /// <param name="id">AVCodecID of the requested encoder</param>
    /// <returns>An encoder if one was found, NULL otherwise.</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVCodec* avcodec_find_encoder(AVCodecID @id);

    /// <summary>Find a registered encoder with the specified name.</summary>
    /// <param name="name">name of the requested encoder</param>
    /// <returns>An encoder if one was found, NULL otherwise.</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVCodec* avcodec_find_encoder_by_name(
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name);

    /// <summary>Reset the internal codec state / flush internal buffers. Should be called e.g. when seeking or when switching to a different stream.</summary>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern void avcodec_flush_buffers(AVCodecContext* @avctx);

    /// <summary>Free the codec context and everything associated with it and write NULL to the provided pointer.</summary>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern void avcodec_free_context(AVCodecContext** @avctx);

    /// <summary>Get the AVClass for AVCodecContext. It can be used in combination with AV_OPT_SEARCH_FAKE_OBJ for examining options.</summary>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVClass* avcodec_get_class();

    /// <summary>Retrieve supported hardware configurations for a codec.</summary>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVCodecHWConfig* avcodec_get_hw_config(AVCodec* @codec, int @index);

    /// <summary>Create and return a AVHWFramesContext with values adequate for hardware decoding. This is meant to get called from the get_format callback, and is a helper for preparing a AVHWFramesContext for AVCodecContext.hw_frames_ctx. This API is for decoding with certain hardware acceleration modes/APIs only.</summary>
    /// <param name="avctx">The context which is currently calling get_format, and which implicitly contains all state needed for filling the returned AVHWFramesContext properly.</param>
    /// <param name="device_ref">A reference to the AVHWDeviceContext describing the device which will be used by the hardware decoder.</param>
    /// <param name="hw_pix_fmt">The hwaccel format you are going to return from get_format.</param>
    /// <param name="out_frames_ref">On success, set to a reference to an _uninitialized_ AVHWFramesContext, created from the given device_ref. Fields will be set to values required for decoding. Not changed if an error is returned.</param>
    /// <returns>zero on success, a negative value on error. The following error codes have special semantics: AVERROR(ENOENT): the decoder does not support this functionality. Setup is always manual, or it is a decoder which does not support setting AVCodecContext.hw_frames_ctx at all, or it is a software format. AVERROR(EINVAL): it is known that hardware decoding is not supported for this configuration, or the device_ref is not supported for the hwaccel referenced by hw_pix_fmt.</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avcodec_get_hw_frames_parameters(AVCodecContext* @avctx, AVBufferRef* @device_ref, AVPixelFormat @hw_pix_fmt, AVBufferRef** @out_frames_ref);

    /// <summary>Get the name of a codec.</summary>
    /// <returns>a static string identifying the codec; never NULL</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public static extern string avcodec_get_name(AVCodecID @id);

    /// <summary>Get the AVClass for AVSubtitleRect. It can be used in combination with AV_OPT_SEARCH_FAKE_OBJ for examining options.</summary>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVClass* avcodec_get_subtitle_rect_class();

    /// <summary>Get the type of the given codec.</summary>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVMediaType avcodec_get_type(AVCodecID @codec_id);

    /// <summary>Returns a positive value if s is open (i.e. avcodec_open2() was called on it with no corresponding avcodec_close()), 0 otherwise.</summary>
    /// <returns>a positive value if s is open (i.e. avcodec_open2() was called on it with no corresponding avcodec_close()), 0 otherwise.</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avcodec_is_open(AVCodecContext* @s);

    /// <summary>Return the libavcodec license.</summary>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public static extern string avcodec_license();

    /// <summary>Initialize the AVCodecContext to use the given AVCodec. Prior to using this function the context has to be allocated with avcodec_alloc_context3().</summary>
    /// <param name="avctx">The context to initialize.</param>
    /// <param name="codec">The codec to open this context for. If a non-NULL codec has been previously passed to avcodec_alloc_context3() or for this context, then this parameter MUST be either NULL or equal to the previously passed codec.</param>
    /// <param name="options">A dictionary filled with AVCodecContext and codec-private options. On return this object will be filled with options that were not found.</param>
    /// <returns>zero on success, a negative value on error</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avcodec_open2(AVCodecContext* @avctx, AVCodec* @codec, AVDictionary** @options);

    /// <summary>Allocate a new AVCodecParameters and set its fields to default values (unknown/invalid/0). The returned struct must be freed with avcodec_parameters_free().</summary>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVCodecParameters* avcodec_parameters_alloc();

    /// <summary>Copy the contents of src to dst. Any allocated fields in dst are freed and replaced with newly allocated duplicates of the corresponding fields in src.</summary>
    /// <returns>&gt;= 0 on success, a negative AVERROR code on failure.</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avcodec_parameters_copy(AVCodecParameters* @dst, AVCodecParameters* @src);

    /// <summary>Free an AVCodecParameters instance and everything associated with it and write NULL to the supplied pointer.</summary>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern void avcodec_parameters_free(AVCodecParameters** @par);

    /// <summary>Fill the parameters struct based on the values from the supplied codec context. Any allocated fields in par are freed and replaced with duplicates of the corresponding fields in codec.</summary>
    /// <returns>&gt;= 0 on success, a negative AVERROR code on failure</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avcodec_parameters_from_context(AVCodecParameters* @par, AVCodecContext* @codec);

    /// <summary>Fill the codec context based on the values from the supplied codec parameters. Any allocated fields in codec that have a corresponding field in par are freed and replaced with duplicates of the corresponding field in par. Fields in codec that do not have a counterpart in par are not touched.</summary>
    /// <returns>&gt;= 0 on success, a negative AVERROR code on failure.</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avcodec_parameters_to_context(AVCodecContext* @codec, AVCodecParameters* @par);

    /// <summary>Return a value representing the fourCC code associated to the pixel format pix_fmt, or 0 if no associated fourCC code can be found.</summary>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint avcodec_pix_fmt_to_codec_tag(AVPixelFormat @pix_fmt);

    /// <summary>Return a name for the specified profile, if available.</summary>
    /// <param name="codec_id">the ID of the codec to which the requested profile belongs</param>
    /// <param name="profile">the profile value for which a name is requested</param>
    /// <returns>A name for the profile if found, NULL otherwise.</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public static extern string avcodec_profile_name(AVCodecID @codec_id, int @profile);

    /// <summary>Return decoded output data from a decoder or encoder (when the AV_CODEC_FLAG_RECON_FRAME flag is used).</summary>
    /// <param name="avctx">codec context</param>
    /// <param name="frame">This will be set to a reference-counted video or audio frame (depending on the decoder type) allocated by the codec. Note that the function will always call av_frame_unref(frame) before doing anything else.</param>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avcodec_receive_frame(AVCodecContext* @avctx, AVFrame* @frame);

    /// <summary>Read encoded data from the encoder.</summary>
    /// <param name="avctx">codec context</param>
    /// <param name="avpkt">This will be set to a reference-counted packet allocated by the encoder. Note that the function will always call av_packet_unref(avpkt) before doing anything else.</param>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avcodec_receive_packet(AVCodecContext* @avctx, AVPacket* @avpkt);

    /// <summary>Supply a raw video or audio frame to the encoder. Use avcodec_receive_packet() to retrieve buffered output packets.</summary>
    /// <param name="avctx">codec context</param>
    /// <param name="frame">AVFrame containing the raw audio or video frame to be encoded. Ownership of the frame remains with the caller, and the encoder will not write to the frame. The encoder may create a reference to the frame data (or copy it if the frame is not reference-counted). It can be NULL, in which case it is considered a flush packet.  This signals the end of the stream. If the encoder still has packets buffered, it will return them after this call. Once flushing mode has been entered, additional flush packets are ignored, and sending frames will return AVERROR_EOF.</param>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avcodec_send_frame(AVCodecContext* @avctx, AVFrame* @frame);

    /// <summary>Supply raw packet data as input to a decoder.</summary>
    /// <param name="avctx">codec context</param>
    /// <param name="avpkt">The input AVPacket. Usually, this will be a single video frame, or several complete audio frames. Ownership of the packet remains with the caller, and the decoder will not write to the packet. The decoder may create a reference to the packet data (or copy it if the packet is not reference-counted). Unlike with older APIs, the packet is always fully consumed, and if it contains multiple frames (e.g. some audio codecs), will require you to call avcodec_receive_frame() multiple times afterwards before you can send a new packet. It can be NULL (or an AVPacket with data set to NULL and size set to 0); in this case, it is considered a flush packet, which signals the end of the stream. Sending the first flush packet will return success. Subsequent ones are unnecessary and will return AVERROR_EOF. If the decoder still has frames buffered, it will return them after sending a flush packet.</param>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int avcodec_send_packet(AVCodecContext* @avctx, AVPacket* @avpkt);

    /// <summary>@}</summary>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern void avcodec_string(byte* @buf, int @buf_size, AVCodecContext* @enc, int @encode);

    /// <summary>Return the LIBAVCODEC_VERSION_INT constant.</summary>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint avcodec_version();

    /// <summary>Free all allocated data in the given subtitle struct.</summary>
    /// <param name="sub">AVSubtitle to free.</param>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern void avsubtitle_free(AVSubtitle* @sub);
}
