namespace SkyDean.FareLiz.Service.Utils
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    using SkyDean.FareLiz.Service.Utils.WinNative;

    /// <summary>The process utils.</summary>
    public static class ProcessUtils
    {
        #region Process functions

        /// <summary>Get the name of current running process</summary>
        public static string CurrentProcessName
        {
            get
            {
                return Process.GetCurrentProcess().ProcessName;
            }
        }

        /// <summary>Gets the current process directory.</summary>
        public static string CurrentProcessDirectory
        {
            get
            {
                return Path.GetDirectoryName(CurrentProcessLocation);
            }
        }

        /// <summary>Gets the current process location.</summary>
        public static string CurrentProcessLocation
        {
            get
            {
                return Process.GetCurrentProcess().MainModule.FileName;
            }
        }

        /// <summary>
        /// Kill all instance of specific process
        /// </summary>
        /// <param name="processExecutable">
        /// Process name (or the full path to the executable file). If the full path is specified, only processes which are
        /// started at that location are terminated
        /// </param>
        /// <param name="attempts">
        /// The attempts.
        /// </param>
        /// <param name="intervalInSeconds">
        /// The interval In Seconds.
        /// </param>
        public static void KillProcess(string processExecutable, int attempts, int intervalInSeconds)
        {
            string processName = Path.GetFileName(processExecutable.TrimEnd(".exe".ToCharArray())), 
                   processLocation = Path.GetDirectoryName(processExecutable);

            var processToKill = Process.GetProcessesByName(processName);

            var forceKill = false;
            foreach (var process in processToKill)
            {
                try
                {
                    if (!string.IsNullOrEmpty(processLocation))
                    {
                        // Check if the current process is started in the specified location
                        var procLocation = GetExecutablePath(process.MainWindowHandle);
                        if (string.IsNullOrEmpty(procLocation))
                        {
                            continue;
                        }

                        var procDir = Path.GetDirectoryName(procLocation);
                        if (!string.Equals(procDir, processLocation, StringComparison.OrdinalIgnoreCase))
                        {
                            // If it was not started in the expected location
                            continue;
                        }
                    }

                    if (process.MainWindowHandle != IntPtr.Zero)
                    {
                        process.WaitForInputIdle(intervalInSeconds);
                        process.CloseMainWindow(); // Close the main window first
                    }

                    var killAttempts = 0;
                    while (!process.HasExited && killAttempts++ < attempts)
                    {
                        process.Kill(); // Forcefully close it
                        process.WaitForExit(intervalInSeconds * 1000);
                        forceKill = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to kill process: " + processExecutable + Environment.NewLine + ex.Message + ex.StackTrace);
                }

                // Ignore
            }

            if (forceKill)
            {
                RefreshTaskbarNotificationArea(); // Cleanup any tray icon
            }
        }

        /// <summary>
        /// The get executable path.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetExecutablePath(IntPtr hwnd)
        {
            uint pathBufferSize = 512; // plenty big enough
            var pathBufferSb = new StringBuilder((int)pathBufferSize);

            if (hwnd == IntPtr.Zero)
            {
                return string.Empty;
            }

            // not a valid window handle

            // Get the process id
            uint processid;
            NativeMethods.GetWindowThreadProcessId(hwnd, out processid);

            // Try the GetModuleFileName method first since it's the fastest. 
            // May return ACCESS_DENIED (due to VM_READ flag) if the process is not owned by the current user.
            // Will fail if we are compiled as x86 and we're trying to open a 64 bit process...not allowed.
            var hprocess = NativeMethods.OpenProcess(ProcessAccessFlags.QueryInformation | ProcessAccessFlags.Read, false, processid);
            if (hprocess != IntPtr.Zero)
            {
                try
                {
                    if (NativeMethods.GetModuleFileNameEx(hprocess, IntPtr.Zero, pathBufferSb, pathBufferSize) > 0)
                    {
                        return pathBufferSb.ToString();
                    }
                }
                finally
                {
                    NativeMethods.CloseHandle(hprocess);
                }
            }

            hprocess = NativeMethods.OpenProcess(ProcessAccessFlags.QueryInformation, false, processid);
            if (hprocess != IntPtr.Zero)
            {
                try
                {
                    // Try this method for Vista or higher operating systems
                    var size = pathBufferSize;
                    if ((Environment.OSVersion.Version.Major >= 6)
                        && (NativeMethods.QueryFullProcessImageName(hprocess, 0, pathBufferSb, ref size) && (size > 0)))
                    {
                        return pathBufferSb.ToString();
                    }

                    // Try the GetProcessImageFileName method
                    if (NativeMethods.GetProcessImageFileName(hprocess, pathBufferSb, pathBufferSize) > 0)
                    {
                        var dospath = pathBufferSb.ToString();
                        foreach (var drive in Environment.GetLogicalDrives())
                        {
                            if (NativeMethods.QueryDosDevice(drive.TrimEnd('\\'), pathBufferSb, pathBufferSize) > 0)
                            {
                                if (dospath.StartsWith(pathBufferSb.ToString()))
                                {
                                    return drive + dospath.Remove(0, pathBufferSb.Length);
                                }
                            }
                        }
                    }
                }
                finally
                {
                    NativeMethods.CloseHandle(hprocess);
                }
            }

            return string.Empty;
        }

        /// <summary>Refresh the tray</summary>
        public static void RefreshTaskbarNotificationArea()
        {
            var systemTrayContainerHandle = NativeMethods.FindWindow("Shell_TrayWnd", null);
            var systemTrayHandle = NativeMethods.FindWindowEx(systemTrayContainerHandle, IntPtr.Zero, "TrayNotifyWnd", null);
            var sysPagerHandle = NativeMethods.FindWindowEx(systemTrayHandle, IntPtr.Zero, "SysPager", null);
            var notificationAreaHandle = NativeMethods.FindWindowEx(sysPagerHandle, IntPtr.Zero, "ToolbarWindow32", "Notification Area");
            if (notificationAreaHandle == IntPtr.Zero)
            {
                notificationAreaHandle = NativeMethods.FindWindowEx(sysPagerHandle, IntPtr.Zero, "ToolbarWindow32", "User Promoted Notification Area");
                var notifyIconOverflowWindowHandle = NativeMethods.FindWindow("NotifyIconOverflowWindow", null);
                var overflowNotificationAreaHandle = NativeMethods.FindWindowEx(
                    notifyIconOverflowWindowHandle, 
                    IntPtr.Zero, 
                    "ToolbarWindow32", 
                    "Overflow Notification Area");
                RefreshTaskbarNotificationArea(overflowNotificationAreaHandle);
            }

            RefreshTaskbarNotificationArea(notificationAreaHandle);
        }

        /// <summary>
        /// The refresh taskbar notification area.
        /// </summary>
        /// <param name="windowHandle">
        /// The window handle.
        /// </param>
        public static void RefreshTaskbarNotificationArea(IntPtr windowHandle)
        {
            const uint wmMousemove = 0x0200;
            RECT rect;
            NativeMethods.GetClientRect(windowHandle, out rect);
            for (var x = 0; x < rect.right; x += 5)
            {
                for (var y = 0; y < rect.bottom; y += 5)
                {
                    NativeMethods.SendMessage(windowHandle, wmMousemove, 0, (y << 16) + x);
                }
            }
        }

        /*  // GET PARENT PROCESS -- NOT NEEDED YET
         /// <summary>
        /// A utility class to determine a process parent.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ParentProcessUtilities
        {
            // These members must match PROCESS_BASIC_INFORMATION
            internal IntPtr Reserved1;
            internal IntPtr PebBaseAddress;
            internal IntPtr Reserved2_0;
            internal IntPtr Reserved2_1;
            internal IntPtr UniqueProcessId;
            internal IntPtr InheritedFromUniqueProcessId;

            [DllImport("ntdll.dll")]
            private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref ParentProcessUtilities processInformation, int processInformationLength, out int returnLength);

            /// <summary>
            /// Gets the parent process of the current process.
            /// </summary>
            /// <returns>An instance of the Process class.</returns>
            public static Process GetParentProcess()
            {
                return GetParentProcess(Process.GetCurrentProcess().Handle);
            }

            /// <summary>
            /// Gets the parent process of specified process.
            /// </summary>
            /// <param name="id">The process id.</param>
            /// <returns>An instance of the Process class.</returns>
            public static Process GetParentProcess(int id)
            {
                Process process = Process.GetProcessById(id);
                return GetParentProcess(process.Handle);
            }

            /// <summary>
            /// Gets the parent process of a specified process.
            /// </summary>
            /// <param name="handle">The process handle.</param>
            /// <returns>An instance of the Process class.</returns>
            public static Process GetParentProcess(IntPtr handle)
            {
                ParentProcessUtilities pbi = new ParentProcessUtilities();
                int returnLength;
                int status = NtQueryInformationProcess(handle, 0, ref pbi, Marshal.SizeOf(pbi), out returnLength);
                if (status != 0)
                    throw new Win32Exception(status);

                try
                {
                    return Process.GetProcessById(pbi.InheritedFromUniqueProcessId.ToInt32());
                }
                catch (ArgumentException)
                {
                    // not found
                    return null;
                }
            }
        }
         */
        #endregion
    }

    /// <summary>The process info.</summary>
    public class ProcessInfo
    {
        /// <summary>The handle.</summary>
        public readonly IntPtr Handle;

        /// <summary>The location.</summary>
        public readonly string Location;

        /// <summary>The main window handle.</summary>
        public readonly IntPtr MainWindowHandle;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessInfo"/> class.
        /// </summary>
        /// <param name="handle">
        /// The handle.
        /// </param>
        /// <param name="mainWndHandle">
        /// The main wnd handle.
        /// </param>
        /// <param name="location">
        /// The location.
        /// </param>
        public ProcessInfo(IntPtr handle, IntPtr mainWndHandle, string location)
        {
            this.Handle = handle;
            this.MainWindowHandle = mainWndHandle;
            this.Location = location;
        }
    }
}