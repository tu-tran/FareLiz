using System;

namespace SkyDean.FareLiz.Core.Presentation
{
    public enum NotificationType { Info, Success, Exclamation, Warning, Error }
    public enum ConfirmationType { Yes, No, Cancel }

    public interface INotification
    {
        /// <summary>
        /// Notify users
        /// </summary>
        /// <param name="text">Description</param>
        /// <param name="title">Title</param>
        /// <param name="type">Type</param>
        void Inform(object sender, string text, string title, NotificationType type);

        /// <summary>
        /// Confirm with users
        /// </summary>
        /// <param name="text">Description</param>
        /// <param name="title">Title</param>
        /// <returns>Confirmation result</returns>
        ConfirmationType Confirm(object sender, string text, string title);
    }
}
