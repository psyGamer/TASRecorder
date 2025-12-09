using Celeste.Mod.TASRecorder.Util;
using System;
using System.Threading;

namespace Celeste.Mod.TASRecorder;

internal static class Syncing {

    private static readonly Barrier barrier = new(participantCount: 2);
    private static readonly TimeSpan timeout = TimeSpan.FromSeconds(10.0f);

    // Spin lock until the next frame
    public static void SyncWithAudio() {
        if (!RecordingManager.RecordingAudio) return;

        if (!barrier.SignalAndWait(timeout)) {
            Log.Warn("Timed-out while waiting for audio to complete");
        }
    }

    public static void SyncWithVideo() {
        if (!RecordingManager.RecordingVideo) return;

        if (!barrier.SignalAndWait(timeout)) {
            Log.Warn("Timed-out while waiting for video to complete");
        }
    }
}
