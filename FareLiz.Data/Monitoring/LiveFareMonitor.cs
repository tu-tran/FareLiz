namespace SkyDean.FareLiz.Data.Monitoring
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Presentation;
    using SkyDean.FareLiz.Core.Utils;

    /// <summary>
    /// Monitor used for live fare
    /// </summary>
    public sealed class LiveFareMonitor : FareRequestMonitor
    {
        private readonly IFareStorage _fareStorage;
        private readonly IFlightNotifier _notifier;
        private readonly List<Journey> _currentMonitorJourneys = new List<Journey>();

        public double PriceLimit { get; set; }
        public override OperationMode Mode { get { return OperationMode.LiveMonitor; } }

        public LiveFareMonitor(IFareStorage fareStorage, IFlightNotifier notifier, IFareBrowserControlFactory controlFactory)
            : base(controlFactory)
        {
            this._fareStorage = fareStorage;
            this._notifier = notifier;
            this.RequestCompleted += this.LiveFareMonitor_OnRequestCompleted;
        }

        void LiveFareMonitor_OnRequestCompleted(FareRequestMonitor sender, FareBrowserRequestArg args)
        {
            var request = args.Request;
            var browser = args.Request.BrowserControl;
            if (browser == null)
                return;

            lock (_notifier)
            {
                try
                {
                    string travelPeriodStr = StringUtil.GetPeriodString(request.DepartureDate, request.ReturnDate);
                    if (browser.RequestState == DataRequestState.NoData)
                    {
                        string header = travelPeriodStr + Environment.NewLine +
                            "From: " + request.Departure + Environment.NewLine +
                            "To: " + request.Destination;
                        _notifier.Show("No AirportData", "There is no flight data for this travel period" + Environment.NewLine + header, null, 7000, NotificationType.Warning, true);
                        return;
                    }

                    var route = browser.LastRetrievedRoute;
                    if (route == null || route.Journeys.Count < 1 || route.Journeys[0].Data.Count < 1)
                        return;

                    var curJourney = route.Journeys[0];
                    // Live Monitor: Store the current journey data
                    Journey oldJourney = this._currentMonitorJourneys.FirstOrDefault(j => j.IsSameTrip(curJourney));
                    var flightData = curJourney.Data[0].Flights;
                    var flightItems = new FlightDisplayItemsCollection();

                    if (oldJourney == null)
                    {
                        flightItems.AddRange(flightData, FlightStatus.New);
                    }
                    else
                    {
                        var oldFlights = oldJourney.Data[0].Flights;
                        // Compare each flight
                        foreach (Flight currentFlight in flightData)
                        {
                            Flight comparableFlight = oldFlights.FirstOrDefault(f => f.IsSameFlight(currentFlight));
                            if (comparableFlight == null) // New flight was found (or first appearance)
                            {
                                flightItems.Add(currentFlight, FlightStatus.New, 0);
                            }
                            else
                            {
                                double priceDiff = currentFlight.Price - comparableFlight.Price;
                                if (Math.Abs(priceDiff) > 1) // Minimum price change is 1 EUR
                                {
                                    if ((priceDiff > 0 && comparableFlight.Price < this.PriceLimit)  // If price was increased and old price is still within price limit
                                        || currentFlight.Price <= this.PriceLimit)                   // or prices has been decreased enough to the limit
                                    {
                                        flightItems.Add(currentFlight, priceDiff > 0 ? FlightStatus.PriceIncreased : FlightStatus.PriceDecreased, comparableFlight.Price);
                                    }
                                }
                            }
                        }
                    }

                    // There are changes in the list of flights
                    if (flightItems.Count > 0)
                    {
                        if (this._fareStorage != null)
                        {
                            try
                            {
                                this._fareStorage.SaveLiveFare(flightItems.Select(i => i.FlightData));
                            }
                            catch (Exception ex)
                            {
                                AppContext.Logger.ErrorFormat("Could not save live data [{0}]: {1}", travelPeriodStr, ex);
                                _notifier.Show(travelPeriodStr, "Could not save live data: " + ex.Message, null, 5000, NotificationType.Error, true);
                            }
                        }

                        string currencyCode = curJourney.Data[0].Currency;
                        string currencySymbol = AppContext.MonitorEnvironment.CurrencyProvider.GetCurrencyInfo(currencyCode).Symbol;
                        string header = travelPeriodStr + " (Currency: " + currencyCode + (currencySymbol == currencyCode ? null : " - " + currencySymbol) + ")" + Environment.NewLine +
                            "From: " + request.Departure + Environment.NewLine +
                            "To: " + request.Destination;
                        _notifier.Show("Fare data was updated", header, flightItems, 5000, NotificationType.Info, true);
                    }

                    if (oldJourney != null)
                        this._currentMonitorJourneys.Remove(oldJourney);
                    this._currentMonitorJourneys.Add(curJourney);
                }
                finally
                {
                    // Repeat the process
                    if (this.State == MonitorState.Running && browser.RequestState != DataRequestState.NoData)
                    {
                        args.Request.Reset();
#if !DEBUG  // Put some delay if we are not running DEBUG mode
                    var interval = TimeSpan.FromMinutes(1 + PendingRequests.Count);
                    System.Threading.Thread.Sleep(interval);
#endif
                        this.Enqueue(args.Request);
                    }
                }
            }
        }
    }
}
