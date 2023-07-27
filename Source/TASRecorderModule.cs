using System;
using Microsoft.Xna.Framework;
using FMOD.Studio;
using Monocle;
using System.Diagnostics.CodeAnalysis;

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
        Settings.CreateSettingsMenu(menu);
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

        RecordingRenderer.Start(frames);
        if (frames > 0) {
            VideoCapture.CurrentFrameCount = 0;
            VideoCapture.TargetFrameCount = frames;
        }

        if (Encoder.HasAudio) AudioCapture.StartRecording();

        Logger.Log(LogLevel.Info, NAME, "Started recording!");
    }
    internal static void StopRecording() {
        _recording = false;

        if (Encoder.HasAudio) AudioCapture.StopRecording();

        _encoder.End();
        _encoder = null;

        Logger.Log(LogLevel.Info, NAME, "Stopped recording!");
    }

    [Command("start_recording", "")] [SuppressMessage("Microsoft.CodeAnalysis", "IDE0051")]
    private static void CmdStartRecording(int frames = -1) {
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

    [Command("stop_recording", "")] [SuppressMessage("Microsoft.CodeAnalysis", "IDE0051")]
    private static void CmdStopRecording() {
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
}
