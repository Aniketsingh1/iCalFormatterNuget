// <copyright file="Attendee.cs" company="Kastle Systems">
// Copyright (c) Kastle Systems. All rights reserved.
// </copyright>

namespace KastleICalInputFormatter.Common.ApplicationModel.ICalEntityModel
{
    using System;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Attendee Model.
    /// </summary>
    public class Attendee
    {
        /// <summary>
        /// Gets or sets CuType.
        /// </summary>
        public string CuType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Role.
        /// </summary>
        public string Role
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Parstat.
        /// </summary>
        public string Parstat
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or set RSVP.
        /// </summary>
        public string RSVP
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Cn i.e. Display Name.
        /// </summary>
        public string? Cn
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets mail id.
        /// </summary>
        public string mailto
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets tel i.e. mobile number.
        /// </summary>
        public string Tel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Kastle visitor company.
        /// </summary>
        [JsonPropertyName("X-KASTLE-VISITORCOMPANY")]
        public string XKastleVisitorCompany
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets Card Holder guid.
        /// </summary>
        [JsonPropertyName("X-KASTLE-CARDHOLDERGUID")]
        public Guid XKastleCardHolderGUID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets XKastleCountryCode.
        /// </summary>
        public string XKastleCountryCode { get; set; }

        /// <summary>
        /// Gets or sets XKastleCountryId.
        /// </summary>
        public string XKastleCountryId { get; set; }

        /// <summary>
        /// Gets or sets XKastleFirstName.
        /// </summary>
        public string? XKastleFirstName { get; set; }

        /// <summary>
        /// Gets or sets XKastleLastName.
        /// </summary>
        public string? XKastleLastName { get; set; }

        /// <summary>
        /// Gets or sets XKastleEmailId.
        /// </summary>
        public string? XKastleEmailId { get; set; }
    }
}
