namespace SkyDean.FareLiz.WinForm.Components.Dialog
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Threading;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core.Presentation;
    using SkyDean.FareLiz.WinForm.Components.Controls;
    using SkyDean.FareLiz.WinForm.Components.Utils;

    /// <summary>Showing progress dialog and running an asynchronous task</summary>
    public partial class ProgressDialog : SmartForm, IProgressCallback
    {
        /// <summary>The visible dialogs.</summary>
        private static readonly List<ProgressDialog> VisibleDialogs = new List<ProgressDialog>();

        /// <summary>The _lock obj.</summary>
        private static readonly object _lockObj = new object();

        /// <summary>The abort event.</summary>
        private readonly ManualResetEvent abortEvent = new ManualResetEvent(false);

        /// <summary>Required designer variable.</summary>
        private readonly Container components = null;

        /// <summary>The init event.</summary>
        private readonly ManualResetEvent initEvent = new ManualResetEvent(false);

        /// <summary>The btn cancel.</summary>
        private Button btnCancel;

        /// <summary>The img progress.</summary>
        private PictureBox imgProgress;

        /// <summary>The lbl text.</summary>
        private Label lblText;

        /// <summary>The progress bar.</summary>
        private Windows7ProgressBar progressBar;

        /// <summary>The requires close.</summary>
        private bool requiresClose = true;

        /// <summary>The title root.</summary>
        private string titleRoot = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressDialog"/> class.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="progressBarStyle">
        /// The progress bar style.
        /// </param>
        /// <param name="alwaysOnTop">
        /// The always on top.
        /// </param>
        /// <param name="visible">
        /// The visibility.
        /// </param>
        public ProgressDialog(string title, string text, ProgressStyle progressBarStyle = ProgressStyle.Blocks, bool alwaysOnTop = true, bool visible = true)
            : this(null, title, text, progressBarStyle, alwaysOnTop, visible)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressDialog"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="progressBarStyle">
        /// The progress bar style.
        /// </param>
        /// <param name="alwaysOnTop">
        /// The always on top.
        /// </param>
        /// <param name="visible">
        /// The visibility.
        /// </param>
        public ProgressDialog(Form parent, string title, string text, ProgressStyle progressBarStyle, bool alwaysOnTop, bool visible)
            : this(visible)
        {
            this.Owner = parent;
            this.Title = title;
            this.lblText.Text = text;
            this.progressBar.Style = (ProgressBarStyle)progressBarStyle;
            this.TopMost = alwaysOnTop;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressDialog"/> class.
        /// </summary>
        /// <param name="visible">
        /// The visible.
        /// </param>
        public ProgressDialog(bool visible)
            : this()
        {
            if (!visible)
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }

        /// <summary>Initializes a new instance of the <see cref="ProgressDialog" /> class.</summary>
        public ProgressDialog()
        {
            lock (_lockObj)
            {
                this.InitializeComponent();
            }
        }

        /// <summary>
        /// The progress dialog_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ProgressDialog_Load(object sender, EventArgs e)
        {
            var totalCount = VisibleDialogs.Count;
            if (totalCount > 0)
            {
                for (var i = totalCount - 1; i >= 0; i--)
                {
                    // Determine the location based on existing dialog window    
                    var lastDialog = VisibleDialogs[i];
                    if (!lastDialog.Visible)
                    {
                        continue;
                    }

                    var lastBounds = lastDialog.Bounds;
                    var screenBounds = Rectangle.Empty;
                    lastDialog.Invoke(new Action(() => screenBounds = Screen.FromControl(lastDialog).WorkingArea));

                    if (screenBounds.Contains(lastBounds))
                    {
                        // If the previous dialog fit into the screen
                        int y;
                        if (lastBounds.Bottom + this.Height <= screenBounds.Bottom)
                        {
                            // Align below
                            y = lastBounds.Bottom;
                        }
                        else if (lastBounds.Top - this.Height > screenBounds.Top)
                        {
                            // Align above
                            y = lastBounds.Top - this.Height;
                        }
                        else
                        {
                            y = lastBounds.Y;
                        }

                        int? x = null;
                        if (y == lastBounds.Y)
                        {
                            // If we are on the same horizontal line: Align left or right
                            if (lastBounds.Right + this.Width <= screenBounds.Right)
                            {
                                // Align to the right
                                x = lastBounds.Right;
                            }
                            else if (lastBounds.Left - this.Width >= screenBounds.Left)
                            {
                                // Align to the left
                                x = lastBounds.Left - this.Width;
                            }
                        }
                        else
                        {
                            x = lastBounds.X;
                        }

                        if (x.HasValue)
                        {
                            this.StartPosition = FormStartPosition.Manual;
                            this.Location = new Point(x.Value, y);
                            break;
                        }
                    }
                }
            }

            if (!VisibleDialogs.Contains(this))
            {
                VisibleDialogs.Add(this);
            }
        }

        /// <summary>
        /// The progress dialog_ form closing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ProgressDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!VisibleDialogs.Contains(this))
            {
                VisibleDialogs.Remove(this);
            }
        }

        #region Implementation of IProgressCallback

        /// <summary>
        /// Call this method from the worker thread to initialize the progress meter.
        /// </summary>
        /// <param name="minimum">
        /// The minimum value in the progress range (e.g. 0)
        /// </param>
        /// <param name="maximum">
        /// The maximum value in the progress range (e.g. 100)
        /// </param>
        public void Begin(int minimum, int maximum)
        {
            this.initEvent.WaitOne();
            this.SafeInvoke(new RangeInvoker(this.DoBegin), minimum, maximum);
        }

        /// <summary>Call this method from the worker thread to initialize the progress callback, without setting the range</summary>
        public void Begin()
        {
            this.initEvent.WaitOne();
            this.SafeInvoke(new MethodInvoker(this.DoBegin));
        }

        /// <summary>
        /// Call this method from the worker thread to reset the range in the progress callback
        /// </summary>
        /// <param name="minimum">
        /// The minimum value in the progress range (e.g. 0)
        /// </param>
        /// <param name="maximum">
        /// The maximum value in the progress range (e.g. 100)
        /// </param>
        /// <remarks>
        /// You must have called one of the Begin() methods prior to this call.
        /// </remarks>
        public void SetRange(int minimum, int maximum)
        {
            this.initEvent.WaitOne();
            this.SafeInvoke(new RangeInvoker(this.DoSetRange), minimum, maximum);
        }

        /// <summary>
        /// Call this method from the worker thread to append the progress text.
        /// </summary>
        /// <param name="text">
        /// The progress text to display
        /// </param>
        public void AppendText(string text)
        {
            this.SafeInvoke(new SetTextInvoker(this.DoAppendText), text);
        }

        /// <summary>
        /// Call this method from the worker thread to increase the progress counter by a specified value.
        /// </summary>
        /// <param name="val">
        /// The amount by which to increment the progress indicator
        /// </param>
        public void Increment(int val)
        {
            this.SafeInvoke(new IncrementInvoker(this.DoIncrement), val);
        }

        /// <summary>
        /// Call this method from the worker thread to step the progress meter to a particular value.
        /// </summary>
        /// <param name="val">
        /// </param>
        public void StepTo(int val)
        {
            this.SafeInvoke(new StepToInvoker(this.DoStepTo), val);
        }

        /// <summary>If this property is true, then you should abort work</summary>
        public bool IsAborting
        {
            get
            {
                return this.IsUnusable() ? true : this.abortEvent.WaitOne(0, false);
            }
        }

        /// <summary>Call this method from the worker thread to finalize the progress meter</summary>
        public void End()
        {
            if (this.requiresClose)
            {
                this.SafeInvoke(new MethodInvoker(this.DoEnd));
            }
        }

        /// <summary>
        /// The inform.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        public void Inform(object sender, string text, string title, NotificationType type)
        {
            var owner = sender as Control;
            if (owner != null && owner.InvokeRequired)
            {
                owner.SafeInvoke(new Action(() => this.Inform(sender, text, title, type)));
                return;
            }

            var icon = MessageBoxIcon.None;
            if (type == NotificationType.Info)
            {
                icon = MessageBoxIcon.Information;
            }
            else if (type == NotificationType.Warning)
            {
                icon = MessageBoxIcon.Warning;
            }
            else if (type == NotificationType.Error)
            {
                icon = MessageBoxIcon.Error;
            }
            else if (type == NotificationType.Exclamation)
            {
                icon = MessageBoxIcon.Exclamation;
            }

            MessageBox.Show(owner, text, title, MessageBoxButtons.OK, icon);
        }

        /// <summary>
        /// The confirm.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <returns>
        /// The <see cref="ConfirmationType"/>.
        /// </returns>
        public ConfirmationType Confirm(object sender, string text, string title)
        {
            var owner = sender as Control;
            if (owner != null && owner.InvokeRequired)
            {
                return (ConfirmationType)owner.SafeInvoke(new Func<ConfirmationType>(() => this.Confirm(sender, text, title)));
            }

            var result = MessageBox.Show(owner, text, title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            switch (result)
            {
                case DialogResult.Yes:
                    return ConfirmationType.Yes;
                case DialogResult.No:
                    return ConfirmationType.No;
                default:
                    return ConfirmationType.Cancel;
            }
        }

        /// <summary>Gets or sets the style.</summary>
        public ProgressStyle Style
        {
            get
            {
                return
                    (ProgressStyle)
                    this.SafeInvoke(
                        new Func<ProgressStyle>(
                        () => (this.progressBar.Style == ProgressBarStyle.Marquee ? ProgressStyle.Marquee : ProgressStyle.Continuous)));
            }

            set
            {
                this.SafeInvoke(
                    new Action(() => this.progressBar.Style = value == ProgressStyle.Marquee ? ProgressBarStyle.Marquee : ProgressBarStyle.Continuous));
            }
        }

        /// <summary>Get or set progress text.</summary>
        public override string Text
        {
            get
            {
                return this.SafeInvoke(new Func<string>(() => { return this.lblText.Text; })) as string;
            }

            set
            {
                this.SafeInvoke((MethodInvoker)delegate { this.lblText.Text = value; });
            }
        }

        /// <summary>Get or set the progress dialog title</summary>
        public string Title
        {
            get
            {
                return this.SafeInvoke(new Func<string>(() => { return base.Text; })) as string;
            }

            set
            {
                this.SafeInvoke((MethodInvoker)delegate { base.Text = this.titleRoot = value; });
            }
        }

        /// <summary>
        /// Shows the progress.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public void Show(object parent)
        {
            var owner = parent is IWin32Window ? (IWin32Window)parent : null;
            this.ShowDialog(owner);
        }

        #endregion

        #region Implementation members invoked on the owner thread

        /// <summary>
        /// The do append text.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        private void DoAppendText(string text)
        {
            this.lblText.Text += text;
        }

        /// <summary>
        /// The do increment.
        /// </summary>
        /// <param name="val">
        /// The val.
        /// </param>
        private void DoIncrement(int val)
        {
            this.progressBar.Increment(val);
            this.UpdateStatusText();
        }

        /// <summary>
        /// The do step to.
        /// </summary>
        /// <param name="val">
        /// The val.
        /// </param>
        private void DoStepTo(int val)
        {
            this.progressBar.Value = val;
            this.UpdateStatusText();
        }

        /// <summary>
        /// The do begin.
        /// </summary>
        /// <param name="minimum">
        /// The minimum.
        /// </param>
        /// <param name="maximum">
        /// The maximum.
        /// </param>
        private void DoBegin(int minimum, int maximum)
        {
            this.DoSetRange(minimum, maximum);
            this.DoBegin();
        }

        /// <summary>The do begin.</summary>
        private void DoBegin()
        {
            this.btnCancel.Enabled = true;
            this.ControlBox = true;
        }

        /// <summary>
        /// The do set range.
        /// </summary>
        /// <param name="minimum">
        /// The minimum.
        /// </param>
        /// <param name="maximum">
        /// The maximum.
        /// </param>
        private void DoSetRange(int minimum, int maximum)
        {
            this.progressBar.Minimum = minimum;
            this.progressBar.Maximum = maximum;
            this.progressBar.Value = minimum;
            this.titleRoot = this.Text;
        }

        /// <summary>The do end.</summary>
        private void DoEnd()
        {
            this.Close();
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Handles the form load, and sets an event to ensure that intialization is synchronized with the appearance of the form.
        /// </summary>
        /// <param name="e">
        /// </param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.ControlBox = false;
            this.initEvent.Set();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.components != null)
                {
                    this.components.Dispose();
                }
            }

            if (this.initEvent != null)
            {
                this.initEvent.Close();
            }

            if (this.abortEvent != null)
            {
                this.abortEvent.Close();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Handler for 'Close' clicking
        /// </summary>
        /// <param name="e">
        /// </param>
        protected override void OnClosing(CancelEventArgs e)
        {
            this.requiresClose = false;
            this.AbortWork();
            base.OnClosing(e);
        }

        #endregion

        #region Implementation Utilities

        /// <summary>Utility function that formats and updates the title bar text</summary>
        private void UpdateStatusText()
        {
            this.Text = this.titleRoot
                        + string.Format(
                            CultureInfo.InvariantCulture,
                            " - {0}% complete",
                            (this.progressBar.Value * 100) / (this.progressBar.Maximum - this.progressBar.Minimum));
        }

        /// <summary>Utility function to terminate the thread</summary>
        private void AbortWork()
        {
            this.abortEvent.Set();
        }

        #endregion
    }
}