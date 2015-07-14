namespace SkyDean.FareLiz.WinForm
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading;

    using SkyDean.FareLiz.WinForm.Components.Utils;

    /// <summary>The win api.</summary>
    public static class WinApi
    {
        /// <summary>The featur e_ disabl e_ navigatio n_ sounds.</summary>
        public const int FEATURE_DISABLE_NAVIGATION_SOUNDS = 21;

        /// <summary>The se t_ featur e_ o n_ process.</summary>
        public const int SET_FEATURE_ON_PROCESS = 0x00000002;

        /// <summary>
        /// The wave out get volume.
        /// </summary>
        /// <param name="hwo">
        /// The hwo.
        /// </param>
        /// <param name="dwVolume">
        /// The dw volume.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("winmm.dll")]
        private static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);

        /// <summary>
        /// The wave out set volume.
        /// </summary>
        /// <param name="hwo">
        /// The hwo.
        /// </param>
        /// <param name="dwVolume">
        /// The dw volume.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("winmm.dll")]
        private static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

        // Necessary dll import
        /// <summary>
        /// The co internet set feature enabled.
        /// </summary>
        /// <param name="FeatureEntry">
        /// The feature entry.
        /// </param>
        /// <param name="dwFlags">
        /// The dw flags.
        /// </param>
        /// <param name="fEnable">
        /// The f enable.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("urlmon.dll")]
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Error)]
        public static extern int CoInternetSetFeatureEnabled(int FeatureEntry, [MarshalAs(UnmanagedType.U4)] int dwFlags, bool fEnable);

        /// <summary>
        /// The set volume.
        /// </summary>
        /// <param name="volume">
        /// The volume.
        /// </param>
        public static void SetVolume(int volume)
        {
            var NewVolume = (ushort.MaxValue / 10) * volume;
            var NewVolumeAllChannels = ((uint)NewVolume & 0x0000ffff) | ((uint)NewVolume << 16);
            waveOutSetVolume(IntPtr.Zero, NewVolumeAllChannels);
        }
    }

    /// <summary>The single instance.</summary>
    public static class SingleInstance
    {
        /// <summary>The hwn d_ broadcast.</summary>
        public const int HWND_BROADCAST = 0xffff;

        /// <summary>The w m_ showfirstinstance.</summary>
        public static readonly int WM_SHOWFIRSTINSTANCE = NativeMethods.RegisterWindowMessage("WM_SHOWFIRSTINSTANCE|" + ProgramInfo.AssemblyGuid);

        /// <summary>The mutex.</summary>
        private static Mutex mutex;

        /// <summary>The started.</summary>
        private static bool started;

        /// <summary>The start.</summary>
        /// <returns>The <see cref="bool" />.</returns>
        public static bool Start()
        {
            var mutexName = string.Format("Local\\{0}", ProgramInfo.AssemblyGuid);

#if DEBUG
            mutexName += "_DEBUG";
#endif

            // if you want your app to be limited to a single instance
            // across ALL SESSIONS (multiple users & terminal services), then use the following line instead:
            // string mutexName = String.Format("Global\\{0}", ProgramInfo.AssemblyGuid);
            mutex = new Mutex(true, mutexName, out started);
            return started;
        }

        /// <summary>The show first instance.</summary>
        public static void ShowFirstInstance()
        {
            NativeMethods.PostMessage((IntPtr)HWND_BROADCAST, WM_SHOWFIRSTINSTANCE, IntPtr.Zero, IntPtr.Zero);
        }

        /// <summary>The stop.</summary>
        public static void Stop()
        {
            if (started)
            {
                mutex.ReleaseMutex();
            }
        }
    }

    /// <summary>The program info.</summary>
    public static class ProgramInfo
    {
        /// <summary>Gets the assembly guid.</summary>
        public static string AssemblyGuid
        {
            get
            {
                var attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(GuidAttribute), false);
                if (attributes.Length == 0)
                {
                    return string.Empty;
                }

                return ((GuidAttribute)attributes[0]).Value;
            }
        }
    }
}