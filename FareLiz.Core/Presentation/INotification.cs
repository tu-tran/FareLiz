namespace SkyDean.FareLiz.Core.Presentation
{
    /// <summary>The notification type.</summary>
    public enum NotificationType
    {
        /// <summary>The info.</summary>
        Info, 

        /// <summary>The success.</summary>
        Success, 

        /// <summary>The exclamation.</summary>
        Exclamation, 

        /// <summary>The warning.</summary>
        Warning, 

        /// <summary>The error.</summary>
        Error
    }

    /// <summary>The confirmation type.</summary>
    public enum ConfirmationType
    {
        /// <summary>The yes.</summary>
        Yes, 

        /// <summary>The no.</summary>
        No, 

        /// <summary>The cancel.</summary>
        Cancel
    }

    /// <summary>The Notification interface.</summary>
    public interface INotification
    {
        /// <summary>
        /// Notify users
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="text">
        /// Description
        /// </param>
        /// <param name="title">
        /// Title
        /// </param>
        /// <param name="type">
        /// Type
        /// </param>
        void Inform(object sender, string text, string title, NotificationType type);

        /// <summary>
        /// Confirm with users
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="text">
        /// Description
        /// </param>
        /// <param name="title">
        /// Title
        /// </param>
        /// <returns>
        /// Confirmation result
        /// </returns>
        ConfirmationType Confirm(object sender, string text, string title);
    }
}