#pragma warning disable CS8524 // If you use an unnamed enum, it's your fault.

using Celeste.Mod.TASRecorder.Util;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static FFmpeg.FFmpeg;

namespace Celeste.Mod.TASRecorder;

internal static class FFmpegLoader {
    private const string DownloadURL_Windows = "https://github.com/psyGamer/TASRecorder/releases/download/1.2.0/ffmpeg-win-x86_64.zip";
    private const string DownloadURL_MacOS = "https://github.com/psyGamer/TASRecorder/releases/download/1.2.0/ffmpeg-osx-x86_64.zip";
    private const string DownloadURL_Linux = "https://github.com/psyGamer/TASRecorder/releases/download/1.2.0/ffmpeg-linux-x86_64.zip";

    private const string ZipHash_Windows = "8742ebf87871493db23353e7e920c1fc";
    private const string ZipHash_MacOS = "18e961aca440791205adbda05e4abe42";
    private const string ZipHash_Linux = "383e868b312e08ef659d02f1004fc86e";

    // LibraryName, LibraryHash
    private static readonly (string, string)[] Libraries_Windows = {
        ("avcodec-60.dll",          "78f17b54ce564c63fa4284223828fb88"),
        ("avformat-60.dll",         "56b7a85bd45af66bc08f2448e2e23268"),
        ("avutil-58.dll",           "3b480fce00a6909525b47e58b9d7c169"),
        ("swresample-4.dll",        "2e320e921e33d3d6ae5cefeb0db7311f"),
        ("swscale-7.dll",           "b65b6381ee377a089963cc4de4d5abdb"),
    };
    private static readonly (string, string)[] Libraries_MacOS = {
        ("libavcodec.59.dylib",     "4f45cc3f7e1eac37bc4371b8eb7a91d5"),
        ("libavformat.59.dylib",    "935eb3770670681473118b49106f592d"),
        ("libavutil.57.dylib",      "4076d57e60d66eacb18f7e319dbd50e7"),
        ("libswresample.4.dylib",   "fcf9226f353ba179898c9f80826dcd71"),
        ("libswscale.6.dylib",      "fc3bf2b9851ad63beaae6bf4cda842fa"),
    };
    private static readonly (string, string)[] Libraries_Linux = {
        ("libavcodec.so.60",        "d5a551c94b3fa6f0d369ea841a041d64"),
        ("libavformat.so.58",       "5d202691a26cc8fa8ccf7052ded306de"),
        ("libavutil.so.58",         "7c111b7bbb1298b1eccf58958c7f209d"),
        ("libswresample.so.4",      "b95825f1a3b0dd0579d4f49f6a4b589f"),
        ("libswscale.so.5",         "7a2f794751e9de78483cae0aaa4c76b6"),
    };

    private static string DownloadPath_Windows => Path.Combine(Everest.Loader.PathCache, "ffmpeg-win-x86_64.zip");
    private static string DownloadPath_MacOS => Path.Combine(Everest.Loader.PathCache, "ffmpeg-osx-x86_64.zip");
    private static string DownloadPath_Linux => Path.Combine(Everest.Loader.PathCache, "ffmpeg-linux-x86_64.zip");

    private static string InstallPath_Windows => Path.Combine(Everest.Loader.PathCache, "unmanaged-libs/lib-win-x64/TASRecorder");
    private static string InstallPath_MacOS => Path.Combine(Everest.Loader.PathCache, "unmanaged-libs/lib-osx/TASRecorder");
    private static string InstallPath_Linux => Path.Combine(Everest.Loader.PathCache, "unmanaged-libs/lib-linux/TASRecorder");

    private static string ChecksumPath_Windows => Path.Combine(Everest.Loader.PathCache, "unmanaged-libs/lib-win-x64/TASRecorder.sum");
    private static string ChecksumPath_MacOS => Path.Combine(Everest.Loader.PathCache, "unmanaged-libs/lib-osx/TASRecorder.sum");
    private static string ChecksumPath_Linux => Path.Combine(Everest.Loader.PathCache, "unmanaged-libs/lib-linux/TASRecorder.sum");

    private static string DownloadURL => OSUtil.Current switch {
        OS.Windows => DownloadURL_Windows,
        OS.MacOS => DownloadURL_MacOS,
        OS.Linux => DownloadURL_Linux,
    };
    private static string ZipHash => OSUtil.Current switch {
        OS.Windows => ZipHash_Windows,
        OS.MacOS => ZipHash_MacOS,
        OS.Linux => ZipHash_Linux,
    };

