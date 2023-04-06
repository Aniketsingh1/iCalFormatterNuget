// <copyright file="RecurrenceModel.cs" company="Kastle Systems">
// Copyright (c) Kastle Systems. All rights reserved.
// </copyright>

namespace KastleICalInputFormatter.Common.ApplicationModel.VisitorEntityModel
{
    /// <summary>
    /// Model to handle recurring visits.
    /// </summary>
    public class RecurrenceModel
    {
        /// <summary>
        /// Gets or sets frequencey of visit eg; weekly, daily, monthly etc.
        /// </summary>
        public string? RecurrenceFrequecny { get; set; }

        /// <summary>
        /// Gets or sets days in week on which the visit will occur eg; Monday, tuesday etc.
        /// </summary>
        public string[]? RecurrenceDays { get; set; }

        /// <summary>
        /// Gets or sets months in which the visit will happen eg. January etc.
        /// </summary>
        public string? RecurrenceMonth { get; set; }

        /// <summary>
        /// Gets or sets date on which the visit will happen.
        /// </summary>
        public string? RecurrenceDate { get; set; }

        /// <summary>
        /// Gets or sets RepeatEvery.
        /// </summary>
        public string? RepeatEvery { get; set; }

        /// <summary>
        /// Gets or sets RepeatDayNumberOfMonth.
        /// </summary>
        public string? RepeatDayNumberOfMonth { get; set; }

        /// <summary>
        /// Gets or sets StartingDayOfTheWeek.
        /// </summary>
        public string? StartingDayOfTheWeek { get; set; }

        /// <summary>
        /// Gets or sets EndingDateOfTheVisit.
        /// </summary>
        public DateTime? EndingDateOfTheVisit { get; set; }

        /// <summary>
        /// Gets or sets NumberOfOccurences i.e. count of visits.
        /// </summary>
        public string? NumberOfOccurences { get; set; }

        /// <summary>
        /// Gets or sets DayNumberForMonth eg. 2nd Friday of Month.
        /// </summary>
        public string? DayNumberForMonth { get; set; }
    }
}
