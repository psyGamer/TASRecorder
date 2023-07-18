using System;
using System.Threading;
using Microsoft.Xna.Framework;
using MonoMod;
using MonoMod.RuntimeDetour;
using FMOD;
using System.IO;
using System.Runtime.InteropServices;

namespace Celeste.Mod.Capture;

public static class AudioCapture {

    internal static void Load() {
        On.Celeste.Audio.Init += on_Audio_Init;
        On.Celeste.Audio.Unload += on_Audio_Unload;
    }
    internal static void Unload() {
        On.Celeste.Audio.Init -= on_Audio_Init;
        On.Celeste.Audio.Unload -= on_Audio_Unload;
    }

    internal static void StartRecording() {
        runThread = true;
        threadHandle = new Thread(captureThread);
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
    private static int targetRecordedSamples = 0;
    // Actuall amount of samples which were recorded
    private static int recordedSamples = 0;

    private static byte[] inBufferArray = new byte[1024]; // 1024 is the expected sample count
    private static GCHandle inBufferArrayHandle = GCHandle.Alloc(inBufferArray, GCHandleType.Pinned);

    private static unsafe RESULT captureCallback(ref DSP_STATE dspState, IntPtr inBuffer, IntPtr outBuffer, uint samples, int inChannels, ref int outChannels) {
        const int sampleSizeBytes = 4; // Size of a float

        Buffer.MemoryCopy(
            inBuffer.ToPointer(),
            outBuffer.ToPointer(),
            samples * inChannels * sampleSizeBytes,
            samples * outChannels * sampleSizeBytes
        );

        // Block until the management thread allows capturing more (also blocks the main FMOD thread which is good, since it avoid artifacts)
        while (CaptureModule.Recording && !allowCapture) { }
        if (!CaptureModule.Recording) return RESULT.OK; // Recording ended during the wait

        CaptureModule.Encoder.PrepareAudio((uint)inChannels, samples);
        NativeMemory.Copy((void*)inBuffer, CaptureModule.Encoder.AudioData, (nuint)(inChannels * samples * Marshal.SizeOf<float>()));
        CaptureModule.Encoder.FinishAudio();

        recordedSamples += (int)samples;

        return RESULT.OK;
    }

    private static void captureThread() {
        totalRecodedSamplesError = 0;
        // Celeste has a sample rate of 48000 samples/second
        targetRecordedSamples = Encoder.AUDIO_SAMPLE_RATE / CaptureModule.Settings.FPS;

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
    private static void on_Audio_Init(On.Celeste.Audio.orig_Init orig) {
        orig();

        var desc = default(DSP_DESCRIPTION);
        desc.version          = 0x00010000;
        desc.numinputbuffers  = 1;
        desc.numoutputbuffers = 1;
        desc.read             = captureCallback;

        Audio.system.getLowLevelSystem(out lowLevelSystem);
        lowLevelSystem.getMasterChannelGroup(out masterChannelGroup);
        lowLevelSystem.createDSP(ref desc, out dsp);
        masterChannelGroup.addDSP(0, dsp);
    }

    // Clean up the DSP
    private static void on_Audio_Unload(On.Celeste.Audio.orig_Unload orig) {
        if (CaptureModule.Recording) {
            CaptureModule.StopRecording();
        }

        masterChannelGroup.removeDSP(dsp);
        dsp.release();

        orig();
    }
}