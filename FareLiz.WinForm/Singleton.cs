using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace SkyDean.FareLiz.WinForm
{
    using SkyDean.FareLiz.WinForm.Components.Utils;

    public static class WinApi
    {
        public const int FEATURE_DISABLE_NAVIGATION_SOUNDS = 21;
        public const int SET_FEATURE_ON_PROCESS = 0x00000002;

        [DllImport("winmm.dll")]
        private static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);

        [DllImport("winmm.dll")]
        private static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

        // Necessary dll import
        [DllImport("urlmon.dll")]
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Error)]
        public static extern int CoInternetSetFeatureEnabled(int FeatureEntry, [MarshalAs(UnmanagedType.U4)] int dwFlags,
                                                             bool fEnable);

        public static void SetVolume(int volume)
        {
            int NewVolume = ((ushort.MaxValue / 10) * volume);
            uint NewVolumeAllChannels = (((uint)NewVolume & 0x0000ffff) | ((uint)NewVolume << 16));
            waveOutSetVolume(IntPtr.Zero, NewVolumeAllChannels);
        }
    }

    public static class SingleInstance
    {
        public const int HWND_BROADCAST = 0xffff;
        public static readonly int WM_SHOWFIRSTINSTANCE =
            NativeMethods.RegisterWindowMessage("WM_SHOWFIRSTINSTANCE|" + ProgramInfo.AssemblyGuid.ToString());

        private static Mutex mutex;
        private static bool started;

        public static bool Start()
        {
            string mutexName = String.Format("Local\\{0}", ProgramInfo.AssemblyGuid);

#if DEBUG
            mutexName += "_DEBUG";
#endif

            // if you want your app to be limited to a single instance
            // across ALL SESSIONS (multiple users & terminal services), then use the following line instead:
            // string mutexName = String.Format("Global\\{0}", ProgramInfo.AssemblyGuid);

            mutex = new Mutex(true, mutexName, out started);
            return started;
        }

        public static void ShowFirstInstance()
        {
            NativeMethods.PostMessage(
                (IntPtr)HWND_BROADCAST,
                WM_SHOWFIRSTINSTANCE,
                IntPtr.Zero,
                IntPtr.Zero);
        }

        public static void Stop()
        {
            if (started)
                mutex.ReleaseMutex();
        }
    }

    public static class ProgramInfo
    {
        public static string AssemblyGuid
        {
            get
            {
                object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(GuidAttribute), false);
                if (attributes.Length == 0)
                {
                    return String.Empty;
                }
                return ((GuidAttribute)attributes[0]).Value;
            }
        }
    }
}