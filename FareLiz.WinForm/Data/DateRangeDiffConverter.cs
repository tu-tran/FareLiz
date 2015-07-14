namespace SkyDean.FareLiz.WinForm.Data
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    using SkyDean.FareLiz.Core;

    /// <summary>The date range diff converter.</summary>
    public class DateRangeDiffConverter : ExpandableObjectConverter
    {
        /// <summary>
        /// The can convert to.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="destinationType">
        /// The destination type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(DateRangeDiff))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// The convert to.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="destinationType">
        /// The destination type.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var castedVal = value as DateRangeDiff;
            if (castedVal != null && destinationType == typeof(String))
            {
                return "+" + castedVal.Plus + "/-" + castedVal.Minus + " days";
            }

            return null;
        }

        /// <summary>
        /// The can convert from.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="sourceType">
        /// The source type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(String) || sourceType == typeof(DateRangeDiff))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// The convert from.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            try
            {
                var s = value as string;
                if (s != null)
                {
                    var sep = s.IndexOf('/');
                    return new DateRangeDiff(
                        int.Parse(s.Substring(1, sep), CultureInfo.InvariantCulture), 
                        int.Parse(s.Substring(sep + 1, s.Length - sep - 1), CultureInfo.InvariantCulture));
                }
            }
            catch
            {
            }

            return DateRangeDiff.Empty;
        }
    }
}