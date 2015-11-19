namespace SkyDean.FareLiz.WinForm.Components.Controls.Custom
{
    using System.Drawing;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core.Presentation;
    using SkyDean.FareLiz.WinForm.Components.Properties;
    using SkyDean.FareLiz.WinForm.Components.Utils;

    /// <summary>
    /// The taskbar notifier base.
    /// </summary>
    public partial class TaskbarNotifierBase
    {
        /// <summary>
        /// The corne r_ radius.
        /// </summary>
        private const int CORNER_RADIUS = 15;

        /// <summary>
        /// The apply theme.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        private void ApplyTheme(NotificationType type)
        {
            switch (type)
            {
                case NotificationType.Info:
                    this.SetThemeColor(Color.FromArgb(74, 114, 237));
                    this.imgIcon.Image = Resources.NotifierInfo;
                    break;
                case NotificationType.Success:
                    this.SetThemeColor(Color.FromArgb(0x7F, 0xBA, 0));
                    this.imgIcon.Image = Resources.NotifierSuccess;
                    break;
                case NotificationType.Warning:
                    this.SetThemeColor(Color.FromArgb(201, 189, 0));
                    this.imgIcon.Image = Resources.NotifierWarning;
                    break;
                case NotificationType.Error:
                    this.imgIcon.Image = Resources.NotifierError;
                    this.SetThemeColor(Color.FromArgb(237, 95, 74));
                    break;
            }

            this.Region = Region.FromHrgn(NativeMethods.CreateRoundRectRgn(0, 0, this.Width, this.Height, CORNER_RADIUS, CORNER_RADIUS));
        }

        /// <summary>
        /// The set theme color.
        /// </summary>
        /// <param name="targetColor">
        /// The target color.
        /// </param>
        private void SetThemeColor(Color targetColor)
        {
            this.BackColor = targetColor;
        }
    }

    /// <summary>
    /// The notification type extensions.
    /// </summary>
    public static class NotificationTypeExtensions
    {
        /// <summary>
        /// The convert to notification type.
        /// </summary>
        /// <param name="iconType">
        /// The icon type.
        /// </param>
        /// <returns>
        /// The <see cref="NotificationType"/>.
        /// </returns>
        public static NotificationType ConvertToNotificationType(this ToolTipIcon iconType)
        {
            switch (iconType)
            {
                case ToolTipIcon.Warning:
                    return NotificationType.Warning;
                case ToolTipIcon.Error:
                    return NotificationType.Error;
                default:
                    return NotificationType.Info;
            }
        }

        /// <summary>
        /// The convert to tooltip icon.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="ToolTipIcon"/>.
        /// </returns>
        public static ToolTipIcon ConvertToTooltipIcon(this NotificationType type)
        {
            switch (type)
            {
                case NotificationType.Warning:
                    return ToolTipIcon.Warning;
                case NotificationType.Error:
                    return ToolTipIcon.Error;
                default:
                    return ToolTipIcon.Info;
            }
        }
    }
}