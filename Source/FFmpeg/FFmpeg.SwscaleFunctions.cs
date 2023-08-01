using System.Runtime.InteropServices;
using FFmpeg.Util;

namespace FFmpeg;

public static unsafe partial class FFmpeg {
    /// <summary>Allocate an empty SwsContext. This must be filled and passed to sws_init_context(). For filling see AVOptions, options.c and sws_setColorspaceDetails().</summary>
    [DllImport("swscale", CallingConvention = CallingConvention.Cdecl)]
    public static extern SwsContext* sws_alloc_context();
    
    /// <summary>Allocate and return an uninitialized vector with length coefficients.</summary>
    [DllImport("swscale", CallingConvention = CallingConvention.Cdecl)]
    public static extern SwsVector* sws_allocVec(int @length);
    
    /// <summary>Convert an 8-bit paletted frame into a frame with a color depth of 24 bits.</summary>
    /// <param name="src">source frame buffer</param>
    /// <param name="dst">destination frame buffer</param>
    /// <param name="num_pixels">number of pixels to convert</param>
    /// <param name="palette">array with [256] entries, which must match color arrangement (RGB or BGR) of src</param>
    [DllImport("swscale", CallingConvention = CallingConvention.Cdecl)]
    public static extern void sws_convertPalette8ToPacked24(byte* @src, byte* @dst, int @num_pixels, byte* @palette);
    
    /// <summary>Convert an 8-bit paletted frame into a frame with a color depth of 32 bits.</summary>
    /// <param name="src">source frame buffer</param>
    /// <param name="dst">destination frame buffer</param>
    /// <param name="num_pixels">number of pixels to convert</param>
    /// <param name="palette">array with [256] entries, which must match color arrangement (RGB or BGR) of src</param>
    [DllImport("swscale", CallingConvention = CallingConvention.Cdecl)]
    public static extern void sws_convertPalette8ToPacked32(byte* @src, byte* @dst, int @num_pixels, byte* @palette);
    
    /// <summary>Finish the scaling process for a pair of source/destination frames previously submitted with sws_frame_start(). Must be called after all sws_send_slice() and sws_receive_slice() calls are done, before any new sws_frame_start() calls.</summary>
    /// <param name="c">The scaling context</param>
    [DllImport("swscale", CallingConvention = CallingConvention.Cdecl)]
    public static extern void sws_frame_end(SwsContext* @c);
    
    /// <summary>Initialize the scaling process for a given pair of source/destination frames. Must be called before any calls to sws_send_slice() and sws_receive_slice().</summary>
    /// <param name="c">The scaling context</param>
    /// <param name="dst">The destination frame.</param>
    /// <param name="src">The source frame. The data buffers must be allocated, but the frame data does not have to be ready at this point. Data availability is then signalled by sws_send_slice().</param>
    /// <returns>0 on success, a negative AVERROR code on failure</returns>
    [DllImport("swscale", CallingConvention = CallingConvention.Cdecl)]
    public static extern int sws_frame_start(SwsContext* @c, AVFrame* @dst, AVFrame* @src);
    
    /// <summary>Free the swscaler context swsContext. If swsContext is NULL, then does nothing.</summary>
    [DllImport("swscale", CallingConvention = CallingConvention.Cdecl)]
    public static extern void sws_freeContext(SwsContext* @swsContext);
    
    [DllImport("swscale", CallingConvention = CallingConvention.Cdecl)]
    public static extern void sws_freeFilter(SwsFilter* @filter);
    
    [DllImport("swscale", CallingConvention = CallingConvention.Cdecl)]
    public static extern void sws_freeVec(SwsVector* @a);
    
    /// <summary>Get the AVClass for swsContext. It can be used in combination with AV_OPT_SEARCH_FAKE_OBJ for examining options.</summary>
    [DllImport("swscale", CallingConvention = CallingConvention.Cdecl)]
    public static extern AVClass* sws_get_class();
    
    /// <summary>Check if context can be reused, otherwise reallocate a new one.</summary>
    [DllImport("swscale", CallingConvention = CallingConvention.Cdecl)]
    public static extern SwsContext* sws_getCachedContext(SwsContext* @context, int @srcW, int @srcH, AVPixelFormat @srcFormat, int @dstW, int @dstH, AVPixelFormat @dstFormat, int @flags, SwsFilter* @srcFilter, SwsFilter* @dstFilter, double* @param);
    
    /// <summary>Return a pointer to yuv&lt;-&gt;rgb coefficients for the given colorspace suitable for sws_setColorspaceDetails().</summary>
    /// <param name="colorspace">One of the SWS_CS_* macros. If invalid, SWS_CS_DEFAULT is used.</param>
    [DllImport("swscale", CallingConvention = CallingConvention.Cdecl)]
    public static extern int* sws_getCoefficients(int @colorspace);
    