    private static (string, string)[] Libraries => OSUtil.Current switch {
        OS.Windows => Libraries_Windows,
        OS.MacOS => Libraries_MacOS,
        OS.Linux => Libraries_Linux,
    };

    private static string DownloadPath => OSUtil.Current switch {
        OS.Windows => DownloadPath_Windows,
        OS.MacOS => DownloadPath_MacOS,
        OS.Linux => DownloadPath_Linux,
    };
    private static string InstallPath => OSUtil.Current switch {
        OS.Windows => InstallPath_Windows,
        OS.MacOS => InstallPath_MacOS,
        OS.Linux => InstallPath_Linux,
    };
    private static string ChecksumPath => OSUtil.Current switch {
        OS.Windows => ChecksumPath_Windows,
        OS.MacOS => ChecksumPath_MacOS,
        OS.Linux => ChecksumPath_Linux,
    };

    private static string AvcodecName => Libraries[0].Item1;
    private static string AvformatName => Libraries[1].Item1;
    private static string AvutilName => Libraries[2].Item1;
    private static string SwresampleName => Libraries[3].Item1;
    private static string SwscaleName => Libraries[4].Item1;

    private static bool _installed = false;
    private static bool _validated = false;
    private static Task _validationTask;

    internal static bool Installed {
        get {
            if (_validated) return _installed;

            _validationTask ??= Validate();
            _validationTask.Wait();

            return _installed;
        }
    }

    private static IntPtr AvutilLibrary;
    private static IntPtr AvformatLibrary;
    private static IntPtr AvcodecLibrary;
    private static IntPtr SwresampleLibrary;
    private static IntPtr SwscaleLibrary;

    internal static void Load() {
        On.Celeste.Mod.EverestModuleAssemblyContext.LoadUnmanagedDll += On_EverestModuleAssemblyContext_LoadUnmanagedDLL;
    }
    internal static void Unload() {
        On.Celeste.Mod.EverestModuleAssemblyContext.LoadUnmanagedDll -= On_EverestModuleAssemblyContext_LoadUnmanagedDLL;
    }

    public static void ValidateIfRequired() {
        if (_validated) return;
        _validationTask = Validate();
    }
    private static Task Validate() => Task.Run(() => {
        try {
            // First, check for system libraries.
            Log.Debug("Attempting to load system FFmpeg libraries...");
            try {
                Log.Debug($"Loading {GetOSLibraryName("avutil")}");
                AvutilLibrary = NativeLibrary.Load(GetOSLibraryName("avutil"));
                Log.Debug($"Loading {GetOSLibraryName("avformat")}");
                AvformatLibrary = NativeLibrary.Load(GetOSLibraryName("avformat"));
                Log.Debug($"Loading {GetOSLibraryName("avcodec")}");
                AvcodecLibrary = NativeLibrary.Load(GetOSLibraryName("avcodec"));
                Log.Debug($"Loading {GetOSLibraryName("swresample")}");
                SwresampleLibrary = NativeLibrary.Load(GetOSLibraryName("swresample"));
                Log.Debug($"Loading {GetOSLibraryName("swscale")}");
                SwscaleLibrary = NativeLibrary.Load(GetOSLibraryName("swscale"));

                Log.Debug($"avutil: {GetVersionString(avutil_version())}");
                Log.Debug($"avformat: {GetVersionString(avformat_version())}");
                Log.Debug($"avcodec: {GetVersionString(avcodec_version())}");
                Log.Debug($"swresample: {GetVersionString(swresample_version())}");
                Log.Debug($"swscale: {GetVersionString(swscale_version())}");

                Log.Info("Successfully loaded system FFmpeg libraries.");

                // Libraries are installed on the system, delete the Cache if it exists.
                if (File.Exists(ChecksumPath))
                    File.Delete(ChecksumPath);
                if (Directory.Exists(InstallPath))
                    DeleteInstallDirectory();

                _installed = true;
                return;
            } catch (Exception) {
                NativeLibrary.Free(AvutilLibrary);
                NativeLibrary.Free(AvformatLibrary);
                NativeLibrary.Free(AvcodecLibrary);
                NativeLibrary.Free(SwresampleLibrary);
                NativeLibrary.Free(SwscaleLibrary);

                Log.Debug("Loading system FFmpeg libraries failed! Trying cache...");
            }

            if (!VerifyCache()) {
                Log.Debug("Invalid cache! Reinstalling...");
                if (File.Exists(ChecksumPath))
                    File.Delete(ChecksumPath);
                if (Directory.Exists(InstallPath))
                    DeleteInstallDirectory();

                if (!InstallLibraries()) {
                    Log.Error("Failed reinstalling libraries! Starting without FFmpeg libraries.");
                    return;
                }

                if (!LoadLibrariesFromCache()) {
                    Log.Error("Failed to load libraries from Cache! Starting without FFmpeg libraries.");
                    // This is very bad...
                    // Just delete the Cache and start the game without the libraries
                    if (File.Exists(ChecksumPath))
                        File.Delete(ChecksumPath);
                    if (Directory.Exists(InstallPath))
                        DeleteInstallDirectory();

                    return;
                }
            }

            // Actually verify that they are linked
            // A bit hacky, but we mark FFmpeg as correctly loaded from now on, until disproven.
            _validated = true;
            _installed = true;
            try {
                Log.Debug($"avutil: {GetVersionString(avutil_version())}");
                Log.Debug($"avformat: {GetVersionString(avformat_version())}");
                Log.Debug($"avcodec: {GetVersionString(avcodec_version())}");
                Log.Debug($"swresample: {GetVersionString(swresample_version())}");
                Log.Debug($"swscale: {GetVersionString(swscale_version())}");
            } catch (Exception ex) {
                Log.Error("Failed linking against FFmpeg libraries! Starting without FFmpeg libraries.");
                Log.Exception(ex);

                _installed = false;
                return;
            }

            Log.Debug("Successfully loaded libraries from cache.");
        } catch (Exception ex) {
            Log.Error("FFmpeg library validation failed! Starting without FFmpeg libraries.");
            Log.Exception(ex);
        } finally {
            _validated = true;
            _validationTask = null;
        }
    });

