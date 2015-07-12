using System;
using System.Collections.Generic;
using System.Diagnostics;
using SkyDean.FareLiz.Data.Monitoring;

namespace SkyDean.FareLiz.WinForm.Presentation.Controllers
{
    internal class CheckFareEvents
    {
        internal readonly Dictionary<OperationMode, FareMonitorEvents> AllEvents = new Dictionary
            <OperationMode, FareMonitorEvents>
        {
            {OperationMode.ShowFare, new FareMonitorEvents()},
            {OperationMode.GetFareAndSave, new FareMonitorEvents()},
            {OperationMode.LiveMonitor, new FareMonitorEvents()}
        };

        internal FareMonitorEvents this[OperationMode mode]
        {
            get { return AllEvents[mode]; }
        }

        internal event FareMonitorHandler MonitorStarting
        {
            add
            {
                foreach (var monitorEvents in AllEvents.Values)
                {
                    monitorEvents.MonitorStarting += value;
                }
            }

            remove
            {
                foreach (var monitorEvents in AllEvents.Values)
                {
                    monitorEvents.MonitorStarting -= value;
                }
            }
        }

        internal event FareMonitorHandler MonitorStopping
        {
            add
            {
                foreach (var monitorEvents in AllEvents.Values)
                {
                    monitorEvents.MonitorStopping += value;
                }
            }

            remove
            {
                foreach (var monitorEvents in AllEvents.Values)
                {
                    monitorEvents.MonitorStopping -= value;
                }
            }
        }
    }

    internal class FareMonitorEvents
    {
        internal event FareMonitorHandler MonitorStarting;
        internal event FareMonitorHandler MonitorStopping;

        internal event FareRequestHandler RequestStarting;
        internal event FareRequestHandler RequestStopping;
        internal event FareRequestHandler RequestCompleted;

        internal void OnRequestStarting(FareRequestMonitor sender, FareBrowserRequestArg args)
        {
            RequestStarting.Raise(sender, args);
        }

        internal void OnRequestStopping(FareRequestMonitor sender, FareBrowserRequestArg args)
        {
            RequestStopping.Raise(sender, args);
        }

        internal void OnRequestCompleted(FareRequestMonitor sender, FareBrowserRequestArg args)
        {
            RequestCompleted.Raise(sender, args);
        }

        internal void OnMonitorStarting(FareRequestMonitor sender)
        {
            MonitorStarting.Raise(sender);
        }

        internal void OnMonitorStopping(FareRequestMonitor sender)
        {
            MonitorStopping.Raise(sender);
        }

        internal void Attach(FareRequestMonitor monitor)
        {
            monitor.MonitorStarting += OnMonitorStarting;
            monitor.MonitorStopping += OnMonitorStopping;

            monitor.RequestStarting += OnRequestStarting;
            monitor.RequestStopping += OnRequestStopping;
            monitor.RequestCompleted += OnRequestCompleted;
        }
    }

    public static class FareMonitorEventsExtensions
    {
        [DebuggerStepThrough]
        public static void Raise(this FareRequestHandler target, FareRequestMonitor sender, FareBrowserRequestArg args)
        {
            if (target != null)
            {
                target(sender, args);
            }
        }

        [DebuggerStepThrough]
        public static void Raise(this FareMonitorHandler target, FareRequestMonitor sender)
        {
            if (target != null)
            {
                target(sender);
            }
        }

        [DebuggerStepThrough]
        public static void Raise<T>(this Action<T> action, T parameter)
        {
            if (action != null)
            {
                action(parameter);
            }
        }
    }
}
