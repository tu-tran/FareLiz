namespace SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.Design
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;

    /// <summary>Custom <see cref="CultureInfo" /> converter that only gives non-neutral cultures back.</summary>
    internal class CultureInfoCustomTypeConverter : CultureInfoConverter
    {
        /// <summary>The non-neutral cultures.</summary>
        private StandardValuesCollection values;

        /// <summary>
        /// Gets a collection of standard values for a <see cref="CultureInfo"/> object using the specified context.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext"/> that provides a format context.
        /// </param>
        /// <returns>
        /// A <see cref="TypeConverter.StandardValuesCollection"/> containing a standard set of valid values, or null if the data type does not support
        /// a standard set of values.
        /// </returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            base.GetStandardValues(context);

            if (this.values == null)
            {
                var list = new List<CultureInfo>(CultureInfo.GetCultures(CultureTypes.AllCultures));

                list.RemoveAll(c => c.IsNeutralCulture);

                list.Sort(
                    (c1, c2) =>
                        {
                            if (c1 == null)
                            {
                                return c2 == null ? 0 : -1;
                            }

                            if (c2 == null)
                            {
                                return 1;
                            }

                            return CultureInfo.CurrentCulture.CompareInfo.Compare(c1.DisplayName, c2.DisplayName, CompareOptions.StringSort);
                        });

                this.values = new StandardValuesCollection(list);
            }

            return this.values;
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
            var retValue = base.ConvertTo(context, culture, value, destinationType);

            // if (destinationType == typeof(string) && value is CultureInfo)
            // {
            // var ci = (CultureInfo)value;

            // var name = ci.DisplayName;

            // if (string.IsNullOrEmpty(name))
            // {
            // name = ci.Name;
            // }

            // return name;
            // }
            return retValue;
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
            return base.ConvertFrom(context, culture, value);
        }
    }
}