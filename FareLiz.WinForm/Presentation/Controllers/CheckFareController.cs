using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using SkyDean.FareLiz.Core;
using SkyDean.FareLiz.Core.Presentation;
using SkyDean.FareLiz.Core.Utils;
using SkyDean.FareLiz.Data.Monitoring;
using SkyDean.FareLiz.Service.Utils;
using SkyDean.FareLiz.WinForm.Components.Controls.Custom;
using SkyDean.FareLiz.WinForm.Data;
using SkyDean.FareLiz.WinForm.Presentation.Views;

namespace SkyDean.FareLiz.WinForm.Presentation.Controllers
{
    internal sealed class CheckFareController
    {
        public CheckFareForm View { get; set; }
        private readonly LiveFareFileStorage _liveFareStorage;
        private readonly AggeratingFareRequestMonitor _monitors;
        private ExecutionParam _executionParam;

        internal CheckFareEvents Events = new CheckFareEvents();

        internal CheckFareController(ExecutionParam executionParam = null)
        {
            this._executionParam = executionParam;
            var liveFareDir = Path.Combine(ProcessUtils.CurrentProcessDirectory, "LiveFare");
            _liveFareStorage = new LiveFareFileStorage(liveFareDir);
            _monitors = new AggeratingFareRequestMonitor();
        }

        /// <summary>
        /// Get list of fare scanning requests based on the condition specified on the view
        /// </summary>
        internal List<FareMonitorRequest> GetRequests()
        {
            int minDuration = View.MinDuration;
            int maxDuration = View.MaxDuration;
            var result = new List<FareMonitorRequest>();
            bool roundTrip = View.IsRoundTrip;
            var departureRange = View.DepartureDateRange;

            for (int i = -Convert.ToInt32(departureRange.Minus); i <= departureRange.Plus; i++)
            {
                int retStart = roundTrip ? -Convert.ToInt32(departureRange.Minus) : 0;
                int retEnd = roundTrip ? Convert.ToInt32(departureRange.Plus) : 0;
                for (int j = retStart; j <= retEnd; j++)
                {
                    DateTime dept = View.DepartureDate.Date.AddDays(i);
                    DateTime ret = roundTrip ? View.ReturnDate.Date.AddDays(j) : DateTime.MinValue;
                    int stayDuration = (int)(ret - dept).TotalDays;
                    if (roundTrip && (stayDuration < minDuration || stayDuration > maxDuration))
                        continue;

                    if (dept.Date >= DateTime.Now.Date && (!roundTrip || ret.Date >= dept.Date)) // Make sure that the travel date range is valid
                        result.Add(new FareMonitorRequest(View.Departure, View.Destination, dept, ret));
                }
            }

            return result;
        }

        /// <summary>
        /// Create new monitor for requests depending on the operation mode
        /// </summary>
        internal FareRequestMonitor CreateMonitor(List<FareMonitorRequest> requests, OperationMode operationMode)
        {
            if (requests == null || requests.Count < 1)
                return null;

            FareRequestMonitor newMon = null;
            FareMonitorEvents monitorEvents = null;
            if (operationMode == OperationMode.LiveMonitor)
            {
                newMon = new LiveFareMonitor(_liveFareStorage, new TaskbarFlightNotifier(), WebFareBrowserControlFactory.Instance);
                monitorEvents = Events[OperationMode.LiveMonitor];
            }
            else
            {
                if (operationMode == OperationMode.ShowFare)
                {
                    newMon = new FareRequestMonitor(WebFareBrowserControlFactory.Instance);
                    monitorEvents = Events[OperationMode.ShowFare];
                }
                else if (operationMode == OperationMode.GetFareAndSave)
                {
                    newMon = new FareExportMonitor(AppContext.MonitorEnvironment.ArchiveManager, WebFareBrowserControlFactory.Instance, View.AutoSync);
                    monitorEvents = Events[OperationMode.GetFareAndSave];
                }
                else
                    throw new ApplicationException("Unsupported opearation mode: " + operationMode);
            }

            monitorEvents.Attach(newMon);
            newMon.Enqueue(requests);

            return newMon;
        }

        internal void Start(FareRequestMonitor monitor)
        {
            _monitors.Start(monitor);
        }

        internal void Stop(FareRequestMonitor monitor)
        {
            _monitors.Clear(monitor.GetType());
        }

        /// <summary>
        /// Start new fare scan and return the monitor responsible for the new requests
        /// </summary>
        internal FareRequestMonitor Monitor(OperationMode mode)
        {
            FareRequestMonitor newMon = null;
            try
            {
                // Validate the location of the journey
                if (View.Departure.Equals(View.Destination))
                {
                    View.Show("Departure location and destination cannot be the same:" + Environment.NewLine + View.Departure, "Invalid Route", MessageBoxIcon.Exclamation);
                    return null;
                }

                // Get list of new requests for the monitor
                var newRequests = GetRequests();
                if (newRequests.Count > 0)
                {
                    newMon = CreateMonitor(newRequests, mode);
                    if (mode != OperationMode.LiveMonitor)
                    {
                        _monitors.Clear(newMon.GetType());
                    }

                    AppContext.Logger.InfoFormat("{0}: Starting monitor for {1} new requests", GetType().Name, newRequests.Count);
                    _monitors.Start(newMon);
                }
                else
                {
                    string period = (View.MinDuration == View.MaxDuration
                        ? View.MinDuration.ToString(CultureInfo.InvariantCulture)
                        : View.MinDuration.ToString(CultureInfo.InvariantCulture) + " and " + View.MaxDuration.ToString(CultureInfo.InvariantCulture));

                    string message = String.Format(@"There is no travel date which satisfies the filter condition for the stay duration between {0} days (You selected travel period {1})!

Double-check the filter conditions and make sure that not all travel dates are in the past", period, StringUtil.GetPeriodString(View.DepartureDate, View.ReturnDate));
                    AppContext.Logger.Debug(message);
                    AppContext.ProgressCallback.Inform(null, message, "Invalid parameters", NotificationType.Exclamation);
                }
            }
            catch (Exception ex)
            {
                AppContext.ProgressCallback.Inform(View, "Failed to start monitors: " + ex.Message, "Check Fares", NotificationType.Error);
                AppContext.Logger.Error("Failed to start monitors: " + ex.ToString());
            }
            finally
            {
                View.SetScanner(true);
            }

            return newMon;
        }

        internal void ShowLiveFare()
        {
            new LiveFareDataForm(_liveFareStorage).ShowDialog(View);
        }

        internal void ClearMonitors()
        {
            _monitors.Clear();
        }

        internal void ClearMonitors(Type type)
        {
            _monitors.Clear(type);
        }
    }
}
