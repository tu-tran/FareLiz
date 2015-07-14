namespace SkyDean.FareLiz.WinForm.Components.Controls.TextBox
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// The auto complete textbox.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public partial class AutoCompleteTextbox<T> : TextBox
    {
        /// <summary>The _mouse index.</summary>
        private int _mouseIndex = -1;

        /// <summary>The _prev sel index.</summary>
        private int _prevSelIndex = -1;

        /// <summary>The _prev sel item.</summary>
        private object _prevSelItem;

        // string to remember a former input
        /// <summary>The _suppress auto suggest.</summary>
        private bool _suppressAutoSuggest;

        /// <summary>The _suppress sel index changed.</summary>
        private bool _suppressSelIndexChanged;

        // the constructor
        /// <summary>Initializes a new instance of the <see cref="AutoCompleteTextbox{T}" /> class.</summary>
        public AutoCompleteTextbox()
        {
            this.InitializeComponent();
            this.listBox.DataSource = this.CurrentAutoCompleteList;
        }

        /// <summary>The selected item changed.</summary>
        public event EventHandler SelectedItemChanged;

        #region Properties

        // the list for our suggestions database
        /// <summary>The _auto completed list.</summary>
        private IList<T> _autoCompletedList;

        /// <summary>Gets or sets the auto complete list.</summary>
        public virtual IList<T> AutoCompleteList
        {
            get
            {
                return this._autoCompletedList;
            }

            set
            {
                this._autoCompletedList = value;
            }
        }

        /// <summary>Gets or sets a value indicating whether always show suggest.</summary>
        public bool AlwaysShowSuggest { get; set; }

        // case sensitivity
        /// <summary>The _case sensitive.</summary>
        private bool _caseSensitive;

        /// <summary>Gets or sets a value indicating whether case sensitive.</summary>
        public bool CaseSensitive
        {
            get
            {
                return this._caseSensitive;
            }

            set
            {
                this._caseSensitive = value;
            }
        }

        // minimum characters to be typed before suggestions are displayed
        /// <summary>The _min typed characters.</summary>
        private int _minTypedCharacters;

        /// <summary>Gets or sets the min typed characters.</summary>
        public int MinTypedCharacters
        {
            get
            {
                return this._minTypedCharacters;
            }

            set
            {
                this._minTypedCharacters = value > 0 ? value : 1;
            }
        }

        /// <summary>Gets or sets the selected index.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int SelectedIndex
        {
            get
            {
                return this.listBox.SelectedIndex;
            }

            set
            {
                this.listBox.SelectedIndex = value;
            }
        }

        /// <summary>Gets or sets the text.</summary>
        public new string Text
        {
            get
            {
                return base.Text;
            }

            set
            {
                this._suppressAutoSuggest = true;
                base.Text = value;
                this.listBox.Items.Clear();
                this.SelectItem();
                this._suppressAutoSuggest = false;
            }
        }

        /// <summary>The _visible items.</summary>
        private int _visibleItems = 10;

        /// <summary>Gets or sets the visible suggest items.</summary>
        public int VisibleSuggestItems
        {
            get
            {
                return this._visibleItems;
            }

            set
            {
                this._visibleItems = value > 3 ? value : 3;
            }
        }

        // the actual list of currently displayed suggestions
        /// <summary>The current auto complete list.</summary>
        private ListBox.ObjectCollection CurrentAutoCompleteList;

        // the parent Form of this control
        /// <summary>Gets the parent form.</summary>
        private Form ParentForm
        {
            get
            {
                return this.Parent.FindForm();
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>The hide suggestion list box.</summary>
        public void HideSuggestionListBox()
        {
            if (this.ParentForm != null)
            {
                this.panel.Hide(); // hiding the panel also hides the listbox
                if (this.ParentForm.Controls.Contains(this.panel))
                {
                    this.ParentForm.Controls.Remove(this.panel);
                }
            }
        }

        /// <summary>
        /// The on key down.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        protected override void OnKeyDown(KeyEventArgs args)
        {
            args.Handled = true;
            if (args.KeyCode == Keys.Up)
            {
                this.MoveSelectionInListBox(this.SelectedIndex - 1); // Up: move the selection in listbox one up
            }
            else if (args.KeyCode == Keys.Down)
            {
                this.MoveSelectionInListBox(this.SelectedIndex + 1); // Down: move the selection in listbox one down
            }
            else if (args.KeyCode == Keys.PageUp)
            {
                this.MoveSelectionInListBox(this.SelectedIndex - 10); // Page Up: move 10 up
            }
            else if (args.KeyCode == Keys.PageDown)
            {
                this.MoveSelectionInListBox((this.SelectedIndex + 10)); // Pade Down: move 10 down
            }
            else if ((args.KeyCode == Keys.Enter))
            {
                this.SelectItem(); // Enter: select the item in the ListBox
            }
            else if (args.KeyCode == Keys.Escape)
            {
                this.HideSuggestionListBox(); // Escape: Hide suggestion box
            }
            else if (args.Control && args.KeyCode == Keys.A)
            {
                this.SelectAll(); // Crtl+A: select all
            }
            else
            {
                base.OnKeyDown(args);

                // work is not done, maybe the base class will process the event, so call it...            
                args.Handled = false;
            }
        }

        /// <summary>
        /// The on got focus.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnGotFocus(EventArgs e)
        {
            if (this.AlwaysShowSuggest && !this.panel.Visible && this.listBox.Items.Count > 0)
            {
                this.ShowSuggests();
            }

            base.OnGotFocus(e);
        }

        /// <summary>
        /// The on lost focus.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnLostFocus(EventArgs e)
        {
            if (!(this.ContainsFocus || this.panel.ContainsFocus))
            {
                this.SelectItem(); // then hide the stuff and select the last valid item
            }

            base.OnLostFocus(e);
        }

        /// <summary>
        /// The on text changed.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        protected override void OnTextChanged(EventArgs args)
        {
            if (!(this.DesignMode || this._suppressAutoSuggest))
            {
                this.ShowSuggests();
            }

            base.OnTextChanged(args);
        }

        /// <summary>
        /// The list box_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.SelectItem();
                e.Handled = true;
            }
        }

        /// <summary>
        /// The list box_ mouse click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listBox_MouseClick(object sender, MouseEventArgs e)
        {
            this.SelectItem();
        }

        /// <summary>
        /// The move selection in list box.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        private void MoveSelectionInListBox(int index)
        {
            if (this.listBox.Items.Count < 1)
            {
                return;
            }

            if (index <= -1)
            {
                // beginning of list
                this.listBox.SelectedIndex = 0;
            }
            else if (index > (this.listBox.Items.Count - 1))
            {
                // end of list
                this.listBox.SelectedIndex = this.listBox.Items.Count - 1;
            }
            else
            {
                this.listBox.SelectedIndex = index; // somewhere in the middle
            }

            if (!this.panel.Visible && this.listBox.Items.Count > 0)
            {
                this.ShowSuggests();
            }
        }

        // selects the current item
        /// <summary>The select item.</summary>
        /// <returns>The <see cref="bool" />.</returns>
        private bool SelectItem()
        {
            if (this.AutoCompleteList == null || this.AutoCompleteList.Count < 1)
            {
                return false;
            }

            this._suppressSelIndexChanged = this._suppressAutoSuggest = true;

            // if the ListBox is not empty
            if (this.listBox.Items.Count > 0)
            {
                if (this._mouseIndex > 0 && this._mouseIndex < this.listBox.Items.Count)
                {
                    this.SelectedIndex = this._mouseIndex;
                }

                if (this.SelectedIndex > -1)
                {
                    base.Text = this.listBox.GetItemText(this.listBox.SelectedItem);

                    // set the Text of the TextBox to the selected item of the ListBox
                    if (this.ContainsFocus)
                    {
                        this.SelectAll();
                    }
                }
                else
                {
                    for (var i = 0; i < this.listBox.Items.Count; i++)
                    {
                        var itemText = this.listBox.GetItemText(this.listBox.Items[i]);
                        if (base.Text == itemText)
                        {
                            this.SelectedIndex = i;
                            break;
                        }
                    }

                    if (this.SelectedIndex < 0)
                    {
                        this.listBox.SelectedItem = this._prevSelItem;
                        if (this.SelectedIndex < 0)
                        {
                            this.SelectedIndex = 0;
                        }

                        base.Text = this.listBox.GetItemText(this.listBox.SelectedItem);
                    }
                }

                // and hide the ListBox
                this.HideSuggestionListBox();
            }
            else
            {
                // The listbox was empty
                object newSelItem = null;

                // ListBox is empty: Try to match the item with the data source
                if (!string.IsNullOrEmpty(base.Text))
                {
                    foreach (var item in this.AutoCompleteList)
                    {
                        var itemText = this.listBox.GetItemText(item);
                        if (base.Text == itemText)
                        {
                            newSelItem = item;
                            break;
                        }
                    }
                }

                if (newSelItem == null)
                {
                    if (this._prevSelItem == null)
                    {
                        newSelItem = this.AutoCompleteList[0];
                    }
                    else
                    {
                        newSelItem = this._prevSelItem;
                    }
                }

                if (newSelItem != null)
                {
                    this.listBox.Items.Add(newSelItem);
                    base.Text = this.listBox.GetItemText(newSelItem);
                }

                this.listBox.SelectedItem = newSelItem;
            }

            var selItem = this.listBox.SelectedItem;
            if (this._prevSelItem != selItem)
            {
                if (this.SelectedItemChanged != null)
                {
                    this.SelectedItemChanged(this, EventArgs.Empty);
                }

                this._prevSelItem = selItem;
            }

            this._suppressSelIndexChanged = this._suppressAutoSuggest = false;
            return true;
        }

        // shows the suggestions in ListBox beneath the TextBox
        // and fitting it into the ParentForm
        /// <summary>The show suggests.</summary>
        private void ShowSuggests()
        {
            // show only if MinTypedCharacters have been typed
            if (base.Text.Length >= this.MinTypedCharacters)
            {
                // prevent overlapping problems with other controls
                // while loading data there is nothing to draw, so suspend layout
                this.panel.SuspendLayout();
                this.UpdateCurrentAutoCompleteListAndPanel();

                if (this.CurrentAutoCompleteList != null && this.CurrentAutoCompleteList.Count > 0)
                {
                    // finally show Panel and ListBox
                    // (but after refresh to prevent drawing empty rectangles)
                    this.listBox.SelectedIndex = 0;
                    this.panel.Show();

                    // at the top of all controls
                    this.panel.BringToFront();

                    // then give the focuse back to the TextBox (this control)
                    this.Focus();
                }

                // or hide if no results
                else
                {
                    this.HideSuggestionListBox();
                }

                // prevent overlapping problems with other controls
                this.panel.ResumeLayout(true);
            }

            // hide if typed chars <= MinCharsTyped
            else
            {
                this.HideSuggestionListBox();
            }
        }

        // This is a timecritical part
        // Fills/ refreshed the CurrentAutoCompleteList with appropreate candidates
        /// <summary>The update current auto complete list and panel.</summary>
        private void UpdateCurrentAutoCompleteListAndPanel()
        {
            var parentForm = this.ParentForm;

            // if there is a ParentForm
            if (parentForm != null)
            {
                // Update auto complete data source
                // the list of suggestions has to be refreshed so clear it
                this.CurrentAutoCompleteList = new ListBox.ObjectCollection(this.listBox);
                var curIndex = 0;
                var txt = base.Text;

                // and find appropreate candidates for the new CurrentAutoCompleteList in AutoCompleteList                
                foreach (object obj in this.AutoCompleteList)
                {
                    var str = obj.ToString();
                    var isMatch = (this.MinTypedCharacters == 0 && string.IsNullOrEmpty(txt))
                                  || str.IndexOf(txt, this.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) > -1;
                    if (isMatch)
                    {
                        this.CurrentAutoCompleteList.Add(obj);

                        // Add candidates to new CurrentAutoCompleteList                        
                        if (++curIndex == this.VisibleSuggestItems)
                        {
                            break;
                        }
                    }
                }

                this.listBox.Items.Clear();
                this.listBox.Items.AddRange(this.CurrentAutoCompleteList);

                // Visual rendering
                // get its width
                this.listBox.Width = this.Width;

                // calculate the remeining height beneath the TextBox
                var itemsHeight = 5
                                  + (this.listBox.DrawMode == DrawMode.Normal
                                         ? TextRenderer.MeasureText(new string('\n', this.CurrentAutoCompleteList.Count), this.listBox.Font).Height
                                         : this.CurrentAutoCompleteList.Count * this.listBox.ItemHeight);

                var borderWidth = 2 * SystemInformation.BorderSize.Width;
                var screenLoc = this.PointToScreen(Point.Empty);
                var relLocation = parentForm.PointToClient(screenLoc);
                var maxHeight = parentForm.ClientSize.Height - this.Height - relLocation.Y;
                this.listBox.Height = itemsHeight > maxHeight ? maxHeight : itemsHeight;

                // and the Location to use
                this.panel.Location = new Point(relLocation.X - borderWidth, relLocation.Y + this.Height - borderWidth);
                this.panel.Size = this.listBox.Size;
                if (!parentForm.Controls.Contains(this.panel))
                {
                    parentForm.Controls.Add(this.panel);
                }
            }
        }

        /// <summary>
        /// The list box_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this._suppressSelIndexChanged)
            {
                return;
            }

            this.RefreshItem(this._prevSelIndex);
            this._prevSelIndex = this.listBox.SelectedIndex;
        }

        /// <summary>
        /// The list box_ mouse leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listBox_MouseLeave(object sender, EventArgs e)
        {
            this.RefreshItem(this._mouseIndex);
            this.RefreshItem(this._prevSelIndex);
            this._mouseIndex = -1;
        }

        /// <summary>
        /// The list box_ mouse move.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listBox_MouseMove(object sender, MouseEventArgs e)
        {
            var index = this.listBox.IndexFromPoint(e.Location);
            if (index != this._mouseIndex)
            {
                this.RefreshItem(this._prevSelIndex);
                this.RefreshItem(this._mouseIndex);
                this.RefreshItem(index);
                this._mouseIndex = index;
            }
        }

        /// <summary>
        /// The refresh item.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        private void RefreshItem(int index)
        {
            if (index > -1 && index < this.listBox.Items.Count)
            {
                var itemRect = this.listBox.GetItemRectangle(index);
                this.listBox.Invalidate(itemRect);
            }
        }

        /// <summary>
        /// The list box on draw item.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ListBoxOnDrawItem(object sender, DrawItemEventArgs e)
        {
            if (this.listBox.Items.Count < 0 || e.Index < 0)
            {
                return;
            }

            // Draw the background of the ListBox control for each item.
            var isHighlighted = e.Index == this._mouseIndex || (this._mouseIndex < 0 && e.Index == this.SelectedIndex);
            Brush backBrush;
            var bounds = e.Bounds;

            if (isHighlighted)
            {
                backBrush = SystemBrushes.Highlight;
            }
            else
            {
                backBrush = SystemBrushes.Window;
            }

            e.Graphics.FillRectangle(backBrush, bounds);

            var text = this.listBox.Items[e.Index].ToString();

            using (var txtBrush = new SolidBrush(isHighlighted ? SystemColors.HighlightText : this.listBox.ForeColor))
            {
                var textSize = TextRenderer.MeasureText(text, this.listBox.Font, new Size(bounds.Width, int.MaxValue), TextFormatFlags.WordBreak);
                var y = bounds.Y + (bounds.Height - textSize.Height) / 2;
                e.Graphics.DrawString(text, e.Font, txtBrush, new Rectangle(2, y, bounds.Width, textSize.Height), StringFormat.GenericDefault);
            }

            e.Graphics.DrawLine(Pens.Gainsboro, 0, bounds.Y, bounds.X + bounds.Width, bounds.Y);

            // If the ListBox has focus, draw a focus rectangle around the selected item.
            e.DrawFocusRectangle();
        }

        #endregion Methods
    }
}