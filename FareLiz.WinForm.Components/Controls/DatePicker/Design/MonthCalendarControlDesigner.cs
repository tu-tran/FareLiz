namespace SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.Design
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Windows.Forms;
    using System.Windows.Forms.Design;

    using SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.DatePicker;

    /// <summary>
   /// The <see cref="MonthCalendar"/> designer.
   /// </summary>
   internal class MonthCalendarControlDesigner : ControlDesigner
   {
      #region Properties

      /// <summary>
      /// Gets the <see cref="SelectionRules"/> for the control.
      /// </summary>
      public override SelectionRules SelectionRules
      {
         get
         {
            return SelectionRules.BottomSizeable
               | SelectionRules.RightSizeable
               | SelectionRules.Moveable
               | SelectionRules.Visible;
         }
      }

      /// <summary>
      /// Gets the design time action list collection which is supported by the designer.
      /// </summary>
      public override DesignerActionListCollection ActionLists
      {
         get
         {
            return new DesignerActionListCollection { new MonthCalendarControlDesignerActionList(this.Component) };
         }
      }

      #endregion

      #region methods

      /// <summary>
      /// Prefilters the properties.
      /// </summary>
      /// <param name="properties">The property dictionary.</param>
      protected override void PreFilterProperties(IDictionary properties)
      {
         base.PreFilterProperties(properties);

         properties.Remove("BackgroundImage");
         properties.Remove("ForeColor");
         properties.Remove("Text");
         properties.Remove("ImeMode");
         properties.Remove("Padding");
         properties.Remove("BackgroundImageLayout");
         properties.Remove("BackColor");
      }

      #endregion

      /// <summary>
      /// Provides the designer action list for the <see cref="CustomControls.MonthCalendar"/> or <see cref="EnhancedDatePicker"/> controls.
      /// </summary>
      internal class MonthCalendarControlDesignerActionList : DesignerActionList
      {
         #region Fields

         private const string FileFilter = "XML File (*.xml)|*.xml";

         /// <summary>
         /// The <see cref="CustomControls.MonthCalendar"/> control.
         /// </summary>
         private readonly EnhancedMonthCalendar cal;

         /// <summary>
         /// The <see cref="IComponentChangeService"/>.
         /// </summary>
         private readonly IComponentChangeService iccs;

         /// <summary>
         /// The <see cref="DesignerActionUIService"/>.
         /// </summary>
         private readonly DesignerActionUIService designerUISvc;

         #endregion

         #region constructor

         /// <summary>
         /// Initializes a new instance of the <see cref="MonthCalendarControlDesignerActionList"/> class.
         /// </summary>
         /// <param name="component">The component.</param>
         public MonthCalendarControlDesignerActionList(IComponent component)
            : base(component)
         {
            Type compType = component.GetType();

            if (component == null || (compType != typeof(EnhancedMonthCalendar) && compType != typeof(EnhancedDatePicker)))
            {
               throw new InvalidOperationException("MonthCalendarDesigner : component is null or not of the correct type.");
            }

            if (compType == typeof(EnhancedDatePicker))
            {
               this.cal = ((EnhancedDatePicker)component).PickerCalendar;
            }
            else
            {
                this.cal = (EnhancedMonthCalendar)component;
            }

            this.iccs = (IComponentChangeService)this.GetService(typeof(IComponentChangeService));

            this.designerUISvc = (DesignerActionUIService)this.GetService(typeof(DesignerActionUIService));
         }

         #endregion

         #region Properties

         /// <summary>
         /// Gets or sets a value indicating whether to show the smart tag area while creating.
         /// </summary>
         public override bool AutoShow
         {
            get { return true; }
            set { base.AutoShow = true; }
         }

         #endregion

         /// <summary>
         /// Returns the collection of <see cref="DesignerActionItem"/>.
         /// </summary>
         /// <returns>A collection of <see cref="DesignerActionItem"/>.</returns>
         public override DesignerActionItemCollection GetSortedActionItems()
         {
            DesignerActionItemCollection actionItems = new DesignerActionItemCollection
                                 {
                                    new DesignerActionMethodItem(this, "LoadColorTable", "Load the color table",
                                                                 "", "Loads the color table from an XML-file",  true),
                                    new DesignerActionMethodItem(this, "SaveColorTable", "Save the color table", "",
                                                                 "Saves the color table to an XML-file", true)
                                 };

            return actionItems;
         }
      }
   }
}