    /// <summary>Returns A negative error code on error, non negative otherwise. If `LIBSWSCALE_VERSION_MAJOR &lt; 7`, returns -1 if not supported.</summary>
    /// <returns>A negative error code on error, non negative otherwise. If `LIBSWSCALE_VERSION_MAJOR &lt; 7`, returns -1 if not supported.</returns>
    [DllImport("swscale", CallingConvention = CallingConvention.Cdecl)]
    public static extern int sws_getColorspaceDetails(SwsContext* @c, int** @inv_table, int* @srcRange, int** @table, int* @dstRange, int* @brightness, int* @contrast, int* @saturation);
    
    /// <summary>Allocate and return an SwsContext. You need it to perform scaling/conversion operations using sws_scale().</summary>
    /// <param name="srcW">the width of the source image</param>
    /// <param name="srcH">the height of the source image</param>
    /// <param name="srcFormat">the source image format</param>
    /// <param name="dstW">the width of the destination image</param>
    /// <param name="dstH">the height of the destination image</param>
    /// <param name="dstFormat">the destination image format</param>
    /// <param name="flags">specify which algorithm and options to use for rescaling</param>
    /// <param name="param">extra parameters to tune the used scaler For SWS_BICUBIC param[0] and [1] tune the shape of the basis function, param[0] tunes f(1) and param[1] f´(1) For SWS_GAUSS param[0] tunes the exponent and thus cutoff frequency For SWS_LANCZOS param[0] tunes the width of the window function</param>
    /// <returns>a pointer to an allocated context, or NULL in case of error</returns>
    [DllImport("swscale", CallingConvention = CallingConvention.Cdecl)]
    public static extern SwsContext* sws_getContext(int @srcW, int @srcH, AVPixelFormat @srcFormat, int @dstW, int @dstH, AVPixelFormat @dstFormat, int @flags, SwsFilter* @srcFilter, SwsFilter* @dstFilter, double* @param);
    
    [DllImport("swscale", CallingConvention = CallingConvention.Cdecl)]
    public static extern SwsFilter* sws_getDefaultFilter(float @lumaGBlur, float @chromaGBlur, float @lumaSharpen, float @chromaSharpen, float @chromaHShift, float @chromaVShift, int @verbose);
    
    /// <summary>Return a normalized Gaussian curve used to filter stuff quality = 3 is high quality, lower is lower quality.</summary>
    [DllImport("swscale", CallingConvention = CallingConvention.Cdecl)]
    public static extern SwsVector* sws_getGaussianVec(double @variance, double @quality);
    
    /// <summary>Initialize the swscaler context sws_context.</summary>
    /// <returns>zero or positive value on success, a negative value on error</returns>
    [DllImport("swscale", CallingConvention = CallingConvention.Cdecl)]
    public static extern int sws_init_context(SwsContext* @sws_context, SwsFilter* @srcFilter, SwsFilter* @dstFilter);
    
    /// <summary>Returns a positive value if an endianness conversion for pix_fmt is supported, 0 otherwise.</summary>
    /// <param name="pix_fmt">the pixel format</param>
    /// <returns>a positive value if an endianness conversion for pix_fmt is supported, 0 otherwise.</returns>
    [DllImport("swscale", CallingConvention = CallingConvention.Cdecl)]
    public static extern int sws_isSupportedEndiannessConversion(AVPixelFormat @pix_fmt);
    
    /// <summary>Return a positive value if pix_fmt is a supported input format, 0 otherwise.</summary>
    [DllImport("swscale", CallingConvention = CallingConvention.Cdecl)]
    public static extern int sws_isSupportedInput(AVPixelFormat @pix_fmt);
    
    /// <summary>Return a positive value if pix_fmt is a supported output format, 0 otherwise.</summary>
    [DllImport("swscale", CallingConvention = CallingConvention.Cdecl)]
    public static extern int sws_isSupportedOutput(AVPixelFormat @pix_fmt);
    
    /// <summary>Scale all the coefficients of a so that their sum equals height.</summary>
    [DllImport("swscale", CallingConvention = CallingConvention.Cdecl)]
    public static extern void sws_normalizeVec(SwsVector* @a, double @height);
    
    /// <summary>Request a horizontal slice of the output data to be written into the frame previously provided to sws_frame_start().</summary>
    /// <param name="c">The scaling context</param>
    /// <param name="slice_start">first row of the slice; must be a multiple of sws_receive_slice_alignment()</param>
    /// <param name="slice_height">number of rows in the slice; must be a multiple of sws_receive_slice_alignment(), except for the last slice (i.e. when slice_start+slice_height is equal to output frame height)</param>
    /// <returns>a non-negative number if the data was successfully written into the output AVERROR(EAGAIN) if more input data needs to be provided before the output can be produced another negative AVERROR code on other kinds of scaling failure</returns>
    [DllImport("swscale", CallingConvention = CallingConvention.Cdecl)]
    public static extern int sws_receive_slice(SwsContext* @c, uint @slice_start, uint @slice_height);
    
