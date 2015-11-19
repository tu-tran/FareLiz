namespace SkyDean.FareLiz.WinForm
{
    using System;
    using System.ComponentModel;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Data.Config;
    using SkyDean.FareLiz.WinForm.Data;

    /// <summary>
    /// The execution info.
    /// </summary>
    [Serializable]
    [DefaultProperty("IsMinimized")]
    public class ExecutionInfo
    {
        /// <summary>
        /// The _departure date range.
        /// </summary>
        private DateRangeDiff _departureDateRange = new DateRangeDiff(0, 0);

        /// <summary>
        /// The _max stay duration.
        /// </summary>
        private int _maxStayDuration = 1;

        /// <summary>
        /// The _min stay duration.
        /// </summary>
        private int _minStayDuration = 1;

        /// <summary>
        /// The _price limit.
        /// </summary>
        private int _priceLimit = 1;

        /// <summary>
        /// The _return date range.
        /// </summary>
        private DateRangeDiff _returnDateRange = new DateRangeDiff(0, 0);

        /// <summary>
        /// Gets or sets a value indicating whether is minimized.
        /// </summary>
        [DisplayName("Start application minimized")]
        [Description("Start application minimized")]
        [Category("Application Settings")]
        [DefaultValue(true)]
        public bool IsMinimized { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether auto sync.
        /// </summary>
        [DisplayName("Auto-synchronize new data")]
        [Description("Send database to synchronizing service after checking for new fare data. This parameter is for operation mode CloseAndExport")]
        [Category("Application Settings")]
        [DefaultValue(true)]
        public bool AutoSync { get; set; }

        /// <summary>
        /// Gets or sets the departure.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [IniConfigurable(true, "Departure")]
        public Airport Departure { get; set; }

        /// <summary>
        /// Gets or sets the destination.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [IniConfigurable(true, "Destination")]
        public Airport Destination { get; set; }

        /// <summary>
        /// Gets or sets the departure date.
        /// </summary>
        [DisplayName("Departure date")]
        [Description("Flight departure date")]
        [Category("Flight Details")]
        public DateTime DepartureDate { get; set; }

        /// <summary>
        /// Gets or sets the departure date range.
        /// </summary>
        [DisplayName("Offset for departure date")]
        [Description("Offset for departure date")]
        [Category("Flight Details")]
        [DefaultValue(15)]
        [TypeConverter(typeof(DateRangeDiffConverter))]
        public DateRangeDiff DepartureDateRange
        {
            get
            {
                return this._departureDateRange;
            }

            set
            {
                this._departureDateRange = value;
            }
        }

        /// <summary>
        /// Gets or sets the return date.
        /// </summary>
        [DisplayName("Return date")]
        [Description("Flight return date")]
        [Category("Flight Details")]
        public DateTime ReturnDate { get; set; }

        /// <summary>
        /// Gets or sets the return date range.
        /// </summary>
        [DisplayName("Offset for return date")]
        [Description("Offset for return date")]
        [Category("Flight Details")]
        [DefaultValue(15)]
        [TypeConverter(typeof(DateRangeDiffConverter))]
        public DateRangeDiff ReturnDateRange
        {
            get
            {
                return this._returnDateRange;
            }

            set
            {
                this._returnDateRange = value;
            }
        }

        /// <summary>
        /// Gets or sets the min stay duration.
        /// </summary>
        [DisplayName("Minimum stay duration")]
        [Description("Minimum stay duration")]
        [Category("Flight Details")]
        [DefaultValue(37)]
        public int MinStayDuration
        {
            get
            {
                return this._minStayDuration;
            }

            set
            {
                this._minStayDuration = value < 1 ? 1 : value;
            }
        }

        /// <summary>
        /// Gets or sets the max stay duration.
        /// </summary>
        [DisplayName("Maximum stay duration")]
        [Description("Maximum stay duration")]
        [Category("Flight Details")]
        [DefaultValue(43)]
        public int MaxStayDuration
        {
            get
            {
                return this._maxStayDuration;
            }

            set
            {
                this._maxStayDuration = value < 1 ? 1 : value;
            }
        }

        /// <summary>
        /// Gets or sets the price limit.
        /// </summary>
        [DisplayName("Price limit")]
        [Description("Price limit")]
        [Category("Flight Details")]
        [DefaultValue(1000)]
        public int PriceLimit
        {
            get
            {
                return this._priceLimit;
            }

            set
            {
                this._priceLimit = value < 1 ? 1 : value;
            }
        }
    }
}