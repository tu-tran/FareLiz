namespace SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.Design
{
    using System.Collections;
    using System.ComponentModel.Design;
    using System.Windows.Forms.Design;

    using SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.DatePicker;

    /// <summary>The <see cref="EnhancedDatePicker" /> designer.</summary>
    internal class DatePickerControlDesigner : ControlDesigner
    {
        /// <summary>Gets the <see cref="SelectionRules" /> for the designer.</summary>
        public override SelectionRules SelectionRules
        {
            get
            {
                return SelectionRules.RightSizeable | SelectionRules.LeftSizeable | SelectionRules.Moveable | SelectionRules.Visible;
            }
        }

        /// <summary>
        /// Gets the action lists.
        /// </summary>
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                return new DesignerActionListCollection { new MonthCalendarControlDesigner.MonthCalendarControlDesignerActionList(this.Component) };
            }
        }

        /// <summary>
        /// Prefilters the properties.
        /// </summary>
        /// <param name="properties">
        /// The property dictionary.
        /// </param>
        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);

            properties.Remove("BackgroundImage");
            properties.Remove("Text");
            properties.Remove("ImeMode");
            properties.Remove("Padding");
            properties.Remove("BackgroundImageLayout");
        }
    }
}