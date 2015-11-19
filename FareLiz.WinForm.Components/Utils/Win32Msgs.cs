namespace SkyDean.FareLiz.WinForm.Components.Utils
{
    /// <summary>First message codes for sub message groups</summary>
    public enum BaseCodes
    {
        /// <summary>
        /// The lv m_ first.
        /// </summary>
        LVM_FIRST = 0x1000, 

        /// <summary>
        /// The t v_ first.
        /// </summary>
        TV_FIRST = 0x1100, 

        /// <summary>
        /// The hd m_ first.
        /// </summary>
        HDM_FIRST = 0x1200, 

        /// <summary>
        /// The tc m_ first.
        /// </summary>
        TCM_FIRST = 0x1300, 

        /// <summary>
        /// The pg m_ first.
        /// </summary>
        PGM_FIRST = 0x1400, 

        /// <summary>
        /// The ec m_ first.
        /// </summary>
        ECM_FIRST = 0x1500, 

        /// <summary>
        /// The bc m_ first.
        /// </summary>
        BCM_FIRST = 0x1600, 

        /// <summary>
        /// The cb m_ first.
        /// </summary>
        CBM_FIRST = 0x1700, 

        /// <summary>
        /// The cc m_ first.
        /// </summary>
        CCM_FIRST = 0x2000, 

        /// <summary>
        /// The n m_ first.
        /// </summary>
        NM_FIRST = 0x0000, 

        /// <summary>
        /// The lv n_ first.
        /// </summary>
        LVN_FIRST = NM_FIRST - 100, 

        /// <summary>
        /// The hd n_ first.
        /// </summary>
        HDN_FIRST = NM_FIRST - 300
    }

    /// <summary>Windows message codes WM_</summary>
    public enum W32_WM
    {
        /// <summary>
        /// The w m_ activate.
        /// </summary>
        WM_ACTIVATE = 0x0006, 

        /// <summary>
        /// The w m_ activateapp.
        /// </summary>
        WM_ACTIVATEAPP = 0x001C, 

        /// <summary>
        /// The w m_ afxfirst.
        /// </summary>
        WM_AFXFIRST = 0x0360, 

        /// <summary>
        /// The w m_ afxlast.
        /// </summary>
        WM_AFXLAST = 0x037F, 

        /// <summary>
        /// The w m_ app.
        /// </summary>
        WM_APP = 0x8000, 

        /// <summary>
        /// The w m_ askcbformatname.
        /// </summary>
        WM_ASKCBFORMATNAME = 0x030C, 

        /// <summary>
        /// The w m_ canceljournal.
        /// </summary>
        WM_CANCELJOURNAL = 0x004B, 

        /// <summary>
        /// The w m_ cancelmode.
        /// </summary>
        WM_CANCELMODE = 0x001F, 

        /// <summary>
        /// The w m_ capturechanged.
        /// </summary>
        WM_CAPTURECHANGED = 0x0215, 

        /// <summary>
        /// The w m_ changecbchain.
        /// </summary>
        WM_CHANGECBCHAIN = 0x030D, 

        /// <summary>
        /// The w m_ char.
        /// </summary>
        WM_CHAR = 0x0102, 

        /// <summary>
        /// The w m_ chartoitem.
        /// </summary>
        WM_CHARTOITEM = 0x002F, 

        /// <summary>
        /// The w m_ childactivate.
        /// </summary>
        WM_CHILDACTIVATE = 0x0022, 

        /// <summary>
        /// The w m_ clear.
        /// </summary>
        WM_CLEAR = 0x0303, 

        /// <summary>
        /// The w m_ close.
        /// </summary>
        WM_CLOSE = 0x0010, 

        /// <summary>
        /// The w m_ command.
        /// </summary>
        WM_COMMAND = 0x0111, 

        /// <summary>
        /// The w m_ commnotify.
        /// </summary>
        WM_COMMNOTIFY = 0x0044, 

        /// <summary>
        /// The w m_ compacting.
        /// </summary>
        WM_COMPACTING = 0x0041, 

        /// <summary>
        /// The w m_ compareitem.
        /// </summary>
        WM_COMPAREITEM = 0x0039, 

        /// <summary>
        /// The w m_ contextmenu.
        /// </summary>
        WM_CONTEXTMENU = 0x007B, 

        /// <summary>
        /// The w m_ copy.
        /// </summary>
        WM_COPY = 0x0301, 

        /// <summary>
        /// The w m_ copydata.
        /// </summary>
        WM_COPYDATA = 0x004A, 

        /// <summary>
        /// The w m_ create.
        /// </summary>
        WM_CREATE = 0x0001, 

        /// <summary>
        /// The w m_ ctlcolor.
        /// </summary>
        WM_CTLCOLOR = 0x0019, 

        /// <summary>
        /// The w m_ ctlcolorbtn.
        /// </summary>
        WM_CTLCOLORBTN = 0x0135, 

        /// <summary>
        /// The w m_ ctlcolordlg.
        /// </summary>
        WM_CTLCOLORDLG = 0x0136, 

        /// <summary>
        /// The w m_ ctlcoloredit.
        /// </summary>
        WM_CTLCOLOREDIT = 0x0133, 

        /// <summary>
        /// The w m_ ctlcolorlistbox.
        /// </summary>
        WM_CTLCOLORLISTBOX = 0x0134, 

        /// <summary>
        /// The w m_ ctlcolormsgbox.
        /// </summary>
        WM_CTLCOLORMSGBOX = 0x0132, 

        /// <summary>
        /// The w m_ ctlcolorscrollbar.
        /// </summary>
        WM_CTLCOLORSCROLLBAR = 0x0137, 

        /// <summary>
        /// The w m_ ctlcolorstatic.
        /// </summary>
        WM_CTLCOLORSTATIC = 0x0138, 

        /// <summary>
        /// The w m_ cut.
        /// </summary>
        WM_CUT = 0x0300, 

        /// <summary>
        /// The w m_ deadchar.
        /// </summary>
        WM_DEADCHAR = 0x0103, 

        /// <summary>
        /// The w m_ deleteitem.
        /// </summary>
        WM_DELETEITEM = 0x002D, 

        /// <summary>
        /// The w m_ destroy.
        /// </summary>
        WM_DESTROY = 0x0002, 

        /// <summary>
        /// The w m_ destroyclipboard.
        /// </summary>
        WM_DESTROYCLIPBOARD = 0x0307, 

        /// <summary>
        /// The w m_ devicechange.
        /// </summary>
        WM_DEVICECHANGE = 0x0219, 

        /// <summary>
        /// The w m_ devmodechange.
        /// </summary>
        WM_DEVMODECHANGE = 0x001B, 

        /// <summary>
        /// The w m_ displaychange.
        /// </summary>
        WM_DISPLAYCHANGE = 0x007E, 

        /// <summary>
        /// The w m_ drawclipboard.
        /// </summary>
        WM_DRAWCLIPBOARD = 0x0308, 

        /// <summary>
        /// The w m_ drawitem.
        /// </summary>
        WM_DRAWITEM = 0x002B, 

        /// <summary>
        /// The w m_ dropfiles.
        /// </summary>
        WM_DROPFILES = 0x0233, 

        /// <summary>
        /// The w m_ enable.
        /// </summary>
        WM_ENABLE = 0x000A, 

        /// <summary>
        /// The w m_ endsession.
        /// </summary>
        WM_ENDSESSION = 0x0016, 

        /// <summary>
        /// The w m_ enteridle.
        /// </summary>
        WM_ENTERIDLE = 0x0121, 

        /// <summary>
        /// The w m_ entermenuloop.
        /// </summary>
        WM_ENTERMENULOOP = 0x0211, 

        /// <summary>
        /// The w m_ entersizemove.
        /// </summary>
        WM_ENTERSIZEMOVE = 0x0231, 

        /// <summary>
        /// The w m_ erasebkgnd.
        /// </summary>
        WM_ERASEBKGND = 0x0014, 

        /// <summary>
        /// The w m_ exitmenuloop.
        /// </summary>
        WM_EXITMENULOOP = 0x0212, 

        /// <summary>
        /// The w m_ exitsizemove.
        /// </summary>
        WM_EXITSIZEMOVE = 0x0232, 

        /// <summary>
        /// The w m_ fontchange.
        /// </summary>
        WM_FONTCHANGE = 0x001D, 

        /// <summary>
        /// The w m_ getdlgcode.
        /// </summary>
        WM_GETDLGCODE = 0x0087, 

        /// <summary>
        /// The w m_ getfont.
        /// </summary>
        WM_GETFONT = 0x0031, 

        /// <summary>
        /// The w m_ gethotkey.
        /// </summary>
        WM_GETHOTKEY = 0x0033, 

        /// <summary>
        /// The w m_ geticon.
        /// </summary>
        WM_GETICON = 0x007F, 

        /// <summary>
        /// The w m_ getminmaxinfo.
        /// </summary>
        WM_GETMINMAXINFO = 0x0024, 

        /// <summary>
        /// The w m_ getobject.
        /// </summary>
        WM_GETOBJECT = 0x003D, 

        /// <summary>
        /// The w m_ gettext.
        /// </summary>
        WM_GETTEXT = 0x000D, 

        /// <summary>
        /// The w m_ gettextlength.
        /// </summary>
        WM_GETTEXTLENGTH = 0x000E, 

        /// <summary>
        /// The w m_ handheldfirst.
        /// </summary>
        WM_HANDHELDFIRST = 0x0358, 

        /// <summary>
        /// The w m_ handheldlast.
        /// </summary>
        WM_HANDHELDLAST = 0x035F, 

        /// <summary>
        /// The w m_ help.
        /// </summary>
        WM_HELP = 0x0053, 

        /// <summary>
        /// The w m_ hotkey.
        /// </summary>
        WM_HOTKEY = 0x0312, 

        /// <summary>
        /// The w m_ hscroll.
        /// </summary>
        WM_HSCROLL = 0x0114, 

        /// <summary>
        /// The w m_ hscrollclipboard.
        /// </summary>
        WM_HSCROLLCLIPBOARD = 0x030E, 

        /// <summary>
        /// The w m_ iconerasebkgnd.
        /// </summary>
        WM_ICONERASEBKGND = 0x0027, 

        /// <summary>
        /// The w m_ im e_ char.
        /// </summary>
        WM_IME_CHAR = 0x0286, 

        /// <summary>
        /// The w m_ im e_ composition.
        /// </summary>
        WM_IME_COMPOSITION = 0x010F, 

        /// <summary>
        /// The w m_ im e_ compositionfull.
        /// </summary>
        WM_IME_COMPOSITIONFULL = 0x0284, 

        /// <summary>
        /// The w m_ im e_ control.
        /// </summary>
        WM_IME_CONTROL = 0x0283, 

        /// <summary>
        /// The w m_ im e_ endcomposition.
        /// </summary>
        WM_IME_ENDCOMPOSITION = 0x010E, 

        /// <summary>
        /// The w m_ im e_ keydown.
        /// </summary>
        WM_IME_KEYDOWN = 0x0290, 

        /// <summary>
        /// The w m_ im e_ keylast.
        /// </summary>
        WM_IME_KEYLAST = 0x010F, 

        /// <summary>
        /// The w m_ im e_ keyup.
        /// </summary>
        WM_IME_KEYUP = 0x0291, 

        /// <summary>
        /// The w m_ im e_ notify.
        /// </summary>
        WM_IME_NOTIFY = 0x0282, 

        /// <summary>
        /// The w m_ im e_ request.
        /// </summary>
        WM_IME_REQUEST = 0x0288, 

        /// <summary>
        /// The w m_ im e_ select.
        /// </summary>
        WM_IME_SELECT = 0x0285, 

        /// <summary>
        /// The w m_ im e_ setcontext.
        /// </summary>
        WM_IME_SETCONTEXT = 0x0281, 

        /// <summary>
        /// The w m_ im e_ startcomposition.
        /// </summary>
        WM_IME_STARTCOMPOSITION = 0x010D, 

        /// <summary>
        /// The w m_ initdialog.
        /// </summary>
        WM_INITDIALOG = 0x0110, 

        /// <summary>
        /// The w m_ initmenu.
        /// </summary>
        WM_INITMENU = 0x0116, 

        /// <summary>
        /// The w m_ initmenupopup.
        /// </summary>
        WM_INITMENUPOPUP = 0x0117, 

        /// <summary>
        /// The w m_ inputlangchange.
        /// </summary>
        WM_INPUTLANGCHANGE = 0x0051, 

        /// <summary>
        /// The w m_ inputlangchangerequest.
        /// </summary>
        WM_INPUTLANGCHANGEREQUEST = 0x0050, 

        /// <summary>
        /// The w m_ keydown.
        /// </summary>
        WM_KEYDOWN = 0x0100, 

        /// <summary>
        /// The w m_ keylast.
        /// </summary>
        WM_KEYLAST = 0x0108, 

        /// <summary>
        /// The w m_ keyup.
        /// </summary>
        WM_KEYUP = 0x0101, 

        /// <summary>
        /// The w m_ killfocus.
        /// </summary>
        WM_KILLFOCUS = 0x0008, 

        /// <summary>
        /// The w m_ lbuttondblclk.
        /// </summary>
        WM_LBUTTONDBLCLK = 0x0203, 

        /// <summary>
        /// The w m_ lbuttondown.
        /// </summary>
        WM_LBUTTONDOWN = 0x0201, 

        /// <summary>
        /// The w m_ lbuttonup.
        /// </summary>
        WM_LBUTTONUP = 0x0202, 

        /// <summary>
        /// The w m_ mbuttondblclk.
        /// </summary>
        WM_MBUTTONDBLCLK = 0x0209, 

        /// <summary>
        /// The w m_ mbuttondown.
        /// </summary>
        WM_MBUTTONDOWN = 0x0207, 

        /// <summary>
        /// The w m_ mbuttonup.
        /// </summary>
        WM_MBUTTONUP = 0x0208, 

        /// <summary>
        /// The w m_ mdiactivate.
        /// </summary>
        WM_MDIACTIVATE = 0x0222, 

        /// <summary>
        /// The w m_ mdicascade.
        /// </summary>
        WM_MDICASCADE = 0x0227, 

        /// <summary>
        /// The w m_ mdicreate.
        /// </summary>
        WM_MDICREATE = 0x0220, 

        /// <summary>
        /// The w m_ mdidestroy.
        /// </summary>
        WM_MDIDESTROY = 0x0221, 

        /// <summary>
        /// The w m_ mdigetactive.
        /// </summary>
        WM_MDIGETACTIVE = 0x0229, 

        /// <summary>
        /// The w m_ mdiiconarrange.
        /// </summary>
        WM_MDIICONARRANGE = 0x0228, 

        /// <summary>
        /// The w m_ mdimaximize.
        /// </summary>
        WM_MDIMAXIMIZE = 0x0225, 

        /// <summary>
        /// The w m_ mdinext.
        /// </summary>
        WM_MDINEXT = 0x0224, 

        /// <summary>
        /// The w m_ mdirefreshmenu.
        /// </summary>
        WM_MDIREFRESHMENU = 0x0234, 

        /// <summary>
        /// The w m_ mdirestore.
        /// </summary>
        WM_MDIRESTORE = 0x0223, 

        /// <summary>
        /// The w m_ mdisetmenu.
        /// </summary>
        WM_MDISETMENU = 0x0230, 

        /// <summary>
        /// The w m_ mditile.
        /// </summary>
        WM_MDITILE = 0x0226, 

        /// <summary>
        /// The w m_ measureitem.
        /// </summary>
        WM_MEASUREITEM = 0x002C, 

        /// <summary>
        /// The w m_ menuchar.
        /// </summary>
        WM_MENUCHAR = 0x0120, 

        /// <summary>
        /// The w m_ menucommand.
        /// </summary>
        WM_MENUCOMMAND = 0x0126, 

        /// <summary>
        /// The w m_ menudrag.
        /// </summary>
        WM_MENUDRAG = 0x0123, 

        /// <summary>
        /// The w m_ menugetobject.
        /// </summary>
        WM_MENUGETOBJECT = 0x0124, 

        /// <summary>
        /// The w m_ menurbuttonup.
        /// </summary>
        WM_MENURBUTTONUP = 0x0122, 

        /// <summary>
        /// The w m_ menuselect.
        /// </summary>
        WM_MENUSELECT = 0x011F, 

        /// <summary>
        /// The w m_ mouseactivate.
        /// </summary>
        WM_MOUSEACTIVATE = 0x0021, 

        /// <summary>
        /// The w m_ mousehover.
        /// </summary>
        WM_MOUSEHOVER = 0x02A1, 

        /// <summary>
        /// The w m_ mouseleave.
        /// </summary>
        WM_MOUSELEAVE = 0x02A3, 

        /// <summary>
        /// The w m_ mousemove.
        /// </summary>
        WM_MOUSEMOVE = 0x0200, 

        /// <summary>
        /// The w m_ mousewheel.
        /// </summary>
        WM_MOUSEWHEEL = 0x020A, 

        /// <summary>
        /// The w m_ move.
        /// </summary>
        WM_MOVE = 0x0003, 

        /// <summary>
        /// The w m_ moving.
        /// </summary>
        WM_MOVING = 0x0216, 

        /// <summary>
        /// The w m_ ncactivate.
        /// </summary>
        WM_NCACTIVATE = 0x0086, 

        /// <summary>
        /// The w m_ nccalcsize.
        /// </summary>
        WM_NCCALCSIZE = 0x0083, 

        /// <summary>
        /// The w m_ nccreate.
        /// </summary>
        WM_NCCREATE = 0x0081, 

        /// <summary>
        /// The w m_ ncdestroy.
        /// </summary>
        WM_NCDESTROY = 0x0082, 

        /// <summary>
        /// The w m_ nchittest.
        /// </summary>
        WM_NCHITTEST = 0x0084, 

        /// <summary>
        /// The w m_ nclbuttondblclk.
        /// </summary>
        WM_NCLBUTTONDBLCLK = 0x00A3, 

        /// <summary>
        /// The w m_ nclbuttondown.
        /// </summary>
        WM_NCLBUTTONDOWN = 0x00A1, 

        /// <summary>
        /// The w m_ nclbuttonup.
        /// </summary>
        WM_NCLBUTTONUP = 0x00A2, 

        /// <summary>
        /// The w m_ ncmbuttondblclk.
        /// </summary>
        WM_NCMBUTTONDBLCLK = 0x00A9, 

        /// <summary>
        /// The w m_ ncmbuttondown.
        /// </summary>
        WM_NCMBUTTONDOWN = 0x00A7, 

        /// <summary>
        /// The w m_ ncmbuttonup.
        /// </summary>
        WM_NCMBUTTONUP = 0x00A8, 

        /// <summary>
        /// The w m_ ncmousemove.
        /// </summary>
        WM_NCMOUSEMOVE = 0x00A0, 

        /// <summary>
        /// The w m_ ncpaint.
        /// </summary>
        WM_NCPAINT = 0x0085, 

        /// <summary>
        /// The w m_ ncrbuttondblclk.
        /// </summary>
        WM_NCRBUTTONDBLCLK = 0x00A6, 

        /// <summary>
        /// The w m_ ncrbuttondown.
        /// </summary>
        WM_NCRBUTTONDOWN = 0x00A4, 

        /// <summary>
        /// The w m_ ncrbuttonup.
        /// </summary>
        WM_NCRBUTTONUP = 0x00A5, 

        /// <summary>
        /// The w m_ nextdlgctl.
        /// </summary>
        WM_NEXTDLGCTL = 0x0028, 

        /// <summary>
        /// The w m_ nextmenu.
        /// </summary>
        WM_NEXTMENU = 0x0213, 

        /// <summary>
        /// The w m_ notify.
        /// </summary>
        WM_NOTIFY = 0x004E, 

        /// <summary>
        /// The w m_ notifyformat.
        /// </summary>
        WM_NOTIFYFORMAT = 0x0055, 

        /// <summary>
        /// The w m_ null.
        /// </summary>
        WM_NULL = 0x0000, 

        /// <summary>
        /// The w m_ paint.
        /// </summary>
        WM_PAINT = 0x000F, 

        /// <summary>
        /// The w m_ paintclipboard.
        /// </summary>
        WM_PAINTCLIPBOARD = 0x0309, 

        /// <summary>
        /// The w m_ painticon.
        /// </summary>
        WM_PAINTICON = 0x0026, 

        /// <summary>
        /// The w m_ palettechanged.
        /// </summary>
        WM_PALETTECHANGED = 0x0311, 

        /// <summary>
        /// The w m_ paletteischanging.
        /// </summary>
        WM_PALETTEISCHANGING = 0x0310, 

        /// <summary>
        /// The w m_ parentnotify.
        /// </summary>
        WM_PARENTNOTIFY = 0x0210, 

        /// <summary>
        /// The w m_ paste.
        /// </summary>
        WM_PASTE = 0x0302, 

        /// <summary>
        /// The w m_ penwinfirst.
        /// </summary>
        WM_PENWINFIRST = 0x0380, 

        /// <summary>
        /// The w m_ penwinlast.
        /// </summary>
        WM_PENWINLAST = 0x038F, 

        /// <summary>
        /// The w m_ power.
        /// </summary>
        WM_POWER = 0x0048, 

        /// <summary>
        /// The w m_ print.
        /// </summary>
        WM_PRINT = 0x0317, 

        /// <summary>
        /// The w m_ printclient.
        /// </summary>
        WM_PRINTCLIENT = 0x0318, 

        /// <summary>
        /// The w m_ querydragicon.
        /// </summary>
        WM_QUERYDRAGICON = 0x0037, 

        /// <summary>
        /// The w m_ queryendsession.
        /// </summary>
        WM_QUERYENDSESSION = 0x0011, 

        /// <summary>
        /// The w m_ querynewpalette.
        /// </summary>
        WM_QUERYNEWPALETTE = 0x030F, 

        /// <summary>
        /// The w m_ queryopen.
        /// </summary>
        WM_QUERYOPEN = 0x0013, 

        /// <summary>
        /// The w m_ queuesync.
        /// </summary>
        WM_QUEUESYNC = 0x0023, 

        /// <summary>
        /// The w m_ quit.
        /// </summary>
        WM_QUIT = 0x0012, 

        /// <summary>
        /// The w m_ rbuttondblclk.
        /// </summary>
        WM_RBUTTONDBLCLK = 0x0206, 

        /// <summary>
        /// The w m_ rbuttondown.
        /// </summary>
        WM_RBUTTONDOWN = 0x0204, 

        /// <summary>
        /// The w m_ rbuttonup.
        /// </summary>
        WM_RBUTTONUP = 0x0205, 

        /// <summary>
        /// The w m_ reflect.
        /// </summary>
        WM_REFLECT = WM_USER + 0x1c00, 

        /// <summary>
        /// The w m_ reflec t_ notify.
        /// </summary>
        WM_REFLECT_NOTIFY = 0x204E, 

        /// <summary>
        /// The w m_ renderallformats.
        /// </summary>
        WM_RENDERALLFORMATS = 0x0306, 

        /// <summary>
        /// The w m_ renderformat.
        /// </summary>
        WM_RENDERFORMAT = 0x0305, 

        /// <summary>
        /// The w m_ setcursor.
        /// </summary>
        WM_SETCURSOR = 0x0020, 

        /// <summary>
        /// The w m_ setfocus.
        /// </summary>
        WM_SETFOCUS = 0x0007, 

        /// <summary>
        /// The w m_ setfont.
        /// </summary>
        WM_SETFONT = 0x0030, 

        /// <summary>
        /// The w m_ sethotkey.
        /// </summary>
        WM_SETHOTKEY = 0x0032, 

        /// <summary>
        /// The w m_ seticon.
        /// </summary>
        WM_SETICON = 0x0080, 

        /// <summary>
        /// The w m_ setredraw.
        /// </summary>
        WM_SETREDRAW = 0x000B, 

        /// <summary>
        /// The w m_ settext.
        /// </summary>
        WM_SETTEXT = 0x000C, 

        /// <summary>
        /// The w m_ settingchange.
        /// </summary>
        WM_SETTINGCHANGE = 0x001A, 

        /// <summary>
        /// The w m_ showwindow.
        /// </summary>
        WM_SHOWWINDOW = 0x0018, 

        /// <summary>
        /// The w m_ size.
        /// </summary>
        WM_SIZE = 0x0005, 

        /// <summary>
        /// The w m_ sizeclipboard.
        /// </summary>
        WM_SIZECLIPBOARD = 0x030B, 

        /// <summary>
        /// The w m_ sizing.
        /// </summary>
        WM_SIZING = 0x0214, 

        /// <summary>
        /// The w m_ spoolerstatus.
        /// </summary>
        WM_SPOOLERSTATUS = 0x002A, 

        /// <summary>
        /// The w m_ stylechanged.
        /// </summary>
        WM_STYLECHANGED = 0x007D, 

        /// <summary>
        /// The w m_ stylechanging.
        /// </summary>
        WM_STYLECHANGING = 0x007C, 

        /// <summary>
        /// The w m_ syncpaint.
        /// </summary>
        WM_SYNCPAINT = 0x0088, 

        /// <summary>
        /// The w m_ syschar.
        /// </summary>
        WM_SYSCHAR = 0x0106, 

        /// <summary>
        /// The w m_ syscolorchange.
        /// </summary>
        WM_SYSCOLORCHANGE = 0x0015, 

        /// <summary>
        /// The w m_ syscommand.
        /// </summary>
        WM_SYSCOMMAND = 0x0112, 

        /// <summary>
        /// The w m_ sysdeadchar.
        /// </summary>
        WM_SYSDEADCHAR = 0x0107, 

        /// <summary>
        /// The w m_ syskeydown.
        /// </summary>
        WM_SYSKEYDOWN = 0x0104, 

        /// <summary>
        /// The w m_ syskeyup.
        /// </summary>
        WM_SYSKEYUP = 0x0105, 

        /// <summary>
        /// The w m_ tcard.
        /// </summary>
        WM_TCARD = 0x0052, 

        /// <summary>
        /// The w m_ timechange.
        /// </summary>
        WM_TIMECHANGE = 0x001E, 

        /// <summary>
        /// The w m_ timer.
        /// </summary>
        WM_TIMER = 0x0113, 

        /// <summary>
        /// The w m_ undo.
        /// </summary>
        WM_UNDO = 0x0304, 

        /// <summary>
        /// The w m_ uninitmenupopup.
        /// </summary>
        WM_UNINITMENUPOPUP = 0x0125, 

        /// <summary>
        /// The w m_ user.
        /// </summary>
        WM_USER = 0x0400, 

        /// <summary>
        /// The w m_ userchanged.
        /// </summary>
        WM_USERCHANGED = 0x0054, 

        /// <summary>
        /// The w m_ vkeytoitem.
        /// </summary>
        WM_VKEYTOITEM = 0x002E, 

        /// <summary>
        /// The w m_ vscroll.
        /// </summary>
        WM_VSCROLL = 0x0115, 

        /// <summary>
        /// The w m_ vscrollclipboard.
        /// </summary>
        WM_VSCROLLCLIPBOARD = 0x030A, 

        /// <summary>
        /// The w m_ windowposchanged.
        /// </summary>
        WM_WINDOWPOSCHANGED = 0x0047, 

        /// <summary>
        /// The w m_ windowposchanging.
        /// </summary>
        WM_WINDOWPOSCHANGING = 0x0046, 

        /// <summary>
        /// The w m_ wininichange.
        /// </summary>
        WM_WININICHANGE = 0x001A
    }

    /// <summary>Notify message codes NM_</summary>
    public enum W32_NM
    {
        /// <summary>
        /// The n m_ first.
        /// </summary>
        NM_FIRST = BaseCodes.NM_FIRST, 

        /// <summary>
        /// The n m_ outofmemory.
        /// </summary>
        NM_OUTOFMEMORY = NM_FIRST - 1, 

        /// <summary>
        /// The n m_ click.
        /// </summary>
        NM_CLICK = NM_FIRST - 2, 

        /// <summary>
        /// The n m_ dblclk.
        /// </summary>
        NM_DBLCLK = NM_FIRST - 3, 

        /// <summary>
        /// The n m_ return.
        /// </summary>
        NM_RETURN = NM_FIRST - 4, 

        /// <summary>
        /// The n m_ rclick.
        /// </summary>
        NM_RCLICK = NM_FIRST - 5, 

        /// <summary>
        /// The n m_ rdblclk.
        /// </summary>
        NM_RDBLCLK = NM_FIRST - 6, 

        /// <summary>
        /// The n m_ setfocus.
        /// </summary>
        NM_SETFOCUS = NM_FIRST - 7, 

        /// <summary>
        /// The n m_ killfocus.
        /// </summary>
        NM_KILLFOCUS = NM_FIRST - 8, 

        /// <summary>
        /// The n m_ customdraw.
        /// </summary>
        NM_CUSTOMDRAW = NM_FIRST - 12, 

        /// <summary>
        /// The n m_ hover.
        /// </summary>
        NM_HOVER = NM_FIRST - 13, 

        /// <summary>
        /// The n m_ nchittest.
        /// </summary>
        NM_NCHITTEST = NM_FIRST - 14, 

        /// <summary>
        /// The n m_ keydown.
        /// </summary>
        NM_KEYDOWN = NM_FIRST - 15, 

        /// <summary>
        /// The n m_ releasedcapture.
        /// </summary>
        NM_RELEASEDCAPTURE = NM_FIRST - 16, 

        /// <summary>
        /// The n m_ setcursor.
        /// </summary>
        NM_SETCURSOR = NM_FIRST - 17, 

        /// <summary>
        /// The n m_ char.
        /// </summary>
        NM_CHAR = NM_FIRST - 18, 

        /// <summary>
        /// The n m_ tooltipscreated.
        /// </summary>
        NM_TOOLTIPSCREATED = NM_FIRST - 19, 

        /// <summary>
        /// The n m_ ldown.
        /// </summary>
        NM_LDOWN = NM_FIRST - 20, 

        /// <summary>
        /// The n m_ rdown.
        /// </summary>
        NM_RDOWN = NM_FIRST - 21, 

        /// <summary>
        /// The n m_ themechanged.
        /// </summary>
        NM_THEMECHANGED = NM_FIRST - 22
    }

    /// <summary>Reflected message codes OCM_</summary>
    public enum W32_OCM
    {
        /// <summary>
        /// The oc m__ base.
        /// </summary>
        OCM__BASE = W32_WM.WM_REFLECT, 

        /// <summary>
        /// The oc m_ command.
        /// </summary>
        OCM_COMMAND = OCM__BASE + W32_WM.WM_COMMAND, 

        /// <summary>
        /// The oc m_ ctlcolorbtn.
        /// </summary>
        OCM_CTLCOLORBTN = OCM__BASE + W32_WM.WM_CTLCOLORBTN, 

        /// <summary>
        /// The oc m_ ctlcoloredit.
        /// </summary>
        OCM_CTLCOLOREDIT = OCM__BASE + W32_WM.WM_CTLCOLOREDIT, 

        /// <summary>
        /// The oc m_ ctlcolordlg.
        /// </summary>
        OCM_CTLCOLORDLG = OCM__BASE + W32_WM.WM_CTLCOLORDLG, 

        /// <summary>
        /// The oc m_ ctlcolorlistbox.
        /// </summary>
        OCM_CTLCOLORLISTBOX = OCM__BASE + W32_WM.WM_CTLCOLORLISTBOX, 

        /// <summary>
        /// The oc m_ ctlcolormsgbox.
        /// </summary>
        OCM_CTLCOLORMSGBOX = OCM__BASE + W32_WM.WM_CTLCOLORMSGBOX, 

        /// <summary>
        /// The oc m_ ctlcolorscrollbar.
        /// </summary>
        OCM_CTLCOLORSCROLLBAR = OCM__BASE + W32_WM.WM_CTLCOLORSCROLLBAR, 

        /// <summary>
        /// The oc m_ ctlcolorstatic.
        /// </summary>
        OCM_CTLCOLORSTATIC = OCM__BASE + W32_WM.WM_CTLCOLORSTATIC, 

        /// <summary>
        /// The oc m_ ctlcolor.
        /// </summary>
        OCM_CTLCOLOR = OCM__BASE + W32_WM.WM_CTLCOLOR, 

        /// <summary>
        /// The oc m_ drawitem.
        /// </summary>
        OCM_DRAWITEM = OCM__BASE + W32_WM.WM_DRAWITEM, 

        /// <summary>
        /// The oc m_ measureitem.
        /// </summary>
        OCM_MEASUREITEM = OCM__BASE + W32_WM.WM_MEASUREITEM, 

        /// <summary>
        /// The oc m_ deleteitem.
        /// </summary>
        OCM_DELETEITEM = OCM__BASE + W32_WM.WM_DELETEITEM, 

        /// <summary>
        /// The oc m_ vkeytoitem.
        /// </summary>
        OCM_VKEYTOITEM = OCM__BASE + W32_WM.WM_VKEYTOITEM, 

        /// <summary>
        /// The oc m_ chartoitem.
        /// </summary>
        OCM_CHARTOITEM = OCM__BASE + W32_WM.WM_CHARTOITEM, 

        /// <summary>
        /// The oc m_ compareitem.
        /// </summary>
        OCM_COMPAREITEM = OCM__BASE + W32_WM.WM_COMPAREITEM, 

        /// <summary>
        /// The oc m_ hscroll.
        /// </summary>
        OCM_HSCROLL = OCM__BASE + W32_WM.WM_HSCROLL, 

        /// <summary>
        /// The oc m_ vscroll.
        /// </summary>
        OCM_VSCROLL = OCM__BASE + W32_WM.WM_VSCROLL, 

        /// <summary>
        /// The oc m_ parentnotify.
        /// </summary>
        OCM_PARENTNOTIFY = OCM__BASE + W32_WM.WM_PARENTNOTIFY, 

        /// <summary>
        /// The oc m_ notify.
        /// </summary>
        OCM_NOTIFY = OCM__BASE + W32_WM.WM_NOTIFY
    }

    /// <summary>ListView message codes LVM_</summary>
    public enum W32_LVM
    {
        /// <summary>
        /// The lv m_ first.
        /// </summary>
        LVM_FIRST = BaseCodes.LVM_FIRST, 

        /// <summary>
        /// The lv m_ getbkcolor.
        /// </summary>
        LVM_GETBKCOLOR = LVM_FIRST + 0, 

        /// <summary>
        /// The lv m_ setbkcolor.
        /// </summary>
        LVM_SETBKCOLOR = LVM_FIRST + 1, 

        /// <summary>
        /// The lv m_ getimagelist.
        /// </summary>
        LVM_GETIMAGELIST = LVM_FIRST + 2, 

        /// <summary>
        /// The lv m_ setimagelist.
        /// </summary>
        LVM_SETIMAGELIST = LVM_FIRST + 3, 

        /// <summary>
        /// The lv m_ getitemcount.
        /// </summary>
        LVM_GETITEMCOUNT = LVM_FIRST + 4, 

        /// <summary>
        /// The lv m_ getitema.
        /// </summary>
        LVM_GETITEMA = LVM_FIRST + 5, 

        /// <summary>
        /// The lv m_ getitemw.
        /// </summary>
        LVM_GETITEMW = LVM_FIRST + 75, 

        /// <summary>
        /// The lv m_ setitema.
        /// </summary>
        LVM_SETITEMA = LVM_FIRST + 6, 

        /// <summary>
        /// The lv m_ setitemw.
        /// </summary>
        LVM_SETITEMW = LVM_FIRST + 76, 

        /// <summary>
        /// The lv m_ insertitema.
        /// </summary>
        LVM_INSERTITEMA = LVM_FIRST + 7, 

        /// <summary>
        /// The lv m_ insertitemw.
        /// </summary>
        LVM_INSERTITEMW = LVM_FIRST + 77, 

        /// <summary>
        /// The lv m_ deleteitem.
        /// </summary>
        LVM_DELETEITEM = LVM_FIRST + 8, 

        /// <summary>
        /// The lv m_ deleteallitems.
        /// </summary>
        LVM_DELETEALLITEMS = LVM_FIRST + 9, 

        /// <summary>
        /// The lv m_ getcallbackmask.
        /// </summary>
        LVM_GETCALLBACKMASK = LVM_FIRST + 10, 

        /// <summary>
        /// The lv m_ setcallbackmask.
        /// </summary>
        LVM_SETCALLBACKMASK = LVM_FIRST + 11, 

        /// <summary>
        /// The lv m_ getnextitem.
        /// </summary>
        LVM_GETNEXTITEM = LVM_FIRST + 12, 

        /// <summary>
        /// The lv m_ finditema.
        /// </summary>
        LVM_FINDITEMA = LVM_FIRST + 13, 

        /// <summary>
        /// The lv m_ finditemw.
        /// </summary>
        LVM_FINDITEMW = LVM_FIRST + 83, 

        /// <summary>
        /// The lv m_ getitemrect.
        /// </summary>
        LVM_GETITEMRECT = LVM_FIRST + 14, 

        /// <summary>
        /// The lv m_ setitemposition.
        /// </summary>
        LVM_SETITEMPOSITION = LVM_FIRST + 15, 

        /// <summary>
        /// The lv m_ getitemposition.
        /// </summary>
        LVM_GETITEMPOSITION = LVM_FIRST + 16, 

        /// <summary>
        /// The lv m_ getstringwidtha.
        /// </summary>
        LVM_GETSTRINGWIDTHA = LVM_FIRST + 17, 

        /// <summary>
        /// The lv m_ getstringwidthw.
        /// </summary>
        LVM_GETSTRINGWIDTHW = LVM_FIRST + 87, 

        /// <summary>
        /// The lv m_ hittest.
        /// </summary>
        LVM_HITTEST = LVM_FIRST + 18, 

        /// <summary>
        /// The lv m_ ensurevisible.
        /// </summary>
        LVM_ENSUREVISIBLE = LVM_FIRST + 19, 

        /// <summary>
        /// The lv m_ scroll.
        /// </summary>
        LVM_SCROLL = LVM_FIRST + 20, 

        /// <summary>
        /// The lv m_ redrawitems.
        /// </summary>
        LVM_REDRAWITEMS = LVM_FIRST + 21, 

        /// <summary>
        /// The lv m_ arrange.
        /// </summary>
        LVM_ARRANGE = LVM_FIRST + 22, 

        /// <summary>
        /// The lv m_ editlabela.
        /// </summary>
        LVM_EDITLABELA = LVM_FIRST + 23, 

        /// <summary>
        /// The lv m_ editlabelw.
        /// </summary>
        LVM_EDITLABELW = LVM_FIRST + 118, 

        /// <summary>
        /// The lv m_ geteditcontrol.
        /// </summary>
        LVM_GETEDITCONTROL = LVM_FIRST + 24, 

        /// <summary>
        /// The lv m_ getcolumna.
        /// </summary>
        LVM_GETCOLUMNA = LVM_FIRST + 25, 

        /// <summary>
        /// The lv m_ getcolumnw.
        /// </summary>
        LVM_GETCOLUMNW = LVM_FIRST + 95, 

        /// <summary>
        /// The lv m_ setcolumna.
        /// </summary>
        LVM_SETCOLUMNA = LVM_FIRST + 26, 

        /// <summary>
        /// The lv m_ setcolumnw.
        /// </summary>
        LVM_SETCOLUMNW = LVM_FIRST + 96, 

        /// <summary>
        /// The lv m_ insertcolumna.
        /// </summary>
        LVM_INSERTCOLUMNA = LVM_FIRST + 27, 

        /// <summary>
        /// The lv m_ insertcolumnw.
        /// </summary>
        LVM_INSERTCOLUMNW = LVM_FIRST + 97, 

        /// <summary>
        /// The lv m_ deletecolumn.
        /// </summary>
        LVM_DELETECOLUMN = LVM_FIRST + 28, 

        /// <summary>
        /// The lv m_ getcolumnwidth.
        /// </summary>
        LVM_GETCOLUMNWIDTH = LVM_FIRST + 29, 

        /// <summary>
        /// The lv m_ setcolumnwidth.
        /// </summary>
        LVM_SETCOLUMNWIDTH = LVM_FIRST + 30, 

        /// <summary>
        /// The lv m_ getheader.
        /// </summary>
        LVM_GETHEADER = LVM_FIRST + 31, 

        /// <summary>
        /// The lv m_ createdragimage.
        /// </summary>
        LVM_CREATEDRAGIMAGE = LVM_FIRST + 33, 

        /// <summary>
        /// The lv m_ getviewrect.
        /// </summary>
        LVM_GETVIEWRECT = LVM_FIRST + 34, 

        /// <summary>
        /// The lv m_ gettextcolor.
        /// </summary>
        LVM_GETTEXTCOLOR = LVM_FIRST + 35, 

        /// <summary>
        /// The lv m_ settextcolor.
        /// </summary>
        LVM_SETTEXTCOLOR = LVM_FIRST + 36, 

        /// <summary>
        /// The lv m_ gettextbkcolor.
        /// </summary>
        LVM_GETTEXTBKCOLOR = LVM_FIRST + 37, 

        /// <summary>
        /// The lv m_ settextbkcolor.
        /// </summary>
        LVM_SETTEXTBKCOLOR = LVM_FIRST + 38, 

        /// <summary>
        /// The lv m_ gettopindex.
        /// </summary>
        LVM_GETTOPINDEX = LVM_FIRST + 39, 

        /// <summary>
        /// The lv m_ getcountperpage.
        /// </summary>
        LVM_GETCOUNTPERPAGE = LVM_FIRST + 40, 

        /// <summary>
        /// The lv m_ getorigin.
        /// </summary>
        LVM_GETORIGIN = LVM_FIRST + 41, 

        /// <summary>
        /// The lv m_ update.
        /// </summary>
        LVM_UPDATE = LVM_FIRST + 42, 

        /// <summary>
        /// The lv m_ setitemstate.
        /// </summary>
        LVM_SETITEMSTATE = LVM_FIRST + 43, 

        /// <summary>
        /// The lv m_ getitemstate.
        /// </summary>
        LVM_GETITEMSTATE = LVM_FIRST + 44, 

        /// <summary>
        /// The lv m_ getitemtexta.
        /// </summary>
        LVM_GETITEMTEXTA = LVM_FIRST + 45, 

        /// <summary>
        /// The lv m_ getitemtextw.
        /// </summary>
        LVM_GETITEMTEXTW = LVM_FIRST + 115, 

        /// <summary>
        /// The lv m_ setitemtexta.
        /// </summary>
        LVM_SETITEMTEXTA = LVM_FIRST + 46, 

        /// <summary>
        /// The lv m_ setitemtextw.
        /// </summary>
        LVM_SETITEMTEXTW = LVM_FIRST + 116, 

        /// <summary>
        /// The lv m_ setitemcount.
        /// </summary>
        LVM_SETITEMCOUNT = LVM_FIRST + 47, 

        /// <summary>
        /// The lv m_ sortitems.
        /// </summary>
        LVM_SORTITEMS = LVM_FIRST + 48, 

        /// <summary>
        /// The lv m_ setitempositio n 32.
        /// </summary>
        LVM_SETITEMPOSITION32 = LVM_FIRST + 49, 

        /// <summary>
        /// The lv m_ getselectedcount.
        /// </summary>
        LVM_GETSELECTEDCOUNT = LVM_FIRST + 50, 

        /// <summary>
        /// The lv m_ getitemspacing.
        /// </summary>
        LVM_GETITEMSPACING = LVM_FIRST + 51, 

        /// <summary>
        /// The lv m_ getisearchstringa.
        /// </summary>
        LVM_GETISEARCHSTRINGA = LVM_FIRST + 52, 

        /// <summary>
        /// The lv m_ getisearchstringw.
        /// </summary>
        LVM_GETISEARCHSTRINGW = LVM_FIRST + 117, 

        /// <summary>
        /// The lv m_ seticonspacing.
        /// </summary>
        LVM_SETICONSPACING = LVM_FIRST + 53, 

        /// <summary>
        /// The lv m_ setextendedlistviewstyle.
        /// </summary>
        LVM_SETEXTENDEDLISTVIEWSTYLE = LVM_FIRST + 54, 

        /// <summary>
        /// The lv m_ getextendedlistviewstyle.
        /// </summary>
        LVM_GETEXTENDEDLISTVIEWSTYLE = LVM_FIRST + 55, 

        /// <summary>
        /// The lv m_ getsubitemrect.
        /// </summary>
        LVM_GETSUBITEMRECT = LVM_FIRST + 56, 

        /// <summary>
        /// The lv m_ subitemhittest.
        /// </summary>
        LVM_SUBITEMHITTEST = LVM_FIRST + 57, 

        /// <summary>
        /// The lv m_ setcolumnorderarray.
        /// </summary>
        LVM_SETCOLUMNORDERARRAY = LVM_FIRST + 58, 

        /// <summary>
        /// The lv m_ getcolumnorderarray.
        /// </summary>
        LVM_GETCOLUMNORDERARRAY = LVM_FIRST + 59, 

        /// <summary>
        /// The lv m_ sethotitem.
        /// </summary>
        LVM_SETHOTITEM = LVM_FIRST + 60, 

        /// <summary>
        /// The lv m_ gethotitem.
        /// </summary>
        LVM_GETHOTITEM = LVM_FIRST + 61, 

        /// <summary>
        /// The lv m_ sethotcursor.
        /// </summary>
        LVM_SETHOTCURSOR = LVM_FIRST + 62, 

        /// <summary>
        /// The lv m_ gethotcursor.
        /// </summary>
        LVM_GETHOTCURSOR = LVM_FIRST + 63, 

        /// <summary>
        /// The lv m_ approximateviewrect.
        /// </summary>
        LVM_APPROXIMATEVIEWRECT = LVM_FIRST + 64, 

        /// <summary>
        /// The lv m_ setworkareas.
        /// </summary>
        LVM_SETWORKAREAS = LVM_FIRST + 65, 

        /// <summary>
        /// The lv m_ getworkareas.
        /// </summary>
        LVM_GETWORKAREAS = LVM_FIRST + 70, 

        /// <summary>
        /// The lv m_ getnumberofworkareas.
        /// </summary>
        LVM_GETNUMBEROFWORKAREAS = LVM_FIRST + 73, 

        /// <summary>
        /// The lv m_ getselectionmark.
        /// </summary>
        LVM_GETSELECTIONMARK = LVM_FIRST + 66, 

        /// <summary>
        /// The lv m_ setselectionmark.
        /// </summary>
        LVM_SETSELECTIONMARK = LVM_FIRST + 67, 

        /// <summary>
        /// The lv m_ sethovertime.
        /// </summary>
        LVM_SETHOVERTIME = LVM_FIRST + 71, 

        /// <summary>
        /// The lv m_ gethovertime.
        /// </summary>
        LVM_GETHOVERTIME = LVM_FIRST + 72, 

        /// <summary>
        /// The lv m_ settooltips.
        /// </summary>
        LVM_SETTOOLTIPS = LVM_FIRST + 74, 

        /// <summary>
        /// The lv m_ gettooltips.
        /// </summary>
        LVM_GETTOOLTIPS = LVM_FIRST + 78, 

        /// <summary>
        /// The lv m_ sortitemsex.
        /// </summary>
        LVM_SORTITEMSEX = LVM_FIRST + 81, 

        /// <summary>
        /// The lv m_ setbkimagea.
        /// </summary>
        LVM_SETBKIMAGEA = LVM_FIRST + 68, 

        /// <summary>
        /// The lv m_ setbkimagew.
        /// </summary>
        LVM_SETBKIMAGEW = LVM_FIRST + 138, 

        /// <summary>
        /// The lv m_ getbkimagea.
        /// </summary>
        LVM_GETBKIMAGEA = LVM_FIRST + 69, 

        /// <summary>
        /// The lv m_ getbkimagew.
        /// </summary>
        LVM_GETBKIMAGEW = LVM_FIRST + 139, 

        /// <summary>
        /// The lv m_ setselectedcolumn.
        /// </summary>
        LVM_SETSELECTEDCOLUMN = LVM_FIRST + 140, 

        /// <summary>
        /// The lv m_ settilewidth.
        /// </summary>
        LVM_SETTILEWIDTH = LVM_FIRST + 141, 

        /// <summary>
        /// The lv m_ setview.
        /// </summary>
        LVM_SETVIEW = LVM_FIRST + 142, 

        /// <summary>
        /// The lv m_ getview.
        /// </summary>
        LVM_GETVIEW = LVM_FIRST + 143, 

        /// <summary>
        /// The lv m_ insertgroup.
        /// </summary>
        LVM_INSERTGROUP = LVM_FIRST + 145, 

        /// <summary>
        /// The lv m_ setgroupinfo.
        /// </summary>
        LVM_SETGROUPINFO = LVM_FIRST + 147, 

        /// <summary>
        /// The lv m_ getgroupinfo.
        /// </summary>
        LVM_GETGROUPINFO = LVM_FIRST + 149, 

        /// <summary>
        /// The lv m_ removegroup.
        /// </summary>
        LVM_REMOVEGROUP = LVM_FIRST + 150, 

        /// <summary>
        /// The lv m_ movegroup.
        /// </summary>
        LVM_MOVEGROUP = LVM_FIRST + 151, 

        /// <summary>
        /// The lv m_ moveitemtogroup.
        /// </summary>
        LVM_MOVEITEMTOGROUP = LVM_FIRST + 154, 

        /// <summary>
        /// The lv m_ setgroupmetrics.
        /// </summary>
        LVM_SETGROUPMETRICS = LVM_FIRST + 155, 

        /// <summary>
        /// The lv m_ getgroupmetrics.
        /// </summary>
        LVM_GETGROUPMETRICS = LVM_FIRST + 156, 

        /// <summary>
        /// The lv m_ enablegroupview.
        /// </summary>
        LVM_ENABLEGROUPVIEW = LVM_FIRST + 157, 

        /// <summary>
        /// The lv m_ sortgroups.
        /// </summary>
        LVM_SORTGROUPS = LVM_FIRST + 158, 

        /// <summary>
        /// The lv m_ insertgroupsorted.
        /// </summary>
        LVM_INSERTGROUPSORTED = LVM_FIRST + 159, 

        /// <summary>
        /// The lv m_ removeallgroups.
        /// </summary>
        LVM_REMOVEALLGROUPS = LVM_FIRST + 160, 

        /// <summary>
        /// The lv m_ hasgroup.
        /// </summary>
        LVM_HASGROUP = LVM_FIRST + 161, 

        /// <summary>
        /// The lv m_ settileviewinfo.
        /// </summary>
        LVM_SETTILEVIEWINFO = LVM_FIRST + 162, 

        /// <summary>
        /// The lv m_ gettileviewinfo.
        /// </summary>
        LVM_GETTILEVIEWINFO = LVM_FIRST + 163, 

        /// <summary>
        /// The lv m_ settileinfo.
        /// </summary>
        LVM_SETTILEINFO = LVM_FIRST + 164, 

        /// <summary>
        /// The lv m_ gettileinfo.
        /// </summary>
        LVM_GETTILEINFO = LVM_FIRST + 165, 

        /// <summary>
        /// The lv m_ setinsertmark.
        /// </summary>
        LVM_SETINSERTMARK = LVM_FIRST + 166, 

        /// <summary>
        /// The lv m_ getinsertmark.
        /// </summary>
        LVM_GETINSERTMARK = LVM_FIRST + 167, 

        /// <summary>
        /// The lv m_ insertmarkhittest.
        /// </summary>
        LVM_INSERTMARKHITTEST = LVM_FIRST + 168, 

        /// <summary>
        /// The lv m_ getinsertmarkrect.
        /// </summary>
        LVM_GETINSERTMARKRECT = LVM_FIRST + 169, 

        /// <summary>
        /// The lv m_ setinsertmarkcolor.
        /// </summary>
        LVM_SETINSERTMARKCOLOR = LVM_FIRST + 170, 

        /// <summary>
        /// The lv m_ getinsertmarkcolor.
        /// </summary>
        LVM_GETINSERTMARKCOLOR = LVM_FIRST + 171, 

        /// <summary>
        /// The lv m_ setinfotip.
        /// </summary>
        LVM_SETINFOTIP = LVM_FIRST + 173, 

        /// <summary>
        /// The lv m_ getselectedcolumn.
        /// </summary>
        LVM_GETSELECTEDCOLUMN = LVM_FIRST + 174, 

        /// <summary>
        /// The lv m_ isgroupviewenabled.
        /// </summary>
        LVM_ISGROUPVIEWENABLED = LVM_FIRST + 175, 

        /// <summary>
        /// The lv m_ getoutlinecolor.
        /// </summary>
        LVM_GETOUTLINECOLOR = LVM_FIRST + 176, 

        /// <summary>
        /// The lv m_ setoutlinecolor.
        /// </summary>
        LVM_SETOUTLINECOLOR = LVM_FIRST + 177, 

        /// <summary>
        /// The lv m_ canceleditlabel.
        /// </summary>
        LVM_CANCELEDITLABEL = LVM_FIRST + 179, 

        /// <summary>
        /// The lv m_ mapindextoid.
        /// </summary>
        LVM_MAPINDEXTOID = LVM_FIRST + 180, 

        /// <summary>
        /// The lv m_ mapidtoindex.
        /// </summary>
        LVM_MAPIDTOINDEX = LVM_FIRST + 181
    }

    /// <summary>ListView notification message codes LVN_</summary>
    public enum W32_LVN
    {
        /// <summary>
        /// The lv n_ first.
        /// </summary>
        LVN_FIRST = BaseCodes.LVN_FIRST, 

        /// <summary>
        /// The lv n_ itemchanging.
        /// </summary>
        LVN_ITEMCHANGING = LVN_FIRST - 0, 

        /// <summary>
        /// The lv n_ itemchanged.
        /// </summary>
        LVN_ITEMCHANGED = LVN_FIRST - 1, 

        /// <summary>
        /// The lv n_ insertitem.
        /// </summary>
        LVN_INSERTITEM = LVN_FIRST - 2, 

        /// <summary>
        /// The lv n_ deleteitem.
        /// </summary>
        LVN_DELETEITEM = LVN_FIRST - 3, 

        /// <summary>
        /// The lv n_ deleteallitems.
        /// </summary>
        LVN_DELETEALLITEMS = LVN_FIRST - 4, 

        /// <summary>
        /// The lv n_ beginlabeledita.
        /// </summary>
        LVN_BEGINLABELEDITA = LVN_FIRST - 5, 

        /// <summary>
        /// The lv n_ endlabeledita.
        /// </summary>
        LVN_ENDLABELEDITA = LVN_FIRST - 6, 

        /// <summary>
        /// The lv n_ columnclick.
        /// </summary>
        LVN_COLUMNCLICK = LVN_FIRST - 8, 

        /// <summary>
        /// The lv n_ begindrag.
        /// </summary>
        LVN_BEGINDRAG = LVN_FIRST - 9, 

        /// <summary>
        /// The lv n_ beginrdrag.
        /// </summary>
        LVN_BEGINRDRAG = LVN_FIRST - 11, 

        /// <summary>
        /// The lv n_ odcachehint.
        /// </summary>
        LVN_ODCACHEHINT = LVN_FIRST - 13, 

        /// <summary>
        /// The lv n_ itemactivate.
        /// </summary>
        LVN_ITEMACTIVATE = LVN_FIRST - 14, 

        /// <summary>
        /// The lv n_ odstatechanged.
        /// </summary>
        LVN_ODSTATECHANGED = LVN_FIRST - 15, 

        /// <summary>
        /// The lv n_ hottrack.
        /// </summary>
        LVN_HOTTRACK = LVN_FIRST - 21, 

        /// <summary>
        /// The lv n_ getdispinfoa.
        /// </summary>
        LVN_GETDISPINFOA = LVN_FIRST - 50, 

        /// <summary>
        /// The lv n_ setdispinfoa.
        /// </summary>
        LVN_SETDISPINFOA = LVN_FIRST - 51, 

        /// <summary>
        /// The lv n_ odfinditema.
        /// </summary>
        LVN_ODFINDITEMA = LVN_FIRST - 52, 

        /// <summary>
        /// The lv n_ keydown.
        /// </summary>
        LVN_KEYDOWN = LVN_FIRST - 55, 

        /// <summary>
        /// The lv n_ marqueebegin.
        /// </summary>
        LVN_MARQUEEBEGIN = LVN_FIRST - 56, 

        /// <summary>
        /// The lv n_ getinfotipa.
        /// </summary>
        LVN_GETINFOTIPA = LVN_FIRST - 57, 

        /// <summary>
        /// The lv n_ getinfotipw.
        /// </summary>
        LVN_GETINFOTIPW = LVN_FIRST - 58, 

        /// <summary>
        /// The lv n_ beginlabeleditw.
        /// </summary>
        LVN_BEGINLABELEDITW = LVN_FIRST - 75, 

        /// <summary>
        /// The lv n_ endlabeleditw.
        /// </summary>
        LVN_ENDLABELEDITW = LVN_FIRST - 76, 

        /// <summary>
        /// The lv n_ getdispinfow.
        /// </summary>
        LVN_GETDISPINFOW = LVN_FIRST - 77, 

        /// <summary>
        /// The lv n_ setdispinfow.
        /// </summary>
        LVN_SETDISPINFOW = LVN_FIRST - 78, 

        /// <summary>
        /// The lv n_ odfinditemw.
        /// </summary>
        LVN_ODFINDITEMW = LVN_FIRST - 79, 

        /// <summary>
        /// The lv n_ beginscroll.
        /// </summary>
        LVN_BEGINSCROLL = LVN_FIRST - 80, 

        /// <summary>
        /// The lv n_ endscroll.
        /// </summary>
        LVN_ENDSCROLL = LVN_FIRST - 81
    }

    /// <summary>Header message codes HDM_</summary>
    public enum W32_HDM
    {
        /// <summary>
        /// The hd m_ first.
        /// </summary>
        HDM_FIRST = BaseCodes.HDM_FIRST, 

        /// <summary>
        /// The hd m_ clearfilter.
        /// </summary>
        HDM_CLEARFILTER = HDM_FIRST + 24, 

        /// <summary>
        /// The hd m_ createdragimage.
        /// </summary>
        HDM_CREATEDRAGIMAGE = HDM_FIRST + 16, 

        /// <summary>
        /// The hd m_ deleteitem.
        /// </summary>
        HDM_DELETEITEM = HDM_FIRST + 2, 

        /// <summary>
        /// The hd m_ editfilter.
        /// </summary>
        HDM_EDITFILTER = HDM_FIRST + 23, 

        /// <summary>
        /// The hd m_ getbitmapmargin.
        /// </summary>
        HDM_GETBITMAPMARGIN = HDM_FIRST + 21, 

        /// <summary>
        /// The hd m_ getimagelist.
        /// </summary>
        HDM_GETIMAGELIST = HDM_FIRST + 9, 

        /// <summary>
        /// The hd m_ getitema.
        /// </summary>
        HDM_GETITEMA = HDM_FIRST + 3, 

        /// <summary>
        /// The hd m_ getitemcount.
        /// </summary>
        HDM_GETITEMCOUNT = HDM_FIRST + 0, 

        /// <summary>
        /// The hd m_ getitemrect.
        /// </summary>
        HDM_GETITEMRECT = HDM_FIRST + 7, 

        /// <summary>
        /// The hd m_ getitemw.
        /// </summary>
        HDM_GETITEMW = HDM_FIRST + 11, 

        /// <summary>
        /// The hd m_ getorderarray.
        /// </summary>
        HDM_GETORDERARRAY = HDM_FIRST + 17, 

        /// <summary>
        /// The hd m_ hittest.
        /// </summary>
        HDM_HITTEST = HDM_FIRST + 6, 

        /// <summary>
        /// The hd m_ insertitema.
        /// </summary>
        HDM_INSERTITEMA = HDM_FIRST + 1, 

        /// <summary>
        /// The hd m_ insertitemw.
        /// </summary>
        HDM_INSERTITEMW = HDM_FIRST + 10, 

        /// <summary>
        /// The hd m_ layout.
        /// </summary>
        HDM_LAYOUT = HDM_FIRST + 5, 

        /// <summary>
        /// The hd m_ ordertoindex.
        /// </summary>
        HDM_ORDERTOINDEX = HDM_FIRST + 15, 

        /// <summary>
        /// The hd m_ setbitmapmargin.
        /// </summary>
        HDM_SETBITMAPMARGIN = HDM_FIRST + 20, 

        /// <summary>
        /// The hd m_ setfilterchangetimeout.
        /// </summary>
        HDM_SETFILTERCHANGETIMEOUT = HDM_FIRST + 22, 

        /// <summary>
        /// The hd m_ sethotdivider.
        /// </summary>
        HDM_SETHOTDIVIDER = HDM_FIRST + 19, 

        /// <summary>
        /// The hd m_ setimagelist.
        /// </summary>
        HDM_SETIMAGELIST = HDM_FIRST + 8, 

        /// <summary>
        /// The hd m_ setitema.
        /// </summary>
        HDM_SETITEMA = HDM_FIRST + 4, 

        /// <summary>
        /// The hd m_ setitemw.
        /// </summary>
        HDM_SETITEMW = HDM_FIRST + 12, 

        /// <summary>
        /// The hd m_ setorderarray.
        /// </summary>
        HDM_SETORDERARRAY = HDM_FIRST + 18
    }

    /// <summary>Header notification message codes HDN_</summary>
    public enum W32_HDN
    {
        /// <summary>
        /// The hd n_ first.
        /// </summary>
        HDN_FIRST = BaseCodes.HDN_FIRST, 

        /// <summary>
        /// The hd n_ itemchanginga.
        /// </summary>
        HDN_ITEMCHANGINGA = HDN_FIRST - 0, 

        /// <summary>
        /// The hd n_ itemchangingw.
        /// </summary>
        HDN_ITEMCHANGINGW = HDN_FIRST - 20, 

        /// <summary>
        /// The hd n_ itemchangeda.
        /// </summary>
        HDN_ITEMCHANGEDA = HDN_FIRST - 1, 

        /// <summary>
        /// The hd n_ itemchangedw.
        /// </summary>
        HDN_ITEMCHANGEDW = HDN_FIRST - 21, 

        /// <summary>
        /// The hd n_ itemclicka.
        /// </summary>
        HDN_ITEMCLICKA = HDN_FIRST - 2, 

        /// <summary>
        /// The hd n_ itemclickw.
        /// </summary>
        HDN_ITEMCLICKW = HDN_FIRST - 22, 

        /// <summary>
        /// The hd n_ itemdblclicka.
        /// </summary>
        HDN_ITEMDBLCLICKA = HDN_FIRST - 3, 

        /// <summary>
        /// The hd n_ itemdblclickw.
        /// </summary>
        HDN_ITEMDBLCLICKW = HDN_FIRST - 23, 

        /// <summary>
        /// The hd n_ dividerdblclicka.
        /// </summary>
        HDN_DIVIDERDBLCLICKA = HDN_FIRST - 5, 

        /// <summary>
        /// The hd n_ dividerdblclickw.
        /// </summary>
        HDN_DIVIDERDBLCLICKW = HDN_FIRST - 25, 

        /// <summary>
        /// The hd n_ begintracka.
        /// </summary>
        HDN_BEGINTRACKA = HDN_FIRST - 6, 

        /// <summary>
        /// The hd n_ begintrackw.
        /// </summary>
        HDN_BEGINTRACKW = HDN_FIRST - 26, 

        /// <summary>
        /// The hd n_ endtracka.
        /// </summary>
        HDN_ENDTRACKA = HDN_FIRST - 7, 

        /// <summary>
        /// The hd n_ endtrackw.
        /// </summary>
        HDN_ENDTRACKW = HDN_FIRST - 27, 

        /// <summary>
        /// The hd n_ tracka.
        /// </summary>
        HDN_TRACKA = HDN_FIRST - 8, 

        /// <summary>
        /// The hd n_ trackw.
        /// </summary>
        HDN_TRACKW = HDN_FIRST - 28, 

        /// <summary>
        /// The hd n_ getdispinfoa.
        /// </summary>
        HDN_GETDISPINFOA = HDN_FIRST - 9, 

        /// <summary>
        /// The hd n_ getdispinfow.
        /// </summary>
        HDN_GETDISPINFOW = HDN_FIRST - 29, 

        /// <summary>
        /// The hd n_ begindrag.
        /// </summary>
        HDN_BEGINDRAG = HDN_FIRST - 10, 

        /// <summary>
        /// The hd n_ enddrag.
        /// </summary>
        HDN_ENDDRAG = HDN_FIRST - 11, 

        /// <summary>
        /// The hd n_ filterchange.
        /// </summary>
        HDN_FILTERCHANGE = HDN_FIRST - 12, 

        /// <summary>
        /// The hd n_ filterbtnclick.
        /// </summary>
        HDN_FILTERBTNCLICK = HDN_FIRST - 13
    }
}