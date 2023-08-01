using System;
using System.Runtime.InteropServices;
using FFmpeg.Util;

namespace FFmpeg;

public static unsafe partial class FFmpeg {
    /// <summary>Add an index entry into a sorted list. Update the entry if the list already contains it.</summary>
    /// <param name="timestamp">timestamp in the time base of the given stream</param>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_add_index_entry(AVStream* @st, long @pos, long @timestamp, int @size, int @distance, int @flags);
    
    /// <summary>Add two rationals.</summary>
    /// <param name="b">First rational</param>
    /// <param name="c">Second rational</param>
    /// <returns>b+c</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVRational av_add_q(AVRational @b, AVRational @c);
    
    /// <summary>Add a value to a timestamp.</summary>
    /// <param name="ts_tb">Input timestamp time base</param>
    /// <param name="ts">Input timestamp</param>
    /// <param name="inc_tb">Time base of `inc`</param>
    /// <param name="inc">Value to be added</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern long av_add_stable(AVRational @ts_tb, long @ts, AVRational @inc_tb, long @inc);
    
    /// <summary>Read data and append it to the current content of the AVPacket. If pkt-&gt;size is 0 this is identical to av_get_packet. Note that this uses av_grow_packet and thus involves a realloc which is inefficient. Thus this function should only be used when there is no reasonable way to know (an upper bound of) the final size.</summary>
    /// <param name="s">associated IO context</param>
    /// <param name="pkt">packet</param>
    /// <param name="size">amount of data to read</param>
    /// <returns>&gt;0 (read size) if OK, AVERROR_xxx otherwise, previous data will not be lost even if an error occurs.</returns>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_append_packet(AVIOContext* @s, AVPacket* @pkt, int @size);
    
    /// <summary>Allocate an AVAudioFifo.</summary>
    /// <param name="sample_fmt">sample format</param>
    /// <param name="channels">number of channels</param>
    /// <param name="nb_samples">initial allocation size, in samples</param>
    /// <returns>newly allocated AVAudioFifo, or NULL on error</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVAudioFifo* av_audio_fifo_alloc(AVSampleFormat @sample_fmt, int @channels, int @nb_samples);
    
    /// <summary>Drain data from an AVAudioFifo.</summary>
    /// <param name="af">AVAudioFifo to drain</param>
    /// <param name="nb_samples">number of samples to drain</param>
    /// <returns>0 if OK, or negative AVERROR code on failure</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_audio_fifo_drain(AVAudioFifo* @af, int @nb_samples);
    
    /// <summary>Free an AVAudioFifo.</summary>
    /// <param name="af">AVAudioFifo to free</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_audio_fifo_free(AVAudioFifo* @af);
    
    /// <summary>Peek data from an AVAudioFifo.</summary>
    /// <param name="af">AVAudioFifo to read from</param>
    /// <param name="data">audio data plane pointers</param>
    /// <param name="nb_samples">number of samples to peek</param>
    /// <returns>number of samples actually peek, or negative AVERROR code on failure. The number of samples actually peek will not be greater than nb_samples, and will only be less than nb_samples if av_audio_fifo_size is less than nb_samples.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_audio_fifo_peek(AVAudioFifo* @af, void** @data, int @nb_samples);
    
    /// <summary>Peek data from an AVAudioFifo.</summary>
    /// <param name="af">AVAudioFifo to read from</param>
    /// <param name="data">audio data plane pointers</param>
    /// <param name="nb_samples">number of samples to peek</param>
    /// <param name="offset">offset from current read position</param>
    /// <returns>number of samples actually peek, or negative AVERROR code on failure. The number of samples actually peek will not be greater than nb_samples, and will only be less than nb_samples if av_audio_fifo_size is less than nb_samples.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_audio_fifo_peek_at(AVAudioFifo* @af, void** @data, int @nb_samples, int @offset);
    
    /// <summary>Read data from an AVAudioFifo.</summary>
    /// <param name="af">AVAudioFifo to read from</param>
    /// <param name="data">audio data plane pointers</param>
    /// <param name="nb_samples">number of samples to read</param>
    /// <returns>number of samples actually read, or negative AVERROR code on failure. The number of samples actually read will not be greater than nb_samples, and will only be less than nb_samples if av_audio_fifo_size is less than nb_samples.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_audio_fifo_read(AVAudioFifo* @af, void** @data, int @nb_samples);
    
    /// <summary>Reallocate an AVAudioFifo.</summary>
    /// <param name="af">AVAudioFifo to reallocate</param>
    /// <param name="nb_samples">new allocation size, in samples</param>
    /// <returns>0 if OK, or negative AVERROR code on failure</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_audio_fifo_realloc(AVAudioFifo* @af, int @nb_samples);
    
    /// <summary>Reset the AVAudioFifo buffer.</summary>
    /// <param name="af">AVAudioFifo to reset</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_audio_fifo_reset(AVAudioFifo* @af);
    
    /// <summary>Get the current number of samples in the AVAudioFifo available for reading.</summary>
    /// <param name="af">the AVAudioFifo to query</param>
    /// <returns>number of samples available for reading</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_audio_fifo_size(AVAudioFifo* @af);
    
    /// <summary>Get the current number of samples in the AVAudioFifo available for writing.</summary>
    /// <param name="af">the AVAudioFifo to query</param>
    /// <returns>number of samples available for writing</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_audio_fifo_space(AVAudioFifo* @af);
    
    /// <summary>Write data to an AVAudioFifo.</summary>
    /// <param name="af">AVAudioFifo to write to</param>
    /// <param name="data">audio data plane pointers</param>
    /// <param name="nb_samples">number of samples to write</param>
    /// <returns>number of samples actually written, or negative AVERROR code on failure. If successful, the number of samples actually written will always be nb_samples.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_audio_fifo_write(AVAudioFifo* @af, void** @data, int @nb_samples);
    
    /// <summary>Append a description of a channel layout to a bprint buffer.</summary>
    [Obsolete("use av_channel_layout_describe()")]
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_bprint_channel_layout(AVBPrint* @bp, int @nb_channels, ulong @channel_layout);

    /// <summary>Allocate an AVBuffer of the given size using av_malloc().</summary>
    /// <returns>an AVBufferRef of given size or NULL when out of memory</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVBufferRef* av_buffer_alloc(ulong @size);
    
    /// <summary>Same as av_buffer_alloc(), except the returned buffer will be initialized to zero.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVBufferRef* av_buffer_allocz(ulong @size);
    
    /// <summary>Create an AVBuffer from an existing array.</summary>
    /// <param name="data">data array</param>
    /// <param name="size">size of data in bytes</param>
    /// <param name="free">a callback for freeing this buffer&apos;s data</param>
    /// <param name="opaque">parameter to be got for processing or passed to free</param>
    /// <param name="flags">a combination of AV_BUFFER_FLAG_*</param>
    /// <returns>an AVBufferRef referring to data on success, NULL on failure.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVBufferRef* av_buffer_create(byte* @data, ulong @size, av_buffer_create_free_func @free, void* @opaque, int @flags);
    
    /// <summary>Default free callback, which calls av_free() on the buffer data. This function is meant to be passed to av_buffer_create(), not called directly.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_buffer_default_free(void* @opaque, byte* @data);
    
    /// <summary>Returns the opaque parameter set by av_buffer_create.</summary>
    /// <returns>the opaque parameter set by av_buffer_create.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void* av_buffer_get_opaque(AVBufferRef* @buf);
    
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_buffer_get_ref_count(AVBufferRef* @buf);
    
    /// <summary>Returns 1 if the caller may write to the data referred to by buf (which is true if and only if buf is the only reference to the underlying AVBuffer). Return 0 otherwise. A positive answer is valid until av_buffer_ref() is called on buf.</summary>
    /// <returns>1 if the caller may write to the data referred to by buf (which is true if and only if buf is the only reference to the underlying AVBuffer). Return 0 otherwise. A positive answer is valid until av_buffer_ref() is called on buf.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_buffer_is_writable(AVBufferRef* @buf);
    
    /// <summary>Create a writable reference from a given buffer reference, avoiding data copy if possible.</summary>
    /// <param name="buf">buffer reference to make writable. On success, buf is either left untouched, or it is unreferenced and a new writable AVBufferRef is written in its place. On failure, buf is left untouched.</param>
    /// <returns>0 on success, a negative AVERROR on failure.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_buffer_make_writable(AVBufferRef** @buf);
    
    /// <summary>Query the original opaque parameter of an allocated buffer in the pool.</summary>
    /// <param name="ref">a buffer reference to a buffer returned by av_buffer_pool_get.</param>
    /// <returns>the opaque parameter set by the buffer allocator function of the buffer pool.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void* av_buffer_pool_buffer_get_opaque(AVBufferRef* @ref);
    
    /// <summary>Allocate a new AVBuffer, reusing an old buffer from the pool when available. This function may be called simultaneously from multiple threads.</summary>
    /// <returns>a reference to the new buffer on success, NULL on error.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVBufferRef* av_buffer_pool_get(AVBufferPool* @pool);
    
    /// <summary>Allocate and initialize a buffer pool.</summary>
    /// <param name="size">size of each buffer in this pool</param>
    /// <param name="alloc">a function that will be used to allocate new buffers when the pool is empty. May be NULL, then the default allocator will be used (av_buffer_alloc()).</param>
    /// <returns>newly created buffer pool on success, NULL on error.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVBufferPool* av_buffer_pool_init(ulong @size, av_buffer_pool_init_alloc_func @alloc);
    
    /// <summary>Allocate and initialize a buffer pool with a more complex allocator.</summary>
    /// <param name="size">size of each buffer in this pool</param>
    /// <param name="opaque">arbitrary user data used by the allocator</param>
    /// <param name="alloc">a function that will be used to allocate new buffers when the pool is empty. May be NULL, then the default allocator will be used (av_buffer_alloc()).</param>
    /// <param name="pool_free">a function that will be called immediately before the pool is freed. I.e. after av_buffer_pool_uninit() is called by the caller and all the frames are returned to the pool and freed. It is intended to uninitialize the user opaque data. May be NULL.</param>
    /// <returns>newly created buffer pool on success, NULL on error.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVBufferPool* av_buffer_pool_init2(ulong @size, void* @opaque, av_buffer_pool_init2_alloc_func @alloc, av_buffer_pool_init2_pool_free_func @pool_free);
    
    /// <summary>Mark the pool as being available for freeing. It will actually be freed only once all the allocated buffers associated with the pool are released. Thus it is safe to call this function while some of the allocated buffers are still in use.</summary>
    /// <param name="pool">pointer to the pool to be freed. It will be set to NULL.</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_buffer_pool_uninit(AVBufferPool** @pool);
    
    /// <summary>Reallocate a given buffer.</summary>
    /// <param name="buf">a buffer reference to reallocate. On success, buf will be unreferenced and a new reference with the required size will be written in its place. On failure buf will be left untouched. *buf may be NULL, then a new buffer is allocated.</param>
    /// <param name="size">required new buffer size.</param>
    /// <returns>0 on success, a negative AVERROR on failure.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_buffer_realloc(AVBufferRef** @buf, ulong @size);
    
    /// <summary>Create a new reference to an AVBuffer.</summary>
    /// <returns>a new AVBufferRef referring to the same AVBuffer as buf or NULL on failure.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVBufferRef* av_buffer_ref(AVBufferRef* @buf);
    
    /// <summary>Ensure dst refers to the same data as src.</summary>
    /// <param name="dst">Pointer to either a valid buffer reference or NULL. On success, this will point to a buffer reference equivalent to src. On failure, dst will be left untouched.</param>
    /// <param name="src">A buffer reference to replace dst with. May be NULL, then this function is equivalent to av_buffer_unref(dst).</param>
    /// <returns>0 on success AVERROR(ENOMEM) on memory allocation failure.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_buffer_replace(AVBufferRef** @dst, AVBufferRef* @src);
    
    /// <summary>Free a given reference and automatically free the buffer if there are no more references to it.</summary>
    /// <param name="buf">the reference to be freed. The pointer is set to NULL on return.</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_buffer_unref(AVBufferRef** @buf);

    /// <summary>Allocate a memory block for an array with av_mallocz().</summary>
    /// <param name="nmemb">Number of elements</param>
    /// <param name="size">Size of the single element</param>
    /// <returns>Pointer to the allocated block, or `NULL` if the block cannot be allocated</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void* av_calloc(ulong @nmemb, ulong @size);
    
    /// <summary>Get a human readable string describing a given channel.</summary>
    /// <param name="buf">pre-allocated buffer where to put the generated string</param>
    /// <param name="buf_size">size in bytes of the buffer.</param>
    /// <param name="channel">the AVChannel whose description to get</param>
    /// <returns>amount of bytes needed to hold the output string, or a negative AVERROR on failure. If the returned value is bigger than buf_size, then the string was truncated.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_channel_description(byte* @buf, ulong @buf_size, AVChannel @channel);
    
    /// <summary>bprint variant of av_channel_description().</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_channel_description_bprint(AVBPrint* @bp, AVChannel @channel_id);
    
