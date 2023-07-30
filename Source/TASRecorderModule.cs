using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using FMOD.Studio;
using Monocle;
using Celeste.Mod.TASRecorder.Interop;

namespace Celeste.Mod.TASRecorder;

public class TASRecorderModule : EverestModule {

    public const string NAME = "TASRecorder";

    public static TASRecorderModule Instance { get; private set; }

    public override Type SettingsType => typeof(TASRecorderModuleSettings);
    public static TASRecorderModuleSettings Settings => (TASRecorderModuleSettings) Instance._Settings;

    public override Type SessionType => typeof(TASRecorderModuleSession);
    public static TASRecorderModuleSession Session => (TASRecorderModuleSession) Instance._Session;

    // Might be recording outside of a session
    private static Encoder _encoder = null;
    public static Encoder Encoder => _encoder;
    private static bool _recording = false;
    public static bool Recording => _recording;

    public TASRecorderModule() {
        Instance = this;

        FFmpeg.DynamicallyLinkedBindings.Initialize();
    }

    public override void Load() {
        VideoCapture.Load();
        AudioCapture.Load();
    }
    public override void Unload() {
        if (Recording) {
            StopRecording();
        }

        VideoCapture.Unload();
        AudioCapture.Unload();
    }

    public override void CreateModMenuSection(TextMenu menu, bool inGame, EventInstance snapshot) {
        CreateModMenuSectionHeader(menu, inGame, snapshot);
        TASRecorderMenu.CreateSettingsMenu(menu);
    }

    internal static void StartRecording(int frames = -1, string fileName = null) {
        _encoder = new Encoder(fileName);
        _recording = true;

        if (!Encoder.HasVideo && !Encoder.HasAudio) {
            Logger.Log(LogLevel.Warn, NAME, "Encoder has neither video nor audio! Aborting recording");
            _recording = false;
            _encoder = null;
            return;
        }

        TASRecorderMenu.DisableMenu();
        RecordingRenderer.Start(frames);

        if (frames > 0) {
            VideoCapture.CurrentFrameCount = 0;
            VideoCapture.TargetFrameCount = frames;
        } else {
            VideoCapture.TargetFrameCount = -1;
        }

        if (Encoder.HasVideo) VideoCapture.StartRecording();
        if (Encoder.HasAudio) AudioCapture.StartRecording();

        Logger.Log(LogLevel.Info, NAME, "Started recording!");
    }
    internal static void StopRecording() {
        _recording = false;

        if (Encoder.HasAudio) AudioCapture.StopRecording();

        _encoder.End();
        _encoder = null;

        TASRecorderMenu.EnableMenu();

        Logger.Log(LogLevel.Info, NAME, "Stopped recording!");
    }

#pragma warning disable IDE0051 // Commands aren't directly called

    [Command("start_recording", "Starts a frame-perfect recording")]
    private static void CmdStartRecording(int frames = -1) {
        if (Recording) {
            Engine.Commands.Log("You are already recording!", Color.OrangeRed);
            return;
        }
        if (!TASRecorderInterop.IsFFmpegInstalled()) {
            Engine.Commands.Log("FFmpeg libraries not correctly installed.", Color.Red);
            return;
        }

        try {
            StartRecording(frames);
            Engine.Commands.Log("Successfully started recording.", Color.LightBlue);
        } catch (Exception ex) {
            Engine.Commands.Log("An unexpeced error occured while trying to start the recording.", Color.Red);
            Engine.Commands.LogStackTrace(ex.StackTrace.ToString());
            Logger.Log(LogLevel.Error, NAME, "Failed to start recording!");
            Logger.LogDetailed(ex, NAME);
        }
    }

    [Command("stop_recording", "Stops the frame-perfect recording")]
    private static void CmdStopRecording() {
        if (!Recording) {
            Engine.Commands.Log("You aren't currently recording!", Color.OrangeRed);
            return;
        }

        try {
            StopRecording();
            Engine.Commands.Log("Successfully stopped recording.", Color.LightBlue);
        } catch (Exception ex) {
            Engine.Commands.Log("An unexpeced error occured while trying to stop the recording.", Color.Red);
            Engine.Commands.LogStackTrace(ex.StackTrace.ToString());
            Logger.Log(LogLevel.Error, NAME, "Failed to stop recording!");
            Logger.LogDetailed(ex, NAME);
        }
    }

    [Command("ffmpeg_check", "Checks wheather the FFmpeg libraries are correctly installed")]
    private static void CmdFFmpegCheck() {
        if (TASRecorderInterop.IsFFmpegInstalled()) {
            Engine.Commands.Log("FFmpeg libraries correctly installed.", Color.Green);
        } else {
            Engine.Commands.Log("FFmpeg libraries not correctly installed.", Color.Red);
        }
    }

