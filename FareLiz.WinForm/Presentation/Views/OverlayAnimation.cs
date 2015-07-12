using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SkyDean.FareLiz.WinForm.Presentation.Views
{
    /// <summary>
    /// Provide overlay form with animating arrow
    /// </summary>
    public partial class OverlayAnimation : Form
    {
        private readonly IDictionary<Control, string> _script;

        private Control _hostControl;
        private Form _parentForm;
        private int _border = 5;
        private int _curBorder = 0;
        private int _borderStep = 1;
        private int _imgStep = 1;
        private int _imgTopLimit;
        private int _imgOffset = 30;
        private Rectangle _controlRect;

        /// <summary>
        /// Initialize a new instance of overlay animator form with the scripts
        /// </summary>
        public OverlayAnimation(IDictionary<Control, string> script)
        {
            if (script == null || script.Count < 1)
                throw new ArgumentException("Script cannot be empty");

            InitializeComponent();
            _script = script;
            AssignMouseHandler(this);
        }

        /// <summary>
        /// Assign the mouse handler to all of child controls so that the script will proceed on mouse click
        /// </summary>
        private void AssignMouseHandler(Control target)
        {
            target.Click += OverlayAnimation_Click;
            foreach (Control c in target.Controls)
                AssignMouseHandler(c);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var rect = _controlRect;
            rect.Inflate(_curBorder, _curBorder);
            using (var pen = new Pen(Color.FromArgb(255 * _curBorder / _border, Color.DarkCyan), _curBorder))
                e.Graphics.DrawRectangle(pen, rect);

            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            base.OnPaint(e);
        }

        private void OverlayAnimation_Shown(object sender, EventArgs e)
        {
            PopScript();
            timer.Enabled = true;
        }

        /// <summary>
        /// Calculate the X-coordinate of the arrow
        /// </summary>
        private int GetXLocation(Control element)
        {
            int widthDiff = (_controlRect.Width - element.Width) / 2;
            int eleX = _controlRect.Left + widthDiff;
            if (eleX + element.Width >= Width)
                eleX = Width - element.Width - 5;
            return eleX;
        }

        /// <summary>
        /// Animate the arrow towards the target control
        /// </summary>
        private void timer_Tick(object sender, EventArgs e)
        {
            // Move the arrow towards the arrow and move up once it touches the control
            if (_curBorder >= _border)
                _borderStep = -1;
            else if (_curBorder < 1)
                _borderStep = 1;
            _curBorder += _borderStep;

            if (imgArrow.Top <= _imgTopLimit)
                _imgStep = 3;
            else if (imgArrow.Bottom >= _controlRect.Top)
                _imgStep = -3;
            imgArrow.Top += _imgStep;

            Invalidate();
        }

        private void OverlayAnimation_Click(object sender, EventArgs e)
        {
            PopScript();
        }

        /// <summary>
        /// Display the next content in the script. If there is no more, close the form
        /// </summary>
        private void PopScript()
        {
            if (_parentForm != null)
                _parentForm.Opacity = 1;

            timer.Enabled = false;
            if (_script.Count < 1)  // Close the form is there is no more script
            {
                Close();
                return;
            }

            // Update the view with the script content
            foreach (var pair in _script)
            {
                _hostControl = pair.Key;
                lblTitle.Text = pair.Value;
                break;
            }

            _script.Remove(_hostControl);

            _parentForm = null;
            var parent = _hostControl.Parent;
            while (parent != null)
            {
                _parentForm = parent as Form;
                if (_parentForm != null)
                    break;
            }

            if (_parentForm == null)
                throw new ArgumentException("The control [" + _hostControl.Name + "] does not belong to any form");

            _parentForm.Opacity = 0.85;
            Size = _parentForm.Size;    // Resize and align the form so that it fully cover the parent form
            Location = _parentForm.Location;
            var screenLoc = _hostControl.PointToScreen(Point.Empty);
            var relLoc = PointToClient(screenLoc);
            _controlRect = new Rectangle(relLoc, _hostControl.Size);

            _imgTopLimit = _controlRect.Top - imgArrow.Height - _imgOffset;
            var arrowX = GetXLocation(imgArrow);
            imgArrow.Location = new Point(arrowX, _imgTopLimit);

            var lblX = GetXLocation(lblTitle);
            int lblY = _imgTopLimit - 10 - lblTitle.Height;
            lblTitle.Location = new Point(lblX, lblY);

            timer.Enabled = true;
        }
    }
}
