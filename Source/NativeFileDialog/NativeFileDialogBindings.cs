#pragma warning disable

using System;
using System.Runtime.InteropServices;

namespace NativeFileDialog.Native;

public struct NFD_PathSet {
    public IntPtr buf;
    public IntPtr indices;
    public UIntPtr count;
}

public enum NFD_Result {
    NFD_ERROR,
    NFD_OKAY,
    NFD_CANCEL
}

public static class NativeFunctions {
    public const string LibraryName = "nfd";

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern unsafe NFD_Result NFD_OpenDialog(byte* filterList, byte* defaultPath, out IntPtr outPath);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern unsafe NFD_Result NFD_OpenDialogMultiple(byte* filterList, byte* defaultPath, NFD_PathSet* outPaths);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern unsafe NFD_Result NFD_SaveDialog(byte* filterList, byte* defaultPath, out IntPtr outPath);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern unsafe NFD_Result NFD_PickFolder(byte* defaultPath, out IntPtr outPath);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern unsafe byte* NFD_GetError();

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern unsafe UIntPtr NFD_PathSet_GetCount(NFD_PathSet* pathSet);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern unsafe byte* NFD_PathSet_GetPath(NFD_PathSet* pathSet, UIntPtr index);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern unsafe void NFD_PathSet_Free(NFD_PathSet* pathSet);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern unsafe void NFD_Dummy();

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern unsafe IntPtr NFD_Malloc(UIntPtr bytes);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern unsafe void NFD_Free(IntPtr ptr);
}
