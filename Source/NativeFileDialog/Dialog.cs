using NativeFileDialog.Native;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NativeFileDialog;

public static class Dialog {
    public static unsafe DialogResult FileOpen(string filterList = null, string defaultPath = null) {
        fixed (byte* pFilterList = filterList != null ? Encoding.UTF8.GetBytes(filterList) : null)
        fixed (byte* pDefaultPath = defaultPath != null ? Encoding.UTF8.GetBytes(defaultPath) : null) {
            string path = null;
            string errorMessage = null;

            var result = NativeFunctions.NFD_OpenDialog(pFilterList, pDefaultPath, out nint pOutPath);

            if (result == NFD_Result.NFD_ERROR) {
                errorMessage = Marshal.PtrToStringUTF8((nint) NativeFunctions.NFD_GetError());
            } else if (result == NFD_Result.NFD_OKAY) {
                path = Marshal.PtrToStringUTF8(pOutPath);
                NativeFunctions.NFD_Free(pOutPath);
            }

            return new DialogResult(result, path, null, errorMessage);
        }
    }

    public static unsafe DialogResult FileSave(string filterList = null, string defaultPath = null) {
        fixed (byte* pFilterList = filterList != null ? Encoding.UTF8.GetBytes(filterList) : null)
        fixed (byte* pDefaultPath = defaultPath != null ? Encoding.UTF8.GetBytes(defaultPath) : null) {
            string path = null;
            string errorMessage = null;

            var result = NativeFunctions.NFD_SaveDialog(pFilterList, pDefaultPath, out nint pOutPath);

            if (result == NFD_Result.NFD_ERROR) {
                errorMessage = Marshal.PtrToStringUTF8((nint) NativeFunctions.NFD_GetError());
            } else if (result == NFD_Result.NFD_OKAY) {
                path = Marshal.PtrToStringUTF8(pOutPath);
                NativeFunctions.NFD_Free(pOutPath);
            }

            return new DialogResult(result, path, null, errorMessage);
        }
    }

    public static unsafe DialogResult FolderPicker(string defaultPath = null) {
        fixed (byte* pDefaultPath = defaultPath != null ? Encoding.UTF8.GetBytes(defaultPath) : null) {
            string path = null;
            string errorMessage = null;

            var result = NativeFunctions.NFD_PickFolder(pDefaultPath, out nint pOutPath);

            if (result == NFD_Result.NFD_ERROR) {
                errorMessage = Marshal.PtrToStringUTF8((nint) NativeFunctions.NFD_GetError());
            } else if (result == NFD_Result.NFD_OKAY) {
                path = Marshal.PtrToStringUTF8(pOutPath);
                NativeFunctions.NFD_Free(pOutPath);
            }

            return new DialogResult(result, path, null, errorMessage);
        }
    }

    public static unsafe DialogResult FileOpenMultiple(string filterList = null, string defaultPath = null) {
        fixed (byte* pFilterList = filterList != null ? Encoding.UTF8.GetBytes(filterList) : null)
        fixed (byte* pDefaultPath = defaultPath != null ? Encoding.UTF8.GetBytes(defaultPath) : null) {
            List<string> paths = null;
            string errorMessage = null;
            NFD_PathSet pathSet;

            var result = NativeFunctions.NFD_OpenDialogMultiple(pFilterList, pDefaultPath, &pathSet);

            if (result == NFD_Result.NFD_ERROR) {
                errorMessage = Marshal.PtrToStringUTF8((nint) NativeFunctions.NFD_GetError());
            } else if (result == NFD_Result.NFD_OKAY) {
                paths = new List<string>((int) NativeFunctions.NFD_PathSet_GetCount(&pathSet));

                for (int i = 0; i < paths.Count; i++) {
                    paths.Add(Marshal.PtrToStringUTF8((nint) NativeFunctions.NFD_PathSet_GetPath(&pathSet, (nuint) i)));
                }

                NativeFunctions.NFD_PathSet_Free(&pathSet);
            }

            return new DialogResult(result, null, paths, errorMessage);
        }
    }
}
