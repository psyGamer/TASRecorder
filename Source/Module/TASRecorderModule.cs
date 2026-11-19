#pragma warning disable IDE0051 // Commands aren't directly called

using Celeste.Mod.TASRecorder.Util;
using FFMpegCore;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.RuntimeDetour;
using System;
using System.IO;
using static FFmpeg.FFmpeg;

namespace Celeste.Mod.TASRecorder;

// ReSharper disable once ClassNeverInstantiated.Global
public class TASRecorderModule : EverestModule {

    public static TASRecorderModule Instance { get; private set; } = null!;

    public override Type SettingsType => typeof(TASRecorderSettings);
    public static TASRecorderSettings Settings => (TASRecorderSettings) Instance._Settings;

    private static Hook? hook_Game_Exit;

    public TASRecorderModule() {
        Instance = this;
    }

    public override void Load() {
        // Activate verbose logging if loaded from a directory or when compiled in DEBUG mode
        if (!string.IsNullOrWhiteSpace(Instance.Metadata.PathDirectory))
            Logger.SetLogLevel(Log.TAG, LogLevel.Verbose);
#if DEBUG
        Logger.SetLogLevel(Log.TAG, LogLevel.Verbose);
#endif

        FFmpegLoader.Load();
        FFmpegLoader.ValidateIfRequired();

        VideoCapture.Load();
        AudioCapture.Load();

        OuiSetupHWAccel.Load();

        hook_Game_Exit = new Hook(
            typeof(Game).GetMethod("Exit")
            ?? throw new Exception($"{typeof(Game)} without Exit???"),
            On_Game_Exit
        );
    }
    public override void Unload() {
        if (RecordingManager.Recording) {
            RecordingManager.StopRecording();
        }

        VideoCapture.Unload();
        AudioCapture.Unload();

        OuiSetupHWAccel.Unload();

        hook_Game_Exit?.Dispose();
    }

    public override void CreateModMenuSection(TextMenu menu, bool inGame, EventInstance snapshot) {
        CreateModMenuSectionHeader(menu, inGame, snapshot);
        TASRecorderMenu.CreateSettingsMenu(menu);
    }

    public override void LoadSettings() {
        base.LoadSettings();

        string localSavePath = Path.Combine(UserIO.SavePath, $"localsettings-{Metadata.Name}.yaml");

        try {
            if (!File.Exists(localSavePath))
                return;

            using var stream = File.OpenRead(localSavePath);
            using var reader = new StreamReader(stream);

            var localSettings = new TASRecorderLocalSettings();
            YamlHelper.DeserializerUsing(localSettings).Deserialize(reader, typeof(TASRecorderLocalSettings));

            Settings.OutputDirectory = localSettings.OutputDirectory;
        } catch (Exception ex) {
            Log.Error($"Failed to load the local settings of {Metadata.Name}!");
            Log.Exception(ex);
        }
    }
    public override void SaveSettings() {
        base.SaveSettings();

        string localSavePath = Path.Combine(UserIO.SavePath, $"localsettings-{Metadata.Name}.yaml");

        try {
            if (File.Exists(localSavePath))
                File.Delete(localSavePath);
            Directory.CreateDirectory(Path.GetDirectoryName(localSavePath)!);

            using var stream = File.Create(localSavePath);
            using var writer = new StreamWriter(stream);

            var localSettings = new TASRecorderLocalSettings { OutputDirectory = Settings.OutputDirectory };

            YamlHelper.Serializer.Serialize(writer, localSettings, typeof(TASRecorderLocalSettings));
        } catch (Exception ex) {
            Log.Error($"Failed to save the local settings of {Metadata.Name}!");
            Log.Exception(ex);
        }
    }

    private delegate void orig_Game_Exit(Game self);
    private static void On_Game_Exit(orig_Game_Exit orig, Game self) {
        if (RecordingManager.Recording) {
            RecordingManager.StopRecording();
        }

        orig(self);
    }

