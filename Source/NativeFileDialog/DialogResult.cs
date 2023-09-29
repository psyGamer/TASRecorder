using NativeFileDialog.Native;
using System.Collections.Generic;

namespace NativeFileDialog;

public class DialogResult {
    private readonly NFD_Result result;

    public readonly string Path;
    public readonly IReadOnlyList<string> Paths;
    public readonly string ErrorMessage;

    public bool IsOk => result == NFD_Result.NFD_OKAY;
    public bool IsCancelled => result == NFD_Result.NFD_CANCEL;
    public bool IsError => result == NFD_Result.NFD_ERROR;

    internal DialogResult(NFD_Result result, string path, IReadOnlyList<string> paths, string errorMessage) {
        this.result = result;
        Path = path;
        Paths = paths;
        ErrorMessage = errorMessage;
    }
}
