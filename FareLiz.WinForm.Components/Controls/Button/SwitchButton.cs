namespace SkyDean.FareLiz.WinForm.Components.Controls.Button
{
    using System;
    using System.ComponentModel;
    using System.Drawing;

    public class SwitchButton : ImageButton
    {
        public event CancelEventHandler StateChanging;

        private bool _autoSwitchStateOnClick = true;
        public bool AutoSwitchStateOnClick { get { return this._autoSwitchStateOnClick; } set { this._autoSwitchStateOnClick = value; } }

        private bool _isSecondState = false;
        public bool IsSecondState
        {
            get { return this._isSecondState; }
            set
            {
                if (this.StateChanging != null)
                {
                    var arg = new CancelEventArgs();
                    this.StateChanging(this, arg);
                    if (arg.Cancel)
                        return;
                }
                this._isSecondState = value;
                this.UpdateState();
            }
        }

        private Image _firstStateImage;
        [Localizable(true)]
        public Image FirstStateImage
        {
            get { return this._firstStateImage; }
            set
            {
                this._firstStateImage = value;
                if (!this.IsSecondState)
                    this.Image = value;
            }
        }

        private Image _secondStateImage;
        [Localizable(true)]
        public Image SecondStateImage
        {
            get { return this._secondStateImage; }
            set
            {
                this._secondStateImage = value;
                if (this.IsSecondState)
                    this.Image = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Image Image { get { return base.Image; } private set { base.Image = value; } }

        private string _firstStateText;
        [Localizable(true)]
        public string FirstStateText
        {
            get { return this._firstStateText; }
            set
            {
                this._firstStateText = value;
                if (!this.IsSecondState)
                    this.Text = value;
            }
        }

        private string _secondStateText;
        [Localizable(true)]
        public string SecondStateText
        {
            get { return this._secondStateText; }
            set
            {
                this._secondStateText = value;
                if (this.IsSecondState)
                    this.Text = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new string Text { get { return base.Text; } private set { base.Text = value; } }

        /// <summary>
        /// The next mouse click won't change the button state
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SuppressStateSwitch { get; set; }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (this.SuppressStateSwitch)
            {
                this.SuppressStateSwitch = false;
                return;
            }

            if (this.AutoSwitchStateOnClick)
                this.IsSecondState = !this.IsSecondState;
        }

        private void UpdateState()
        {
            this.Image = this.IsSecondState ? this.SecondStateImage : this.FirstStateImage;
            this.Text = this.IsSecondState ? this.SecondStateText : this.FirstStateText;
        }
    }
}
