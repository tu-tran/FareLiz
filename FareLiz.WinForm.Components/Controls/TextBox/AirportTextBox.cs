using SkyDean.FareLiz.Data;

namespace SkyDean.FareLiz.WinForm.Components.Controls.TextBox
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    using SkyDean.FareLiz.Core.Data;

    public class AirportTextBox : AutoCompleteTextbox<Airport>
    {
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public override IList<Airport> AutoCompleteList
        {
            get { return base.AutoCompleteList; }
            set { base.AutoCompleteList = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public Airport SelectedAirport
        {
            get { return this.listBox.SelectedItem as Airport; }
            set
            {
                if (value != null)
                    this.Text = value.ToString();
                else
                    this.listBox.SelectedItem = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public string SelectedAirportCode
        {
            get { return this.SelectedAirport == null ? null : this.SelectedAirport.IATA; }
            set
            {
                if (value == null)
                    this.listBox.SelectedItem = null;
                else
                    foreach (var a in this.AutoCompleteList)
                    {
                        if (String.Equals(a.IATA, value, StringComparison.OrdinalIgnoreCase))
                        {
                            this.Text = a.ToString();
                            return;
                        }
                    }
            }
        }

        public AirportTextBox()
        {
            this.listBox.ItemHeight = this.listBox.Font.Height * 2 + 10; // 2 rows and padding              
            this.listBox.ValueMember = "IATA";
            this.listBox.DisplayMember = "";
            this.InitializeData();
        }

        internal void InitializeData()
        {
            this.InitializeData(AirportDataProvider.Airports);
        }

        internal void InitializeData(IList<Airport> data)
        {
            this.AutoCompleteList = data;
        }
    }
}
