using Celeste.Mod.TASRecorder.Util;
using FFMpegCore;
using FFMpegCore.Helpers;
using Instances;
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
using Exception = System.Exception;

namespace Celeste.Mod.TASRecorder;

internal static class FFmpegLoader {
    private static readonly int MinimumAvcodecVersion    = AV_VERSION_INT(60, 0, 0);
    private static readonly int MinimumAvformatVersion   = AV_VERSION_INT(60, 0, 0);
    private static readonly int MinimumAvutilVersion     = AV_VERSION_INT(58, 0, 0);
    private static readonly int MinimumSwresampleVersion = AV_VERSION_INT(4, 0, 0);
    private static readonly int MinimumSwscaleVersion    = AV_VERSION_INT(7, 0, 0);

    #region OS Specific Configuration
    private const string DownloadURL_Windows = "https://github.com/psyGamer/TASRecorder/releases/download/1.7.0/ffmpeg-win-x86_64.zip";
    private const string DownloadURL_MacOS = "https://github.com/psyGamer/TASRecorder/releases/download/1.7.0/ffmpeg-osx-x86_64.zip";
    private const string DownloadURL_Linux = "https://github.com/psyGamer/TASRecorder/releases/download/1.7.1/ffmpeg-linux-x86_64.zip";

    private const string ZipHash_Windows = "8a7ee646500392f28ce49c09cd8dc859";
    private const string ZipHash_MacOS = "6c579e8d08b5aeea8c9409d5acc289bd";
    private const string ZipHash_Linux = "a39d0d20a2c76dd7a19c14ca9409f7b6";

    // Pair of the file name and it's MD5 hash
    private static readonly (string, string)[] Libraries_Windows = {
        ("avcodec-60.dll",            "5689e446e6533d0aa0152b6add3d9811"),
        ("avformat-60.dll",           "4772904e1a42be1cc97c9c658bb74960"),
        ("avutil-58.dll",             "46e4312361c90a3b6aa9eca8dc50fd52"),
        ("swresample-4.dll",          "4edb2cacfe7051ce78a5dcf2ca2c3b1e"),
        ("swscale-7.dll",             "bc2d0aca8a0de40b8d8bd71b71dd7d8f"),
        ("ffmpeg.exe",                "dd412087c5afd1de96744f07804a52f8"),
    };
    private static readonly (string, string)[] Libraries_MacOS = {
        ("libavcodec.60.dylib",       "f2988372be194a5216d972bc584903a5"),
        ("libavformat.60.dylib",      "e3709f3dd44533059e568c90b9076833"),
        ("libavutil.58.dylib",        "5d014e086b96a89edd294ff99c4b7a4b"),
        ("libswresample.4.dylib",     "b2dae6d20631d1313242965b669e8009"),
        ("libswscale.7.dylib",        "5c70072e6d713664576a553e986b8f64"),
        ("ffmpeg",                    "c7bbeeae6e5f5235814074c3d062e875"),
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
        ("libavcodec.so.60",          "b5033f99966e8ba1d3c40888d396993b"),
        ("libavformat.so.60",         "78d3d849f8635c1ab1e1272dba4c7273"),
        ("libavutil.so.58",           "3e616a4e2a7bd788380f564f4ed08507"),
        ("libswresample.so.4",        "0c1978d061944ea2510cd4242e24c6a8"),
        ("libswscale.so.7",           "6b733d54a8ee8b0404cc16cb6ca54fd1"),
        ("ffmpeg",                    "a88c5b472252a6b611c7b809b292a08b"),
        // Dependencies, since some distros cant manage to ship up-to-date ones
        ("libdrm.so.2",               "6a353987d769c2b4bd6746dc5749c759"),
        ("libm.so.6",                 "94a7f3ad26fb52d578da8a6465b5d4b5"),
        ("libOpenCL.so.1",            "00c3c4e49078d939658d7d48afdd1fff"),
        ("libavfilter.so.9",          "be589d9bbbddabc697c36ecf2b0a3080"),
        ("libavdevice.so.60",         "f86c2623bf41c2ab297c0c4aebf0abb7"),
        ("libplacebo.so.338",         "6c557de58e86fe8a65a617d7bedf4b8e"),
        ("libpostproc.so.57",         "8605e8d42363f07cead1ce3047ee9d8f"),
        ("libva-drm.so.2",            "df6e73aa8a3f481d6abdf6479f644b5e"),
        ("libva.so.2",                "b0a97ea72d04fccb4bcfefcb344eb322"),
        ("libvdpau.so.1",             "a5dc1731f5799f1c9484420db7c47bf5"),
        ("libvpx.so.8",               "ba922345752b86f3bd2029c25a2f263f"),
    };

