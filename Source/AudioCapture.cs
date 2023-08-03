using FMOD;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Celeste.Mod.TASRecorder;

public static class AudioCapture {
    private static Thread threadHandle;
    private static bool runThread;

    private static ChannelGroup masterChannelGroup;
    private static DSP dsp;

    // Theoretical perfect amount of samples per frame
    private static int TargetRecordedSamples => Encoder.AUDIO_SAMPLE_RATE / TASRecorderModule.Settings.FPS;

    // Manages weather the DSP is allowed to record. Blocks the FMOD thread otherwise.
    private static bool allowCapture = false;
    // The accumulated overhead, since audio chunks contain more data than 1 frame.
    private static int totalRecodedSamplesError = 0;
    // Actuall amount of samples which were recorded
    private static int recordedSamples = 0;
    // Amount of callback-batches to ignore, to avoid leaking of previous audio. Should be kept low.
    private static int batchesToIgnore = 5;

    internal static void Load() => Task.Run(() => {
        var desc = default(DSP_DESCRIPTION);
        desc.version = 0x00010000;
        desc.numinputbuffers = 1;
        desc.numoutputbuffers = 1;
        desc.read = CaptureCallback;

        // Wait until the Audio is loaded
        while (!Audio.AudioInitialized) {
            Task.Delay(100).Wait();
        }

        Audio.System.getLowLevelSystem(out var lowLevelSystem);
        lowLevelSystem.getMasterChannelGroup(out masterChannelGroup);
        lowLevelSystem.createDSP(ref desc, out dsp);
        masterChannelGroup.addDSP(0, dsp);
        dsp?.setBypass(true);
    });
    internal static void Unload() {
        masterChannelGroup?.removeDSP(dsp);
        masterChannelGroup = null;
        dsp?.release();
        dsp = null;
    }

    internal static void StartRecording() {
        dsp?.setBypass(false);
        runThread = true;
        threadHandle = new Thread(CaptureThread);
        threadHandle.Start();
    }
    internal static void StopRecording() {
        dsp?.setBypass(true);
        runThread = false;
        threadHandle.Join();
        threadHandle = null;
    }

    private static unsafe RESULT CaptureCallback(ref DSP_STATE dspState, IntPtr inBuffer, IntPtr outBuffer, uint samples, int inChannels, ref int outChannels) {
        float* src = (float*) inBuffer;
        float* dst = (float*) outBuffer;

        if (inChannels == outChannels) {
            NativeMemory.Copy(src, dst, (nuint) (inChannels * samples * Marshal.SizeOf<float>()));
        } else if (inChannels > outChannels) {
            // Cut the remaining channels off
            for (int sample = 0; sample < samples; sample++) {
                NativeMemory.Copy(src + sample * inChannels, dst + sample * outChannels, (nuint) (outChannels * Marshal.SizeOf<float>()));
            }
        } else {
            // Repeat the last channel to fill the remaining ones
            for (int sample = 0; sample < samples; sample++) {
                NativeMemory.Copy(src + sample * inChannels, dst + sample * outChannels, (nuint) (inChannels * Marshal.SizeOf<float>()));
                for (int channel = inChannels; channel < outChannels; channel++) {
                    dst[sample * outChannels + channel] = src[sample * inChannels + inChannels - 1];
                }
            }
        }

        // Block until the management thread allows capturing more (also blocks the main FMOD thread which is good, since it avoid artifacts)
        while (TASRecorderModule.Recording && !allowCapture) { }
        if (!TASRecorderModule.Recording) return RESULT.OK; // Recording ended during the wait

        TASRecorderModule.Encoder.PrepareAudio((uint) inChannels, samples);
        if (batchesToIgnore > 0) {
            float* encoderDst = (float*) TASRecorderModule.Encoder.AudioData;
            for (int i = 0; i < inChannels * samples; i++) {
                encoderDst[i] = 0.0f;
            }
            batchesToIgnore--;
        } else {
            NativeMemory.Copy(src, TASRecorderModule.Encoder.AudioData, (nuint) (inChannels * samples * Marshal.SizeOf<float>()));
        }
        TASRecorderModule.Encoder.FinishAudio();

        recordedSamples += (int) samples;

        return RESULT.OK;
    }

    private static void CaptureThread() {
        totalRecodedSamplesError = 0;
        batchesToIgnore = 5;

        while (runThread) {
            Syncing.SyncWithVideo();
            if (!runThread) return; // While syncing, the recording might stop
            // Copy to local variable, to avoid it changing while capturing
            int targetRecordedSamples = TargetRecordedSamples;

            // Skip a frame to let the video catch up again
            if (totalRecodedSamplesError >= targetRecordedSamples) {
                totalRecodedSamplesError -= targetRecordedSamples;
                continue;
            }

            // Capture atleast a frame of data on the FMOD/DSP thread
            allowCapture = true;
            while (runThread && recordedSamples < targetRecordedSamples) { }
            allowCapture = false;

            // Accumulate the data overhead
            totalRecodedSamplesError += recordedSamples - targetRecordedSamples;
            recordedSamples = 0;
        }
    }
}
