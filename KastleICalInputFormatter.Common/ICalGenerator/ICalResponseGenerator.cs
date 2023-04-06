// <copyright file="ICalResponseGenerator.cs" company="Kastle Systems">
// Copyright (c) Kastle Systems. All rights reserved.
// </copyright>

namespace KastleICalInputFormatter.Common.ICalGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using KastleICalInputFormatter.Common.ApplicationModel;
    using KastleICalInputFormatter.Common.ApplicationModel.ICalEntityModel;
    using KastleICalInputFormatter.Common.ApplicationModel.VisitorEntityModel;

    /// <summary>
    /// Class for Ical response.
    /// </summary>
    public class ICalResponseGenerator : IICalResponseGenerator
    {
        /// <summary>
        /// Method to generate IcalModel from UI Provided Visitor request Model.
        /// </summary>
        /// <param name="visitorRequestModel">visitorRequestModel.</param>
        /// <returns>IcalModel.</returns>
        public ICalModel GenerateICalResponseFromVisitorModel(VisitorRequestModel visitorRequestModel)
        {
            ICalModel calModel = new ();
            string dtStart, dtEnd = string.Empty;
            bool isValidArrivalTime = TimeSpan.TryParse(visitorRequestModel.ArrivalTime, out _);
            bool isValidDepartureTime = TimeSpan.TryParse(visitorRequestModel.DepartureTime, out _);
            if (!isValidArrivalTime || !isValidDepartureTime)
            {
                throw new ArgumentException("Invalid format of arrival or departure time.");
            }

            var arrivalDateTime = Convert.ToDateTime(visitorRequestModel.ArrivalDate.Date.Add(TimeOnly.Parse(visitorRequestModel.ArrivalTime).ToTimeSpan()));
            var departureDateTime = Convert.ToDateTime(visitorRequestModel.DepartureDate.Date.Add(TimeOnly.Parse(visitorRequestModel.DepartureTime).ToTimeSpan()));

            DateTime arrivalUTCDateTime = this.ConvertToUTC(arrivalDateTime, visitorRequestModel.Timezone);
            DateTime departureUTCDateTime = this.ConvertToUTC(departureDateTime, visitorRequestModel.Timezone);

            if (visitorRequestModel.DepartureDate.Date > visitorRequestModel.ArrivalDate.Date)
            {
                dtStart = Convert.ToDateTime(visitorRequestModel.ArrivalDate.Date.Add(TimeOnly.Parse(visitorRequestModel.ArrivalTime).ToTimeSpan())).ToString("yyyyMMddTHHmmss");
                dtEnd = Convert.ToDateTime(visitorRequestModel.DepartureDate/*.AddDays(1)*/.Date.Add(TimeOnly.Parse(visitorRequestModel.DepartureTime).ToTimeSpan())).ToString("yyyyMMddTHHmmss");
            }
            else if (visitorRequestModel.DepartureDate.Date == visitorRequestModel.ArrivalDate.Date)
            {
                if (TimeOnly.Parse(visitorRequestModel.DepartureTime) > TimeOnly.Parse(visitorRequestModel.ArrivalTime))
                {
                    dtStart = Convert.ToDateTime(visitorRequestModel.ArrivalDate.Date.Add(TimeOnly.Parse(visitorRequestModel.ArrivalTime).ToTimeSpan())).ToString("yyyyMMddTHHmmss");
                    dtEnd = Convert.ToDateTime(visitorRequestModel.DepartureDate/*.AddDays(1)*/.Date.Add(TimeOnly.Parse(visitorRequestModel.DepartureTime).ToTimeSpan())).ToString("yyyyMMddTHHmmss");
                }
                else
                {
                    throw new ArgumentException("Departure time must be greater than arrival time for same arrival and departure date.");
                }
            }
            else
            {
                throw new ArgumentException("Departure date must be greater than arrival date");
            }

            VCalender vCalender = new ();

            // fill standard properties of iCal.
            Standard_Properties standardProperties = new ();
            standardProperties.Method = ApplicationConstants.REQUEST;

            standardProperties.ProdID = visitorRequestModel.ProdID;

            standardProperties.Version = ApplicationConstants.VERSION;

            vCalender.Standard_Properties = standardProperties;

            // fill timezone properties of iCal.
            if (visitorRequestModel.TimeZoneDetails != null)
            {
                vCalender.VTimezone = this.GetTimeZoneDetails(visitorRequestModel);
            }

            // fill VEvent properties.
            VEvent vEvent = new ();
            DtStart vEventDtStart = new ()
            {
                DateTime = dtStart,
                Tzid = visitorRequestModel.Timezone,
            };
            vEvent.DtStart = vEventDtStart;

            DtEnd vEventDtEnd = new ()
            {
                Tzid = visitorRequestModel.Timezone,
                DateTime = dtEnd,
            };
            vEvent.DtEnd = vEventDtEnd;

            if (visitorRequestModel.RecurrenceModel != null)
            {
                vEvent.Rrule = this.GetRruleDetails(visitorRequestModel);
            }

            if (visitorRequestModel.CreatedOn != null)
            {
                var creationDate = Convert.ToDateTime(visitorRequestModel.CreatedOn).ToString("yyyyMMddTHHmmssZ");
                vEvent.DtStamp = creationDate;
            }
            else
            {
                vEvent.DtStamp = DateTime.Now.ToString("yyyyMMddTHHmmssZ");
            }

            var createDate = Convert.ToDateTime(visitorRequestModel.CreationDate).ToString("yyyyMMddTHHmmssZ");
            vEvent.Created = createDate;

            Organizer organizer = new ();
            if (!string.IsNullOrEmpty(visitorRequestModel.UserLastName))
            {
                if (string.IsNullOrEmpty(visitorRequestModel.UserFirstName))
                {
                    bool isValidUserLastName = this.IsValidName(visitorRequestModel.UserLastName.Trim());
                    if (isValidUserLastName)
                    {
                        if (visitorRequestModel.UserLastName.Trim().Length <= 40)
                        {
                            organizer.Cn = visitorRequestModel.UserLastName.Trim();
                        }
                        else
                        {
                            throw new ArgumentException("UserLastName cannot be more than 40 characters long.");
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(visitorRequestModel.UserFirstName))
                {
                    bool isValidFirstName = this.IsValidName(visitorRequestModel.UserFirstName.Trim());
                    bool isValidLastName = this.IsValidName(visitorRequestModel.UserLastName.Trim());
                    if (isValidFirstName && isValidLastName)
                    {
                        if (visitorRequestModel.UserFirstName.Length <= 40 && visitorRequestModel.UserLastName.Length <= 40)
                        {
                            organizer.Cn = string.Format("{0} {1}", visitorRequestModel.UserFirstName.Trim(), visitorRequestModel.UserLastName.Trim());
                        }
                        else
                        {
                            throw new ArgumentException("UserFirstName or UserLastName length must be less than or equal to 40 characters.");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Inavlid UserFirst or UserLastName provided.");
                    }
                }
            }
            else
            {
                throw new ArgumentNullException("UserLastName cannot be null or empty.");
            }

            organizer.MailTo = visitorRequestModel.UserEmail;

            vEvent.Organizer = organizer;

            if (visitorRequestModel.VisitGuid != Guid.Empty && visitorRequestModel.VisitGuid != null)
            {
                vEvent.UID = (Guid)visitorRequestModel.VisitGuid;
            }
            else
            {
                Guid uIDGuid = Guid.NewGuid();
                vEvent.UID = uIDGuid;
            }

            // fill attendee details.
            if (visitorRequestModel.VisitorModel != null)
            {
                vEvent.Attendees = this.GetVisitorsDetails(visitorRequestModel.VisitorModel);
            }
            else
            {
                throw new ArgumentNullException("Visitors cannot be null.");
            }

            if (!string.IsNullOrEmpty(visitorRequestModel.Description))
            {
                vEvent.Description = visitorRequestModel.Description;
            }

            if (!string.IsNullOrEmpty(visitorRequestModel.Summary))
            {
                vEvent.Summary = visitorRequestModel.Summary;
            }

            if (this.IsIntegerValue(visitorRequestModel.PriorityLevel.ToString()))
            {
                if (Enumerable.Range(0, 9).Contains(visitorRequestModel.PriorityLevel))
                {
                    vEvent.Priority = visitorRequestModel.PriorityLevel.ToString();
                }
                else
                {
                    throw new ArgumentException("PriorityLevel can only be between 0 and 9.");
                }
            }
            else
            {
                throw new ArgumentException("PriorityLevel cannot be string. It should be number.");
            }

            if (!string.IsNullOrEmpty(visitorRequestModel.Transparency))
            {
                vEvent.Transp = visitorRequestModel.Transparency;
            }

            if (!string.IsNullOrEmpty(visitorRequestModel.MeetingStatus))
            {
                vEvent.Status = visitorRequestModel.MeetingStatus;
            }

            if (this.IsIntegerValue(visitorRequestModel.NumberOfTimeMeetingModified.ToString()))
            {
                vEvent.Sequence = visitorRequestModel.NumberOfTimeMeetingModified.ToString();
            }
            else
            {
                throw new ArgumentException("NumberOfTimeMeetingModified cannot be string. It should be number.");
            }

            if (visitorRequestModel.Location != null)
            {
                vEvent.Location = visitorRequestModel.Location;
            }

            if (!string.IsNullOrEmpty(visitorRequestModel.FloorId))
            {
                if (this.IsIntegerValue(visitorRequestModel.FloorId))
                {
                    vEvent.XKastleFloorId = visitorRequestModel.FloorId;
                }
                else
                {
                    throw new ArgumentException("FloorId cannot be string. It should be a valid integer.");
                }
            }
            else
            {
                throw new ArgumentNullException("FloorID Cannot be null or empty.");
            }

            if (visitorRequestModel.InstitutionID != null && this.IsIntegerValue(visitorRequestModel.InstitutionID) && visitorRequestModel.InstitutionID != "0")
            {
                vEvent.XKastleInstitutionID = visitorRequestModel.InstitutionID;
            }
            else
            {
                throw new ArgumentNullException("Please provide valid Institution Id.");
            }

            if (!string.IsNullOrEmpty(visitorRequestModel.IntendedStatus))
            {
                vEvent.XMicrosoftCdoIntendedStatus = visitorRequestModel.IntendedStatus;
            }

            vEvent.XMicrosoftCdoAllDayEvent = visitorRequestModel.IsAllDayEvent.ToString();
            vEvent.XKastleIdentityType = visitorRequestModel.IdentityType;

            if (!string.IsNullOrEmpty(visitorRequestModel.UniqueAppointmentIdentifier))
            {
                vEvent.XMicrosoftCdoOwnerApptID = visitorRequestModel.UniqueAppointmentIdentifier;
            }

            // Commenting notification model use as this is not currently in use. Only email notifications are there. For future it can be used.
            //VAlarm vALARM = new ();
            //if (visitorRequestModel.NotificationModel != null)
            //{
            //    if (visitorRequestModel.NotificationModel.NotificationTriggerTime != null && visitorRequestModel.NotificationModel.NotificationDuration != null)
            //    {
            //        if (visitorRequestModel.NotificationModel.NotificationDuration == "Days")
            //        {
            //            vALARM.Trigger = string.Concat("-", "P", visitorRequestModel.NotificationModel.NotificationTriggerTime, visitorRequestModel.NotificationModel.NotificationDuration.Substring(0, 1));
            //        }
            //        else
            //        {
            //            vALARM.Trigger = string.Concat("-", "PT", visitorRequestModel.NotificationModel.NotificationTriggerTime, visitorRequestModel.NotificationModel.NotificationDuration.Substring(0, 1));
            //        }
            //    }
            //    else
            //    {
            //        throw new ArgumentNullException("Trigger time or duration cannot be null");
            //    }

            //    if (!string.IsNullOrEmpty(visitorRequestModel.NotificationModel.Action))
            //    {
            //        vALARM.Action = visitorRequestModel.NotificationModel.Action;
            //    }

            //    if (!string.IsNullOrEmpty(visitorRequestModel.NotificationModel.NotificationDescription))
            //    {
            //        vALARM.Description = visitorRequestModel.NotificationModel.NotificationDescription;
            //    }

            //    vEvent.VAlarm = vALARM;
            //}

            if (visitorRequestModel.LastModified != null)
            {
                vEvent.LastModified = Convert.ToDateTime(visitorRequestModel.LastModified).ToString("yyyyMMddTHHmmssZ");
            }
            else
            {
                vEvent.LastModified = DateTime.Now.ToString("yyyyMMddTHHmmssZ");
            }

            if (visitorRequestModel.AccessProfiles != null)
            {
                vEvent.XkastleAccessProfiles = visitorRequestModel.AccessProfiles;
            }

            if (!string.IsNullOrEmpty(visitorRequestModel.Notes))
            {
                vEvent.XKastleNotes = visitorRequestModel.Notes;
            }

            if (!string.IsNullOrEmpty(visitorRequestModel.SpecialInstructions))
            {
                vEvent.XKastleSpecialInstructions = visitorRequestModel.SpecialInstructions;
            }

            if (!string.IsNullOrEmpty(visitorRequestModel.SuiteInfo))
            {
                vEvent.XKastleSuite = visitorRequestModel.SuiteInfo;
            }

            if (visitorRequestModel.IsCommunityLevel != null)
            {
                if ((bool)visitorRequestModel.IsCommunityLevel)
                {
                    vEvent.XKastleKMFInfo = ((int)ApplicationConstants.KMFFlag.CommunityLevel).ToString();
                }
                else
                {
                    vEvent.XKastleKMFInfo = ((int)ApplicationConstants.KMFFlag.ApartmentLevel).ToString();
                }
            }

            if (!string.IsNullOrEmpty(visitorRequestModel.IsSendMailToVisitors.ToString()))
            {
                vEvent.XKastleIsSendMailToVisitors = visitorRequestModel.IsSendMailToVisitors.ToString();
            }

            if (!string.IsNullOrEmpty(visitorRequestModel.NotifyOnArrival.ToString()))
            {
                vEvent.XKastleNotifyOnArrival = visitorRequestModel.NotifyOnArrival.ToString();
            }

            vEvent.XKastleUTCArrivalDate = arrivalUTCDateTime.ToString("yyyy-MM-dd");
            vEvent.XKastleUTCArrivalTime = arrivalUTCDateTime.ToString("HH:mm", CultureInfo.InvariantCulture);
            vEvent.XKastleUTCDepartureDate = departureUTCDateTime.ToString("yyyy-MM-dd");
            vEvent.XKastleUTCDepartureTime = departureUTCDateTime.ToString("HH:mm", CultureInfo.InvariantCulture);

            vCalender.VEvent = vEvent;

            VVenue visitingCompanyDetails = new ();
            if (visitorRequestModel.VisitingCompanyDetails != null)
            {
                visitingCompanyDetails = this.GetVistingCompanyDetails(visitorRequestModel.VisitingCompanyDetails);
                vCalender.VVenue = visitingCompanyDetails;
            }

            calModel.VCalender = vCalender;
            return calModel;
        }

        /// <summary>
        /// Method to Convert Ical Model to Ical string.
        /// </summary>
        /// <param name="calModel">Ical model.</param>
        /// <returns>Ical string.</returns>
        public string ConvertToIcalString(ICalModel calModel)
        {
            try
            {
                StringBuilder iCalStringBuilder = new ();

                // Create iCal string for Standard properties.
                iCalStringBuilder.Append(string.Concat("BEGIN:", ApplicationConstants.VCALENDER));
                iCalStringBuilder.Append(Environment.NewLine);
                iCalStringBuilder.Append(string.Concat("METHOD:", calModel.VCalender.Standard_Properties.Method));
                iCalStringBuilder.Append(Environment.NewLine);
                iCalStringBuilder.Append(string.Concat("PRODID:", calModel.VCalender.Standard_Properties.ProdID));
                iCalStringBuilder.Append(Environment.NewLine);
                iCalStringBuilder.Append(string.Concat("VERSION:", calModel.VCalender.Standard_Properties.Version));
                iCalStringBuilder.Append(Environment.NewLine);
                // create iCal string for VTimezone Properties.
                if (calModel.VCalender.VTimezone != null)
                {
                    iCalStringBuilder.Append(string.Concat("BEGIN:", ApplicationConstants.VTIMEZONE));
                    iCalStringBuilder.Append(Environment.NewLine);
                    iCalStringBuilder.Append(string.Concat("TZID:", calModel.VCalender.VTimezone.Tzid));
                    iCalStringBuilder.Append(Environment.NewLine);
                    iCalStringBuilder.Append(string.Concat("BEGIN:", ApplicationConstants.STANDARD));
                    iCalStringBuilder.Append(Environment.NewLine);
                    iCalStringBuilder.Append(string.Concat("DTSTART:", calModel.VCalender.VTimezone.Standard.DtStart.DateTime));
                    iCalStringBuilder.Append(Environment.NewLine);
                    iCalStringBuilder.Append(string.Concat("TZOFFSETFROM:", calModel.VCalender.VTimezone.Standard.TzOffSetFrom));
                    iCalStringBuilder.Append(Environment.NewLine);
                    iCalStringBuilder.Append(string.Concat("TZOFFSETTO:", calModel.VCalender.VTimezone.Standard.TzOffSetTo));
                    iCalStringBuilder.Append(Environment.NewLine);
                    if (!string.IsNullOrEmpty(calModel.VCalender.VTimezone.Standard.TzName))
                    {
                        iCalStringBuilder.Append(string.Concat("TZNAME:", calModel.VCalender.VTimezone.Standard.TzName));
                    }

                    iCalStringBuilder.Append(Environment.NewLine);
                    iCalStringBuilder.Append(string.Concat("END:", ApplicationConstants.STANDARD));
                    iCalStringBuilder.Append(Environment.NewLine);
                    if (calModel.VCalender.VTimezone.Daylight != null)
                    {
                        iCalStringBuilder.Append(string.Concat("BEGIN:", ApplicationConstants.DAYLIGHT));
                        iCalStringBuilder.Append(Environment.NewLine);
                        iCalStringBuilder.Append(string.Concat("DTSTART:", calModel.VCalender.VTimezone.Daylight.DtStart.DateTime));
                        iCalStringBuilder.Append(Environment.NewLine);
                        iCalStringBuilder.Append(string.Concat("TZOFFSETFROM:", calModel.VCalender.VTimezone.Daylight.TzOffSetFrom));
                        iCalStringBuilder.Append(Environment.NewLine);
                        iCalStringBuilder.Append(string.Concat("TZOFFSETTO:", calModel.VCalender.VTimezone.Daylight.TzOffSetTo));
                        iCalStringBuilder.Append(Environment.NewLine);
                        if (!string.IsNullOrEmpty(calModel.VCalender.VTimezone.Daylight.TzName))
                        {
                            iCalStringBuilder.Append(string.Concat("TZNAME:", calModel.VCalender.VTimezone.Daylight.TzName));
                            iCalStringBuilder.Append(Environment.NewLine);
                        }

                        iCalStringBuilder.Append(string.Concat("END:", ApplicationConstants.DAYLIGHT));
                        iCalStringBuilder.Append(Environment.NewLine);
                    }

                    iCalStringBuilder.Append(string.Concat("END:", ApplicationConstants.VTIMEZONE));
                    iCalStringBuilder.Append(Environment.NewLine);
                }

                // Create iCal string for VEvent Properties.
                iCalStringBuilder.Append(string.Concat("BEGIN:", ApplicationConstants.VEVENT));
                iCalStringBuilder.Append(Environment.NewLine);
                iCalStringBuilder.Append(string.Concat("DTSTART;TZID=", calModel.VCalender.VEvent.DtStart.Tzid, ":", calModel.VCalender.VEvent.DtStart.DateTime));
                iCalStringBuilder.Append(Environment.NewLine);
                iCalStringBuilder.Append(string.Concat("DTEND;TZID=", calModel.VCalender.VEvent.DtEnd.Tzid + ":", calModel.VCalender.VEvent.DtEnd.DateTime));
                iCalStringBuilder.Append(Environment.NewLine);
                if (calModel.VCalender.VEvent.Rrule != null)
                {
                    iCalStringBuilder.Append(this.CreateRruleString(calModel.VCalender.VEvent.Rrule));
                    iCalStringBuilder.Append(Environment.NewLine);
                }

                iCalStringBuilder.Append(string.Concat("DTSTAMP:", calModel.VCalender.VEvent.DtStamp));
                iCalStringBuilder.Append(Environment.NewLine);
                if (calModel.VCalender.VEvent.Organizer != null)
                {
                    StringBuilder orgStringBuilder = new ();
                    int countOrg = 0;
                    var orgString = this.SplitString(string.Concat("ORGANIZER;CN=", calModel.VCalender.VEvent.Organizer.Cn, ":mailto:", calModel.VCalender.VEvent.Organizer.MailTo));
                    foreach (var org in orgString)
                    {
                        if (countOrg == 1)
                        {
                            orgStringBuilder.Append(string.Concat(" ", org));
                            orgStringBuilder.Append(Environment.NewLine);
                        }
                        else if (countOrg == 2)
                        {
                            orgStringBuilder.Append(string.Concat(" ", org.Trim()));
                            orgStringBuilder.Append(Environment.NewLine);
                        }
                        else
                        {
                            orgStringBuilder.Append(org);
                            orgStringBuilder.Append(Environment.NewLine);
                        }

                        countOrg++;
                    }

                    iCalStringBuilder.Append(orgStringBuilder);
                }

                iCalStringBuilder.Append(string.Concat("UID:", calModel.VCalender.VEvent.UID));
                iCalStringBuilder.Append(Environment.NewLine);
                StringBuilder attendeeStringBuilder = new ();

                foreach (var attendee in calModel.VCalender.VEvent.Attendees)
                {
                    var attendeeString = this.SplitString(this.CreateAttendeeString(attendee));
                    attendeeStringBuilder.Append(this.CreateSplitString(attendeeString));
                }

                iCalStringBuilder.Append(attendeeStringBuilder.ToString());
                iCalStringBuilder.Append(string.Concat("CREATED:", calModel.VCalender.VEvent.Created));
                iCalStringBuilder.Append(Environment.NewLine);
                iCalStringBuilder.Append(string.Concat("LAST-MODIFIED:", calModel.VCalender.VEvent.LastModified));
                iCalStringBuilder.Append(Environment.NewLine);
                if (!string.IsNullOrEmpty(calModel.VCalender.VEvent.Description))
                {
                    var description = this.SplitString(string.Concat("DESCRIPTION:", calModel.VCalender.VEvent.Description));
                    iCalStringBuilder.Append(this.CreateSplitString(description));
                }

                iCalStringBuilder.Append(string.Concat("SUMMARY:", calModel.VCalender.VEvent.Summary));
                iCalStringBuilder.Append(Environment.NewLine);

                if (!string.IsNullOrEmpty(calModel.VCalender.VEvent.Location))
                {
                    iCalStringBuilder.Append(string.Concat("LOCATION:", calModel.VCalender.VEvent.Location));
                    iCalStringBuilder.Append(Environment.NewLine);
                }

                if (!string.IsNullOrEmpty(calModel.VCalender.VEvent.Class))
                {
                    iCalStringBuilder.Append(string.Concat("CLASS:", calModel.VCalender.VEvent.Class));
                    iCalStringBuilder.Append(Environment.NewLine);
                }

                iCalStringBuilder.Append(string.Concat("PRIORITY:", calModel.VCalender.VEvent.Priority));
                iCalStringBuilder.Append(Environment.NewLine);

                if (!string.IsNullOrEmpty(calModel.VCalender.VEvent.Transp))
                {
                    iCalStringBuilder.Append(string.Concat("TRANSP:", calModel.VCalender.VEvent.Transp));
                    iCalStringBuilder.Append(Environment.NewLine);
                }

                if (!string.IsNullOrEmpty(calModel.VCalender.VEvent.Status))
                {
                    iCalStringBuilder.Append(string.Concat("STATUS:", calModel.VCalender.VEvent.Status));
                    iCalStringBuilder.Append(Environment.NewLine);
                }

                iCalStringBuilder.Append(string.Concat("SEQUENCE:", calModel.VCalender.VEvent.Sequence));
                iCalStringBuilder.Append(Environment.NewLine);

                if (calModel.VCalender.VEvent.XkastleAccessProfiles != null)
                {
                    foreach (var accessProfiles in calModel.VCalender.VEvent.XkastleAccessProfiles)
                    {
                        iCalStringBuilder.Append(string.Concat("X-KASTLE-ACCESSPROFILES;ACCESSPROFILEID:", accessProfiles));
                        iCalStringBuilder.Append(Environment.NewLine);
                    }
                }

                iCalStringBuilder.Append(string.Concat("X-KASTLE-INSTITUTIONID:", calModel.VCalender.VEvent.XKastleInstitutionID));
                iCalStringBuilder.Append(Environment.NewLine);
                iCalStringBuilder.Append(string.Concat("X-KASTLE-IDENTITYTYPE:", calModel.VCalender.VEvent.XKastleIdentityType));
                iCalStringBuilder.Append(Environment.NewLine);

                if (!string.IsNullOrEmpty(calModel.VCalender.VEvent.XMicrosoftCdoOwnerApptID))
                {
                    iCalStringBuilder.Append(string.Concat("X-MICROSOFT-CDO-OWNERAPPTID:", calModel.VCalender.VEvent.XMicrosoftCdoOwnerApptID));
                    iCalStringBuilder.Append(Environment.NewLine);
                }

                if (!string.IsNullOrEmpty(calModel.VCalender.VEvent.XMicrosoftCdoApptSequence))
                {
                    iCalStringBuilder.Append(string.Concat("X-MICROSOFT-CDO-APPT-SEQUENCE:", calModel.VCalender.VEvent.XMicrosoftCdoApptSequence));
                    iCalStringBuilder.Append(Environment.NewLine);
                }

                if (!string.IsNullOrEmpty(calModel.VCalender.VEvent.XMicrosoftCdoBusyStatus))
                {
                    iCalStringBuilder.Append(string.Concat("X-MICROSOFT-CDO-BUSYSTATUS:", calModel.VCalender.VEvent.XMicrosoftCdoBusyStatus));
                    iCalStringBuilder.Append(Environment.NewLine);
                }

                if (!string.IsNullOrEmpty(calModel.VCalender.VEvent.XMicrosoftCdoIntendedStatus))
                {
                    iCalStringBuilder.Append(string.Concat("X-MICROSOFT-CDO-INTENDEDSTATUS:", calModel.VCalender.VEvent.XMicrosoftCdoIntendedStatus));
                    iCalStringBuilder.Append(Environment.NewLine);
                }

                if (!string.IsNullOrEmpty(calModel.VCalender.VEvent.XMicrosoftCdoAllDayEvent))
                {
                    iCalStringBuilder.Append(string.Concat("X-MICROSOFT-CDO-ALLDAYEVENT:", calModel.VCalender.VEvent.XMicrosoftCdoAllDayEvent));
                    iCalStringBuilder.Append(Environment.NewLine);
                }

                if (!string.IsNullOrEmpty(calModel.VCalender.VEvent.XMicrosoftCdoImportance))
                {
                    iCalStringBuilder.Append(string.Concat("X-MICROSOFT-CDO-IMPORTANCE:", calModel.VCalender.VEvent.XMicrosoftCdoImportance));
                    iCalStringBuilder.Append(Environment.NewLine);

                }

                if (!string.IsNullOrEmpty(calModel.VCalender.VEvent.XMicrosoftCdoInstType))
                {
                    iCalStringBuilder.Append(string.Concat("X-MICROSOFT-CDO-INSTTYPE:", calModel.VCalender.VEvent.XMicrosoftCdoInstType));
                    iCalStringBuilder.Append(Environment.NewLine);
                }

                if (!string.IsNullOrEmpty(calModel.VCalender.VEvent.XMicrosoftDoNotForwardMeeting))
                {
                    iCalStringBuilder.Append(string.Concat("X-MICROSOFT-DONOTFORWARDMEETING:", calModel.VCalender.VEvent.XMicrosoftDoNotForwardMeeting));
                    iCalStringBuilder.Append(Environment.NewLine);
                }

                if (!string.IsNullOrEmpty(calModel.VCalender.VEvent.XMicrosoftDisallowCounter))
                {
                    iCalStringBuilder.Append(string.Concat("X-MICROSOFT-DISALLOW-COUNTER:", calModel.VCalender.VEvent.XMicrosoftDisallowCounter));
                    iCalStringBuilder.Append(Environment.NewLine);
                }

                if (!string.IsNullOrEmpty(calModel.VCalender.VEvent.XKastleKMFInfo))
                {
                    iCalStringBuilder.Append(string.Concat("X-KASTLE-FANDFINFO:", calModel.VCalender.VEvent.XKastleKMFInfo));
                    iCalStringBuilder.Append(Environment.NewLine);
                }

                if (!string.IsNullOrEmpty(calModel.VCalender.VEvent.XKastleSuite))
                {
                    iCalStringBuilder.Append(string.Concat("X-KASTLE-SUITE:", calModel.VCalender.VEvent.XKastleSuite));
                    iCalStringBuilder.Append(Environment.NewLine);
                }

                if (!string.IsNullOrEmpty(calModel.VCalender.VEvent.XKastleNotifyOnArrival))
                {
                    iCalStringBuilder.Append(string.Concat("X-KASTLE-NOTIFYONARRIVAL:", calModel.VCalender.VEvent.XKastleNotifyOnArrival));
                    iCalStringBuilder.Append(Environment.NewLine);
                }

                if (!string.IsNullOrEmpty(calModel.VCalender.VEvent.XKastleIsSendMailToVisitors))
                {
                    iCalStringBuilder.Append(string.Concat("X-KASTLE-SENDMAILTOVISITORS:", calModel.VCalender.VEvent.XKastleIsSendMailToVisitors));
                    iCalStringBuilder.Append(Environment.NewLine);
                }

                if (!string.IsNullOrEmpty(calModel.VCalender.VEvent.XKastleNotes))
                {
                    var notes = this.SplitString(string.Concat("X-KASTLE-NOTES:", calModel.VCalender.VEvent.XKastleNotes));
                    iCalStringBuilder.Append(this.CreateSplitString(notes));
                }

                if (!string.IsNullOrEmpty(calModel.VCalender.VEvent.XKastleSpecialInstructions))
                {
                    var specialInstructions = this.SplitString(string.Concat("X-KASTLE-SPECIALINSTRUCTIONS:", calModel.VCalender.VEvent.XKastleSpecialInstructions));
                    iCalStringBuilder.Append(this.CreateSplitString(specialInstructions));
                }

                //if (calModel.VCalender.VEvent.VAlarm != null)
                //{
                //    iCalStringBuilder.Append(string.Concat("BEGIN:", ApplicationConstants.VALARM));
                //    iCalStringBuilder.Append(string.Concat("DESCRIPTION:", calModel.VCalender.VEvent.VAlarm.Description));
                //    iCalStringBuilder.Append(string.Concat("TRIGGER;RELATED=START:", calModel.VCalender.VEvent.VAlarm.Trigger));
                //    iCalStringBuilder.Append(string.Concat("ACTION:", calModel.VCalender.VEvent.VAlarm.Action));
                //    iCalStringBuilder.Append(string.Concat("END:", ApplicationConstants.VALARM));
                //}

                iCalStringBuilder.Append(string.Concat("X-KASTLE-UTCARRIVALDATE:", calModel.VCalender.VEvent.XKastleUTCArrivalDate));
                iCalStringBuilder.Append(Environment.NewLine);
                iCalStringBuilder.Append(string.Concat("X-KASTLE-UTCARRIVALTIME:", calModel.VCalender.VEvent.XKastleUTCArrivalTime));
                iCalStringBuilder.Append(Environment.NewLine);
                iCalStringBuilder.Append(string.Concat("X-KASTLE-UTCDEPARTUREDATE:", calModel.VCalender.VEvent.XKastleUTCDepartureDate));
                iCalStringBuilder.Append(Environment.NewLine);
                iCalStringBuilder.Append(string.Concat("X-KASTLE-UTCDEPARTURETIME:", calModel.VCalender.VEvent.XKastleUTCDepartureTime));
                iCalStringBuilder.Append(Environment.NewLine);
                iCalStringBuilder.Append(string.Concat("X-KASTLE-FLOORID:", calModel.VCalender.VEvent.XKastleFloorId));
                iCalStringBuilder.Append(Environment.NewLine);

                iCalStringBuilder.Append(string.Concat("END:", ApplicationConstants.VEVENT));
                iCalStringBuilder.Append(Environment.NewLine);

                if (calModel.VCalender.VVenue != null)
                {
                    iCalStringBuilder.Append(string.Concat("BEGIN:", ApplicationConstants.VVENUE));
                    iCalStringBuilder.Append(Environment.NewLine);
                    iCalStringBuilder.Append(string.Concat("UID:", calModel.VCalender.VVenue.UID));
                    iCalStringBuilder.Append(Environment.NewLine);
                    if (!string.IsNullOrEmpty(calModel.VCalender.VVenue.Name))
                    {
                        iCalStringBuilder.Append(string.Concat("NAME:", calModel.VCalender.VVenue.Name));
                        iCalStringBuilder.Append(Environment.NewLine);
                    }

                    if (!string.IsNullOrEmpty(calModel.VCalender.VVenue.StreetAddress))
                    {
                        iCalStringBuilder.Append(string.Concat("STREET-ADDRESS:", calModel.VCalender.VVenue.StreetAddress));
                        iCalStringBuilder.Append(Environment.NewLine);
                    }

                    iCalStringBuilder.Append(string.Concat("END:", ApplicationConstants.VVENUE));
                    iCalStringBuilder.Append(Environment.NewLine);
                }

                iCalStringBuilder.Append(string.Concat("END:", ApplicationConstants.VCALENDER));
                iCalStringBuilder.Append(Environment.NewLine);

                string iCalString = iCalStringBuilder.ToString().Replace("\r\n", "\n");
                return iCalString;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool IsValidEmail(string mail)
        {
            bool isValidEmail = Regex.IsMatch(mail, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));
            AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromMilliseconds(100));
            return isValidEmail;
        }

        private bool IsValidName(string name)
        {
            bool isValidName = Regex.IsMatch(name, @"^[ A-Za-z0-9_'./&-]*$", RegexOptions.None, TimeSpan.FromMilliseconds(100));
            AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromMilliseconds(100));
            if (isValidName)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private string CreateRruleString(Rrule rule)
        {
            string ruleString = string.Empty;
            string frequency = string.Concat("RRULE:FREQ=", rule.Freq);
            string byDay = string.IsNullOrEmpty(rule.ByDay) ? string.Empty : string.Concat(";BYDAY=", rule.ByDay);
            string byMonth = string.IsNullOrEmpty(rule.ByMonth.ToString()) ? string.Empty : string.Concat(";BYMONTH=", rule.ByMonth);
            string until = string.IsNullOrEmpty(rule.Until) ? string.Empty : string.Concat(";UNTIL=", rule.Until);
            string interval = string.IsNullOrEmpty(rule.Interval.ToString()) ? string.Empty : string.Concat(";INTERVAL=", rule.Interval);
            string count = string.IsNullOrEmpty(rule.Count.ToString()) ? string.Empty : string.Concat(";COUNT=", rule.Count);
            string bySetPos = string.IsNullOrEmpty(rule.BySetPos) ? string.Empty : string.Concat(";BYSETPOS=", rule.BySetPos);
            string byMonthDay = string.IsNullOrEmpty(rule.ByMonthDay.ToString()) ? string.Empty : string.Concat(";BYMONTHDAY=", rule.ByMonthDay);
            string wkst = string.IsNullOrEmpty(rule.WKST) ? string.Empty : string.Concat(";WKST=", rule.WKST);

            ruleString = string.Concat(frequency, byDay, byMonth, until, interval, count, bySetPos, byMonthDay, wkst);

            return ruleString;
        }

        private string CreateAttendeeString(Attendee attendee)
        {
            string cuType = string.Concat("ATTENDEE;CUTYPE=", attendee.CuType);
            string role = string.IsNullOrEmpty(attendee.Role) ? string.Empty : string.Concat(";ROLE=", attendee.Role);
            string parstat = string.IsNullOrEmpty(attendee.Parstat) ? string.Empty : string.Concat(";PARSTAT=", attendee.Parstat);
            string rsvp = string.IsNullOrEmpty(attendee.RSVP) ? string.Empty : string.Concat(";RSVP=", attendee.RSVP.ToUpper());
            string xKastleCardHolderGUID = attendee.XKastleCardHolderGUID == Guid.Empty ? string.Empty : string.Concat(":X-KASTLE-CARDHOLDERGUID:", attendee.XKastleCardHolderGUID);
            string tel = string.IsNullOrEmpty(attendee.Tel) ? string.Empty : string.Concat(":Tel:", attendee.Tel);
            string xKastleCountryCode = string.IsNullOrEmpty(attendee.XKastleCountryCode) ? string.Empty : string.Concat(":X-KASTLE-COUNTRYCODE:", attendee.XKastleCountryCode);
            string xKastleCountryID = string.IsNullOrEmpty(attendee.XKastleCountryId) ? string.Empty : string.Concat(":X-KASTLE-COUNTRYID:", attendee.XKastleCountryId);
            string xKastleVisitorCompany = string.IsNullOrEmpty(attendee.XKastleVisitorCompany) ? string.Empty : string.Concat(":X-KASTLE-VISITORCOMPANY:", attendee.XKastleVisitorCompany);
            string xKastleFirstName = string.IsNullOrEmpty(attendee.XKastleFirstName) ? string.Empty : string.Concat(":X-KASTLE-FN:", attendee.XKastleFirstName);
            string xKastleLastName = string.IsNullOrEmpty(attendee.XKastleLastName) ? string.Empty : string.Concat(":X-KASTLE-LN:", attendee.XKastleLastName);
            string mail = string.IsNullOrEmpty(attendee.XKastleEmailId) ? string.Empty : string.Concat(":X-KASTLE-MAILID:", attendee.XKastleEmailId);
            string cn = string.IsNullOrEmpty(attendee.Cn) ? string.Empty : string.Concat(";CN=", attendee.Cn);
            string mailTo = string.IsNullOrEmpty(attendee.mailto) ? string.Empty : string.Concat(":mailto:", attendee.mailto);

            string attendeeString = string.Concat(cuType, role, parstat, rsvp, /*xKastleCardHolderGUID, tel, xKastleCountryCode, xKastleCountryID, xKastleVisitorCompany, xKastleFirstName, xKastleLastName, mail,*/ cn, mailTo, tel, xKastleCardHolderGUID, xKastleCountryCode, xKastleCountryID, xKastleVisitorCompany, xKastleFirstName, xKastleLastName, mail);

            return attendeeString;
        }

        private IEnumerable<string> SplitString(string str)
        {
            var words = new List<string>();

            for (int i = 0; i < str.Length; i += 74)
            {
                if (str.Length - i >= 74)
                {
                    words.Add(str.Substring(i, 74));
                }
                else
                {
                    words.Add(str.Substring(i, str.Length - i));
                }
            }

            return words;
        }

        private VVenue GetVistingCompanyDetails(VisitingCompanyDetails visitingCompanyDetails)
        {
            VVenue vVenue = new ();
            vVenue.UID = visitingCompanyDetails.UID.ToString();
            vVenue.Name = visitingCompanyDetails.Name;
            vVenue.StreetAddress = visitingCompanyDetails.StreetAddress;
            return vVenue;
        }

        private List<Attendee> GetVisitorsDetails(List<VisitorModel> visitorModel)
        {
            var listAttendees = new List<Attendee>();
            foreach (var visitor in visitorModel)
            {
                if (visitor != null)
                {
                    Attendee attendee = new ();
                    attendee.CuType = visitor.TypeOfVisitor;
                    attendee.Role = visitor.Role;
                    attendee.Parstat = visitor.ParticipationStatus;
                    attendee.RSVP = visitor.ResponseRequired.ToString();
                    if (!string.IsNullOrEmpty(visitor.Email))
                    {
                        attendee.XKastleEmailId = visitor.Email;
                        attendee.mailto = visitor.Email;
                    }
                    else
                    {
                        Guid randomEmailGuid = Guid.NewGuid();
                        var visitorMail = randomEmailGuid + "@Kastle.com";
                        attendee.XKastleEmailId = visitorMail;
                        visitor.Email = visitorMail;
                        attendee.mailto = visitorMail;
                    }

                    if (!string.IsNullOrEmpty(visitor.LastName))
                    {
                        if (string.IsNullOrEmpty(visitor.FirstName))
                        {
                            bool isValidLastName = this.IsValidName(visitor.LastName.Trim());
                            if (isValidLastName)
                            {
                                // removed validation for 40 characters to support anonymity.
                                attendee.XKastleLastName = visitor.LastName.Trim();
                                attendee.Cn = visitor.LastName.Trim();
                            }
                            else
                            {
                                throw new ArgumentException("Invalid format of LastName provided for visitor.");
                            }
                        }
                        else if (!string.IsNullOrEmpty(visitor.FirstName))
                        {
                            bool isValidFirstName = this.IsValidName(visitor.FirstName.Trim());
                            bool isValidLastName = this.IsValidName(visitor.LastName.Trim());
                            if (isValidFirstName && isValidLastName)
                            {
                                // removed validation for 40 characters to support anonymity.
                                attendee.XKastleFirstName = visitor.FirstName.Trim();
                                attendee.XKastleLastName = visitor.LastName.Trim();
                                attendee.Cn = string.Format("{0} {1}", visitor.FirstName.Trim(), visitor.LastName.Trim());
                            }
                            else
                            {
                                throw new ArgumentException("Invalid format of First or Last name of the visitor provided.");
                            }
                        }
                    }
                    else
                    {
                        throw new ArgumentNullException("LastName of the visitor cannot be null or empty.");
                    }

                    if (!string.IsNullOrEmpty(visitor.MobileNumber))
                    {
                        if (Regex.IsMatch(visitor.MobileNumber, "^\\+?[1-9][0-9]{7,14}$", RegexOptions.None, TimeSpan.FromMilliseconds(100)))
                        {
                            attendee.Tel = visitor.MobileNumber;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid Mobile number.");
                        }
                    }

                    if (!string.IsNullOrEmpty(visitor.MobileNumberCountryPrifix))
                    {
                        attendee.XKastleCountryCode = visitor.MobileNumberCountryPrifix;
                    }

                    if (!string.IsNullOrEmpty(visitor.CountryId))
                    {
                        attendee.XKastleCountryId = visitor.CountryId;
                    }

                    // Attendee custom properties fill.
                    if (!string.IsNullOrEmpty(visitor.VisitorCompany))
                    {
                        attendee.XKastleVisitorCompany = visitor.VisitorCompany;
                    }

                    if (visitor.CardholderGuid != Guid.Empty && visitor.CardholderGuid != null)
                    {
                        attendee.XKastleCardHolderGUID = (Guid)visitor.CardholderGuid;
                    }
                    else
                    {
                        Guid guid = Guid.NewGuid();
                        attendee.XKastleCardHolderGUID = guid;
                    }

                    listAttendees.Add(attendee);
                }
            }

            return listAttendees;
        }

        private Rrule GetRruleDetails(VisitorRequestModel visitorRequestModel)
        {
            var rule = new Rrule();

            if (visitorRequestModel.RecurrenceModel != null)
            {
                if (string.IsNullOrEmpty(visitorRequestModel.RecurrenceModel.RecurrenceFrequecny))
                {
                    throw new ArgumentNullException("Recurrence Frequency Cannot be null.");
                }

                if (visitorRequestModel.RecurrenceModel.RecurrenceFrequecny.ToUpper() == ApplicationConstants.RECURRINGFREQUENCYDAILY)
                {
                    rule.Freq = visitorRequestModel.RecurrenceModel.RecurrenceFrequecny.ToUpper();
                    if (!string.IsNullOrEmpty(visitorRequestModel.RecurrenceModel.RepeatEvery))
                    {
                        if (this.IsIntegerValue(visitorRequestModel.RecurrenceModel.RepeatEvery))
                        {
                            rule.Interval = Convert.ToInt32(visitorRequestModel.RecurrenceModel.RepeatEvery);
                        }
                        else
                        {
                            throw new ArgumentException("Repeat Every can only be integer");
                        }
                    }

                    if (!string.IsNullOrEmpty(visitorRequestModel.RecurrenceModel.StartingDayOfTheWeek))
                    {
                        rule.WKST = visitorRequestModel.RecurrenceModel.StartingDayOfTheWeek.Substring(0, 2).ToUpper();
                    }

                    if (visitorRequestModel.RecurrenceModel.EndingDateOfTheVisit != null)
                    {
                        if (visitorRequestModel.RecurrenceModel.EndingDateOfTheVisit < DateTime.Now.Date && visitorRequestModel.RecurrenceModel.EndingDateOfTheVisit > visitorRequestModel.ArrivalDate.Date)
                        {
                            throw new ArgumentException("Ending date of the visit cannot be less than current date or arrival date.");
                        }

                        rule.Until = Convert.ToDateTime(visitorRequestModel.RecurrenceModel.EndingDateOfTheVisit).ToString("yyyyMMddTHHmmssZ");
                    }

                    if (!string.IsNullOrEmpty(visitorRequestModel.RecurrenceModel.NumberOfOccurences))
                    {
                        if (this.IsIntegerValue(visitorRequestModel.RecurrenceModel.NumberOfOccurences) && visitorRequestModel.RecurrenceModel.NumberOfOccurences != "0")
                        {
                            rule.Count = Convert.ToInt32(visitorRequestModel.RecurrenceModel.NumberOfOccurences);
                        }
                        else
                        {
                            throw new ArgumentException("NumberOfOccurences can only be integer and should be non zero.");
                        }
                    }
                }
                else if (visitorRequestModel.RecurrenceModel.RecurrenceFrequecny.ToUpper() == ApplicationConstants.RECURRINGFREQUENCYWEEKLY)
                {
                    rule.Freq = visitorRequestModel.RecurrenceModel.RecurrenceFrequecny.ToUpper();
                    if (!string.IsNullOrEmpty(visitorRequestModel.RecurrenceModel.RepeatEvery))
                    {
                        if (this.IsIntegerValue(visitorRequestModel.RecurrenceModel.RepeatEvery))
                        {
                            rule.Interval = Convert.ToInt32(visitorRequestModel.RecurrenceModel.RepeatEvery);
                        }
                        else
                        {
                            throw new ArgumentException("Repeat Every can only be integer");
                        }
                    }

                    if (visitorRequestModel.RecurrenceModel.RecurrenceDays != null && visitorRequestModel.RecurrenceModel.RecurrenceDays.Count() > 0)
                    {
                        var recurrenceDays = string.Join(",", visitorRequestModel.RecurrenceModel.RecurrenceDays.Select(s => s.Substring(0, 2)).ToArray().Select(s => s.ToUpper()).ToArray());
                        rule.ByDay = recurrenceDays;
                    }
                    else
                    {
                        throw new ArgumentNullException("Recurrence Days are mandatory in weekly frequency.");
                    }

                    if (!string.IsNullOrEmpty(visitorRequestModel.RecurrenceModel.StartingDayOfTheWeek))
                    {
                        rule.WKST = visitorRequestModel.RecurrenceModel.StartingDayOfTheWeek.Substring(0, 2).ToUpper();
                    }

                    if (visitorRequestModel.RecurrenceModel.EndingDateOfTheVisit != null)
                    {
                        if (visitorRequestModel.RecurrenceModel.EndingDateOfTheVisit < DateTime.Now.Date && visitorRequestModel.RecurrenceModel.EndingDateOfTheVisit > visitorRequestModel.ArrivalDate.Date)
                        {
                            throw new ArgumentException("Ending date of the visit cannot be less than current date or arrival date.");
                        }

                        rule.Until = Convert.ToDateTime(visitorRequestModel.RecurrenceModel.EndingDateOfTheVisit).ToString("yyyyMMddTHHmmssZ");
                    }

                    if (!string.IsNullOrEmpty(visitorRequestModel.RecurrenceModel.NumberOfOccurences))
                    {
                        if (this.IsIntegerValue(visitorRequestModel.RecurrenceModel.NumberOfOccurences))
                        {
                            rule.Count = Convert.ToInt32(visitorRequestModel.RecurrenceModel.NumberOfOccurences);
                        }
                        else
                        {
                            throw new ArgumentException("NumberOfOccurences can only be integer");
                        }
                    }
                }
                else if (visitorRequestModel.RecurrenceModel.RecurrenceFrequecny.ToUpper() == ApplicationConstants.RECURRINGFREQUENCYMONTHLY)
                {
                    rule.Freq = visitorRequestModel.RecurrenceModel.RecurrenceFrequecny.ToUpper();
                    if (!string.IsNullOrEmpty(visitorRequestModel.RecurrenceModel.StartingDayOfTheWeek))
                    {
                        rule.WKST = visitorRequestModel.RecurrenceModel.StartingDayOfTheWeek.Substring(0, 2).ToUpper();
                    }

                    if (!string.IsNullOrEmpty(visitorRequestModel.RecurrenceModel.RepeatEvery))
                    {
                        if (this.IsIntegerValue(visitorRequestModel.RecurrenceModel.RepeatEvery))
                        {
                            rule.Interval = Convert.ToInt32(visitorRequestModel.RecurrenceModel.RepeatEvery);
                        }
                        else
                        {
                            throw new ArgumentException("RepeatEvery can only be integer");
                        }
                    }

                    if (!string.IsNullOrEmpty(visitorRequestModel.RecurrenceModel.RecurrenceMonth))
                    {
                        if (this.IsIntegerValue(visitorRequestModel.RecurrenceModel.RecurrenceMonth))
                        {
                            if (Enumerable.Range(1, 12).Contains(Convert.ToInt32(visitorRequestModel.RecurrenceModel.RecurrenceMonth)))
                            {
                                rule.ByMonth = Convert.ToInt32(visitorRequestModel.RecurrenceModel.RecurrenceMonth);
                            }
                            else
                            {
                                throw new ArgumentException("RecurrenceMonth must be between 1 and 12");
                            }
                        }
                        else
                        {
                            throw new ArgumentException("RecurrenceMonth ,can only be integer");
                        }
                    }

                    if (visitorRequestModel.RecurrenceModel.EndingDateOfTheVisit != null)
                    {
                        if (visitorRequestModel.RecurrenceModel.EndingDateOfTheVisit < DateTime.Now.Date && visitorRequestModel.RecurrenceModel.EndingDateOfTheVisit > visitorRequestModel.ArrivalDate.Date)
                        {
                            throw new ArgumentException("Ending date of the visit cannot be less than current date or arrival date.");
                        }

                        rule.Until = Convert.ToDateTime(visitorRequestModel.RecurrenceModel.EndingDateOfTheVisit).ToString("yyyyMMddTHHmmssZ");
                    }

                    if (!string.IsNullOrEmpty(visitorRequestModel.RecurrenceModel.NumberOfOccurences))
                    {
                        if (this.IsIntegerValue(visitorRequestModel.RecurrenceModel.NumberOfOccurences))
                        {
                            rule.Count = Convert.ToInt32(visitorRequestModel.RecurrenceModel.NumberOfOccurences);
                        }
                        else
                        {
                            throw new ArgumentException("NumberOfOccurences can only be integer");
                        }
                    }

                    if (!string.IsNullOrEmpty(visitorRequestModel.RecurrenceModel.DayNumberForMonth))
                    {
                        if (visitorRequestModel.RecurrenceModel.RecurrenceDays != null)
                        {
                            if (visitorRequestModel.RecurrenceModel.RecurrenceDays.Length == 1)
                            {
                                var recurrenceDays = string.Join(",", visitorRequestModel.RecurrenceModel.RecurrenceDays.Select(s => s.Substring(0, 2)).ToArray().Select(s => s.ToUpper()).ToArray());
                                rule.ByDay = string.Concat(visitorRequestModel.RecurrenceModel.DayNumberForMonth, recurrenceDays);
                            }
                            else
                            {
                                throw new ArgumentException("More than one day is provided for recurrence.");
                            }
                        }
                        else
                        {
                            throw new ArgumentNullException("Day Must be mentioned for recurrence.");
                        }
                    }

                    if (!string.IsNullOrEmpty(visitorRequestModel.RecurrenceModel.RepeatDayNumberOfMonth))
                    {
                        if (Enumerable.Range(1, 31).Contains(Convert.ToInt32(visitorRequestModel.RecurrenceModel.RepeatDayNumberOfMonth)))
                        {
                            rule.ByMonthDay = Convert.ToInt32(visitorRequestModel.RecurrenceModel.RepeatDayNumberOfMonth);
                        }
                        else
                        {
                            throw new ArgumentException("RepeatDayNumberOfMonth must be between 1 and 31");
                        }
                    }
                }
                else if (visitorRequestModel.RecurrenceModel.RecurrenceFrequecny.ToUpper() == ApplicationConstants.RECURRINGFREQUENCYYEARLY)
                {
                    rule.Freq = visitorRequestModel.RecurrenceModel.RecurrenceFrequecny.ToUpper();
                    if (!string.IsNullOrEmpty(visitorRequestModel.RecurrenceModel.RepeatEvery))
                    {
                        if (this.IsIntegerValue(visitorRequestModel.RecurrenceModel.RepeatEvery))
                        {
                            rule.Interval = Convert.ToInt32(visitorRequestModel.RecurrenceModel.RepeatEvery);
                        }
                        else
                        {
                            throw new ArgumentException("Repeat Every can only be integer");
                        }
                    }

                    if (!string.IsNullOrEmpty(visitorRequestModel.RecurrenceModel.StartingDayOfTheWeek))
                    {
                        rule.WKST = visitorRequestModel.RecurrenceModel.StartingDayOfTheWeek.Substring(0, 2).ToUpper();
                    }

                    if (visitorRequestModel.RecurrenceModel.EndingDateOfTheVisit != null)
                    {
                        if (visitorRequestModel.RecurrenceModel.EndingDateOfTheVisit < DateTime.Now.Date && visitorRequestModel.RecurrenceModel.EndingDateOfTheVisit > visitorRequestModel.ArrivalDate.Date)
                        {
                            throw new ArgumentException("Ending date of the visit cannot be less than current date or arrival date.");
                        }

                        rule.Until = Convert.ToDateTime(visitorRequestModel.RecurrenceModel.EndingDateOfTheVisit).ToString("yyyyMMddTHHmmssZ");
                    }

                    if (!string.IsNullOrEmpty(visitorRequestModel.RecurrenceModel.NumberOfOccurences))
                    {
                        if (this.IsIntegerValue(visitorRequestModel.RecurrenceModel.NumberOfOccurences))
                        {
                            rule.Count = Convert.ToInt32(visitorRequestModel.RecurrenceModel.NumberOfOccurences);
                        }
                        else
                        {
                            throw new ArgumentException("NumberOfOccurences can only be integer");
                        }
                    }

                    if (!string.IsNullOrEmpty(visitorRequestModel.RecurrenceModel.DayNumberForMonth))
                    {
                        if (visitorRequestModel.RecurrenceModel.RecurrenceDays != null)
                        {
                            if (visitorRequestModel.RecurrenceModel.RecurrenceDays.Length == 1)
                            {
                                var recurrenceDays = string.Join(",", visitorRequestModel.RecurrenceModel.RecurrenceDays.Select(s => s.Substring(0, 2)).ToArray().Select(s => s.ToUpper()).ToArray());
                                rule.ByDay = string.Concat(visitorRequestModel.RecurrenceModel.DayNumberForMonth, recurrenceDays);
                            }
                            else
                            {
                                throw new ArgumentException("More than one day is provided for recurrence.");
                            }
                        }
                        else
                        {
                            throw new ArgumentNullException("Day Must be mentioned for recurrence.");
                        }
                    }

                    if (!string.IsNullOrEmpty(visitorRequestModel.RecurrenceModel.RecurrenceMonth))
                    {
                        if (this.IsIntegerValue(visitorRequestModel.RecurrenceModel.RecurrenceMonth))
                        {
                            if (Enumerable.Range(1, 12).Contains(Convert.ToInt32(visitorRequestModel.RecurrenceModel.RecurrenceMonth)))
                            {
                                rule.ByMonth = Convert.ToInt32(visitorRequestModel.RecurrenceModel.RecurrenceMonth);
                            }
                            else
                            {
                                throw new ArgumentException("RecurrenceMonth must be between 1 and 12");
                            }
                        }
                        else
                        {
                            throw new ArgumentException("RecurrenceMonth can only be integer");
                        }
                    }

                    if (!string.IsNullOrEmpty(visitorRequestModel.RecurrenceModel.RepeatDayNumberOfMonth))
                    {
                        if (Enumerable.Range(1, 31).Contains(Convert.ToInt32(visitorRequestModel.RecurrenceModel.RepeatDayNumberOfMonth)))
                        {
                            rule.ByMonthDay = Convert.ToInt32(visitorRequestModel.RecurrenceModel.RepeatDayNumberOfMonth);
                        }
                        else
                        {
                            throw new ArgumentException("RepeatDayNumberOfMonth must be between 1 and 31");
                        }
                    }
                }
            }

            return rule;
        }

        private VTimezone GetTimeZoneDetails(VisitorRequestModel visitorRequestModel)
        {
            VTimezone vTimeZone = new ();

            if (visitorRequestModel.TimeZoneDetails != null)
            {
                if (!string.IsNullOrEmpty(visitorRequestModel.TimeZoneDetails.Timezone))
                {
                    vTimeZone.Tzid = visitorRequestModel.TimeZoneDetails.Timezone;
                }
                else
                {
                    throw new ArgumentNullException("Timezone is null");
                }

                Standard standard = new ();
                DtStart standardTimeZoneDtStart = new ();
                standardTimeZoneDtStart.DateTime = Convert.ToDateTime(visitorRequestModel.TimeZoneDetails.StandardOffSetDate).ToString("yyyyMMddTHHmmss");
                standard.DtStart = standardTimeZoneDtStart;
                if (!string.IsNullOrEmpty(visitorRequestModel.TimeZoneDetails.StandardOffSetStartFrom))
                {
                    standard.TzOffSetFrom = visitorRequestModel.TimeZoneDetails.StandardOffSetStartFrom;
                }
                else
                {
                    throw new ArgumentNullException("Standard off set from is null");
                }

                if (!string.IsNullOrEmpty(visitorRequestModel.TimeZoneDetails.StandardOffSetStartTo))
                {
                    standard.TzOffSetTo = visitorRequestModel.TimeZoneDetails.StandardOffSetStartTo;
                }
                else
                {
                    throw new ArgumentNullException("Standard off set to is null");
                }

                if (!string.IsNullOrEmpty(visitorRequestModel.TimeZoneDetails.StandardTimeZoneName))
                {
                    standard.TzName = visitorRequestModel.TimeZoneDetails.StandardTimeZoneName;
                }

                vTimeZone.Standard = standard;
                if (visitorRequestModel.TimeZoneDetails.IsDayLightSavings)
                {
                    Daylight daylight = new ();
                    DtStart daylightTimezoneDtStart = new ();
                    daylightTimezoneDtStart.DateTime = Convert.ToDateTime(visitorRequestModel.TimeZoneDetails.DaylightOffSetDate).ToString("yyyyMMddTHHmmss");
                    daylight.DtStart = daylightTimezoneDtStart;

                    if (!string.IsNullOrEmpty(visitorRequestModel.TimeZoneDetails.DaylightStandardOffSetStartFrom))
                    {
                        daylight.TzOffSetFrom = visitorRequestModel.TimeZoneDetails.DaylightStandardOffSetStartFrom;
                    }
                    else
                    {
                        throw new ArgumentNullException("Daylight off set from is null");
                    }

                    if (!string.IsNullOrEmpty(visitorRequestModel.TimeZoneDetails.DaylightStandardOffSetStartTo))
                    {
                        daylight.TzOffSetTo = visitorRequestModel.TimeZoneDetails.DaylightStandardOffSetStartTo;
                    }
                    else
                    {
                        throw new ArgumentNullException("Daylight off set to is null");
                    }

                    if (!string.IsNullOrEmpty(visitorRequestModel.TimeZoneDetails.DaylightTimeZoneName))
                    {
                        daylight.TzName = visitorRequestModel.TimeZoneDetails.DaylightTimeZoneName;
                    }

                    vTimeZone.Daylight = daylight;
                }
            }

            return vTimeZone;
        }

        private bool IsIntegerValue(string val)
        {
            bool isIntegerValue = Regex.IsMatch(val, @"^-?[0-9]\d*(\.\d+)?$", RegexOptions.None, TimeSpan.FromMilliseconds(100));
            AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromMilliseconds(100));
            return isIntegerValue;
        }

        private StringBuilder CreateSplitString(IEnumerable<string> inputStr)
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;
            foreach (var line in inputStr)
            {
                if (count == 1)
                {
                    sb.Append(string.Concat(" ", line));
                    sb.Append(Environment.NewLine);
                }
                else if (count == 2)
                {
                    sb.Append(string.Concat(" ", line.Trim()));
                    sb.Append(Environment.NewLine);
                }
                else if (count == 3)
                {
                    sb.Append(string.Concat(" ", line.Trim()));
                    sb.Append(Environment.NewLine);
                }
                else if (count == 4)
                {
                    sb.Append(string.Concat(" ", line.Trim()));
                    sb.Append(Environment.NewLine);
                }
                else if (count == 5)
                {
                    sb.Append(string.Concat(" ", line.Trim()));
                    sb.Append(Environment.NewLine);
                }
                else if (count == 6)
                {
                    sb.Append(string.Concat(" ", line.Trim()));
                    sb.Append(Environment.NewLine);
                }
                else if (count == 7)
                {
                    sb.Append(string.Concat(" ", line.Trim()));
                    sb.Append(Environment.NewLine);
                }
                else if (count == 8)
                {
                    sb.Append(string.Concat(" ", line.Trim()));
                    sb.Append(Environment.NewLine);
                }
                else if (count == 9)
                {
                    sb.Append(string.Concat(" ", line.Trim()));
                    sb.Append(Environment.NewLine);
                }
                else if (count == 10)
                {
                    sb.Append(string.Concat(" ", line.Trim()));
                    sb.Append(Environment.NewLine);
                }
                else
                {
                    sb.Append(line);
                    sb.Append(Environment.NewLine);
                }

                count++;
            }

            return sb;
        }

        private DateTime ConvertToUTC(DateTime dateTime, string timezone)
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            //DateTime destinationTimeZone = TimeZoneInfo.ConvertTime(DateTime.Now, timeZone);
            //DateTime sdsds = TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Local, timeZone);
            //var time = dateTime.ToUniversalTime();
            //var utcDateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateTime, timezone, "UTC");
            var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified), timeZone);
            return utcDateTime;
        }
    }
}
