using SkyDean.FareLiz.Core.Data;

namespace SkyDean.FareLiz.WinForm.Components.Controls.Custom
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core.Presentation;

    public sealed class TaskbarFlightNotifier : TaskbarNotifierBase, IFlightNotifier
    {
        private FlightItemsPanel _mainPanel = new FlightItemsPanel() { Dock = DockStyle.Fill };

        public void Show(string title, string header, IList<FlightMonitorItem> data, int timeToStay, NotificationType type, bool waitTillHidden)
        {
            if (waitTillHidden)
                this.WaitTillHidden();

            if (this.InvokeRequired)
                this.SafeInvoke(new Action(() => this.Show(title, header, data, timeToStay, type, false)));
            else
            {
                this.SetTitle(title);
                this._mainPanel.Bind(header, data);
                this.Display(timeToStay, type);
            }
        }

        protected override void InitializeContentPanel(Panel contentPanel)
        {
            contentPanel.Controls.Add(this._mainPanel);
        }

        protected override Size MeasureContentSize(int maxWidth, int maxHeight)
        {
            var expectedSize = this._mainPanel.AutoResize(maxHeight);
            return expectedSize;
        }
    }
}