    /// <summary>This is the inverse function of av_channel_name().</summary>
    /// <returns>the channel with the given name AV_CHAN_NONE when name does not identify a known channel</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVChannel av_channel_from_string(    
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name);
    
    /// <summary>Get the channel with the given index in a channel layout.</summary>
    /// <param name="channel_layout">input channel layout</param>
    /// <param name="idx">index of the channel</param>
    /// <returns>channel with the index idx in channel_layout on success or AV_CHAN_NONE on failure (if idx is not valid or the channel order is unspecified)</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVChannel av_channel_layout_channel_from_index(AVChannelLayout* @channel_layout, uint @idx);
    
    /// <summary>Get a channel described by the given string.</summary>
    /// <param name="channel_layout">input channel layout</param>
    /// <param name="name">string describing the channel to obtain</param>
    /// <returns>a channel described by the given string in channel_layout on success or AV_CHAN_NONE on failure (if the string is not valid or the channel order is unspecified)</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVChannel av_channel_layout_channel_from_string(AVChannelLayout* @channel_layout,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name);
    
    /// <summary>Check whether a channel layout is valid, i.e. can possibly describe audio data.</summary>
    /// <param name="channel_layout">input channel layout</param>
    /// <returns>1 if channel_layout is valid, 0 otherwise.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_channel_layout_check(AVChannelLayout* @channel_layout);
    
    /// <summary>Check whether two channel layouts are semantically the same, i.e. the same channels are present on the same positions in both.</summary>
    /// <param name="chl">input channel layout</param>
    /// <param name="chl1">input channel layout</param>
    /// <returns>0 if chl and chl1 are equal, 1 if they are not equal. A negative AVERROR code if one or both are invalid.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_channel_layout_compare(AVChannelLayout* @chl, AVChannelLayout* @chl1);
    
    /// <summary>Make a copy of a channel layout. This differs from just assigning src to dst in that it allocates and copies the map for AV_CHANNEL_ORDER_CUSTOM.</summary>
    /// <param name="dst">destination channel layout</param>
    /// <param name="src">source channel layout</param>
    /// <returns>0 on success, a negative AVERROR on error.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_channel_layout_copy(AVChannelLayout* @dst, AVChannelLayout* @src);
    
    /// <summary>Get the default channel layout for a given number of channels.</summary>
    /// <param name="ch_layout">the layout structure to be initialized</param>
    /// <param name="nb_channels">number of channels</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_channel_layout_default(AVChannelLayout* @ch_layout, int @nb_channels);
    
    /// <summary>Get a human-readable string describing the channel layout properties. The string will be in the same format that is accepted by av_channel_layout_from_string(), allowing to rebuild the same channel layout, except for opaque pointers.</summary>
    /// <param name="channel_layout">channel layout to be described</param>
    /// <param name="buf">pre-allocated buffer where to put the generated string</param>
    /// <param name="buf_size">size in bytes of the buffer.</param>
    /// <returns>amount of bytes needed to hold the output string, or a negative AVERROR on failure. If the returned value is bigger than buf_size, then the string was truncated.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_channel_layout_describe(AVChannelLayout* @channel_layout, byte* @buf, ulong @buf_size);
    
    /// <summary>bprint variant of av_channel_layout_describe().</summary>
    /// <returns>0 on success, or a negative AVERROR value on failure.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_channel_layout_describe_bprint(AVChannelLayout* @channel_layout, AVBPrint* @bp);
    
    /// <summary>Get the channel with the given index in channel_layout.</summary>
    [Obsolete("use av_channel_layout_channel_from_index()")]
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern ulong av_channel_layout_extract_channel(ulong @channel_layout, int @index);
    
    /// <summary>Initialize a native channel layout from a bitmask indicating which channels are present.</summary>
    /// <param name="channel_layout">the layout structure to be initialized</param>
    /// <param name="mask">bitmask describing the channel layout</param>
    /// <returns>0 on success AVERROR(EINVAL) for invalid mask values</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_channel_layout_from_mask(AVChannelLayout* @channel_layout, ulong @mask);
    
    /// <summary>Initialize a channel layout from a given string description. The input string can be represented by: - the formal channel layout name (returned by av_channel_layout_describe()) - single or multiple channel names (returned by av_channel_name(), eg. &quot;FL&quot;, or concatenated with &quot;+&quot;, each optionally containing a custom name after a &quot;&quot;, eg. &quot;FL+FR+LFE&quot;) - a decimal or hexadecimal value of a native channel layout (eg. &quot;4&quot; or &quot;0x4&quot;) - the number of channels with default layout (eg. &quot;4c&quot;) - the number of unordered channels (eg. &quot;4C&quot; or &quot;4 channels&quot;) - the ambisonic order followed by optional non-diegetic channels (eg. &quot;ambisonic 2+stereo&quot;)</summary>
    /// <param name="channel_layout">input channel layout</param>
    /// <param name="str">string describing the channel layout</param>
    /// <returns>0 channel layout was detected, AVERROR_INVALIDATATA otherwise</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_channel_layout_from_string(AVChannelLayout* @channel_layout,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @str);
    
    /// <summary>Get the index of a given channel in a channel layout. In case multiple channels are found, only the first match will be returned.</summary>
    /// <param name="channel_layout">input channel layout</param>
    /// <param name="channel">the channel whose index to obtain</param>
    /// <returns>index of channel in channel_layout on success or a negative number if channel is not present in channel_layout.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_channel_layout_index_from_channel(AVChannelLayout* @channel_layout, AVChannel @channel);
    
    /// <summary>Get the index in a channel layout of a channel described by the given string. In case multiple channels are found, only the first match will be returned.</summary>
    /// <param name="channel_layout">input channel layout</param>
    /// <param name="name">string describing the channel whose index to obtain</param>
    /// <returns>a channel index described by the given string, or a negative AVERROR value.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_channel_layout_index_from_string(AVChannelLayout* @channel_layout,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name);
    
    /// <summary>Iterate over all standard channel layouts.</summary>
    /// <param name="opaque">a pointer where libavutil will store the iteration state. Must point to NULL to start the iteration.</param>
    /// <returns>the standard channel layout or NULL when the iteration is finished</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVChannelLayout* av_channel_layout_standard(void** @opaque);
    
    /// <summary>Find out what channels from a given set are present in a channel layout, without regard for their positions.</summary>
    /// <param name="channel_layout">input channel layout</param>
    /// <param name="mask">a combination of AV_CH_* representing a set of channels</param>
    /// <returns>a bitfield representing all the channels from mask that are present in channel_layout</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern ulong av_channel_layout_subset(AVChannelLayout* @channel_layout, ulong @mask);
    
    /// <summary>Free any allocated data in the channel layout and reset the channel count to 0.</summary>
    /// <param name="channel_layout">the layout structure to be uninitialized</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_channel_layout_uninit(AVChannelLayout* @channel_layout);
    
    /// <summary>Get a human readable string in an abbreviated form describing a given channel. This is the inverse function of av_channel_from_string().</summary>
    /// <param name="buf">pre-allocated buffer where to put the generated string</param>
    /// <param name="buf_size">size in bytes of the buffer.</param>
    /// <param name="channel">the AVChannel whose name to get</param>
    /// <returns>amount of bytes needed to hold the output string, or a negative AVERROR on failure. If the returned value is bigger than buf_size, then the string was truncated.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_channel_name(byte* @buf, ulong @buf_size, AVChannel @channel);
    
    /// <summary>bprint variant of av_channel_name().</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_channel_name_bprint(AVBPrint* @bp, AVChannel @channel_id);
    
    /// <summary>Converts AVChromaLocation to swscale x/y chroma position.</summary>
    /// <param name="xpos">horizontal chroma sample position</param>
    /// <param name="ypos">vertical   chroma sample position</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_chroma_location_enum_to_pos(int* @xpos, int* @ypos, AVChromaLocation @pos);
    
    /// <summary>Returns the AVChromaLocation value for name or an AVError if not found.</summary>
    /// <returns>the AVChromaLocation value for name or an AVError if not found.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_chroma_location_from_name(    
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name);
    
    /// <summary>Returns the name for provided chroma location or NULL if unknown.</summary>
    /// <returns>the name for provided chroma location or NULL if unknown.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public static extern string av_chroma_location_name(AVChromaLocation @location);
    
    /// <summary>Converts swscale x/y chroma position to AVChromaLocation.</summary>
    /// <param name="xpos">horizontal chroma sample position</param>
    /// <param name="ypos">vertical   chroma sample position</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVChromaLocation av_chroma_location_pos_to_enum(int @xpos, int @ypos);

    /// <summary>Returns the AVColorPrimaries value for name or an AVError if not found.</summary>
    /// <returns>the AVColorPrimaries value for name or an AVError if not found.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_color_primaries_from_name(    
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name);
    
    /// <summary>Returns the name for provided color primaries or NULL if unknown.</summary>
    /// <returns>the name for provided color primaries or NULL if unknown.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public static extern string av_color_primaries_name(AVColorPrimaries @primaries);
    
    /// <summary>Returns the AVColorRange value for name or an AVError if not found.</summary>
    /// <returns>the AVColorRange value for name or an AVError if not found.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_color_range_from_name(    
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name);
    
    /// <summary>Returns the name for provided color range or NULL if unknown.</summary>
    /// <returns>the name for provided color range or NULL if unknown.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public static extern string av_color_range_name(AVColorRange @range);
    
    /// <summary>Returns the AVColorSpace value for name or an AVError if not found.</summary>
    /// <returns>the AVColorSpace value for name or an AVError if not found.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_color_space_from_name(    
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name);
    
    /// <summary>Returns the name for provided color space or NULL if unknown.</summary>
    /// <returns>the name for provided color space or NULL if unknown.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public static extern string av_color_space_name(AVColorSpace @space);
    
    /// <summary>Returns the AVColorTransferCharacteristic value for name or an AVError if not found.</summary>
    /// <returns>the AVColorTransferCharacteristic value for name or an AVError if not found.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_color_transfer_from_name(    
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name);
    
    /// <summary>Returns the name for provided color transfer or NULL if unknown.</summary>
    /// <returns>the name for provided color transfer or NULL if unknown.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public static extern string av_color_transfer_name(AVColorTransferCharacteristic @transfer);
    
    /// <summary>Compare the remainders of two integer operands divided by a common divisor.</summary>
    /// <param name="a">Operand</param>
    /// <param name="b">Operand</param>
    /// <param name="mod">Divisor; must be a power of 2</param>
    /// <returns>- a negative value if `a % mod &lt; b % mod` - a positive value if `a % mod &gt; b % mod` - zero             if `a % mod == b % mod`</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern long av_compare_mod(ulong @a, ulong @b, ulong @mod);
    
    /// <summary>Compare two timestamps each in its own time base.</summary>
    /// <returns>One of the following values: - -1 if `ts_a` is before `ts_b` - 1 if `ts_a` is after `ts_b` - 0 if they represent the same position</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_compare_ts(long @ts_a, AVRational @tb_a, long @ts_b, AVRational @tb_b);
    
    /// <summary>Allocate an AVContentLightMetadata structure and set its fields to default values. The resulting struct can be freed using av_freep().</summary>
    /// <returns>An AVContentLightMetadata filled with default values or NULL on failure.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVContentLightMetadata* av_content_light_metadata_alloc(ulong* @size);
    
    /// <summary>Allocate a complete AVContentLightMetadata and add it to the frame.</summary>
    /// <param name="frame">The frame which side data is added to.</param>
    /// <returns>The AVContentLightMetadata structure to be filled by caller.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVContentLightMetadata* av_content_light_metadata_create_side_data(AVFrame* @frame);

    /// <summary>Returns the number of logical CPU cores present.</summary>
    /// <returns>the number of logical CPU cores present.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_cpu_count();
    
    /// <summary>Overrides cpu count detection and forces the specified count. Count &lt; 1 disables forcing of specific count.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_cpu_force_count(int @count);
    
    /// <summary>Get the maximum data alignment that may be required by FFmpeg.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern ulong av_cpu_max_align();
    
    /// <summary>Convert a double precision floating point number to a rational.</summary>
    /// <param name="d">`double` to convert</param>
    /// <param name="max">Maximum allowed numerator and denominator</param>
    /// <returns>`d` in AVRational form</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVRational av_d2q(double @d, int @max);

    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVClassCategory av_default_get_category(void* @ptr);
    
    /// <summary>Return the context name</summary>
    /// <param name="ctx">The AVClass context</param>
    /// <returns>The AVClass class_name</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public static extern string av_default_item_name(void* @ctx);

    /// <summary>Copy entries from one AVDictionary struct into another.</summary>
    /// <param name="dst">Pointer to a pointer to a AVDictionary struct to copy into. If *dst is NULL, this function will allocate a struct for you and put it in *dst</param>
    /// <param name="src">Pointer to the source AVDictionary struct to copy items from.</param>
    /// <param name="flags">Flags to use when setting entries in *dst</param>
    /// <returns>0 on success, negative AVERROR code on failure. If dst was allocated by this function, callers should free the associated memory.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_dict_copy(AVDictionary** @dst, AVDictionary* @src, int @flags);
    
    /// <summary>Get number of entries in dictionary.</summary>
    /// <param name="m">dictionary</param>
    /// <returns>number of entries in dictionary</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_dict_count(AVDictionary* @m);
    
    /// <summary>Free all the memory allocated for an AVDictionary struct and all keys and values.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_dict_free(AVDictionary** @m);
    
    /// <summary>Get a dictionary entry with matching key.</summary>
    /// <param name="key">Matching key</param>
    /// <param name="prev">Set to the previous matching element to find the next. If set to NULL the first matching element is returned.</param>
    /// <param name="flags">A collection of AV_DICT_* flags controlling how the entry is retrieved</param>
    /// <returns>Found entry or NULL in case no matching entry was found in the dictionary</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVDictionaryEntry* av_dict_get(AVDictionary* @m,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @key, AVDictionaryEntry* @prev, int @flags);
    
    /// <summary>Get dictionary entries as a string.</summary>
    /// <param name="m">The dictionary</param>
    /// <param name="buffer">Pointer to buffer that will be allocated with string containg entries. Buffer must be freed by the caller when is no longer needed.</param>
    /// <param name="key_val_sep">Character used to separate key from value</param>
    /// <param name="pairs_sep">Character used to separate two pairs from each other</param>
    /// <returns>&gt;= 0 on success, negative on error</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_dict_get_string(AVDictionary* @m, byte** @buffer, byte @key_val_sep, byte @pairs_sep);
    
    /// <summary>Iterate over a dictionary</summary>
    /// <param name="m">The dictionary to iterate over</param>
    /// <param name="prev">Pointer to the previous AVDictionaryEntry, NULL initially</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVDictionaryEntry* av_dict_iterate(AVDictionary* @m, AVDictionaryEntry* @prev);
    
    /// <summary>Parse the key/value pairs list and add the parsed entries to a dictionary.</summary>
    /// <param name="key_val_sep">A 0-terminated list of characters used to separate key from value</param>
    /// <param name="pairs_sep">A 0-terminated list of characters used to separate two pairs from each other</param>
    /// <param name="flags">Flags to use when adding to the dictionary. ::AV_DICT_DONT_STRDUP_KEY and ::AV_DICT_DONT_STRDUP_VAL are ignored since the key/value tokens will always be duplicated.</param>
    /// <returns>0 on success, negative AVERROR code on failure</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_dict_parse_string(AVDictionary** @pm,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @str,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @key_val_sep,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @pairs_sep, int @flags);
    
    /// <summary>Set the given entry in *pm, overwriting an existing entry.</summary>
    /// <param name="pm">Pointer to a pointer to a dictionary struct. If *pm is NULL a dictionary struct is allocated and put in *pm.</param>
    /// <param name="key">Entry key to add to *pm (will either be av_strduped or added as a new key depending on flags)</param>
    /// <param name="value">Entry value to add to *pm (will be av_strduped or added as a new key depending on flags). Passing a NULL value will cause an existing entry to be deleted.</param>
    /// <returns>&gt;= 0 on success otherwise an error code &lt; 0</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_dict_set(AVDictionary** @pm,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @key,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @value, int @flags);
    
    /// <summary>Convenience wrapper for av_dict_set() that converts the value to a string and stores it.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_dict_set_int(AVDictionary** @pm,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @key, long @value, int @flags);
    
    /// <summary>Flip the input matrix horizontally and/or vertically.</summary>
    /// <param name="matrix">a transformation matrix</param>
    /// <param name="hflip">whether the matrix should be flipped horizontally</param>
    /// <param name="vflip">whether the matrix should be flipped vertically</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_display_matrix_flip(ref int9 @matrix, int @hflip, int @vflip);
    
    /// <summary>Extract the rotation component of the transformation matrix.</summary>
    /// <param name="matrix">the transformation matrix</param>
    /// <returns>the angle (in degrees) by which the transformation rotates the frame counterclockwise. The angle will be in range [-180.0, 180.0], or NaN if the matrix is singular.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern double av_display_rotation_get(in int9 @matrix);
    
    /// <summary>Initialize a transformation matrix describing a pure clockwise rotation by the specified angle (in degrees).</summary>
    /// <param name="matrix">a transformation matrix (will be fully overwritten by this function)</param>
    /// <param name="angle">rotation angle in degrees.</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_display_rotation_set(ref int9 @matrix, double @angle);

    /// <summary>Divide one rational by another.</summary>
    /// <param name="b">First rational</param>
    /// <param name="c">Second rational</param>
    /// <returns>b/c</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVRational av_div_q(AVRational @b, AVRational @c);

    /// <summary>Allocate an AVDynamicHDRPlus structure and set its fields to default values. The resulting struct can be freed using av_freep().</summary>
    /// <returns>An AVDynamicHDRPlus filled with default values or NULL on failure.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVDynamicHDRPlus* av_dynamic_hdr_plus_alloc(ulong* @size);
    
    /// <summary>Allocate a complete AVDynamicHDRPlus and add it to the frame.</summary>
    /// <param name="frame">The frame which side data is added to.</param>
    /// <returns>The AVDynamicHDRPlus structure to be filled by caller or NULL on failure.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVDynamicHDRPlus* av_dynamic_hdr_plus_create_side_data(AVFrame* @frame);
    
    /// <summary>Add the pointer to an element to a dynamic array.</summary>
    /// <param name="tab_ptr">Pointer to the array to grow</param>
    /// <param name="nb_ptr">Pointer to the number of elements in the array</param>
    /// <param name="elem">Element to add</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_dynarray_add(void* @tab_ptr, int* @nb_ptr, void* @elem);
    
    /// <summary>Add an element to a dynamic array.</summary>
    /// <returns>&gt;=0 on success, negative otherwise</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_dynarray_add_nofree(void* @tab_ptr, int* @nb_ptr, void* @elem);
    
    /// <summary>Add an element of size `elem_size` to a dynamic array.</summary>
    /// <param name="tab_ptr">Pointer to the array to grow</param>
    /// <param name="nb_ptr">Pointer to the number of elements in the array</param>
    /// <param name="elem_size">Size in bytes of an element in the array</param>
    /// <param name="elem_data">Pointer to the data of the element to add. If `NULL`, the space of the newly added element is allocated but left uninitialized.</param>
    /// <returns>Pointer to the data of the element to copy in the newly allocated space</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void* av_dynarray2_add(void** @tab_ptr, int* @nb_ptr, ulong @elem_size, byte* @elem_data);
    
    /// <summary>Allocate a buffer, reusing the given one if large enough.</summary>
    /// <param name="ptr">Pointer to pointer to an already allocated buffer. `*ptr` will be overwritten with pointer to new buffer on success or `NULL` on failure</param>
    /// <param name="size">Pointer to the size of buffer `*ptr`. `*size` is updated to the new allocated size, in particular 0 in case of failure.</param>
    /// <param name="min_size">Desired minimal size of buffer `*ptr`</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_fast_malloc(void* @ptr, uint* @size, ulong @min_size);
    
    /// <summary>Allocate and clear a buffer, reusing the given one if large enough.</summary>
    /// <param name="ptr">Pointer to pointer to an already allocated buffer. `*ptr` will be overwritten with pointer to new buffer on success or `NULL` on failure</param>
    /// <param name="size">Pointer to the size of buffer `*ptr`. `*size` is updated to the new allocated size, in particular 0 in case of failure.</param>
    /// <param name="min_size">Desired minimal size of buffer `*ptr`</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_fast_mallocz(void* @ptr, uint* @size, ulong @min_size);

    /// <summary>Reallocate the given buffer if it is not large enough, otherwise do nothing.</summary>
    /// <param name="ptr">Already allocated buffer, or `NULL`</param>
    /// <param name="size">Pointer to the size of buffer `ptr`. `*size` is updated to the new allocated size, in particular 0 in case of failure.</param>
    /// <param name="min_size">Desired minimal size of buffer `ptr`</param>
    /// <returns>`ptr` if the buffer is large enough, a pointer to newly reallocated buffer if the buffer was not large enough, or `NULL` in case of error</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void* av_fast_realloc(void* @ptr, uint* @size, ulong @min_size);
    
    /// <summary>Read the file with name filename, and put its content in a newly allocated buffer or map it with mmap() when available. In case of success set *bufptr to the read or mmapped buffer, and *size to the size in bytes of the buffer in *bufptr. Unlike mmap this function succeeds with zero sized files, in this case *bufptr will be set to NULL and *size will be set to 0. The returned buffer must be released with av_file_unmap().</summary>
    /// <param name="filename">path to the file</param>
    /// <param name="bufptr">pointee is set to the mapped or allocated buffer</param>
    /// <param name="size">pointee is set to the size in bytes of the buffer</param>
    /// <param name="log_offset">loglevel offset used for logging</param>
    /// <param name="log_ctx">context used for logging</param>
    /// <returns>a non negative number in case of success, a negative value corresponding to an AVERROR error code in case of failure</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_file_map(    
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @filename, byte** @bufptr, ulong* @size, int @log_offset, void* @log_ctx);
    
    /// <summary>Unmap or free the buffer bufptr created by av_file_map().</summary>
    /// <param name="bufptr">the buffer previously created with av_file_map()</param>
    /// <param name="size">size in bytes of bufptr, must be the same as returned by av_file_map()</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_file_unmap(byte* @bufptr, ulong @size);

    /// <summary>Compute what kind of losses will occur when converting from one specific pixel format to another. When converting from one pixel format to another, information loss may occur. For example, when converting from RGB24 to GRAY, the color information will be lost. Similarly, other losses occur when converting from some formats to other formats. These losses can involve loss of chroma, but also loss of resolution, loss of color depth, loss due to the color space conversion, loss of the alpha bits or loss due to color quantization. av_get_fix_fmt_loss() informs you about the various types of losses which will occur when converting from one pixel format to another.</summary>
    /// <param name="src_pix_fmt">source pixel format</param>
    /// <param name="has_alpha">Whether the source pixel format alpha channel is used.</param>
    /// <returns>Combination of flags informing you what kind of losses will occur (maximum loss for an invalid dst_pix_fmt).</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVPixelFormat av_find_best_pix_fmt_of_2(AVPixelFormat @dst_pix_fmt1, AVPixelFormat @dst_pix_fmt2, AVPixelFormat @src_pix_fmt, int @has_alpha, int* @loss_ptr);

    /// <summary>Find the value in a list of rationals nearest a given reference rational.</summary>
    /// <param name="q">Reference rational</param>
    /// <param name="q_list">Array of rationals terminated by `{0, 0}`</param>
    /// <returns>Index of the nearest value found in the array</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_find_nearest_q_idx(AVRational @q, AVRational* @q_list);

    /// <summary>Open a file using a UTF-8 filename. The API of this function matches POSIX fopen(), errors are returned through errno.</summary>
    [Obsolete("Avoid using it, as on Windows, the FILE* allocated by this function may be allocated with a different CRT than the caller who uses the FILE*. No replacement provided in public API.")]
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern _iobuf* av_fopen_utf8(    
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @path,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @mode);
    
    /// <summary>Disables cpu detection and forces the specified flags. -1 is a special case that disables forcing of specific flags.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_force_cpu_flags(int @flags);

    /// <summary>Fill the provided buffer with a string containing a FourCC (four-character code) representation.</summary>
    /// <param name="buf">a buffer with size in bytes of at least AV_FOURCC_MAX_STRING_SIZE</param>
    /// <param name="fourcc">the fourcc to represent</param>
    /// <returns>the buffer in input</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern byte* av_fourcc_make_string(byte* @buf, uint @fourcc);
    
    /// <summary>Allocate an AVFrame and set its fields to default values. The resulting struct must be freed using av_frame_free().</summary>
    /// <returns>An AVFrame filled with default values or NULL on failure.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVFrame* av_frame_alloc();
    
    /// <summary>Crop the given video AVFrame according to its crop_left/crop_top/crop_right/ crop_bottom fields. If cropping is successful, the function will adjust the data pointers and the width/height fields, and set the crop fields to 0.</summary>
    /// <param name="frame">the frame which should be cropped</param>
    /// <param name="flags">Some combination of AV_FRAME_CROP_* flags, or 0.</param>
    /// <returns>&gt;= 0 on success, a negative AVERROR on error. If the cropping fields were invalid, AVERROR(ERANGE) is returned, and nothing is changed.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_frame_apply_cropping(AVFrame* @frame, int @flags);
    
    /// <summary>Create a new frame that references the same data as src.</summary>
    /// <returns>newly created AVFrame on success, NULL on error.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVFrame* av_frame_clone(AVFrame* @src);
    
    /// <summary>Copy the frame data from src to dst.</summary>
    /// <returns>&gt;= 0 on success, a negative AVERROR on error.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_frame_copy(AVFrame* @dst, AVFrame* @src);
    
    /// <summary>Copy only &quot;metadata&quot; fields from src to dst.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_frame_copy_props(AVFrame* @dst, AVFrame* @src);
    
    /// <summary>Free the frame and any dynamically allocated objects in it, e.g. extended_data. If the frame is reference counted, it will be unreferenced first.</summary>
    /// <param name="frame">frame to be freed. The pointer will be set to NULL.</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_frame_free(AVFrame** @frame);
    
    /// <summary>Allocate new buffer(s) for audio or video data.</summary>
    /// <param name="frame">frame in which to store the new buffers.</param>
    /// <param name="align">Required buffer size alignment. If equal to 0, alignment will be chosen automatically for the current CPU. It is highly recommended to pass 0 here unless you know what you are doing.</param>
    /// <returns>0 on success, a negative AVERROR on error.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_frame_get_buffer(AVFrame* @frame, int @align);
    
    /// <summary>Get the buffer reference a given data plane is stored in.</summary>
    /// <param name="frame">the frame to get the plane&apos;s buffer from</param>
    /// <param name="plane">index of the data plane of interest in frame-&gt;extended_data.</param>
    /// <returns>the buffer reference that contains the plane or NULL if the input frame is not valid.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVBufferRef* av_frame_get_plane_buffer(AVFrame* @frame, int @plane);
    
    /// <summary>Returns a pointer to the side data of a given type on success, NULL if there is no side data with such type in this frame.</summary>
    /// <returns>a pointer to the side data of a given type on success, NULL if there is no side data with such type in this frame.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVFrameSideData* av_frame_get_side_data(AVFrame* @frame, AVFrameSideDataType @type);
    
    /// <summary>Check if the frame data is writable.</summary>
    /// <returns>A positive value if the frame data is writable (which is true if and only if each of the underlying buffers has only one reference, namely the one stored in this frame). Return 0 otherwise.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_frame_is_writable(AVFrame* @frame);
    
    /// <summary>Ensure that the frame data is writable, avoiding data copy if possible.</summary>
    /// <returns>0 on success, a negative AVERROR on error.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_frame_make_writable(AVFrame* @frame);
    
    /// <summary>Move everything contained in src to dst and reset src.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_frame_move_ref(AVFrame* @dst, AVFrame* @src);
    
    /// <summary>Add a new side data to a frame.</summary>
    /// <param name="frame">a frame to which the side data should be added</param>
    /// <param name="type">type of the added side data</param>
    /// <param name="size">size of the side data</param>
    /// <returns>newly added side data on success, NULL on error</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVFrameSideData* av_frame_new_side_data(AVFrame* @frame, AVFrameSideDataType @type, ulong @size);
    
    /// <summary>Add a new side data to a frame from an existing AVBufferRef</summary>
    /// <param name="frame">a frame to which the side data should be added</param>
    /// <param name="type">the type of the added side data</param>
    /// <param name="buf">an AVBufferRef to add as side data. The ownership of the reference is transferred to the frame.</param>
    /// <returns>newly added side data on success, NULL on error. On failure the frame is unchanged and the AVBufferRef remains owned by the caller.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVFrameSideData* av_frame_new_side_data_from_buf(AVFrame* @frame, AVFrameSideDataType @type, AVBufferRef* @buf);
    
    /// <summary>Set up a new reference to the data described by the source frame.</summary>
    /// <returns>0 on success, a negative AVERROR on error</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_frame_ref(AVFrame* @dst, AVFrame* @src);
    
    /// <summary>Remove and free all side data instances of the given type.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_frame_remove_side_data(AVFrame* @frame, AVFrameSideDataType @type);
    
    /// <summary>Returns a string identifying the side data type</summary>
    /// <returns>a string identifying the side data type</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public static extern string av_frame_side_data_name(AVFrameSideDataType @type);
    
    /// <summary>Unreference all the buffers referenced by frame and reset the frame fields.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_frame_unref(AVFrame* @frame);
    
    /// <summary>Free a memory block which has been allocated with a function of av_malloc() or av_realloc() family.</summary>
    /// <param name="ptr">Pointer to the memory block which should be freed.</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_free(void* @ptr);
    
    /// <summary>Free a memory block which has been allocated with a function of av_malloc() or av_realloc() family, and set the pointer pointing to it to `NULL`.</summary>
    /// <param name="ptr">Pointer to the pointer to the memory block which should be freed</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_freep(void* @ptr);
    
    /// <summary>Compute the greatest common divisor of two integer operands.</summary>
    /// <param name="a">Operand</param>
    /// <param name="b">Operand</param>
    /// <returns>GCD of a and b up to sign; if a &gt;= 0 and b &gt;= 0, return value is &gt;= 0; if a == 0 and b == 0, returns 0.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern long av_gcd(long @a, long @b);
    
    /// <summary>Return the best rational so that a and b are multiple of it. If the resulting denominator is larger than max_den, return def.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVRational av_gcd_q(AVRational @a, AVRational @b, int @max_den, AVRational @def);
    
    /// <summary>Return the planar&lt;-&gt;packed alternative form of the given sample format, or AV_SAMPLE_FMT_NONE on error. If the passed sample_fmt is already in the requested planar/packed format, the format returned is the same as the input.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVSampleFormat av_get_alt_sample_fmt(AVSampleFormat @sample_fmt, int @planar);

    /// <summary>Return the number of bits per pixel used by the pixel format described by pixdesc. Note that this is not the same as the number of bits per sample.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_get_bits_per_pixel(AVPixFmtDescriptor* @pixdesc);
    
    /// <summary>Return codec bits per sample.</summary>
    /// <param name="codec_id">the codec</param>
    /// <returns>Number of bits per sample or zero if unknown for the given codec.</returns>
    [DllImport("avcodec", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_get_bits_per_sample(AVCodecID @codec_id);
    
    /// <summary>Return number of bytes per sample.</summary>
    /// <param name="sample_fmt">the sample format</param>
    /// <returns>number of bytes per sample or zero if unknown for the given sample format</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_get_bytes_per_sample(AVSampleFormat @sample_fmt);
    
    /// <summary>Get the description of a given channel.</summary>
    /// <param name="channel">a channel layout with a single channel</param>
    /// <returns>channel description on success, NULL on error</returns>
    [Obsolete("use av_channel_description()")]
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public static extern string av_get_channel_description(ulong @channel);
    
    /// <summary>Return a channel layout id that matches name, or 0 if no match is found.</summary>
    [Obsolete("use av_channel_layout_from_string()")]
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern ulong av_get_channel_layout(    
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name);
    
    /// <summary>Get the index of a channel in channel_layout.</summary>
    /// <param name="channel_layout">channel layout bitset</param>
    /// <param name="channel">a channel layout describing exactly one channel which must be present in channel_layout.</param>
    /// <returns>index of channel in channel_layout on success, a negative AVERROR on error.</returns>
    [Obsolete("use av_channel_layout_index_from_channel()")]
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_get_channel_layout_channel_index(ulong @channel_layout, ulong @channel);
    
    /// <summary>Return the number of channels in the channel layout.</summary>
    [Obsolete("use AVChannelLayout.nb_channels")]
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_get_channel_layout_nb_channels(ulong @channel_layout);
    
    /// <summary>Return a description of a channel layout. If nb_channels is &lt;= 0, it is guessed from the channel_layout.</summary>
    /// <param name="buf">put here the string containing the channel layout</param>
    /// <param name="buf_size">size in bytes of the buffer</param>
    /// <param name="nb_channels">number of channels</param>
    /// <param name="channel_layout">channel layout bitset</param>
    [Obsolete("use av_channel_layout_describe()")]
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_get_channel_layout_string(byte* @buf, int @buf_size, int @nb_channels, ulong @channel_layout);
    
    /// <summary>Get the name of a given channel.</summary>
    /// <returns>channel name on success, NULL on error.</returns>
    [Obsolete("use av_channel_name()")]
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public static extern string av_get_channel_name(ulong @channel);
    
    /// <summary>Return the flags which specify extensions supported by the CPU. The returned value is affected by av_force_cpu_flags() if that was used before. So av_get_cpu_flags() can easily be used in an application to detect the enabled cpu flags.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_get_cpu_flags();
    
    /// <summary>Return default channel layout for a given number of channels.</summary>
    [Obsolete("use av_channel_layout_default()")]
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern long av_get_default_channel_layout(int @nb_channels);

    /// <summary>Return a channel layout and the number of channels based on the specified name.</summary>
    /// <param name="name">channel layout specification string</param>
    /// <param name="channel_layout">parsed channel layout (0 if unknown)</param>
    /// <param name="nb_channels">number of channels</param>
    /// <returns>0 on success, AVERROR(EINVAL) if the parsing fails.</returns>
    [Obsolete("use av_channel_layout_from_string()")]
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_get_extended_channel_layout(    
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name, ulong* @channel_layout, int* @nb_channels);

    /// <summary>Return a string describing the media_type enum, NULL if media_type is unknown.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public static extern string av_get_media_type_string(AVMediaType @media_type);

    /// <summary>Get the packed alternative form of the given sample format.</summary>
    /// <returns>the packed alternative form of the given sample format or AV_SAMPLE_FMT_NONE on error.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVSampleFormat av_get_packed_sample_fmt(AVSampleFormat @sample_fmt);
    
    /// <summary>Return the number of bits per pixel for the pixel format described by pixdesc, including any padding or unused bits.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_get_padded_bits_per_pixel(AVPixFmtDescriptor* @pixdesc);

    /// <summary>Return a single letter to describe the given picture type pict_type.</summary>
    /// <param name="pict_type">the picture type</param>
    /// <returns>a single character representing the picture type, &apos;?&apos; if pict_type is unknown</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern byte av_get_picture_type_char(AVPictureType @pict_type);
    
    /// <summary>Return the pixel format corresponding to name.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVPixelFormat av_get_pix_fmt(    
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name);
    
    /// <summary>Compute what kind of losses will occur when converting from one specific pixel format to another. When converting from one pixel format to another, information loss may occur. For example, when converting from RGB24 to GRAY, the color information will be lost. Similarly, other losses occur when converting from some formats to other formats. These losses can involve loss of chroma, but also loss of resolution, loss of color depth, loss due to the color space conversion, loss of the alpha bits or loss due to color quantization. av_get_fix_fmt_loss() informs you about the various types of losses which will occur when converting from one pixel format to another.</summary>
    /// <param name="dst_pix_fmt">destination pixel format</param>
    /// <param name="src_pix_fmt">source pixel format</param>
    /// <param name="has_alpha">Whether the source pixel format alpha channel is used.</param>
    /// <returns>Combination of flags informing you what kind of losses will occur (maximum loss for an invalid dst_pix_fmt).</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_get_pix_fmt_loss(AVPixelFormat @dst_pix_fmt, AVPixelFormat @src_pix_fmt, int @has_alpha);
    
    /// <summary>Return the short name for a pixel format, NULL in case pix_fmt is unknown.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public static extern string av_get_pix_fmt_name(AVPixelFormat @pix_fmt);
    
    /// <summary>Print in buf the string corresponding to the pixel format with number pix_fmt, or a header if pix_fmt is negative.</summary>
    /// <param name="buf">the buffer where to write the string</param>
    /// <param name="buf_size">the size of buf</param>
    /// <param name="pix_fmt">the number of the pixel format to print the corresponding info string, or a negative value to print the corresponding header.</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern byte* av_get_pix_fmt_string(byte* @buf, int @buf_size, AVPixelFormat @pix_fmt);
    
    /// <summary>Get the planar alternative form of the given sample format.</summary>
    /// <returns>the planar alternative form of the given sample format or AV_SAMPLE_FMT_NONE on error.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVSampleFormat av_get_planar_sample_fmt(AVSampleFormat @sample_fmt);

    /// <summary>Return a sample format corresponding to name, or AV_SAMPLE_FMT_NONE on error.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVSampleFormat av_get_sample_fmt(    
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name);
    
    /// <summary>Return the name of sample_fmt, or NULL if sample_fmt is not recognized.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public static extern string av_get_sample_fmt_name(AVSampleFormat @sample_fmt);
    
    /// <summary>Generate a string corresponding to the sample format with sample_fmt, or a header if sample_fmt is negative.</summary>
    /// <param name="buf">the buffer where to write the string</param>
    /// <param name="buf_size">the size of buf</param>
    /// <param name="sample_fmt">the number of the sample format to print the corresponding info string, or a negative value to print the corresponding header.</param>
    /// <returns>the pointer to the filled buffer or NULL if sample_fmt is unknown or in case of other errors</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern byte* av_get_sample_fmt_string(byte* @buf, int @buf_size, AVSampleFormat @sample_fmt);
    
    /// <summary>Get the value and name of a standard channel layout.</summary>
    /// <param name="index">index in an internal list, starting at 0</param>
    /// <param name="layout">channel layout mask</param>
    /// <param name="name">name of the layout</param>
    /// <returns>0  if the layout exists,  &lt; 0 if index is beyond the limits</returns>
    [Obsolete("use av_channel_layout_standard()")]
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_get_standard_channel_layout(uint @index, ulong* @layout, byte** @name);
    
    /// <summary>Return the fractional representation of the internal time base.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVRational av_get_time_base_q();
    
    /// <summary>Get the current time in microseconds.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern long av_gettime();
    
    /// <summary>Get the current time in microseconds since some unspecified starting point. On platforms that support it, the time comes from a monotonic clock This property makes this time source ideal for measuring relative time. The returned values may not be monotonic on platforms where a monotonic clock is not available.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern long av_gettime_relative();
    
    /// <summary>Indicates with a boolean result if the av_gettime_relative() time source is monotonic.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_gettime_relative_is_monotonic();

    /// <summary>Allocate an AVHWDeviceContext for a given hardware type.</summary>
    /// <param name="type">the type of the hardware device to allocate.</param>
    /// <returns>a reference to the newly created AVHWDeviceContext on success or NULL on failure.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVBufferRef* av_hwdevice_ctx_alloc(AVHWDeviceType @type);
    
    /// <summary>Open a device of the specified type and create an AVHWDeviceContext for it.</summary>
    /// <param name="device_ctx">On success, a reference to the newly-created device context will be written here. The reference is owned by the caller and must be released with av_buffer_unref() when no longer needed. On failure, NULL will be written to this pointer.</param>
    /// <param name="type">The type of the device to create.</param>
    /// <param name="device">A type-specific string identifying the device to open.</param>
    /// <param name="opts">A dictionary of additional (type-specific) options to use in opening the device. The dictionary remains owned by the caller.</param>
    /// <param name="flags">currently unused</param>
    /// <returns>0 on success, a negative AVERROR code on failure.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_hwdevice_ctx_create(AVBufferRef** @device_ctx, AVHWDeviceType @type,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @device, AVDictionary* @opts, int @flags);
    
    /// <summary>Create a new device of the specified type from an existing device.</summary>
    /// <param name="dst_ctx">On success, a reference to the newly-created AVHWDeviceContext.</param>
    /// <param name="type">The type of the new device to create.</param>
    /// <param name="src_ctx">A reference to an existing AVHWDeviceContext which will be used to create the new device.</param>
    /// <param name="flags">Currently unused; should be set to zero.</param>
    /// <returns>Zero on success, a negative AVERROR code on failure.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_hwdevice_ctx_create_derived(AVBufferRef** @dst_ctx, AVHWDeviceType @type, AVBufferRef* @src_ctx, int @flags);
    
    /// <summary>Create a new device of the specified type from an existing device.</summary>
    /// <param name="dst_ctx">On success, a reference to the newly-created AVHWDeviceContext.</param>
    /// <param name="type">The type of the new device to create.</param>
    /// <param name="src_ctx">A reference to an existing AVHWDeviceContext which will be used to create the new device.</param>
    /// <param name="options">Options for the new device to create, same format as in av_hwdevice_ctx_create.</param>
    /// <param name="flags">Currently unused; should be set to zero.</param>
    /// <returns>Zero on success, a negative AVERROR code on failure.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_hwdevice_ctx_create_derived_opts(AVBufferRef** @dst_ctx, AVHWDeviceType @type, AVBufferRef* @src_ctx, AVDictionary* @options, int @flags);
    
    /// <summary>Finalize the device context before use. This function must be called after the context is filled with all the required information and before it is used in any way.</summary>
    /// <param name="ref">a reference to the AVHWDeviceContext</param>
    /// <returns>0 on success, a negative AVERROR code on failure</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_hwdevice_ctx_init(AVBufferRef* @ref);
    
    /// <summary>Look up an AVHWDeviceType by name.</summary>
    /// <param name="name">String name of the device type (case-insensitive).</param>
    /// <returns>The type from enum AVHWDeviceType, or AV_HWDEVICE_TYPE_NONE if not found.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVHWDeviceType av_hwdevice_find_type_by_name(    
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name);
    
    /// <summary>Get the constraints on HW frames given a device and the HW-specific configuration to be used with that device. If no HW-specific configuration is provided, returns the maximum possible capabilities of the device.</summary>
    /// <param name="ref">a reference to the associated AVHWDeviceContext.</param>
    /// <param name="hwconfig">a filled HW-specific configuration structure, or NULL to return the maximum possible capabilities of the device.</param>
    /// <returns>AVHWFramesConstraints structure describing the constraints on the device, or NULL if not available.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVHWFramesConstraints* av_hwdevice_get_hwframe_constraints(AVBufferRef* @ref, void* @hwconfig);
    
    /// <summary>Get the string name of an AVHWDeviceType.</summary>
    /// <param name="type">Type from enum AVHWDeviceType.</param>
    /// <returns>Pointer to a static string containing the name, or NULL if the type is not valid.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public static extern string av_hwdevice_get_type_name(AVHWDeviceType @type);
    
    /// <summary>Allocate a HW-specific configuration structure for a given HW device. After use, the user must free all members as required by the specific hardware structure being used, then free the structure itself with av_free().</summary>
    /// <param name="device_ctx">a reference to the associated AVHWDeviceContext.</param>
    /// <returns>The newly created HW-specific configuration structure on success or NULL on failure.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void* av_hwdevice_hwconfig_alloc(AVBufferRef* @device_ctx);
    
    /// <summary>Iterate over supported device types.</summary>
    /// <param name="prev">AV_HWDEVICE_TYPE_NONE initially, then the previous type returned by this function in subsequent iterations.</param>
    /// <returns>The next usable device type from enum AVHWDeviceType, or AV_HWDEVICE_TYPE_NONE if there are no more.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVHWDeviceType av_hwdevice_iterate_types(AVHWDeviceType @prev);
    
    /// <summary>Free an AVHWFrameConstraints structure.</summary>
    /// <param name="constraints">The (filled or unfilled) AVHWFrameConstraints structure.</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_hwframe_constraints_free(AVHWFramesConstraints** @constraints);
    
    /// <summary>Allocate an AVHWFramesContext tied to a given device context.</summary>
    /// <param name="device_ctx">a reference to a AVHWDeviceContext. This function will make a new reference for internal use, the one passed to the function remains owned by the caller.</param>
    /// <returns>a reference to the newly created AVHWFramesContext on success or NULL on failure.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVBufferRef* av_hwframe_ctx_alloc(AVBufferRef* @device_ctx);
    
    /// <summary>Create and initialise an AVHWFramesContext as a mapping of another existing AVHWFramesContext on a different device.</summary>
    /// <param name="derived_frame_ctx">On success, a reference to the newly created AVHWFramesContext.</param>
    /// <param name="format">The AVPixelFormat for the derived context.</param>
    /// <param name="derived_device_ctx">A reference to the device to create the new AVHWFramesContext on.</param>
    /// <param name="source_frame_ctx">A reference to an existing AVHWFramesContext which will be mapped to the derived context.</param>
    /// <param name="flags">Some combination of AV_HWFRAME_MAP_* flags, defining the mapping parameters to apply to frames which are allocated in the derived device.</param>
    /// <returns>Zero on success, negative AVERROR code on failure.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_hwframe_ctx_create_derived(AVBufferRef** @derived_frame_ctx, AVPixelFormat @format, AVBufferRef* @derived_device_ctx, AVBufferRef* @source_frame_ctx, int @flags);
    
    /// <summary>Finalize the context before use. This function must be called after the context is filled with all the required information and before it is attached to any frames.</summary>
    /// <param name="ref">a reference to the AVHWFramesContext</param>
    /// <returns>0 on success, a negative AVERROR code on failure</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_hwframe_ctx_init(AVBufferRef* @ref);
    
    /// <summary>Allocate a new frame attached to the given AVHWFramesContext.</summary>
    /// <param name="hwframe_ctx">a reference to an AVHWFramesContext</param>
    /// <param name="frame">an empty (freshly allocated or unreffed) frame to be filled with newly allocated buffers.</param>
    /// <param name="flags">currently unused, should be set to zero</param>
    /// <returns>0 on success, a negative AVERROR code on failure</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_hwframe_get_buffer(AVBufferRef* @hwframe_ctx, AVFrame* @frame, int @flags);
    
    /// <summary>Map a hardware frame.</summary>
    /// <param name="dst">Destination frame, to contain the mapping.</param>
    /// <param name="src">Source frame, to be mapped.</param>
    /// <param name="flags">Some combination of AV_HWFRAME_MAP_* flags.</param>
    /// <returns>Zero on success, negative AVERROR code on failure.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_hwframe_map(AVFrame* @dst, AVFrame* @src, int @flags);
    
    /// <summary>Copy data to or from a hw surface. At least one of dst/src must have an AVHWFramesContext attached.</summary>
    /// <param name="dst">the destination frame. dst is not touched on failure.</param>
    /// <param name="src">the source frame.</param>
    /// <param name="flags">currently unused, should be set to zero</param>
    /// <returns>0 on success, a negative AVERROR error code on failure.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_hwframe_transfer_data(AVFrame* @dst, AVFrame* @src, int @flags);
    
    /// <summary>Get a list of possible source or target formats usable in av_hwframe_transfer_data().</summary>
    /// <param name="hwframe_ctx">the frame context to obtain the information for</param>
    /// <param name="dir">the direction of the transfer</param>
    /// <param name="formats">the pointer to the output format list will be written here. The list is terminated with AV_PIX_FMT_NONE and must be freed by the caller when no longer needed using av_free(). If this function returns successfully, the format list will have at least one item (not counting the terminator). On failure, the contents of this pointer are unspecified.</param>
    /// <param name="flags">currently unused, should be set to zero</param>
    /// <returns>0 on success, a negative AVERROR code on failure.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_hwframe_transfer_get_formats(AVBufferRef* @hwframe_ctx, AVHWFrameTransferDirection @dir, AVPixelFormat** @formats, int @flags);
    
    /// <summary>Allocate an image with size w and h and pixel format pix_fmt, and fill pointers and linesizes accordingly. The allocated image buffer has to be freed by using av_freep(&amp;pointers[0]).</summary>
    /// <param name="pointers">array to be filled with the pointer for each image plane</param>
    /// <param name="linesizes">the array filled with the linesize for each plane</param>
    /// <param name="w">width of the image in pixels</param>
    /// <param name="h">height of the image in pixels</param>
    /// <param name="pix_fmt">the AVPixelFormat of the image</param>
    /// <param name="align">the value to use for buffer size alignment</param>
    /// <returns>the size in bytes required for the image buffer, a negative error code in case of failure</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_image_alloc(ref byte_ptr4 @pointers, ref int4 @linesizes, int @w, int @h, AVPixelFormat @pix_fmt, int @align);
    
    /// <summary>Check if the given sample aspect ratio of an image is valid.</summary>
    /// <param name="w">width of the image</param>
    /// <param name="h">height of the image</param>
    /// <param name="sar">sample aspect ratio of the image</param>
    /// <returns>0 if valid, a negative AVERROR code otherwise</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_image_check_sar(uint @w, uint @h, AVRational @sar);
    
    /// <summary>Check if the given dimension of an image is valid, meaning that all bytes of the image can be addressed with a signed int.</summary>
    /// <param name="w">the width of the picture</param>
    /// <param name="h">the height of the picture</param>
    /// <param name="log_offset">the offset to sum to the log level for logging with log_ctx</param>
    /// <param name="log_ctx">the parent logging context, it may be NULL</param>
    /// <returns>&gt;= 0 if valid, a negative error code otherwise</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_image_check_size(uint @w, uint @h, int @log_offset, void* @log_ctx);
    
    /// <summary>Check if the given dimension of an image is valid, meaning that all bytes of a plane of an image with the specified pix_fmt can be addressed with a signed int.</summary>
    /// <param name="w">the width of the picture</param>
    /// <param name="h">the height of the picture</param>
    /// <param name="max_pixels">the maximum number of pixels the user wants to accept</param>
    /// <param name="pix_fmt">the pixel format, can be AV_PIX_FMT_NONE if unknown.</param>
    /// <param name="log_offset">the offset to sum to the log level for logging with log_ctx</param>
    /// <param name="log_ctx">the parent logging context, it may be NULL</param>
    /// <returns>&gt;= 0 if valid, a negative error code otherwise</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_image_check_size2(uint @w, uint @h, long @max_pixels, AVPixelFormat @pix_fmt, int @log_offset, void* @log_ctx);
    
    /// <summary>Copy image in src_data to dst_data.</summary>
    /// <param name="dst_data">destination image data buffer to copy to</param>
    /// <param name="dst_linesizes">linesizes for the image in dst_data</param>
    /// <param name="src_data">source image data buffer to copy from</param>
    /// <param name="src_linesizes">linesizes for the image in src_data</param>
    /// <param name="pix_fmt">the AVPixelFormat of the image</param>
    /// <param name="width">width of the image in pixels</param>
    /// <param name="height">height of the image in pixels</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_image_copy(ref byte_ptr4 @dst_data, ref int4 @dst_linesizes, in byte_ptr4 @src_data, in int4 @src_linesizes, AVPixelFormat @pix_fmt, int @width, int @height);
    
    /// <summary>Copy image plane from src to dst. That is, copy &quot;height&quot; number of lines of &quot;bytewidth&quot; bytes each. The first byte of each successive line is separated by *_linesize bytes.</summary>
    /// <param name="dst">destination plane to copy to</param>
    /// <param name="dst_linesize">linesize for the image plane in dst</param>
    /// <param name="src">source plane to copy from</param>
    /// <param name="src_linesize">linesize for the image plane in src</param>
    /// <param name="height">height (number of lines) of the plane</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_image_copy_plane(byte* @dst, int @dst_linesize, byte* @src, int @src_linesize, int @bytewidth, int @height);
    
    /// <summary>Copy image data located in uncacheable (e.g. GPU mapped) memory. Where available, this function will use special functionality for reading from such memory, which may result in greatly improved performance compared to plain av_image_copy_plane().</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_image_copy_plane_uc_from(byte* @dst, long @dst_linesize, byte* @src, long @src_linesize, long @bytewidth, int @height);
    
    /// <summary>Copy image data from an image into a buffer.</summary>
    /// <param name="dst">a buffer into which picture data will be copied</param>
    /// <param name="dst_size">the size in bytes of dst</param>
    /// <param name="src_data">pointers containing the source image data</param>
    /// <param name="src_linesize">linesizes for the image in src_data</param>
    /// <param name="pix_fmt">the pixel format of the source image</param>
    /// <param name="width">the width of the source image in pixels</param>
    /// <param name="height">the height of the source image in pixels</param>
    /// <param name="align">the assumed linesize alignment for dst</param>
    /// <returns>the number of bytes written to dst, or a negative value (error code) on error</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_image_copy_to_buffer(byte* @dst, int @dst_size, in byte_ptr4 @src_data, in int4 @src_linesize, AVPixelFormat @pix_fmt, int @width, int @height, int @align);
    
    /// <summary>Copy image data located in uncacheable (e.g. GPU mapped) memory. Where available, this function will use special functionality for reading from such memory, which may result in greatly improved performance compared to plain av_image_copy().</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_image_copy_uc_from(ref byte_ptr4 @dst_data, in long4 @dst_linesizes, in byte_ptr4 @src_data, in long4 @src_linesizes, AVPixelFormat @pix_fmt, int @width, int @height);
    
    /// <summary>Setup the data pointers and linesizes based on the specified image parameters and the provided array.</summary>
    /// <param name="dst_data">data pointers to be filled in</param>
    /// <param name="dst_linesize">linesizes for the image in dst_data to be filled in</param>
    /// <param name="src">buffer which will contain or contains the actual image data, can be NULL</param>
    /// <param name="pix_fmt">the pixel format of the image</param>
    /// <param name="width">the width of the image in pixels</param>
    /// <param name="height">the height of the image in pixels</param>
    /// <param name="align">the value used in src for linesize alignment</param>
    /// <returns>the size in bytes required for src, a negative error code in case of failure</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_image_fill_arrays(ref byte_ptr4 @dst_data, ref int4 @dst_linesize, byte* @src, AVPixelFormat @pix_fmt, int @width, int @height, int @align);
    
    /// <summary>Overwrite the image data with black. This is suitable for filling a sub-rectangle of an image, meaning the padding between the right most pixel and the left most pixel on the next line will not be overwritten. For some formats, the image size might be rounded up due to inherent alignment.</summary>
    /// <param name="dst_data">data pointers to destination image</param>
    /// <param name="dst_linesize">linesizes for the destination image</param>
    /// <param name="pix_fmt">the pixel format of the image</param>
    /// <param name="range">the color range of the image (important for colorspaces such as YUV)</param>
    /// <param name="width">the width of the image in pixels</param>
    /// <param name="height">the height of the image in pixels</param>
    /// <returns>0 if the image data was cleared, a negative AVERROR code otherwise</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_image_fill_black(ref byte_ptr4 @dst_data, in long4 @dst_linesize, AVPixelFormat @pix_fmt, AVColorRange @range, int @width, int @height);
    
    /// <summary>Fill plane linesizes for an image with pixel format pix_fmt and width width.</summary>
    /// <param name="linesizes">array to be filled with the linesize for each plane</param>
    /// <param name="pix_fmt">the AVPixelFormat of the image</param>
    /// <param name="width">width of the image in pixels</param>
    /// <returns>&gt;= 0 in case of success, a negative error code otherwise</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_image_fill_linesizes(ref int4 @linesizes, AVPixelFormat @pix_fmt, int @width);
    
    /// <summary>Compute the max pixel step for each plane of an image with a format described by pixdesc.</summary>
    /// <param name="max_pixsteps">an array which is filled with the max pixel step for each plane. Since a plane may contain different pixel components, the computed max_pixsteps[plane] is relative to the component in the plane with the max pixel step.</param>
    /// <param name="max_pixstep_comps">an array which is filled with the component for each plane which has the max pixel step. May be NULL.</param>
    /// <param name="pixdesc">the AVPixFmtDescriptor for the image, describing its format</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_image_fill_max_pixsteps(ref int4 @max_pixsteps, ref int4 @max_pixstep_comps, AVPixFmtDescriptor* @pixdesc);
    
    /// <summary>Fill plane sizes for an image with pixel format pix_fmt and height height.</summary>
    /// <param name="size">the array to be filled with the size of each image plane</param>
    /// <param name="pix_fmt">the AVPixelFormat of the image</param>
    /// <param name="height">height of the image in pixels</param>
    /// <param name="linesizes">the array containing the linesize for each plane, should be filled by av_image_fill_linesizes()</param>
    /// <returns>&gt;= 0 in case of success, a negative error code otherwise</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_image_fill_plane_sizes(ref ulong4 @size, AVPixelFormat @pix_fmt, int @height, in long4 @linesizes);
    
    /// <summary>Fill plane data pointers for an image with pixel format pix_fmt and height height.</summary>
    /// <param name="data">pointers array to be filled with the pointer for each image plane</param>
    /// <param name="pix_fmt">the AVPixelFormat of the image</param>
    /// <param name="height">height of the image in pixels</param>
    /// <param name="ptr">the pointer to a buffer which will contain the image</param>
    /// <param name="linesizes">the array containing the linesize for each plane, should be filled by av_image_fill_linesizes()</param>
    /// <returns>the size in bytes required for the image buffer, a negative error code in case of failure</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_image_fill_pointers(ref byte_ptr4 @data, AVPixelFormat @pix_fmt, int @height, byte* @ptr, in int4 @linesizes);
    
    /// <summary>Return the size in bytes of the amount of data required to store an image with the given parameters.</summary>
    /// <param name="pix_fmt">the pixel format of the image</param>
    /// <param name="width">the width of the image in pixels</param>
    /// <param name="height">the height of the image in pixels</param>
    /// <param name="align">the assumed linesize alignment</param>
    /// <returns>the buffer size in bytes, a negative error code in case of failure</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_image_get_buffer_size(AVPixelFormat @pix_fmt, int @width, int @height, int @align);
    
    /// <summary>Compute the size of an image line with format pix_fmt and width width for the plane plane.</summary>
    /// <returns>the computed size in bytes</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_image_get_linesize(AVPixelFormat @pix_fmt, int @width, int @plane);

    /// <summary>Compute the length of an integer list.</summary>
    /// <param name="elsize">size in bytes of each list element (only 1, 2, 4 or 8)</param>
    /// <param name="list">pointer to the list</param>
    /// <param name="term">list terminator (usually 0 or -1)</param>
    /// <returns>length of the list, in elements, not counting the terminator</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint av_int_list_length_for_size(uint @elsize, void* @list, ulong @term);
    
    /// <summary>Send the specified message to the log if the level is less than or equal to the current av_log_level. By default, all logging messages are sent to stderr. This behavior can be altered by setting a different logging callback function.</summary>
    /// <param name="avcl">A pointer to an arbitrary struct of which the first field is a pointer to an AVClass struct or NULL if general log.</param>
    /// <param name="level">The importance level of the message expressed using a &quot;Logging Constant&quot;.</param>
    /// <param name="fmt">The format string (printf-compatible) that specifies how subsequent arguments are converted to output.</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_log(void* @avcl, int @level,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @fmt);
    
    /// <summary>Default logging callback</summary>
    /// <param name="avcl">A pointer to an arbitrary struct of which the first field is a pointer to an AVClass struct.</param>
    /// <param name="level">The importance level of the message expressed using a &quot;Logging Constant&quot;.</param>
    /// <param name="fmt">The format string (printf-compatible) that specifies how subsequent arguments are converted to output.</param>
    /// <param name="vl">The arguments referenced by the format string.</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_log_default_callback(void* @avcl, int @level,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @fmt, byte* @vl);
    
    /// <summary>Format a line of log the same way as the default callback.</summary>
    /// <param name="line">buffer to receive the formatted line</param>
    /// <param name="line_size">size of the buffer</param>
    /// <param name="print_prefix">used to store whether the prefix must be printed; must point to a persistent integer initially set to 1</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_log_format_line(void* @ptr, int @level,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @fmt, byte* @vl, byte* @line, int @line_size, int* @print_prefix);
    
    /// <summary>Format a line of log the same way as the default callback.</summary>
    /// <param name="line">buffer to receive the formatted line; may be NULL if line_size is 0</param>
    /// <param name="line_size">size of the buffer; at most line_size-1 characters will be written to the buffer, plus one null terminator</param>
    /// <param name="print_prefix">used to store whether the prefix must be printed; must point to a persistent integer initially set to 1</param>
    /// <returns>Returns a negative value if an error occurred, otherwise returns the number of characters that would have been written for a sufficiently large buffer, not including the terminating null character. If the return value is not less than line_size, it means that the log message was truncated to fit the buffer.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_log_format_line2(void* @ptr, int @level,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @fmt, byte* @vl, byte* @line, int @line_size, int* @print_prefix);
    
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_log_get_flags();
    
    /// <summary>Get the current log level</summary>
    /// <returns>Current log level</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_log_get_level();
    
    /// <summary>Send the specified message to the log once with the initial_level and then with the subsequent_level. By default, all logging messages are sent to stderr. This behavior can be altered by setting a different logging callback function.</summary>
    /// <param name="avcl">A pointer to an arbitrary struct of which the first field is a pointer to an AVClass struct or NULL if general log.</param>
    /// <param name="initial_level">importance level of the message expressed using a &quot;Logging Constant&quot; for the first occurance.</param>
    /// <param name="subsequent_level">importance level of the message expressed using a &quot;Logging Constant&quot; after the first occurance.</param>
    /// <param name="state">a variable to keep trak of if a message has already been printed this must be initialized to 0 before the first use. The same state must not be accessed by 2 Threads simultaneously.</param>
    /// <param name="fmt">The format string (printf-compatible) that specifies how subsequent arguments are converted to output.</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_log_once(void* @avcl, int @initial_level, int @subsequent_level, int* @state,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @fmt);
    
    /// <summary>Set the logging callback</summary>
    /// <param name="callback">A logging function with a compatible signature.</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_log_set_callback(av_log_set_callback_callback_func @callback);
    
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_log_set_flags(int @arg);
    
    /// <summary>Set the log level</summary>
    /// <param name="level">Logging level</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_log_set_level(int @level);
    
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_log2(uint @v);
    
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_log2_16bit(uint @v);
    
    /// <summary>Allocate a memory block with alignment suitable for all memory accesses (including vectors if available on the CPU).</summary>
    /// <param name="size">Size in bytes for the memory block to be allocated</param>
    /// <returns>Pointer to the allocated block, or `NULL` if the block cannot be allocated</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void* av_malloc(ulong @size);
    
    /// <summary>Allocate a memory block for an array with av_malloc().</summary>
    /// <param name="nmemb">Number of element</param>
    /// <param name="size">Size of a single element</param>
    /// <returns>Pointer to the allocated block, or `NULL` if the block cannot be allocated</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void* av_malloc_array(ulong @nmemb, ulong @size);
    
    /// <summary>Allocate a memory block with alignment suitable for all memory accesses (including vectors if available on the CPU) and zero all the bytes of the block.</summary>
    /// <param name="size">Size in bytes for the memory block to be allocated</param>
    /// <returns>Pointer to the allocated block, or `NULL` if it cannot be allocated</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void* av_mallocz(ulong @size);
    
    /// <summary>Allocate an AVMasteringDisplayMetadata structure and set its fields to default values. The resulting struct can be freed using av_freep().</summary>
    /// <returns>An AVMasteringDisplayMetadata filled with default values or NULL on failure.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVMasteringDisplayMetadata* av_mastering_display_metadata_alloc();
    
    /// <summary>Allocate a complete AVMasteringDisplayMetadata and add it to the frame.</summary>
    /// <param name="frame">The frame which side data is added to.</param>
    /// <returns>The AVMasteringDisplayMetadata structure to be filled by caller.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVMasteringDisplayMetadata* av_mastering_display_metadata_create_side_data(AVFrame* @frame);
    
    /// <summary>Return a positive value if the given filename has one of the given extensions, 0 otherwise.</summary>
    /// <param name="filename">file name to check against the given extensions</param>
    /// <param name="extensions">a comma-separated list of filename extensions</param>
    [DllImport("avformat", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_match_ext(    
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @filename,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @extensions);
    
    /// <summary>Set the maximum size that may be allocated in one block.</summary>
    /// <param name="max">Value to be set as the new maximum size</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_max_alloc(ulong @max);
    
    /// <summary>Overlapping memcpy() implementation.</summary>
    /// <param name="dst">Destination buffer</param>
    /// <param name="back">Number of bytes back to start copying (i.e. the initial size of the overlapping window); must be &gt; 0</param>
    /// <param name="cnt">Number of bytes to copy; must be &gt;= 0</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_memcpy_backptr(byte* @dst, int @back, int @cnt);
    
    /// <summary>Duplicate a buffer with av_malloc().</summary>
    /// <param name="p">Buffer to be duplicated</param>
    /// <param name="size">Size in bytes of the buffer copied</param>
    /// <returns>Pointer to a newly allocated buffer containing a copy of `p` or `NULL` if the buffer cannot be allocated</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void* av_memdup(void* @p, ulong @size);
    
    /// <summary>Multiply two rationals.</summary>
    /// <param name="b">First rational</param>
    /// <param name="c">Second rational</param>
    /// <returns>b*c</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVRational av_mul_q(AVRational @b, AVRational @c);

    /// <summary>Find which of the two rationals is closer to another rational.</summary>
    /// <param name="q">Rational to be compared against</param>
    /// <param name="q1">Rational to be tested</param>
    /// <param name="q2">Rational to be tested</param>
    /// <returns>One of the following values: - 1 if `q1` is nearer to `q` than `q2` - -1 if `q2` is nearer to `q` than `q1` - 0 if they have the same distance</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_nearer_q(AVRational @q, AVRational @q1, AVRational @q2);
    
    /// <summary>Iterate over potential AVOptions-enabled children of parent.</summary>
    /// <param name="iter">a pointer where iteration state is stored.</param>
    /// <returns>AVClass corresponding to next potential child or NULL</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVClass* av_opt_child_class_iterate(AVClass* @parent, void** @iter);
    
    /// <summary>Iterate over AVOptions-enabled children of obj.</summary>
    /// <param name="prev">result of a previous call to this function or NULL</param>
    /// <returns>next AVOptions-enabled child or NULL</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void* av_opt_child_next(void* @obj, void* @prev);
    
    /// <summary>Copy options from src object into dest object.</summary>
    /// <param name="dest">Object to copy from</param>
    /// <param name="src">Object to copy into</param>
    /// <returns>0 on success, negative on error</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_copy(void* @dest, void* @src);
    
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_eval_double(void* @obj, AVOption* @o,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @val, double* @double_out);
    
    /// <summary>@{ This group of functions can be used to evaluate option strings and get numbers out of them. They do the same thing as av_opt_set(), except the result is written into the caller-supplied pointer.</summary>
    /// <param name="obj">a struct whose first element is a pointer to AVClass.</param>
    /// <param name="o">an option for which the string is to be evaluated.</param>
    /// <param name="val">string to be evaluated.</param>
    /// <returns>0 on success, a negative number on failure.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_eval_flags(void* @obj, AVOption* @o,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @val, int* @flags_out);
    
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_eval_float(void* @obj, AVOption* @o,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @val, float* @float_out);
    
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_eval_int(void* @obj, AVOption* @o,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @val, int* @int_out);
    
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_eval_int64(void* @obj, AVOption* @o,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @val, long* @int64_out);
    
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_eval_q(void* @obj, AVOption* @o,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @val, AVRational* @q_out);
    
    /// <summary>Look for an option in an object. Consider only options which have all the specified flags set.</summary>
    /// <param name="obj">A pointer to a struct whose first element is a pointer to an AVClass. Alternatively a double pointer to an AVClass, if AV_OPT_SEARCH_FAKE_OBJ search flag is set.</param>
    /// <param name="name">The name of the option to look for.</param>
    /// <param name="unit">When searching for named constants, name of the unit it belongs to.</param>
    /// <param name="opt_flags">Find only options with all the specified flags set (AV_OPT_FLAG).</param>
    /// <param name="search_flags">A combination of AV_OPT_SEARCH_*.</param>
    /// <returns>A pointer to the option found, or NULL if no option was found.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVOption* av_opt_find(void* @obj,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @unit, int @opt_flags, int @search_flags);
    
    /// <summary>Look for an option in an object. Consider only options which have all the specified flags set.</summary>
    /// <param name="obj">A pointer to a struct whose first element is a pointer to an AVClass. Alternatively a double pointer to an AVClass, if AV_OPT_SEARCH_FAKE_OBJ search flag is set.</param>
    /// <param name="name">The name of the option to look for.</param>
    /// <param name="unit">When searching for named constants, name of the unit it belongs to.</param>
    /// <param name="opt_flags">Find only options with all the specified flags set (AV_OPT_FLAG).</param>
    /// <param name="search_flags">A combination of AV_OPT_SEARCH_*.</param>
    /// <param name="target_obj">if non-NULL, an object to which the option belongs will be written here. It may be different from obj if AV_OPT_SEARCH_CHILDREN is present in search_flags. This parameter is ignored if search_flags contain AV_OPT_SEARCH_FAKE_OBJ.</param>
    /// <returns>A pointer to the option found, or NULL if no option was found.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVOption* av_opt_find2(void* @obj,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @unit, int @opt_flags, int @search_flags, void** @target_obj);
    
    /// <summary>Check whether a particular flag is set in a flags field.</summary>
    /// <param name="field_name">the name of the flag field option</param>
    /// <param name="flag_name">the name of the flag to check</param>
    /// <returns>non-zero if the flag is set, zero if the flag isn&apos;t set, isn&apos;t of the right type, or the flags field doesn&apos;t exist.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_flag_is_set(void* @obj,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @field_name,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @flag_name);
    
    /// <summary>Free all allocated objects in obj.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_opt_free(void* @obj);
    
    /// <summary>Free an AVOptionRanges struct and set it to NULL.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_opt_freep_ranges(AVOptionRanges** @ranges);
    
    /// <summary>@{ Those functions get a value of the option with the given name from an object.</summary>
    /// <param name="obj">a struct whose first element is a pointer to an AVClass.</param>
    /// <param name="name">name of the option to get.</param>
    /// <param name="search_flags">flags passed to av_opt_find2. I.e. if AV_OPT_SEARCH_CHILDREN is passed here, then the option may be found in a child of obj.</param>
    /// <param name="out_val">value of the option will be written here</param>
    /// <returns>&gt;=0 on success, a negative error code otherwise</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_get(void* @obj,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name, int @search_flags, byte** @out_val);
    
    [Obsolete()]
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_get_channel_layout(void* @obj,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name, int @search_flags, long* @ch_layout);
    
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_get_chlayout(void* @obj,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name, int @search_flags, AVChannelLayout* @layout);
    
    /// <param name="out_val">The returned dictionary is a copy of the actual value and must be freed with av_dict_free() by the caller</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_get_dict_val(void* @obj,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name, int @search_flags, AVDictionary** @out_val);
    
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_get_double(void* @obj,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name, int @search_flags, double* @out_val);
    
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_get_image_size(void* @obj,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name, int @search_flags, int* @w_out, int* @h_out);
    
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_get_int(void* @obj,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name, int @search_flags, long* @out_val);
    
    /// <summary>Extract a key-value pair from the beginning of a string.</summary>
    /// <param name="ropts">pointer to the options string, will be updated to point to the rest of the string (one of the pairs_sep or the final NUL)</param>
    /// <param name="key_val_sep">a 0-terminated list of characters used to separate key from value, for example &apos;=&apos;</param>
    /// <param name="pairs_sep">a 0-terminated list of characters used to separate two pairs from each other, for example &apos;:&apos; or &apos;,&apos;</param>
    /// <param name="flags">flags; see the AV_OPT_FLAG_* values below</param>
    /// <param name="rkey">parsed key; must be freed using av_free()</param>
    /// <param name="rval">parsed value; must be freed using av_free()</param>
    /// <returns>&gt;=0 for success, or a negative value corresponding to an AVERROR code in case of error; in particular: AVERROR(EINVAL) if no key is present</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_get_key_value(byte** @ropts,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @key_val_sep,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @pairs_sep, uint @flags, byte** @rkey, byte** @rval);
    
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_get_pixel_fmt(void* @obj,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name, int @search_flags, AVPixelFormat* @out_fmt);
    
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_get_q(void* @obj,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name, int @search_flags, AVRational* @out_val);
    
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_get_sample_fmt(void* @obj,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name, int @search_flags, AVSampleFormat* @out_fmt);
    
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_get_video_rate(void* @obj,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name, int @search_flags, AVRational* @out_val);
    
    /// <summary>Check if given option is set to its default value.</summary>
    /// <param name="obj">AVClass object to check option on</param>
    /// <param name="o">option to be checked</param>
    /// <returns>&gt;0 when option is set to its default, 0 when option is not set its default,  &lt; 0 on error</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_is_set_to_default(void* @obj, AVOption* @o);
    
    /// <summary>Check if given option is set to its default value.</summary>
    /// <param name="obj">AVClass object to check option on</param>
    /// <param name="name">option name</param>
    /// <param name="search_flags">combination of AV_OPT_SEARCH_*</param>
    /// <returns>&gt;0 when option is set to its default, 0 when option is not set its default,  &lt; 0 on error</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_is_set_to_default_by_name(void* @obj,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name, int @search_flags);
    
    /// <summary>Iterate over all AVOptions belonging to obj.</summary>
    /// <param name="obj">an AVOptions-enabled struct or a double pointer to an AVClass describing it.</param>
    /// <param name="prev">result of the previous call to av_opt_next() on this object or NULL</param>
    /// <returns>next AVOption or NULL</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVOption* av_opt_next(void* @obj, AVOption* @prev);
    
    /// <summary>@}</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void* av_opt_ptr(AVClass* @avclass, void* @obj,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name);
    
    /// <summary>Get a list of allowed ranges for the given option.</summary>
    /// <param name="flags">is a bitmask of flags, undefined flags should not be set and should be ignored AV_OPT_SEARCH_FAKE_OBJ indicates that the obj is a double pointer to a AVClass instead of a full instance AV_OPT_MULTI_COMPONENT_RANGE indicates that function may return more than one component,</param>
    /// <returns>number of compontents returned on success, a negative errro code otherwise</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_query_ranges(AVOptionRanges** @p0, void* @obj,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @key, int @flags);
    
    /// <summary>Get a default list of allowed ranges for the given option.</summary>
    /// <param name="flags">is a bitmask of flags, undefined flags should not be set and should be ignored AV_OPT_SEARCH_FAKE_OBJ indicates that the obj is a double pointer to a AVClass instead of a full instance AV_OPT_MULTI_COMPONENT_RANGE indicates that function may return more than one component,</param>
    /// <returns>number of compontents returned on success, a negative errro code otherwise</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_query_ranges_default(AVOptionRanges** @p0, void* @obj,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @key, int @flags);
    
    /// <summary>Serialize object&apos;s options.</summary>
    /// <param name="obj">AVClass object to serialize</param>
    /// <param name="opt_flags">serialize options with all the specified flags set (AV_OPT_FLAG)</param>
    /// <param name="flags">combination of AV_OPT_SERIALIZE_* flags</param>
    /// <param name="buffer">Pointer to buffer that will be allocated with string containg serialized options. Buffer must be freed by the caller when is no longer needed.</param>
    /// <param name="key_val_sep">character used to separate key from value</param>
    /// <param name="pairs_sep">character used to separate two pairs from each other</param>
    /// <returns>&gt;= 0 on success, negative on error</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_serialize(void* @obj, int @opt_flags, int @flags, byte** @buffer, byte @key_val_sep, byte @pairs_sep);
    
    /// <summary>@{ Those functions set the field of obj with the given name to value.</summary>
    /// <param name="obj">A struct whose first element is a pointer to an AVClass.</param>
    /// <param name="name">the name of the field to set</param>
    /// <param name="val">The value to set. In case of av_opt_set() if the field is not of a string type, then the given string is parsed. SI postfixes and some named scalars are supported. If the field is of a numeric type, it has to be a numeric or named scalar. Behavior with more than one scalar and +- infix operators is undefined. If the field is of a flags type, it has to be a sequence of numeric scalars or named flags separated by &apos;+&apos; or &apos;-&apos;. Prefixing a flag with &apos;+&apos; causes it to be set without affecting the other flags; similarly, &apos;-&apos; unsets a flag. If the field is of a dictionary type, it has to be a &apos;:&apos; separated list of key=value parameters. Values containing &apos;:&apos; special characters must be escaped.</param>
    /// <param name="search_flags">flags passed to av_opt_find2. I.e. if AV_OPT_SEARCH_CHILDREN is passed here, then the option may be set on a child of obj.</param>
    /// <returns>0 if the value has been set, or an AVERROR code in case of error: AVERROR_OPTION_NOT_FOUND if no matching option exists AVERROR(ERANGE) if the value is out of range AVERROR(EINVAL) if the value is not valid</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_set(void* @obj,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @val, int @search_flags);
    
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_set_bin(void* @obj,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name, byte* @val, int @size, int @search_flags);
    
    [Obsolete()]
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_set_channel_layout(void* @obj,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name, long @ch_layout, int @search_flags);
    
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_set_chlayout(void* @obj,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name, AVChannelLayout* @layout, int @search_flags);
    
    /// <summary>Set the values of all AVOption fields to their default values.</summary>
    /// <param name="s">an AVOption-enabled struct (its first member must be a pointer to AVClass)</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_opt_set_defaults(void* @s);
    
    /// <summary>Set the values of all AVOption fields to their default values. Only these AVOption fields for which (opt-&gt;flags &amp; mask) == flags will have their default applied to s.</summary>
    /// <param name="s">an AVOption-enabled struct (its first member must be a pointer to AVClass)</param>
    /// <param name="mask">combination of AV_OPT_FLAG_*</param>
    /// <param name="flags">combination of AV_OPT_FLAG_*</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_opt_set_defaults2(void* @s, int @mask, int @flags);
    
    /// <summary>Set all the options from a given dictionary on an object.</summary>
    /// <param name="obj">a struct whose first element is a pointer to AVClass</param>
    /// <param name="options">options to process. This dictionary will be freed and replaced by a new one containing all options not found in obj. Of course this new dictionary needs to be freed by caller with av_dict_free().</param>
    /// <returns>0 on success, a negative AVERROR if some option was found in obj, but could not be set.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_set_dict(void* @obj, AVDictionary** @options);
    
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_set_dict_val(void* @obj,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name, AVDictionary* @val, int @search_flags);
    
    /// <summary>Set all the options from a given dictionary on an object.</summary>
    /// <param name="obj">a struct whose first element is a pointer to AVClass</param>
    /// <param name="options">options to process. This dictionary will be freed and replaced by a new one containing all options not found in obj. Of course this new dictionary needs to be freed by caller with av_dict_free().</param>
    /// <param name="search_flags">A combination of AV_OPT_SEARCH_*.</param>
    /// <returns>0 on success, a negative AVERROR if some option was found in obj, but could not be set.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_set_dict2(void* @obj, AVDictionary** @options, int @search_flags);
    
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_set_double(void* @obj,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name, double @val, int @search_flags);
    
    /// <summary>Parse the key-value pairs list in opts. For each key=value pair found, set the value of the corresponding option in ctx.</summary>
    /// <param name="ctx">the AVClass object to set options on</param>
    /// <param name="opts">the options string, key-value pairs separated by a delimiter</param>
    /// <param name="shorthand">a NULL-terminated array of options names for shorthand notation: if the first field in opts has no key part, the key is taken from the first element of shorthand; then again for the second, etc., until either opts is finished, shorthand is finished or a named option is found; after that, all options must be named</param>
    /// <param name="key_val_sep">a 0-terminated list of characters used to separate key from value, for example &apos;=&apos;</param>
    /// <param name="pairs_sep">a 0-terminated list of characters used to separate two pairs from each other, for example &apos;:&apos; or &apos;,&apos;</param>
    /// <returns>the number of successfully set key=value pairs, or a negative value corresponding to an AVERROR code in case of error: AVERROR(EINVAL) if opts cannot be parsed, the error code issued by av_set_string3() if a key/value pair cannot be set</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_set_from_string(void* @ctx,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @opts, byte** @shorthand,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @key_val_sep,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @pairs_sep);
    
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_set_image_size(void* @obj,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name, int @w, int @h, int @search_flags);
    
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_set_int(void* @obj,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name, long @val, int @search_flags);
    
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_set_pixel_fmt(void* @obj,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name, AVPixelFormat @fmt, int @search_flags);
    
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_set_q(void* @obj,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name, AVRational @val, int @search_flags);
    
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_set_sample_fmt(void* @obj,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name, AVSampleFormat @fmt, int @search_flags);
    
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_set_video_rate(void* @obj,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @name, AVRational @val, int @search_flags);
    
    /// <summary>Show the obj options.</summary>
    /// <param name="av_log_obj">log context to use for showing the options</param>
    /// <param name="req_flags">requested flags for the options to show. Show only the options for which it is opt-&gt;flags &amp; req_flags.</param>
    /// <param name="rej_flags">rejected flags for the options to show. Show only the options for which it is !(opt-&gt;flags &amp; req_flags).</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_opt_show2(void* @obj, void* @av_log_obj, int @req_flags, int @rej_flags);

    /// <summary>Parse CPU caps from a string and update the given AV_CPU_* flags based on that.</summary>
    /// <returns>negative on error.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_parse_cpu_caps(uint* @flags,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @s);

    /// <summary>Returns number of planes in pix_fmt, a negative AVERROR if pix_fmt is not a valid pixel format.</summary>
    /// <returns>number of planes in pix_fmt, a negative AVERROR if pix_fmt is not a valid pixel format.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_pix_fmt_count_planes(AVPixelFormat @pix_fmt);
    
    /// <summary>Returns a pixel format descriptor for provided pixel format or NULL if this pixel format is unknown.</summary>
    /// <returns>a pixel format descriptor for provided pixel format or NULL if this pixel format is unknown.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVPixFmtDescriptor* av_pix_fmt_desc_get(AVPixelFormat @pix_fmt);
    
    /// <summary>Returns an AVPixelFormat id described by desc, or AV_PIX_FMT_NONE if desc is not a valid pointer to a pixel format descriptor.</summary>
    /// <returns>an AVPixelFormat id described by desc, or AV_PIX_FMT_NONE if desc is not a valid pointer to a pixel format descriptor.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVPixelFormat av_pix_fmt_desc_get_id(AVPixFmtDescriptor* @desc);
    
    /// <summary>Iterate over all pixel format descriptors known to libavutil.</summary>
    /// <param name="prev">previous descriptor. NULL to get the first descriptor.</param>
    /// <returns>next descriptor or NULL after the last descriptor</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVPixFmtDescriptor* av_pix_fmt_desc_next(AVPixFmtDescriptor* @prev);
    
    /// <summary>Utility function to access log2_chroma_w log2_chroma_h from the pixel format AVPixFmtDescriptor.</summary>
    /// <param name="pix_fmt">the pixel format</param>
    /// <param name="h_shift">store log2_chroma_w (horizontal/width shift)</param>
    /// <param name="v_shift">store log2_chroma_h (vertical/height shift)</param>
    /// <returns>0 on success, AVERROR(ENOSYS) on invalid or unknown pixel format</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_pix_fmt_get_chroma_sub_sample(AVPixelFormat @pix_fmt, int* @h_shift, int* @v_shift);
    
    /// <summary>Utility function to swap the endianness of a pixel format.</summary>
    /// <param name="pix_fmt">the pixel format</param>
    /// <returns>pixel format with swapped endianness if it exists, otherwise AV_PIX_FMT_NONE</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVPixelFormat av_pix_fmt_swap_endianness(AVPixelFormat @pix_fmt);

    /// <summary>Convert an AVRational to a IEEE 32-bit `float` expressed in fixed-point format.</summary>
    /// <param name="q">Rational to be converted</param>
    /// <returns>Equivalent floating-point value, expressed as an unsigned 32-bit integer.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint av_q2intfloat(AVRational @q);
    
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_read_image_line(ushort* @dst, in byte_ptr4 @data, in int4 @linesize, AVPixFmtDescriptor* @desc, int @x, int @y, int @c, int @w, int @read_pal_component);
    
    /// <summary>Read a line from an image, and write the values of the pixel format component c to dst.</summary>
    /// <param name="data">the array containing the pointers to the planes of the image</param>
    /// <param name="linesize">the array containing the linesizes of the image</param>
    /// <param name="desc">the pixel format descriptor for the image</param>
    /// <param name="x">the horizontal coordinate of the first pixel to read</param>
    /// <param name="y">the vertical coordinate of the first pixel to read</param>
    /// <param name="w">the width of the line to read, that is the number of values to write to dst</param>
    /// <param name="read_pal_component">if not zero and the format is a paletted format writes the values corresponding to the palette component c in data[1] to dst, rather than the palette indexes in data[0]. The behavior is undefined if the format is not paletted.</param>
    /// <param name="dst_element_size">size of elements in dst array (2 or 4 byte)</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_read_image_line2(void* @dst, in byte_ptr4 @data, in int4 @linesize, AVPixFmtDescriptor* @desc, int @x, int @y, int @c, int @w, int @read_pal_component, int @dst_element_size);

    /// <summary>Allocate, reallocate, or free a block of memory.</summary>
    /// <param name="ptr">Pointer to a memory block already allocated with av_realloc() or `NULL`</param>
    /// <param name="size">Size in bytes of the memory block to be allocated or reallocated</param>
    /// <returns>Pointer to a newly-reallocated block or `NULL` if the block cannot be reallocated</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void* av_realloc(void* @ptr, ulong @size);
    
    /// <summary>Allocate, reallocate, or free an array.</summary>
    /// <param name="ptr">Pointer to a memory block already allocated with av_realloc() or `NULL`</param>
    /// <param name="nmemb">Number of elements in the array</param>
    /// <param name="size">Size of the single element of the array</param>
    /// <returns>Pointer to a newly-reallocated block or NULL if the block cannot be reallocated</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void* av_realloc_array(void* @ptr, ulong @nmemb, ulong @size);
    
    /// <summary>Allocate, reallocate, or free a block of memory.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void* av_realloc_f(void* @ptr, ulong @nelem, ulong @elsize);
    
    /// <summary>Allocate, reallocate, or free a block of memory through a pointer to a pointer.</summary>
    /// <param name="ptr">Pointer to a pointer to a memory block already allocated with av_realloc(), or a pointer to `NULL`. The pointer is updated on success, or freed on failure.</param>
    /// <param name="size">Size in bytes for the memory block to be allocated or reallocated</param>
    /// <returns>Zero on success, an AVERROR error code on failure</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_reallocp(void* @ptr, ulong @size);
    
    /// <summary>Allocate, reallocate an array through a pointer to a pointer.</summary>
    /// <param name="ptr">Pointer to a pointer to a memory block already allocated with av_realloc(), or a pointer to `NULL`. The pointer is updated on success, or freed on failure.</param>
    /// <param name="nmemb">Number of elements</param>
    /// <param name="size">Size of the single element</param>
    /// <returns>Zero on success, an AVERROR error code on failure</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_reallocp_array(void* @ptr, ulong @nmemb, ulong @size);
    
    /// <summary>Reduce a fraction.</summary>
    /// <param name="dst_num">Destination numerator</param>
    /// <param name="dst_den">Destination denominator</param>
    /// <param name="num">Source numerator</param>
    /// <param name="den">Source denominator</param>
    /// <param name="max">Maximum allowed values for `dst_num` &amp; `dst_den`</param>
    /// <returns>1 if the operation is exact, 0 otherwise</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_reduce(int* @dst_num, int* @dst_den, long @num, long @den, long @max);
    
    /// <summary>Rescale a 64-bit integer with rounding to nearest.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern long av_rescale(long @a, long @b, long @c);
    
    /// <summary>Rescale a timestamp while preserving known durations.</summary>
    /// <param name="in_tb">Input time base</param>
    /// <param name="in_ts">Input timestamp</param>
    /// <param name="fs_tb">Duration time base; typically this is finer-grained (greater) than `in_tb` and `out_tb`</param>
    /// <param name="duration">Duration till the next call to this function (i.e. duration of the current packet/frame)</param>
    /// <param name="last">Pointer to a timestamp expressed in terms of `fs_tb`, acting as a state variable</param>
    /// <param name="out_tb">Output timebase</param>
    /// <returns>Timestamp expressed in terms of `out_tb`</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern long av_rescale_delta(AVRational @in_tb, long @in_ts, AVRational @fs_tb, int @duration, long* @last, AVRational @out_tb);
    
    /// <summary>Rescale a 64-bit integer by 2 rational numbers.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern long av_rescale_q(long @a, AVRational @bq, AVRational @cq);
    
    /// <summary>Rescale a 64-bit integer by 2 rational numbers with specified rounding.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern long av_rescale_q_rnd(long @a, AVRational @bq, AVRational @cq, AVRounding @rnd);
    
    /// <summary>Rescale a 64-bit integer with specified rounding.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern long av_rescale_rnd(long @a, long @b, long @c, AVRounding @rnd);
    
    /// <summary>Check if the sample format is planar.</summary>
    /// <param name="sample_fmt">the sample format to inspect</param>
    /// <returns>1 if the sample format is planar, 0 if it is interleaved</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_sample_fmt_is_planar(AVSampleFormat @sample_fmt);
    
    /// <summary>Allocate a samples buffer for nb_samples samples, and fill data pointers and linesize accordingly. The allocated samples buffer can be freed by using av_freep(&amp;audio_data[0]) Allocated data will be initialized to silence.</summary>
    /// <param name="audio_data">array to be filled with the pointer for each channel</param>
    /// <param name="linesize">aligned size for audio buffer(s), may be NULL</param>
    /// <param name="nb_channels">number of audio channels</param>
    /// <param name="nb_samples">number of samples per channel</param>
    /// <param name="sample_fmt">the sample format</param>
    /// <param name="align">buffer size alignment (0 = default, 1 = no alignment)</param>
    /// <returns>&gt;=0 on success or a negative error code on failure</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_samples_alloc(byte** @audio_data, int* @linesize, int @nb_channels, int @nb_samples, AVSampleFormat @sample_fmt, int @align);
    
    /// <summary>Allocate a data pointers array, samples buffer for nb_samples samples, and fill data pointers and linesize accordingly.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_samples_alloc_array_and_samples(byte*** @audio_data, int* @linesize, int @nb_channels, int @nb_samples, AVSampleFormat @sample_fmt, int @align);
    
    /// <summary>Copy samples from src to dst.</summary>
    /// <param name="dst">destination array of pointers to data planes</param>
    /// <param name="src">source array of pointers to data planes</param>
    /// <param name="dst_offset">offset in samples at which the data will be written to dst</param>
    /// <param name="src_offset">offset in samples at which the data will be read from src</param>
    /// <param name="nb_samples">number of samples to be copied</param>
    /// <param name="nb_channels">number of audio channels</param>
    /// <param name="sample_fmt">audio sample format</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_samples_copy(byte** @dst, byte** @src, int @dst_offset, int @src_offset, int @nb_samples, int @nb_channels, AVSampleFormat @sample_fmt);
    
    /// <summary>Fill plane data pointers and linesize for samples with sample format sample_fmt.</summary>
    /// <param name="audio_data">array to be filled with the pointer for each channel</param>
    /// <param name="linesize">calculated linesize, may be NULL</param>
    /// <param name="buf">the pointer to a buffer containing the samples</param>
    /// <param name="nb_channels">the number of channels</param>
    /// <param name="nb_samples">the number of samples in a single channel</param>
    /// <param name="sample_fmt">the sample format</param>
    /// <param name="align">buffer size alignment (0 = default, 1 = no alignment)</param>
    /// <returns>minimum size in bytes required for the buffer on success, or a negative error code on failure</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_samples_fill_arrays(byte** @audio_data, int* @linesize, byte* @buf, int @nb_channels, int @nb_samples, AVSampleFormat @sample_fmt, int @align);
    
    /// <summary>Get the required buffer size for the given audio parameters.</summary>
    /// <param name="linesize">calculated linesize, may be NULL</param>
    /// <param name="nb_channels">the number of channels</param>
    /// <param name="nb_samples">the number of samples in a single channel</param>
    /// <param name="sample_fmt">the sample format</param>
    /// <param name="align">buffer size alignment (0 = default, 1 = no alignment)</param>
    /// <returns>required buffer size, or negative error code on failure</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_samples_get_buffer_size(int* @linesize, int @nb_channels, int @nb_samples, AVSampleFormat @sample_fmt, int @align);
    
    /// <summary>Fill an audio buffer with silence.</summary>
    /// <param name="audio_data">array of pointers to data planes</param>
    /// <param name="offset">offset in samples at which to start filling</param>
    /// <param name="nb_samples">number of samples to fill</param>
    /// <param name="nb_channels">number of audio channels</param>
    /// <param name="sample_fmt">audio sample format</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_samples_set_silence(byte** @audio_data, int @offset, int @nb_samples, int @nb_channels, AVSampleFormat @sample_fmt);

    /// <summary>Parse the key/value pairs list in opts. For each key/value pair found, stores the value in the field in ctx that is named like the key. ctx must be an AVClass context, storing is done using AVOptions.</summary>
    /// <param name="opts">options string to parse, may be NULL</param>
    /// <param name="key_val_sep">a 0-terminated list of characters used to separate key from value</param>
    /// <param name="pairs_sep">a 0-terminated list of characters used to separate two pairs from each other</param>
    /// <returns>the number of successfully set key/value pairs, or a negative value corresponding to an AVERROR code in case of error: AVERROR(EINVAL) if opts cannot be parsed, the error code issued by av_opt_set() if a key/value pair cannot be set</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_set_options_string(void* @ctx,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @opts,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @key_val_sep,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @pairs_sep);

    /// <summary>Multiply two `size_t` values checking for overflow.</summary>
    /// <param name="a">Operand of multiplication</param>
    /// <param name="b">Operand of multiplication</param>
    /// <param name="r">Pointer to the result of the operation</param>
    /// <returns>0 on success, AVERROR(EINVAL) on overflow</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_size_mult(ulong @a, ulong @b, ulong* @r);
    
    /// <summary>Duplicate a string.</summary>
    /// <param name="s">String to be duplicated</param>
    /// <returns>Pointer to a newly-allocated string containing a copy of `s` or `NULL` if the string cannot be allocated</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern byte* av_strdup(    
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @s);

    /// <summary>Put a description of the AVERROR code errnum in errbuf. In case of failure the global variable errno is set to indicate the error. Even in case of failure av_strerror() will print a generic error message indicating the errnum provided to errbuf.</summary>
    /// <param name="errnum">error code to describe</param>
    /// <param name="errbuf">buffer to which description is written</param>
    /// <param name="errbuf_size">the size in bytes of errbuf</param>
    /// <returns>0 on success, a negative value if a description for errnum cannot be found</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_strerror(int @errnum, byte* @errbuf, ulong @errbuf_size);
    
    /// <summary>Duplicate a substring of a string.</summary>
    /// <param name="s">String to be duplicated</param>
    /// <param name="len">Maximum length of the resulting string (not counting the terminating byte)</param>
    /// <returns>Pointer to a newly-allocated string containing a substring of `s` or `NULL` if the string cannot be allocated</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern byte* av_strndup(    
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @s, ulong @len);
    
    /// <summary>Subtract one rational from another.</summary>
    /// <param name="b">First rational</param>
    /// <param name="c">Second rational</param>
    /// <returns>b-c</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVRational av_sub_q(AVRational @b, AVRational @c);
    
    /// <summary>Wrapper to work around the lack of mkstemp() on mingw. Also, tries to create file in /tmp first, if possible. *prefix can be a character constant; *filename will be allocated internally.</summary>
    /// <returns>file descriptor of opened file (or negative value corresponding to an AVERROR code on error) and opened file name in **filename.</returns>
    [Obsolete("as fd numbers cannot be passed saftely between libs on some platforms")]
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_tempfile(    
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @prefix, byte** @filename, int @log_offset, void* @log_ctx);
    
    /// <summary>Adjust frame number for NTSC drop frame time code.</summary>
    /// <param name="framenum">frame number to adjust</param>
    /// <param name="fps">frame per second, multiples of 30</param>
    /// <returns>adjusted frame number</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_timecode_adjust_ntsc_framenum2(int @framenum, int @fps);
    
    /// <summary>Check if the timecode feature is available for the given frame rate</summary>
    /// <returns>0 if supported, &lt; 0 otherwise</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_timecode_check_frame_rate(AVRational @rate);
    
    /// <summary>Convert sei info to SMPTE 12M binary representation.</summary>
    /// <param name="rate">frame rate in rational form</param>
    /// <param name="drop">drop flag</param>
    /// <param name="hh">hour</param>
    /// <param name="mm">minute</param>
    /// <param name="ss">second</param>
    /// <param name="ff">frame number</param>
    /// <returns>the SMPTE binary representation</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint av_timecode_get_smpte(AVRational @rate, int @drop, int @hh, int @mm, int @ss, int @ff);
    
    /// <summary>Convert frame number to SMPTE 12M binary representation.</summary>
    /// <param name="tc">timecode data correctly initialized</param>
    /// <param name="framenum">frame number</param>
    /// <returns>the SMPTE binary representation</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint av_timecode_get_smpte_from_framenum(AVTimecode* @tc, int @framenum);
    
    /// <summary>Init a timecode struct with the passed parameters.</summary>
    /// <param name="tc">pointer to an allocated AVTimecode</param>
    /// <param name="rate">frame rate in rational form</param>
    /// <param name="flags">miscellaneous flags such as drop frame, +24 hours, ... (see AVTimecodeFlag)</param>
    /// <param name="frame_start">the first frame number</param>
    /// <param name="log_ctx">a pointer to an arbitrary struct of which the first field is a pointer to an AVClass struct (used for av_log)</param>
    /// <returns>0 on success, AVERROR otherwise</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_timecode_init(AVTimecode* @tc, AVRational @rate, int @flags, int @frame_start, void* @log_ctx);
    
    /// <summary>Init a timecode struct from the passed timecode components.</summary>
    /// <param name="tc">pointer to an allocated AVTimecode</param>
    /// <param name="rate">frame rate in rational form</param>
    /// <param name="flags">miscellaneous flags such as drop frame, +24 hours, ... (see AVTimecodeFlag)</param>
    /// <param name="hh">hours</param>
    /// <param name="mm">minutes</param>
    /// <param name="ss">seconds</param>
    /// <param name="ff">frames</param>
    /// <param name="log_ctx">a pointer to an arbitrary struct of which the first field is a pointer to an AVClass struct (used for av_log)</param>
    /// <returns>0 on success, AVERROR otherwise</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_timecode_init_from_components(AVTimecode* @tc, AVRational @rate, int @flags, int @hh, int @mm, int @ss, int @ff, void* @log_ctx);
    
    /// <summary>Parse timecode representation (hh:mm:ss[:;.]ff).</summary>
    /// <param name="tc">pointer to an allocated AVTimecode</param>
    /// <param name="rate">frame rate in rational form</param>
    /// <param name="str">timecode string which will determine the frame start</param>
    /// <param name="log_ctx">a pointer to an arbitrary struct of which the first field is a pointer to an AVClass struct (used for av_log).</param>
    /// <returns>0 on success, AVERROR otherwise</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_timecode_init_from_string(AVTimecode* @tc, AVRational @rate,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @str, void* @log_ctx);
    
    /// <summary>Get the timecode string from the 25-bit timecode format (MPEG GOP format).</summary>
    /// <param name="buf">destination buffer, must be at least AV_TIMECODE_STR_SIZE long</param>
    /// <param name="tc25bit">the 25-bits timecode</param>
    /// <returns>the buf parameter</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern byte* av_timecode_make_mpeg_tc_string(byte* @buf, uint @tc25bit);
    
    /// <summary>Get the timecode string from the SMPTE timecode format.</summary>
    /// <param name="buf">destination buffer, must be at least AV_TIMECODE_STR_SIZE long</param>
    /// <param name="tcsmpte">the 32-bit SMPTE timecode</param>
    /// <param name="prevent_df">prevent the use of a drop flag when it is known the DF bit is arbitrary</param>
    /// <returns>the buf parameter</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern byte* av_timecode_make_smpte_tc_string(byte* @buf, uint @tcsmpte, int @prevent_df);
    
    /// <summary>Get the timecode string from the SMPTE timecode format.</summary>
    /// <param name="buf">destination buffer, must be at least AV_TIMECODE_STR_SIZE long</param>
    /// <param name="rate">frame rate of the timecode</param>
    /// <param name="tcsmpte">the 32-bit SMPTE timecode</param>
    /// <param name="prevent_df">prevent the use of a drop flag when it is known the DF bit is arbitrary</param>
    /// <param name="skip_field">prevent the use of a field flag when it is known the field bit is arbitrary (e.g. because it is used as PC flag)</param>
    /// <returns>the buf parameter</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern byte* av_timecode_make_smpte_tc_string2(byte* @buf, AVRational @rate, uint @tcsmpte, int @prevent_df, int @skip_field);
    
    /// <summary>Load timecode string in buf.</summary>
    /// <param name="tc">timecode data correctly initialized</param>
    /// <param name="buf">destination buffer, must be at least AV_TIMECODE_STR_SIZE long</param>
    /// <param name="framenum">frame number</param>
    /// <returns>the buf parameter</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern byte* av_timecode_make_string(AVTimecode* @tc, byte* @buf, int @framenum);
    
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_tree_destroy(AVTreeNode* @t);
    
    /// <summary>Apply enu(opaque, &amp;elem) to all the elements in the tree in a given range.</summary>
    /// <param name="cmp">a comparison function that returns &lt; 0 for an element below the range, &gt; 0 for an element above the range and == 0 for an element inside the range</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_tree_enumerate(AVTreeNode* @t, void* @opaque, av_tree_enumerate_cmp_func @cmp, av_tree_enumerate_enu_func @enu);
    
    /// <summary>Find an element.</summary>
    /// <param name="root">a pointer to the root node of the tree</param>
    /// <param name="cmp">compare function used to compare elements in the tree, API identical to that of Standard C&apos;s qsort It is guaranteed that the first and only the first argument to cmp() will be the key parameter to av_tree_find(), thus it could if the user wants, be a different type (like an opaque context).</param>
    /// <param name="next">If next is not NULL, then next[0] will contain the previous element and next[1] the next element. If either does not exist, then the corresponding entry in next is unchanged.</param>
    /// <returns>An element with cmp(key, elem) == 0 or NULL if no such element exists in the tree.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void* av_tree_find(AVTreeNode* @root, void* @key, av_tree_find_cmp_func @cmp, ref void_ptr2 @next);
    
    /// <summary>Insert or remove an element.</summary>
    /// <param name="rootp">A pointer to a pointer to the root node of the tree; note that the root node can change during insertions, this is required to keep the tree balanced.</param>
    /// <param name="key">pointer to the element key to insert in the tree</param>
    /// <param name="cmp">compare function used to compare elements in the tree, API identical to that of Standard C&apos;s qsort</param>
    /// <param name="next">Used to allocate and free AVTreeNodes. For insertion the user must set it to an allocated and zeroed object of at least av_tree_node_size bytes size. av_tree_insert() will set it to NULL if it has been consumed. For deleting elements *next is set to NULL by the user and av_tree_insert() will set it to the AVTreeNode which was used for the removed element. This allows the use of flat arrays, which have lower overhead compared to many malloced elements. You might want to define a function like:</param>
    /// <returns>If no insertion happened, the found element; if an insertion or removal happened, then either key or NULL will be returned. Which one it is depends on the tree state and the implementation. You should make no assumptions that it&apos;s one or the other in the code.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void* av_tree_insert(AVTreeNode** @rootp, void* @key, av_tree_insert_cmp_func @cmp, AVTreeNode** @next);
    
    /// <summary>Allocate an AVTreeNode.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVTreeNode* av_tree_node_alloc();

    /// <summary>Sleep for a period of time. Although the duration is expressed in microseconds, the actual delay may be rounded to the precision of the system timer.</summary>
    /// <param name="usec">Number of microseconds to sleep.</param>
    /// <returns>zero on success or (negative) error code.</returns>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern int av_usleep(uint @usec);
    
    /// <summary>Return an informative version string. This usually is the actual release version number or a git commit description. This string has no fixed format and can change any time. It should never be parsed by code.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public static extern string av_version_info();
    
    /// <summary>Send the specified message to the log if the level is less than or equal to the current av_log_level. By default, all logging messages are sent to stderr. This behavior can be altered by setting a different logging callback function.</summary>
    /// <param name="avcl">A pointer to an arbitrary struct of which the first field is a pointer to an AVClass struct.</param>
    /// <param name="level">The importance level of the message expressed using a &quot;Logging Constant&quot;.</param>
    /// <param name="fmt">The format string (printf-compatible) that specifies how subsequent arguments are converted to output.</param>
    /// <param name="vl">The arguments referenced by the format string.</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_vlog(void* @avcl, int @level,     
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8Marshaler))]
    string @fmt, byte* @vl);

    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_write_image_line(ushort* @src, ref byte_ptr4 @data, in int4 @linesize, AVPixFmtDescriptor* @desc, int @x, int @y, int @c, int @w);
    
    /// <summary>Write the values from src to the pixel format component c of an image line.</summary>
    /// <param name="src">array containing the values to write</param>
    /// <param name="data">the array containing the pointers to the planes of the image to write into. It is supposed to be zeroed.</param>
    /// <param name="linesize">the array containing the linesizes of the image</param>
    /// <param name="desc">the pixel format descriptor for the image</param>
    /// <param name="x">the horizontal coordinate of the first pixel to write</param>
    /// <param name="y">the vertical coordinate of the first pixel to write</param>
    /// <param name="w">the width of the line to write, that is the number of values to write to the image line</param>
    /// <param name="src_element_size">size of elements in src array (2 or 4 byte)</param>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern void av_write_image_line2(void* @src, ref byte_ptr4 @data, in int4 @linesize, AVPixFmtDescriptor* @desc, int @x, int @y, int @c, int @w, int @src_element_size);

    /// <summary>Return the libavutil build-time configuration.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public static extern string avutil_configuration();
    
    /// <summary>Return the libavutil license.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public static extern string avutil_license();
    
    /// <summary>Return the LIBAVUTIL_VERSION_INT constant.</summary>
    [DllImport("avutil", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint avutil_version();
}