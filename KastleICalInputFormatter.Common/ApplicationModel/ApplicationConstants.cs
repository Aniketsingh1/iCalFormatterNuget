// <copyright file="ApplicationConstants.cs" company="Kastle Systems">
// Copyright (c) Kastle Systems. All rights reserved.
// </copyright>

namespace KastleICalInputFormatter.Common.ApplicationModel
{
    /// <summary>
    /// Application constants.
    /// </summary>
    public class ApplicationConstants
    {
        /// <summary>
        /// Represents constant BEGIN.
        /// </summary>
        public const string BEGIN = "BEGIN";

        /// <summary>
        /// Represents constant END.
        /// </summary>
        public const string END = "END";

        /// <summary>
        /// Represents constant VCALENDER.
        /// </summary>
        public const string VCALENDER = "VCALENDAR";

        /// <summary>
        /// Represents constant VTIMEZONE.
        /// </summary>
        public const string VTIMEZONE = "VTIMEZONE";

        /// <summary>
        /// Represents constant STANDARD.
        /// </summary>
        public const string STANDARD = "STANDARD";

        /// <summary>
        /// Represents constant DAYLIGHT.
        /// </summary>
        public const string DAYLIGHT = "DAYLIGHT";

        /// <summary>
        /// Represents constant VEVENT.
        /// </summary>
        public const string VEVENT = "VEVENT";

        /// <summary>
        /// Represents constant VALARM.
        /// </summary>
        public const string VALARM = "VALARM";

        /// <summary>
        /// Represents constant REQUEST.
        /// </summary>
        public const string REQUEST = "REQUEST";

        /// <summary>
        /// Represents constant VERSION.
        /// </summary>
        public const string VERSION = "2.0";

        /// <summary>
        /// Represents constant TYPEOFCALENDER.
        /// </summary>
        public const string TYPEOFCALENDER = "INDIVIDUAL";

        /// <summary>
        /// Represents constant Type of Weekly frequency.
        /// </summary>
        public const string RECURRINGFREQUENCYWEEKLY = "WEEKLY";

        /// <summary>
        /// Represents constant Type of Monthly frequency.
        /// </summary>
        public const string RECURRINGFREQUENCYMONTHLY = "MONTHLY";

        /// <summary>
        /// Represents constant Type of Yearly frequency.
        /// </summary>
        public const string RECURRINGFREQUENCYYEARLY = "YEARLY";

        /// <summary>
        /// Represents constant Type of Daily frequency.
        /// </summary>
        public const string RECURRINGFREQUENCYDAILY = "DAILY";

        /// <summary>
        /// Represents constant identity type.
        /// </summary>
        public const string IDENTITYTYPEVISITOR = "VISITOR";

        /// <summary>
        /// Represents constant prodId.
        /// </summary>
        public const string PRODID = "PRODID";

        /// <summary>
        /// Represents constant UID.
        /// </summary>
        public const string UID = "UID";

        /// <summary>
        /// Represents constant created.
        /// </summary>
        public const string CREATED = "CREATED";

        /// <summary>
        /// Represents constant attendee.
        /// </summary>
        public const string ATTENDEE = "ATTENDEE";

        /// <summary>
        /// Represents constant special instructions.
        /// </summary>
        public const string NOTES = "X-KASTLE-NOTES";

        /// <summary>
        /// Represents constant Guard note.
        /// </summary>
        public const string SPECIALINSTRUCTIONS = "X-KASTLE-SPECIALINSTRUCTIONS";

        /// <summary>
        /// Represents constant VVENUE.
        /// </summary>
        public const string VVENUE = "VVENUE";

        /// <summary>
        /// Enum for KMF.
        /// </summary>
        public enum KMFFlag
        {
            /// <summary>
            /// Give community Level value.
            /// </summary>
            CommunityLevel = 1,

            /// <summary>
            /// Gives Apartment Level value.
            /// </summary>
            ApartmentLevel = 0,
        }

        /// <summary>
        /// Represents constant VVENUE.
        /// </summary>
        public const string ORGANIZER = "ORGANIZER";

        /// <summary>
        /// Represents constant DESCRIPTION.
        /// </summary>
        public const string DESCRIPTION = "DESCRIPTION";

    }
}
