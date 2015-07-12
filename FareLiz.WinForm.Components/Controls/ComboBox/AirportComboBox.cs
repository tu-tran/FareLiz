namespace SkyDean.FareLiz.WinForm.Components.Controls.ComboBox
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core.Data;

    public class AirportComboBox : ComboBox
    {
        private int _lastSelectedIndex = -1;
        private object _lastSelectedItem = null;
        private bool _suppressSelectedIndexChange = false;
        private bool _suppressSelectedItemChange = false;

        public new object DataSource
        {
            get { return base.DataSource; }
            set
            {
                this.SetDataSource(value);
                this.MeasureDimensions();
            }
        }

        public Airport SelectedAirport
        {
            get { return this.SelectedItem as Airport; }
            set { this.SelectedItem = value; }
        }

        public string SelectedAirportCode
        {
            get { return this.SelectedValue == null ? null : this.SelectedValue.ToString(); }
            set
            {
                if (value == null)
                    this.SelectedItem = null;
                else
                    this.SelectedValue = value;
            }
        }

        public AirportComboBox()
        {
            this.DrawMode = DrawMode.OwnerDrawVariable;
            this.MaxDropDownItems = 10;            
            this.ValueMember = "IATA";
            this.DisplayMember = "";
            this.DrawItem += this.OnDrawItem;
            this.MeasureItem += this.OnMeasureItem;
            this.LostFocus += this.OnLostFocus;
        }

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

                triggerSelIndexChange = (prevSelIndex != this.SelectedIndex);
                triggerSelItemChange = (prevSelItem != this.SelectedItem);
            }
            finally
            {
                this._suppressSelectedIndexChange = this._suppressSelectedItemChange = false;
                if (triggerSelItemChange)
                    this.OnSelectedItemChanged(EventArgs.Empty);
                if (triggerSelIndexChange)
                    this.OnSelectedIndexChanged(EventArgs.Empty);
            }
        }

        public int DefaultItemHeight
        {
            get { return 2 * this.Font.Height + 10; }
        }

        protected override void OnClick(EventArgs e)
        {
            if (!this.DroppedDown)
                this.DroppedDown = true;
            base.OnClick(e);
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            if (!this._suppressSelectedIndexChange)
                base.OnSelectedIndexChanged(e);

            this._lastSelectedIndex = this.SelectedIndex;
        }

        protected override void OnSelectedItemChanged(EventArgs e)
        {
            if (!this._suppressSelectedItemChange)
                base.OnSelectedItemChanged(e);

            this._lastSelectedItem = this.SelectedItem;
        }

        private void OnLostFocus(object sender, EventArgs eventArgs)
        {
            this.Select(0, 0);
            if (this.Items.Count < 1)
                return;

            if (this.SelectedIndex > -1)
                base.Text = this.SelectedItem.ToString(); // set the Text to the selected item
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
                        base.Text = this.Items[0].ToString();
                        this.SelectedIndex = 0;
                    }
                }
            }
        }

        internal void MeasureDimensions()
        {
            int visibleItems = (this.MaxDropDownItems > this.Items.Count ? this.Items.Count : this.MaxDropDownItems);

            if (visibleItems == 0)
                this.DropDownHeight = 1;
            else
                this.DropDownHeight = visibleItems * (this.DefaultItemHeight + 2 * SystemInformation.BorderSize.Height);
        }

        private void OnMeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = this.DefaultItemHeight;
        }

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
