namespace SkyDean.FareLiz.WinForm.Components.Controls.ListView
{
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using System.Windows.Forms;

    using SkyDean.FareLiz.WinForm.Components.Utils;

    /// <summary>The list view extensions.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ListViewExtensions
    {
        /// <summary>
        /// The set double buffering.
        /// </summary>
        /// <param name="listView">
        /// The list view.
        /// </param>
        /// <param name="enabled">
        /// The enabled.
        /// </param>
        public static void SetDoubleBuffering(this ListView listView, bool enabled)
        {
            var method = listView.GetType().GetMethod("SetStyle", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(listView, new object[] { ControlStyles.OptimizedDoubleBuffer, enabled });
        }

        /// <summary>
        /// The set sort icon.
        /// </summary>
        /// <param name="listViewControl">
        /// The list view control.
        /// </param>
        /// <param name="columnIndex">
        /// The column index.
        /// </param>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <exception cref="Win32Exception">
        /// </exception>
        public static void SetSortIcon(this ListView listViewControl, int columnIndex, SortOrder order)
        {
            var columnHeader = NativeMethods.SendMessage(listViewControl.Handle, (int)W32_LVM.LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);
            for (var columnNumber = 0; columnNumber <= listViewControl.Columns.Count - 1; columnNumber++)
            {
                var columnPtr = new IntPtr(columnNumber);
                var item = new HDITEM { mask = W32_HDI.HDI_FORMAT };

                if (NativeMethods.SendMessage(columnHeader, (int)W32_HDM.HDM_GETITEMW, columnPtr, ref item) == IntPtr.Zero)
                {
                    throw new Win32Exception();
                }

                if (order != SortOrder.None && columnNumber == columnIndex)
                {
                    switch (order)
                    {
                        case SortOrder.Ascending:
                            item.fmt &= ~W32_HDF.HDF_SORTDOWN;
                            item.fmt |= W32_HDF.HDF_SORTUP;
                            break;
                        case SortOrder.Descending:
                            item.fmt &= ~W32_HDF.HDF_SORTUP;
                            item.fmt |= W32_HDF.HDF_SORTDOWN;
                            break;
                    }
                }
                else
                {
                    item.fmt &= ~W32_HDF.HDF_SORTDOWN & ~W32_HDF.HDF_SORTUP;
                }

                if (NativeMethods.SendMessage(columnHeader, (int)W32_HDM.HDM_SETITEMW, columnPtr, ref item) == IntPtr.Zero)
                {
                    throw new Win32Exception();
                }
            }
        }
    }
}