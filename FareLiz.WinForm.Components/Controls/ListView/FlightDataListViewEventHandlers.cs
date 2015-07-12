namespace SkyDean.FareLiz.WinForm.Components.Controls.ListView
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core.Data;

    public delegate void ContextMenuStripEventHandler(FlightDataListView sender, MenuBuilderEventArgs e);
    public class MenuBuilderEventArgs : CancelEventArgs
    {
        public Flight SelectedFlight { get; private set; }

        private readonly List<ToolStripItem> _newItems = new List<ToolStripItem>();

        public void AddItem(ToolStripItem item)
        {
            if (item != null)
                this._newItems.Add(item);
        }

        public ToolStripItem[] NewItems { get { return this._newItems.ToArray(); } }

        public MenuBuilderEventArgs(Flight selectedFlight)
        {
            this.SelectedFlight = selectedFlight;
        }
    }
}
