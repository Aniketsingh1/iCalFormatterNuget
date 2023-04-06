// <copyright file="TimeZoneDetails.cs" company="Kastle Systems">
// Copyright (c) Kastle Systems. All rights reserved.
// </copyright>

namespace KastleICalInputFormatter.Common.VisitorRequestGenerator
{
    using System;

    /// <summary>
    /// Class represents TimeZoneInfo.
    /// </summary>
    public class TimeZoneDetails
    {
        /// <summary>
        /// Gets or sets standard time zone name.
        /// </summary>
        public string StandardTimeZoneName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets IsDaylightSavings.
        /// </summary>
        public bool IsDayLightSavings { get; set; } = false;

        /// <summary>
        /// Gets or sets standard offset from.
        /// </summary>
        public string StandardOffSetStartFrom { get; set; }

        /// <summary>
        /// Gets or sets standard local time off set to.
        /// </summary>
        public string StandardOffSetStartTo { get; set; }

        /// <summary>
        /// Gets or sets StandardOffSetDate.
        /// </summary>
        public DateTime StandardOffSetDate { get; set; }

        /// <summary>
        /// Gets or sets Daylight local time offset from.
        /// </summary>
        public string? DaylightStandardOffSetStartFrom { get; set; }

        /// <summary>
        /// Gets or sets Daylight off set to.
        /// </summary>
        public string? DaylightStandardOffSetStartTo { get; set; }

        /// <summary>
        /// Gets or sets DaylightOffSetDate.
        /// </summary>
        public DateTime DaylightOffSetDate { get; set; }

        /// <summary>
        /// Gets or sets Daylight time zone name.
        /// </summary>
        public string? DaylightTimeZoneName { get; set; }

        /// <summary>
        /// Gets or sets timeZone in which is visit is created eg; Asia/Kolkata.
        /// </summary>
        public string Timezone { get; set; }
    }
}
