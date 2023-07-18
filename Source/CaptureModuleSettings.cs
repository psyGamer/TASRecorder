namespace Celeste.Mod.Capture;

public class CaptureModuleSettings : EverestModuleSettings {

    public int FPS = 60;

    public int VideoWidth = 1920;
    public int VideoHeight = 1080;
    public int VideoBitrate = 6500000;

    public int AudioBitrate = 128000;

    public string VideoCodec = null;
    public string AudioCodec = null;

    public string ContainerType = "mp4";
}
