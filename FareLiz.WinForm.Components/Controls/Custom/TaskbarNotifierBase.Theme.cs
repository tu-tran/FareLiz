namespace SkyDean.FareLiz.WinForm.Components.Controls.Custom
{
    using System.Drawing;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core.Presentation;
    using SkyDean.FareLiz.WinForm.Components.Properties;
    using SkyDean.FareLiz.WinForm.Components.Utils;    

    public partial class TaskbarNotifierBase
    {
        const int CORNER_RADIUS = 15;
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

        private void SetThemeColor(Color targetColor)
        {
            this.BackColor = targetColor;
        }
    }

    public static class NotificationTypeExtensions
    {
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
