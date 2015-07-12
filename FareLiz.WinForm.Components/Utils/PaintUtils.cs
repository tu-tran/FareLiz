namespace SkyDean.FareLiz.WinForm.Components.Utils
{
    using System.Drawing;
    using System.Drawing.Drawing2D;

    internal static class PaintUtils
    {
        internal static GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius, int borderWidth)
        {
            int x = rect.X, y = rect.Y, h = rect.Height - borderWidth, w = rect.Width - borderWidth;

            var p = new GraphicsPath();
            p.StartFigure();
            p.AddArc(x, y, 2 * radius, 2 * radius, 180, 90);
            p.AddLine(x + radius, y, x + w - radius, y);
            p.AddArc(x + w - 2 * radius, y, 2 * radius, 2 * radius, 270, 90);
            p.AddLine(x + w, y + radius, x + w, y + h - radius);
            p.AddArc(x + w - 2 * radius, y + h - 2 * radius, 2 * radius, 2 * radius, 0, 90);
            p.AddLine(x + w - radius, y + h, x + radius, y + h);
            p.AddArc(x, y + h - 2 * radius, 2 * radius, 2 * radius, 90, 90);
            p.AddLine(x, y + h - radius, x, y + radius);
            p.CloseFigure();

            return p;
        }
    }
}
