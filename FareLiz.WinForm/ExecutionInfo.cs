using SkyDean.FareLiz.Core;
using SkyDean.FareLiz.Core.Data;
using SkyDean.FareLiz.Data.Config;
using SkyDean.FareLiz.WinForm.Config;
using SkyDean.FareLiz.WinForm.Data;
using System;
using System.ComponentModel;

namespace SkyDean.FareLiz.WinForm
{
    [Serializable]
    [DefaultProperty("IsMinimized")]
    public class ExecutionInfo
    {
        [DisplayName("Start application minimized"), Description("Start application minimized"), Category("Application Settings"), DefaultValue(true)]
        public bool IsMinimized { get; set; }

        [DisplayName("Auto-synchronize new data")]
        [Description("Send database to synchronizing service after checking for new fare data. This parameter is for operation mode CloseAndExport"), Category("Application Settings"), DefaultValue(true)]
        public bool AutoSync { get; set; }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        [IniConfigurable(true, "Departure")]
        public Airport Departure { get; set; }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        [IniConfigurable(true, "Destination")]
        public Airport Destination { get; set; }

        [DisplayName("Departure date"), Description("Flight departure date"), Category("Flight Details")]
        public DateTime DepartureDate { get; set; }

        [DisplayName("Offset for departure date"), Description("Offset for departure date"), Category("Flight Details"), DefaultValue(15), TypeConverter(typeof(DateRangeDiffConverter))]
        public DateRangeDiff DepartureDateRange
        {
            get { return _departureDateRange; }
            set { _departureDateRange = value; }
        }
        DateRangeDiff _departureDateRange = new DateRangeDiff(0, 0);

        [DisplayName("Return date"), Description("Flight return date"), Category("Flight Details")]
        public DateTime ReturnDate { get; set; }

        [DisplayName("Offset for return date"), Description("Offset for return date"), Category("Flight Details"), DefaultValue(15), TypeConverter(typeof(DateRangeDiffConverter))]
        public DateRangeDiff ReturnDateRange
        {
            get { return _returnDateRange; }
            set { _returnDateRange = value; }
        }
        DateRangeDiff _returnDateRange = new DateRangeDiff(0, 0);

        [DisplayName("Minimum stay duration"), Description("Minimum stay duration"), Category("Flight Details"), DefaultValue(37)]
        public int MinStayDuration
        {
            get { return _minStayDuration; }
            set { _minStayDuration = (value < 1 ? 1 : value); }
        }
        int _minStayDuration = 1;

        [DisplayName("Maximum stay duration"), Description("Maximum stay duration"), Category("Flight Details"), DefaultValue(43)]
        public int MaxStayDuration
        {
            get { return _maxStayDuration; }
            set { _maxStayDuration = (value < 1 ? 1 : value); }
        }
        int _maxStayDuration = 1;

        [DisplayName("Price limit"), Description("Price limit"), Category("Flight Details"), DefaultValue(1000)]
        public int PriceLimit
        {
            get { return _priceLimit; }
            set { _priceLimit = (value < 1 ? 1 : value); }
        }
        int _priceLimit = 1;
    }
}