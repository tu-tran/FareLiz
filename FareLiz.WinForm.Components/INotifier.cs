namespace SkyDean.FareLiz.WinForm.Components
{
    using SkyDean.FareLiz.Core.Presentation;

    /// <summary>The Notifier interface.</summary>
    public interface INotifier
    {
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
        void Show(string title, string content, int timeToStay, NotificationType type, bool isRTF);

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
        void Show(string title, string content, int timeToStay, NotificationType type);
    }
}