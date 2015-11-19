namespace SkyDean.FareLiz.WinForm.Components.Controls.ComboBox
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core.Data;

    /// <summary>
    /// The airport combo box.
    /// </summary>
    public class AirportComboBox : ComboBox
    {
        /// <summary>
        /// The _last selected index.
        /// </summary>
        private int _lastSelectedIndex = -1;

        /// <summary>
        /// The _last selected item.
        /// </summary>
        private object _lastSelectedItem;

        /// <summary>
        /// The _suppress selected index change.
        /// </summary>
        private bool _suppressSelectedIndexChange;

        /// <summary>
        /// The _suppress selected item change.
        /// </summary>
        private bool _suppressSelectedItemChange;

        /// <summary>
        /// Initializes a new instance of the <see cref="AirportComboBox"/> class.
        /// </summary>
        public AirportComboBox()
        {
            this.DrawMode = DrawMode.OwnerDrawVariable;
            this.MaxDropDownItems = 10;
            this.ValueMember = "IATA";
            this.DisplayMember = string.Empty;
            this.DrawItem += this.OnDrawItem;
            this.MeasureItem += this.OnMeasureItem;
            this.LostFocus += this.OnLostFocus;
        }

        /// <summary>
        /// Gets or sets the data source.
        /// </summary>
        public new object DataSource
        {
            get
            {
                return base.DataSource;
            }

            set
            {
                this.SetDataSource(value);
                this.MeasureDimensions();
            }
        }

        /// <summary>
        /// Gets or sets the selected airport.
        /// </summary>
        public Airport SelectedAirport
        {
            get
            {
                return this.SelectedItem as Airport;
            }

            set
            {
                this.SelectedItem = value;
            }
        }

        /// <summary>
        /// Gets or sets the selected airport code.
        /// </summary>
        public string SelectedAirportCode
        {
            get
            {
                return this.SelectedValue == null ? null : this.SelectedValue.ToString();
            }

            set
            {
                if (value == null)
                {
                    this.SelectedItem = null;
                }
                else
                {
                    this.SelectedValue = value;
                }
            }
        }

        /// <summary>
        /// Gets the default item height.
        /// </summary>
        public int DefaultItemHeight
        {
            get
            {
                return 2 * this.Font.Height + 10;
            }
        }

        /// <summary>
        /// The set data source.
        /// </summary>
        /// <param name="dataSource">
        /// The data source.
        /// </param>
        private void SetDataSource(object dataSource)
        {
            this._suppressSelectedIndexChange = this._suppressSelectedItemChange = true;
            bool triggerSelItemChange = true, triggerSelIndexChange = true;
            try
            {
                var prevSelIndex = this._lastSelectedIndex;
                var prevSelItem = this._lastSelectedItem;
                var prevText = this.Text;
                base.DataSource = dataSource;
                foreach (var item in this.Items)
                {
                    var itemText = this.GetItemText(item);
                    if (prevText == itemText)
                    {
                        this.SelectedItem = item;
                        break;
                    }
                }

                triggerSelIndexChange = prevSelIndex != this.SelectedIndex;
                triggerSelItemChange = prevSelItem != this.SelectedItem;
            }
            finally
            {
                this._suppressSelectedIndexChange = this._suppressSelectedItemChange = false;
                if (triggerSelItemChange)
                {
                    this.OnSelectedItemChanged(EventArgs.Empty);
                }

                if (triggerSelIndexChange)
                {
                    this.OnSelectedIndexChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// The on click.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnClick(EventArgs e)
        {
            if (!this.DroppedDown)
            {
                this.DroppedDown = true;
            }

            base.OnClick(e);
        }

        /// <summary>
        /// The on selected index changed.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            if (!this._suppressSelectedIndexChange)
            {
                base.OnSelectedIndexChanged(e);
            }

            this._lastSelectedIndex = this.SelectedIndex;
        }

        /// <summary>
        /// The on selected item changed.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnSelectedItemChanged(EventArgs e)
        {
            if (!this._suppressSelectedItemChange)
            {
                base.OnSelectedItemChanged(e);
            }

            this._lastSelectedItem = this.SelectedItem;
        }

        /// <summary>
        /// The on lost focus.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        private void OnLostFocus(object sender, EventArgs eventArgs)
        {
            this.Select(0, 0);
            if (this.Items.Count < 1)
            {
                return;
            }

            if (this.SelectedIndex > -1)
            {
                this.Text = this.SelectedItem.ToString(); // set the Text to the selected item
            }
            else
            {
                for (int i = 0; i < this.Items.Count; i++)
                {
                    var item = this.Items[i].ToString();
                    if (this.Text == item)
                    {
                        this.SelectedIndex = i;
                        break;
                    }
                }

                if (this.SelectedIndex < 0)
                {
                    this.SelectedItem = this._lastSelectedItem;
                    if (this.SelectedIndex < 0)
                    {
                        this.Text = this.Items[0].ToString();
                        this.SelectedIndex = 0;
                    }
                }
            }
        }

        /// <summary>
        /// The measure dimensions.
        /// </summary>
        internal void MeasureDimensions()
        {
            int visibleItems = this.MaxDropDownItems > this.Items.Count ? this.Items.Count : this.MaxDropDownItems;

            if (visibleItems == 0)
            {
                this.DropDownHeight = 1;
            }
            else
            {
                this.DropDownHeight = visibleItems * (this.DefaultItemHeight + 2 * SystemInformation.BorderSize.Height);
            }
        }

        /// <summary>
        /// The on measure item.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnMeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = this.DefaultItemHeight;
        }

        /// <summary>
        /// The on draw item.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnDrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            var bounds = e.Bounds;
            string text = this.Items[e.Index].ToString();
            bool isHighlighted = (e.State & DrawItemState.Focus) == DrawItemState.Focus;
            using (var txtBrush = new SolidBrush(isHighlighted ? SystemColors.HighlightText : e.ForeColor))
            {
                var textSize = TextRenderer.MeasureText(text, e.Font, new Size(bounds.Width, int.MaxValue), TextFormatFlags.WordBreak);
                int y = bounds.Y + (bounds.Height - textSize.Height) / 2;
                e.Graphics.DrawString(text, e.Font, txtBrush, new Rectangle(2, y, bounds.Width, textSize.Height), StringFormat.GenericDefault);
            }

            e.Graphics.DrawLine(Pens.Gainsboro, 0, bounds.Y, bounds.X + bounds.Width, bounds.Y);

            e.DrawFocusRectangle();
        }
    }
}