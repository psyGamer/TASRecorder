using System;
using System.Threading;
using Microsoft.Xna.Framework;
using MonoMod;
using MonoMod.RuntimeDetour;
using FMOD;

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
    private static uint totalRecodedSamplesError = 0;
    // Theoretical perfect amount of samples per frame
    private static uint targetRecordedSamples = 0;
    // Actuall amount of samples which were recorded
    private static uint recordedSamples = 0;

    private static unsafe RESULT captureCallback(ref DSP_STATE dspState, nint inBuffer, nint outBuffer, uint length, int inChannels, ref int outChannels) {
        if (inChannels == outChannels) {
            Buffer.MemoryCopy(((void*)outBuffer), ((void*)inBuffer), outChannels * length * sizeof(float), inChannels * length * sizeof(float));
        } else if (inChannels > outChannels) {
            // Cut the remaining channels off
            for (int sample = 0; sample < length; sample++) {
                // Would Buffer.MemoryCopy be faster here?
                for (int channel = 0; channel < outChannels; channel++) {
                    ((float*)outBuffer)[sample * outChannels + channel] = ((float*)inBuffer)[sample * inChannels + channel];
                }
            }
        } else {
            // Repeat the last channel to fill the remaining ones
            for (int sample = 0; sample < length; sample++) {
                // Would Buffer.MemoryCopy be faster here?
                int channel = 0;
                for (; channel < inChannels; channel++) {
                    ((float*)outBuffer)[sample * outChannels + channel] = ((float*)inBuffer)[sample * inChannels + channel];
                }
                for (; channel < outChannels; channel++) {
                    ((float*)outBuffer)[sample * outChannels + channel] = ((float*)inBuffer)[sample * inChannels + inChannels - 1];
                }
            }
        }

        // Block until the management thread allows capturing more (also blocks the main FMOD thread which is good, since it avoid artifacts)
        while (CaptureModule.Recording && !allowCapture) {}

        if (!CaptureModule.Recording)
            // Recording ended during the wait
            return RESULT.OK;

        // encoder_t* encoder = encoder_get_current();
        // encoder_prepare_audio(encoder, inChannels, length, ENCODER_AUDIO_FORMAT_PCM_F32);
        // memcpy(encoder->audio_data, inBuffer, inChannels * length * sizeof(f32));
        // encoder_flush_audio(encoder);

        Console.WriteLine($"Captured {length}");
        recordedSamples += length;

        return RESULT.OK;
    }

    private static void captureThread() {
        // Celeste has a sample rate of 48000 samples/second
        targetRecordedSamples = 48000u / CaptureModule.Settings.FPS;

        // This thread gets stopped once the recording ends.
        while (runThread) {
            Syncing.SyncWithVideo();

            // Skip a frame to let the video catch up again
            if (totalRecodedSamplesError >= targetRecordedSamples) {
                totalRecodedSamplesError -= targetRecordedSamples;
                continue;
            }

            // Capture atleast a frame of data on the FMOD/DSP thread
            recordedSamples = 0;
            allowCapture = true;
            while (recordedSamples < targetRecordedSamples);
            allowCapture = false;

            // Accumulate the data overhead
            totalRecodedSamplesError += recordedSamples - targetRecordedSamples;
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