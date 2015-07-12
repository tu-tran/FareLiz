namespace SkyDean.FareLiz.WinForm.Components.Controls.Custom
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public class PopupMessageToolTip : ToolTip
    {
        private Font _font = SystemFonts.MessageBoxFont;
        public Font Font { get { return this._font; } set { this._font = value; } }

        private Padding _padding = new Padding(5);
        public Padding Padding { get { return this._padding; } set { this._padding = value; } }

        public PopupMessageToolTip()
        {
            this.OwnerDraw = true;
            this.BackColor = Color.DimGray;
            this.ForeColor = Color.White;
            this.Popup += new PopupEventHandler(this.OnPopup);
            this.Draw += new DrawToolTipEventHandler(this.OnDraw);
        }

        public void Show(string text, Control target, Point arrowLocation)
        {
            if (target == null)
                throw new ArgumentException("Target control cannot be null");

            var activeScreen = Screen.FromControl(target).WorkingArea;

            var contentSize = this.GetSize(text);
            var abLoc = target.PointToScreen(arrowLocation);
            abLoc.X -= contentSize.Width;
            abLoc.Y -= contentSize.Height / 2;

            int diffToScreenBottom = activeScreen.Bottom - (abLoc.Y + contentSize.Height);
            if (diffToScreenBottom < 0)
                abLoc.Y += diffToScreenBottom;

            var ttLoc = target.PointToClient(abLoc);
            base.Show(text, target, ttLoc);
        }

        private int ArrowWidth { get { return (int)(0.8 * this.Font.Height); } }

        private void OnPopup(object sender, PopupEventArgs e)
        {
            var text = this.GetToolTip(e.AssociatedControl);
            var ttSize = this.GetSize(text);
            e.ToolTipSize = ttSize;
        }

        /// <summary>
        /// Get the total size of the popup message (including the arrow)
        /// </summary>
        private Size GetSize(string text)
        {
            var txtSize = TextRenderer.MeasureText(text, this.Font, Size.Empty, TextFormatFlags.Internal);
            var ttSize = new Size(txtSize.Width + this.Padding.Left + this.Padding.Right + 10, txtSize.Height + this.Padding.Top + this.Padding.Bottom);
            return ttSize;
        }

        private void OnDraw(object sender, DrawToolTipEventArgs e)
        {
            e.DrawBorder();
            e.DrawBackground();
            using (var brush = new SolidBrush(this.ForeColor))
            {
                e.Graphics.DrawString(e.ToolTipText, this.Font, brush, e.Bounds.X + this.Padding.Left, e.Bounds.Y + this.Padding.Top);
            }
        }
    }
}
