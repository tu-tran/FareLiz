// Copyright (c) Microsoft Corporation.  All rights reserved.
namespace SkyDean.FareLiz.WinForm.Components.Utils
{
    using System;

    /// <summary>The primary coordinator of the Windows 7 taskbar-related activities.</summary>
    public static class Windows7Taskbar
    {
        /// <summary>
        /// The _taskbar list.
        /// </summary>
        private static ITaskbarList3 _taskbarList;

        /// <summary>
        /// The os info.
        /// </summary>
        private static readonly OperatingSystem osInfo = Environment.OSVersion;

        /// <summary>
        /// The windows 7 or greater.
        /// </summary>
        internal static readonly bool Windows7OrGreater = (osInfo.Version.Major == 6 && osInfo.Version.Minor >= 1) || (osInfo.Version.Major > 6);

        /// <summary>
        /// Gets the taskbar list.
        /// </summary>
        internal static ITaskbarList3 TaskbarList
        {
            get
            {
                if (Windows7OrGreater && _taskbarList == null)
                {
                    lock (typeof(Windows7Taskbar))
                    {
                        if (_taskbarList == null)
                        {
                            _taskbarList = (ITaskbarList3)new CTaskbarList();
                            _taskbarList.HrInit();
                        }
                    }
                }

                return _taskbarList;
            }
        }

        /// <summary>
        /// Sets the progress RequestState of the specified window's taskbar button.
        /// </summary>
        /// <param name="hwnd">
        /// The window handle.
        /// </param>
        /// <param name="state">
        /// The progress RequestState.
        /// </param>
        public static void SetProgressState(IntPtr hwnd, ThumbnailProgressState state)
        {
            if (Windows7OrGreater && TaskbarList != null)
            {
                TaskbarList.SetProgressState(hwnd, state);
            }
        }

        /// <summary>
        /// Sets the progress value of the specified window's taskbar button.
        /// </summary>
        /// <param name="hwnd">
        /// The window handle.
        /// </param>
        /// <param name="current">
        /// The current value.
        /// </param>
        /// <param name="maximum">
        /// The maximum value.
        /// </param>
        public static void SetProgressValue(IntPtr hwnd, ulong current, ulong maximum)
        {
            if (Windows7OrGreater && TaskbarList != null)
            {
                TaskbarList.SetProgressValue(hwnd, current, maximum);
            }
        }
    }
}