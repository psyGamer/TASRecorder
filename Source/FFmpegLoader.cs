using Celeste.Mod.TASRecorder.Util;
using FFMpegCore;
using Instances;
using System;
using System.Diagnostics;
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
    public const string AvcodecName = "avcodec";
    public const string AvdeviceName = "avdevice";
    public const string AvfilterName = "avfilter";
    public const string AvformatName = "avformat";
    public const string AvutilName = "avutil";
    public const string SwresampleName ="swresample";
    public const string SwscaleName = "swscale";
    public const string FFmpegName = "ffmpeg";

    private const string AvcodecVersion = "63";
    private const string AvdeviceVersion = "63";
    private const string AvfilterVersion = "12";
    private const string AvformatVersion = "63";
    private const string AvutilVersion = "61";
    private const string SwresampleVersion = "7";
    private const string SwscaleVersion = "10";

    #region OS Specific Configuration
    private const string DownloadURL_Windows = "https://nightly.link/psyGamer/TASRecorder/actions/runs/44617589657/ffmpeg-win-x86_64.zip.zip";
    private const string DownloadURL_Linux = "https://nightly.link/psyGamer/TASRecorder/actions/runs/44617589657/ffmpeg-linux-x86_64.zip.zip";
    private const string DownloadURL_MacOS = "https://nightly.link/psyGamer/TASRecorder/actions/runs/44617589657/ffmpeg-osx-x86_64.zip.zip";

    private const string ZipHash_Windows = "d9933b340129a1c676734b998b70e743";
    private const string ZipHash_Linux = "a4a2930a5ee18c8661463b2a4745784a";
    private const string ZipHash_MacOS = "85634d33e64b399fc51f7ca2683c49f3";

    private static readonly (string FileName, string FileHash)[] CacheFiles_Windows = [
        ($"{FFmpegName}.exe",                         "9e2d528497d537cd4be7806bf204ec04"),
        ($"{AvcodecName}-{AvcodecVersion}.dll",       "3e245a7d17463c7a1405eb6d64ba5773"),
        ($"{AvdeviceName}-{AvdeviceVersion}.dll",     "6ef903dafca842b2e383cf1b6232efea"),
        ($"{AvfilterName}-{AvfilterVersion}.dll",     "26f7f0fcfff46dd2c4150d1a42f11dbc"),
        ($"{AvformatName}-{AvformatVersion}.dll",     "e2feb8fd1eee9dadbe8c1d6179879d95"),
        ($"{AvutilName}-{AvutilVersion}.dll",         "c647df910280084145890e748fb692eb"),
        ($"{SwresampleName}-{SwresampleVersion}.dll", "99da4a5136fc43caf39f163a1589bdcd"),
        ($"{SwscaleName}-{SwscaleVersion}.dll",       "a388a4e409ee66ef3127fd0bec734dfb"),
    ];
    private static readonly (string FileName, string FileHash)[] CacheFiles_Linux = [
        ($"{FFmpegName}",                               "c03a932c9687e6dfe7ebdc704b0060c2"),
        ($"lib{AvcodecName}.so.{AvcodecVersion}",       "b9d9ac00aa8f62ea7512dc8dbec7a85c"),
        ($"lib{AvdeviceName}.so.{AvdeviceVersion}",     "2febd9e9c069e2baa00da6257387316f"),
        ($"lib{AvfilterName}.so.{AvfilterVersion}",     "eb046d5aa8355f5480322c542bb2fea0"),
        ($"lib{AvformatName}.so.{AvformatVersion}",     "f7b4a45d6c6913b1287d635ea21da38c"),
        ($"lib{AvutilName}.so.{AvutilVersion}",         "cc7f1cdc192a9999df6a17e8d4bc41f4"),
        ($"lib{SwresampleName}.so.{SwresampleVersion}", "fe34abc33a80ed3d448b10d3d07602a1"),
        ($"lib{SwscaleName}.so.{SwscaleVersion}",       "cb7285edd12e0ac4c6298764ef15b2cc"),
    ];
    private static readonly (string FileName, string FileHash)[] CacheFiles_MacOS = [
        ($"{FFmpegName}",                                  "1ef6da1cd0592808aa2a69c9502d5234"),
        ($"lib{AvcodecName}.{AvcodecVersion}.dylib",       "ab64f02da4d12009a5031823d5c8de5a"),
        ($"lib{AvdeviceName}.{AvdeviceVersion}.dylib",     "c1f3d9dd529f886fcb0e05119b13fca9"),
        ($"lib{AvfilterName}.{AvfilterVersion}.dylib",     "bb7da354b9f73cfd777bd4abd68e8da3"),
        ($"lib{AvformatName}.{AvformatVersion}.dylib",     "29734069f06ac014981853d52f2d6081"),
        ($"lib{AvutilName}.{AvutilVersion}.dylib",         "d834d0f53fea3849c91db3be845f8ff3"),
        ($"lib{SwresampleName}.{SwresampleVersion}.dylib", "2e51e529ca3917e8ce59ec5c1969204b"),
        ($"lib{SwscaleName}.{SwscaleVersion}.dylib",       "a38974225c8787017e21c82e282dc9ac"),
    ];

    private static string DownloadPath_Windows => Path.Combine(InstallPath, "ffmpeg-win-x86_64.zip");
    private static string DownloadPath_Linux => Path.Combine(InstallPath, "ffmpeg-linux-x86_64.zip");
    private static string DownloadPath_MacOS => Path.Combine(InstallPath, "ffmpeg-osx-x86_64.zip");

    private static string InstallPath => Path.Combine(Everest.Loader.PathCache, "TASRecorder");

