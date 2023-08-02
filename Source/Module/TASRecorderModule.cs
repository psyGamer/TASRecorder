#pragma warning disable IDE0051 // Commands aren't directly called

using Celeste.Mod.TASRecorder.Interop;
using Celeste.Mod.TASRecorder.Util;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.TASRecorder;

public class TASRecorderModule : EverestModule {

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
    }

    public override void Load() {
        // Activate verbose logging if loaded from a directory
        if (!string.IsNullOrWhiteSpace(Instance.Metadata.PathDirectory))
            Logger.SetLogLevel(Log.TAG, LogLevel.Verbose);

        FFmpegLoader.Load();
        FFmpegLoader.ValidateIfRequired();

        VideoCapture.Load();
        AudioCapture.Load();
    }
    public override void Unload() {
        if (Recording) {
            StopRecording();
        }

        VideoCapture.Unload();
        AudioCapture.Unload();

        FFmpegLoader.Unload();
    }

    public override void CreateModMenuSection(TextMenu menu, bool inGame, EventInstance snapshot) {
        CreateModMenuSectionHeader(menu, inGame, snapshot);
        TASRecorderMenu.CreateSettingsMenu(menu);
    }

    internal static void StartRecording(int frames = -1, string fileName = null) {
        _encoder = new Encoder(fileName);
        _recording = true;

        if (!Encoder.HasVideo && !Encoder.HasAudio) {
            Log.Warn("Encoder has neither video nor audio! Aborting recording");
            _recording = false;
            _encoder = null;
            return;
        }

        RecordingRenderer.Start(frames);
        TASRecorderMenu.OnStateChanged();

        if (frames > 0) {
            VideoCapture.CurrentFrameCount = 0;
            VideoCapture.TargetFrameCount = frames;
        } else {
            VideoCapture.TargetFrameCount = -1;
        }

        if (Encoder.HasAudio) AudioCapture.StartRecording();

        Log.Info("Started recording!");
    }
    internal static void StopRecording() {
        _recording = false;

        if (Encoder.HasAudio) AudioCapture.StopRecording();

        _encoder.End();
        _encoder = null;

        // Can't use properties directly, since they have a recording check and it already stopped
        Settings._speed = 1.0f;
        if (Settings.resetSfxMuteState) {
            Audio.BusMuted(Buses.GAMEPLAY, false);
            Audio.BusMuted(Buses.UI, false);
            Audio.BusMuted(Buses.STINGS, false);
            Settings.resetSfxMuteState = false;
        }

        TASRecorderMenu.OnStateChanged();

        Log.Info("Stopped recording!");
    }

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
            Log.Error("Failed to start recording!");
            Log.Exception(ex);
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
            Log.Error("Failed to stop recording!");
            Log.Exception(ex);
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
}
