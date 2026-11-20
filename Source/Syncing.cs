using Celeste.Mod.TASRecorder.Util;
using System;
using System.Threading;

namespace Celeste.Mod.TASRecorder;

internal static class Syncing {

    public static readonly Barrier barrier = new(participantCount: 2);
    private static readonly TimeSpan timeout = TimeSpan.FromSeconds(10.0f);

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

    public static void ClearBarrier() {
        // Only trigger signal if something is currently waiting
        if (barrier.ParticipantsRemaining < 2) {
            barrier.SignalAndWait();
        }
    }
}