#pragma warning disable CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.
    private static string DownloadURL => OSUtil.Current switch {
        OS.Windows => DownloadURL_Windows,
        OS.Linux => DownloadURL_Linux,
        OS.MacOS => DownloadURL_MacOS,
    };
    private static string ZipHash => OSUtil.Current switch {
        OS.Windows => ZipHash_Windows,
        OS.Linux => ZipHash_Linux,
        OS.MacOS => ZipHash_MacOS,
    };

    private static (string FileName, string FileHash)[] CacheFiles => OSUtil.Current switch {
        OS.Windows => CacheFiles_Windows,
        OS.Linux => CacheFiles_Linux,
        OS.MacOS => CacheFiles_MacOS,
    };

    private static string DownloadPath => OSUtil.Current switch {
        OS.Windows => DownloadPath_Windows,
        OS.Linux => DownloadPath_Linux,
        OS.MacOS => DownloadPath_MacOS,
    };
#pragma warning restore CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.

    #endregion

    private static bool _usingSystemBinary;
    private static bool _binaryInstalled;
    private static bool _libraryInstalled;
    private static bool _validated;
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

    private static IntPtr AvcodecLibrary = IntPtr.Zero;
    private static IntPtr AvdeviceLibrary = IntPtr.Zero;
    private static IntPtr AvfilterLibrary = IntPtr.Zero;
    private static IntPtr AvformatLibrary = IntPtr.Zero;
    private static IntPtr AvutilLibrary = IntPtr.Zero;
    private static IntPtr SwresampleLibrary = IntPtr.Zero;
    private static IntPtr SwscaleLibrary = IntPtr.Zero;

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
                AvcodecLibrary    = LoadLibrary(GetOSLibraryName(AvcodecName, AvcodecVersion));
                AvdeviceLibrary   = LoadLibrary(GetOSLibraryName(AvdeviceName, AvdeviceVersion));
                AvfilterLibrary   = LoadLibrary(GetOSLibraryName(AvfilterName, AvfilterVersion));
                AvformatLibrary   = LoadLibrary(GetOSLibraryName(AvformatName, AvformatVersion));
                AvutilLibrary     = LoadLibrary(GetOSLibraryName(AvutilName, AvutilVersion));
                SwresampleLibrary = LoadLibrary(GetOSLibraryName(SwresampleName, SwresampleVersion));
                SwscaleLibrary    = LoadLibrary(GetOSLibraryName(SwscaleName, SwscaleVersion));

                // Flags are enabled early, so that the library resolver doesn't hang
                // They are reset on failure
                _validated = true;
                _libraryInstalled = true;

                // Actually verify that they are linked and have the correct major version
                bool invalid = false;
                invalid |= CheckLibraryVersion(AvcodecName, avcodec_version(), uint.Parse(AvcodecVersion));
                invalid |= CheckLibraryVersion(AvdeviceName, avdevice_version(), uint.Parse(AvdeviceVersion));
                invalid |= CheckLibraryVersion(AvfilterName, avfilter_version(), uint.Parse(AvfilterVersion));
                invalid |= CheckLibraryVersion(AvformatName, avformat_version(), uint.Parse(AvformatVersion));
                invalid |= CheckLibraryVersion(AvutilName, avutil_version(), uint.Parse(AvutilVersion));
                invalid |= CheckLibraryVersion(SwresampleName, swresample_version(), uint.Parse(SwresampleVersion));
                invalid |= CheckLibraryVersion(SwscaleName, swscale_version(), uint.Parse(SwscaleVersion));
                if (invalid)
                    throw new Exception("System FFmpeg libraries invalid");

                Log.Info("Successfully loaded system FFmpeg libraries.");
            } catch (Exception ex) {
                NativeLibrary.Free(AvcodecLibrary);
                NativeLibrary.Free(AvdeviceLibrary);
                NativeLibrary.Free(AvfilterLibrary);
                NativeLibrary.Free(AvformatLibrary);
                NativeLibrary.Free(AvutilLibrary);
                NativeLibrary.Free(SwresampleLibrary);
                NativeLibrary.Free(SwscaleLibrary);

                _libraryInstalled = false;

                Log.Debug("Loading system FFmpeg libraries failed! Trying cache...");
                Log.Debug($"  => {ex.GetType().FullName}: {ex.Message}");
            } finally {
                _validated = false;
            }

            Log.Debug("Attempting to load system FFmpeg binary...");
            try {
                // Check if the binary works
                var versionOutput = Instance.Finish(GlobalFFOptions.GetFFMpegBinaryPath(), "-version").OutputData;

                // Example Output:

                // ffmpeg version 9.0.1 Copyright (c) 2000-2026 the FFmpeg developers
                // built with gcc 15.2.0 (crosstool-NG 1.28.0)
                // configuration: ...
                // libavutil      61.  1.101 / 61.  1.101
                // libavcodec     63.  1.101 / 63.  1.101
                // libavformat    63.  1.101 / 63.  1.101
                // libavdevice    63.  1.101 / 63.  1.101
                // libavfilter    12.  1.101 / 12.  1.101
                // libswscale     10.  1.101 / 10.  1.101
                // libswresample   7.  1.101 /  7.  1.101

                bool outdated = false;
                for (int i = 0; i < versionOutput.Count; i++) {
                    if (i is 1 or 2) continue; // Skip compile options

                    string line = versionOutput[i];
                    Log.Debug(line);

                    int slashIdx = line.IndexOf('/');
                    if (slashIdx < 0) {
                        // Skip non-version lines
                        continue;
                    }

                    string libraryName = line.Split(' ')[0];
                    string libraryVersion = line[libraryName.Length..slashIdx].Trim();

                    uint minVersion = uint.Parse(libraryName switch {
                        $"lib{AvcodecName}" => AvcodecVersion,
                        $"lib{AvdeviceName}" => AvdeviceVersion,
                        $"lib{AvfilterName}" => AvfilterVersion,
                        $"lib{AvformatName}" => AvformatVersion,
                        $"lib{AvutilName}" => AvutilVersion,
                        $"lib{SwresampleName}" => SwresampleVersion,
                        $"lib{SwscaleName}" => SwscaleVersion,
                        _ => "0",
                    });

                    outdated |= CheckBinaryVersion(libraryName, libraryVersion, minVersion);
                }

                if (outdated)
                    throw new Exception("System FFmpeg binary outdated");

                _binaryInstalled = true;
                _usingSystemBinary = true;

                Log.Info("Successfully loaded system FFmpeg binary.");
            } catch (Exception ex) {
                Log.Debug("Loading system FFmpeg binary failed! Trying cache...");
                Log.Debug($"  => {ex.GetType().FullName}: {ex.Message}");
            }

            if (_libraryInstalled && _binaryInstalled) {
                // We have a fully working install
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
                    Log.Error("Failed to load FFmpeg libraries from Cache! Starting without FFmpeg libraries.");
                }
            }

            // The same as above, except use the libraries/binary from the cache instead

            if (!_libraryInstalled) {
                try {
                    _validated = true;
                    _libraryInstalled = true;

                    Log.Debug($"{AvcodecName} cache-library v{GetVersionString(avcodec_version())}");
                    Log.Debug($"{AvdeviceName} cache-library v{GetVersionString(avdevice_version())}");
                    Log.Debug($"{AvfilterName} cache-library v{GetVersionString(avfilter_version())}");
                    Log.Debug($"{AvformatName} cache-library v{GetVersionString(avformat_version())}");
                    Log.Debug($"{AvutilName} cache-library v{GetVersionString(avutil_version())}");
                    Log.Debug($"{SwresampleName} cache-library v{GetVersionString(swresample_version())}");
                    Log.Debug($"{SwscaleName} cache-library v{GetVersionString(swscale_version())}");

                    Log.Debug("Successfully loaded FFMpeg libraries from cache.");
                } catch (Exception ex) {
                    Log.Error("Failed linking against FFmpeg libraries! Starting without FFmpeg libraries.");
                    Log.Error($"  => {ex.GetType().FullName}: {ex.Message}");

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
                    Log.Error($"  => {ex.GetType().FullName}: {ex.Message}");
                }
            }
        } catch (Exception ex) {
            Log.Error("FFmpeg validation failed! Starting without FFmpeg.");
            Log.Error($"  => {ex.GetType().FullName}: {ex.Message}");
        } finally {
            _validated = true;
            _validationTask = null;
        }
    });

    private static bool VerifyCache() {
        Log.Debug("Verifying cache...");

        Log.Debug("Checking for install directory...");
        if (!Directory.Exists(InstallPath)) {
            Log.Debug($"Install directory ({InstallPath}) not found!");
            return false;
        }

        // TODO: There might be more checks required for other platforms
        var files = Directory.GetFiles(InstallPath)
            .Select(Path.GetFileName)
            .Where(file => file is not null)
            .Select(x => x!)
            .Where(file => !file.StartsWith(".fuse_hidden")); // Linux

        using var md5 = MD5.Create();

        foreach (string file in files) {
            int fileIndex = Array.FindIndex(CacheFiles, tuple => tuple.Item1 == file);
            if (fileIndex < 0) {
                Log.Warn($"Unknown file found in cache: {file}");
                continue;
            }

            string targetHash = CacheFiles[fileIndex].Item2;

            using var fs = File.OpenRead(Path.Combine(InstallPath, file));
            string actualHash = BitConverter.ToString(md5.ComputeHash(fs)).Replace("-", "").ToLower();

            if (!targetHash.Equals(actualHash, StringComparison.OrdinalIgnoreCase)) {
                Log.Debug($"Invalid checksum for file '{file}': Expected {targetHash}, got {actualHash}");
                return false;
            }

            Log.Verbose($"File '{file}' has a valid checksum: {actualHash}");
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
                Log.Debug($"  => {ex.GetType().FullName}: {ex.Message}");

                return false;
            }
        }

        return true;
    }

    private static bool InstallFFmpeg() {
        try {
            Log.Info($"Downloading FFmpeg from '{DownloadURL}'...");
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
                Log.Error("Download failed! The ZIP file went missing.");
                return false;
            }
            Log.Info("Finished downloading FFmpeg.");

            using var md5 = MD5.Create();

            // Verify downloaded .zip
            using (var fs = File.OpenRead(DownloadPath)) {
                string hash = BitConverter.ToString(md5.ComputeHash(fs)).Replace("-", "").ToLower();
                if (!ZipHash.Equals(hash, StringComparison.OrdinalIgnoreCase)) {
                    Log.Error($"Installing FFmpeg failed - Invalid checksum for ZIP file: Expected {ZipHash}, got {hash}");
                    return false;
                }
                Log.Verbose($"ZIP has a valid checksum: {hash}");
            }

            Log.Info($"Extracting '{DownloadPath}' into '{InstallPath}'...");
            ZipFile.ExtractToDirectory(DownloadPath, InstallPath);
            Log.Info("Successfully extracted FFmpeg.");

            // Cleanup ZIP
            if (File.Exists(DownloadPath))
                File.Delete(DownloadPath);

            // Verify downloaded files
            foreach (var library in CacheFiles) {
                using var fs = File.OpenRead(Path.Combine(InstallPath, library.FileName));
                string hash = BitConverter.ToString(md5.ComputeHash(fs)).Replace("-", "").ToLower();
                if (!library.FileHash.Equals(hash, StringComparison.OrdinalIgnoreCase)) {
                    Log.Error($"Installing FFmpeg failed - Invalid checksum for {library.FileName}: Expected {library.FileHash}, got {hash}");
                    return false;
                }
                Log.Verbose($"File '{library.FileName}' has a valid checksum: {hash}");
            }

            // Make binary executable
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                if (!ExecuteCommand(["chmod", "+x", Path.Combine(InstallPath, "ffmpeg")])) {
                    Log.Error($"Installing FFmpeg failed - Couldn't make FFmpeg binary executable");
                    return false;
                }
            }
            // Clear code-signing flags
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                foreach ((string file, _) in CacheFiles) {
                    if (!ExecuteCommand(["xattr", "-c", Path.Combine(InstallPath, file)])) {
                        Log.Error($"Installing FFmpeg failed - Couldn't clear attributes of '{file}'");
                        return false;
                    }
                }
            }

            return true;
        } catch (Exception ex) {
            Log.Error("Installing FFmpeg libraries failed!");
            Log.Error($"  => {ex.GetType().FullName}: {ex.Message}");
            return false;
        }
    }

    private static bool ExecuteCommand(string[] parameters) {
        var startInfo = new ProcessStartInfo(parameters[0]) {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };
        foreach (string param in parameters[1..]) {
            startInfo.ArgumentList.Add(param);
        }

        var proc = Process.Start(startInfo)!;
        proc.WaitForExit();

        string? line;
        if (proc.ExitCode != 0) {
            while ((line = proc.StandardOutput.ReadLine()) != null) {
                Log.Info(line);
            }
            while ((line = proc.StandardError.ReadLine()) != null) {
                Log.Error(line);
            }
        } else {
            while ((line = proc.StandardOutput.ReadLine()) != null) {
                Log.Debug(line);
            }
        }

        return proc.ExitCode == 0;
    }

    private static bool LoadLibrariesFromCache() {
        Log.Debug("Trying to load libraries from cache...");
        try {
            AvcodecLibrary    = LoadLibrary(Path.Combine(InstallPath, GetOSLibraryName(AvcodecName, AvcodecVersion)));
            AvdeviceLibrary   = LoadLibrary(Path.Combine(InstallPath, GetOSLibraryName(AvdeviceName, AvdeviceVersion)));
            AvfilterLibrary   = LoadLibrary(Path.Combine(InstallPath, GetOSLibraryName(AvfilterName, AvfilterVersion)));
            AvformatLibrary   = LoadLibrary(Path.Combine(InstallPath, GetOSLibraryName(AvformatName, AvformatVersion)));
            AvutilLibrary     = LoadLibrary(Path.Combine(InstallPath, GetOSLibraryName(AvutilName, AvutilVersion)));
            SwresampleLibrary = LoadLibrary(Path.Combine(InstallPath, GetOSLibraryName(SwresampleName, SwresampleVersion)));
            SwscaleLibrary    = LoadLibrary(Path.Combine(InstallPath, GetOSLibraryName(SwscaleName, SwscaleVersion)));

            return true;
        } catch (Exception ex) {
            NativeLibrary.Free(AvcodecLibrary);
            NativeLibrary.Free(AvdeviceLibrary);
            NativeLibrary.Free(AvfilterLibrary);
            NativeLibrary.Free(AvformatLibrary);
            NativeLibrary.Free(AvutilLibrary);
            NativeLibrary.Free(SwresampleLibrary);
            NativeLibrary.Free(SwscaleLibrary);

            Log.Debug("Failed to load libraries from cache!");
            Log.Debug($"  => {ex.GetType().FullName}: {ex.Message}");

            return false;
        }
    }

    private static nint FFmpegDLLResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath) {
        if (!LibraryInstalled) return IntPtr.Zero;

        return libraryName switch {
            $"{AvcodecName}-{AvcodecVersion}" => AvcodecLibrary,
            $"{AvdeviceName}-{AvdeviceVersion}" => AvdeviceLibrary,
            $"{AvfilterName}-{AvfilterVersion}" => AvfilterLibrary,
            $"{AvformatName}-{AvformatVersion}" => AvformatLibrary,
            $"{AvutilName}-{AvutilVersion}" => AvutilLibrary,
            $"{SwresampleName}-{SwresampleVersion}" => SwresampleLibrary,
            $"{SwscaleName}-{SwscaleVersion}" => SwscaleLibrary,
            _ => IntPtr.Zero,
        };
    }

    private static IntPtr LoadLibrary(string libraryName) {
        Log.Debug($"Loading '{libraryName}'...");
        return NativeLibrary.Load(libraryName);
    }

    // The installation directory is still used by Everest, so we can only delete the contents
    private static void DeleteInstallDirectory() {
        foreach (string file in Directory.GetFiles(InstallPath)) {
            try {
                File.Delete(file);
            } catch (IOException ex) {
                Log.Error($"Failed to delete '{file}' from cache.");
                Log.Error($"  => {ex.GetType().FullName}: {ex.Message}");
            }
        }
    }

