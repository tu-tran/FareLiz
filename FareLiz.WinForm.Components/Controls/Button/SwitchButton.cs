namespace SkyDean.FareLiz.WinForm.Components.Controls.Button
{
    using System;
    using System.ComponentModel;
    using System.Drawing;

    /// <summary>
    /// The switch button.
    /// </summary>
    public class SwitchButton : ImageButton
    {
        /// <summary>
        /// The _auto switch state on click.
        /// </summary>
        private bool _autoSwitchStateOnClick = true;

        /// <summary>
        /// The _first state image.
        /// </summary>
        private Image _firstStateImage;

        /// <summary>
        /// The _first state text.
        /// </summary>
        private string _firstStateText;

        /// <summary>
        /// The _is second state.
        /// </summary>
        private bool _isSecondState;

        /// <summary>
        /// The _second state image.
        /// </summary>
        private Image _secondStateImage;

        /// <summary>
        /// The _second state text.
        /// </summary>
        private string _secondStateText;

        /// <summary>
        /// Gets or sets a value indicating whether auto switch state on click.
        /// </summary>
        public bool AutoSwitchStateOnClick
        {
            get
            {
                return this._autoSwitchStateOnClick;
            }

            set
            {
                this._autoSwitchStateOnClick = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is second state.
        /// </summary>
        public bool IsSecondState
        {
            get
            {
                return this._isSecondState;
            }

            set
            {
                if (this.StateChanging != null)
                {
                    var arg = new CancelEventArgs();
                    this.StateChanging(this, arg);
                    if (arg.Cancel)
                    {
                        return;
                    }
                }

                this._isSecondState = value;
                this.UpdateState();
            }
        }

        /// <summary>
        /// Gets or sets the first state image.
        /// </summary>
        [Localizable(true)]
        public Image FirstStateImage
        {
            get
            {
                return this._firstStateImage;
            }

            set
            {
                this._firstStateImage = value;
                if (!this.IsSecondState)
                {
                    this.Image = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the second state image.
        /// </summary>
        [Localizable(true)]
        public Image SecondStateImage
        {
            get
            {
                return this._secondStateImage;
            }

            set
            {
                this._secondStateImage = value;
                if (this.IsSecondState)
                {
                    this.Image = value;
                }
            }
        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Image Image
        {
            get
            {
                return base.Image;
            }

            private set
            {
                base.Image = value;
            }
        }

        /// <summary>
        /// Gets or sets the first state text.
        /// </summary>
        [Localizable(true)]
        public string FirstStateText
        {
            get
            {
                return this._firstStateText;
            }

            set
            {
                this._firstStateText = value;
                if (!this.IsSecondState)
                {
                    this.Text = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the second state text.
        /// </summary>
        [Localizable(true)]
        public string SecondStateText
        {
            get
            {
                return this._secondStateText;
            }

            set
            {
                this._secondStateText = value;
                if (this.IsSecondState)
                {
                    this.Text = value;
                }
            }
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new string Text
        {
            get
            {
                return base.Text;
            }

            private set
            {
                base.Text = value;
            }
        }

        /// <summary>The next mouse click won't change the button state</summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SuppressStateSwitch { get; set; }

        /// <summary>
        /// The state changing.
        /// </summary>
        public event CancelEventHandler StateChanging;

        /// <summary>
        /// The on click.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (this.SuppressStateSwitch)
            {
                this.SuppressStateSwitch = false;
                return;
            }

            if (this.AutoSwitchStateOnClick)
            {
                this.IsSecondState = !this.IsSecondState;
            }
        }

        /// <summary>
        /// The update state.
        /// </summary>
        private void UpdateState()
        {
            this.Image = this.IsSecondState ? this.SecondStateImage : this.FirstStateImage;
            this.Text = this.IsSecondState ? this.SecondStateText : this.FirstStateText;
        }
    }
}