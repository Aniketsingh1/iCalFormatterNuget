// <copyright file="VisitorRequestModel.cs" company="Kastle Systems">
// Copyright (c) Kastle Systems. All rights reserved.
// </copyright>

namespace KastleICalInputFormatter.Common.ApplicationModel.VisitorEntityModel
{
    using System;
    using System.Collections.Generic;
    using KastleICalInputFormatter.Common.VisitorRequestGenerator;

    /// <summary>
    /// Request Model for Ical.
    /// </summary>
    public class VisitorRequestModel : BaseRequestModel
    {
        /// <summary>
        /// Gets or sets list of visitors.
        /// </summary>
        public List<VisitorModel> VisitorModel { get; set; }

        /// <summary>
        /// Gets or sets model for recurring visits.
        /// </summary>
        public RecurrenceModel? RecurrenceModel { get; set; }

        /// <summary>
        /// Gets or sets visit guid for every new visit.
        /// </summary>
        public Guid? VisitGuid { get; set; }

        /// <summary>
        /// Gets or sets company to which the visit is happening.
        /// </summary>
        public string? VisitingCompany { get; set; }

        /// <summary>
        /// Gets or sets application specific id from which is visit is created eg; google, Microsoft.
        /// </summary>
        public string ProdID { get; set; }

        /// <summary>
        /// Gets or sets timeZone in which is visit is created eg; Asia/Kolkata.
        /// </summary>
        public string Timezone { get; set; }

        /// <summary>
        /// Gets or sets institution in which visit is happening.
        /// </summary>
        public string InstitutionID { get; set; }

        /// <summary>
        /// Gets or sets arrival Date for visit.
        /// </summary>
        public DateTime ArrivalDate { get; set; }

        /// <summary>
        /// Gets or sets ArrivalTime.
        /// </summary>
        public string ArrivalTime { get; set; }

        /// <summary>
        /// Gets or sets end Date for visit.
        /// </summary>
        public DateTime DepartureDate { get; set; }

        /// <summary>
        /// Gets or sets DepartureTime.
        /// </summary>
        public string DepartureTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether check if visit is for whole day or not.
        /// </summary>
        public bool IsAllDayEvent { get; set; } = false;

        /// <summary>
        /// Gets or sets description for the visit.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets title for the visit.
        /// </summary>
        public string? Summary { get; set; }

        /// <summary>
        /// Gets or sets last Modified date for visit.
        /// </summary>
        public DateTime? LastModified { get; set; }

        /// <summary>
        /// Gets or sets date on which the visit is created.
        /// </summary>
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets Intended status.
        /// </summary>
        public string IntendedStatus { get; set; } = "BUSY";

        /// <summary>
        /// Gets or sets Priority Level.
        /// </summary>
        public int PriorityLevel { get; set; }

        /// <summary>
        /// Gets or sets Transparency.
        /// </summary>
        public string Transparency { get; set; } = "OPAQUE";

        /// <summary>
        /// Gets or sets Meeting Status.
        /// </summary>
        public string MeetingStatus { get; set; } = "CONFIRMED";

        /// <summary>
        /// Gets or sets number of time meeting is modified.
        /// </summary>
        public int NumberOfTimeMeetingModified { get; set; }

        /// <summary>
        /// Gets or sets Location.
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// Gets or sets Identity type.
        /// </summary>
        public string IdentityType { get; set; } = ApplicationConstants.IDENTITYTYPEVISITOR;

        /// <summary>
        /// Gets or sets Notification model.
        /// </summary>
       // public NotificationModel? NotificationModel { get; set; }

        /// <summary>
        /// Gets or sets appointment identifier.
        /// </summary>
        public string? UniqueAppointmentIdentifier { get; set; }

        /// <summary>
        /// Gets or sets AccessProfiles.
        /// </summary>
        public List<string>? AccessProfiles { get; set; }

        /// <summary>
        /// Gets or sets Properties of timezone info.
        /// </summary>
        public TimeZoneDetails? TimeZoneDetails { get; set; }

        /// <summary>
        /// Gets or sets CreationDate.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets or sets Notes.
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Gets or sets SpecialInstructions.
        /// </summary>
        public string? SpecialInstructions { get; set; }

        /// <summary>
        /// Gets or sets SuiteInfo.
        /// </summary>
        public string? SuiteInfo { get; set; }

        /// <summary>
        /// Gets or sets VisitingCompanyDetails.
        /// </summary>
        public VisitingCompanyDetails? VisitingCompanyDetails { get; set; }

        /// <summary>
        /// Gets or sets IsCommunityLevel.
        /// </summary>
        public bool? IsCommunityLevel { get; set; }

        /// <summary>
        /// Gets or sets NotifyOnArrival.
        /// </summary>
        public bool? NotifyOnArrival { get; set; }

        /// <summary>
        /// Gets or sets IsSendMailToVisitors.
        /// </summary>
        public bool? IsSendMailToVisitors { get; set; }

        /// <summary>
        /// Gets or sets FloorId.
        /// </summary>
        public string FloorId { get; set; }
    }

    /// <summary>
    /// Class for VisitingCompanyDetails.
    /// </summary>
    public class VisitingCompanyDetails
    {
        /// <summary>
        /// Gets or sets UID.
        /// </summary>
        public Guid? UID { get; set; }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets StreetAddress.
        /// </summary>
        public string? StreetAddress { get; set; }
    }
}
