using System;
using System.ComponentModel;

namespace SkyDean.FareLiz.WinForm
{
    [DefaultProperty("isEnabled")]
    internal class SchedulerSetting
    {
        private bool _isEnabled = true;
        private DateTime _scheduledTime = DateTime.Now;

        public SchedulerSetting()
        {
        }

        public SchedulerSetting(string name)
        {
            TaskName = name;
        }

        [Description("Name of scheduled task")]
        public string TaskName { get; set; }

        [Description("Enable/Disable the scheduler")]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        [Description("Scheduled time to check for new fares")]
        public DateTime ScheduledStartTime
        {
            get { return _scheduledTime; }
            set { _scheduledTime = value; }
        }
    }
}