    private static bool VerifyCache() {
        Log.Debug("Verifying cache");

        Log.Debug("Checking for checksum...");
        if (!File.Exists(ChecksumPath)) return false;
        Log.Debug("Checking for install directory...");
        if (!Directory.Exists(InstallPath)) return false;

        // TODO: There might be more checks required for other platforms
        var files = Directory.GetFiles(InstallPath)
                             .Select(Path.GetFileName)
                             .Where(file => !file.StartsWith(".fuse_hidden")); // Linux

        using var md5 = MD5.Create();

        foreach (string file in files) {
            int libraryIndex = Array.FindIndex(Libraries, tuple => tuple.Item1 == file);
            if (libraryIndex == -1) {
                Log.Warn($"Unknown file found in TAS Recorder library cache: {file}");
                continue;
            }
            string libraryHash = Libraries[libraryIndex].Item2;

            using var fs = File.OpenRead(Path.Combine(InstallPath, file));
            string hash = BitConverter.ToString(md5.ComputeHash(fs)).Replace("-", "");

            if (!libraryHash.Equals(hash, StringComparison.OrdinalIgnoreCase)) return false;

            Log.Debug($"{file} has a valid checksum: {hash}");
        }

        // Try to load the libraries to check they're working.
        if (!LoadLibrariesFromCache()) return false;

        // The checksum might be outdated. If that's the case, Everest could just delete it.
        // If we made it here, we can safely renew it.
        string checksum = Everest.GetChecksum(TASRecorderModule.Instance.Metadata).ToHexadecimalString();
        File.WriteAllText(ChecksumPath, checksum);

        return true;
    }

