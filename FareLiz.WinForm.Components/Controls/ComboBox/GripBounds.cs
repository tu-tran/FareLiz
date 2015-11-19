namespace SkyDean.FareLiz.WinForm.Components.Controls.ComboBox
{
    using System.Drawing;

    /// <summary>CodeProject.com "Simple pop-up control" "http://www.codeproject.com/cs/miscctrl/simplepopup.asp".</summary>
    internal struct GripBounds
    {
        /// <summary>
        /// The grip size.
        /// </summary>
        private const int GripSize = 6;

        /// <summary>
        /// The corner grip size.
        /// </summary>
        private const int CornerGripSize = GripSize << 1;

        /// <summary>
        /// The client rectangle.
        /// </summary>
        private readonly Rectangle clientRectangle;

        /// <summary>
        /// Initializes a new instance of the <see cref="GripBounds"/> struct.
        /// </summary>
        /// <param name="clientRectangle">
        /// The client rectangle.
        /// </param>
        public GripBounds(Rectangle clientRectangle)
        {
            this.clientRectangle = clientRectangle;
        }

        /// <summary>
        /// Gets the client rectangle.
        /// </summary>
        public Rectangle ClientRectangle
        {
            get
            {
                return this.clientRectangle;
            }

            // set { clientRectangle = value; }
        }

        /// <summary>
        /// Gets the bottom.
        /// </summary>
        public Rectangle Bottom
        {
            get
            {
                Rectangle rect = this.ClientRectangle;
                rect.Y = rect.Bottom - GripSize + 1;
                rect.Height = GripSize;
                return rect;
            }
        }

        /// <summary>
        /// Gets the bottom right.
        /// </summary>
        public Rectangle BottomRight
        {
            get
            {
                Rectangle rect = this.ClientRectangle;
                rect.Y = rect.Bottom - CornerGripSize + 1;
                rect.Height = CornerGripSize;
                rect.X = rect.Width - CornerGripSize + 1;
                rect.Width = CornerGripSize;
                return rect;
            }
        }

        /// <summary>
        /// Gets the top.
        /// </summary>
        public Rectangle Top
        {
            get
            {
                Rectangle rect = this.ClientRectangle;
                rect.Height = GripSize;
                return rect;
            }
        }

        /// <summary>
        /// Gets the top right.
        /// </summary>
        public Rectangle TopRight
        {
            get
            {
                Rectangle rect = this.ClientRectangle;
                rect.Height = CornerGripSize;
                rect.X = rect.Width - CornerGripSize + 1;
                rect.Width = CornerGripSize;
                return rect;
            }
        }

        /// <summary>
        /// Gets the left.
        /// </summary>
        public Rectangle Left
        {
            get
            {
                Rectangle rect = this.ClientRectangle;
                rect.Width = GripSize;
                return rect;
            }
        }

        /// <summary>
        /// Gets the bottom left.
        /// </summary>
        public Rectangle BottomLeft
        {
            get
            {
                Rectangle rect = this.ClientRectangle;
                rect.Width = CornerGripSize;
                rect.Y = rect.Height - CornerGripSize + 1;
                rect.Height = CornerGripSize;
                return rect;
            }
        }

        /// <summary>
        /// Gets the right.
        /// </summary>
        public Rectangle Right
        {
            get
            {
                Rectangle rect = this.ClientRectangle;
                rect.X = rect.Right - GripSize + 1;
                rect.Width = GripSize;
                return rect;
            }
        }

        /// <summary>
        /// Gets the top left.
        /// </summary>
        public Rectangle TopLeft
        {
            get
            {
                Rectangle rect = this.ClientRectangle;
                rect.Width = CornerGripSize;
                rect.Height = CornerGripSize;
                return rect;
            }
        }
    }
}