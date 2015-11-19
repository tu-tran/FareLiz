namespace SkyDean.FareLiz.WinForm.Presentation.Views
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
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
    using SkyDean.FareLiz.WinForm.Properties;

    /// <summary>Provides GUI for scheduling an automated task</summary>
    internal partial class SchedulerForm : SmartForm
    {
        /// <summary>
        /// The _default param.
        /// </summary>
        private readonly ExecutionParam _defaultParam;

        /// <summary>
        /// The _logger.
        /// </summary>
        private readonly ILog _logger;

        /// <summary>
        /// The _removing tasks.
        /// </summary>
        private readonly HashSet<string> _removingTasks = new HashSet<string>();

        /// <summary>
        /// The _scheduler.
        /// </summary>
        private readonly SchedulerManager _scheduler;

        /// <summary>
        /// The load progress.
        /// </summary>
        private readonly ToolStripControl<Windows7ProgressBar> loadProgress = new ToolStripControl<Windows7ProgressBar> { Visible = false };

        /// <summary>
        /// The _binding object.
        /// </summary>
        private bool _bindingObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulerForm"/> class.
        /// </summary>
        /// <param name="defaultParam">
        /// The default param.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public SchedulerForm(ExecutionParam defaultParam, ILog logger)
        {
            this.InitializeComponent();
            this.InitializeImageList();
            this._logger = logger;
            this._scheduler = new SchedulerManager(this._logger);
            this.Text = AppUtil.ProductName + " " + this.Text;
            this.loadProgress.ControlItem.ContainerControl = this;
            this.loadProgress.ControlItem.ShowInTaskbar = false;
            this.statusStrip.Items.Add(this.loadProgress);
            this._defaultParam = defaultParam.ReflectionDeepClone(logger);
            if (this._defaultParam.OperationMode != OperationMode.GetFareAndSave && this._defaultParam.OperationMode != OperationMode.LiveMonitor)
            {
                this._defaultParam.OperationMode = OperationMode.GetFareAndSave;
            }

            this.loadProgress.VisibleChanged += this.LoadProgress_VisibleChanged;
        }

        /// <summary>
        /// The initialize image list.
        /// </summary>
        private void InitializeImageList()
        {
            this.imgListSchedules.Images.Add("Add", Resources.Add);
            this.imgListSchedules.Images.Add("Edit", Resources.Edit);
            this.imgListSchedules.Images.Add("Error", Resources.Stop);
        }

        /// <summary>
        /// The resize status strip.
        /// </summary>
        private void ResizeStatusStrip()
        {
            int minusWidth = (this.statusStrip.SizingGrip ? this.statusStrip.SizeGripBounds.Width : 0) + 5 + 2 * SystemInformation.BorderSize.Width
                             + this.loadProgress.Margin.Left + this.loadProgress.Margin.Right;
            foreach (ToolStripItem item in this.statusStrip.Items)
            {
                if (item != this.loadProgress && item.Visible)
                {
                    minusWidth += item.Width + item.Margin.Left + item.Margin.Right;
                }
            }

            int newWidth = this.statusStrip.Width - minusWidth;
            if (newWidth > 0)
            {
                this.loadProgress.Size = new Size(newWidth, this.statusStrip.Height / 2);
            }
        }

        /// <summary>
        /// The reload.
        /// </summary>
        private void Reload()
        {
            this.btnRefresh.Enabled = false;
            string initalStatus = "Loading scheduled tasks...";
            this.lblStatus.Text = initalStatus;
            this.lblStatus.Image = Resources.Loading;
            this.loadProgress.ControlItem.Style = ProgressBarStyle.Marquee;
            this.loadProgress.Visible = true;
            this.SetEditMode(false);

            ThreadPool.QueueUserWorkItem(
                o =>
                    {
                        try
                        {
                            AppUtil.NameCurrentThread(this.GetType().Name + "-Load");
                            List<ScheduleItem> tasks = this._scheduler.GetScheduledTasks();
                            this.SafeInvoke(
                                new MethodInvoker(
                                    () =>
                                        {
                                            var oldItems = this.lvSchedules.Items;

                                            // Compare the old item list with the new one
                                            for (int i = 0; i < oldItems.Count; i++)
                                            {
                                                var curLvItem = oldItems[i];
                                                var curItem = curLvItem.Tag as ScheduleItem;
                                                if (curItem != null && curItem.IsCreated)
                                                {
                                                    bool isRemoved = true;
                                                    for (int j = 0; j < tasks.Count; j++)
                                                    {
                                                        ScheduleItem t = tasks[j];
                                                        if (string.Equals(t.CreatedName, curItem.CreatedName, StringComparison.OrdinalIgnoreCase))
                                                        {
                                                            isRemoved = false;
                                                            curLvItem.Tag = t;
                                                            tasks.RemoveAt(j);
                                                            break;
                                                        }
                                                    }

                                                    if (isRemoved)
                                                    {
                                                        this.lvSchedules.Items.RemoveAt(i--);
                                                    }
                                                }
                                            }

                                            if (tasks.Count > 0)
                                            {
                                                foreach (var t in tasks)
                                                {
                                                    this.lvSchedules.Items.Add(new ListViewItem(t.Setting.TaskName) { Tag = t });
                                                }
                                            }

                                            this.lvSchedules.Focus();
                                            if (this.lvSchedules.Items.Count > 0)
                                            {
                                                this.lvSchedules.Items[0].Selected = true;
                                            }

                                            this.SetGridSize();
                                        }));
                        }
                        finally
                        {
                            this.SafeInvoke(
                                new Action(
                                    () =>
                                        {
                                            if (this.lblStatus.Text == initalStatus)
                                            {
                                                this.loadProgress.Visible = false;
                                                this.lblStatus.Text = "Idle";
                                                this.lblStatus.Image = null;
                                                this.btnRefresh.Enabled = true;
                                            }

                                            this.BindScheduleItemProperty();
                                            this.SetEditMode(true);
                                            this.UpdateButtons();
                                        }));
                        }
                    });
        }

        /// <summary>
        /// The update buttons.
        /// </summary>
        private void UpdateButtons()
        {
            bool canSave = false;
            foreach (ListViewItem lvItem in this.lvSchedules.Items)
            {
                var item = lvItem.Tag as ScheduleItem;
                if (item != null)
                {
                    if (item.IsCreated && !item.IsDirty)
                    {
                        continue;
                    }

                    canSave = true;
                    break;
                }
            }

            this.btnSave.Enabled = canSave;
        }

        /// <summary>
        /// The get execution param args.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        private string GetExecutionParamArgs()
        {
            string cmdLine = null;
            var obj = this.flightScannerProperties.SelectedObject as ExecutionParam;
            if (obj != null)
            {
                if (obj.OperationMode == OperationMode.Unspecified)
                {
                    throw new ArgumentException("Operation Mode was not specified");
                }

                cmdLine = obj.GenerateCommandLine();
            }

            return cmdLine;
        }

        /// <summary>
        /// The set grid size.
        /// </summary>
        private void SetGridSize()
        {
            this.flightScannerProperties.SetLabelColumnWidth(150);
        }

        /// <summary>
        /// The set edit mode.
        /// </summary>
        /// <param name="enabled">
        /// The enabled.
        /// </param>
        private void SetEditMode(bool enabled)
        {
            this.btnSave.Enabled = this.btnAdd.Enabled = this.btnDelete.Enabled = this.btnDeleteAll.Enabled = enabled;
        }

        /// <summary>
        /// The bind schedule item property.
        /// </summary>
        private void BindScheduleItemProperty()
        {
            if (this.lvSchedules.SelectedItems.Count == 1)
            {
                var selectedItem = this.lvSchedules.SelectedItems[0].Tag as ScheduleItem;
                this.flightScannerProperties.SelectedObject = selectedItem.Parameters;
                this.schedulerProperties.SelectedObject = selectedItem.Setting;
                this.SetGridSize();
            }
            else
            {
                this.flightScannerProperties.SelectedObject = null;
                this.schedulerProperties.SelectedObject = null;
            }
        }

        /// <summary>
        /// Mark the active item as dirty (being edited) and returns the active item
        /// </summary>
        private void MarkActiveItemDirty()
        {
            var activeItem = this.GetActiveItem();
            if (activeItem != null)
            {
                ScheduleItem selItem = activeItem.Value;
                selItem.IsDirty = true;
                if (selItem.IsCreated)
                {
                    activeItem.Key.ImageKey = "Edit";
                }
            }

            this.UpdateButtons();
        }

        /// <summary>
        /// The get active item.
        /// </summary>
        /// <returns>
        /// The <see cref="KeyValue"/>.
        /// </returns>
        private KeyValue<ListViewItem, ScheduleItem> GetActiveItem()
        {
            if (this.lvSchedules.SelectedItems.Count == 1)
            {
                var selLvItem = this.lvSchedules.SelectedItems[0];
                var selItem = selLvItem.Tag as ScheduleItem;
                if (selItem != null)
                {
                    return new KeyValue<ListViewItem, ScheduleItem>(selLvItem, selItem);
                }
            }

            return null;
        }

        /// <summary>
        /// The scheduler form_ shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SchedulerForm_Shown(object sender, EventArgs e)
        {
            this.lvSchedules.Columns[0].Width = this.lvSchedules.Width - 21;
            this.schedulerProperties.DescriptionAreaLineCount = 0;
            this.schedulerProperties.DescriptionAreaHeight = 0;
            this.Reload();
        }

        /// <summary>
        /// The scheduler form_ size changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SchedulerForm_SizeChanged(object sender, EventArgs e)
        {
            this.ResizeStatusStrip();
        }

        /// <summary>
        /// The lv schedules_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void lvSchedules_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.BindScheduleItemProperty();
        }

        /// <summary>
        /// The btn add_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string newItemName = Guid.NewGuid().ToString();
            var newLvItem = new ListViewItem(newItemName)
                                {
                                    ImageKey = "Add", 
                                    Tag =
                                        new ScheduleItem(
                                        new SchedulerSetting(newItemName), 
                                        this._defaultParam, 
                                        false, 
                                        true, 
                                        null)
                                };
            this.lvSchedules.Items.Add(newLvItem);
            this.lvSchedules.Select();
            if (this.lvSchedules.SelectedItems.Count == 1)
            {
                this.lvSchedules.SelectedItems[0].Selected = false;
            }

            if (this.lvSchedules.SelectedItems.Count == 0)
            {
                newLvItem.Selected = true;
            }
        }

        /// <summary>
        /// The btn delete_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (this.lvSchedules.SelectedItems.Count > 0)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Are you sure you want to delete the following scheduled tasks:" + Environment.NewLine);
                foreach (ListViewItem selItem in this.lvSchedules.SelectedItems)
                {
                    sb.AppendLine("\t" + selItem.Text);
                }

                if (MessageBox.Show(this, sb.ToString(), "Delete confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var firstSelIndex = this.lvSchedules.SelectedIndices[0];
                    foreach (ListViewItem selItem in this.lvSchedules.SelectedItems)
                    {
                        var scheduleItem = selItem.Tag as ScheduleItem;
                        if (scheduleItem != null && scheduleItem.IsCreated)
                        {
                            var taskName = scheduleItem.CreatedName;
                            this._scheduler.RemoveTask(taskName);
                        }

                        this.lvSchedules.Items.Remove(selItem);
                    }

                    if (--firstSelIndex >= 0)
                    {
                        this.lvSchedules.Items[firstSelIndex].Selected = true;
                    }
                    else if (this.lvSchedules.Items.Count > 0)
                    {
                        this.lvSchedules.Items[0].Selected = true;
                    }
                    else
                    {
                        this.lvSchedules_SelectedIndexChanged(null, null);
                    }

                    this.lvSchedules.Select();
                }
            }
        }

        /// <summary>
        /// The btn delete all_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            if (this.lvSchedules.Items.Count > 0)
            {
                if (MessageBox.Show(
                    this, 
                    "Are you sure you want to delete all scheduled tasks?", 
                    "Delete confirmation", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    for (int i = 0; i < this.lvSchedules.Items.Count; i++)
                    {
                        var item = this.lvSchedules.Items[i].Tag as ScheduleItem;
                        if (item != null && item.IsCreated)
                        {
                            string taskName = string.IsNullOrEmpty(item.CreatedName) ? item.Setting.TaskName : item.CreatedName;
                            this._scheduler.RemoveTask(taskName);
                        }

                        this.lvSchedules.Items.RemoveAt(i--);
                    }
                }
            }
        }

        /// <summary>
        /// The btn save_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <exception cref="ArgumentException">
        /// </exception>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this.lvSchedules.Items.Count < 1)
            {
                return;
            }

            var itemTags = new Dictionary<ListViewItem, ScheduleItem>();
            foreach (ListViewItem item in this.lvSchedules.Items)
            {
                var scheduleItem = item.Tag as ScheduleItem;
                if (scheduleItem != null)
                {
                    itemTags.Add(item, scheduleItem);
                }
            }

            if (itemTags.Count > 0)
            {
                string initialStatus = this.lblStatus.Text = "Saving scheduled tasks...";
                this.lblStatus.Image = Resources.Loading;
                this.btnSave.Enabled = false;
                this.loadProgress.Visible = true;
                var progressBar = this.loadProgress.ControlItem;
                progressBar.Value = 0;
                progressBar.Maximum = this.lvSchedules.Items.Count;
                progressBar.Style = itemTags.Count > 1 ? ProgressBarStyle.Blocks : ProgressBarStyle.Marquee;

                ThreadPool.QueueUserWorkItem(
                    o =>
                        {
                            AppUtil.NameCurrentThread(this.GetType().Name + "-Save");
                            try
                            {
                                foreach (var delItem in this._removingTasks)
                                {
                                    this._scheduler.RemoveTask(delItem);
                                }

                                this._removingTasks.Clear();

                                foreach (var pair in itemTags)
                                {
                                    bool success = true;
                                    string error = null;
                                    var task = pair.Value;
                                    try
                                    {
                                        if (task.Parameters.DepartureDate == DateTime.MinValue || task.Parameters.ReturnDate == DateTime.MinValue)
                                        {
                                            throw new ArgumentException("The departure and return date must be specified!");
                                        }

                                        this._scheduler.Schedule(task);
                                    }
                                    catch (Exception ex)
                                    {
                                        success = false;
                                        error = ex.Message;
                                    }

                                    this.SafeInvoke(
                                        new MethodInvoker(
                                            () =>
                                                {
                                                    pair.Key.ImageKey = success ? null : "Error";
                                                    if (!success)
                                                    {
                                                        MessageBox.Show(
                                                            this, 
                                                            "Failed to create task with ID: " + task.Setting.TaskName + Environment.NewLine
                                                            + Environment.NewLine + error, 
                                                            "Failed to create scheduled task", 
                                                            MessageBoxButtons.OK, 
                                                            MessageBoxIcon.Error);
                                                    }

                                                    progressBar.Value++;
                                                }));
                                }
                            }
                            finally
                            {
                                this.SafeInvoke(
                                    new MethodInvoker(
                                        () =>
                                            {
                                                if (this.lblStatus.Text == initialStatus)
                                                {
                                                    this.lblStatus.Text = "Idle";
                                                    this.lblStatus.Image = null;
                                                    progressBar.Value = progressBar.Maximum = 0;
                                                    this.loadProgress.Visible = false;
                                                }

                                                this.btnSave.Enabled = true;
                                                this.lvSchedules.Refresh();
                                                this.UpdateButtons();
                                            }));
                            }
                        });
            }
        }

        /// <summary>
        /// The btn exit_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// The scheduler properties_ property value changed.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void schedulerProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            this.MarkActiveItemDirty();

            // In case of renaming the task
            if (e.ChangedItem.Label == "TaskName")
            {
                var newTaskName = e.ChangedItem.Value.ToString();
                var oldTaskName = e.OldValue.ToString();
                if (this.lvSchedules.SelectedItems.Count > 0)
                {
                    foreach (ListViewItem item in this.lvSchedules.Items)
                    {
                        if (item.Text == newTaskName)
                        {
                            MessageBox.Show(
                                string.Format("Task name [{0}] already existed!", item.Text), 
                                "Error", 
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Exclamation);
                            var obj = this.schedulerProperties.SelectedObject as SchedulerSetting;
                            obj.TaskName = oldTaskName;
                            return;
                        }
                    }

                    var activeItem = this.GetActiveItem();
                    if (activeItem != null)
                    {
                        ScheduleItem selTask = activeItem.Value;
                        if (selTask != null && selTask.IsCreated && !selTask.IsDirty)
                        {
                            this._removingTasks.Add(oldTaskName);
                        }
                    }

                    this.lvSchedules.SelectedItems[0].Text = newTaskName;
                }
            }
        }

        /// <summary>
        /// The flight scanner properties_ property value changed.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void flightScannerProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            this.MarkActiveItemDirty();
        }

        /// <summary>
        /// The flight scanner properties_ selected objects changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void flightScannerProperties_SelectedObjectsChanged(object sender, EventArgs e)
        {
            var selObject = this.flightScannerProperties.SelectedObject as ExecutionParam;
            this.txtDeparture.Enabled = this.txtDestination.Enabled = selObject != null;
            if (selObject == null)
            {
                this.txtDeparture.SelectedAirport = this.txtDestination.SelectedAirport = null;
            }
            else
            {
                this._bindingObject = true;
                this.txtDeparture.SelectedAirport = selObject.Departure;
                this.txtDestination.SelectedAirport = selObject.Destination;
                this._bindingObject = false;
            }
        }

        /// <summary>
        /// The scheduler form_ resize.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SchedulerForm_Resize(object sender, EventArgs e)
        {
            this.SetGridSize();
        }

        /// <summary>
        /// The scanner setting context menu_ opening.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void scannerSettingContextMenu_Opening(object sender, CancelEventArgs e)
        {
            bool selected = this.flightScannerProperties.SelectedObject != null;
            foreach (ToolStripItem mnuItem in this.scannerSettingContextMenu.Items)
            {
                mnuItem.Visible = selected;
            }
        }

        /// <summary>
        /// The btn close_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        /// <summary>
        /// The mnu copy params to clipboard_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void mnuCopyParamsToClipboard_Click(object sender, EventArgs e)
        {
            try
            {
                string args = this.GetExecutionParamArgs();
                if (!string.IsNullOrEmpty(args))
                {
                    Clipboard.SetText(args);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Invalid setting", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// The mnu execute_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void mnuExecute_Click(object sender, EventArgs e)
        {
            try
            {
                string args = this.GetExecutionParamArgs();
                if (!string.IsNullOrEmpty(args))
                {
                    Process.Start(SchedulerManager.CurrentExe, args);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Execution Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// The task context menu strip_ opening.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void taskContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            this.mnuDeleteTask.Visible = !(this.lvSchedules.SelectedIndices.Count < 1);
            this.mnuSeparator.Visible = this.mnuDeleteAll.Visible = !(this.lvSchedules.Items.Count < 1);
        }

        /// <summary>
        /// The btn refresh_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            this.Reload();
        }

        /// <summary>
        /// The location_ selected item changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Location_SelectedItemChanged(object sender, EventArgs e)
        {
            if (this._bindingObject)
            {
                return;
            }

            var selParam = this.flightScannerProperties.SelectedObject as ExecutionParam;
            if (selParam != null)
            {
                if (sender == this.txtDeparture)
                {
                    selParam.Departure = this.txtDeparture.SelectedAirport;
                }
                else if (sender == this.txtDestination)
                {
                    selParam.Destination = this.txtDestination.SelectedAirport;
                }
            }

            this.MarkActiveItemDirty();
        }

        /// <summary>
        /// The load progress_ visible changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void LoadProgress_VisibleChanged(object sender, EventArgs e)
        {
            this.loadProgress.ControlItem.ShowInTaskbar = this.loadProgress.Visible;
            this.ResizeStatusStrip();
        }

        /// <summary>
        /// The lbl status_ text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void lblStatus_TextChanged(object sender, EventArgs e)
        {
            this.ResizeStatusStrip();
        }

        /// <summary>
        /// The lv schedules_ items added.
        /// </summary>
        /// <param name="listView">
        /// The list view.
        /// </param>
        /// <param name="items">
        /// The items.
        /// </param>
        private void lvSchedules_ItemsAdded(EnhancedListView listView, List<ListViewItem> items)
        {
            this.UpdateButtons();
        }

        /// <summary>
        /// The lv schedules_ item removed.
        /// </summary>
        /// <param name="listView">
        /// The list view.
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>
        private void lvSchedules_ItemRemoved(EnhancedListView listView, ListViewItem item)
        {
            this.UpdateButtons();
        }
    }

    /// <summary>
    /// The property grid extension.
    /// </summary>
    public static class PropertyGridExtension
    {
        /// <summary>
        /// The set label column width.
        /// </summary>
        /// <param name="grid">
        /// The grid.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        public static void SetLabelColumnWidth(this PropertyGrid grid, int width)
        {
            var view = (Control)grid.GetType().GetField("gridView", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(grid);
            FieldInfo fi = view.GetType().GetField("labelWidth", BindingFlags.Instance | BindingFlags.NonPublic);
            fi.SetValue(view, width);
            view.Invalidate();
        }
    }
}