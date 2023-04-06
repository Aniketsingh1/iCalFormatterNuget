// <copyright file="ICalModel.cs" company="Kastle Systems">
// Copyright (c) Kastle Systems. All rights reserved.
// </copyright>

namespace KastleICalInputFormatter.Common.ApplicationModel.ICalEntityModel
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Ical Model.
    /// </summary>
    public class ICalModel
    {
        /// <summary>
        /// Gets or sets VCalender class.
        /// </summary>
        public VCalender VCalender
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Gets or sets VCalender properties.
    /// </summary>
    public class VCalender
    {
        /// <summary>
        /// Gets or sets Standard properties of VCalender.
        /// </summary>
        public Standard_Properties Standard_Properties { get; set; }

        /// <summary>
        /// Gets or sets VTimezone.
        /// </summary>
        public VTimezone VTimezone { get; set; }

        /// <summary>
        /// Gets or sets VEvent.
        /// </summary>
        public VEvent VEvent { get; set; }

        /// <summary>
        /// Gets or sets VVenue.
        /// </summary>
        public VVenue? VVenue { get; set; }
    }

    /// <summary>
    /// Standard property class.
    /// </summary>
    public class Standard_Properties
    {
        /// <summary>
        /// Gets or sets ProdID.
        /// </summary>
        public string ProdID { get; set; }

        /// <summary>
        /// Gets or sets Version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets Calscale.
        /// </summary>
        public string Calscale { get; set; }

        /// <summary>
        /// Gets or sets Method.
        /// </summary>
        public string Method { get; set; }
    }

    /// <summary>
    /// Class for VTimezone.
    /// </summary>
    public class VTimezone
    {
        /// <summary>
        /// Gets or sets Tzid.
        /// </summary>
        public string Tzid { get; set; }

        /// <summary>
        /// Gets or sets Standard class.
        /// </summary>
        public Standard Standard { get; set; }

        /// <summary>
        /// Gets or sets Daylight class.
        /// </summary>
        public Daylight? Daylight { get; set; }
    }

    /// <summary>
    /// Represents Standard class.
    /// </summary>
    public class Standard
    {
        /// <summary>
        /// Gets or sets TzOffSetFrom.
        /// </summary>
        public string TzOffSetFrom { get; set; }

        /// <summary>
        /// Gets or sets TzOffSetTo.
        /// </summary>
        public string TzOffSetTo { get; set; }

        /// <summary>
        /// Gets or sets TzName.
        /// </summary>
        public string TzName { get; set; }

        /// <summary>
        /// Gets or sets DtStart.
        /// </summary>
        public DtStart DtStart { get; set; }
    }

    /// <summary>
    /// Represents Class Daylight.
    /// </summary>
    public class Daylight
    {
        /// <summary>
        /// Gets or sets DtStart.
        /// </summary>
        public DtStart DtStart { get; set; }

        /// <summary>
        /// Gets or sets TzOffSetFrom.
        /// </summary>
        public string TzOffSetFrom { get; set; }

        /// <summary>
        /// Gets or sets TzOffSetTo.
        /// </summary>
        public string TzOffSetTo { get; set; }

        /// <summary>
        /// Gets or sets TzName.
        /// </summary>
        public string TzName { get; set; }
    }

    /// <summary>
    /// Represents class VEvent.
    /// </summary>
    public class VEvent
    {
        /// <summary>
        /// Gets or sets DtStart.
        /// </summary>
        public DtStart DtStart { get; set; }

        /// <summary>
        /// Gets or sets DtEnd.
        /// </summary>
        public DtEnd DtEnd { get; set; }

        /// <summary>
        /// Gets or sets Rrule.
        /// </summary>
        public Rrule? Rrule { get; set; }

        /// <summary>
        /// Gets or sets DtStamp.
        /// </summary>
        public string DtStamp { get; set; }

        /// <summary>
        /// Gets or sets Organizer.
        /// </summary>
        public Organizer Organizer { get; set; }

        /// <summary>
        /// Gets or sets UID.
        /// </summary>
        public Guid UID { get; set; }

        /// <summary>
        /// Gets or sets Attendees.
        /// </summary>
        public List<Attendee> Attendees { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets Language.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets Class.
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// Gets or sets Priority.
        /// </summary>
        public string Priority { get; set; }

        /// <summary>
        /// Gets or sets transparency.
        /// </summary>
        public string Transp { get; set; }

        /// <summary>
        /// Gets or sets Status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets Seuqence.
        /// </summary>
        public string Sequence { get; set; }

        /// <summary>
        /// Gets or sets Location.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets X-MICROSOFT-CDO-APPT-SEQUENCE.
        /// </summary>
        public string XMicrosoftCdoApptSequence { get; set; }

        /// <summary>
        /// Gets or sets X-MICROSOFT-CDO-OWNERAPPTID.
        /// </summary>
        public string XMicrosoftCdoOwnerApptID { get; set; }

        /// <summary>
        /// Gets or sets X-KASTLE-INSTITUTIONID.
        /// </summary>
        public string XKastleInstitutionID { get; set; }

        /// <summary>
        /// Gets or sets X-MICROSOFT-CDO-BUSYSTATUS.
        /// </summary>
        public string? XMicrosoftCdoBusyStatus { get; set; }

        /// <summary>
        /// Gets or sets X-MICROSOFT-CDO-INTENDEDSTATUS.
        /// </summary>
        public string XMicrosoftCdoIntendedStatus { get; set; }

        /// <summary>
        /// Gets or sets X-MICROSOFT-CDO-ALLDAYEVENT.
        /// </summary>
        public string XMicrosoftCdoAllDayEvent { get; set; }

        /// <summary>
        /// Gets or sets X-MICROSOFT-CDO-IMPORTANCE.
        /// </summary>
        public string? XMicrosoftCdoImportance { get; set; }

        /// <summary>
        /// Gets or sets X-MICROSOFT-CDO-INSTTYPE.
        /// </summary>
        public string? XMicrosoftCdoInstType { get; set; }

        /// <summary>
        /// Gets or sets X-MICROSOFT-DONOTFORWARDMEETING.
        /// </summary>
        public string? XMicrosoftDoNotForwardMeeting { get; set; }

        /// <summary>
        /// Gets or sets X-MICROSOFT-DISALLOW-COUNTER.
        /// </summary>
        public string? XMicrosoftDisallowCounter { get; set; }

        /// <summary>
        /// Gets or sets X-MICROSOFT-LOCATIONS.
        /// </summary>
        public string[]? XMicrosoftLocations { get; set; }

        /// <summary>
        /// Gets or sets X-KASTLE-IDENTITYTYPE.
        /// </summary>
        public string XKastleIdentityType { get; set; }

        /// <summary>
        /// Gets or sets X-KASTLE-ACCESSPROFILES.
        /// </summary>
        public List<string>? XkastleAccessProfiles { get; set; }

        /// <summary>
        /// Gets or sets Last-modified.
        /// </summary>
        public string LastModified { get; set; }

        /// <summary>
        /// Gets or sets Summary.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets Created.
        /// </summary>
        public string? Created { get; set; }

        /// <summary>
        /// Gets or sets class VAlarm.
        /// </summary>
        public VAlarm? VAlarm { get; set; }

        /// <summary>
        /// Gets or sets XKastleNotes.
        /// </summary>
        public string? XKastleNotes { get; set; }

        /// <summary>
        /// Gets or sets XKastleSpecialInstructions.
        /// </summary>
        public string? XKastleSpecialInstructions { get; set; }

        /// <summary>
        /// Gets or sets XKastleSuite.
        /// </summary>
        public string? XKastleSuite { get; set; }

        /// <summary>
        /// Gets or sets XKastleKMFInfo.
        /// </summary>
        public string? XKastleKMFInfo { get; set; }

        /// <summary>
        /// Gets or sets XKastleNotifyOnArrival.
        /// </summary>
        public string? XKastleNotifyOnArrival { get; set; }

        /// <summary>
        /// Gets or sets XKastleIsSendMailToVisitors.
        /// </summary>
        public string? XKastleIsSendMailToVisitors { get; set; }

        /// <summary>
        /// Gets or sets XKastleUTCArrivalDate.
        /// </summary>
        public string? XKastleUTCArrivalDate { get; set; }

        /// <summary>
        /// Gets or sets XKastleUTCArrivalTime.
        /// </summary>
        public string? XKastleUTCArrivalTime { get; set; }

        /// <summary>
        /// Gets or sets XKastleUTCDepartureDate.
        /// </summary>
        public string? XKastleUTCDepartureDate { get; set; }

        /// <summary>
        /// Gets or sets XKastleUTCDepartureTime.
        /// </summary>
        public string? XKastleUTCDepartureTime { get; set; }

        /// <summary>
        /// Gets or sets XKastleFloorId.
        /// </summary>
        public string XKastleFloorId { get; set; }

    }

    /// <summary>
    /// Represents Class DtStart.
    /// </summary>
    public class DtStart
    {
        /// <summary>
        /// Gets or sets Tzid.
        /// </summary>
        public string Tzid { get; set; }

        /// <summary>
        /// Gets or sets Date-time.
        /// </summary>

        [JsonPropertyName("date-time")]
        public string DateTime { get; set; }

        /// <summary>
        /// Gets or sets Date.
        /// </summary>
        public string Date { get; set; }
    }

    /// <summary>
    /// Represents class DtEnd.
    /// </summary>
    public class DtEnd
    {
        /// <summary>
        /// Gets or sets Tzid.
        /// </summary>
        public string Tzid { get; set; }

        /// <summary>
        /// Gets or sets Date-time.
        /// </summary>
        [JsonPropertyName("date-time")]
        public string DateTime { get; set; }

        /// <summary>
        /// Gets or sets Date.
        /// </summary>
        public string Date { get; set; }
    }

    /// <summary>
    /// Represenents Class Rrule.
    /// </summary>
    public class Rrule
    {
        /// <summary>
        /// Gets or sets Freq.
        /// </summary>
        public string Freq { get; set; }

        /// <summary>
        /// Gets or sets ByDay.
        /// </summary>
        public string? ByDay { get; set; }

        /// <summary>
        /// Gets or sets ByMonth.
        /// </summary>
        public int? ByMonth { get; set; }

        /// <summary>
        /// Gets or sets Until.
        /// </summary>
        public string? Until { get; set; }

        /// <summary>
        /// Gets or sets Interval.
        /// </summary>
        public int? Interval { get; set; }

        /// <summary>
        /// Gets or sets Count.
        /// </summary>
        public int? Count { get; set; }

        /// <summary>
        /// Gets or sets BySetPos.
        /// </summary>
        public string? BySetPos { get; set; }

        /// <summary>
        /// Gets or sets ByMonthDay.
        /// </summary>
        public int? ByMonthDay { get; set; }

        /// <summary>
        /// Gets or sets WKST.
        /// </summary>
        public string? WKST { get; set; }

        /// <summary>
        /// Gets or sets ByYearDay.
        /// </summary>
        public string? ByYearDay { get; set; }

        /// <summary>
        /// Gets or sets ByWeekNo.
        /// </summary>
        public string? ByWeekNo { get; set; }
    }

    /// <summary>
    /// Represents class Organizer.
    /// </summary>
    public class Organizer
    {
        /// <summary>
        /// Gets or sets Cn i.e. Display name.
        /// </summary>
        public string Cn { get; set; }

        /// <summary>
        /// Gets or sets Mailto.
        /// </summary>
        public string? MailTo { get; set; }

        /// <summary>
        /// Gets or sets tel.
        /// </summary>
        public string Tel { get; set; }
    }

    /// <summary>
    /// Represents class VAlarm.
    /// </summary>
    public class VAlarm
    {
        /// <summary>
        /// Gets or sets trigger.
        /// </summary>
        public string Trigger { get; set; }

        /// <summary>
        /// Gets or sets Action.
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        public string? Description { get; set; }
    }

    /// <summary>
    /// Class represents Vvenue.
    /// </summary>
    public class VVenue
    {
        /// <summary>
        /// Gets or sets UID.
        /// </summary>
        public string? UID { get; set; }

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
