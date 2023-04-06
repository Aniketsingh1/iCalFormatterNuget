// <copyright file="NotificationModel.cs" company="Kastle Systems">
// Copyright (c) Kastle Systems. All rights reserved.
// </copyright>

namespace KastleICalInputFormatter.Common.ApplicationModel.VisitorEntityModel
{
    /// <summary>
    /// Notification Model.
    /// </summary>
    public class NotificationModel
    {
        /// <summary>
        /// Gets or sets time at which the notification needs to be triggered for meeting.
        /// </summary>
        public string NotificationTriggerTime { get; set; }

        /// <summary>
        /// Gets or sets duration of Notification.
        /// </summary>
        public string NotificationDuration { get; set; }

        /// <summary>
        /// Gets or sets action Required for Notification for eg. Display.
        /// </summary>
        public string? Action { get; set; } = "DISPLAY";

        /// <summary>
        /// Gets or sets description to be displayed for notification.
        /// </summary>
        public string? NotificationDescription { get; set; }
    }
}
