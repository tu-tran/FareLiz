namespace SkyDean.FareLiz.WinForm.Components.Controls.ComboBox
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// The check box properties.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CheckBoxProperties
    {
        #region PRIVATE PROPERTIES

        /// <summary>
        /// The _ appearance.
        /// </summary>
        private Appearance _Appearance = Appearance.Normal;

        /// <summary>
        /// The _ auto check.
        /// </summary>
        private bool _AutoCheck = true;

        /// <summary>
        /// The _ auto ellipsis.
        /// </summary>
        private bool _AutoEllipsis;

        /// <summary>
        /// The _ auto size.
        /// </summary>
        private bool _AutoSize = true;

        /// <summary>
        /// The _ check align.
        /// </summary>
        private ContentAlignment _CheckAlign = ContentAlignment.MiddleLeft;

        /// <summary>
        /// The _ flat appearance border color.
        /// </summary>
        private Color _FlatAppearanceBorderColor = Color.Empty;

        /// <summary>
        /// The _ flat appearance border size.
        /// </summary>
        private int _FlatAppearanceBorderSize = 1;

        /// <summary>
        /// The _ flat appearance checked back color.
        /// </summary>
        private Color _FlatAppearanceCheckedBackColor = Color.Empty;

        /// <summary>
        /// The _ flat appearance mouse down back color.
        /// </summary>
        private Color _FlatAppearanceMouseDownBackColor = Color.Empty;

        /// <summary>
        /// The _ flat appearance mouse over back color.
        /// </summary>
        private Color _FlatAppearanceMouseOverBackColor = Color.Empty;

        /// <summary>
        /// The _ flat style.
        /// </summary>
        private FlatStyle _FlatStyle = FlatStyle.Standard;

        /// <summary>
        /// The _ fore color.
        /// </summary>
        private Color _ForeColor = SystemColors.ControlText;

        /// <summary>
        /// The _ right to left.
        /// </summary>
        private RightToLeft _RightToLeft = RightToLeft.No;

        /// <summary>
        /// The _ text align.
        /// </summary>
        private ContentAlignment _TextAlign = ContentAlignment.MiddleLeft;

        /// <summary>
        /// The _ three state.
        /// </summary>
        private bool _ThreeState;

        #endregion

        #region PUBLIC PROPERTIES

        /// <summary>
        /// Gets or sets the appearance.
        /// </summary>
        [DefaultValue(Appearance.Normal)]
        public Appearance Appearance
        {
            get
            {
                return this._Appearance;
            }

            set
            {
                this._Appearance = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether auto check.
        /// </summary>
        [DefaultValue(true)]
        public bool AutoCheck
        {
            get
            {
                return this._AutoCheck;
            }

            set
            {
                this._AutoCheck = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether auto ellipsis.
        /// </summary>
        [DefaultValue(false)]
        public bool AutoEllipsis
        {
            get
            {
                return this._AutoEllipsis;
            }

            set
            {
                this._AutoEllipsis = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether auto size.
        /// </summary>
        [DefaultValue(true)]
        public bool AutoSize
        {
            get
            {
                return this._AutoSize;
            }

            set
            {
                this._AutoSize = true;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the check align.
        /// </summary>
        [DefaultValue(ContentAlignment.MiddleLeft)]
        public ContentAlignment CheckAlign
        {
            get
            {
                return this._CheckAlign;
            }

            set
            {
                this._CheckAlign = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the flat appearance border color.
        /// </summary>
        [DefaultValue(typeof(Color), "")]
        public Color FlatAppearanceBorderColor
        {
            get
            {
                return this._FlatAppearanceBorderColor;
            }

            set
            {
                this._FlatAppearanceBorderColor = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the flat appearance border size.
        /// </summary>
        [DefaultValue(1)]
        public int FlatAppearanceBorderSize
        {
            get
            {
                return this._FlatAppearanceBorderSize;
            }

            set
            {
                this._FlatAppearanceBorderSize = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the flat appearance checked back color.
        /// </summary>
        [DefaultValue(typeof(Color), "")]
        public Color FlatAppearanceCheckedBackColor
        {
            get
            {
                return this._FlatAppearanceCheckedBackColor;
            }

            set
            {
                this._FlatAppearanceCheckedBackColor = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the flat appearance mouse down back color.
        /// </summary>
        [DefaultValue(typeof(Color), "")]
        public Color FlatAppearanceMouseDownBackColor
        {
            get
            {
                return this._FlatAppearanceMouseDownBackColor;
            }

            set
            {
                this._FlatAppearanceMouseDownBackColor = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the flat appearance mouse over back color.
        /// </summary>
        [DefaultValue(typeof(Color), "")]
        public Color FlatAppearanceMouseOverBackColor
        {
            get
            {
                return this._FlatAppearanceMouseOverBackColor;
            }

            set
            {
                this._FlatAppearanceMouseOverBackColor = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the flat style.
        /// </summary>
        [DefaultValue(FlatStyle.Popup)]
        public FlatStyle FlatStyle
        {
            get
            {
                return this._FlatStyle;
            }

            set
            {
                this._FlatStyle = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the fore color.
        /// </summary>
        [DefaultValue(typeof(SystemColors), "ControlText")]
        public Color ForeColor
        {
            get
            {
                return this._ForeColor;
            }

            set
            {
                this._ForeColor = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the right to left.
        /// </summary>
        [DefaultValue(RightToLeft.No)]
        public RightToLeft RightToLeft
        {
            get
            {
                return this._RightToLeft;
            }

            set
            {
                this._RightToLeft = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the text align.
        /// </summary>
        [DefaultValue(ContentAlignment.MiddleLeft)]
        public ContentAlignment TextAlign
        {
            get
            {
                return this._TextAlign;
            }

            set
            {
                this._TextAlign = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether three state.
        /// </summary>
        [DefaultValue(false)]
        public bool ThreeState
        {
            get
            {
                return this._ThreeState;
            }

            set
            {
                this._ThreeState = value;
                this.OnPropertyChanged();
            }
        }

        #endregion

        #region EVENTS AND EVENT CALLERS

        /// <summary>Called when any property changes.</summary>
        public event EventHandler PropertyChanged;

        /// <summary>
        /// The on property changed.
        /// </summary>
        protected void OnPropertyChanged()
        {
            EventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}