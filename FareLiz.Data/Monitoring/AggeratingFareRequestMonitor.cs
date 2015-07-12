namespace SkyDean.FareLiz.Data.Monitoring
{
    using System;

    using SkyDean.FareLiz.Core.Utils;

    /// <summary>
    /// Aggerating monitor which keeps track of multiple monitors
    /// </summary>
    public class AggeratingFareRequestMonitor : IDisposable
    {
        private readonly DataQueue<FareRequestMonitor> _startedMonitors = new DataQueue<FareRequestMonitor>();
        private readonly object _syncObj = new object();

        public void Stop()
        {
            var startedMons = this._startedMonitors.ToList();
            foreach (var mon in startedMons)
                mon.Stop();
        }

        public void Start(FareRequestMonitor monitor)
        {
            monitor.Start();
            this._startedMonitors.Enqueue(monitor);
        }

        public void Clear()
        {
            this.Stop();
            this._startedMonitors.Clear(true);
        }

        public void Clear(Type monitorType)
        {
            if (this._startedMonitors.Count < 1)
                return;

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

        public void Dispose()
        {
            lock (this._syncObj)
            {
                this.Stop();
                this.Clear();
            }
        }
    }
}
