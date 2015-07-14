namespace SkyDean.FareLiz.WinForm.Components.Controls.Custom
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>The popup message tool tip.</summary>
    public class PopupMessageToolTip : ToolTip
    {
        /// <summary>The _font.</summary>
        private Font _font = SystemFonts.MessageBoxFont;

        /// <summary>The _padding.</summary>
        private Padding _padding = new Padding(5);

        /// <summary>Initializes a new instance of the <see cref="PopupMessageToolTip" /> class.</summary>
        public PopupMessageToolTip()
        {
            this.OwnerDraw = true;
            this.BackColor = Color.DimGray;
            this.ForeColor = Color.White;
            this.Popup += this.OnPopup;
            this.Draw += this.OnDraw;
        }

        /// <summary>Gets or sets the font.</summary>
        public Font Font
        {
            get
            {
                return this._font;
            }

            set
            {
                this._font = value;
            }
        }

        /// <summary>Gets or sets the padding.</summary>
        public Padding Padding
        {
            get
            {
                return this._padding;
            }

            set
            {
                this._padding = value;
            }
        }

        /// <summary>Gets the arrow width.</summary>
        private int ArrowWidth
        {
            get
            {
                return (int)(0.8 * this.Font.Height);
            }
        }

        /// <summary>
        /// The show.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="arrowLocation">
        /// The arrow location.
        /// </param>
        /// <exception cref="ArgumentException">
        /// </exception>
        public void Show(string text, Control target, Point arrowLocation)
        {
            if (target == null)
            {
                throw new ArgumentException("Target control cannot be null");
            }

            var activeScreen = Screen.FromControl(target).WorkingArea;

            var contentSize = this.GetSize(text);
            var abLoc = target.PointToScreen(arrowLocation);
            abLoc.X -= contentSize.Width;
            abLoc.Y -= contentSize.Height / 2;

            var diffToScreenBottom = activeScreen.Bottom - (abLoc.Y + contentSize.Height);
            if (diffToScreenBottom < 0)
            {
                abLoc.Y += diffToScreenBottom;
            }

            var ttLoc = target.PointToClient(abLoc);
            this.Show(text, target, ttLoc);
        }

        /// <summary>
        /// The on popup.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnPopup(object sender, PopupEventArgs e)
        {
            var text = this.GetToolTip(e.AssociatedControl);
            var ttSize = this.GetSize(text);
            e.ToolTipSize = ttSize;
        }

        /// <summary>
        /// Get the total size of the popup message (including the arrow)
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="Size"/>.
        /// </returns>
        private Size GetSize(string text)
        {
            var txtSize = TextRenderer.MeasureText(text, this.Font, Size.Empty, TextFormatFlags.Internal);
            var ttSize = new Size(
                txtSize.Width + this.Padding.Left + this.Padding.Right + 10, 
                txtSize.Height + this.Padding.Top + this.Padding.Bottom);
            return ttSize;
        }

        /// <summary>
        /// The on draw.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
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