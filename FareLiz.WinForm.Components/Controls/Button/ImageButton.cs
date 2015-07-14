namespace SkyDean.FareLiz.WinForm.Components.Controls.Button
{
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>The image button.</summary>
    public class ImageButton : Button
    {
        /// <summary>The _auto align.</summary>
        private bool _autoAlign = true;

        /// <summary>The _is aligning.</summary>
        private bool _isAligning;

        /// <summary>Gets or sets a value indicating whether auto align.</summary>
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool AutoAlign
        {
            get
            {
                return this._autoAlign;
            }

            set
            {
                this._autoAlign = value;
            }
        }

        /// <summary>Gets or sets the image.</summary>
        public new Image Image
        {
            get
            {
                return base.Image;
            }

            set
            {
                base.Image = value;
                if (this.AutoAlign)
                {
                    this.AutoPadding();
                }
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
                base.Text = value;
                if (this.AutoAlign)
                {
                    this.AutoPadding();
                }
            }
        }

        /// <summary>Gets or sets the padding.</summary>
        public new Padding Padding
        {
            get
            {
                return base.Padding;
            }

            set
            {
                base.Padding = value;
                if (this.AutoAlign)
                {
                    this.AutoPadding();
                }
            }
        }

        /// <summary>Gets or sets the size.</summary>
        public new Size Size
        {
            get
            {
                return base.Size;
            }

            set
            {
                base.Size = value;
                if (this.AutoAlign)
                {
                    this.AutoPadding();
                }
            }
        }

        /// <summary>Gets or sets the text align.</summary>
        public override ContentAlignment TextAlign
        {
            get
            {
                return this.AutoAlign ? ContentAlignment.MiddleRight : base.TextAlign;
            }

            set
            {
                base.TextAlign = this.AutoAlign ? ContentAlignment.MiddleRight : value;
            }
        }

        /// <summary>The auto padding.</summary>
        public void AutoPadding()
        {
            if (!this._isAligning && this.Image != null)
            {
                this._isAligning = true;
                var imgWidth = this.Image.Width + this.Padding.Left + 6;
                var txtAreaSize = new Size(this.Width - imgWidth, this.Height - 6);
                var txtSize = TextRenderer.MeasureText(base.Text, this.Font, txtAreaSize);
                if (txtSize.Width > txtAreaSize.Width)
                {
                    txtSize.Width = txtAreaSize.Width;
                }

                base.Padding = new Padding(this.Padding.Left, this.Padding.Top, (txtAreaSize.Width - txtSize.Width) / 2, this.Padding.Bottom);
                this._isAligning = false;
            }
        }
    }
}