    /// <summary>Get the alignment required for slices</summary>
    /// <param name="c">The scaling context</param>
    /// <returns>alignment required for output slices requested with sws_receive_slice(). Slice offsets and sizes passed to sws_receive_slice() must be multiples of the value returned from this function.</returns>
    [DllImport("swscale", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint sws_receive_slice_alignment(SwsContext* @c);
    
    /// <summary>Scale the image slice in srcSlice and put the resulting scaled slice in the image in dst. A slice is a sequence of consecutive rows in an image.</summary>
    /// <param name="c">the scaling context previously created with sws_getContext()</param>
    /// <param name="srcSlice">the array containing the pointers to the planes of the source slice</param>
    /// <param name="srcStride">the array containing the strides for each plane of the source image</param>
    /// <param name="srcSliceY">the position in the source image of the slice to process, that is the number (counted starting from zero) in the image of the first row of the slice</param>
    /// <param name="srcSliceH">the height of the source slice, that is the number of rows in the slice</param>
    /// <param name="dst">the array containing the pointers to the planes of the destination image</param>
    /// <param name="dstStride">the array containing the strides for each plane of the destination image</param>
    /// <returns>the height of the output slice</returns>
    [DllImport("swscale", CallingConvention = CallingConvention.Cdecl)]
    public static extern int sws_scale(SwsContext* @c, byte*[] @srcSlice, int[] @srcStride, int @srcSliceY, int @srcSliceH, byte*[] @dst, int[] @dstStride);
    
    /// <summary>Scale source data from src and write the output to dst.</summary>
    /// <param name="c">The scaling context</param>
    /// <param name="dst">The destination frame. See documentation for sws_frame_start() for more details.</param>
    /// <param name="src">The source frame.</param>
    /// <returns>0 on success, a negative AVERROR code on failure</returns>
    [DllImport("swscale", CallingConvention = CallingConvention.Cdecl)]
    public static extern int sws_scale_frame(SwsContext* @c, AVFrame* @dst, AVFrame* @src);
    
    /// <summary>Scale all the coefficients of a by the scalar value.</summary>
    [DllImport("swscale", CallingConvention = CallingConvention.Cdecl)]
    public static extern void sws_scaleVec(SwsVector* @a, double @scalar);
    
    /// <summary>Indicate that a horizontal slice of input data is available in the source frame previously provided to sws_frame_start(). The slices may be provided in any order, but may not overlap. For vertically subsampled pixel formats, the slices must be aligned according to subsampling.</summary>
    /// <param name="c">The scaling context</param>
    /// <param name="slice_start">first row of the slice</param>
    /// <param name="slice_height">number of rows in the slice</param>
    /// <returns>a non-negative number on success, a negative AVERROR code on failure.</returns>
    [DllImport("swscale", CallingConvention = CallingConvention.Cdecl)]
    public static extern int sws_send_slice(SwsContext* @c, uint @slice_start, uint @slice_height);
    
    /// <summary>Returns A negative error code on error, non negative otherwise. If `LIBSWSCALE_VERSION_MAJOR &lt; 7`, returns -1 if not supported.</summary>
    /// <param name="c">the scaling context</param>
    /// <param name="inv_table">the yuv2rgb coefficients describing the input yuv space, normally ff_yuv2rgb_coeffs[x]</param>
    /// <param name="srcRange">flag indicating the while-black range of the input (1=jpeg / 0=mpeg)</param>
    /// <param name="table">the yuv2rgb coefficients describing the output yuv space, normally ff_yuv2rgb_coeffs[x]</param>
    /// <param name="dstRange">flag indicating the while-black range of the output (1=jpeg / 0=mpeg)</param>
    /// <param name="brightness">16.16 fixed point brightness correction</param>
    /// <param name="contrast">16.16 fixed point contrast correction</param>
    /// <param name="saturation">16.16 fixed point saturation correction</param>
    /// <returns>A negative error code on error, non negative otherwise. If `LIBSWSCALE_VERSION_MAJOR &lt; 7`, returns -1 if not supported.</returns>
    [DllImport("swscale", CallingConvention = CallingConvention.Cdecl)]
    public static extern int sws_setColorspaceDetails(SwsContext* @c, in int4 @inv_table, int @srcRange, in int4 @table, int @dstRange, int @brightness, int @contrast, int @saturation);
    
    /// <summary>Return the libswscale build-time configuration.</summary>
    [DllImport("swscale", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public static extern string swscale_configuration();
    
    /// <summary>Return the libswscale license.</summary>
    [DllImport("swscale", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
    public static extern string swscale_license();
    
    /// <summary>Color conversion and scaling library.</summary>
    [DllImport("swscale", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint swscale_version();
}