    private static bool InstallLibraries() {
        try {
            Log.Info($"Starting download of {DownloadURL}");
            Everest.Updater.DownloadFileWithProgress(DownloadURL, DownloadPath, (_, _, _) => true);
            if (!File.Exists(DownloadPath)) {
                Log.Error($"Download failed! The ZIP file went missing");
                return false;
            }
            Log.Info($"Finished download");

            using var md5 = MD5.Create();

            // Verify downloaded .zip
            using (var fs = File.OpenRead(DownloadPath)) {
                string hash = BitConverter.ToString(md5.ComputeHash(fs)).Replace("-", "");
                if (!ZipHash.Equals(hash, StringComparison.OrdinalIgnoreCase)) {
                    Log.Error($"Installing FFmpeg libraries failed - Invalid checksum for ZIP file: Expected {ZipHash} got {hash}");
                    return false;
                }
                Log.Verbose($"ZIP has a valid checksum: {hash}");
            }

            Log.Info($"Extracting {DownloadPath} into {DownloadPath}");
            ZipFile.ExtractToDirectory(DownloadPath, InstallPath);
            Log.Info($"Successfully extracted ZIP");

            // Cleanup ZIP
            if (File.Exists(DownloadPath))
                File.Delete(DownloadPath);

            // Verify downloaded libraries
            foreach (var (library, libraryHash) in Libraries) {
                using var fs = File.OpenRead(Path.Combine(InstallPath, library));
                string hash = BitConverter.ToString(md5.ComputeHash(fs)).Replace("-", "");
                if (!libraryHash.Equals(hash, StringComparison.OrdinalIgnoreCase)) {
                    Log.Error($"Installing FFmpeg libraries failed - Invalid checksum for {library}: Expected {libraryHash} got {hash}");
                    return false;
                }
                Log.Verbose($"{library} has a valid checksum: {hash}");
            }

            // Make Everest think that it installed the libraries
            string checksum = Everest.GetChecksum(TASRecorderModule.Instance.Metadata).ToHexadecimalString();
            File.WriteAllText(ChecksumPath, checksum);

            return true;
        } catch (Exception ex) {
            Log.Error("Installing FFmpeg libraries failed!");
            Log.Exception(ex);
            return false;
        }
    }

    private static bool LoadLibrariesFromCache() {
        // THE ORDER IS IMPORTANT!
        // On MacOS/Linux it will try to search in the system path if it's not already loaded.
        // However if we are loading from Cache, there is no system library.
        Log.Debug("Trying to load libraries from cache...");
        try {
            Log.Debug($"Loading {Path.Combine(InstallPath, AvutilName)}");
            AvutilLibrary = NativeLibrary.Load(Path.Combine(InstallPath, AvutilName));
            Log.Debug($"Loading {Path.Combine(InstallPath, SwresampleName)}");
            SwresampleLibrary = NativeLibrary.Load(Path.Combine(InstallPath, SwresampleName)); // Depends on: avutil
            Log.Debug($"Loading {Path.Combine(InstallPath, SwscaleName)}");
            SwscaleLibrary = NativeLibrary.Load(Path.Combine(InstallPath, SwscaleName));       // Depends on: avutil
            Log.Debug($"Loading {Path.Combine(InstallPath, AvcodecName)}");
            AvcodecLibrary = NativeLibrary.Load(Path.Combine(InstallPath, AvcodecName));       // Depends on: avutil, swresample
            Log.Debug($"Loading {Path.Combine(InstallPath, AvformatName)}");
            AvformatLibrary = NativeLibrary.Load(Path.Combine(InstallPath, AvformatName));     // Depends on: avutil, avcodec, swresample

            return true;
        } catch (Exception) {
            NativeLibrary.Free(AvutilLibrary);
            NativeLibrary.Free(AvformatLibrary);
            NativeLibrary.Free(AvcodecLibrary);
            NativeLibrary.Free(SwresampleLibrary);
            NativeLibrary.Free(SwscaleLibrary);

            return false;
        }
    }

    private static nint On_EverestModuleAssemblyContext_LoadUnmanagedDLL(On.Celeste.Mod.EverestModuleAssemblyContext.orig_LoadUnmanagedDll orig, EverestModuleAssemblyContext self, string name) {
        if (name is "avutil" or "avformat" or "avcodec" or "swresample" or "swscale") {
            if (!Installed) return IntPtr.Zero;

            return name switch {
                "avutil" => AvutilLibrary,
                "avformat" => AvformatLibrary,
                "avcodec" => AvcodecLibrary,
                "swresample" => SwresampleLibrary,
                "swscale" => SwscaleLibrary,
                _ => IntPtr.Zero,
            };
        }

        return orig(self, name);
    }

    // The install directory is still used by Everest, so we can only delete the contents
    private static void DeleteInstallDirectory() {
        foreach (string file in Directory.GetFiles(InstallPath)) {
            try {
                File.Delete(file);
            } catch (IOException ex) {
                Log.Error($"Failed deleting {file}");
                Log.Exception(ex);
            }
        }
    }

    private static string GetOSLibraryName(string name) => OSUtil.Current switch {
        OS.Windows => $"{name}.dll",
        OS.MacOS => $"lib{name}.dylib",
        OS.Linux => $"lib{name}.so",
    };

    private static string GetVersionString(uint version) {
        uint major = version >> 16;
        uint minor = (version >> 8) & 0xF;
        uint micro = version & 0x0F;

        return $"{major}.{minor}.{micro}";
    }
}