    [Command("ffmpeg_install", "Installs the FFmpeg libraires (Windows Only)")]
    private static void CmdFFmpegInstall() {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            Engine.Commands.Log("This installer is only available for Windows! Please install the FFmpeg libraries by yourself.", Color.Red);
            return;
        }

        const string downloadURL = "https://github.com/psyGamer/TASRecorder/releases/download/1.0.1/ffmpeg-win-x86_64.zip";
        const string zipHash = "bedc399df7a793658f3ec8b515f4eb50";

        var dllHashes = new[] {
            ("avcodec-60.dll",       "78f17b54ce564c63fa4284223828fb88"),
            ("avformat-60.dll",      "56b7a85bd45af66bc08f2448e2e23268"),
            ("avutil-58.dll",        "3b480fce00a6909525b47e58b9d7c169"),
            ("swresample-4.dll",     "2e320e921e33d3d6ae5cefeb0db7311f"),
            ("swscale-7.dll",        "b65b6381ee377a089963cc4de4d5abdb"),
            ("uninstall_ffmpeg.bat", "767d94aae89580f51e96fdb7532249fc"),
        };
        var dllCopies = new[] {
            ("avcodec-60.dll",   "avcodec.dll"),
            ("avformat-60.dll",  "avformat.dll"),
            ("avutil-58.dll",    "avutil.dll"),
            ("swresample-4.dll", "swresample.dll"),
            ("swscale-7.dll",    "swscale.dll"),
        };

        string destinationPath = Path.Combine(Everest.PathEverest, "Mods/Cache/ffmpeg-win-x86_64.zip");
        string libFolderPath = Path.Combine(Everest.PathEverest, "Mods/Cache/unmanaged-libs/lib-win-x64/TASRecorder");
        string modHashPath = Path.Combine(Everest.PathEverest, "Mods/Cache/unmanaged-libs/lib-win-x64/TASRecorder.sum");

        bool cleanupDLLs = true;

        try {
            Everest.Updater.DownloadFileWithProgress(downloadURL, destinationPath, (_, _, _) => true);

            using var md5 = MD5.Create();

            // Verify downloaded .zip
            using (var fs = File.OpenRead(destinationPath)) {
                string hash = BitConverter.ToString(md5.ComputeHash(fs)).Replace("-", "");
                if (!zipHash.Equals(hash, StringComparison.OrdinalIgnoreCase)) {
                    Engine.Commands.Log($"Download failed! Invalid hash: Expected {zipHash} got {hash}", Color.Red);
                    return;
                }
            }

            ZipFile.ExtractToDirectory(destinationPath, libFolderPath);

            foreach (var (dll, dllHash) in dllHashes) {
                using var fs = File.OpenRead(Path.Combine(libFolderPath, dll));
                string hash = BitConverter.ToString(md5.ComputeHash(fs)).Replace("-", "");
                if (!dllHash.Equals(hash, StringComparison.OrdinalIgnoreCase)) {
                    Engine.Commands.Log($"Download failed! Invalid hash for {dll}: Expected {dllHash} got {hash}", Color.Red);
                    return;
                }
            }

            foreach (var (dll, dllCopy) in dllCopies) {
                File.Copy(Path.Combine(libFolderPath, dll), Path.Combine(libFolderPath, dllCopy));
            }

            // Make sure Everest treats our manually cached files as legit
            string modHash = (Instance.Metadata.Hash ?? Instance.Metadata.Multimeta.First(meta => meta.Hash != null).Hash).ToHexadecimalString();
            File.WriteAllText(modHashPath, modHash);

            cleanupDLLs = false;
            Engine.Commands.Log("Successfully installed the FFmpeg libraires! Please restart your game.", Color.Green);
        } catch (Exception ex) {
            Engine.Commands.Log("Download failed! An unexpected error occured!", Color.Red);
            Engine.Commands.LogStackTrace(ex.StackTrace.ToString());
            Logger.Log(LogLevel.Error, NAME, "Failed to install FFmpeg libraries!");
            Logger.LogDetailed(ex, NAME);
        } finally {
            // Clean-up
            if (File.Exists(destinationPath)) {
                File.Delete(destinationPath);
            }

            if (cleanupDLLs) {
                foreach (var (dll, _) in dllHashes) {
                    string path = Path.Combine(libFolderPath, dll);
                    if (File.Exists(path)) {
                        File.Delete(path);
                    }
                }
            }
        }
    }
}
