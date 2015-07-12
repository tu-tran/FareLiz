using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using log4net;
using SkyDean.FareLiz.Core.Utils;
using SkyDean.FareLiz.Data.Monitoring;
using SkyDean.FareLiz.WinForm.Components.Controls;
using SkyDean.FareLiz.WinForm.Components.Controls.Custom;
using SkyDean.FareLiz.WinForm.Components.Controls.ListView;
using SkyDean.FareLiz.WinForm.Components.Dialog;
using SkyDean.FareLiz.WinForm.Components.Utils;

namespace SkyDean.FareLiz.WinForm.Presentation.Views
{
    /// <summary>
    /// Provides GUI for scheduling an automated task
    /// </summary>
    internal partial class SchedulerForm : SmartForm
    {
        private readonly ExecutionParam _defaultParam;
        private readonly SchedulerManager _scheduler;
        private readonly ToolStripControl<Windows7ProgressBar> loadProgress = new ToolStripControl<Windows7ProgressBar>() { Visible = false };
        private readonly ILog _logger;
        private bool _bindingObject = false;
        private readonly HashSet<string> _removingTasks = new HashSet<string>();

        public SchedulerForm(ExecutionParam defaultParam, ILog logger)
        {
            InitializeComponent();
            InitializeImageList();
            _logger = logger;
            _scheduler = new SchedulerManager(_logger);
            Text = AppUtil.ProductName + " " + Text;
            loadProgress.ControlItem.ContainerControl = this;
            loadProgress.ControlItem.ShowInTaskbar = false;
            statusStrip.Items.Add(loadProgress);
            _defaultParam = defaultParam.ReflectionDeepClone(logger);
            if (_defaultParam.OperationMode != OperationMode.GetFareAndSave && _defaultParam.OperationMode != OperationMode.LiveMonitor)
                _defaultParam.OperationMode = OperationMode.GetFareAndSave;
            loadProgress.VisibleChanged += LoadProgress_VisibleChanged;
        }

        private void InitializeImageList()
        {
            imgListSchedules.Images.Add("Add", Properties.Resources.Add);
            imgListSchedules.Images.Add("Edit", Properties.Resources.Edit);
            imgListSchedules.Images.Add("Error", Properties.Resources.Stop);
        }

        private void ResizeStatusStrip()
        {
            int minusWidth = (statusStrip.SizingGrip ? statusStrip.SizeGripBounds.Width : 0) + 5 + 2 * SystemInformation.BorderSize.Width + loadProgress.Margin.Left + loadProgress.Margin.Right;
            foreach (ToolStripItem item in statusStrip.Items)
                if (item != loadProgress && item.Visible)
                    minusWidth += item.Width + item.Margin.Left + item.Margin.Right;

            int newWidth = statusStrip.Width - minusWidth;
            if (newWidth > 0)
                loadProgress.Size = new Size(newWidth, statusStrip.Height / 2);
        }

        private void Reload()
        {
            btnRefresh.Enabled = false;
            string initalStatus = "Loading scheduled tasks...";
            lblStatus.Text = initalStatus;
            lblStatus.Image = Properties.Resources.Loading;
            loadProgress.ControlItem.Style = ProgressBarStyle.Marquee;
            loadProgress.Visible = true;
            SetEditMode(false);

            ThreadPool.QueueUserWorkItem(o =>
            {
                try
                {
                    AppUtil.NameCurrentThread(GetType().Name + "-Load");
                    List<ScheduleItem> tasks = _scheduler.GetScheduledTasks();
                    this.SafeInvoke(new MethodInvoker(() =>
                    {
                        var oldItems = lvSchedules.Items;
                        // Compare the old item list with the new one
                        for (int i = 0; i < oldItems.Count; i++)
                        {
                            var curLvItem = oldItems[i];
                            var curItem = (curLvItem.Tag as ScheduleItem);
                            if (curItem != null && curItem.IsCreated)
                            {
                                bool isRemoved = true;
                                for (int j = 0; j < tasks.Count; j++)
                                {
                                    ScheduleItem t = tasks[j];
                                    if (String.Equals(t.CreatedName, curItem.CreatedName, StringComparison.OrdinalIgnoreCase))
                                    {
                                        isRemoved = false;
                                        curLvItem.Tag = t;
                                        tasks.RemoveAt(j);
                                        break;
                                    }
                                }

                                if (isRemoved)
                                    lvSchedules.Items.RemoveAt(i--);
                            }
                        }

                        if (tasks.Count > 0)
                            foreach (var t in tasks)
                                lvSchedules.Items.Add(new ListViewItem(t.Setting.TaskName) { Tag = t });

                        lvSchedules.Focus();
                        if (lvSchedules.Items.Count > 0)
                            lvSchedules.Items[0].Selected = true;
                        SetGridSize();
                    }));
                }
                finally
                {
                    this.SafeInvoke(new Action(() =>
                    {
                        if (lblStatus.Text == initalStatus)
                        {
                            loadProgress.Visible = false;
                            lblStatus.Text = "Idle";
                            lblStatus.Image = null;
                            btnRefresh.Enabled = true;
                        }
                        BindScheduleItemProperty();
                        SetEditMode(true);
                        UpdateButtons();
                    }));
                }
            });
        }

