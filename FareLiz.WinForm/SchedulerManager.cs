namespace SkyDean.FareLiz.WinForm
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    using log4net;

    using Microsoft.Win32.TaskScheduler;

    using SkyDean.FareLiz.Core.Utils;

    /// <summary>
    /// The schedule item.
    /// </summary>
    internal class ScheduleItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleItem"/> class.
        /// </summary>
        public ScheduleItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleItem"/> class.
        /// </summary>
        /// <param name="setting">
        /// The setting.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="isCreated">
        /// The is created.
        /// </param>
        /// <param name="isDirty">
        /// The is dirty.
        /// </param>
        /// <param name="createdName">
        /// The created name.
        /// </param>
        public ScheduleItem(SchedulerSetting setting, ExecutionParam parameters, bool isCreated, bool isDirty, string createdName)
        {
            this.Setting = setting;
            this.Parameters = parameters;
            if (this.Parameters.DepartureDate == DateTime.MinValue)
            {
                this.Parameters.DepartureDate = DateTime.Now.Date;
            }

            if (this.Parameters.ReturnDate == DateTime.MinValue)
            {
                this.Parameters.ReturnDate = this.Parameters.DepartureDate.Date.AddDays(this.Parameters.MaxStayDuration);
            }

            this.IsCreated = isCreated;
            this.CreatedName = createdName;
        }

        /// <summary>
        /// Gets or sets the setting.
        /// </summary>
        public SchedulerSetting Setting { get; set; }

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        public ExecutionParam Parameters { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is created.
        /// </summary>
        public bool IsCreated { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is dirty.
        /// </summary>
        public bool IsDirty { get; set; }

        /// <summary>
        /// Gets or sets the created name.
        /// </summary>
        public string CreatedName { get; set; }
    }

    /// <summary>
    /// The scheduler manager.
    /// </summary>
    internal sealed class SchedulerManager
    {
        /// <summary>
        /// The current exe.
        /// </summary>
        public static readonly string CurrentExe = Assembly.GetEntryAssembly().Location;

        /// <summary>
        /// The _logger.
        /// </summary>
        private readonly ILog _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulerManager"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        internal SchedulerManager(ILog logger)
        {
            this._logger = logger;
        }

        /// <summary>
        /// The get scheduled tasks.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<ScheduleItem> GetScheduledTasks()
        {
            var result = new List<ScheduleItem>();

            // Get the service on the local machine
            using (var ts = new TaskService())
            {
                TaskCollection tc = ts.RootFolder.GetTasks(null);
                foreach (Task task in tc)
                {
                    try
                    {
                        if (task.Definition.Actions.Count > 0)
                        {
                            var action = task.Definition.Actions[0] as ExecAction;
                            if (action != null)
                            {
                                if (string.Compare(
                                    CurrentExe, 
                                    Path.GetFullPath(action.Path.Replace("\"", string.Empty)), 
                                    StringComparison.InvariantCultureIgnoreCase) == 0)
                                {
                                    try
                                    {
                                        ExecutionParam param = null;
                                        try
                                        {
                                            ExecutionParam.Parse(action.Arguments.SplitCommandLine(), null, this._logger, out param);
                                        }
                                        catch
                                        {
                                            param = new ExecutionParam();
                                        }

                                        DateTime activeTriggerDate = DateTime.MinValue;
                                        foreach (Trigger trigger in task.Definition.Triggers)
                                        {
                                            var timeTrigger = trigger as DailyTrigger;
                                            if (timeTrigger != null)
                                            {
                                                activeTriggerDate = timeTrigger.StartBoundary;
                                                break;
                                            }
                                        }

                                        var setting = new SchedulerSetting
                                                          {
                                                              TaskName = task.Name.Replace(AppUtil.ProductName, string.Empty), 
                                                              IsEnabled = task.Enabled, 
                                                              ScheduledStartTime = activeTriggerDate
                                                          };
                                        result.Add(new ScheduleItem(setting, param, true, false, task.Name));
                                    }
                                    catch (Exception ex)
                                    {
                                        this._logger.Warn("Failed to get scheduled tasks: " + ex.Message);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this._logger.ErrorFormat("Could not load task [{0}]: {1}", task.Name, ex.Message);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// The schedule.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        internal void Schedule(ScheduleItem item)
        {
            if (item.IsCreated && !item.IsDirty)
            {
                return;
            }

            // Get the service on the local machine
            using (var ts = new TaskService())
            {
                // Create a new task definition and assign properties
                if (item.IsCreated)
                {
                    this.RemoveTask(item.CreatedName, ts);
                    item.IsCreated = false;
                }

                TaskDefinition td = ts.NewTask();
                td.Settings.Enabled = item.Setting.IsEnabled;
                string deptRangeStr = string.Format(
                    "{0}(+{1} -{2})", 
                    item.Parameters.DepartureDate.ToShortDayAndDateString(), 
                    item.Parameters.DepartureDateRange.Plus, 
                    item.Parameters.DepartureDateRange.Minus);
                string retRangeStr = item.Parameters.ReturnDate.IsUndefined()
                                          ? null
                                          : string.Format(
                                              "{0}(+{1} -{2})", 
                                              item.Parameters.ReturnDate.ToShortDayAndDateString(), 
                                              item.Parameters.ReturnDateRange.Plus, 
                                              item.Parameters.ReturnDateRange.Minus);
                string stayDurationStr = item.Parameters.ReturnDate.IsUndefined()
                                              ? "0"
                                              : item.Parameters.MinStayDuration + " <-> " + item.Parameters.MaxStayDuration;

                td.RegistrationInfo.Description = string.Format(
                    @"{0} {1} | 
Departure: {2} | 
Destination: {3} | 
Travel Period: {4} <-> {5} | 
Stay Duration: {6} | 
Price Limit: {7}", 
                    AppUtil.CompanyName, 
                    AppUtil.ProductName, 
                    item.Parameters.Departure, 
                    item.Parameters.Destination, 
                    deptRangeStr, 
                    retRangeStr, 
                    stayDurationStr, 
                    item.Parameters.PriceLimit);

                td.Triggers.Add(new DailyTrigger(0) { DaysInterval = 1, Enabled = true, StartBoundary = item.Setting.ScheduledStartTime });

                string cmdLine = item.Parameters.GenerateCommandLine();
                td.Actions.Add(new ExecAction(CurrentExe, cmdLine, Path.GetDirectoryName(CurrentExe)));

                // Register the task in the root folder
                string taskName = AppUtil.ProductName + " " + item.Setting.TaskName;
                item.CreatedName = taskName;
                ts.RootFolder.RegisterTaskDefinition(item.CreatedName, td);
                item.IsDirty = false;
                item.IsCreated = true;
            }
        }

        /// <summary>
        /// The remove task.
        /// </summary>
        /// <param name="taskName">
        /// The task name.
        /// </param>
        internal void RemoveTask(string taskName)
        {
            using (var ts = new TaskService())
            {
                this.RemoveTask(taskName, ts);
            }
        }

        /// <summary>
        /// The remove task.
        /// </summary>
        /// <param name="taskName">
        /// The task name.
        /// </param>
        /// <param name="service">
        /// The service.
        /// </param>
        private void RemoveTask(string taskName, TaskService service)
        {
            Task existTask = service.FindTask(taskName, false);
            if (existTask != null)
            {
                service.RootFolder.DeleteTask(taskName);
            }
        }
    }
}