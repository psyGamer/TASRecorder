using System;
using Monocle;

namespace Celeste.Mod.Capture;

public class CaptureModule : EverestModule {

    public const string NAME = "Capture";

    public static CaptureModule Instance { get; private set; }

    public override Type SettingsType => typeof(CaptureModuleSettings);
    public static CaptureModuleSettings Settings => (CaptureModuleSettings) Instance._Settings;

    public override Type SessionType => typeof(CaptureModuleSession);
    public static CaptureModuleSession Session => (CaptureModuleSession) Instance._Session;

    // Might be recording outside of a session
    private static Encoder? _encoder = null;
    public static Encoder? Encoder { get => _encoder; }
    public static bool Recording { get => _encoder != null; }

    public CaptureModule() {
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

    public static void StartRecording() {
        _encoder = new Encoder();
        AudioCapture.StartRecording();
    }
    public static void StopRecording() {
        AudioCapture.StopRecording();
        _encoder.End();
        _encoder = null;
    }
    
    [Command("start_rec", "")]
    private static void CmdStartRec() {
        Console.WriteLine("Started recording");
        try {
            StartRecording();
        } catch (Exception ex) {
            Console.WriteLine(ex);
        }
    }

    [Command("stop_rec", "")]
    private static void CmdStopRec() {
        Console.WriteLine("Stopped recording");
        try {
            StopRecording();
        } catch (Exception ex) {
            Console.WriteLine(ex);
        }
    }
}