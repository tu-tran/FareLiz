namespace SkyDean.FareLiz.WinForm.Components.Controls.Custom
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Presentation;

    /// <summary>The taskbar flight notifier.</summary>
    public sealed class TaskbarFlightNotifier : TaskbarNotifierBase, IFlightNotifier
    {
        /// <summary>The _main panel.</summary>
        private readonly FlightItemsPanel _mainPanel = new FlightItemsPanel { Dock = DockStyle.Fill };

        /// <summary>
        /// The show.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="header">
        /// The header.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="timeToStay">
        /// The time to stay.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="waitTillHidden">
        /// The wait till hidden.
        /// </param>
        public void Show(string title, string header, IList<FlightMonitorItem> data, int timeToStay, NotificationType type, bool waitTillHidden)
        {
            if (waitTillHidden)
            {
                this.WaitTillHidden();
            }

            this.InvokeActionIfNeeded((MethodInvoker)delegate
                {
                    this.SetTitle(title);
                    this._mainPanel.Bind(header, data);
                    this.Display(timeToStay, type);
                });
        }

        /// <summary>
        /// The initialize content panel.
        /// </summary>
        /// <param name="contentPanel">
        /// The content panel.
        /// </param>
        protected override void InitializeContentPanel(Panel contentPanel)
        {
            contentPanel.Controls.Add(this._mainPanel);
        }

        /// <summary>
        /// The measure content size.
        /// </summary>
        /// <param name="maxWidth">
        /// The max width.
        /// </param>
        /// <param name="maxHeight">
        /// The max height.
        /// </param>
        /// <returns>
        /// The <see cref="Size"/>.
        /// </returns>
        protected override Size MeasureContentSize(int maxWidth, int maxHeight)
        {
            var expectedSize = this._mainPanel.AutoResize(maxHeight);
            return expectedSize;
        }
    }
}