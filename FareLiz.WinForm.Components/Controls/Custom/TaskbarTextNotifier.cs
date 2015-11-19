namespace SkyDean.FareLiz.WinForm.Components.Controls.Custom
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core.Presentation;

    /// <summary>TaskbarNotifier allows to display styled popups</summary>
    public sealed partial class TaskbarTextNotifier : TaskbarNotifierBase, INotifier
    {
        /// <summary>
        /// Gets or sets the content font.
        /// </summary>
        public Font ContentFont
        {
            get
            {
                return this.txtContent.Font;
            }

            set
            {
                this.txtContent.Font = value;
            }
        }

        /// <summary>
        /// Displays the popup for a certain amount of time
        /// </summary>
        /// <param name="title">
        /// Popup title
        /// </param>
        /// <param name="content">
        /// Popup content
        /// </param>
        /// <param name="timeToStay">
        /// Time to stay in milliseconds
        /// </param>
        /// <param name="type">
        /// Notification type
        /// </param>
        /// <param name="isRTF">
        /// Is the content Rich Text Format
        /// </param>
        public void Show(string title, string content, int timeToStay, NotificationType type, bool isRTF)
        {
            if (this.InvokeRequired)
            {
                this.SafeInvoke(new Action(() => this.Show(title, content, timeToStay, type, isRTF)));
            }
            else
            {
                this.SetTitle(title);
                if (isRTF)
                {
                    this.txtContent.Rtf = content;
                }
                else
                {
                    this.txtContent.Text = content;
                }

                this.Display(timeToStay, type);
            }
        }

        /// <summary>
        /// Displays the popup for a certain amount of time
        /// </summary>
        /// <param name="title">
        /// Popup title
        /// </param>
        /// <param name="content">
        /// Popup content
        /// </param>
        /// <param name="timeToStay">
        /// Time to stay in milliseconds
        /// </param>
        /// <param name="type">
        /// Notification type
        /// </param>
        public void Show(string title, string content, int timeToStay, NotificationType type)
        {
            this.Show(title, content, timeToStay, type, false);
        }

        /// <summary>
        /// The initialize content panel.
        /// </summary>
        /// <param name="contentPanel">
        /// The content panel.
        /// </param>
        protected override void InitializeContentPanel(Panel contentPanel)
        {
            this.InitializeComponent();
            contentPanel.Controls.Add(this.txtContent);
        }

        /// <summary>
        /// The measure content size.
        /// </summary>
        /// <param name="maxWidth">
        /// The max width.
        /// </param>
        /// <param name="maxHeight">
        /// The max height.
        /// </param>
        /// <returns>
        /// The <see cref="Size"/>.
        /// </returns>
        protected override Size MeasureContentSize(int maxWidth, int maxHeight)
        {
            var result = new Size(maxWidth, maxHeight);
            result = TextRenderer.MeasureText(this.txtContent.Text, this.txtContent.Font, result, TextFormatFlags.LeftAndRightPadding);
            bool ver = result.Height > maxHeight;
            bool hor = result.Width > maxWidth;

            RichTextBoxScrollBars scroll = (hor && ver)
                                               ? RichTextBoxScrollBars.Both
                                               : (hor
                                                      ? RichTextBoxScrollBars.Horizontal
                                                      : (ver ? RichTextBoxScrollBars.Vertical : RichTextBoxScrollBars.None));

            this.txtContent.ScrollBars = scroll;

            return result;
        }
    }
}