    private static string DownloadPath_Windows => Path.Combine(Everest.Loader.PathCache, "TASRecorder", "ffmpeg-win-x86_64.zip");
    private static string DownloadPath_MacOS => Path.Combine(Everest.Loader.PathCache, "TASRecorder", "ffmpeg-osx-x86_64.zip");
    private static string DownloadPath_Linux => Path.Combine(Everest.Loader.PathCache, "TASRecorder", "ffmpeg-linux-x86_64.zip");

    private static string InstallPath => Path.Combine(Everest.Loader.PathCache, "TASRecorder");

#pragma warning disable CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.
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

    private static (string Name, string Hash)[] Libraries => OSUtil.Current switch {
        OS.Windows => Libraries_Windows,
        OS.MacOS => Libraries_MacOS,
        OS.Linux => Libraries_Linux,
    };

    private static string DownloadPath => OSUtil.Current switch {
        OS.Windows => DownloadPath_Windows,
        OS.MacOS => DownloadPath_MacOS,
        OS.Linux => DownloadPath_Linux,
    };
#pragma warning restore CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.

    private static string AvcodecName => Libraries[0].Item1;
    private static string AvformatName => Libraries[1].Item1;
    private static string AvutilName => Libraries[2].Item1;
    private static string SwresampleName => Libraries[3].Item1;
    private static string SwscaleName => Libraries[4].Item1;
    #endregion

    private static bool _usingSystemBinary = false;
    private static bool _binaryInstalled = false;
    private static bool _libraryInstalled = false;
    private static bool _validated = false;
    private static Task? _validationTask;

    internal static string BinaryPath => _usingSystemBinary ? string.Empty : InstallPath;
    internal static bool BinaryInstalled {
        get {
            if (_validated) return _binaryInstalled;

            _validationTask ??= Validate();
            Log.Debug("Waiting for validation to finish...");
            _validationTask.Wait();
            Log.Debug("Validation wait finished");

            return _binaryInstalled;
        }
    }
    internal static bool LibraryInstalled {
        get {
            if (_validated) return _libraryInstalled;

            _validationTask ??= Validate();
            Log.Debug("Waiting for validation to finish...");
            _validationTask.Wait();
            Log.Debug("Validation wait finished");

            return _libraryInstalled;
        }
    }

    private static IntPtr AvcodecLibrary;
    private static IntPtr AvformatLibrary;
    private static IntPtr AvutilLibrary;
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
            // First, check for a system install
            Log.Debug("Attempting to load system FFmpeg libraries...");
            try {
                AvcodecLibrary    = LoadLibrary(GetOSLibraryName("avcodec"));
                AvformatLibrary   = LoadLibrary(GetOSLibraryName("avformat"));
                AvutilLibrary     = LoadLibrary(GetOSLibraryName("avutil"));
                SwresampleLibrary = LoadLibrary(GetOSLibraryName("swresample"));
                SwscaleLibrary    = LoadLibrary(GetOSLibraryName("swscale"));

                // Flags are enabled early, so that the library resolver doesn't hang
                // They are reset on failure
                _validated = true;
                _libraryInstalled = true;

                // Actually verify that they are linked
                bool outdated = false;
                outdated |= CheckOutdatedLibrary("avcodec", avcodec_version(), MinimumAvcodecVersion);
                outdated |= CheckOutdatedLibrary("avformat", avformat_version(), MinimumAvformatVersion);
                outdated |= CheckOutdatedLibrary("avutil", avutil_version(), MinimumAvutilVersion);
                outdated |= CheckOutdatedLibrary("swresample", swresample_version(), MinimumSwresampleVersion);
                outdated |= CheckOutdatedLibrary("swscale", swscale_version(), MinimumSwscaleVersion);
                if (outdated)
                    throw new Exception("FFmpeg libraries outdated");

                Log.Info("Successfully loaded system FFmpeg libraries.");
            } catch (Exception ex) {
                NativeLibrary.Free(AvcodecLibrary);
                NativeLibrary.Free(AvformatLibrary);
                NativeLibrary.Free(AvutilLibrary);
                NativeLibrary.Free(SwresampleLibrary);
                NativeLibrary.Free(SwscaleLibrary);

                _libraryInstalled = false;

                Log.Debug("Loading system FFmpeg libraries failed! Trying cache...");
                // if (Logger.shouldLog(Log.TAG, LogLevel.Debug)) {
                    Log.Exception(ex);
                // }
            } finally {
                _validated = false;
            }

