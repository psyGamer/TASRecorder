using System;
using System.Threading;
using Microsoft.Xna.Framework;
using MonoMod;
using MonoMod.RuntimeDetour;
using FMOD;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;

namespace Celeste.Mod.TASRecorder;

public static class AudioCapture {

    internal static void Load() {
        On.Celeste.Audio.Init += On_Audio_Init;
        On.Celeste.Audio.Unload += On_Audio_Unload;
    }
    internal static void Unload() {
        On.Celeste.Audio.Init -= On_Audio_Init;
        On.Celeste.Audio.Unload -= On_Audio_Unload;
    }

    internal static void StartRecording() {
        runThread = true;
        threadHandle = new Thread(CaptureThread);
        threadHandle.Start();
    }
    internal static void StopRecording() {
        runThread = false;
        threadHandle.Join();
        threadHandle = null;
    }

    private static Thread threadHandle;
    private static bool runThread;

    private static FMOD.System lowLevelSystem;
    private static ChannelGroup masterChannelGroup;
    private static DSP dsp;

    // Manages weather the DSP is allowed to record. Blocks the FMOD thread otherwise.
    private static bool allowCapture = false;
    // The accumulated overhead, since audio chunks contain more data than 1 frame.
    private static int totalRecodedSamplesError = 0;
    // Theoretical perfect amount of samples per frame
    internal static int targetRecordedSamples = 0;
    // Actuall amount of samples which were recorded
    private static int recordedSamples = 0;
    // Amount of callback-batches to ignore, to avoid leaking of previous audio. Should be kept low.
    private static int batchesToIgnore = 5;

    private static unsafe RESULT CaptureCallback(ref DSP_STATE dspState, IntPtr inBuffer, IntPtr outBuffer, uint samples, int inChannels, ref int outChannels) {
        const int sampleSizeBytes = 4; // Size of a float

        Buffer.MemoryCopy(
            inBuffer.ToPointer(),
            outBuffer.ToPointer(),
            samples * inChannels * sampleSizeBytes,
            samples * outChannels * sampleSizeBytes
        );

        // Block until the management thread allows capturing more (also blocks the main FMOD thread which is good, since it avoid artifacts)
        while (TASRecorderModule.Recording && !allowCapture) { }
        if (!TASRecorderModule.Recording) return RESULT.OK; // Recording ended during the wait

        TASRecorderModule.Encoder.PrepareAudio((uint)inChannels, samples);
        if (batchesToIgnore > 0) {
            float* dst = (float*) TASRecorderModule.Encoder.AudioData;
            for (int i = 0; i < inChannels * samples; i++) {
                dst[i] = 0.0f;
            }
            batchesToIgnore--;
        } else {
            NativeMemory.Copy((void*)inBuffer, TASRecorderModule.Encoder.AudioData, (nuint)(inChannels * samples * Marshal.SizeOf<float>()));
        }
        TASRecorderModule.Encoder.FinishAudio();

        recordedSamples += (int)samples;

        return RESULT.OK;
    }

    private static void CaptureThread() {
        totalRecodedSamplesError = 0;
        // Celeste has a sample rate of 48000 samples/second
        targetRecordedSamples = Encoder.AUDIO_SAMPLE_RATE / TASRecorderModule.Settings.FPS;
        batchesToIgnore = 5;

        while (runThread) {
            Syncing.SyncWithVideo();
            if (!runThread) return; // While syncing, the recording might stop

            // Skip a frame to let the video catch up again
            if (totalRecodedSamplesError >= targetRecordedSamples) {
                totalRecodedSamplesError -= targetRecordedSamples;
                continue;
            }

            // Capture atleast a frame of data on the FMOD/DSP thread
            allowCapture = true;
            while (runThread && recordedSamples < targetRecordedSamples);
            allowCapture = false;

            // Accumulate the data overhead
            totalRecodedSamplesError += recordedSamples - targetRecordedSamples;
            recordedSamples = 0;
        }
    }

    // Create the DSP for capturing the audio data
    private static void On_Audio_Init(On.Celeste.Audio.orig_Init orig) {
        orig();

        var desc = default(DSP_DESCRIPTION);
        desc.version          = 0x00010000;
        desc.numinputbuffers  = 1;
        desc.numoutputbuffers = 1;
        desc.read             = CaptureCallback;

        Audio.system.getLowLevelSystem(out lowLevelSystem);
        lowLevelSystem.getMasterChannelGroup(out masterChannelGroup);
        lowLevelSystem.createDSP(ref desc, out dsp);
        masterChannelGroup.addDSP(0, dsp);
    }

    // Clean up the DSP
    private static void On_Audio_Unload(On.Celeste.Audio.orig_Unload orig) {
        if (TASRecorderModule.Recording) {
            TASRecorderModule.StopRecording();
        }

        masterChannelGroup?.removeDSP(dsp);
        dsp?.release();

        orig();
    }
}
