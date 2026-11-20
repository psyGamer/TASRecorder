using System;
using System.Runtime.InteropServices;

namespace Celeste.Mod.TASRecorder.Util;

internal enum OS {
    Windows, Linux, MacOS
}

internal static class OSUtil {
    public static OS Current => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? OS.Windows :
                                RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? OS.Linux :
                                RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? OS.MacOS :
                                throw new PlatformNotSupportedException("Your operating system is not supported by TAS Recorder");
}
