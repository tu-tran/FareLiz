namespace SkyDean.FareLiz.WinForm.Components.Utils
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;

    /// <summary>RECT structure</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        /// <summary>
        /// The left.
        /// </summary>
        public int left;

        /// <summary>
        /// The top.
        /// </summary>
        public int top;

        /// <summary>
        /// The right.
        /// </summary>
        public int right;

        /// <summary>
        /// The bottom.
        /// </summary>
        public int bottom;
    }

    /// <summary>Header item data</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct HDITEM
    {
        /// <summary>
        /// The mask.
        /// </summary>
        public W32_HDI mask;

        /// <summary>
        /// The cxy.
        /// </summary>
        public int cxy;

        /// <summary>
        /// The psz text.
        /// </summary>
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pszText;

        /// <summary>
        /// The hbm.
        /// </summary>
        public IntPtr hbm;

        /// <summary>
        /// The cch text max.
        /// </summary>
        public int cchTextMax;

        /// <summary>
        /// The fmt.
        /// </summary>
        public W32_HDF fmt;

        /// <summary>
        /// The l param.
        /// </summary>
        public int lParam;

        // _WIN32_IE >= 0x0300 
        /// <summary>
        /// The i image.
        /// </summary>
        public int iImage;

        /// <summary>
        /// The i order.
        /// </summary>
        public int iOrder;

        // _WIN32_IE >= 0x0500
        /// <summary>
        /// The type.
        /// </summary>
        public uint type;

        /// <summary>
        /// The pv filter.
        /// </summary>
        public IntPtr pvFilter;

        // _WIN32_WINNT >= 0x0600
        /// <summary>
        /// The state.
        /// </summary>
        public uint state;
    };

    /// <summary>Base notify message header</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NMHDR
    {
        /// <summary>
        /// The hwnd from.
        /// </summary>
        public IntPtr hwndFrom;

        /// <summary>
        /// The id from.
        /// </summary>
        public int idFrom;

        /// <summary>
        /// The code.
        /// </summary>
        public int code;
    }

    /// <summary>Standard notify message header</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NMHEADER
    {
        /// <summary>
        /// The hdr.
        /// </summary>
        public NMHDR hdr;

        /// <summary>
        /// The i item.
        /// </summary>
        public int iItem;

        /// <summary>
        /// The i button.
        /// </summary>
        public int iButton;

        /// <summary>
        /// The pitem.
        /// </summary>
        public IntPtr pitem;
    }

    /// <summary>Custom draw notify message</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NMCUSTOMDRAW
    {
        /// <summary>
        /// The hdr.
        /// </summary>
        public NMHDR hdr;

        /// <summary>
        /// The dw draw stage.
        /// </summary>
        public int dwDrawStage;

        /// <summary>
        /// The hdc.
        /// </summary>
        public IntPtr hdc;

        /// <summary>
        /// The rc.
        /// </summary>
        public RECT rc;

        /// <summary>
        /// The dw item spec.
        /// </summary>
        public int dwItemSpec;

        /// <summary>
        /// The u item state.
        /// </summary>
        public int uItemState;

        /// <summary>
        /// The l iteml param.
        /// </summary>
        public IntPtr lItemlParam;
    }

    /// <summary>ListView specialized custom draw message</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NMLVCUSTOMDRAW
    {
        /// <summary>
        /// The nmcd.
        /// </summary>
        public NMCUSTOMDRAW nmcd;

        /// <summary>
        /// The clr text.
        /// </summary>
        public int clrText;

        /// <summary>
        /// The clr text bk.
        /// </summary>
        public int clrTextBk;

        /// <summary>
        /// The i sub item.
        /// </summary>
        public int iSubItem;

        /// <summary>
        /// The dw item type.
        /// </summary>
        public uint dwItemType;

        /// <summary>
        /// The clr face.
        /// </summary>
        public int clrFace;

        /// <summary>
        /// The i icon effect.
        /// </summary>
        public int iIconEffect;

        /// <summary>
        /// The i icon phase.
        /// </summary>
        public int iIconPhase;

        /// <summary>
        /// The i part id.
        /// </summary>
        public int iPartId;

        /// <summary>
        /// The i state id.
        /// </summary>
        public int iStateId;

        /// <summary>
        /// The rc text.
        /// </summary>
        public RECT rcText;

        /// <summary>
        /// The u align.
        /// </summary>
        public uint uAlign;
    }

    /// <summary>ListView item data</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct LVITEM
    {
        /// <summary>
        /// The mask.
        /// </summary>
        public W32_LVIF mask;

        /// <summary>
        /// The i item.
        /// </summary>
        public int iItem;

        /// <summary>
        /// The i sub item.
        /// </summary>
        public int iSubItem;

        /// <summary>
        /// The state.
        /// </summary>
        public uint state;

        /// <summary>
        /// The state mask.
        /// </summary>
        public uint stateMask;

        /// <summary>
        /// The psz text.
        /// </summary>
        public string pszText;

        /// <summary>
        /// The cch text max.
        /// </summary>
        public int cchTextMax;

        /// <summary>
        /// The i image.
        /// </summary>
        public int iImage;

        /// <summary>
        /// The l param.
        /// </summary>
        public int lParam;

        /// <summary>
        /// The i indent.
        /// </summary>
        public int iIndent;
    }

    /// <summary>Header hittest information</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct HDHITTESTINFO
    {
        /// <summary>
        /// The x.
        /// </summary>
        public long x;

        /// <summary>
        /// The y.
        /// </summary>
        public long y;

        /// <summary>
        /// The flags.
        /// </summary>
        public uint flags;

        /// <summary>
        /// The i item.
        /// </summary>
        public int iItem;
    }

    /// <summary>Structure for header layout</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct HDLAYOUT
    {
        /// <summary>
        /// The prc.
        /// </summary>
        public IntPtr prc; // RECT*

        /// <summary>
        /// The pwpos.
        /// </summary>
        public IntPtr pwpos; // WINDOWPOS*
    }

    /// <summary>Header filter text data</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct HDTEXTFILTER
    {
        /// <summary>
        /// The psz text.
        /// </summary>
        public string pszText;

        /// <summary>
        /// The cch text max.
        /// </summary>
        public int cchTextMax;
    }

    /// <summary>Window position structure</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWPOS
    {
        /// <summary>
        /// The hwnd.
        /// </summary>
        private readonly IntPtr hwnd;

        /// <summary>
        /// The hwnd insert after.
        /// </summary>
        private readonly IntPtr hwndInsertAfter;

        /// <summary>
        /// The x.
        /// </summary>
        private readonly int x;

        /// <summary>
        /// The y.
        /// </summary>
        private readonly int y;

        /// <summary>
        /// The cx.
        /// </summary>
        private readonly int cx;

        /// <summary>
        /// The cy.
        /// </summary>
        private readonly int cy;

        /// <summary>
        /// The flags.
        /// </summary>
        private readonly uint flags;
    }

    /// <summary>
    /// The minmaxinfo.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MINMAXINFO
    {
        /// <summary>
        /// The reserved.
        /// </summary>
        public Point reserved;

        /// <summary>
        /// The max size.
        /// </summary>
        public Size maxSize;

        /// <summary>
        /// The max position.
        /// </summary>
        public Point maxPosition;

        /// <summary>
        /// The min track size.
        /// </summary>
        public Size minTrackSize;

        /// <summary>
        /// The max track size.
        /// </summary>
        public Size maxTrackSize;
    }

    /// <summary>
    /// The tchittestinfo.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct TCHITTESTINFO
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TCHITTESTINFO"/> struct.
        /// </summary>
        /// <param name="location">
        /// The location.
        /// </param>
        public TCHITTESTINFO(Point location)
        {
            this.pt = location;
            this.flags = TCHITTESTFLAGS.TCHT_ONITEM;
        }

        /// <summary>
        /// The pt.
        /// </summary>
        public Point pt;

        /// <summary>
        /// The flags.
        /// </summary>
        public TCHITTESTFLAGS flags;
    }

    /// <summary>
    /// The tchittestflags.
    /// </summary>
    [Flags]
    public enum TCHITTESTFLAGS
    {
        /// <summary>
        /// The tch t_ nowhere.
        /// </summary>
        TCHT_NOWHERE = 1, 

        /// <summary>
        /// The tch t_ onitemicon.
        /// </summary>
        TCHT_ONITEMICON = 2, 

        /// <summary>
        /// The tch t_ onitemlabel.
        /// </summary>
        TCHT_ONITEMLABEL = 4, 

        /// <summary>
        /// The tch t_ onitem.
        /// </summary>
        TCHT_ONITEM = TCHT_ONITEMICON | TCHT_ONITEMLABEL
    }

    /// <summary>
    /// The nccalcsiz e_ params.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NCCALCSIZE_PARAMS
    {
        /// <summary>
        /// The rgc.
        /// </summary>
        public RECT rgc;

        /// <summary>
        /// The wndpos.
        /// </summary>
        public WINDOWPOS wndpos;
    }
}