namespace SkyDean.FareLiz.Data.Monitoring
{
    using System;

    using SkyDean.FareLiz.Core.Utils;

    /// <summary>Aggerating monitor which keeps track of multiple monitors</summary>
    public class AggeratingFareRequestMonitor : IDisposable
    {
        /// <summary>
        /// The _started monitors.
        /// </summary>
        private readonly DataQueue<FareRequestMonitor> _startedMonitors = new DataQueue<FareRequestMonitor>();

        /// <summary>
        /// The _sync obj.
        /// </summary>
        private readonly object _syncObj = new object();

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            lock (this._syncObj)
            {
                this.Stop();
                this.Clear();
            }
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            var startedMons = this._startedMonitors.ToList();
            foreach (var mon in startedMons)
            {
                mon.Stop();
            }
        }

        /// <summary>
        /// The start.
        /// </summary>
        /// <param name="monitor">
        /// The monitor.
        /// </param>
        public void Start(FareRequestMonitor monitor)
        {
            monitor.Start();
            this._startedMonitors.Enqueue(monitor);
        }

        /// <summary>
        /// The clear.
        /// </summary>
        public void Clear()
        {
            this.Stop();
            this._startedMonitors.Clear(true);
        }

        /// <summary>
        /// The clear.
        /// </summary>
        /// <param name="monitorType">
        /// The monitor type.
        /// </param>
        public void Clear(Type monitorType)
        {
            if (this._startedMonitors.Count < 1)
            {
                return;
            }

            var allMons = this._startedMonitors.ToList();
            foreach (var mon in allMons)
            {
                if (monitorType.IsInstanceOfType(mon))
                {
                    using (mon)
                    {
                        mon.Stop();
                        this._startedMonitors.Remove(mon);
                    }
                }
            }
        }
    }
}