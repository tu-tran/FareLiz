namespace SkyDean.FareLiz.WinForm.Presentation.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using SkyDean.FareLiz.Data.Monitoring;

    /// <summary>
    /// The check fare events.
    /// </summary>
    internal class CheckFareEvents
    {
        /// <summary>
        /// The all events.
        /// </summary>
        internal readonly Dictionary<OperationMode, FareMonitorEvents> AllEvents = new Dictionary<OperationMode, FareMonitorEvents>
                                                                                       {
                                                                                           {
                                                                                               OperationMode
                                                                                               .ShowFare, 
                                                                                               new FareMonitorEvents
                                                                                               ()
                                                                                           }, 
                                                                                           {
                                                                                               OperationMode
                                                                                               .GetFareAndSave, 
                                                                                               new FareMonitorEvents
                                                                                               ()
                                                                                           }, 
                                                                                           {
                                                                                               OperationMode
                                                                                               .LiveMonitor, 
                                                                                               new FareMonitorEvents
                                                                                               ()
                                                                                           }
                                                                                       };

        /// <summary>
        /// The this.
        /// </summary>
        /// <param name="mode">
        /// The mode.
        /// </param>
        /// <returns>
        /// The <see cref="FareMonitorEvents"/>.
        /// </returns>
        internal FareMonitorEvents this[OperationMode mode]
        {
            get
            {
                return this.AllEvents[mode];
            }
        }

        /// <summary>
        /// The monitor starting.
        /// </summary>
        internal event FareMonitorHandler MonitorStarting
        {
            add
            {
                foreach (var monitorEvents in this.AllEvents.Values)
                {
                    monitorEvents.MonitorStarting += value;
                }
            }

            remove
            {
                foreach (var monitorEvents in this.AllEvents.Values)
                {
                    monitorEvents.MonitorStarting -= value;
                }
            }
        }

        /// <summary>
        /// The monitor stopping.
        /// </summary>
        internal event FareMonitorHandler MonitorStopping
        {
            add
            {
                foreach (var monitorEvents in this.AllEvents.Values)
                {
                    monitorEvents.MonitorStopping += value;
                }
            }

            remove
            {
                foreach (var monitorEvents in this.AllEvents.Values)
                {
                    monitorEvents.MonitorStopping -= value;
                }
            }
        }
    }

    /// <summary>
    /// The fare monitor events.
    /// </summary>
    internal class FareMonitorEvents
    {
        /// <summary>
        /// The monitor starting.
        /// </summary>
        internal event FareMonitorHandler MonitorStarting;

        /// <summary>
        /// The monitor stopping.
        /// </summary>
        internal event FareMonitorHandler MonitorStopping;

        /// <summary>
        /// The request starting.
        /// </summary>
        internal event FareRequestHandler RequestStarting;

        /// <summary>
        /// The request stopping.
        /// </summary>
        internal event FareRequestHandler RequestStopping;

        /// <summary>
        /// The request completed.
        /// </summary>
        internal event FareRequestHandler RequestCompleted;

        /// <summary>
        /// The on request starting.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        internal void OnRequestStarting(FareRequestMonitor sender, FareBrowserRequestArg args)
        {
            this.RequestStarting.Raise(sender, args);
        }

        /// <summary>
        /// The on request stopping.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        internal void OnRequestStopping(FareRequestMonitor sender, FareBrowserRequestArg args)
        {
            this.RequestStopping.Raise(sender, args);
        }

        /// <summary>
        /// The on request completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        internal void OnRequestCompleted(FareRequestMonitor sender, FareBrowserRequestArg args)
        {
            this.RequestCompleted.Raise(sender, args);
        }

        /// <summary>
        /// The on monitor starting.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        internal void OnMonitorStarting(FareRequestMonitor sender)
        {
            this.MonitorStarting.Raise(sender);
        }

        /// <summary>
        /// The on monitor stopping.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        internal void OnMonitorStopping(FareRequestMonitor sender)
        {
            this.MonitorStopping.Raise(sender);
        }

        /// <summary>
        /// The attach.
        /// </summary>
        /// <param name="monitor">
        /// The monitor.
        /// </param>
        internal void Attach(FareRequestMonitor monitor)
        {
            monitor.MonitorStarting += this.OnMonitorStarting;
            monitor.MonitorStopping += this.OnMonitorStopping;

            monitor.RequestStarting += this.OnRequestStarting;
            monitor.RequestStopping += this.OnRequestStopping;
            monitor.RequestCompleted += this.OnRequestCompleted;
        }
    }

    /// <summary>
    /// The fare monitor events extensions.
    /// </summary>
    public static class FareMonitorEventsExtensions
    {
        /// <summary>
        /// The raise.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        [DebuggerStepThrough]
        public static void Raise(this FareRequestHandler target, FareRequestMonitor sender, FareBrowserRequestArg args)
        {
            if (target != null)
            {
                target(sender, args);
            }
        }

        /// <summary>
        /// The raise.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="sender">
        /// The sender.
        /// </param>
        [DebuggerStepThrough]
        public static void Raise(this FareMonitorHandler target, FareRequestMonitor sender)
        {
            if (target != null)
            {
                target(sender);
            }
        }

        /// <summary>
        /// The raise.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
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