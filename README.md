<h1 align="center">TASRecorder</h1>
<h3 align="center">Create frame-perfect TAS recordings</h3>

A convenient way for you to record a TAS. If you've used [.kkapture](https://github.com/DemoJameson/kkapture) before, or struggled to get it working, this tool is perfect for you!

And if you've used [ldcapture](https://github.com/psyGamer/ldcapture), this might feel familiar. That's because this is more or less a direct port of ldcapture, however in an easier to use and more cross-platform way.

# Installation

Just install it with [Olympus](https://github.com/EverestAPI/Olympus) or [mons](https://github.com/coloursofnoise/mons). Alternatively you can download the .zip directly from [Gamebanana](https://gamebanana.com/mods/53697) or from the [Github Releases](https://github.com/CommunalHelper/CommunalHelper/releases/).

# Requirements

- Atleast Everest 4099 (this is the `core` branch)
- FFmpeg libraries, such as libavcodec, libavformat, etc.
    - Windows: In-game, open the debug console and enter `ffmpeg_install`. If this was successful, you can restart your game and have everything setup.
    - MacOS: `brew install ffmpeg` (Requires [Homebrew](https://brew.sh/))
    - Ubuntu/Debian based: `sudo apt-get install ffmpeg` (Might not include all codec you want. You'll need to look for them yourself)
    - Fedora based: `sudo dnf install ffmpeg-free` (Might not include all codec you want. You'll need to look for them yourself)
    - Arch Linux based: `sudo pacman -S ffmpeg`
    - Other: You'll need to install the libraries on you own. However, if you aren't using something from above, you probably know what you're doing
- (Optional, but very recommended) CelesteTAS for easier recording

You can use the debug console command `ffmpeg_check` to verify that you've successfully installed the FFmpeg libraries.

# Usage

All recordings will be saved to `TAS-Recordings` in your Celeste install folder.

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
The quick restart is used to restart the music. This can be replaced with console load if you don't care about that.
`1000` after the `ChapterTime` is in this case for the fade-out of a CollabUtils Mini Heart. The recording will finish on level exit.

## Recording Segments

If you just want to record certain segments of a TAS, you can use the `StartRecording` and `StopRecording` commands. Everything between those commands will get recorded. The file name is the current date and time.

You can also use `StartRecording <frames>` if you want to record for a certain amount of frames.

## Playback Speed

TASes can be recorded at faster speeds by using `Set, TASRecorder.Speed, x`, where x is the speed (ex: 2.0 would playback at twice the speed). You can also mute sound effects with `Set, TASRecorder.MuteSFX, true`, since those can be unpleasant at high speeds.

## Recording RTA

This is not really recommended, since the frame rate is not locked to 60FPS, but rather the speed at which you PC can record all frames.

This is however capped at 60FPS, unless you are recording without audio, in which case, doesn't have a frame rate cap.

The debug console commands work basically the same as the TAS commands, but are available outside of TASes: `start_recording` and `stop_recording`.
`start_recording` accepts a frame count as well.

# Contact

If you have feedback/bug reports/questions, you can ask me on the [Celeste Discord](https://discord.gg/celeste) under `psygamer`

# Credits

- [.kkapture](https://github.com/DemoJameson/kkapture) for being the initial inspiration for this
- [ldcapture](https://github.com/psyGamer/ldcapture) my port of .kkapture, which this is based on
- [CelesteTAS](https://github.com/EverestAPI/CelesteTAS-EverestInterop) for providing easy and convenient TASing tools
