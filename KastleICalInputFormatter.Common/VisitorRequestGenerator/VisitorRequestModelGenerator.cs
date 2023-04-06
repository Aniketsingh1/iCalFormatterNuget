// <copyright file="VisitorRequestModelGenerator.cs" company="Kastle Systems">
// Copyright (c) Kastle Systems. All rights reserved.
// </copyright>

namespace KastleICalInputFormatter.Common.VisitorRequestGenerator
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;
    using KastleICalInputFormatter.Common.ApplicationModel;
    using KastleICalInputFormatter.Common.ApplicationModel.VisitorEntityModel;

    /// <summary>
    /// Represents Class VisitorRequestModelGenerator.
    /// </summary>
    public class VisitorRequestModelGenerator : IVisitorRequestGenerator
    {
        /// <summary>
        /// Method to Generate Visitor Request model from iCal String.
        /// </summary>
        /// <param name="iCalRequestString">iCalRequestString.</param>
        /// <returns>VisitorRequestModel.</returns>
        public VisitorRequestModel GenerateVisitorRequestFromICalString(string iCalRequestString)
        {
            VisitorRequestModel visitorRequestModel = new ();
            var accessProfiles = new List<string>();
            if (!string.IsNullOrEmpty(iCalRequestString))
            {
               // string prodIDString = iCalRequestString.Split(new[] { '\r', '\n' })[4];
                visitorRequestModel.ProdID = "Microsoft Exchange server 2010";

                string vTimezoneString = this.GetOperationalStringFromIcalString(iCalRequestString, string.Concat(ApplicationConstants.BEGIN, ":", ApplicationConstants.VTIMEZONE), string.Concat(ApplicationConstants.END, ":", ApplicationConstants.VTIMEZONE));

                TimeZoneDetails timeZoneDetails = new ();
                if (!string.IsNullOrEmpty(vTimezoneString))
                {
                    timeZoneDetails = this.FillTimeZoneDetails(vTimezoneString);
                    visitorRequestModel.TimeZoneDetails = timeZoneDetails;
                }

                string vVenueString = this.GetOperationalStringFromIcalString(iCalRequestString, string.Concat(ApplicationConstants.BEGIN, ":", ApplicationConstants.VVENUE), string.Concat(ApplicationConstants.END, ":", ApplicationConstants.VVENUE));
                if (!string.IsNullOrEmpty(vVenueString))
                {
                    var vVenue = this.FillVVenueDetails(vVenueString);
                    visitorRequestModel.VisitingCompanyDetails = vVenue;
                }

                string vEventString = this.GetOperationalStringFromIcalString(iCalRequestString, string.Concat(ApplicationConstants.BEGIN, ":", ApplicationConstants.VEVENT), string.Concat(ApplicationConstants.END, ":", ApplicationConstants.VEVENT));

                visitorRequestModel.VisitorModel = this.GetVisitorModels(vEventString);

                if (vEventString.Contains(ApplicationConstants.NOTES))
                {
                    var outputSpecialInstructions = this.GetDynamicParamString(vEventString, ApplicationConstants.NOTES);
                    string notes = string.Join("\n", outputSpecialInstructions).Split(':')[1].Trim().Replace(Environment.NewLine, string.Empty);
                    visitorRequestModel.Notes = notes;
                }

                if (vEventString.Contains(ApplicationConstants.SPECIALINSTRUCTIONS))
                {
                    var outputGuardNote = this.GetDynamicParamString(vEventString, ApplicationConstants.SPECIALINSTRUCTIONS);
                    string guardNote = string.Join("\n", outputGuardNote).Split(':')[1].Trim().Replace(Environment.NewLine, string.Empty);
                    visitorRequestModel.SpecialInstructions = guardNote;
                }

                if (vEventString.Contains(ApplicationConstants.DESCRIPTION))
                {
                    var desc = this.GetDynamicParamString(vEventString, ApplicationConstants.DESCRIPTION);
                    string[] dsStr = string.Join(string.Empty, desc).Split(":").Skip(1).ToArray();
                    string description = string.Join(string.Empty, dsStr)/*.Split(':')[1].Trim().Replace(Environment.NewLine, string.Empty);*/;
                    visitorRequestModel.Description = description;
                }

                if (vEventString.Contains(ApplicationConstants.ORGANIZER))
                {
                    var organizerListString = this.GetDynamicParamString(vEventString, ApplicationConstants.ORGANIZER);
                    string organizer = string.Join("\n", organizerListString);
                    organizer = organizer.Split('=')[1];
                    if (organizer.Contains(" "))
                    {
                        string firstName = organizer.Split(':')[0].Split(' ')[0];
                        string lastName = organizer.Split(':')[0].Split(' ')[1];
                        string email = organizer.Split(':').Skip(2).FirstOrDefault();
                        if (!string.IsNullOrEmpty(firstName))
                        {
                            visitorRequestModel.UserFirstName = firstName.Replace("\n", string.Empty);
                        }

                        if (!string.IsNullOrEmpty(lastName))
                        {
                            visitorRequestModel.UserLastName = lastName.Replace("\n", string.Empty);
                        }

                        visitorRequestModel.UserEmail = email;
                    }
                    else
                    {
                        string userEmail = organizer.Split(':')[0].Split(' ')[0];
                        visitorRequestModel.UserEmail = userEmail;
                    }
                }

                if (!string.IsNullOrEmpty(vEventString))
                {
                    using (StringReader reader = new (vEventString))
                    {
                        while (reader.Peek() > -1)
                        {
                            var iCalParam = reader.ReadLine();
                            if (!string.IsNullOrEmpty(iCalParam))
                            {
                                string[] paramValue = iCalParam.Split(":");
                                if (paramValue[0].Equals("UID"))
                                {
                                    string uID = paramValue[1];
                                    bool isValid = Guid.TryParse(uID, out _);
                                    if (isValid)
                                    {
                                        visitorRequestModel.VisitGuid = Guid.Parse(uID);
                                    }
                                    else
                                    {
                                        Guid visitGuid = Guid.NewGuid();
                                        visitorRequestModel.VisitGuid = visitGuid;
                                    }
                                }

                                if (iCalParam.Contains("DTSTART"))
                                {
                                    var dtStartString = iCalParam.Split('=')[1];
                                    string timezone = dtStartString.Split(':')[0];
                                    string arrivalDate = dtStartString.Split(':')[1].Split('T')[0];
                                    string arrivalTime = dtStartString.Split(':')[1].Split('T')[1];
                                    visitorRequestModel.ArrivalDate = DateTime.ParseExact(arrivalDate, "yyyyMMdd", null);
                                    visitorRequestModel.Timezone = timezone;
                                    var res = new List<string>();
                                    for (int i = 0; i < arrivalTime.Length - 1; i += 2)
                                    {
                                        res.Add(arrivalTime.Substring(i, 2));
                                    }

                                    var timeOnly = TimeOnly.Parse(string.Join(":", res.ToArray()));
                                    visitorRequestModel.ArrivalTime = timeOnly.ToString("HH:mm");
                                }

                                if (iCalParam.Contains("DTEND"))
                                {
                                    var dtStartString = iCalParam.Split('=')[1];
                                    string timezone = dtStartString.Split(':')[0];
                                    string departureDate = dtStartString.Split(':')[1].Split('T')[0];
                                    string departureTime = dtStartString.Split(':')[1].Split('T')[1];
                                    visitorRequestModel.DepartureDate = DateTime.ParseExact(departureDate, "yyyyMMdd", null);
                                    var res = new List<string>();
                                    for (int i = 0; i < departureTime.Length - 1; i += 2)
                                    {
                                        res.Add(departureTime.Substring(i, 2));
                                    }

                                    var timeOnly = TimeOnly.Parse(string.Join(":", res.ToArray()));
                                    visitorRequestModel.DepartureTime = timeOnly.ToString("HH:mm");
                                }

                                if (iCalParam.StartsWith("RRULE"))
                                {
                                    visitorRequestModel.RecurrenceModel = this.FillRecurrenceDetails(iCalParam);
                                }

                                if (iCalParam.Contains("DTSTAMP"))
                                {
                                    string createdOn = iCalParam.Split(':')[1];
                                    createdOn = createdOn.Replace("T", string.Empty).ToString().Replace("Z", string.Empty);
                                    visitorRequestModel.CreatedOn = DateTime.ParseExact(createdOn, "yyyyMMddHHmmss", null);
                                }

                                if (iCalParam.Contains("CREATED"))
                                {
                                    string createDate = iCalParam.Split(':')[1];
                                    createDate = createDate.Replace("T", string.Empty).ToString().Replace("Z", string.Empty);
                                    visitorRequestModel.CreationDate = DateTime.ParseExact(createDate, "yyyyMMddHHmmss", null);
                                }

                                if (iCalParam.Contains("LAST-MODIFIED"))
                                {
                                    string lastModified = iCalParam.Split(':')[1];
                                    lastModified = lastModified.Replace("T", string.Empty).ToString().Replace("Z", string.Empty);
                                    visitorRequestModel.LastModified = DateTime.ParseExact(lastModified, "yyyyMMddHHmmss", null);
                                }

                                if (iCalParam.Contains("SUMMARY"))
                                {
                                    string summary = iCalParam.Split(':')[1];
                                    if (!string.IsNullOrEmpty(summary))
                                    {
                                        visitorRequestModel.Summary = summary;
                                    }
                                }

                                if (iCalParam.Contains("LOCATION"))
                                {
                                    string location = iCalParam.Split(':')[1];
                                    if (!string.IsNullOrEmpty(location))
                                    {
                                        visitorRequestModel.Location = location;
                                    }
                                }

                                if (iCalParam.Contains("PRIORITY"))
                                {
                                    string priority = iCalParam.Split(':')[1];
                                    if (!string.IsNullOrEmpty(priority))
                                    {
                                        visitorRequestModel.PriorityLevel = Convert.ToInt32(priority);
                                    }
                                }

                                if (iCalParam.Contains("TRANSP"))
                                {
                                    string transparency = iCalParam.Split(':')[1];
                                    if (!string.IsNullOrEmpty(transparency))
                                    {
                                        visitorRequestModel.Transparency = transparency;
                                    }
                                }

                                if (iCalParam.Contains("STATUS"))
                                {
                                    string status = iCalParam.Split(':')[1];
                                    if (!string.IsNullOrEmpty(status))
                                    {
                                        visitorRequestModel.MeetingStatus = status;
                                    }
                                }

                                if (iCalParam.Contains("SEQUENCE"))
                                {
                                    string sequence = iCalParam.Split(':')[1];
                                    if (!string.IsNullOrEmpty(sequence))
                                    {
                                        visitorRequestModel.PriorityLevel = Convert.ToInt32(sequence);
                                    }
                                }

                                if (iCalParam.Contains("X-KASTLE-INSTITUTIONID"))
                                {
                                    string insitutionID = iCalParam.Split(':')[1];
                                    if (!string.IsNullOrEmpty(insitutionID))
                                    {
                                        visitorRequestModel.InstitutionID = insitutionID;
                                    }
                                }

                                if (iCalParam.Contains("X-KASTLE-IDENTITYTYPE"))
                                {
                                    string identityType = iCalParam.Split(':')[1];
                                    if (!string.IsNullOrEmpty(identityType))
                                    {
                                        visitorRequestModel.IdentityType = identityType;
                                    }
                                }

                                if (iCalParam.Contains("X-MICROSOFT-CDO-OWNERAPPTID"))
                                {
                                    string uniqueAppointmentIdentifier = iCalParam.Split(':')[1];
                                    if (!string.IsNullOrEmpty(uniqueAppointmentIdentifier))
                                    {
                                        visitorRequestModel.UniqueAppointmentIdentifier = uniqueAppointmentIdentifier;
                                    }
                                }

                                if (iCalParam.Contains("X-MICROSOFT-CDO-INTENDEDSTATUS"))
                                {
                                    string intendedStatus = iCalParam.Split(':')[1];
                                    if (!string.IsNullOrEmpty(intendedStatus))
                                    {
                                        visitorRequestModel.IntendedStatus = intendedStatus;
                                    }
                                }

                                if (iCalParam.Contains("X-MICROSOFT-CDO-ALLDAYEVENT"))
                                {
                                    string isAllDayEvent = iCalParam.Split(':')[1];
                                    if (!string.IsNullOrEmpty(isAllDayEvent))
                                    {
                                        visitorRequestModel.IsAllDayEvent = Convert.ToBoolean(isAllDayEvent);
                                    }
                                }

                                if (iCalParam.Contains("X-KASTLE-ACCESSPROFILES"))
                                {
                                    string accessProfileId = iCalParam.Split(':')[1];
                                    if (!string.IsNullOrEmpty(accessProfileId))
                                    {
                                        accessProfiles.Add(accessProfileId);
                                    }
                                }

                                if (iCalParam.Contains("X-KASTLE-SUITE"))
                                {
                                    string suiteInfo = iCalParam.Split(':')[1];
                                    if (!string.IsNullOrEmpty(suiteInfo))
                                    {
                                        visitorRequestModel.SuiteInfo = suiteInfo;
                                    }
                                }

                                if (iCalParam.Contains("X-KASTLE-FANDFINFO"))
                                {
                                    string isCommunityLevel = iCalParam.Split(':')[1];
                                    if (!string.IsNullOrEmpty(isCommunityLevel))
                                    {
                                        if (isCommunityLevel == "1")
                                        {
                                            visitorRequestModel.IsCommunityLevel = true;
                                        }
                                        else
                                        {
                                            visitorRequestModel.IsCommunityLevel = false;
                                        }
                                    }
                                }

                                if (iCalParam.Contains("X-KASTLE-NOTIFYONARRIVAL"))
                                {
                                    string notifyOnArrival = iCalParam.Split(':')[1];
                                    if (!string.IsNullOrEmpty(notifyOnArrival))
                                    {
                                        visitorRequestModel.NotifyOnArrival = Convert.ToBoolean(notifyOnArrival);
                                    }
                                }

                                if (iCalParam.Contains("X-KASTLE-SENDMAILTOVISITORS"))
                                {
                                    string isSendMailToVisitors = iCalParam.Split(':')[1];
                                    if (!string.IsNullOrEmpty(isSendMailToVisitors))
                                    {
                                        visitorRequestModel.IsSendMailToVisitors = Convert.ToBoolean(isSendMailToVisitors);
                                    }
                                }

                                if (iCalParam.StartsWith("X-KASTLE-FLOORID"))
                                {
                                    string floorId = iCalParam.Split(':')[1];
                                    if (!string.IsNullOrEmpty(floorId))
                                    {
                                        visitorRequestModel.FloorId = floorId;
                                    }
                                }

                                // Commenting out as this is not in use currently. Uncomment in future if use comes.
                                //string vAlaramString = this.GetOperationalStringFromIcalString(vEventString, string.Concat(ApplicationConstants.BEGIN, ":", ApplicationConstants.VALARM), string.Concat(ApplicationConstants.END, ":", ApplicationConstants.VALARM));
                                //if (!string.IsNullOrEmpty(vAlaramString))
                                //{
                                //    visitorRequestModel.NotificationModel = this.FillVAlarmDetails(vAlaramString);
                                //}
                            }
                        }
                    }
                }

                if (accessProfiles.Any())
                {
                    visitorRequestModel.AccessProfiles = accessProfiles;
                }

                foreach (var visitor in visitorRequestModel.VisitorModel)
                {
                    if (visitor.CardholderGuid == Guid.Empty || visitor.CardholderGuid == null)
                    {
                        visitor.CardholderGuid = Guid.NewGuid();
                    }
                }

                return visitorRequestModel;
            }
            else
            {
                throw new ArgumentNullException("Input Ical String is null");
            }
        }

        /// <summary>
        /// Method to return UID from ICal string.
        /// </summary>
        /// <param name="iCalString">input ical string.</param>
        /// <returns>UID.</returns>
        public string GetUIDFromICalString(string iCalString)
        {
            string operationString = this.GetOperationalStringFromIcalString(iCalString, ApplicationConstants.UID, ApplicationConstants.ATTENDEE);
            string uID = operationString.Replace("\n", string.Empty).Replace(":", string.Empty);
            return uID;
        }

        public string GetOperationalStringFromIcalString(string request, string startString, string endString)
        {
            int startPosition = request.IndexOf(startString) + startString.Length;
            int endPosition = request.IndexOf(endString);
            if (endPosition == -1)
            {
                return string.Empty;
            }

            string response = request.Substring(startPosition, endPosition - startPosition);
            response = Regex.Replace(response, @"^\s+$[\n]*", string.Empty, RegexOptions.Multiline, TimeSpan.FromMilliseconds(100));
            return response;
        }

        public TimeZoneDetails FillTimeZoneDetails(string vTimeZoneString)
        {

            try {
                TimeZoneDetails zoneDetails = new();
                //if (!string.IsNullOrEmpty(vTimeZone))
                //{
                //    string timezone = vTimeZone.Split("\n")[0].Split(':')[1];
                //    //string timezone = vTimeZone.Split(new[] { '\r', '\n' })[0];
                //    zoneDetails.Timezone = timezone;
                //}
                StringBuilder strings = new StringBuilder();
                using (StringReader sr = new StringReader(vTimeZoneString))
                {
                    while (sr.Peek() > -1)
                    {
                        var timeParam = sr.ReadLine();
                        if (!string.IsNullOrEmpty(timeParam))
                        {
                            strings.Append(timeParam);
                            strings.Append(Environment.NewLine);
                        }
                    }
                }

                string vTimeZone = strings.ToString();
                vTimeZone = vTimeZone.Replace("\r\n", "\n");
                zoneDetails.Timezone = vTimeZone.Split("\n")[0].Split(':')[1];
                string standardPropertyString = this.GetOperationalStringFromIcalString(vTimeZone, string.Concat(ApplicationConstants.BEGIN, ":", ApplicationConstants.STANDARD), string.Concat(ApplicationConstants.END, ":", ApplicationConstants.STANDARD));
                if (!string.IsNullOrEmpty(standardPropertyString))
                {
                    using (StringReader reader = new (standardPropertyString))
                    {
                        while (reader.Peek() > -1)
                        {
                            var iCalParam = reader.ReadLine();
                            if (!string.IsNullOrEmpty(iCalParam))
                            {
                                if (iCalParam.Contains("DTSTART"))
                                {
                                    if (!string.IsNullOrEmpty(iCalParam.Split(':')[1]))
                                    {
                                        string dtStart = iCalParam.Split(':')[1];
                                        dtStart = dtStart.Replace("T", string.Empty);
                                        zoneDetails.StandardOffSetDate = DateTime.ParseExact(dtStart, "yyyyMMddHHmmss", null);
                                    }
                                }

                                if (iCalParam.Contains("TZOFFSETFROM"))
                                {
                                    string offSetStartFrom = iCalParam.Split(':')[1];
                                    zoneDetails.StandardOffSetStartFrom = offSetStartFrom;
                                }

                                if (iCalParam.Contains("TZOFFSETTO"))
                                {
                                    string offSetStartTo = iCalParam.Split(':')[1];
                                    zoneDetails.StandardOffSetStartTo = offSetStartTo;
                                }

                                if (iCalParam.Contains("TZNAME"))
                                {
                                    string timezoneName = iCalParam.Split(':')[1];
                                    zoneDetails.StandardTimeZoneName = timezoneName;
                                }
                            }
                        }
                    }
                }

                string daylightPropertyString = this.GetOperationalStringFromIcalString(vTimeZone, string.Concat(ApplicationConstants.BEGIN, ":", ApplicationConstants.DAYLIGHT), string.Concat(ApplicationConstants.END, ":", ApplicationConstants.DAYLIGHT));
                if (!string.IsNullOrEmpty(daylightPropertyString))
                {
                    using (StringReader reader = new(daylightPropertyString))
                    {
                        while (reader.Peek() > -1)
                        {
                            var iCalParam = reader.ReadLine();
                            if (!string.IsNullOrEmpty(iCalParam))
                            {
                                if (iCalParam.Contains("DTSTART"))
                                {
                                    if (!string.IsNullOrEmpty(iCalParam.Split(':')[1]))
                                    {
                                        string dtStart = iCalParam.Split(':')[1];
                                        dtStart = dtStart.Replace("T", string.Empty);
                                        zoneDetails.DaylightOffSetDate = DateTime.ParseExact(dtStart, "yyyyMMddHHmmss", null);
                                    }
                                }

                                if (iCalParam.Contains("TZOFFSETFROM"))
                                {
                                    string offSetStartFrom = iCalParam.Split(':')[1];
                                    zoneDetails.DaylightStandardOffSetStartFrom = offSetStartFrom;
                                }

                                if (iCalParam.Contains("TZOFFSETTO"))
                                {
                                    string offSetStartTo = iCalParam.Split(':')[1];
                                    zoneDetails.DaylightStandardOffSetStartTo = offSetStartTo;
                                }

                                if (iCalParam.Contains("TZNAME"))
                                {
                                    string timezoneName = iCalParam.Split(':')[1];
                                    zoneDetails.DaylightTimeZoneName = timezoneName;
                                }
                            }
                        }
                    }
                }

                return zoneDetails;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private RecurrenceModel FillRecurrenceDetails(string recurrenceString)
        {
            RecurrenceModel recurrence = new ();
            string[] param = recurrenceString.Split(':');
            string[] recurrenceParams = param[1].Split(';');
            foreach (var item in recurrenceParams)
            {
                if (item.Contains("FREQ"))
                {
                    recurrence.RecurrenceFrequecny = item.Split('=')[1];
                }

                if (item.Contains("BYDAY"))
                {
                    var value = item.Split('=')[1];
                    if (!Regex.IsMatch(value, @"\d+", RegexOptions.None, TimeSpan.FromMilliseconds(100)))
                    {
                        recurrence.RecurrenceDays = item.Split('=')[1].Split(',');
                    }
                    else
                    {
                        var resultString = Regex.Match(value, @"\d+", RegexOptions.None, TimeSpan.FromMilliseconds(100)).Value;
                        recurrence.DayNumberForMonth = resultString;
                        recurrence.RecurrenceDays = new[] { value.Substring(1, 2) };
                    }
                }

                if (item.Split('=')[0] == "BYMONTH")
                {
                    recurrence.RecurrenceMonth = item.Split('=')[1];
                }

                if (item.Contains("UNTIL"))
                {
                    string until = item.Split('=')[1];
                    until = item.Split('=')[1].Replace("T", string.Empty).ToString().Replace("Z", string.Empty);
                    recurrence.EndingDateOfTheVisit = DateTime.ParseExact(until, "yyyyMMddHHmmss", null);
                }

                if (item.Contains("INTERVAL"))
                {
                    recurrence.RepeatEvery = item.Split('=')[1];
                }

                if (item.Contains("COUNT"))
                {
                    recurrence.NumberOfOccurences = item.Split('=')[1];
                }

                if (item.Split('=')[0] == "BYMONTHDAY")
                {
                    recurrence.RepeatDayNumberOfMonth = item.Split('=')[1];
                }

                if (item.Contains("WKST"))
                {
                    recurrence.StartingDayOfTheWeek = item.Split('=')[1];
                }
            }

            return recurrence;
        }

        //private NotificationModel FillVAlarmDetails(string vAlarmString)
        //{
        //    var notification = new NotificationModel();
        //    using (StringReader reader = new StringReader(vAlarmString))
        //    {
        //        while (reader.Peek() > -1)
        //        {
        //            var iCalParam = reader.ReadLine();
        //            if (!string.IsNullOrEmpty(iCalParam))
        //            {
        //                if (iCalParam.Contains("DESCRIPTION"))
        //                {
        //                    string description = iCalParam.Split(':')[1];
        //                    if (!string.IsNullOrEmpty(description))
        //                    {
        //                        notification.NotificationDescription = description;
        //                    }
        //                }

        //                if (iCalParam.Contains("TRIGGER"))
        //                {
        //                    string param = iCalParam.Split('-')[1];
        //                    var resultString = Regex.Match(param, @"\d+").Value;
        //                    notification.NotificationTriggerTime = resultString;
        //                    string lastChar = param.Substring(param.Length - 1);
        //                    notification.NotificationDuration = lastChar;
        //                }

        //                if (iCalParam.Contains("ACTION"))
        //                {
        //                    string action = iCalParam.Split(':')[1];
        //                    if (!string.IsNullOrEmpty(action))
        //                    {
        //                        notification.Action = action;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return notification;
        //}

        private VisitorModel FillVisitorInputs(List<string> input)
        {
            var visitor = new VisitorModel();
            string inputstring = string.Join("\n", input);
            using (StringReader reader = new (inputstring))
            {
                while (reader.Peek() > -1)
                {
                    var iCalParam = reader.ReadToEnd();
                    if (!string.IsNullOrEmpty(iCalParam))
                    {
                        string[] param = iCalParam.Split(';');
                        foreach (var item in param)
                        {
                            var newItem = item.Replace("\n ", string.Empty);
                            if (newItem.Contains("CUTYPE"))
                            {
                                string value = item.Split('=')[1];
                                visitor.TypeOfVisitor = value;
                            }

                            if (newItem.Contains("ROLE"))
                            {
                                string value = item.Split('=')[1];
                                visitor.Role = value;
                            }

                            if (newItem.Contains("PARSTAT"))
                            {
                                string value = item.Split('=')[1];
                                visitor.ParticipationStatus = value;
                            }

                            if (newItem.Contains("RSVP"))
                            {
                                string value = item.Split('=')[1];
                                visitor.ResponseRequired = Convert.ToBoolean(value);
                            }

                            //if (newItem.Contains("X-KASTLE-CARDHOLDERGUID"))
                            //{
                            //    string value = item.Split('=')[1];
                            //    visitor.CardholderGuid = Guid.Parse(value);
                            //}

                            //if (newItem.Contains("TEL"))
                            //{
                            //    string value = item.Split('=')[1];
                            //    visitor.MobileNumber = value.Replace("\n ", string.Empty);
                            //}

                            //if (newItem.Contains("X-KASTLE-COUNTRYCODE"))
                            //{
                            //    string value = item.Split('=')[1];
                            //    visitor.MobileNumberCountryPrifix = value.Replace("\n", string.Empty);
                            //}

                            //if (newItem.Contains("X-KASTLE-COUNTRYID"))
                            //{
                            //    string value = item.Split('=')[1];
                            //    visitor.CountryId = value.Replace("\n", string.Empty);
                            //}

                            //if (newItem.Contains("X-KASTLE-VISITORCOMPANY"))
                            //{
                            //    string value = item.Split('=')[1];
                            //    visitor.VisitorCompany = value.Replace("\n", string.Empty).Trim();
                            //}

                            //if (newItem.Contains("X-KASTLE-FN"))
                            //{
                            //    string value = item.Split('=')[1];
                            //    visitor.FirstName = value.Replace("\n ", string.Empty).Replace("\n ", string.Empty).Trim();
                            //}

                            //if (newItem.Contains("X-KASTLE-LN"))
                            //{
                            //    string value = item.Split('=')[1];
                            //    visitor.LastName = value.Replace("\n ", string.Empty).Replace("\n ", string.Empty).Trim();
                            //}

                            //if (newItem.Contains("X-KASTLE-MAILID"))
                            //{
                            //    string value = item.Split('=')[1];
                            //    visitor.Email = value.Replace("\n", string.Empty).Replace("\n ", string.Empty).Replace(" ", string.Empty).Trim();
                            //}

                            if (newItem.Contains("CN"))
                            {
                                string name = item.Split('=')[1].Split(':')[0].Replace("\n ", string.Empty);
                                string[] value = item.Split(':');
                                for (int i = 0; i < value.Length; i++)
                                {
                                    if (value[i].Replace("\n ", string.Empty) == "Tel")
                                    {
                                        visitor.MobileNumber = value[i + 1].Replace("\n ", string.Empty).Trim();
                                    }
                                    else if (value[i].Replace("\n ", string.Empty) == "X-KASTLE-CARDHOLDERGUID")
                                    {
                                        visitor.CardholderGuid = Guid.Parse(value[i + 1].Replace("\n ", string.Empty).Trim());
                                    }
                                    else if (value[i].Replace("\n ", string.Empty) == "mailto")
                                    {
                                        visitor.Email = value[i + 1].Replace("\n ", string.Empty).Trim();
                                    }
                                    else if (value[i].Replace("\n ", string.Empty) == "X-KASTLE-COUNTRYCODE")
                                    {
                                        visitor.MobileNumberCountryPrifix = value[i + 1].Replace("\n ", string.Empty).Trim();
                                    }
                                    else if (value[i].Replace("\n ", string.Empty) == "X-KASTLE-COUNTRYID")
                                    {
                                        visitor.CountryId = value[i + 1].Replace("\n ", string.Empty).Trim();
                                    }
                                    else if (value[i].Replace("\n ", string.Empty) == "X-KASTLE-VISITORCOMPANY")
                                    {
                                        visitor.VisitorCompany = value[i + 1].Replace("\n ", string.Empty).Trim();
                                    }
                                    else if (value[i].Replace("\n ", string.Empty) == "X-KASTLE-FN")
                                    {
                                        visitor.FirstName = value[i + 1].Replace("\n ", string.Empty).Trim();
                                    }
                                    else if (value[i].Replace("\n ", string.Empty) == "X-KASTLE-LN")
                                    {
                                        visitor.LastName = value[i + 1].Replace("\n ", string.Empty).Trim();
                                    }
                                    else if (value[i].Replace("\n ", string.Empty) == "X-KASTLE-MAILID")
                                    {
                                        visitor.Email = value[i + 1].Replace("\n ", string.Empty).Trim();
                                    }
                                }

                                string[] validName = name.Split(' ');
                                if (validName.Length == 2)
                                {
                                    visitor.FirstName = validName[0];
                                    visitor.LastName = validName[validName.Length - 1];
                                }
                                else if (validName.Length > 2)
                                {
                                    visitor.FirstName = string.Join(" ", validName.Take(2)).Replace("\n", string.Empty).Replace("\n ", string.Empty).Trim();
                                    visitor.LastName = string.Join(" ", validName.Skip(2)).Replace("\n", string.Empty).Replace("\n ", string.Empty).Trim();
                                }
                                else if (validName.Length == 1)
                                {
                                    visitor.LastName = validName[0].Replace("\n ", string.Empty);
                                }

                                //if (this.IsValidEmail(validName[0]))
                                //{
                                //    visitor.Email = validName[0];
                                //}
                                //else
                                //{
                                //    visitor.Email = value[2].Replace("\n ", string.Empty);
                                //}
                            }
                        }
                    }
                }
            }

            return visitor;
        }

        private bool IsValidEmail(string mail)
        {
            bool isValidEmail = Regex.IsMatch(mail, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            return isValidEmail;
        }

        private VisitingCompanyDetails FillVVenueDetails(string vVenueString)
        {
            VisitingCompanyDetails visitingCompanyDetails = new ();
            using (StringReader reader = new StringReader(vVenueString))
            {
                while (reader.Peek() > -1)
                {
                    var iCalParam = reader.ReadLine();
                    if (!string.IsNullOrEmpty(iCalParam))
                    {
                        if (iCalParam.Contains("UID"))
                        {
                            string uID = iCalParam.Split(':')[1];
                            if (!string.IsNullOrEmpty(uID))
                            {
                                visitingCompanyDetails.UID = Guid.Parse(uID);
                            }
                        }

                        if (iCalParam.Contains("NAME"))
                        {
                            string name = iCalParam.Split(':')[1];
                            if (!string.IsNullOrEmpty(name))
                            {
                                visitingCompanyDetails.Name = name;
                            }
                        }

                        if (iCalParam.Contains("STREET-ADDRESS"))
                        {
                            string streetAddress = iCalParam.Split(':')[1];
                            if (!string.IsNullOrEmpty(streetAddress))
                            {
                                visitingCompanyDetails.StreetAddress = streetAddress;
                            }
                        }
                    }
                }
            }

            return visitingCompanyDetails;
        }

        private List<VisitorModel> GetVisitorModels(string vEventString)
        {
            bool isAttendee = false;
            List<string> listAttendee = new List<string>();
            var visitors = new List<VisitorModel>();

            using (StringReader red = new (vEventString))
            {
                while (red.Peek() > -1)
                {
                    var iCalParam = red.ReadLine();
                    if (!string.IsNullOrEmpty(iCalParam))
                    {
                        if (isAttendee)
                        {
                            if (!ParameterList.ParamList.Any(s => iCalParam.StartsWith(s)))
                            {
                                listAttendee.Add(iCalParam);
                            }
                            else
                            {
                                var visitorDet = this.FillVisitorInputs(listAttendee);
                                visitors.Add(visitorDet);
                                listAttendee = new List<string>();
                                isAttendee = false;
                            }
                        }

                        if (iCalParam.StartsWith("ATTENDEE"))
                        {
                            listAttendee.Add(iCalParam);
                            isAttendee = true;
                        }
                    }
                }
            }

            return visitors;
        }

        private List<string> GetDynamicParamString(string vEventString, string property)
        {
            var outputStr = new List<string>();
            bool isParamAvailable = false;
            using (StringReader red = new (vEventString))
            {
                while (red.Peek() > -1)
                {
                    var iCalParam = red.ReadLine();
                    if (!string.IsNullOrEmpty(iCalParam))
                    {
                        if (isParamAvailable)
                        {
                            if (!ParameterList.ParamList.Any(s => iCalParam.StartsWith(s)))
                            {
                                outputStr.Add(iCalParam.Trim());
                            }
                            else
                            {
                                isParamAvailable = false;
                            }
                        }

                        if (iCalParam.StartsWith(property))
                        {
                            outputStr.Add(iCalParam.Trim());
                            isParamAvailable = true;
                        }
                    }
                }
            }

            return outputStr;
        }

        private string Extract(string str)
        {
            bool end = false;
            int length = 0;
            foreach (char c in str)
            {
                if (c == ' ' && end == false)
                {
                    end = true;
                }
                else if (c == ' ' && end == true)
                {
                    break;
                }

                length++;
            }

            return str.Substring(0, length);
        }
    }
}
