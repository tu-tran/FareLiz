namespace SkyDean.FareLiz.WinForm.Presentation.Views
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.Data.Monitoring;
    using SkyDean.FareLiz.WinForm.Components.Controls.ListView;
    using SkyDean.FareLiz.WinForm.Components.Dialog;

    /// <summary>The live fare data form.</summary>
    internal partial class LiveFareDataForm : SmartForm
    {
        /// <summary>The _file storage.</summary>
        private readonly LiveFareFileStorage _fileStorage;

        /// <summary>
        /// Initializes a new instance of the <see cref="LiveFareDataForm"/> class.
        /// </summary>
        /// <param name="fileStorage">
        /// The file storage.
        /// </param>
        public LiveFareDataForm(LiveFareFileStorage fileStorage)
        {
            this._fileStorage = fileStorage;
            this.InitializeComponent();

            this.Text = AppUtil.ProductName + " " + this.Text;
            this.lvFareData.AttachMenuStrip(this.lvFareDataContextMenuStrip);
        }

        /// <summary>The bind routes.</summary>
        private void BindRoutes()
        {
            var routes = this._fileStorage.GetRoutesInfo();
            this.lbRoute.DataSource = routes;
            if (routes.Count > 0 && this.lbRoute.SelectedIndex < 0)
            {
                this.lbRoute.SelectedIndex = 0;
            }
        }

        /// <summary>The bind travel dates.</summary>
        private void BindTravelDates()
        {
            var selRoute = this.lbRoute.SelectedItem as RouteInfo;
            if (selRoute != null)
            {
                var travelDates = this._fileStorage.GetTravelDates(selRoute);
                this.lbFlightDate.DataSource = travelDates;
                if (travelDates.Count > 0 && this.lbFlightDate.SelectedIndex < 0)
                {
                    this.lbFlightDate.SelectedIndex = 0;
                }
            }
        }

        /// <summary>The bind data date.</summary>
        private void BindDataDate()
        {
            var selRoute = this.lbRoute.SelectedItem as RouteInfo;
            var selTravelDate = this.lbFlightDate.SelectedItem as DatePeriod;
            if (selRoute != null && selTravelDate != null)
            {
                var dataDates = this._fileStorage.GetDataDates(selRoute, selTravelDate);
                this.cbDataPeriod.DataSource = dataDates;
                if (dataDates.Count > 0 && this.cbDataPeriod.SelectedIndex < 0)
                {
                    this.cbDataPeriod.SelectedIndex = 0;
                }
            }
        }

        /// <summary>The bind fare data.</summary>
        private void BindFareData()
        {
            var routeInfo = this.lbRoute.SelectedItem as RouteInfo;
            var travelPeriod = this.lbFlightDate.SelectedItem as DatePeriod;
            var dataPeriod = this.cbDataPeriod.SelectedItem as DatePeriod;

            if (routeInfo != null && travelPeriod != null && dataPeriod != null)
            {
                var dataFile = this._fileStorage.GetDataFile(routeInfo, travelPeriod, dataPeriod);
                if (string.IsNullOrEmpty(dataFile) || !File.Exists(dataFile))
                {
                    return;
                }

                this.lvFareData.BindData(dataFile, CsvDataType.CsvFile);
            }
        }

        /// <summary>
        /// The lb flight date_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void lbFlightDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.BindDataDate();
        }

        /// <summary>
        /// The cb data period_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void cbDataPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.BindFareData();
        }

        /// <summary>
        /// The btn refresh_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            this.cbDataPeriod_SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// The move previous.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="itemList">
        /// The item list.
        /// </param>
        private static void MovePrevious(int count, params ComboBox[] itemList)
        {
            foreach (var item in itemList)
            {
                if (item.SelectedIndex > count - 1)
                {
                    item.SelectedIndex = item.SelectedIndex - count;
                }
            }
        }

        /// <summary>
        /// The move next.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="itemList">
        /// The item list.
        /// </param>
        private static void MoveNext(int count, params ComboBox[] itemList)
        {
            foreach (var item in itemList)
            {
                if (item.SelectedIndex < item.Items.Count - count)
                {
                    item.SelectedIndex = item.SelectedIndex + count;
                }
            }
        }

        /// <summary>
        /// The btn next data date_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnNextDataDate_Click(object sender, EventArgs e)
        {
            MoveNext(1, this.cbDataPeriod);
        }

        /// <summary>
        /// The btn prev data date_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnPrevDataDate_Click(object sender, EventArgs e)
        {
            MovePrevious(1, this.cbDataPeriod);
        }

        /// <summary>
        /// The btn refresh data date_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnRefreshDataDate_Click(object sender, EventArgs e)
        {
            this.BindDataDate();
        }

        /// <summary>
        /// The live fare data form_ shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void LiveFareDataForm_Shown(object sender, EventArgs e)
        {
            this.BindRoutes();
        }

        /// <summary>
        /// The lb route_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void lbRoute_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.BindTravelDates();
        }

        /// <summary>
        /// The live fare data form_ size changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void LiveFareDataForm_SizeChanged(object sender, EventArgs e)
        {
            this.cbDataPeriod.Invalidate();
            this.btnNextDataDate.Invalidate();
            this.btnPrevDataDate.Invalidate();
        }
    }
}