            Log.Debug("Attempting to load system FFmpeg binary...");
            try {
                // Check if the binary works
                var data = Instance.Finish(GlobalFFOptions.GetFFMpegBinaryPath(), "-version").OutputData;

                // Example Output:

                // 0:  ffmpeg version n6.1.1 Copyright (c) 2000-2023 the FFmpeg developers
                // 1:     built with gcc 13.2.1 (GCC) 20230801
                // 2:  configuration: --prefix=/usr ...
                // 3:  libavutil      58. 29.100 / 58. 29.100
                // 4:  libavcodec     60. 31.102 / 60. 31.102
                // 5:  libavformat    60. 16.100 / 60. 16.100
                // 6:  libavdevice    60.  3.100 / 60.  3.100
                // 7:  libavfilter     9. 12.100 /  9. 12.100
                // 8:  libswscale      7.  5.100 /  7.  5.100
                // 9:  libswresample   4. 12.100 /  4. 12.100
                // 10: libpostproc    57.  3.100 / 57.  3.100

                bool outdated = false;
                Log.Debug(data[0]);
                outdated |= CheckOutdatedBinary("libavutil", data[3], MinimumAvutilVersion);
                outdated |= CheckOutdatedBinary("libavcodec", data[4], MinimumAvcodecVersion);
                outdated |= CheckOutdatedBinary("libavformat", data[5], MinimumAvformatVersion);
                Log.Debug(data[6]);
                Log.Debug(data[7]);
                outdated |= CheckOutdatedBinary("libswscale", data[8], MinimumSwscaleVersion);
                outdated |= CheckOutdatedBinary("libswresample", data[9], MinimumSwresampleVersion);
                Log.Debug(data[7]);

                if (outdated)
                    throw new Exception("FFmpeg binary outdated");

                for (int i = 0; i < data.Count; i++) {
                    if (i is 1 or 2) continue; // Skip compile options
                    Log.Debug(data[i]);
                }

                _binaryInstalled = true;
                _usingSystemBinary = true;

                Log.Info("Successfully loaded system FFmpeg binary.");
            } catch (Exception ex) {
                Log.Debug("Loading system FFmpeg binary failed! Trying cache...");
                // if (Logger.shouldLog(Log.TAG, LogLevel.Debug)) {
                Log.Exception(ex);
                // }
            }

            if (_libraryInstalled && _binaryInstalled) {
                _validated = true;
                return;
            }

            if (!VerifyCache()) {
                Log.Debug("Invalid cache! Reinstalling...");

                if (Directory.Exists(InstallPath))
                    DeleteInstallDirectory();

                if (!InstallFFmpeg()) {
                    Log.Error("Failed to reinstall FFmpeg! Starting without FFmpeg.");
                    return;
                }

                if (!LoadLibrariesFromCache()) {
                    // This is very bad...
                    Log.Error("Failed to load FFmpeg libraries from Cache! Starting without FFmpeg libraries.");
                    return;
                }
            }

            // The the same as above, except use the libraries/binary from the cache instead

            if (!_libraryInstalled) {
                try {
                    _validated = true;
                    _libraryInstalled = true;

                    Log.Debug($"avcodec: {GetVersionString(avcodec_version())}");
                    Log.Debug($"avformat: {GetVersionString(avformat_version())}");
                    Log.Debug($"avutil: {GetVersionString(avutil_version())}");
                    Log.Debug($"swresample: {GetVersionString(swresample_version())}");
                    Log.Debug($"swscale: {GetVersionString(swscale_version())}");

                    Log.Debug("Successfully loaded FFMpeg libraries from cache.");
                } catch (Exception ex) {
                    Log.Error("Failed linking against FFmpeg libraries! Starting without FFmpeg libraries.");
                    Log.Exception(ex);

                    _libraryInstalled = false;
                } finally {
                    _validated = false;
                }
            }

