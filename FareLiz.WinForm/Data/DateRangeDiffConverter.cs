using SkyDean.FareLiz.Core;
using System;
using System.ComponentModel;
using System.Globalization;

namespace SkyDean.FareLiz.WinForm.Data
{
    public class DateRangeDiffConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(DateRangeDiff))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var castedVal = value as DateRangeDiff;
            if (castedVal != null && destinationType == typeof(String))
                return "+" + castedVal.Plus + "/-" + castedVal.Minus + " days";
            return null;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(String) || sourceType == typeof(DateRangeDiff))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            try
            {
                var s = value as string;
                if (s != null)
                {
                    int sep = s.IndexOf('/');
                    return new DateRangeDiff(Int32.Parse(s.Substring(1, sep), CultureInfo.InvariantCulture),
                                             Int32.Parse(s.Substring(sep + 1, s.Length - sep - 1), CultureInfo.InvariantCulture));
                }
            }
            catch { }
            return DateRangeDiff.Empty;
        }
    }
}
