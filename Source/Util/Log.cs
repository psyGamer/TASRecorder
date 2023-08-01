using System;
using ModLogger = Celeste.Mod.Logger;

namespace Celeste.Mod.TASRecorder.Util;

public static class Log {
    public const string TAG = "TASRecorder";

    public static void Verbose(string message) => ModLogger.Log(LogLevel.Verbose, TAG, message);
    public static void Debug(string message) => ModLogger.Log(LogLevel.Debug, TAG, message);
    public static void Info(string message) => ModLogger.Log(LogLevel.Info, TAG, message);
    public static void Warn(string message) => ModLogger.Log(LogLevel.Warn, TAG, message);
    public static void Error(string message) => ModLogger.Log(LogLevel.Error, TAG, message);
    public static void Exception(Exception ex) => ModLogger.LogDetailed(ex, TAG);
}