            if (!_binaryInstalled) {
                try {
                    var res = Instance.Finish(GlobalFFOptions.GetFFMpegBinaryPath(new FFOptions { BinaryFolder = InstallPath }), "-version");
                    if (res.ExitCode != 0)
                        throw new Exception($"FFmpeg binary exited with non-zero exit code: {string.Join(Environment.NewLine, res.ErrorData)}");

                    _binaryInstalled = true;

                    Log.Debug("Successfully loaded FFmpeg binary from cache.");
                } catch (Exception ex) {
                    Log.Error("Failed executing FFmpeg binary! Starting without FFmpeg binary.");
                    Log.Exception(ex);
                }
            }
        } catch (Exception ex) {
            Log.Error("FFmpeg validation failed! Starting without FFmpeg.");
            Log.Exception(ex);
        } finally {
            _validated = true;
            _validationTask = null;
        }
    });

    private static bool VerifyCache() {
        Log.Debug("Verifying cache");

        Log.Debug("Checking for install directory...");
        if (!Directory.Exists(InstallPath)) {
            Log.Debug($"Install directory ({InstallPath}) not found!");
            return false;
        }

        // TODO: There might be more checks required for other platforms
        var files = Directory.GetFiles(InstallPath)
                             .Select(Path.GetFileName)
                             .Where(file => !file?.StartsWith(".fuse_hidden") ?? false) // Linux
                             .Select(x => x!);

        using var md5 = MD5.Create();

        foreach (string? file in files) {
            int libraryIndex = Array.FindIndex(Libraries, tuple => tuple.Item1 == file);
            if (libraryIndex == -1) {
                Log.Warn($"Unknown file found in cache: {file}");
                continue;
            }
            string libraryHash = Libraries[libraryIndex].Item2;

            using var fs = File.OpenRead(Path.Combine(InstallPath, file));
            string hash = BitConverter.ToString(md5.ComputeHash(fs)).Replace("-", "");

            if (!libraryHash.Equals(hash, StringComparison.OrdinalIgnoreCase)) {
                Log.Debug($"Checksum mismatch: Expected {libraryHash} got {hash}");
                return false;
            }

            Log.Verbose($"{file} has a valid checksum: {hash}");
        }

        // Try to load the libraries to check they're working.
        if (!_libraryInstalled && !LoadLibrariesFromCache()) return false;

        // Try to execute the binary
        if (!_binaryInstalled) {
            try {
                var res = Instance.Finish(GlobalFFOptions.GetFFMpegBinaryPath(new FFOptions { BinaryFolder = InstallPath }), "-version");
                if (res.ExitCode != 0)
                    throw new Exception($"FFmpeg binary exited with non-zero exit code: {string.Join(Environment.NewLine, res.ErrorData)}");
            } catch (Exception ex) {
                Log.Debug("Failed to execute binary from cache!");
                // if (Logger.shouldLog(Log.TAG, LogLevel.Debug)) {
                Log.Exception(ex);
                // }

                return false;
            }
        }

        return true;
    }

    private static bool InstallFFmpeg() {
        try {
            Log.Info($"Starting download of {DownloadURL}");
            using (HttpClient client = new()) {
                client.Timeout = TimeSpan.FromMinutes(5);
                using var res = client.GetAsync(DownloadURL).GetAwaiter().GetResult();

                string? path = Path.GetDirectoryName(DownloadPath);
                if (path != null && !Directory.Exists(path))
                    Directory.CreateDirectory(path);

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
                    Log.Error($"Installing FFmpeg failed - Invalid checksum for ZIP file: Expected {ZipHash} got {hash}");
                    return false;
                }
                Log.Verbose($"ZIP has a valid checksum: {hash}");
            }

            Log.Info($"Extracting {DownloadPath} into {InstallPath}");
            ZipFile.ExtractToDirectory(DownloadPath, InstallPath);
            Log.Info($"Successfully extracted ZIP");

            // Cleanup ZIP
            if (File.Exists(DownloadPath))
                File.Delete(DownloadPath);

            // Verify downloaded files
            foreach (var library in Libraries) {
                using var fs = File.OpenRead(Path.Combine(InstallPath, library.Name));
                string hash = BitConverter.ToString(md5.ComputeHash(fs)).Replace("-", "");
                if (!library.Hash.Equals(hash, StringComparison.OrdinalIgnoreCase)) {
                    Log.Error($"Installing FFmpeg failed - Invalid checksum for {library.Name}: Expected {library.Hash} got {hash}");
                    return false;
                }
                Log.Verbose($"{library.Name} has a valid checksum: {hash}");
            }

            return true;
        } catch (Exception ex) {
            Log.Error("Installing FFmpeg libraries failed!");
            Log.Exception(ex);
            return false;
        }
    }

    private static bool LoadLibrariesFromCache() {
        Log.Debug("Trying to load libraries from cache...");
        try {
            AvcodecLibrary    = LoadLibrary(Path.Combine(InstallPath, AvcodecName));
            AvformatLibrary   = LoadLibrary(Path.Combine(InstallPath, AvformatName));
            AvutilLibrary     = LoadLibrary(Path.Combine(InstallPath, AvutilName));
            SwresampleLibrary = LoadLibrary(Path.Combine(InstallPath, SwresampleName));
            SwscaleLibrary    = LoadLibrary(Path.Combine(InstallPath, SwscaleName));

            return true;
        } catch (Exception ex) {
            NativeLibrary.Free(AvcodecLibrary);
            NativeLibrary.Free(AvformatLibrary);
            NativeLibrary.Free(AvutilLibrary);
            NativeLibrary.Free(SwresampleLibrary);
            NativeLibrary.Free(SwscaleLibrary);

            Log.Debug("Failed to load libraries from cache!");
            // if (Logger.shouldLog(Log.TAG, LogLevel.Debug)) {
                Log.Exception(ex);
            // }

            return false;
        }
    }

    private static nint FFmpegDLLResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath) {
        if (!LibraryInstalled) return IntPtr.Zero;

        return libraryName switch {
            "avutil" => AvutilLibrary,
            "avformat" => AvformatLibrary,
            "avcodec" => AvcodecLibrary,
            "swresample" => SwresampleLibrary,
            "swscale" => SwscaleLibrary,
            _ => IntPtr.Zero,
        };
    }

    private static IntPtr LoadLibrary(string libraryName) {
        Log.Debug($"Loading {libraryName}...");
        return NativeLibrary.Load(libraryName);
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

#pragma warning disable CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.
    private static string GetOSLibraryName(string name) => OSUtil.Current switch {
        OS.Windows => $"{name}.dll",
        OS.MacOS => $"lib{name}.dylib",
        OS.Linux => $"lib{name}.so",
    };
#pragma warning restore CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.

    private static bool CheckOutdatedLibrary(string name, uint version, int minVersion) {
        if (version < minVersion) {
            Log.Warn($"{name}: {GetVersionString(version)} OUTDATED");
            return true;
        }

        Log.Debug($"{name}: {GetVersionString(version)}");
        return false;
    }

    private static bool CheckOutdatedBinary(string name, string versionText, int minVersion) {
        int start = name.Length;
        int end = versionText.IndexOf('/');

        string[] parts = end == -1
            ? versionText[start..].Trim().Split('.', '/')
            : versionText[start..end].Trim().Split('.', '/');

        if (parts.Length < 3) {
            Log.Warn($"{versionText} INVALID");
            return true;
        }

        bool failed = false;
        failed |= !uint.TryParse(parts[0].Trim(), out uint major);
        failed |= !uint.TryParse(parts[1].Trim(), out uint minor);
        failed |= !uint.TryParse(parts[2].Trim(), out uint micro);
        if (failed) {
            Log.Warn($"{versionText} INVALID");
            return true;
        }

        int version = AV_VERSION_INT(major, minor, micro);
        if (version < minVersion) {
            Log.Warn($"{versionText} OUTDATED");
            return true;
        }

        Log.Debug(versionText);
        return false;
    }

    public static string GetVersionString(uint version) {
        uint major = version >> 16;
        uint minor = (version >> 8) & 0xF;
        uint micro = version & 0x0F;

        return $"{major}.{minor}.{micro}";
    }
}
