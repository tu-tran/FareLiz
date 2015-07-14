namespace SkyDean.FareLiz.Service.Utils.WinNative
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>The native methods.</summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// The get module file name ex.
        /// </summary>
        /// <param name="hProcess">
        /// The h process.
        /// </param>
        /// <param name="hModule">
        /// The h module.
        /// </param>
        /// <param name="lpFilename">
        /// The lp filename.
        /// </param>
        /// <param name="nSize">
        /// The n size.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        [DllImport("psapi.dll")]
        public static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, StringBuilder lpFilename, uint nSize);

        /// <summary>
        /// The get process image file name.
        /// </summary>
        /// <param name="hProcess">
        /// The h process.
        /// </param>
        /// <param name="lpImageFileName">
        /// The lp image file name.
        /// </param>
        /// <param name="nSize">
        /// The n size.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        [DllImport("psapi.dll")]
        public static extern uint GetProcessImageFileName(IntPtr hProcess, StringBuilder lpImageFileName, uint nSize);

        /// <summary>
        /// The open process.
        /// </summary>
        /// <param name="dwDesiredAccess">
        /// The dw desired access.
        /// </param>
        /// <param name="bInheritHandle">
        /// The b inherit handle.
        /// </param>
        /// <param name="dwProcessId">
        /// The dw process id.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        /// <summary>
        /// The query full process image name.
        /// </summary>
        /// <param name="hProcess">
        /// The h process.
        /// </param>
        /// <param name="dwFlags">
        /// The dw flags.
        /// </param>
        /// <param name="lpExeName">
        /// The lp exe name.
        /// </param>
        /// <param name="lpdwSize">
        /// The lpdw size.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [DllImport("kernel32.dll")]
        public static extern bool QueryFullProcessImageName(IntPtr hProcess, uint dwFlags, StringBuilder lpExeName, ref uint lpdwSize);

        /// <summary>
        /// The query dos device.
        /// </summary>
        /// <param name="lpDeviceName">
        /// The lp device name.
        /// </param>
        /// <param name="lpTargetPath">
        /// The lp target path.
        /// </param>
        /// <param name="uuchMax">
        /// The uuch max.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        [DllImport("kernel32.dll")]
        public static extern uint QueryDosDevice(string lpDeviceName, StringBuilder lpTargetPath, uint uuchMax);

        /// <summary>
        /// The get module handle.
        /// </summary>
        /// <param name="lpModuleName">
        /// The lp module name.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        /// <summary>
        /// The get window thread process id.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <param name="lpdwProcessId">
        /// The lpdw process id.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        /// <summary>
        /// The close handle.
        /// </summary>
        /// <param name="hObject">
        /// The h object.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);

        /// <summary>
        /// The find window.
        /// </summary>
        /// <param name="lpClassName">
        /// The lp class name.
        /// </param>
        /// <param name="lpWindowName">
        /// The lp window name.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// The find window ex.
        /// </summary>
        /// <param name="hwndParent">
        /// The hwnd parent.
        /// </param>
        /// <param name="hwndChildAfter">
        /// The hwnd child after.
        /// </param>
        /// <param name="lpszClass">
        /// The lpsz class.
        /// </param>
        /// <param name="lpszWindow">
        /// The lpsz window.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        /// <summary>
        /// The get client rect.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <param name="lpRect">
        /// The lp rect.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        /// <summary>
        /// The send message.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <param name="msg">
        /// The msg.
        /// </param>
        /// <param name="wParam">
        /// The w param.
        /// </param>
        /// <param name="lParam">
        /// The l param.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);
    }
}