        private void UpdateButtons()
        {
            bool canSave = false;
            foreach (ListViewItem lvItem in lvSchedules.Items)
            {
                var item = lvItem.Tag as ScheduleItem;
                if (item != null)
                {
                    if (item.IsCreated && !item.IsDirty)
                        continue;

                    canSave = true;
                    break;
                }
            }

            btnSave.Enabled = canSave;
        }

        private string GetExecutionParamArgs()
        {
            string cmdLine = null;
            var obj = flightScannerProperties.SelectedObject as ExecutionParam;
            if (obj != null)
            {
                if (obj.OperationMode == OperationMode.Unspecified)
                    throw new ArgumentException("Operation Mode was not specified");

                cmdLine = obj.GenerateCommandLine();
            }

            return cmdLine;
        }

        private void SetGridSize()
        {
            flightScannerProperties.SetLabelColumnWidth(150);
        }

        private void SetEditMode(bool enabled)
        {
            btnSave.Enabled = btnAdd.Enabled = btnDelete.Enabled = btnDeleteAll.Enabled = enabled;
        }

        private void BindScheduleItemProperty()
        {
            if (lvSchedules.SelectedItems.Count == 1)
            {
                var selectedItem = lvSchedules.SelectedItems[0].Tag as ScheduleItem;
                flightScannerProperties.SelectedObject = selectedItem.Parameters;
                schedulerProperties.SelectedObject = selectedItem.Setting;
                SetGridSize();
            }
            else
            {
                flightScannerProperties.SelectedObject = null;
                schedulerProperties.SelectedObject = null;
            }
        }

        /// <summary>
        /// Mark the active item as dirty (being edited) and returns the active item
        /// </summary>
        /// <returns></returns>
        private void MarkActiveItemDirty()
        {
            var activeItem = GetActiveItem();
            if (activeItem != null)
            {
                ScheduleItem selItem = activeItem.Value;
                selItem.IsDirty = true;
                if (selItem.IsCreated)
                    activeItem.Key.ImageKey = "Edit";
            }
            UpdateButtons();
        }

        private KeyValue<ListViewItem, ScheduleItem> GetActiveItem()
        {
            if (lvSchedules.SelectedItems.Count == 1)
            {
                var selLvItem = lvSchedules.SelectedItems[0];
                var selItem = selLvItem.Tag as ScheduleItem;
                if (selItem != null)
                    return new KeyValue<ListViewItem, ScheduleItem>(selLvItem, selItem);
            }

            return null;
        }

        private void SchedulerForm_Shown(object sender, EventArgs e)
        {
            lvSchedules.Columns[0].Width = lvSchedules.Width - 21;
            schedulerProperties.DescriptionAreaLineCount = 0;
            schedulerProperties.DescriptionAreaHeight = 0;
            Reload();
        }

        private void SchedulerForm_SizeChanged(object sender, EventArgs e)
        {
            ResizeStatusStrip();
        }

        private void lvSchedules_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindScheduleItemProperty();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string newItemName = Guid.NewGuid().ToString();
            var newLvItem = new ListViewItem(newItemName)
                {
                    ImageKey = "Add",
                    Tag = new ScheduleItem(new SchedulerSetting(newItemName), _defaultParam, false, true, null)
                };
            lvSchedules.Items.Add(newLvItem);
            lvSchedules.Select();
            if (lvSchedules.SelectedItems.Count == 1)
                lvSchedules.SelectedItems[0].Selected = false;
            if (lvSchedules.SelectedItems.Count == 0)
                newLvItem.Selected = true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lvSchedules.SelectedItems.Count > 0)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Are you sure you want to delete the following scheduled tasks:" + Environment.NewLine);
                foreach (ListViewItem selItem in lvSchedules.SelectedItems)
                    sb.AppendLine("\t" + selItem.Text);

