namespace SkyDean.FareLiz.WinForm.Components.Controls.ListView
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using SkyDean.FareLiz.WinForm.Components.Controls.Custom;
    using SkyDean.FareLiz.WinForm.Components.Utils;

    /// <summary>The enhanced list view.</summary>
    public partial class EnhancedListView
    {
        // Only Vista and forward allows collaps of ListViewGroups
        /// <summary>The support collapsible groups.</summary>
        public static readonly bool SupportCollapsibleGroups = Environment.OSVersion.Version.Major > 5;

        /// <summary>The group id prop.</summary>
        private static readonly PropertyInfo groupIdProp = typeof(ListViewGroup).GetProperty("ID", BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        /// The send message.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <param name="Msg">
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
        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, ref LVGROUP lParam);

        /// <summary>
        /// Set selected column as the base column for grouping. This method does not refresh the view (Call AutoGroup() to refresh the view with the
        /// new group)
        /// </summary>
        /// <param name="col">
        /// </param>
        public void SetGroupColumn(ColumnHeader col)
        {
            var colIndex = this.GetColumn(col);

            if (colIndex > -1)
            {
                this.SetGroupColumn(colIndex);
            }
            else
            {
                throw new ArgumentException("Could not set group column since the column does not belong to the ListView");
            }
        }

        /// <summary>
        /// Set the column to be used for grouping
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        public void SetGroupColumn(int index)
        {
            this.GroupColumnIndex = index;
            this.AutoGroup();
            RadioToolStripMenuItem item;
            if (this._groupStrip.TryGetValue(index, out item))
            {
                item.Checked = true;
            }
        }

        /// <summary>Group the items according to the GroupIndexColumn</summary>
        public void AutoGroup()
        {
            if (this.GroupColumnIndex < 0)
            {
                throw new ArgumentException("GroupColumnIndex was not set properly");
            }

            if (this.VirtualMode)
            {
                throw new InvalidOperationException("Could not group items in Virtual Mode");
            }

            if (this.Items.Count > 0)
            {
                this.BeginUpdate();
                try
                {
                    this.Groups.Clear();

                    for (var i = 0; i < this.Items.Count; i++)
                    {
                        var item = this.Items[i];
                        var groupName = item.SubItems[this.GroupColumnIndex].Text;
                        ListViewGroup group = null;
                        foreach (ListViewGroup g in this.Groups)
                        {
                            if (g.Header == groupName || g.Name == groupName)
                            {
                                group = g;
                                break;
                            }
                        }

                        if (group == null)
                        {
                            group = new ListViewGroup(groupName);
                            this.Groups.Add(group);
                            this.SetGroupState(group, ListViewGroupState.Collapsible | ListViewGroupState.Normal);
                        }

                        item.Group = group;
                    }
                }
                finally
                {
                    this.EndUpdate();
                }
            }
        }

        /// <summary>
        /// The set group state.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        private void SetGroupState(ListViewGroupState state)
        {
            if (!SupportCollapsibleGroups)
            {
                return;
            }

            foreach (ListViewGroup lvg in this.Groups)
            {
                this.SetGroupState(lvg, state);
            }
        }

        /// <summary>
        /// The set group state.
        /// </summary>
        /// <param name="lstvwgrp">
        /// The lstvwgrp.
        /// </param>
        /// <param name="state">
        /// The state.
        /// </param>
        private void SetGroupState(ListViewGroup lstvwgrp, ListViewGroupState state)
        {
            if (!SupportCollapsibleGroups)
            {
                return;
            }

            if (lstvwgrp == null)
            {
                return;
            }

            var grpId = GetGroupID(lstvwgrp);
            var grpIdParam = grpId ?? lstvwgrp.ListView.Groups.IndexOf(lstvwgrp);

            var group = new LVGROUP { CbSize = Marshal.SizeOf(typeof(LVGROUP)), State = state, Mask = ListViewGroupMask.State, IGroupId = grpIdParam };

            SendMessage(this.Handle, (int)W32_LVM.LVM_SETGROUPINFO, grpIdParam, ref group);
        }

        /// <summary>
        /// The get group id.
        /// </summary>
        /// <param name="lstvwgrp">
        /// The lstvwgrp.
        /// </param>
        /// <returns>
        /// The <see cref="int?"/>.
        /// </returns>
        private static int? GetGroupID(ListViewGroup lstvwgrp)
        {
            int? rtnval = null;
            if (groupIdProp != null)
            {
                var tmprtnval = groupIdProp.GetValue(lstvwgrp, null);
                if (tmprtnval != null)
                {
                    rtnval = tmprtnval as int?;
                }
            }

            return rtnval;
        }

        /// <summary>
        /// The set group footer.
        /// </summary>
        /// <param name="lstvwgrp">
        /// The lstvwgrp.
        /// </param>
        /// <param name="footer">
        /// The footer.
        /// </param>
        public void SetGroupFooter(ListViewGroup lstvwgrp, string footer)
        {
            if (!SupportCollapsibleGroups)
            {
                return;
            }

            if (lstvwgrp == null)
            {
                return;
            }

            var grpId = GetGroupID(lstvwgrp);
            var grpIdParam = grpId ?? this.Groups.IndexOf(lstvwgrp);

            var group = new LVGROUP
                            {
                                CbSize = Marshal.SizeOf(typeof(LVGROUP)), 
                                PszFooter = footer, 
                                Mask = ListViewGroupMask.Footer, 
                                IGroupId = grpIdParam
                            };

            SendMessage(lstvwgrp.ListView.Handle, (int)W32_LVM.LVM_SETGROUPINFO, grpIdParam, ref group);
        }

        /// <summary>The call back set group state.</summary>
        /// <param name="lstvwgrp">The lstvwgrp.</param>
        /// <param name="state">The state.</param>
        private delegate void CallBackSetGroupState(ListViewGroup lstvwgrp, ListViewGroupState state);

        /// <summary>The callback set group string.</summary>
        /// <param name="lstvwgrp">The lstvwgrp.</param>
        /// <param name="value">The value.</param>
        private delegate void CallbackSetGroupString(ListViewGroup lstvwgrp, string value);
    }
}