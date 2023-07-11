using System;
using Monocle;

namespace Celeste.Mod.Capture;

public class CaptureModule : EverestModule {
    public static CaptureModule Instance { get; private set; }

    public override Type SettingsType => typeof(CaptureModuleSettings);
    public static CaptureModuleSettings Settings => (CaptureModuleSettings) Instance._Settings;

    public override Type SessionType => typeof(CaptureModuleSession);
    public static CaptureModuleSession Session => (CaptureModuleSession) Instance._Session;

    // Might be recording outside of a session
    private static bool _recording = false;
    public static bool Recording { get => _recording; }

    public CaptureModule() {
        Instance = this;
    }

    public override void Load() {
        Syncing.Load();
        AudioCapture.Load();
    }
    public override void Unload() {
        if (Recording) {
            StopRecording();
        }

        Syncing.Unload();
        AudioCapture.Unload();
    }

    public static void StartRecording() {
        _recording = true;
        AudioCapture.StartRecording();
    }
    public static void StopRecording() {
        AudioCapture.StopRecording();
        _recording = false;
    }
    
    [Command("start_rec", "aa")]
    private static void CmdStartRec() {
        Console.WriteLine("hi");
        StartRecording();
    }
}