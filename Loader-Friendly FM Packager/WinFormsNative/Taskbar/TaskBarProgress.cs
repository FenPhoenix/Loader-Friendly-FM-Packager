﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Loader_Friendly_FM_Packager.WinFormsNative.Taskbar;

internal enum TaskbarStates
{
    NoProgress = 0,
    Indeterminate = 1,
    Normal = 2,
    Error = 4,
    Paused = 8,
}

internal static class TaskBarProgress
{
    [ComImport]
    [Guid("56fdf344-fd6d-11d0-958a-006097c9a090")]
    [ClassInterface(ClassInterfaceType.None)]
    private class TaskbarInstance;

    [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
    private static readonly ITaskbarList3 _instance = (ITaskbarList3)new TaskbarInstance();

    private static readonly bool _taskbarSupported = WinVersion.Is7OrAbove;

    internal static void SetState(IntPtr windowHandle, TaskbarStates taskbarState)
    {
        if (!_taskbarSupported || windowHandle == IntPtr.Zero) return;
        _instance.SetProgressState(windowHandle, taskbarState);
    }

    internal static void SetValue(IntPtr windowHandle, int progressValue, int progressMax)
    {
        if (!_taskbarSupported || windowHandle == IntPtr.Zero) return;
        _instance.SetProgressValue(windowHandle, (ulong)progressValue, (ulong)progressMax);
    }
}
