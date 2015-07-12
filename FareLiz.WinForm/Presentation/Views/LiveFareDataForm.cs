using System;
using System.IO;
using System.Windows.Forms;
using SkyDean.FareLiz.Core.Data;
using SkyDean.FareLiz.Core.Utils;
using SkyDean.FareLiz.Data.Monitoring;
using SkyDean.FareLiz.WinForm.Components.Controls.ListView;
using SkyDean.FareLiz.WinForm.Components.Dialog;

namespace SkyDean.FareLiz.WinForm.Presentation.Views
{
    internal partial class LiveFareDataForm : SmartForm
    {
        private readonly LiveFareFileStorage _fileStorage;

        public LiveFareDataForm(LiveFareFileStorage fileStorage)
        {
            _fileStorage = fileStorage;
            InitializeComponent();

            Text = AppUtil.ProductName + " " + Text;
            lvFareData.AttachMenuStrip(lvFareDataContextMenuStrip);
        }

        private void BindRoutes()
        {
            var routes = _fileStorage.GetRoutesInfo();
            lbRoute.DataSource = routes;
            if (routes.Count > 0 && lbRoute.SelectedIndex < 0)
                lbRoute.SelectedIndex = 0;
        }

        private void BindTravelDates()
        {
            var selRoute = lbRoute.SelectedItem as RouteInfo;
            if (selRoute != null)
            {
                var travelDates = _fileStorage.GetTravelDates(selRoute);
                lbFlightDate.DataSource = travelDates;
                if (travelDates.Count > 0 && lbFlightDate.SelectedIndex < 0)
                    lbFlightDate.SelectedIndex = 0;
            }
        }

        private void BindDataDate()
        {
            var selRoute = lbRoute.SelectedItem as RouteInfo;
            var selTravelDate = lbFlightDate.SelectedItem as DatePeriod;
            if (selRoute != null && selTravelDate != null)
            {
                var dataDates = _fileStorage.GetDataDates(selRoute, selTravelDate);
                cbDataPeriod.DataSource = dataDates;
                if (dataDates.Count > 0 && cbDataPeriod.SelectedIndex < 0)
                    cbDataPeriod.SelectedIndex = 0;
            }
        }

        private void BindFareData()
        {
            var routeInfo = lbRoute.SelectedItem as RouteInfo;
            var travelPeriod = lbFlightDate.SelectedItem as DatePeriod;
            var dataPeriod = cbDataPeriod.SelectedItem as DatePeriod;

            if (routeInfo != null && travelPeriod != null && dataPeriod != null)
            {
                var dataFile = _fileStorage.GetDataFile(routeInfo, travelPeriod, dataPeriod);
                if (String.IsNullOrEmpty(dataFile) || !File.Exists(dataFile))
                    return;
                lvFareData.BindData(dataFile, CsvDataType.CsvFile);
            }
        }

        private void lbFlightDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindDataDate();
        }

        private void cbDataPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindFareData();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            cbDataPeriod_SelectedIndexChanged(null, null);
        }


        private static void MovePrevious(int count, params ComboBox[] itemList)
        {
            foreach (ComboBox item in itemList)
                if (item.SelectedIndex > count - 1)
                    item.SelectedIndex = item.SelectedIndex - count;
        }

        private static void MoveNext(int count, params ComboBox[] itemList)
        {
            foreach (ComboBox item in itemList)
                if (item.SelectedIndex < item.Items.Count - count)
                    item.SelectedIndex = item.SelectedIndex + count;
        }

        private void btnNextDataDate_Click(object sender, EventArgs e)
        {
            MoveNext(1, cbDataPeriod);
        }

        private void btnPrevDataDate_Click(object sender, EventArgs e)
        {
            MovePrevious(1, cbDataPeriod);
        }

        private void btnRefreshDataDate_Click(object sender, EventArgs e)
        {
            BindDataDate();
        }

        private void LiveFareDataForm_Shown(object sender, EventArgs e)
        {
            BindRoutes();
        }

        private void lbRoute_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindTravelDates();
        }

        private void LiveFareDataForm_SizeChanged(object sender, EventArgs e)
        {
            cbDataPeriod.Invalidate();
            btnNextDataDate.Invalidate();
            btnPrevDataDate.Invalidate();
        }
    }
}