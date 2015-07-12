namespace SkyDean.FareLiz.WinForm.Components.Controls.Custom
{
    using System;
    using System.Drawing;
    using System.Threading;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core.Presentation;
    using SkyDean.FareLiz.WinForm.Components.Dialog;
    using SkyDean.FareLiz.WinForm.Components.Utils;

    using Timer = System.Windows.Forms.Timer;

    public delegate void OnFormatText(RichTextBox target);

    /// <summary>
    /// List of the different popup animation status
    /// </summary>
    public enum NotiferState
    {
        Hidden = 0,
        Appearing = 1,
        Visible = 2,
        Disappearing = 3
    }

    /// <summary>
    /// TaskbarNotifier allows to display styled popups
    /// </summary>
    public partial class TaskbarNotifierBase : SmartForm
    {
        private readonly Timer _timer = new Timer();
        private NotiferState _state = NotiferState.Hidden;
        private int _stayDuration;
        private bool _bIsMouseOverPopup;

        private double _opacityStep = 0.1;
        private const int TIMER_STEP = 50;

        public int FadeTime
        {
            get { return (int)(TIMER_STEP / this._opacityStep); }
            set { this._opacityStep = TIMER_STEP / value; }
        }

        protected virtual void InitializeContentPanel(Panel contentPanel) { }
        protected virtual Size MeasureContentSize(int maxWidth, int maxHeight)
        {
            return Size.Empty;
        }

        /// <summary>
        /// The Constructor for TaskbarNotifier
        /// </summary>
        protected TaskbarNotifierBase()
        {
            this.InitializeComponent();
            this.InitializeContentPanel(this.pnlMiddle);
            this.CreateHandle(); // Allocate the form on the thread where it was created
            this.AssignMouseHandler(this);
            this._timer.Tick += this.OnTimer;
        }

        public void WaitTillHidden()
        {
            while (this.Visible)
                Thread.Sleep(500);
        }

        private void AssignMouseHandler(Control targetControl)
        {
            targetControl.MouseEnter += this.OnMouseEnter;
            targetControl.MouseLeave += this.OnMouseLeave;
            foreach (Control control in targetControl.Controls)
            {                
                this.AssignMouseHandler(control);
            }
        }

        private void OnTimer(Object obj, EventArgs ea)
        {
            switch (this._state)
            {
                case NotiferState.Appearing:
                    if (this.Opacity < 1)
                        this.Opacity += this._opacityStep;
                    else
                    {
                        this._timer.Stop();
                        this._timer.Interval = this._stayDuration;
                        this._state = NotiferState.Visible;
                        this._timer.Start();
                    }
                    break;

                case NotiferState.Visible:
                    this._timer.Stop();
                    this._timer.Interval = TIMER_STEP;
                    if (!this._bIsMouseOverPopup)
                    {
                        this._state = NotiferState.Disappearing;
                    }
                    this._timer.Start();
                    break;

                case NotiferState.Disappearing:
                    if (this._bIsMouseOverPopup)
                    {
                        this._state = NotiferState.Appearing;
                    }
                    else
                    {
                        if (this.Opacity > 0)
                            this.Opacity -= this._opacityStep;
                        else
                            this.Hide();
                    }
                    break;

                case NotiferState.Hidden:
                    this._timer.Enabled = false;
                    break;
            }

        }

        /// <summary>
        /// Displays the popup for a certain amount of time
        /// </summary>
        /// <param name="timeToStay">Time to stay in milliseconds</param>
        /// <param name="type">Notification type</param>
        protected void Display(int timeToStay, NotificationType type)
        {
            this._stayDuration = timeToStay;
            this.SetWindowMetrics();
            this.ApplyTheme(type);
            this._timer.Enabled = true;
            this._timer.Interval = 100;
            this.pnlMiddle.Invalidate();

            switch (this._state)
            {
                case NotiferState.Hidden:
                case NotiferState.Disappearing:
                    this._timer.Stop();
                    this._state = NotiferState.Appearing;
                    NativeMethods.ShowWindow(this.Handle, 4); // We Show the popup without stealing focus
                    this._timer.Interval = TIMER_STEP;
                    this._timer.Start();
                    break;

                case NotiferState.Appearing:
                    if (!this._timer.Enabled)
                    {
                        this._timer.Interval = TIMER_STEP;
                        this._timer.Start();
                    }
                    break;

                case NotiferState.Visible:
                    this._timer.Stop();
                    this._timer.Interval = this._stayDuration;
                    this._timer.Start();
                    break;
            }
        }

        protected void SetTitle(string title)
        {
            this.lblTitle.Text = title;
        }

        private void SetWindowMetrics()
        {
            int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;

            int maxWidth = screenWidth / 3;
            int maxHeight = screenHeight / 3;
            Size szContent = this.MeasureContentSize(maxWidth, maxHeight);

            Size szTitle = TextRenderer.MeasureText(this.lblTitle.Text, this.lblTitle.Font, szContent, TextFormatFlags.SingleLine);
            int height = szContent.Height + this.pnlTop.Height + this.pnlBottom.Height + this.pnlMiddle.Padding.Top + this.pnlMiddle.Padding.Bottom + this.Padding.Top + this.Padding.Bottom,
                contentWidth = szContent.Width + this.pnlMiddle.Padding.Left + this.pnlMiddle.Padding.Right,
                titleWidth = szTitle.Width + this.pnlTopRight.Width + this.pnlTopLeft.Width + this.lblTitle.Padding.Left + this.lblTitle.Padding.Right,
                width = ((contentWidth > titleWidth) ? contentWidth : titleWidth) + this.Padding.Left + this.Padding.Right;

            if (height > maxHeight)
                height = maxHeight;
            if (width > maxWidth)
                width = maxWidth;

            int left = screenWidth - width,
                top = screenHeight - height;

            NativeMethods.SetWindowPos(this.Handle.ToInt32(), -1, left, top, width, height, 0x0010);
        }

        /// <summary>
        /// Hides the popup
        /// </summary>
        /// <returns>Nothing</returns>
        public new void Hide()
        {
            if (this._state != NotiferState.Hidden)
            {
                this._timer.Stop();
                this._state = NotiferState.Hidden;
                this.imgClose.Visible = false;
                base.Hide();
            }
        }

        private void OnMouseEnter(object sender, EventArgs ea)
        {
            this._bIsMouseOverPopup = this.imgClose.Visible = true;
            this.Opacity = 1;
            this._state = NotiferState.Visible;
        }

        private void OnMouseLeave(object sender, EventArgs ea)
        {
            this.DetectMouseLeave();
        }

        private void DetectMouseLeave()
        {
            if (Cursor.Position.X < this.Location.X
                || Cursor.Position.Y < this.Location.Y
                || Cursor.Position.X > this.Location.X + this.Width
                || Cursor.Position.Y > this.Location.Y + this.Height)
            {
                this._bIsMouseOverPopup = this.imgClose.Visible = false;
                this._timer.Stop();
                this._timer.Interval = this._stayDuration;
                this._timer.Start();
            }
        }

        private void imgClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
