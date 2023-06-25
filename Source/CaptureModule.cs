using System;

namespace Celeste.Mod.Capture;

public class CaptureModule : EverestModule {
    public static CaptureModule Instance { get; private set; }

    public override Type SettingsType => typeof(CaptureModuleSettings);
    public static CaptureModuleSettings Settings => (CaptureModuleSettings) Instance._Settings;

    public override Type SessionType => typeof(CaptureModuleSession);
    public static CaptureModuleSession Session => (CaptureModuleSession) Instance._Session;

    // Might be outside of a session
    public static bool Recording = false;

    public CaptureModule() {
        Instance = this;
    }

    public override void Load() {
        Timings.Load();
    }

    public override void Unload() {
        Timings.Unload();
    }
}