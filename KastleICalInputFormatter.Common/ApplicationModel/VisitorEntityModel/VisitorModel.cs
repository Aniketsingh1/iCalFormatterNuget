// <copyright file="VisitorModel.cs" company="Kastle Systems">
// Copyright (c) Kastle Systems. All rights reserved.
// </copyright>

namespace KastleICalInputFormatter.Common.ApplicationModel.VisitorEntityModel
{
    /// <summary>
    /// Visitor Model.
    /// </summary>
    public class VisitorModel
    {
        /// <summary>
        /// Gets or sets company from which visitor is coming.
        /// </summary>
        public string? VisitorCompany { get; set; }

        /// <summary>
        /// Gets or sets first Name of the Visitor.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets last Name of the Visitor.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets mobile Number of the Visitor.
        /// </summary>
        public string? MobileNumber { get; set; }

        /// <summary>
        /// Gets or sets email Id of the visitor.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets action as Per the Visitor.
        /// </summary>
        public string? Action { get; set; }

        /// <summary>
        /// Gets or sets country code for Mobile number.
        /// </summary>
        public string? MobileNumberCountryPrifix { get; set; }

        /// <summary>
        /// Gets or sets ical tag for if the visitor is required or optional.
        /// </summary>
        public string Role { get; set; } = "REQ-PARTICIPANT";

        /// <summary>
        /// Gets or sets status if meeting is accepted or needs action or rejected.
        /// </summary>
        public string ParticipationStatus { get; set; } = "NEEDS-ACTION";

        /// <summary>
        /// Gets or sets a value indicating whether response from Visitor is required or not.
        /// </summary>
        public bool ResponseRequired { get; set; } = false;

        /// <summary>
        /// Gets or sets if visitor is individual or a group coming under one visitor sign up.
        /// </summary>
        public string TypeOfVisitor { get; set; } = "INDIVIDUAL";

        /// <summary>
        /// Gets or sets CardholderGuid.
        /// </summary>
        public Guid? CardholderGuid { get; set; }

        /// <summary>
        /// Gets or sets CountryId.
        /// </summary>
        public string? CountryId { get; set; }
    }
}
