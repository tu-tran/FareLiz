namespace SkyDean.FareLiz.WinForm.Components.Controls.Custom
{
    using System;
    using System.Windows.Forms;

    public class RadioToolStripMenuItem : ToolStripMenuItem
    {
        public RadioToolStripMenuItem() : this(null, null) { }
        public RadioToolStripMenuItem(string text, object tag)
            : base(text)
        {
            this.Click += this.RadioToolStripMenuItem_Click;
            this.CheckedChanged += this.RadioToolStripMenuItem_CheckedChanged;
            this.CheckOnClick = true;
            this.Tag = tag;
        }

        void RadioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.Checked)
                this.Checked = true;
        }

        void RadioToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            var mnuItem = sender as ToolStripMenuItem;
            if (mnuItem != null && mnuItem.CheckOnClick)
            {
                // If this item is no longer in the checked RequestState or if its  
                // parent has not yet been initialized, do nothing. 
                if (!mnuItem.Checked || mnuItem.OwnerItem == null) return;

                // Clear the checked RequestState for all siblings.  
                var parent = mnuItem.OwnerItem as ToolStripDropDownItem;
                if (parent != null)
                {
                    foreach (var item in parent.DropDownItems)
                    {
                        var sibling = item as ToolStripMenuItem;
                        if (sibling != null && sibling != mnuItem)
                            sibling.Checked = false;
                    }
                }
            }
        }
    }
}
