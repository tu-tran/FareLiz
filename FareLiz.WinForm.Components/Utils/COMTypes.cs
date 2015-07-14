namespace SkyDean.FareLiz.WinForm.Components.Utils
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>Represents the thumbnail progress bar RequestState.</summary>
    public enum ThumbnailProgressState
    {
        /// <summary>No progress is displayed.</summary>
        NoProgress = 0, 

        /// <summary>The progress is indeterminate (marquee).</summary>
        Indeterminate = 0x1, 

        /// <summary>Normal progress is displayed.</summary>
        Normal = 0x2, 

        /// <summary>An error occurred (red).</summary>
        Error = 0x4, 

        /// <summary>The operation is paused (yellow).</summary>
        Paused = 0x8
    }

    // Based on Rob Jarett's wrappers for the desktop integration PDC demos.
    /// <summary>The TaskbarList3 interface.</summary>
    [ComImport]
    [Guid("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ITaskbarList3
    {
        // ITaskbarList
        /// <summary>The hr init.</summary>
        [PreserveSig]
        void HrInit();

        /// <summary>
        /// The add tab.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        [PreserveSig]
        void AddTab(IntPtr hwnd);

        /// <summary>
        /// The delete tab.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        [PreserveSig]
        void DeleteTab(IntPtr hwnd);

        /// <summary>
        /// The activate tab.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        [PreserveSig]
        void ActivateTab(IntPtr hwnd);

        /// <summary>
        /// The set active alt.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        [PreserveSig]
        void SetActiveAlt(IntPtr hwnd);

        // ITaskbarList2
        /// <summary>
        /// The mark fullscreen window.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <param name="fFullscreen">
        /// The f fullscreen.
        /// </param>
        [PreserveSig]
        void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

        // ITaskbarList3
        /// <summary>
        /// The set progress value.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <param name="ullCompleted">
        /// The ull completed.
        /// </param>
        /// <param name="ullTotal">
        /// The ull total.
        /// </param>
        void SetProgressValue(IntPtr hwnd, ulong ullCompleted, ulong ullTotal);

        /// <summary>
        /// The set progress state.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <param name="tbpFlags">
        /// The tbp flags.
        /// </param>
        void SetProgressState(IntPtr hwnd, ThumbnailProgressState tbpFlags);

        // yadda, yadda - there's more to the interface, but we don't need it.
    }

    /// <summary>The c taskbar list.</summary>
    [Guid("56FDF344-FD6D-11d0-958A-006097C9A090")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComImport]
    internal class CTaskbarList
    {
    }
}