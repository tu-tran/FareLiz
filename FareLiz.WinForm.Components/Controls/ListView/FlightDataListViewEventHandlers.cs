namespace SkyDean.FareLiz.WinForm.Components.Controls.ListView
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core.Data;

    /// <summary>
    /// The context menu strip event handler.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    public delegate void ContextMenuStripEventHandler(FlightDataListView sender, MenuBuilderEventArgs e);

    /// <summary>
    /// The menu builder event args.
    /// </summary>
    public class MenuBuilderEventArgs : CancelEventArgs
    {
        /// <summary>
        /// The _new items.
        /// </summary>
        private readonly List<ToolStripItem> _newItems = new List<ToolStripItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuBuilderEventArgs"/> class.
        /// </summary>
        /// <param name="selectedFlight">
        /// The selected flight.
        /// </param>
        public MenuBuilderEventArgs(Flight selectedFlight)
        {
            this.SelectedFlight = selectedFlight;
        }

        /// <summary>
        /// Gets the selected flight.
        /// </summary>
        public Flight SelectedFlight { get; private set; }

        /// <summary>
        /// Gets the new items.
        /// </summary>
        public ToolStripItem[] NewItems
        {
            get
            {
                return this._newItems.ToArray();
            }
        }

        /// <summary>
        /// The add item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void AddItem(ToolStripItem item)
        {
            if (item != null)
            {
                this._newItems.Add(item);
            }
        }
    }
}