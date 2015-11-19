namespace SkyDean.FareLiz.WinForm.Presentation.Controllers
{
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

    /// <summary>
    /// The check fare controller.
    /// </summary>
    internal sealed class CheckFareController
    {
        /// <summary>
        /// The _live fare storage.
        /// </summary>
        private readonly LiveFareFileStorage _liveFareStorage;

        /// <summary>
        /// The _monitors.
        /// </summary>
        private readonly AggeratingFareRequestMonitor _monitors;

        /// <summary>
        /// The _execution param.
        /// </summary>
        private ExecutionParam _executionParam;

        /// <summary>
        /// The events.
        /// </summary>
        internal CheckFareEvents Events = new CheckFareEvents();

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckFareController"/> class.
        /// </summary>
        /// <param name="executionParam">
        /// The execution param.
        /// </param>
        internal CheckFareController(ExecutionParam executionParam = null)
        {
            this._executionParam = executionParam;
            var liveFareDir = Path.Combine(ProcessUtils.CurrentProcessDirectory, "LiveFare");
            this._liveFareStorage = new LiveFareFileStorage(liveFareDir);
            this._monitors = new AggeratingFareRequestMonitor();
        }

        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        public CheckFareForm View { get; set; }

        /// <summary>
        /// Get list of fare scanning requests based on the condition specified on the view
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        internal List<FareMonitorRequest> GetRequests()
        {
            int minDuration = this.View.MinDuration;
            int maxDuration = this.View.MaxDuration;
            var result = new List<FareMonitorRequest>();
            bool roundTrip = this.View.IsRoundTrip;
            var departureRange = this.View.DepartureDateRange;

            for (int i = -Convert.ToInt32(departureRange.Minus); i <= departureRange.Plus; i++)
            {
                int retStart = roundTrip ? -Convert.ToInt32(departureRange.Minus) : 0;
                int retEnd = roundTrip ? Convert.ToInt32(departureRange.Plus) : 0;
                for (int j = retStart; j <= retEnd; j++)
                {
                    DateTime dept = this.View.DepartureDate.Date.AddDays(i);
                    DateTime ret = roundTrip ? this.View.ReturnDate.Date.AddDays(j) : DateTime.MinValue;
                    int stayDuration = (int)(ret - dept).TotalDays;
                    if (roundTrip && (stayDuration < minDuration || stayDuration > maxDuration))
                    {
                        continue;
                    }

                    if (dept.Date >= DateTime.Now.Date && (!roundTrip || ret.Date >= dept.Date))
                    {
                        // Make sure that the travel date range is valid
                        result.Add(new FareMonitorRequest(this.View.Departure, this.View.Destination, dept, ret));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Create new monitor for requests depending on the operation mode
        /// </summary>
        /// <param name="requests">
        /// The requests.
        /// </param>
        /// <param name="operationMode">
        /// The operation Mode.
        /// </param>
        /// <returns>
        /// The <see cref="FareRequestMonitor"/>.
        /// </returns>
        internal FareRequestMonitor CreateMonitor(List<FareMonitorRequest> requests, OperationMode operationMode)
        {
            if (requests == null || requests.Count < 1)
            {
                return null;
            }

            FareRequestMonitor newMon = null;
            FareMonitorEvents monitorEvents = null;
            if (operationMode == OperationMode.LiveMonitor)
            {
                newMon = new LiveFareMonitor(this._liveFareStorage, new TaskbarFlightNotifier(), WebFareBrowserControlFactory.Instance);
                monitorEvents = this.Events[OperationMode.LiveMonitor];
            }
            else
            {
                if (operationMode == OperationMode.ShowFare)
                {
                    newMon = new FareRequestMonitor(WebFareBrowserControlFactory.Instance);
                    monitorEvents = this.Events[OperationMode.ShowFare];
                }
                else if (operationMode == OperationMode.GetFareAndSave)
                {
                    newMon = new FareExportMonitor(
                        AppContext.MonitorEnvironment.ArchiveManager, 
                        WebFareBrowserControlFactory.Instance, 
                        this.View.AutoSync);
                    monitorEvents = this.Events[OperationMode.GetFareAndSave];
                }
                else
                {
                    throw new ApplicationException("Unsupported opearation mode: " + operationMode);
                }
            }

            monitorEvents.Attach(newMon);
            newMon.Enqueue(requests);

            return newMon;
        }

        /// <summary>
        /// The start.
        /// </summary>
        /// <param name="monitor">
        /// The monitor.
        /// </param>
        internal void Start(FareRequestMonitor monitor)
        {
            this._monitors.Start(monitor);
        }

        /// <summary>
        /// The stop.
        /// </summary>
        /// <param name="monitor">
        /// The monitor.
        /// </param>
        internal void Stop(FareRequestMonitor monitor)
        {
            this._monitors.Clear(monitor.GetType());
        }

        /// <summary>
        /// Start new fare scan and return the monitor responsible for the new requests
        /// </summary>
        /// <param name="mode">
        /// The mode.
        /// </param>
        /// <returns>
        /// The <see cref="FareRequestMonitor"/>.
        /// </returns>
        internal FareRequestMonitor Monitor(OperationMode mode)
        {
            FareRequestMonitor newMon = null;
            try
            {
                // Validate the location of the journey
                if (this.View.Departure.Equals(this.View.Destination))
                {
                    this.View.Show(
                        "Departure location and destination cannot be the same:" + Environment.NewLine + this.View.Departure, 
                        "Invalid Route", 
                        MessageBoxIcon.Exclamation);
                    return null;
                }

                // Get list of new requests for the monitor
                var newRequests = this.GetRequests();
                if (newRequests.Count > 0)
                {
                    newMon = this.CreateMonitor(newRequests, mode);
                    if (mode != OperationMode.LiveMonitor)
                    {
                        this._monitors.Clear(newMon.GetType());
                    }

                    AppContext.Logger.InfoFormat("{0}: Starting monitor for {1} new requests", this.GetType().Name, newRequests.Count);
                    this._monitors.Start(newMon);
                }
                else
                {
                    string period = this.View.MinDuration == this.View.MaxDuration
                                         ? this.View.MinDuration.ToString(CultureInfo.InvariantCulture)
                                         : this.View.MinDuration.ToString(CultureInfo.InvariantCulture) + " and "
                                           + this.View.MaxDuration.ToString(CultureInfo.InvariantCulture);

                    string message =
                        string.Format(
                            @"There is no travel date which satisfies the filter condition for the stay duration between {0} days (You selected travel period {1})!

Double-check the filter conditions and make sure that not all travel dates are in the past", 
                            period, 
                            StringUtil.GetPeriodString(this.View.DepartureDate, this.View.ReturnDate));
                    AppContext.Logger.Debug(message);
                    AppContext.ProgressCallback.Inform(null, message, "Invalid parameters", NotificationType.Exclamation);
                }
            }
            catch (Exception ex)
            {
                AppContext.ProgressCallback.Inform(this.View, "Failed to start monitors: " + ex.Message, "Check Fares", NotificationType.Error);
                AppContext.Logger.Error("Failed to start monitors: " + ex);
            }
            finally
            {
                this.View.SetScanner(true);
            }

            return newMon;
        }

        /// <summary>
        /// The show live fare.
        /// </summary>
        internal void ShowLiveFare()
        {
            new LiveFareDataForm(this._liveFareStorage).ShowDialog(this.View);
        }

        /// <summary>
        /// The clear monitors.
        /// </summary>
        internal void ClearMonitors()
        {
            this._monitors.Clear();
        }

        /// <summary>
        /// The clear monitors.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        internal void ClearMonitors(Type type)
        {
            this._monitors.Clear(type);
        }
    }
}