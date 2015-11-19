namespace SkyDean.FareLiz.WinForm.Components.Utils
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    /// <summary>
    /// The native methods.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class NativeMethods
    {
        /// <summary>
        /// The cb n_ dropdown.
        /// </summary>
        public const int CBN_DROPDOWN = 7;

        /// <summary>
        /// The g w_ child.
        /// </summary>
        public const int GW_CHILD = 5;

        /// <summary>
        /// The g w_ hwndfirst.
        /// </summary>
        public const int GW_HWNDFIRST = 0;

        /// <summary>
        /// The g w_ hwndlast.
        /// </summary>
        public const int GW_HWNDLAST = 1;

        /// <summary>
        /// The g w_ hwndnext.
        /// </summary>
        public const int GW_HWNDNEXT = 2;

        /// <summary>
        /// The g w_ hwndprev.
        /// </summary>
        public const int GW_HWNDPREV = 3;

        /// <summary>
        /// The g w_ owner.
        /// </summary>
        public const int GW_OWNER = 4;

        /// <summary>
        /// The gw l_ wndproc.
        /// </summary>
        public const int GWL_WNDPROC = -4;

        /// <summary>
        /// The h c_ action.
        /// </summary>
        public const int HC_ACTION = 0;

        /// <summary>
        /// The htbottom.
        /// </summary>
        public const int HTBOTTOM = 15;

        /// <summary>
        /// The htbottomleft.
        /// </summary>
        public const int HTBOTTOMLEFT = 16;

        /// <summary>
        /// The htbottomright.
        /// </summary>
        public const int HTBOTTOMRIGHT = 17;

        /// <summary>
        /// The htleft.
        /// </summary>
        public const int HTLEFT = 10;

        /// <summary>
        /// The htright.
        /// </summary>
        public const int HTRIGHT = 11;

        /// <summary>
        /// The httop.
        /// </summary>
        public const int HTTOP = 12;

        /// <summary>
        /// The httopleft.
        /// </summary>
        public const int HTTOPLEFT = 13;

        /// <summary>
        /// The httopright.
        /// </summary>
        public const int HTTOPRIGHT = 14;

        /// <summary>
        /// The httransparent.
        /// </summary>
        public const int HTTRANSPARENT = -1;

        /// <summary>
        /// The hwn d_ broadcast.
        /// </summary>
        public const int HWND_BROADCAST = 0xffff;

        /// <summary>
        /// The s w_ shownormal.
        /// </summary>
        public const int SW_SHOWNORMAL = 1;

        /// <summary>
        /// The tc m_ hittest.
        /// </summary>
        public const int TCM_HITTEST = 0x130D;

        /// <summary>
        /// The w h_ callwndproc.
        /// </summary>
        public const int WH_CALLWNDPROC = 4;

        /// <summary>
        /// The w s_ e x_ noactivate.
        /// </summary>
        public const int WS_EX_NOACTIVATE = 0x08000000;

        /// <summary>
        /// The hi word.
        /// </summary>
        /// <param name="n">
        /// The n.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int HiWord(int n)
        {
            return (n >> 16) & 0xffff;
        }

        /// <summary>
        /// The hi word.
        /// </summary>
        /// <param name="n">
        /// The n.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int HiWord(IntPtr n)
        {
            return HiWord(unchecked((int)(long)n));
        }

        /// <summary>
        /// The lo word.
        /// </summary>
        /// <param name="n">
        /// The n.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int LoWord(int n)
        {
            return n & 0xffff;
        }

        /// <summary>
        /// The lo word.
        /// </summary>
        /// <param name="n">
        /// The n.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int LoWord(IntPtr n)
        {
            return LoWord(unchecked((int)(long)n));
        }

        /// <summary>
        /// The to int ptr.
        /// </summary>
        /// <param name="structure">
        /// The structure.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        public static IntPtr ToIntPtr(object structure)
        {
            IntPtr lparam = IntPtr.Zero;
            lparam = Marshal.AllocCoTaskMem(Marshal.SizeOf(structure));
            Marshal.StructureToPtr(structure, lparam, false);
            return lparam;
        }

        /// <summary>
        /// The get window dc.
        /// </summary>
        /// <param name="handle">
        /// The handle.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindowDC(IntPtr handle);

        /// <summary>
        /// The release dc.
        /// </summary>
        /// <param name="handle">
        /// The handle.
        /// </param>
        /// <param name="hDC">
        /// The h dc.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr ReleaseDC(IntPtr handle, IntPtr hDC);

        /// <summary>
        /// The create compatible dc.
        /// </summary>
        /// <param name="hdc">
        /// The hdc.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport("Gdi32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        /// <summary>
        /// The get class name.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <param name="className">
        /// The class name.
        /// </param>
        /// <param name="maxCount">
        /// The max count.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hwnd, char[] className, int maxCount);

        /// <summary>
        /// The get window.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <param name="uCmd">
        /// The u cmd.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindow(IntPtr hwnd, int uCmd);

        /// <summary>
        /// The is window visible.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool IsWindowVisible(IntPtr hwnd);

        /// <summary>
        /// The get client rect.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <param name="lpRect">
        /// The lp rect.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern int GetClientRect(IntPtr hwnd, ref RECT lpRect);

        /// <summary>
        /// The get client rect.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <param name="rect">
        /// The rect.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern int GetClientRect(IntPtr hwnd, [In] [Out] ref Rectangle rect);

        /// <summary>
        /// The move window.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <param name="X">
        /// The x.
        /// </param>
        /// <param name="Y">
        /// The y.
        /// </param>
        /// <param name="nWidth">
        /// The n width.
        /// </param>
        /// <param name="nHeight">
        /// The n height.
        /// </param>
        /// <param name="bRepaint">
        /// The b repaint.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern bool MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        /// <summary>
        /// The update window.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern bool UpdateWindow(IntPtr hwnd);

        /// <summary>
        /// The invalidate rect.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <param name="rect">
        /// The rect.
        /// </param>
        /// <param name="bErase">
        /// The b erase.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern bool InvalidateRect(IntPtr hwnd, ref Rectangle rect, bool bErase);

        /// <summary>
        /// The validate rect.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <param name="rect">
        /// The rect.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern bool ValidateRect(IntPtr hwnd, ref Rectangle rect);

        /// <summary>
        /// The get window rect.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <param name="rect">
        /// The rect.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetWindowRect(IntPtr hWnd, [In] [Out] ref Rectangle rect);

        /// <summary>
        /// The attach console.
        /// </summary>
        /// <param name="dwProcessId">
        /// The dw process id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [DllImport("kernel32.dll")]
        public static extern bool AttachConsole(int dwProcessId);

        /// <summary>
        /// The create round rect rgn.
        /// </summary>
        /// <param name="nLeftRect">
        /// The n left rect.
        /// </param>
        /// <param name="nTopRect">
        /// The n top rect.
        /// </param>
        /// <param name="nRightRect">
        /// The n right rect.
        /// </param>
        /// <param name="nBottomRect">
        /// The n bottom rect.
        /// </param>
        /// <param name="nWidthEllipse">
        /// The n width ellipse.
        /// </param>
        /// <param name="nHeightEllipse">
        /// The n height ellipse.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        public static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, 

            // x-coordinate of upper-left corner
            int nTopRect, 

            // y-coordinate of upper-left corner
            int nRightRect, 

            // x-coordinate of lower-right corner
            int nBottomRect, 

            // y-coordinate of lower-right corner
            int nWidthEllipse, 

            // height of ellipse
            int nHeightEllipse // width of ellipse
            );

        /// <summary>
        /// The get dlg item.
        /// </summary>
        /// <param name="hDlg">
        /// The h dlg.
        /// </param>
        /// <param name="nControlID">
        /// The n control id.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetDlgItem(IntPtr hDlg, int nControlID);

        /// <summary>
        /// The get window long.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <param name="flag">
        /// The flag.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, W32_GWL flag);

        /// <summary>
        /// The post message.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <param name="msg">
        /// The msg.
        /// </param>
        /// <param name="wparam">
        /// The wparam.
        /// </param>
        /// <param name="lparam">
        /// The lparam.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        /// <summary>
        /// The register window message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("user32")]
        public static extern int RegisterWindowMessage(string message);

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
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

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
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, ref HDITEM lParam);

        /// <summary>
        /// The send message.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <param name="wMsg">
        /// The w msg.
        /// </param>
        /// <param name="wParam">
        /// The w param.
        /// </param>
        /// <param name="lParam">
        /// The l param.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        /// <summary>
        /// The send message.
        /// </summary>
        /// <param name="wnd">
        /// The wnd.
        /// </param>
        /// <param name="msg">
        /// The msg.
        /// </param>
        /// <param name="param">
        /// The param.
        /// </param>
        /// <param name="lparam">
        /// The lparam.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr wnd, int msg, bool param, int lparam);

        /// <summary>
        /// The set window long.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <param name="flag">
        /// The flag.
        /// </param>
        /// <param name="dwNewLong">
        /// The dw new long.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, W32_GWL flag, int dwNewLong);

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
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, W32_HDM msg, int wParam, int lParam);

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
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, W32_HDM msg, int wParam, IntPtr lParam);

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
        /// <param name="hditem">
        /// The hditem.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, W32_HDM msg, int wParam, ref HDITEM hditem);

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
        /// <param name="rect">
        /// The rect.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, W32_HDM msg, int wParam, ref RECT rect);

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
        public static IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            // 	This Method replaces the User32 method SendMessage, but will only work for sending messages to Managed controls.
            Control control = Control.FromHandle(hWnd);
            if (control == null)
            {
                return IntPtr.Zero;
            }

            var message = new Message { HWnd = hWnd, LParam = lParam, WParam = wParam, Msg = msg };
            MethodInfo wproc = control.GetType()
                .GetMethod(
                    "WndProc", 
                    BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase
                    | BindingFlags.Instance);
            var args = new object[] { message };
            wproc.Invoke(control, args);

            return ((Message)args[0]).Result;
        }

        /// <summary>
        /// The set window pos.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <param name="hWndInsertAfter">
        /// The h wnd insert after.
        /// </param>
        /// <param name="X">
        /// The x.
        /// </param>
        /// <param name="Y">
        /// The y.
        /// </param>
        /// <param name="cx">
        /// The cx.
        /// </param>
        /// <param name="cy">
        /// The cy.
        /// </param>
        /// <param name="uFlags">
        /// The u flags.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern bool SetWindowPos(int hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        /// <summary>
        /// The set foreground window.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// The show window.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <param name="nCmdShow">
        /// The n cmd show.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        /// <summary>
        /// The show to front.
        /// </summary>
        /// <param name="window">
        /// The window.
        /// </param>
        public static void ShowToFront(IntPtr window)
        {
            ShowWindow(window, SW_SHOWNORMAL);
            SetForegroundWindow(window);
        }
    }
}