                if (MessageBox.Show(this, sb.ToString(), "Delete confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var firstSelIndex = lvSchedules.SelectedIndices[0];
                    foreach (ListViewItem selItem in lvSchedules.SelectedItems)
                    {
                        var scheduleItem = selItem.Tag as ScheduleItem;
                        if (scheduleItem != null && scheduleItem.IsCreated)
                        {
                            var taskName = scheduleItem.CreatedName;
                            _scheduler.RemoveTask(taskName);
                        }
                        lvSchedules.Items.Remove(selItem);
                    }

                    if (--firstSelIndex >= 0)
                        lvSchedules.Items[firstSelIndex].Selected = true;
                    else if (lvSchedules.Items.Count > 0)
                        lvSchedules.Items[0].Selected = true;
                    else
                        lvSchedules_SelectedIndexChanged(null, null);
                    lvSchedules.Select();
                }
            }
        }

        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            if (lvSchedules.Items.Count > 0)
            {
                if (MessageBox.Show(this, "Are you sure you want to delete all scheduled tasks?", "Delete confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    for (int i = 0; i < lvSchedules.Items.Count; i++)
                    {
                        var item = lvSchedules.Items[i].Tag as ScheduleItem;
                        if (item != null && item.IsCreated)
                        {
                            string taskName = (String.IsNullOrEmpty(item.CreatedName) ? item.Setting.TaskName : item.CreatedName);
                            _scheduler.RemoveTask(taskName);
                        }
                        lvSchedules.Items.RemoveAt(i--);
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (lvSchedules.Items.Count < 1)
                return;

            var itemTags = new Dictionary<ListViewItem, ScheduleItem>();
            foreach (ListViewItem item in lvSchedules.Items)
            {
                var scheduleItem = item.Tag as ScheduleItem;
                if (scheduleItem != null)
                    itemTags.Add(item, scheduleItem);
            }

            if (itemTags.Count > 0)
            {
                string initialStatus = lblStatus.Text = "Saving scheduled tasks...";
                lblStatus.Image = Properties.Resources.Loading;
                btnSave.Enabled = false;
                loadProgress.Visible = true;
                var progressBar = loadProgress.ControlItem;
                progressBar.Value = 0;
                progressBar.Maximum = lvSchedules.Items.Count;
                progressBar.Style = (itemTags.Count > 1 ? ProgressBarStyle.Blocks : ProgressBarStyle.Marquee);

                ThreadPool.QueueUserWorkItem(o =>
                    {
                        AppUtil.NameCurrentThread(GetType().Name + "-Save");
                        try
                        {
                            foreach (var delItem in _removingTasks)
                                _scheduler.RemoveTask(delItem);
                            _removingTasks.Clear();

                            foreach (var pair in itemTags)
                            {
                                bool success = true;
                                string error = null;
                                var task = pair.Value;
                                try
                                {
                                    if (task.Parameters.DepartureDate == DateTime.MinValue ||
                                        task.Parameters.ReturnDate == DateTime.MinValue)
                                        throw new ArgumentException("The departure and return date must be specified!");
                                    else
                                        _scheduler.Schedule(task);
                                }
                                catch (Exception ex)
                                {
                                    success = false;
                                    error = ex.Message;
                                }

                                this.SafeInvoke(new MethodInvoker(() =>
                                    {
                                        pair.Key.ImageKey = (success ? null : "Error");
                                        if (!success)
                                            MessageBox.Show(this, "Failed to create task with ID: " + task.Setting.TaskName + Environment.NewLine + Environment.NewLine
                                                + error, "Failed to create scheduled task", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        progressBar.Value++;
                                    }));
                            }
                        }
                        finally
                        {
                            this.SafeInvoke(new MethodInvoker(() =>
                                {
                                    if (lblStatus.Text == initialStatus)
                                    {
                                        lblStatus.Text = "Idle";
                                        lblStatus.Image = null;
                                        progressBar.Value = progressBar.Maximum = 0;
                                        loadProgress.Visible = false;
                                    }

                                    btnSave.Enabled = true;
                                    lvSchedules.Refresh();
                                    UpdateButtons();
                                }));
                        }
                    });
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void schedulerProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            MarkActiveItemDirty();

            // In case of renaming the task
            if (e.ChangedItem.Label == "TaskName")
            {
                var newTaskName = e.ChangedItem.Value.ToString();
                var oldTaskName = e.OldValue.ToString();
                if (lvSchedules.SelectedItems.Count > 0)
                {
                    foreach (ListViewItem item in lvSchedules.Items)
                        if (item.Text == newTaskName)
                        {
                            MessageBox.Show(String.Format("Task name [{0}] already existed!", item.Text), "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            var obj = schedulerProperties.SelectedObject as SchedulerSetting;
                            obj.TaskName = oldTaskName;
                            return;
                        }

                    var activeItem = GetActiveItem();
                    if (activeItem != null)
                    {
                        ScheduleItem selTask = activeItem.Value;
                        if (selTask != null && selTask.IsCreated && !selTask.IsDirty)
                            _removingTasks.Add(oldTaskName);
                    }

                    lvSchedules.SelectedItems[0].Text = newTaskName;
                }
            }
        }

        private void flightScannerProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            MarkActiveItemDirty();
        }

        private void flightScannerProperties_SelectedObjectsChanged(object sender, EventArgs e)
        {
            var selObject = flightScannerProperties.SelectedObject as ExecutionParam;
            txtDeparture.Enabled = txtDestination.Enabled = (selObject != null);
            if (selObject == null)
                txtDeparture.SelectedAirport = txtDestination.SelectedAirport = null;
            else
            {
                _bindingObject = true;
                txtDeparture.SelectedAirport = selObject.Departure;
                txtDestination.SelectedAirport = selObject.Destination;
                _bindingObject = false;
            }
        }

        private void SchedulerForm_Resize(object sender, EventArgs e)
        {
            SetGridSize();
        }

        private void scannerSettingContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool selected = (flightScannerProperties.SelectedObject != null);
            foreach (ToolStripItem mnuItem in scannerSettingContextMenu.Items)
                mnuItem.Visible = selected;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void mnuCopyParamsToClipboard_Click(object sender, EventArgs e)
        {
            try
            {
                string args = GetExecutionParamArgs();
                if (!String.IsNullOrEmpty(args))
                    Clipboard.SetText(args);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Invalid setting", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void mnuExecute_Click(object sender, EventArgs e)
        {
            try
            {
                string args = GetExecutionParamArgs();
                if (!String.IsNullOrEmpty(args))
                    Process.Start(SchedulerManager.CurrentExe, args);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Execution Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void taskContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mnuDeleteTask.Visible = !(lvSchedules.SelectedIndices.Count < 1);
            mnuSeparator.Visible = mnuDeleteAll.Visible = !(lvSchedules.Items.Count < 1);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Reload();
        }

        private void Location_SelectedItemChanged(object sender, EventArgs e)
        {
            if (_bindingObject)
                return;

            var selParam = flightScannerProperties.SelectedObject as ExecutionParam;
            if (selParam != null)
            {
                if (sender == txtDeparture)
                    selParam.Departure = txtDeparture.SelectedAirport;
                else if (sender == txtDestination)
                    selParam.Destination = txtDestination.SelectedAirport;
            }

            MarkActiveItemDirty();
        }

        private void LoadProgress_VisibleChanged(object sender, EventArgs e)
        {
            loadProgress.ControlItem.ShowInTaskbar = loadProgress.Visible;
            ResizeStatusStrip();
        }

        private void lblStatus_TextChanged(object sender, EventArgs e)
        {
            ResizeStatusStrip();
        }

        private void lvSchedules_ItemsAdded(EnhancedListView listView, List<ListViewItem> items)
        {
            UpdateButtons();
        }

        private void lvSchedules_ItemRemoved(EnhancedListView listView, ListViewItem item)
        {
            UpdateButtons();
        }
    }

    public static class PropertyGridExtension
    {
        public static void SetLabelColumnWidth(this PropertyGrid grid, int width)
        {
            var view = (Control)grid.GetType().GetField("gridView", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(grid);
            FieldInfo fi = view.GetType().GetField("labelWidth", BindingFlags.Instance | BindingFlags.NonPublic);
            fi.SetValue(view, width);
            view.Invalidate();
        }
    }
}