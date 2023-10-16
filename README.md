<h1 align="center">TASRecorder</h1>
<h3 align="center">Create frame-perfect TAS recordings</h3>

A convenient way for you to record a TAS. If you've used [.kkapture](https://github.com/DemoJameson/kkapture) before, or struggled to get it working, this tool is perfect for you!

And if you've used [ldcapture](https://github.com/psyGamer/ldcapture), this might feel familiar. That's because this is more or less a direct port of ldcapture, however in an easier to use and more cross-platform way.

# Installation

Just install it with [Olympus](https://github.com/EverestAPI/Olympus) or [mons](https://github.com/coloursofnoise/mons). Alternatively you can download the .zip directly from [Gamebanana](https://gamebanana.com/tools/14085) or from the [Github Releases](https://github.com/psyGamer/TASRecorder/releases/latest).

# Requirements

- Atleast **Everest 4287** (this is the `core` branch)
- **FFmpeg libraries** (avutil, avformat, avcodec, swresample, swscale)
- (Optional, but very recommended) **CelesteTAS** for easier recording

**NOTE**: If you are on MacOS/Linux (x86-64), it is strongly recommended to install the FFmpeg libraries with your system's package-manager!

If you don't have the FFmpeg libraries installed, TAS Recorder will install them for you on startup.

You can use the debug console command `ffmpeg_check` to verify that the FFmpeg libraries are working correctly.
In case this fails, make sure to check your log, to find out what went wrong!

# Usage

All recordings will be saved to `TAS-Recordings` in your Celeste install folder.
This can be changed in the mod options.

## Recording entire levels

In Celeste Studio, you can go to `File` and then `Record TAS`.
Note that this will simply record all inputs and stop after that. You probably want to add additonal frames after the end, for a smooth ending. If the TAS ends, because it goes to the overworld, the recording will finish.

In case you can't use Celeste Studio, the same can be achived with the following:
```
#console load ...
#   1
   1,Q
  32,J
StartRecording

#Start
...
ChapterTime: ...
1000
```
The quick restart is used to restart the music (**NOTE**: The `Record TAS` button already does this). This can be replaced with the console load if you don't care about that.
`1000` after the `ChapterTime` is in this case for the fade-out of a CollabUtils Mini Heart. The recording will finish on level exit.

## Recording Segments

If you just want to record certain segments of a TAS, you can use the `StartRecording` and `StopRecording` commands. Everything between those commands will get recorded. The file name is the current date and time.

You can also use `StartRecording <frames>` if you want to record for a certain amount of frames.

## Speeding up certain segments

If there are any boring / uninteresting parts of your TAS, you can speed them up with the following command:
```
Set, TASRecorder.Speed, 2.0
```
This means that from that point on, the recording will play twice as fast. Any value (except 0) is technically supported. `1.0` is normal speed, `0.5` is half speed, `2.0` is twice the speed, and so on. Note that anything beyond `10.0` becomes quite difficult to watch so it's not really recommended.

While the video is sped up, audio stays at the normal speed. However with high speeds, sound effects can become quite loud and unpleasant to hear. Therefore it is recommended to mute sound effects during high speeds with the following command:
```
Set, TASRecorder.MuteSFX, true
```
This can be reverted by setting it to `false` again.

Both those settings will get reset once the recording has finished and must be set **after** the recording has already started.

## Recording RTA

This is not really recommended, since the frame rate is not locked to 60FPS, but rather the speed at which you PC can record all frames.

This is however capped at 60FPS, unless you are recording without audio, in which case, doesn't have a frame rate cap.

The debug console commands work basically the same as the TAS commands but are available outside of TASes: `start_recording` and `stop_recording`.
`start_recording` accepts a frame count as well.

# Contact

If you have feedback/bug reports/questions, you can ask me on the [Celeste Discord](https://discord.gg/celeste) under `psygamer`

# Credits

- [.kkapture](https://github.com/DemoJameson/kkapture) for being the initial inspiration for this
- [ldcapture](https://github.com/psyGamer/ldcapture) my port of .kkapture, which this is based on
- [CelesteTAS](https://github.com/EverestAPI/CelesteTAS-EverestInterop) for providing easy and convenient TASing tools
