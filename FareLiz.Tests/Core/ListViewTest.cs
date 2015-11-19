namespace FareLiz.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Data;
    using SkyDean.FareLiz.WinForm.Components.Controls.ListView;

    /// <summary>Summary description for CodedUITest1</summary>
    [TestClass]
    public class ListViewTest
    {
        /// <summary>
        /// The test context instance.
        /// </summary>
        private TestContext testContextInstance;

        /// <summary>Gets or sets the test context which provides information about and functionality for the current test run.</summary>
        public TestContext TestContext
        {
            get
            {
                return this.testContextInstance;
            }

            set
            {
                this.testContextInstance = value;
            }
        }

        /// <summary>
        /// The enhanced list view test.
        /// </summary>
        [TestMethod]
        public void EnhancedListViewTest()
        {
            // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
            // For more information on generated code, see http://go.microsoft.com/fwlink/?LinkId=179463
            using (var frm = new Form())
            {
                using (var lv = new EnhancedListView { Dock = DockStyle.Fill, View = View.Details })
                {
                    for (int j = 0; j < 4; j++)
                    {
                        lv.Columns.Add(j.ToString());
                    }

                    for (int i = 0; i < 1000; i++)
                    {
                        var item = new ListViewItem(i.ToString());
                        for (int j = 0; j < 4; j++)
                        {
                            item.SubItems.Add(j.ToString());
                        }

                        lv.Items.Add(item);
                    }

                    frm.Controls.Add(lv);
                    frm.ShowDialog();
                }
            }
        }

        /// <summary>
        /// The flight data list view test.
        /// </summary>
        [TestMethod]
        public void FlightDataListViewTest()
        {
            // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
            // For more information on generated code, see http://go.microsoft.com/fwlink/?LinkId=179463
            using (var frm = new Form())
            {
                using (var lv = new FlightDataListView { Dock = DockStyle.Fill })
                {
                    var route = new TravelRoute(1, AirportDataProvider.FromIATA("HEL"), AirportDataProvider.FromIATA("SGN"));
                    var j = new Journey(1, route, DateTime.Now, DateTime.Now.AddDays(7));
                    var d = new JourneyData(1, "EUR", DateTime.Now);

                    var ds = new List<Flight>(100);
                    for (int i = 0; i < 100; i++)
                    {
                        var f = new Flight(
                            d, 
                            Guid.NewGuid().ToString(), 
                            i + 10, 
                            new TravelAgency(Guid.NewGuid().ToString(), "http://google.com"), 
                            new FlightLeg(DateTime.Now, DateTime.Now.AddDays(1), TimeSpan.FromHours(i + 12), 2), 
                            new FlightLeg(DateTime.Now.AddDays(7), DateTime.Now.AddDays(8), TimeSpan.FromHours(i + 13), 2));
                        ds.Add(f);
                    }

                    j.AddData(d);
                    d.SetFlightLinks();
                    lv.SetDataSourceAsync(ds, true);
                    frm.Controls.Add(lv);
                    frm.ShowDialog();
                }
            }
        }

        #region Additional test attributes

        // You can use the following additional attributes as you write your tests:

        ////Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize()
        // {        
        // // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        // // For more information on generated code, see http://go.microsoft.com/fwlink/?LinkId=179463
        // }

        ////Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup()
        // {        
        // // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        // // For more information on generated code, see http://go.microsoft.com/fwlink/?LinkId=179463
        // }
        #endregion
    }
}