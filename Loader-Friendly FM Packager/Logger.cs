﻿using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Loader_Friendly_FM_Packager;

public static class Logger
{
    private static readonly object _lock = new();

    private static string _logFile = "";
    // ReSharper disable once InconsistentlySynchronizedField
    public static void SetLogFile(string logFile) => _logFile = logFile;

    #region Interop

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [StructLayout(LayoutKind.Sequential)]
    private struct SYSTEMTIME
    {
        internal ushort wYear;
        internal ushort wMonth;
        internal ushort wDayOfWeek;
        internal ushort wDay;
        internal ushort wHour;
        internal ushort wMinute;
        internal ushort wSecond;
        internal ushort wMilliseconds;
    }

    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern void GetLocalTime(ref SYSTEMTIME systemTime);

    // For logging purposes: It takes an entire 5ms to get one DateTime.Now, but I don't really need hardcore
    // accuracy in logging dates, they're really just there for vague temporality. Because we log on startup,
    // this claws back some startup time.
    private static string GetDateTimeStringFast()
    {
        var dt = new SYSTEMTIME();
        GetLocalTime(ref dt);
        return dt.wYear + "/" + dt.wMonth + "/" + dt.wDay + " " +
               dt.wHour + ":" + dt.wMinute + ":" + dt.wSecond;
    }

    #endregion

    private static void ClearLogFile()
    {
        lock (_lock)
        {
            if (_logFile.IsEmpty()) return;

            try
            {
                File.Delete(_logFile);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }

    /// <summary>
    /// A faster version without locking or unnecessary options for running on startup.
    /// </summary>
    /// <param name="message"></param>
    [SuppressMessage("ReSharper", "InconsistentlySynchronizedField")]
    public static void LogStartup(string message)
    {
        lock (_lock)
        {
            if (_logFile.IsEmpty()) return;

            try
            {
                try
                {
                    if (new FileInfo(_logFile).Length > ByteSize.MB * 50) ClearLogFile();
                }
                catch
                {
                    // file doesn't exist - ignore
                }

                using var sw = new StreamWriter(_logFile);
                sw.WriteLine(GetDateTimeStringFast() + " " + message + $"{NL}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }

    public static void Log(
        string message = "",
        Exception? ex = null,
        bool stackTrace = false,
        [CallerMemberName] string callerMemberName = "")
    {
        lock (_lock)
        {
            if (_logFile.IsEmpty()) return;

            try
            {
                using var sw = new StreamWriter(_logFile, append: true);

                sw.WriteLine(GetDateTimeStringFast() + " " + callerMemberName + $"{NL}" + message);
                if (ex != null) sw.WriteLine($"EXCEPTION:{NL}" + ex);
                if (stackTrace) sw.WriteLine($"STACK TRACE:{NL}" + new StackTrace(1));
                sw.WriteLine();
            }
            catch (Exception logEx)
            {
                Debug.WriteLine(logEx);
            }
        }
    }
}
