namespace SkyDean.FareLiz.WinForm.Components.Dialog
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// Helper class for properly center-aligned form
    /// </summary>
    public class SmartForm : Form
    {
        protected override void OnLoad(EventArgs e)
        {
            // If the start location of the form is set to be centered align and or the parent form is missing
            if (this.StartPosition == FormStartPosition.CenterScreen
                || (this.StartPosition == FormStartPosition.CenterParent && this.ParentForm == null))
            {
                var screen = Screen.FromPoint(Cursor.Position);
                this.Left = screen.Bounds.Left + screen.Bounds.Width / 2 - this.Width / 2;
                this.Top = screen.Bounds.Top + screen.Bounds.Height / 2 - this.Height / 2;
            }

            base.OnLoad(e);
        }
    }
}
