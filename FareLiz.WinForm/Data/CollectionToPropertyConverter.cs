namespace SkyDean.FareLiz.WinForm.Data
{
    using System.Collections;
    using System.ComponentModel;

    /// <summary>
    /// The collection to property converter.
    /// </summary>
    public abstract class CollectionToPropertyConverter : StringConverter
    {
        /// <summary>
        /// The get standard values supported.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            // true means show a combobox
            return true;
        }

        /// <summary>
        /// The get standard values exclusive.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            // true will limit to list. false will show the list, but allow free-form entry
            return true;
        }

        /// <summary>
        /// The get standard values.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="StandardValuesCollection"/>.
        /// </returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(this.GetAllowedValues());
        }

        /// <summary>
        /// The get allowed values.
        /// </summary>
        /// <returns>
        /// The <see cref="ICollection"/>.
        /// </returns>
        protected abstract ICollection GetAllowedValues();
    }
}