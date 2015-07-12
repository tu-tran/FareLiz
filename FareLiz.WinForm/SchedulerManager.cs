using log4net;
using SkyDean.FareLiz.Core.Utils;
using SkyDean.FareLiz.Service.TaskScheduler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SkyDean.FareLiz.WinForm
{
    internal class ScheduleItem
    {
        public ScheduleItem()
        {
        }

        public ScheduleItem(SchedulerSetting setting, ExecutionParam parameters, bool isCreated, bool isDirty, string createdName)
        {
            Setting = setting;
            Parameters = parameters;
            if (Parameters.DepartureDate == DateTime.MinValue)
                Parameters.DepartureDate = DateTime.Now.Date;
            if (Parameters.ReturnDate == DateTime.MinValue)
                Parameters.ReturnDate = Parameters.DepartureDate.Date.AddDays(Parameters.MaxStayDuration);
            IsCreated = isCreated;
            CreatedName = createdName;
        }

        public SchedulerSetting Setting { get; set; }
        public ExecutionParam Parameters { get; set; }
        public bool IsCreated { get; set; }
        public bool IsDirty { get; set; }
        public string CreatedName { get; set; }
    }

    internal sealed class SchedulerManager
    {
        public static readonly string CurrentExe = Assembly.GetEntryAssembly().Location;
        private readonly ILog _logger;

        internal SchedulerManager(ILog logger)
        {
            _logger = logger;
        }

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
                                if (String.Compare(CurrentExe,
                                                   Path.GetFullPath(action.Path.Replace("\"", String.Empty)),
                                                   StringComparison.InvariantCultureIgnoreCase) == 0)
                                {
                                    try
                                    {
                                        ExecutionParam param = null;
                                        try
                                        {
                                            ExecutionParam.Parse(action.Arguments.SplitCommandLine(), null, _logger, out param);
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
                                                TaskName = task.Name.Replace(AppUtil.ProductName, ""),
                                                IsEnabled = task.Enabled,
                                                ScheduledStartTime = activeTriggerDate
                                            };
                                        result.Add(new ScheduleItem(setting, param, true, false, task.Name));
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.Warn("Failed to get scheduled tasks: " + ex.Message);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.ErrorFormat("Could not load task [{0}]: {1}", task.Name, ex.Message);
                    }
                }
            }

            return result;
        }

        internal void Schedule(ScheduleItem item)
        {
            if (item.IsCreated && !item.IsDirty)
                return;

            // Get the service on the local machine
            using (var ts = new TaskService())
            {
                // Create a new task definition and assign properties
                if (item.IsCreated)
                {
                    RemoveTask(item.CreatedName, ts);
                    item.IsCreated = false;
                }

                TaskDefinition td = ts.NewTask();
                td.Settings.Enabled = item.Setting.IsEnabled;
                string deptRangeStr = String.Format("{0}(+{1} -{2})", item.Parameters.DepartureDate.ToShortDayAndDateString(), item.Parameters.DepartureDateRange.Plus, item.Parameters.DepartureDateRange.Minus);
                string retRangeStr = (item.Parameters.ReturnDate.IsUndefined() ? null : String.Format("{0}(+{1} -{2})", item.Parameters.ReturnDate.ToShortDayAndDateString(), item.Parameters.ReturnDateRange.Plus, item.Parameters.ReturnDateRange.Minus));
                string stayDurationStr = (item.Parameters.ReturnDate.IsUndefined() ? "0" : item.Parameters.MinStayDuration + " <-> " + item.Parameters.MaxStayDuration);

                td.RegistrationInfo.Description = String.Format(@"{0} {1} | 
Departure: {2} | 
Destination: {3} | 
Travel Period: {4} <-> {5} | 
Stay Duration: {6} | 
Price Limit: {7}", AppUtil.CompanyName, AppUtil.ProductName, item.Parameters.Departure, item.Parameters.Destination,
                    deptRangeStr, retRangeStr,
                    stayDurationStr,
                    item.Parameters.PriceLimit);

                td.Triggers.Add(new DailyTrigger(0)
                    {
                        DaysInterval = 1,
                        Enabled = true,
                        StartBoundary = item.Setting.ScheduledStartTime
                    });

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

        internal void RemoveTask(string taskName)
        {
            using (var ts = new TaskService())
            {
                RemoveTask(taskName, ts);
            }
        }

        private void RemoveTask(string taskName, TaskService service)
        {
            Task existTask = service.FindTask(taskName, false);
            if (existTask != null)
                service.RootFolder.DeleteTask(taskName);
        }
    }
}