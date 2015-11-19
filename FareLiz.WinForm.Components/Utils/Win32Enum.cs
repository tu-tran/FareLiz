namespace SkyDean.FareLiz.WinForm.Components.Utils
{
    /// <summary>Custom draw draw stage values</summary>
    public enum W32_CDDS
    {
        /// <summary>
        /// The cdd s_ prepaint.
        /// </summary>
        CDDS_PREPAINT = 0x00000001, 

        /// <summary>
        /// The cdd s_ postpaint.
        /// </summary>
        CDDS_POSTPAINT = 0x00000002, 

        /// <summary>
        /// The cdd s_ preerase.
        /// </summary>
        CDDS_PREERASE = 0x00000003, 

        /// <summary>
        /// The cdd s_ posterase.
        /// </summary>
        CDDS_POSTERASE = 0x00000004, 

        /// <summary>
        /// The cdd s_ item.
        /// </summary>
        CDDS_ITEM = 0x00010000, 

        /// <summary>
        /// The cdd s_ itemprepaint.
        /// </summary>
        CDDS_ITEMPREPAINT = CDDS_ITEM | CDDS_PREPAINT, 

        /// <summary>
        /// The cdd s_ itempostpaint.
        /// </summary>
        CDDS_ITEMPOSTPAINT = CDDS_ITEM | CDDS_POSTPAINT, 

        /// <summary>
        /// The cdd s_ itempreerase.
        /// </summary>
        CDDS_ITEMPREERASE = CDDS_ITEM | CDDS_PREERASE, 

        /// <summary>
        /// The cdd s_ itemposterase.
        /// </summary>
        CDDS_ITEMPOSTERASE = CDDS_ITEM | CDDS_POSTERASE, 

        /// <summary>
        /// The cdd s_ subitem.
        /// </summary>
        CDDS_SUBITEM = 0x00020000, 

        /// <summary>
        /// The cdd s_ subitemprepaint.
        /// </summary>
        CDDS_SUBITEMPREPAINT = CDDS_SUBITEM | CDDS_ITEMPREPAINT, 

        /// <summary>
        /// The cdd s_ subitempostpaint.
        /// </summary>
        CDDS_SUBITEMPOSTPAINT = CDDS_SUBITEM | CDDS_ITEMPOSTPAINT, 

        /// <summary>
        /// The cdd s_ subitempreerase.
        /// </summary>
        CDDS_SUBITEMPREERASE = CDDS_SUBITEM | CDDS_ITEMPREERASE, 

        /// <summary>
        /// The cdd s_ subitemposterase.
        /// </summary>
        CDDS_SUBITEMPOSTERASE = CDDS_SUBITEM | CDDS_ITEMPOSTERASE
    }

    /// <summary>Custom draw RequestState information</summary>
    public enum W32_CDIS
    {
        /// <summary>
        /// The cdi s_ selected.
        /// </summary>
        CDIS_SELECTED = 0x0001, 

        /// <summary>
        /// The cdi s_ grayed.
        /// </summary>
        CDIS_GRAYED = 0x0002, 

        /// <summary>
        /// The cdi s_ disabled.
        /// </summary>
        CDIS_DISABLED = 0x0004, 

        /// <summary>
        /// The cdi s_ checked.
        /// </summary>
        CDIS_CHECKED = 0x0008, 

        /// <summary>
        /// The cdi s_ focus.
        /// </summary>
        CDIS_FOCUS = 0x0010, 

        /// <summary>
        /// The cdi s_ default.
        /// </summary>
        CDIS_DEFAULT = 0x0020, 

        /// <summary>
        /// The cdi s_ hot.
        /// </summary>
        CDIS_HOT = 0x0040, 

        /// <summary>
        /// The cdi s_ marked.
        /// </summary>
        CDIS_MARKED = 0x0080, 

        /// <summary>
        /// The cdi s_ indeterminate.
        /// </summary>
        CDIS_INDETERMINATE = 0x0100, 

        /// <summary>
        /// The cdi s_ showkeyboardcues.
        /// </summary>
        CDIS_SHOWKEYBOARDCUES = 0x0200
    }

    /// <summary>Custom draw return values</summary>
    public enum W32_CDRF
    {
        /// <summary>
        /// The cdr f_ dodefault.
        /// </summary>
        CDRF_DODEFAULT = 0x0000, 

        /// <summary>
        /// The cdr f_ newfont.
        /// </summary>
        CDRF_NEWFONT = 0x0002, 

        /// <summary>
        /// The cdr f_ skipdefault.
        /// </summary>
        CDRF_SKIPDEFAULT = 0x0004, 

        /// <summary>
        /// The cdr f_ notifypostpaint.
        /// </summary>
        CDRF_NOTIFYPOSTPAINT = 0x0010, 

        /// <summary>
        /// The cdr f_ notifyitemdraw.
        /// </summary>
        CDRF_NOTIFYITEMDRAW = 0x0020, 

        /// <summary>
        /// The cdr f_ notifysubitemdraw.
        /// </summary>
        CDRF_NOTIFYSUBITEMDRAW = 0x0020, 

        /// <summary>
        /// The cdr f_ notifyposterase.
        /// </summary>
        CDRF_NOTIFYPOSTERASE = 0x0040
    }

    /// <summary>GetWindowLong flags</summary>
    public enum W32_GWL
    {
        /// <summary>
        /// The gw l_ wndproc.
        /// </summary>
        GWL_WNDPROC = -4, 

        /// <summary>
        /// The gw l_ hinstance.
        /// </summary>
        GWL_HINSTANCE = -6, 

        /// <summary>
        /// The gw l_ hwndparent.
        /// </summary>
        GWL_HWNDPARENT = -8, 

        /// <summary>
        /// The gw l_ style.
        /// </summary>
        GWL_STYLE = -16, 

        /// <summary>
        /// The gw l_ exstyle.
        /// </summary>
        GWL_EXSTYLE = -20, 

        /// <summary>
        /// The gw l_ userdata.
        /// </summary>
        GWL_USERDATA = -21, 

        /// <summary>
        /// The gw l_ id.
        /// </summary>
        GWL_ID = -12
    }

    /// <summary>Header control item format</summary>
    public enum W32_HDF
    {
        /// <summary>
        /// The hd f_ left.
        /// </summary>
        HDF_LEFT = 0x0000, 

        /// <summary>
        /// The hd f_ right.
        /// </summary>
        HDF_RIGHT = 0x0001, 

        /// <summary>
        /// The hd f_ center.
        /// </summary>
        HDF_CENTER = 0x0002, 

        /// <summary>
        /// The hd f_ justifymask.
        /// </summary>
        HDF_JUSTIFYMASK = 0x0003, 

        /// <summary>
        /// The hd f_ nojustify.
        /// </summary>
        HDF_NOJUSTIFY = 0xFFFC, 

        /// <summary>
        /// The hd f_ rtlreading.
        /// </summary>
        HDF_RTLREADING = 0x0004, 

        /// <summary>
        /// The hd f_ sortdown.
        /// </summary>
        HDF_SORTDOWN = 0x0200, 

        /// <summary>
        /// The hd f_ sortup.
        /// </summary>
        HDF_SORTUP = 0x0400, 

        /// <summary>
        /// The hd f_ sorted.
        /// </summary>
        HDF_SORTED = 0x0600, 

        /// <summary>
        /// The hd f_ nosort.
        /// </summary>
        HDF_NOSORT = 0xF1FF, 

        /// <summary>
        /// The hd f_ image.
        /// </summary>
        HDF_IMAGE = 0x0800, 

        /// <summary>
        /// The hd f_ bitma p_ o n_ right.
        /// </summary>
        HDF_BITMAP_ON_RIGHT = 0x1000, 

        /// <summary>
        /// The hd f_ bitmap.
        /// </summary>
        HDF_BITMAP = 0x2000, 

        /// <summary>
        /// The hd f_ string.
        /// </summary>
        HDF_STRING = 0x4000, 

        /// <summary>
        /// The hd f_ ownerdraw.
        /// </summary>
        HDF_OWNERDRAW = 0x8000
    }

    /// <summary>Header control filter type</summary>
    public enum W32_HDFT
    {
        /// <summary>
        /// The hdf t_ isstring.
        /// </summary>
        HDFT_ISSTRING = 0x0000, 

        /// <summary>
        /// The hdf t_ isnumber.
        /// </summary>
        HDFT_ISNUMBER = 0x0001, 

        /// <summary>
        /// The hdf t_ hasnovalue.
        /// </summary>
        HDFT_HASNOVALUE = 0x8000
    }

    /// <summary>Header control item masks</summary>
    public enum W32_HDI
    {
        /// <summary>
        /// The hd i_ width.
        /// </summary>
        HDI_WIDTH = 0x0001, 

        /// <summary>
        /// The hd i_ height.
        /// </summary>
        HDI_HEIGHT = HDI_WIDTH, 

        /// <summary>
        /// The hd i_ text.
        /// </summary>
        HDI_TEXT = 0x0002, 

        /// <summary>
        /// The hd i_ format.
        /// </summary>
        HDI_FORMAT = 0x0004, 

        /// <summary>
        /// The hd i_ lparam.
        /// </summary>
        HDI_LPARAM = 0x0008, 

        /// <summary>
        /// The hd i_ bitmap.
        /// </summary>
        HDI_BITMAP = 0x0010, 

        /// <summary>
        /// The hd i_ image.
        /// </summary>
        HDI_IMAGE = 0x0020, 

        /// <summary>
        /// The hd i_ d i_ setitem.
        /// </summary>
        HDI_DI_SETITEM = 0x0040, 

        /// <summary>
        /// The hd i_ order.
        /// </summary>
        HDI_ORDER = 0x0080, 

        /// <summary>
        /// The hd i_ filter.
        /// </summary>
        HDI_FILTER = 0x0100
    }

    /// <summary>Header control styles</summary>
    public enum W32_HDS
    {
        /// <summary>
        /// The hd s_ horz.
        /// </summary>
        HDS_HORZ = 0x0000, 

        /// <summary>
        /// The hd s_ buttons.
        /// </summary>
        HDS_BUTTONS = 0x0002, 

        /// <summary>
        /// The hd s_ hottrack.
        /// </summary>
        HDS_HOTTRACK = 0x0004, 

        /// <summary>
        /// The hd s_ hidden.
        /// </summary>
        HDS_HIDDEN = 0x0008, 

        /// <summary>
        /// The hd s_ dragdrop.
        /// </summary>
        HDS_DRAGDROP = 0x0040, 

        /// <summary>
        /// The hd s_ fulldrag.
        /// </summary>
        HDS_FULLDRAG = 0x0080, 

        /// <summary>
        /// The hd s_ filterbar.
        /// </summary>
        HDS_FILTERBAR = 0x0100
    }

    /// <summary>Header control hittest results</summary>
    public enum W32_HHT
    {
        /// <summary>
        /// The hh t_ nowhere.
        /// </summary>
        HHT_NOWHERE = 0x0001, 

        /// <summary>
        /// The hh t_ onheader.
        /// </summary>
        HHT_ONHEADER = 0x0002, 

        /// <summary>
        /// The hh t_ ondivider.
        /// </summary>
        HHT_ONDIVIDER = 0x0004, 

        /// <summary>
        /// The hh t_ ondivopen.
        /// </summary>
        HHT_ONDIVOPEN = 0x0008, 

        /// <summary>
        /// The hh t_ onfilter.
        /// </summary>
        HHT_ONFILTER = 0x0010, 

        /// <summary>
        /// The hh t_ onfilterbutton.
        /// </summary>
        HHT_ONFILTERBUTTON = 0x0020, 

        /// <summary>
        /// The hh t_ above.
        /// </summary>
        HHT_ABOVE = 0x0100, 

        /// <summary>
        /// The hh t_ below.
        /// </summary>
        HHT_BELOW = 0x0200, 

        /// <summary>
        /// The hh t_ toright.
        /// </summary>
        HHT_TORIGHT = 0x0400, 

        /// <summary>
        /// The hh t_ toleft.
        /// </summary>
        HHT_TOLEFT = 0x0800
    }

    /// <summary>ListView item masks</summary>
    public enum W32_LVIF
    {
        /// <summary>
        /// The lvi f_ text.
        /// </summary>
        LVIF_TEXT = 0x0001, 

        /// <summary>
        /// The lvi f_ image.
        /// </summary>
        LVIF_IMAGE = 0x0002, 

        /// <summary>
        /// The lvi f_ param.
        /// </summary>
        LVIF_PARAM = 0x0004, 

        /// <summary>
        /// The lvi f_ state.
        /// </summary>
        LVIF_STATE = 0x0008, 

        /// <summary>
        /// The lvi f_ indent.
        /// </summary>
        LVIF_INDENT = 0x0010, 

        /// <summary>
        /// The lvi f_ norecompute.
        /// </summary>
        LVIF_NORECOMPUTE = 0x0800
    }

    /// <summary>ListView item rectangle type</summary>
    public enum W32_LVIR
    {
        /// <summary>
        /// The lvi r_ bounds.
        /// </summary>
        LVIR_BOUNDS = 0x0000, 

        /// <summary>
        /// The lvi r_ icon.
        /// </summary>
        LVIR_ICON = 0x0001, 

        /// <summary>
        /// The lvi r_ label.
        /// </summary>
        LVIR_LABEL = 0x0002, 

        /// <summary>
        /// The lvi r_ selectbounds.
        /// </summary>
        LVIR_SELECTBOUNDS = 0x0003
    }

    /// <summary>ListView item states</summary>
    public enum W32_LVIS
    {
        /// <summary>
        /// The lvi s_ focused.
        /// </summary>
        LVIS_FOCUSED = 0x0001, 

        /// <summary>
        /// The lvi s_ selected.
        /// </summary>
        LVIS_SELECTED = 0x0002, 

        /// <summary>
        /// The lvi s_ cut.
        /// </summary>
        LVIS_CUT = 0x0004, 

        /// <summary>
        /// The lvi s_ drophilited.
        /// </summary>
        LVIS_DROPHILITED = 0x0008, 

        /// <summary>
        /// The lvi s_ activating.
        /// </summary>
        LVIS_ACTIVATING = 0x0020, 

        /// <summary>
        /// The lvi s_ overlaymask.
        /// </summary>
        LVIS_OVERLAYMASK = 0x0F00, 

        /// <summary>
        /// The lvi s_ stateimagemask.
        /// </summary>
        LVIS_STATEIMAGEMASK = 0xF000
    }

    /// <summary>
    /// The list view group mask.
    /// </summary>
    public enum ListViewGroupMask
    {
        /// <summary>
        /// The none.
        /// </summary>
        None = 0x00000, 

        /// <summary>
        /// The header.
        /// </summary>
        Header = 0x00001, 

        /// <summary>
        /// The footer.
        /// </summary>
        Footer = 0x00002, 

        /// <summary>
        /// The state.
        /// </summary>
        State = 0x00004, 

        /// <summary>
        /// The align.
        /// </summary>
        Align = 0x00008, 

        /// <summary>
        /// The group id.
        /// </summary>
        GroupId = 0x00010, 

        /// <summary>
        /// The sub title.
        /// </summary>
        SubTitle = 0x00100, 

        /// <summary>
        /// The task.
        /// </summary>
        Task = 0x00200, 

        /// <summary>
        /// The description top.
        /// </summary>
        DescriptionTop = 0x00400, 

        /// <summary>
        /// The description bottom.
        /// </summary>
        DescriptionBottom = 0x00800, 

        /// <summary>
        /// The title image.
        /// </summary>
        TitleImage = 0x01000, 

        /// <summary>
        /// The extended image.
        /// </summary>
        ExtendedImage = 0x02000, 

        /// <summary>
        /// The items.
        /// </summary>
        Items = 0x04000, 

        /// <summary>
        /// The subset.
        /// </summary>
        Subset = 0x08000, 

        /// <summary>
        /// The subset items.
        /// </summary>
        SubsetItems = 0x10000
    }
}