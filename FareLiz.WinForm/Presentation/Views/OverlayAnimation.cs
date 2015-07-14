namespace SkyDean.FareLiz.WinForm.Presentation.Views
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    /// <summary>Provide overlay form with animating arrow</summary>
    public partial class OverlayAnimation : Form
    {
        /// <summary>The _border.</summary>
        private readonly int _border = 5;

        /// <summary>The _img offset.</summary>
        private readonly int _imgOffset = 30;

        /// <summary>The _script.</summary>
        private readonly IDictionary<Control, string> _script;

        /// <summary>The _border step.</summary>
        private int _borderStep = 1;

        /// <summary>The _control rect.</summary>
        private Rectangle _controlRect;

        /// <summary>The _cur border.</summary>
        private int _curBorder;

        /// <summary>The _host control.</summary>
        private Control _hostControl;

        /// <summary>The _img step.</summary>
        private int _imgStep = 1;

        /// <summary>The _img top limit.</summary>
        private int _imgTopLimit;

        /// <summary>The _parent form.</summary>
        private Form _parentForm;

        /// <summary>
        /// Initializes a new instance of the <see cref="OverlayAnimation"/> class. Initialize a new instance of overlay animator form with the scripts
        /// </summary>
        /// <param name="script">
        /// The script.
        /// </param>
        public OverlayAnimation(IDictionary<Control, string> script)
        {
            if (script == null || script.Count < 1)
            {
                throw new ArgumentException("Script cannot be empty");
            }

            this.InitializeComponent();
            this._script = script;
            this.AssignMouseHandler(this);
        }

        /// <summary>
        /// Assign the mouse handler to all of child controls so that the script will proceed on mouse click
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        private void AssignMouseHandler(Control target)
        {
            target.Click += this.OverlayAnimation_Click;
            foreach (Control c in target.Controls)
            {
                this.AssignMouseHandler(c);
            }
        }

        /// <summary>
        /// The on paint.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnPaint(PaintEventArgs e)
        {
            var rect = this._controlRect;
            rect.Inflate(this._curBorder, this._curBorder);
            using (var pen = new Pen(Color.FromArgb(255 * this._curBorder / this._border, Color.DarkCyan), this._curBorder)) e.Graphics.DrawRectangle(pen, rect);

            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            base.OnPaint(e);
        }

        /// <summary>
        /// The overlay animation_ shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OverlayAnimation_Shown(object sender, EventArgs e)
        {
            this.PopScript();
            this.timer.Enabled = true;
        }

        /// <summary>
        /// Calculate the X-coordinate of the arrow
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int GetXLocation(Control element)
        {
            var widthDiff = (this._controlRect.Width - element.Width) / 2;
            var eleX = this._controlRect.Left + widthDiff;
            if (eleX + element.Width >= this.Width)
            {
                eleX = this.Width - element.Width - 5;
            }

            return eleX;
        }

        /// <summary>
        /// Animate the arrow towards the target control
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void timer_Tick(object sender, EventArgs e)
        {
            // Move the arrow towards the arrow and move up once it touches the control
            if (this._curBorder >= this._border)
            {
                this._borderStep = -1;
            }
            else if (this._curBorder < 1)
            {
                this._borderStep = 1;
            }

            this._curBorder += this._borderStep;

            if (this.imgArrow.Top <= this._imgTopLimit)
            {
                this._imgStep = 3;
            }
            else if (this.imgArrow.Bottom >= this._controlRect.Top)
            {
                this._imgStep = -3;
            }

            this.imgArrow.Top += this._imgStep;

            this.Invalidate();
        }

        /// <summary>
        /// The overlay animation_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OverlayAnimation_Click(object sender, EventArgs e)
        {
            this.PopScript();
        }

        /// <summary>Display the next content in the script. If there is no more, close the form</summary>
        private void PopScript()
        {
            if (this._parentForm != null)
            {
                this._parentForm.Opacity = 1;
            }

            this.timer.Enabled = false;
            if (this._script.Count < 1)
            {
                // Close the form is there is no more script
                this.Close();
                return;
            }

            // Update the view with the script content
            foreach (var pair in this._script)
            {
                this._hostControl = pair.Key;
                this.lblTitle.Text = pair.Value;
                break;
            }

            this._script.Remove(this._hostControl);

            this._parentForm = null;
            var parent = this._hostControl.Parent;
            while (parent != null)
            {
                this._parentForm = parent as Form;
                if (this._parentForm != null)
                {
                    break;
                }
            }

            if (this._parentForm == null)
            {
                throw new ArgumentException("The control [" + this._hostControl.Name + "] does not belong to any form");
            }

            this._parentForm.Opacity = 0.85;
            this.Size = this._parentForm.Size; // Resize and align the form so that it fully cover the parent form
            this.Location = this._parentForm.Location;
            var screenLoc = this._hostControl.PointToScreen(Point.Empty);
            var relLoc = this.PointToClient(screenLoc);
            this._controlRect = new Rectangle(relLoc, this._hostControl.Size);

            this._imgTopLimit = this._controlRect.Top - this.imgArrow.Height - this._imgOffset;
            var arrowX = this.GetXLocation(this.imgArrow);
            this.imgArrow.Location = new Point(arrowX, this._imgTopLimit);

            var lblX = this.GetXLocation(this.lblTitle);
            var lblY = this._imgTopLimit - 10 - this.lblTitle.Height;
            this.lblTitle.Location = new Point(lblX, lblY);

            this.timer.Enabled = true;
        }
    }
}