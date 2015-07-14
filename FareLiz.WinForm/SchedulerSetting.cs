namespace SkyDean.FareLiz.WinForm
{
    using System;
    using System.ComponentModel;

    /// <summary>The scheduler setting.</summary>
    [DefaultProperty("isEnabled")]
    internal class SchedulerSetting
    {
        /// <summary>The _is enabled.</summary>
        private bool _isEnabled = true;

        /// <summary>The _scheduled time.</summary>
        private DateTime _scheduledTime = DateTime.Now;

        /// <summary>Initializes a new instance of the <see cref="SchedulerSetting" /> class.</summary>
        public SchedulerSetting()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulerSetting"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public SchedulerSetting(string name)
        {
            this.TaskName = name;
        }

        /// <summary>Gets or sets the task name.</summary>
        [Description("Name of scheduled task")]
        public string TaskName { get; set; }

        /// <summary>Gets or sets a value indicating whether is enabled.</summary>
        [Description("Enable/Disable the scheduler")]
        public bool IsEnabled
        {
            get
            {
                return this._isEnabled;
            }

            set
            {
                this._isEnabled = value;
            }
        }

        /// <summary>Gets or sets the scheduled start time.</summary>
        [Description("Scheduled time to check for new fares")]
        public DateTime ScheduledStartTime
        {
            get
            {
                return this._scheduledTime;
            }

            set
            {
                this._scheduledTime = value;
            }
        }
    }
}