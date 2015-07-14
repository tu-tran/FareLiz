namespace SkyDean.FareLiz.WinForm.Components.Controls.TextBox
{
    using System.Windows.Forms;

    partial class AutoCompleteTextbox<T>
    {
        private void InitializeComponent()
        {
            this.listBox = new ListBox();
            this.panel = new Panel();

            this.listBox.Name = "SuggestionListBox";
            this.listBox.Visible = true;

            this.panel.Visible = false;
            this.panel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;  // to be able to fit to changing sizes of the parent form
            this.panel.ClientSize = new System.Drawing.Size(1, 1);  // initialize with minimum size to avoid overlaping or flickering problems
            this.panel.Controls.Add(this.listBox);
            this.panel.Name = "SuggestionPanel";
            this.panel.PerformLayout();

            this.listBox.SelectionMode = SelectionMode.One;
            this.listBox.DrawMode = DrawMode.OwnerDrawFixed;
            this.listBox.DrawItem += this.ListBoxOnDrawItem;
            this.listBox.KeyDown += new KeyEventHandler(this.listBox_KeyDown);
            this.listBox.MouseClick += new MouseEventHandler(this.listBox_MouseClick);            
            this.listBox.MouseMove += this.listBox_MouseMove;
            this.listBox.MouseLeave += this.listBox_MouseLeave;
            this.listBox.SelectedIndexChanged += this.listBox_SelectedIndexChanged;

            this.Font = System.Drawing.SystemFonts.DefaultFont;
        }

        protected ListBox listBox;
        protected Panel panel;
    }

}
