namespace SkyDean.FareLiz.WinForm.Components.Utils
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;

    /// <summary>
    ///     RECT structure
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    /// <summary>
    ///     Header item data
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct HDITEM
    {
        public W32_HDI mask;
        public int cxy;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pszText;
        public IntPtr hbm;
        public int cchTextMax;
        public W32_HDF fmt;
        public int lParam;
        // _WIN32_IE >= 0x0300 
        public int iImage;
        public int iOrder;
        // _WIN32_IE >= 0x0500
        public uint type;
        public IntPtr pvFilter;
        // _WIN32_WINNT >= 0x0600
        public uint state;
    };


    /// <summary>
    ///     Base notify message header
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NMHDR
    {
        public IntPtr hwndFrom;
        public int idFrom;
        public int code;
    }


    /// <summary>
    ///     Standard notify message header
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NMHEADER
    {
        public NMHDR hdr;
        public int iItem;
        public int iButton;
        public IntPtr pitem;
    }


    /// <summary>
    ///     Custom draw notify message
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NMCUSTOMDRAW
    {
        public NMHDR hdr;
        public int dwDrawStage;
        public IntPtr hdc;
        public RECT rc;
        public int dwItemSpec;
        public int uItemState;
        public IntPtr lItemlParam;
    }


    /// <summary>
    ///     ListView specialized custom draw message
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NMLVCUSTOMDRAW
    {
        public NMCUSTOMDRAW nmcd;
        public int clrText;
        public int clrTextBk;
        public int iSubItem;
        public uint dwItemType;
        public int clrFace;
        public int iIconEffect;
        public int iIconPhase;
        public int iPartId;
        public int iStateId;
        public RECT rcText;
        public uint uAlign;
    }


    /// <summary>
    ///     ListView item data
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct LVITEM
    {
        public W32_LVIF mask;
        public int iItem;
        public int iSubItem;
        public uint state;
        public uint stateMask;
        public String pszText;
        public int cchTextMax;
        public int iImage;
        public int lParam;
        public int iIndent;
    }


    /// <summary>
    ///     Header hittest information
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct HDHITTESTINFO
    {
        public long x;
        public long y;
        public uint flags;
        public int iItem;
    }


    /// <summary>
    ///     Structure for header layout
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct HDLAYOUT
    {
        public IntPtr prc; // RECT*
        public IntPtr pwpos; // WINDOWPOS*
    }


    /// <summary>
    ///     Header filter text data
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct HDTEXTFILTER
    {
        public String pszText;
        public int cchTextMax;
    }


    /// <summary>
    ///     Window position structure
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWPOS
    {
        private readonly IntPtr hwnd;
        private readonly IntPtr hwndInsertAfter;
        private readonly int x;
        private readonly int y;
        private readonly int cx;
        private readonly int cy;
        private readonly uint flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MINMAXINFO
    {
        public Point reserved;
        public Size maxSize;
        public Point maxPosition;
        public Size minTrackSize;
        public Size maxTrackSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TCHITTESTINFO
    {
        public TCHITTESTINFO(Point location)
        {
            this.pt = location;
            this.flags = TCHITTESTFLAGS.TCHT_ONITEM;
        }

        public Point pt;
        public TCHITTESTFLAGS flags;
    }

    [Flags]
    public enum TCHITTESTFLAGS
    {
        TCHT_NOWHERE = 1,
        TCHT_ONITEMICON = 2,
        TCHT_ONITEMLABEL = 4,
        TCHT_ONITEM = TCHT_ONITEMICON | TCHT_ONITEMLABEL
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NCCALCSIZE_PARAMS
    {
        public RECT rgc;
        public WINDOWPOS wndpos;
    }    
}