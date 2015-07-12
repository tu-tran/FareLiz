using System;
using System.Windows.Forms;

namespace SkyDean.FareLiz.WinForm.Utils
{
    using SkyDean.FareLiz.WinForm.Components.Utils;

    /// <summary>
    /// GUI Builder utility class
    /// </summary>
    internal static class GUIBuilder
    {
        /// <summary>
        /// Attach the default context menu into tray icon for Win32 Window (for restoring form and exit application)
        /// </summary>
        internal static void AttachMenuToTrayIcon(Form parent, NotifyIcon trayIcon, bool showInTaskbarOnRestore)
        {
            // Restore window and put it to front
            var restoreWindowHandler = new EventHandler((o, e) =>
                {
                    parent.Show();
                    parent.ShowInTaskbar = showInTaskbarOnRestore;
                    parent.WindowState = FormWindowState.Normal;
                    NativeMethods.ShowToFront(parent.Handle);
                });

            var mnuShow = new ToolStripMenuItem("Show Main Window", null, restoreWindowHandler);
            var toolStripSeparator = new ToolStripSeparator();
            var mnuExit = new ToolStripMenuItem("Exit", null, new EventHandler((o, e) =>
            {
                Environment.Exit(0);
            }));

            var trayIconMenuStrip = new ContextMenuStrip();
            trayIconMenuStrip.Items.Add(mnuShow);
            trayIconMenuStrip.Items.Add(toolStripSeparator);
            trayIconMenuStrip.Items.Add(mnuExit);

            // Attach the handler for double-clicking the tray icon
            trayIcon.DoubleClick -= restoreWindowHandler;
            trayIcon.DoubleClick += restoreWindowHandler;

            // Attach the menu strip to the tray icon
            trayIcon.ContextMenuStrip = trayIconMenuStrip;
        }
    }
}
