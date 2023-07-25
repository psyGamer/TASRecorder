using System;
using Microsoft.Xna.Framework;
using FMOD.Studio;
using Monocle;

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

    public static void StartRecording() {
        _encoder = new Encoder();
        _recording = true;

        if (!Encoder.HasVideo && !Encoder.HasAudio) {
            _recording = false;
            _encoder = null;
            return;
        }

        if (Encoder.HasAudio) AudioCapture.StartRecording();

        Logger.Log(LogLevel.Info, NAME, "Started recording!");
    }
    public static void StopRecording() {
        _recording = false;

        if (Encoder.HasAudio) AudioCapture.StopRecording();

        _encoder.End();
        _encoder = null;

        Logger.Log(LogLevel.Info, NAME, "Stopped recording!");
    }

    [Command("start_recording", "")]
    private static void CmdStartRecording() {
        try {
            StartRecording();
            Engine.Commands.Log("Successfully started recording.", Color.LightBlue);
        } catch (Exception ex) {
            Engine.Commands.Log("An unexpeced error occured while trying to start the recording.", Color.Red);
            Engine.Commands.LogStackTrace(ex.StackTrace.ToString());
            Logger.Log(LogLevel.Error, NAME, "Failed to start recording!");
            Logger.LogDetailed(ex, NAME);
        }
    }

    [Command("stop_recording", "")]
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
