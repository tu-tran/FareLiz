﻿namespace SkyDean.FareLiz.WinForm.Components.Controls.ComboBox
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CheckBoxProperties
    {
        #region PRIVATE PROPERTIES

        private Appearance _Appearance = Appearance.Normal;
        private bool _AutoCheck = true;
        private bool _AutoEllipsis;
        private bool _AutoSize = true;
        private ContentAlignment _CheckAlign = ContentAlignment.MiddleLeft;
        private Color _FlatAppearanceBorderColor = Color.Empty;
        private int _FlatAppearanceBorderSize = 1;
        private Color _FlatAppearanceCheckedBackColor = Color.Empty;
        private Color _FlatAppearanceMouseDownBackColor = Color.Empty;
        private Color _FlatAppearanceMouseOverBackColor = Color.Empty;
        private FlatStyle _FlatStyle = FlatStyle.Standard;
        private Color _ForeColor = SystemColors.ControlText;
        private RightToLeft _RightToLeft = RightToLeft.No;
        private ContentAlignment _TextAlign = ContentAlignment.MiddleLeft;
        private bool _ThreeState;

        #endregion

        #region PUBLIC PROPERTIES

        [DefaultValue(Appearance.Normal)]
        public Appearance Appearance
        {
            get { return this._Appearance; }
            set
            {
                this._Appearance = value;
                this.OnPropertyChanged();
            }
        }

        [DefaultValue(true)]
        public bool AutoCheck
        {
            get { return this._AutoCheck; }
            set
            {
                this._AutoCheck = value;
                this.OnPropertyChanged();
            }
        }

        [DefaultValue(false)]
        public bool AutoEllipsis
        {
            get { return this._AutoEllipsis; }
            set
            {
                this._AutoEllipsis = value;
                this.OnPropertyChanged();
            }
        }

        [DefaultValue(true)]
        public bool AutoSize
        {
            get { return this._AutoSize; }
            set
            {
                this._AutoSize = true;
                this.OnPropertyChanged();
            }
        }

        [DefaultValue(ContentAlignment.MiddleLeft)]
        public ContentAlignment CheckAlign
        {
            get { return this._CheckAlign; }
            set
            {
                this._CheckAlign = value;
                this.OnPropertyChanged();
            }
        }

        [DefaultValue(typeof(Color), "")]
        public Color FlatAppearanceBorderColor
        {
            get { return this._FlatAppearanceBorderColor; }
            set
            {
                this._FlatAppearanceBorderColor = value;
                this.OnPropertyChanged();
            }
        }

        [DefaultValue(1)]
        public int FlatAppearanceBorderSize
        {
            get { return this._FlatAppearanceBorderSize; }
            set
            {
                this._FlatAppearanceBorderSize = value;
                this.OnPropertyChanged();
            }
        }

        [DefaultValue(typeof(Color), "")]
        public Color FlatAppearanceCheckedBackColor
        {
            get { return this._FlatAppearanceCheckedBackColor; }
            set
            {
                this._FlatAppearanceCheckedBackColor = value;
                this.OnPropertyChanged();
            }
        }

        [DefaultValue(typeof(Color), "")]
        public Color FlatAppearanceMouseDownBackColor
        {
            get { return this._FlatAppearanceMouseDownBackColor; }
            set
            {
                this._FlatAppearanceMouseDownBackColor = value;
                this.OnPropertyChanged();
            }
        }

        [DefaultValue(typeof(Color), "")]
        public Color FlatAppearanceMouseOverBackColor
        {
            get { return this._FlatAppearanceMouseOverBackColor; }
            set
            {
                this._FlatAppearanceMouseOverBackColor = value;
                this.OnPropertyChanged();
            }
        }

        [DefaultValue(FlatStyle.Popup)]
        public FlatStyle FlatStyle
        {
            get { return this._FlatStyle; }
            set
            {
                this._FlatStyle = value;
                this.OnPropertyChanged();
            }
        }

        [DefaultValue(typeof(SystemColors), "ControlText")]
        public Color ForeColor
        {
            get { return this._ForeColor; }
            set
            {
                this._ForeColor = value;
                this.OnPropertyChanged();
            }
        }

        [DefaultValue(RightToLeft.No)]
        public RightToLeft RightToLeft
        {
            get { return this._RightToLeft; }
            set
            {
                this._RightToLeft = value;
                this.OnPropertyChanged();
            }
        }

        [DefaultValue(ContentAlignment.MiddleLeft)]
        public ContentAlignment TextAlign
        {
            get { return this._TextAlign; }
            set
            {
                this._TextAlign = value;
                this.OnPropertyChanged();
            }
        }

        [DefaultValue(false)]
        public bool ThreeState
        {
            get { return this._ThreeState; }
            set
            {
                this._ThreeState = value;
                this.OnPropertyChanged();
            }
        }

        #endregion

        #region EVENTS AND EVENT CALLERS

        /// <summary>
        ///     Called when any property changes.
        /// </summary>
        public event EventHandler PropertyChanged;

        protected void OnPropertyChanged()
        {
            EventHandler handler = this.PropertyChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion
    }
}
