// <copyright file="ParameterList.cs" company="Kastle Systems">
// Copyright (c) Kastle Systems. All rights reserved.
// </copyright>

namespace KastleICalInputFormatter.Common.ApplicationModel
{
    using System.Collections.Generic;

    /// <summary>
    /// ParameterList static class.
    /// </summary>
    public static class ParameterList
    {
        public static List<string> ParamList = new List<string>()
        {
        "DTSTART",
        "DTEND",
        "DTSTAMP",
        "ORGANIZER",
        "UID",
        "CREATED",
        "LAST-MODIFIED",
        "DESCRIPTION",
        "SUMMARY",
        "LOCATION",
        "CLASS",
        "PRIORITY",
        "TRANSP",
        "STATUS",
        "SEQUENCE",
        "X-KASTLE-ACCESSPROFILES",
        "X-KASTLE-INSTITUTIONID",
        "X-KASTLE-IDENTITYTYPE",
        "X-MICROSOFT-CDO-OWNERAPPTID",
        "X-MICROSOFT-CDO-APPT-SEQUENCE",
        "X-MICROSOFT-CDO-BUSYSTATUS",
        "X-MICROSOFT-CDO-INTENDEDSTATUS",
        "X-MICROSOFT-CDO-ALLDAYEVENT",
        "X-MICROSOFT-CDO-IMPORTANCE",
        "X-MICROSOFT-CDO-INSTTYPE",
        "X-MICROSOFT-DONOTFORWARDMEETING",
        "X-MICROSOFT-DISALLOW-COUNTER",
        "X-KASTLE-F&FINFO",
        "X-KASTLE-SUITE",
        "X-KASTLE-NOTIFYONARRIVAL",
        "X-KASTLE-SENDMAILTOVISITORS",
        "X-KASTLE-NOTES",
        "X-KASTLE-SPECIALINSTRUCTIONS",
        "RRULE",
        "ATTENDEE",
        "X-KASTLE-UTCARRIVALDATE",
        "X-KASTLE-UTCARRIVALTIME",
        "X-KASTLE-UTCDEPARTUREDATE",
        "X-KASTLE-UTCDEPARTURETIME",
        "X-KASTLE-FLOORID",
        };
    }
}