    // ReSharper disable UnusedMember.Local
    [Command("start_recording", "Starts a frame-perfect recording")]
    private static void CmdStartRecording() {
        if (!TASRecorderAPI.IsFFmpegInstalled()) {
            Engine.Commands.Log("FFmpeg is not not correctly installed!", Color.Red);
            return;
        }
        if (RecordingManager.Recording) {
            Engine.Commands.Log("You are already recording!", Color.OrangeRed);
            return;
        }

        try {
            RecordingManager.StartRecording();
            Engine.Commands.Log("Successfully started recording.", Color.LightBlue);
        } catch (Exception ex) {
            Engine.Commands.Log("An unexpected error occured while trying to start the recording.", Color.Red);
            Engine.Commands.LogStackTrace(ex.StackTrace);
            Log.Error("Failed to start recording!");
            Log.Exception(ex);
        }
    }

    [Command("stop_recording", "Stops the recording")]
    private static void CmdStopRecording() {
        if (!TASRecorderAPI.IsFFmpegInstalled()) {
            Engine.Commands.Log("FFmpeg libraries are not correctly installed!", Color.Red);
            return;
        }
        if (!RecordingManager.Recording) {
            Engine.Commands.Log("You aren't currently recording!", Color.OrangeRed);
            return;
        }

        try {
            RecordingManager.StopRecording();
            Engine.Commands.Log("Successfully stopped recording.", Color.LightBlue);
        } catch (Exception ex) {
            Engine.Commands.Log("An unexpected error occured while trying to stop the recording.", Color.Red);
            Engine.Commands.LogStackTrace(ex.StackTrace);
            Log.Error("Failed to stop recording!");
            Log.Exception(ex);
        }
    }

    [Command("ffmpeg_check", "Checks whether FFmpeg is correctly installed")]
    private static void CmdFFmpegCheck() {
        if (FFmpegLoader.BinaryInstalled) {
            Engine.Commands.Log("FFmpeg binary correctly installed.", Color.Green);
            var data = Instances.Instance.Finish(GlobalFFOptions.GetFFMpegBinaryPath(new FFOptions { BinaryFolder = FFmpegLoader.BinaryPath }), "-version").OutputData;
            for (int i = 0; i < data.Count; i++) {
                if (i is 1 or 2) continue; // Skip compile options
                Engine.Commands.Log(data[i], Color.BlueViolet);
            }
        } else {
            Engine.Commands.Log("FFmpeg binary is not correctly installed!", Color.Red);
        }

        if (FFmpegLoader.LibraryInstalled) {
            Engine.Commands.Log("FFmpeg libraries correctly installed.", Color.Green);
            Engine.Commands.Log($"{FFmpegLoader.AvcodecName}: {FFmpegLoader.GetVersionString(avcodec_version())}", Color.BlueViolet);
            Engine.Commands.Log($"{FFmpegLoader.AvdeviceName}: {FFmpegLoader.GetVersionString(avdevice_version())}", Color.BlueViolet);
            Engine.Commands.Log($"{FFmpegLoader.AvfilterName}: {FFmpegLoader.GetVersionString(avfilter_version())}", Color.BlueViolet);
            Engine.Commands.Log($"{FFmpegLoader.AvformatName}: {FFmpegLoader.GetVersionString(avformat_version())}", Color.BlueViolet);
            Engine.Commands.Log($"{FFmpegLoader.AvutilName}: {FFmpegLoader.GetVersionString(avutil_version())}", Color.BlueViolet);
            Engine.Commands.Log($"{FFmpegLoader.SwresampleName}: {FFmpegLoader.GetVersionString(swresample_version())}", Color.BlueViolet);
            Engine.Commands.Log($"{FFmpegLoader.SwscaleName}: {FFmpegLoader.GetVersionString(swscale_version())}", Color.BlueViolet);
        } else {
            Engine.Commands.Log("FFmpeg libraries are not correctly installed!", Color.Red);
        }
    }
}
