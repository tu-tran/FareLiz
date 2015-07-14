namespace SkyDean.FareLiz.WinForm.Components.Controls.TextBox
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Data;

    /// <summary>The airport text box.</summary>
    public class AirportTextBox : AutoCompleteTextbox<Airport>
    {
        /// <summary>Initializes a new instance of the <see cref="AirportTextBox" /> class.</summary>
        public AirportTextBox()
        {
            this.listBox.ItemHeight = this.listBox.Font.Height * 2 + 10; // 2 rows and padding              
            this.listBox.ValueMember = "IATA";
            this.listBox.DisplayMember = string.Empty;
            this.InitializeData();
        }

        /// <summary>Gets or sets the auto complete list.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override IList<Airport> AutoCompleteList
        {
            get
            {
                return base.AutoCompleteList;
            }

            set
            {
                base.AutoCompleteList = value;
            }
        }

        /// <summary>Gets or sets the selected airport.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Airport SelectedAirport
        {
            get
            {
                return this.listBox.SelectedItem as Airport;
            }

            set
            {
                if (value != null)
                {
                    this.Text = value.ToString();
                }
                else
                {
                    this.listBox.SelectedItem = value;
                }
            }
        }

        /// <summary>Gets or sets the selected airport code.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string SelectedAirportCode
        {
            get
            {
                return this.SelectedAirport == null ? null : this.SelectedAirport.IATA;
            }

            set
            {
                if (value == null)
                {
                    this.listBox.SelectedItem = null;
                }
                else
                {
                    foreach (var a in this.AutoCompleteList)
                    {
                        if (string.Equals(a.IATA, value, StringComparison.OrdinalIgnoreCase))
                        {
                            this.Text = a.ToString();
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>The initialize data.</summary>
        internal void InitializeData()
        {
            this.InitializeData(AirportDataProvider.Airports);
        }

        /// <summary>
        /// The initialize data.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        internal void InitializeData(IList<Airport> data)
        {
            this.AutoCompleteList = data;
        }
    }
}