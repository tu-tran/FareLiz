using System.Collections;
using System.ComponentModel;

namespace SkyDean.FareLiz.WinForm.Data
{
    public abstract class CollectionToPropertyConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            //true means show a combobox
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            //true will limit to list. false will show the list, but allow free-form entry
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(GetAllowedValues());
        }

        protected abstract ICollection GetAllowedValues();
    }
}
