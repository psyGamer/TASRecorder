using System.Runtime.InteropServices;

namespace Celeste.Mod.TASRecorder;

public class NullEncoder : Encoder {
    public NullEncoder(string? fileName = null) : base(fileName) {
        HasVideo = true;
        HasAudio = true;

        unsafe {
            VideoData = (byte*) NativeMemory.AllocZeroed(1920*1080*4*2);
            AudioData = (byte*) NativeMemory.AllocZeroed(1024*4*2);
        }
    }

    public override void End() {
        unsafe {
            NativeMemory.Free(VideoData);
            NativeMemory.Free(AudioData);
        }

        RecordingManager.MarkEncoderFinished();
    }

    public override void PrepareVideo(int width, int height) {
    }

    public override void PrepareAudio(uint channelCount, uint sampleCount) {
    }

    public override void FinishVideo() {
    }

    public override void FinishAudio() {
    }
}
