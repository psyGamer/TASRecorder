using System;
using System.Runtime.InteropServices;

namespace Celeste.Mod.TASRecorder.Util;

public enum OS {
    Windows, MacOS, Linux
}

public static class OSUtil {
    public static OS Current => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? OS.Windows :
                                RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? OS.MacOS :
                                RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? OS.Linux :
                                throw new PlatformNotSupportedException("Your operating system is not supported by TAS Recorder");
}
