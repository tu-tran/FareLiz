namespace SkyDean.FareLiz.WinForm.Components.Controls.TabControl
{
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    partial class FlatTabControl
    {
        /// <summary>
        /// Get the solid brush for tab page background
        /// </summary>
        private Brush GetPageBackgroundBrush(int index)
        {
            //	Capture the colours dependant on selection RequestState of the tab
            Brush result;
            if (!this.TabPages[index].Enabled)
                result = new SolidBrush(Color.Silver);  // For disabled tab
            else
            {
                if (this.SelectedIndex == index)
                    result = new SolidBrush(Color.LightGray);
                else
                {
                    var tabBackColor = this.TabPages[index].BackColor;
                    if (this.HotTrack && index == this.ActiveIndex)
                        result = new SolidBrush(ControlPaint.LightLight(tabBackColor));
                    else
                        result = new SolidBrush(tabBackColor);
                }
            }

            return result;
        }

        private void AddPageBorder(GraphicsPath path, Rectangle pageBounds, Rectangle tabBounds)
        {
            switch (this.Alignment)
            {
                case TabAlignment.Top:
                    path.AddLine(tabBounds.Right, pageBounds.Y, pageBounds.Right, pageBounds.Y);
                    path.AddLine(pageBounds.Right, pageBounds.Y, pageBounds.Right, pageBounds.Bottom);
                    path.AddLine(pageBounds.Right, pageBounds.Bottom, pageBounds.X, pageBounds.Bottom);
                    path.AddLine(pageBounds.X, pageBounds.Bottom, pageBounds.X, pageBounds.Y);
                    path.AddLine(pageBounds.X, pageBounds.Y, tabBounds.X, pageBounds.Y);
                    break;
                case TabAlignment.Bottom:
                    path.AddLine(tabBounds.X, pageBounds.Bottom, pageBounds.X, pageBounds.Bottom);
                    path.AddLine(pageBounds.X, pageBounds.Bottom, pageBounds.X, pageBounds.Y);
                    path.AddLine(pageBounds.X, pageBounds.Y, pageBounds.Right, pageBounds.Y);
                    path.AddLine(pageBounds.Right, pageBounds.Y, pageBounds.Right, pageBounds.Bottom);
                    path.AddLine(pageBounds.Right, pageBounds.Bottom, tabBounds.Right, pageBounds.Bottom);
                    break;
                case TabAlignment.Left:
                    path.AddLine(pageBounds.X, tabBounds.Y, pageBounds.X, pageBounds.Y);
                    path.AddLine(pageBounds.X, pageBounds.Y, pageBounds.Right, pageBounds.Y);
                    path.AddLine(pageBounds.Right, pageBounds.Y, pageBounds.Right, pageBounds.Bottom);
                    path.AddLine(pageBounds.Right, pageBounds.Bottom, pageBounds.X, pageBounds.Bottom);
                    path.AddLine(pageBounds.X, pageBounds.Bottom, pageBounds.X, tabBounds.Bottom);
                    break;
                case TabAlignment.Right:
                    path.AddLine(pageBounds.Right, tabBounds.Bottom, pageBounds.Right, pageBounds.Bottom);
                    path.AddLine(pageBounds.Right, pageBounds.Bottom, pageBounds.X, pageBounds.Bottom);
                    path.AddLine(pageBounds.X, pageBounds.Bottom, pageBounds.X, pageBounds.Y);
                    path.AddLine(pageBounds.X, pageBounds.Y, pageBounds.Right, pageBounds.Y);
                    path.AddLine(pageBounds.Right, pageBounds.Y, pageBounds.Right, tabBounds.Y);
                    break;
            }
        }

        private Rectangle GetPageBounds(int index)
        {
            Rectangle pageBounds = this.TabPages[index].Bounds;
            pageBounds.Width += 1;
            pageBounds.Height += 1;
            pageBounds.X -= 1;
            pageBounds.Y -= 1;

            if (pageBounds.Bottom > this.Height - 4)
            {
                pageBounds.Height -= (pageBounds.Bottom - this.Height + 4);
            }
            return pageBounds;
        }

        private GraphicsPath GetTabPageBorder(int index)
        {
            GraphicsPath path = new GraphicsPath();
            Rectangle pageBounds = this.GetPageBounds(index);
            Rectangle tabBounds = this.GetTabRect(index);
            this.AddTabBorder(path, tabBounds);
            this.AddPageBorder(path, pageBounds, tabBounds);

            path.CloseFigure();
            return path;
        }
    }
}
