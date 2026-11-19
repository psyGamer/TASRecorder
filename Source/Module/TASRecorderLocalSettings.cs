using System.IO;

namespace Celeste.Mod.TASRecorder;

/// These settings are device-specific and NOT synced between PCs via Steam Cloud.
public class TASRecorderLocalSettings {
    public string OutputDirectory { get; set; } = Path.Combine(Everest.PathGame, "TAS-Recordings");
}
