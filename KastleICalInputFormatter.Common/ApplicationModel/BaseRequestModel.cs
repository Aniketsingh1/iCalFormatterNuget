// <copyright file="BaseRequestModel.cs" company="Kastle Systems">
// Copyright (c) Kastle Systems. All rights reserved.
// </copyright>

namespace KastleICalInputFormatter.Common.ApplicationModel
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents Class BaseRequestModel.
    /// </summary>
    public class BaseRequestModel
    {
        /// <summary>
        /// Gets or sets UserFirstName.
        /// </summary>
        public string? UserFirstName { get; set; }

        /// <summary>
        /// Gets or sets UserLastName.
        /// </summary>
        public string? UserLastName { get; set; }

        /// <summary>
        /// Gets or sets UserMail.
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// Gets or sets UserCardHolderId.
        /// </summary>
        public string? UserCardHolderID { get; set; }
    }
}
