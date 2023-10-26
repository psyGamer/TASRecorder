#pragma warning disable CS8524 // If you use an unnamed enum, it's your fault.

using Celeste.Mod.TASRecorder.Util;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using static FFmpeg.FFmpeg;

namespace Celeste.Mod.TASRecorder;

internal static class FFmpegLoader {
    private const string DownloadURL_Windows = "https://github.com/psyGamer/TASRecorder/releases/download/1.6.0/ffmpeg-win-x86_64.zip";
    private const string DownloadURL_MacOS = "https://github.com/psyGamer/TASRecorder/releases/download/1.5.0/ffmpeg-osx-x86_64.zip";
    private const string DownloadURL_Linux = "https://github.com/psyGamer/TASRecorder/releases/download/1.2.0/ffmpeg-linux-x86_64.zip";

    private const string ZipHash_Windows = "f2c5f692067d0500e94495a286b240e6";
    private const string ZipHash_MacOS = "aae8e853cbf736aa56f7e408276a4a23";
    private const string ZipHash_Linux = "383e868b312e08ef659d02f1004fc86e";

    // Pair of the file name and it's MD5 hash
    private static readonly (string, string)[] Libraries_Windows = {
        ("avcodec-60.dll",            "5689e446e6533d0aa0152b6add3d9811"),
        ("avformat-60.dll",           "4772904e1a42be1cc97c9c658bb74960"),
        ("avutil-58.dll",             "46e4312361c90a3b6aa9eca8dc50fd52"),
        ("swresample-4.dll",          "4edb2cacfe7051ce78a5dcf2ca2c3b1e"),
        ("swscale-7.dll",             "bc2d0aca8a0de40b8d8bd71b71dd7d8f"),
    };
    private static readonly (string, string)[] Libraries_MacOS = {
        ("libavcodec.60.dylib",       "f2988372be194a5216d972bc584903a5"),
        ("libavformat.60.dylib",      "e3709f3dd44533059e568c90b9076833"),
        ("libavutil.58.dylib",        "5d014e086b96a89edd294ff99c4b7a4b"),
        ("libswresample.4.dylib",     "b2dae6d20631d1313242965b669e8009"),
        ("libswscale.7.dylib",        "5c70072e6d713664576a553e986b8f64"),
        // Since MacOS for ARM64 doesn't have those libraries as x86-64, we need to provide them ourselves...
        ("libaom.3.dylib",            "3d5bf56d00f8e477e92a1ccc873532c8"),
        ("libaribb24.0.dylib",        "c15a0b7e478a9d4b86e5c0b6405ff7a1"),
        ("libbluray.2.dylib",         "71c08880757dab8807f28b1538ff87fc"),
        ("libbrotlicommon.1.dylib",   "49462746d4e5647ef0d7ceeb228f16b8"),
        ("libbrotlidec.1.dylib",      "76e37e040adfbe45944e00507fef46c6"),
        ("libbrotlienc.1.dylib",      "6aebcc6c0a6b3d8098222d99fb6eb62b"),
        ("libcjson.1.dylib",          "7cd61e2fedd6cc0a5d928241e01d10f7"),
        ("libcrypto.3.dylib",         "0e5b23a16902334f8a131cc1aa3688f4"),
        ("libdav1d.6.dylib",          "3ba22b58365e4964001b0d143e972011"),
        ("libfontconfig.1.dylib",     "416c30b9d93e98eeedcc63c136e4808e"),
        ("libfreetype.6.dylib",       "158b34311289d3d03b2289d42221ad08"),
        ("libgmp.10.dylib",           "c632e0a46a623bc1412bf912dc2c0686"),
        ("libgnutls.30.dylib",        "928984d69167ed542d7e5a400119fbb1"),
        ("libhogweed.6.dylib",        "a322807d61da249c8bb760302fd50e60"),
        ("libhwy.1.dylib",            "506adac922f6e519322f2bc85e924447"),
        ("libidn2.0.dylib",           "ba97a112547f1cbda8372b8f570689fd"),
        ("libintl.8.dylib",           "df1672429c2ee62bffdadd6173636344"),
        ("libjxl.0.8.dylib",          "d0a763fbac063b8217b6226ac6d0b2db"),
        ("libjxl_threads.0.8.dylib",  "ae84a3dce81caa70e6a3941848c52b96"),
        ("liblcms2.2.dylib",          "c64d180b7be14cc65ed4afa9bc51ca2c"),
        ("liblzma.5.dylib",           "b08e91750bac483720886905539feaf0"),
        ("libmbedcrypto.14.dylib",    "83637d0540b283d919b56a7378f37f2d"),
        ("libmp3lame.0.dylib",        "9fd321010f3993f7d9b528f24635ab96"),
        ("libnettle.8.dylib",         "9bf816ae7456d7b75eaa33acfbc004db"),
        ("libogg.0.dylib",            "6737a65148f23fa61656004e89cba784"),
        ("libopencore-amrnb.0.dylib", "e34496e3814c3f4f139e5b0416ecccf4"),
        ("libopencore-amrwb.0.dylib", "011cd81fd60de2f3ef239d7cc9271900"),
        ("libopenjp2.7.dylib",        "48f730e9210994df271beb5e06ed34ae"),
        ("libopus.0.dylib",           "a96a7913e6701833a721a7d2bf027e0d"),
        ("libp11-kit.0.dylib",        "45c8258209f779fe07bfee841ab2a3ff"),
        ("libpng16.16.dylib",         "897732890755f36b5550ce8bcb218d1a"),
        ("librav1e.0.6.dylib",        "6d7061344d6bec1110ceeb08330f1d68"),
        ("librist.4.dylib",           "ebcaec816d507f46b3e44758fe9c3429"),
        ("libsharpyuv.0.dylib",       "1f15566029b7e5916c1ce2f5fb579ecb"),
        ("libsnappy.1.dylib",         "595bc030e2333ff2613c59e29da7cc25"),
        ("libsodium.23.dylib",        "7f8e7b0bbef78463f89dc4c3ad008933"),
        ("libsoxr.0.dylib",           "e24c9955bdece4110b3e7d184b308bd0"),
        ("libspeex.1.dylib",          "cb4b62f6005593d1c787ef95272f487a"),
        ("libsrt.1.5.dylib",          "df0f75788c2ecfcc77e97172b7666b28"),
        ("libssl.3.dylib",            "ddafa47c6046a6e3043cadd2825ad044"),
        ("libSvtAv1Enc.1.dylib",      "88c17dff14a5e8cdd3f48eb45cd8e4d2"),
        ("libtasn1.6.dylib",          "a2f2c320057d6ec63ea36698c05c7ad5"),
        ("libtheoradec.1.dylib",      "f960ffc2b218e6fb3888aab3e0138be2"),
        ("libtheoraenc.1.dylib",      "811468611b4a9934a84d20fbae7fea12"),
        ("libunistring.5.dylib",      "8f802a9167510b505e63d44cde9760ad"),
        ("libvmaf.1.dylib",           "54d94f738c2b878734284afc87b26164"),
        ("libvorbis.0.dylib",         "66f12ca239d7e99fbb3f804073180cf2"),
        ("libvorbisenc.2.dylib",      "0ac3b82a268180774f5bc80ec8fb7ad6"),
        ("libvpx.8.dylib",            "ca1d3c2146cf26f18f2a034d6391e09b"),
        ("libwebp.7.dylib",           "708aa67aab6ce7880f99b7caf3acee6b"),
        ("libwebpmux.3.dylib",        "7d1e486a94b9b8c34541ececa4b938f7"),
        ("libX11.6.dylib",            "028cb3fb4739e6c3a3177548cca135f2"),
        ("libx264.164.dylib",         "d50949534e78e3c1cada2cbbf2beb7ef"),
        ("libx265.199.dylib",         "4be8a8e05242a6b6cd7cb221f99ca04d"),
        ("libXau.6.dylib",            "8352526dcb2d771c0e1b48b29809292f"),
        ("libxcb.1.dylib",            "44dd257f39394352d5c752108f44cde4"),
        ("libXdmcp.6.dylib",          "8a76d31cf8911cced30130fa46d8d891"),
        ("libzmq.5.dylib",            "cab51eff14e0415ef0944524c72d78ef"),
    };
    private static readonly (string, string)[] Libraries_Linux = {
        ("libavcodec.so.60",          "d5a551c94b3fa6f0d369ea841a041d64"),
        ("libavformat.so.58",         "5d202691a26cc8fa8ccf7052ded306de"),
        ("libavutil.so.58",           "7c111b7bbb1298b1eccf58958c7f209d"),
        ("libswresample.so.4",        "b95825f1a3b0dd0579d4f49f6a4b589f"),
        ("libswscale.so.5",           "7a2f794751e9de78483cae0aaa4c76b6"),
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
            Log.Debug("Waiting for validation to finish...");
            _validationTask.Wait();
            Log.Debug("Validation wait finished");

            return _installed;
        }
    }

    private static IntPtr AvutilLibrary;
    private static IntPtr AvformatLibrary;
    private static IntPtr AvcodecLibrary;
    private static IntPtr SwresampleLibrary;
    private static IntPtr SwscaleLibrary;

    internal static void Load() {
        NativeLibrary.SetDllImportResolver(typeof(TASRecorderModule).Assembly, FFmpegDLLResolver);
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
                Log.Debug($"Loading {GetOSLibraryName("avutil")}...");
                AvutilLibrary = NativeLibrary.Load(GetOSLibraryName("avutil"));
                Log.Debug($"Loading {GetOSLibraryName("avformat")}...");
                AvformatLibrary = NativeLibrary.Load(GetOSLibraryName("avformat"));
                Log.Debug($"Loading {GetOSLibraryName("avcodec")}...");
                AvcodecLibrary = NativeLibrary.Load(GetOSLibraryName("avcodec"));
                Log.Debug($"Loading {GetOSLibraryName("swresample")}...");
                SwresampleLibrary = NativeLibrary.Load(GetOSLibraryName("swresample"));
                Log.Debug($"Loading {GetOSLibraryName("swscale")}...");
                SwscaleLibrary = NativeLibrary.Load(GetOSLibraryName("swscale"));

                // Actually verify that they are linked
                // Mark FFmpeg as correctly loaded from now on, until disproven
                _validated = true;
                _installed = true;

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

                return;
            } catch (Exception ex) {
                NativeLibrary.Free(AvutilLibrary);
                NativeLibrary.Free(AvformatLibrary);
                NativeLibrary.Free(AvcodecLibrary);
                NativeLibrary.Free(SwresampleLibrary);
                NativeLibrary.Free(SwscaleLibrary);

                _validated = false;
                _installed = false;

                Log.Debug("Loading system FFmpeg libraries failed! Trying cache...");
                if (Logger.shouldLog(Log.TAG, LogLevel.Debug)) {
                    Log.Exception(ex);
                }
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
            // Mark FFmpeg as correctly loaded from now on, until disproven
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
        if (!File.Exists(ChecksumPath)) {
            Log.Debug($"Checksum file ({ChecksumPath}) not found!");
            return false;
        }
        Log.Debug("Checking for install directory...");
        if (!Directory.Exists(InstallPath)) {
            Log.Debug($"Install directory ({InstallPath}) not found!");
            return false;
        }

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

            if (!libraryHash.Equals(hash, StringComparison.OrdinalIgnoreCase)) {
                Log.Debug($"Checksum mismatch: Expected {libraryHash} got {hash}");
                return false;
            }

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
            using (var client = new HttpClient()) {
                using var res = client.GetAsync(DownloadURL).GetAwaiter().GetResult();
                using var fs = File.OpenWrite(DownloadPath);
                res.Content.CopyTo(fs, null, CancellationToken.None);
            }
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
            Log.Debug($"Loading {Path.Combine(InstallPath, AvutilName)}...");
            AvutilLibrary = NativeLibrary.Load(Path.Combine(InstallPath, AvutilName));
            Log.Debug($"Loading {Path.Combine(InstallPath, SwresampleName)}...");
            SwresampleLibrary = NativeLibrary.Load(Path.Combine(InstallPath, SwresampleName)); // Depends on: avutil
            Log.Debug($"Loading {Path.Combine(InstallPath, SwscaleName)}...");
            SwscaleLibrary = NativeLibrary.Load(Path.Combine(InstallPath, SwscaleName));       // Depends on: avutil
            Log.Debug($"Loading {Path.Combine(InstallPath, AvcodecName)}...");
            AvcodecLibrary = NativeLibrary.Load(Path.Combine(InstallPath, AvcodecName));       // Depends on: avutil, swresample
            Log.Debug($"Loading {Path.Combine(InstallPath, AvformatName)}...");
            AvformatLibrary = NativeLibrary.Load(Path.Combine(InstallPath, AvformatName));     // Depends on: avutil, avcodec, swresample

            return true;
        } catch (Exception ex) {
            NativeLibrary.Free(AvutilLibrary);
            NativeLibrary.Free(AvformatLibrary);
            NativeLibrary.Free(AvcodecLibrary);
            NativeLibrary.Free(SwresampleLibrary);
            NativeLibrary.Free(SwscaleLibrary);

            Log.Debug("Failed to load libraries libraries from cache!");
            if (Logger.shouldLog(Log.TAG, LogLevel.Debug)) {
                Log.Exception(ex);
            }

            return false;
        }
    }

    private static nint FFmpegDLLResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath) {
        if (!Installed) return IntPtr.Zero;

        return libraryName switch {
            "avutil" => AvutilLibrary,
            "avformat" => AvformatLibrary,
            "avcodec" => AvcodecLibrary,
            "swresample" => SwresampleLibrary,
            "swscale" => SwscaleLibrary,
            _ => IntPtr.Zero,
        };
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

    public static string GetVersionString(uint version) {
        uint major = version >> 16;
        uint minor = (version >> 8) & 0xF;
        uint micro = version & 0x0F;

        return $"{major}.{minor}.{micro}";
    }
}