#pragma warning disable CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.
    private static string GetOSLibraryName(string name, string version) => OSUtil.Current switch {
        OS.Windows => $"{name}-{version}.dll",
        OS.Linux => $"lib{name}.so.{version}",
        OS.MacOS => $"lib{name}.{version}.dylib",
    };
#pragma warning restore CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.

    private static bool CheckLibraryVersion(string name, uint currVersion, uint targetMajorVersion) {
        uint currMajor = AV_VERSION_MAJOR(currVersion);

        if (currMajor < targetMajorVersion) {
            Log.Warn($"{name} system-library v{GetVersionString(currVersion)}: Major version is older than {targetMajorVersion}");
            return true;
        }
        if (currMajor > targetMajorVersion) {
            Log.Warn($"{name} system-library v{GetVersionString(currVersion)}: Major version is newer than {targetMajorVersion}");
            return true;
        }

        Log.Debug($"{name} system-library v{GetVersionString(currVersion)}: Major version matches target");
        return false;
    }

    private static bool CheckBinaryVersion(string name, string versionText, uint minMajorVersion) {
        string[] parts = versionText.Split('.', '/');
        if (parts.Length != 3) {
            Log.Warn($"{name} system-binary v{versionText}: Invalid version format");
            return true;
        }

        bool invalid = false;
        invalid |= !uint.TryParse(parts[0].Trim(), out uint major);
        invalid |= !uint.TryParse(parts[1].Trim(), out uint minor);
        invalid |= !uint.TryParse(parts[2].Trim(), out uint micro);
        if (invalid) {
            Log.Warn($"{name} binary v{versionText}: Invalid version format");
            return true;
        }

        int version = AV_VERSION_INT(major, minor, micro);
        if (version < minMajorVersion) {
            Log.Warn($"{name} binary v{versionText}: Major version is older than {minMajorVersion}");
            return true;
        }

        Log.Debug($"{name} binary v{version}: Major version satisfies target");
        return false;
    }

    public static string GetVersionString(uint version) {
        uint major = version >> 16;
        uint minor = (version >> 8) & 0xF;
        uint micro = version & 0x0F;

        return $"{major}.{minor}.{micro}";
    }
}
