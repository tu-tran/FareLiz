// Modified from Windows7ProgressBar v1.0, created by Wyatt O'Day
// Visit: http://wyday.com/windows-7-progress-bar/
namespace SkyDean.FareLiz.WinForm.Components.Utils
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// The windows 7 progress bar.
    /// </summary>
    [ToolboxBitmap(typeof(ProgressBar))]
    public class Windows7ProgressBar : ProgressBar
    {
        /// <summary>
        /// The m_ state.
        /// </summary>
        private ProgressBarState m_State = ProgressBarState.Normal;

        /// <summary>
        /// The owner form.
        /// </summary>
        private ContainerControl ownerForm;

        /// <summary>
        /// The show in taskbar.
        /// </summary>
        private bool showInTaskbar = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="Windows7ProgressBar"/> class.
        /// </summary>
        public Windows7ProgressBar()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Windows7ProgressBar"/> class.
        /// </summary>
        /// <param name="parentControl">
        /// The parent control.
        /// </param>
        public Windows7ProgressBar(ContainerControl parentControl)
        {
            this.ContainerControl = parentControl;
        }

        /// <summary>
        /// Gets or sets the container control.
        /// </summary>
        public ContainerControl ContainerControl
        {
            get
            {
                return this.ownerForm;
            }

            set
            {
                this.ownerForm = value;

                if (!this.ownerForm.Visible)
                {
                    ((Form)this.ownerForm).Shown += this.Windows7ProgressBar_Shown;
                }
            }
        }

        /// <summary>
        /// Sets the site.
        /// </summary>
        public override ISite Site
        {
            set
            {
                // Runs at design time, ensures designer initializes ContainerControl
                base.Site = value;
                if (value == null)
                {
                    return;
                }

                IDesignerHost service = value.GetService(typeof(IDesignerHost)) as IDesignerHost;
                if (service == null)
                {
                    return;
                }

                IComponent rootComponent = service.RootComponent;

                this.ContainerControl = rootComponent as ContainerControl;
            }
        }

        /// <summary>Show progress in taskbar</summary>
        [DefaultValue(true)]
        public bool ShowInTaskbar
        {
            get
            {
                return this.showInTaskbar;
            }

            set
            {
                if (this.showInTaskbar != value)
                {
                    this.showInTaskbar = value;

                    // send signal to the taskbar.
                    if (this.ownerForm != null)
                    {
                        if (this.Style != ProgressBarStyle.Marquee)
                        {
                            this.SetValueInTB();
                        }

                        this.SetStateInTB();
                    }
                }
            }
        }

        /// <summary>Gets or sets the current position of the progress bar.</summary>
        /// <returns>The position within the range of the progress bar. The default is 0.</returns>
        public new int Value
        {
            get
            {
                return base.Value;
            }

            set
            {
                if (value < this.Minimum || value > this.Maximum)
                {
                    return;
                }

                base.Value = value;

                // send signal to the taskbar.
                this.SetValueInTB();
            }
        }

        /// <summary>Gets or sets the manner in which progress should be indicated on the progress bar.</summary>
        /// <returns>One of the ProgressBarStyle values. The default is ProgressBarStyle.Blocks</returns>
        public new ProgressBarStyle Style
        {
            get
            {
                return base.Style;
            }

            set
            {
                base.Style = value;

                // set the style of the progress bar
                if (this.showInTaskbar && this.ownerForm != null)
                {
                    this.SetStateInTB();
                }
            }
        }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        [DefaultValue(ProgressBarState.Normal)]
        public ProgressBarState State
        {
            get
            {
                return this.m_State;
            }

            set
            {
                this.m_State = value;

                bool wasMarquee = this.Style == ProgressBarStyle.Marquee;

                if (wasMarquee)
                {
                    // sets the RequestState to normal (and implicity calls SetStateInTB() )
                    this.Style = ProgressBarStyle.Blocks;
                }

                // set the progress bar RequestState (Normal, Error, Paused)
                NativeMethods.SendMessage(this.Handle, 0x410, (int)value, 0);

                if (wasMarquee)
                {
                    // the Taskbar PB value needs to be reset
                    this.SetValueInTB();
                }
                else
                {
                    // there wasn't a marquee, thus we need to update the taskbar
                    this.SetStateInTB();
                }
            }
        }

        /// <summary>
        /// The windows 7 progress bar_ shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Windows7ProgressBar_Shown(object sender, EventArgs e)
        {
            if (this.ShowInTaskbar)
            {
                if (this.Style != ProgressBarStyle.Marquee)
                {
                    this.SetValueInTB();
                }

                this.SetStateInTB();
            }

            ((Form)this.ownerForm).Shown -= this.Windows7ProgressBar_Shown;
        }

        /// <summary>
        /// Advances the current position of the progress bar by the specified amount.
        /// </summary>
        /// <param name="value">
        /// The amount by which to increment the progress bar's current position.
        /// </param>
        public new void Increment(int value)
        {
            base.Increment(value);

            // send signal to the taskbar.
            this.SetValueInTB();
        }

        /// <summary>Advances the current position of the progress bar by the amount of the System.Windows.Forms.ProgressBar.Step property.</summary>
        public new void PerformStep()
        {
            base.PerformStep();

            // send signal to the taskbar.
            this.SetValueInTB();
        }

        /// <summary>
        /// The set value in tb.
        /// </summary>
        private void SetValueInTB()
        {
            if (this.showInTaskbar)
            {
                ulong maximum = (ulong)(this.Maximum - this.Minimum);
                ulong progress = (ulong)(this.Value - this.Minimum);

                Windows7Taskbar.SetProgressValue(this.ownerForm.Handle, progress, maximum);
            }
        }

        /// <summary>
        /// The set state in tb.
        /// </summary>
        private void SetStateInTB()
        {
            if (this.ownerForm == null)
            {
                return;
            }

            ThumbnailProgressState thmState = ThumbnailProgressState.Normal;

            if (!this.showInTaskbar)
            {
                thmState = ThumbnailProgressState.NoProgress;
            }
            else if (this.Style == ProgressBarStyle.Marquee)
            {
                thmState = ThumbnailProgressState.Indeterminate;
            }
            else if (this.m_State == ProgressBarState.Error)
            {
                thmState = ThumbnailProgressState.Error;
            }
            else if (this.m_State == ProgressBarState.Pause)
            {
                thmState = ThumbnailProgressState.Paused;
            }

            Windows7Taskbar.SetProgressState(this.ownerForm.Handle, thmState);
        }
    }

    /// <summary>
    /// The progress bar state.
    /// </summary>
    public enum ProgressBarState
    {
        /// <summary>Indicates normal progress</summary>
        Normal = 1, 

        /// <summary>Indicates an error in the progress</summary>
        Error = 2, 

        /// <summary>Indicates paused progress</summary>
        Pause = 3
    }
}