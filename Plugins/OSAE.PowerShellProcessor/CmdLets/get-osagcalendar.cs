namespace OSAE.PowerShellProcessor.CmdLets
{
    using System.Management.Automation;
    using Google.GData.Client;
    using Google.GData.Calendar;
    using System;
    using System.Collections.Generic;
    using Google.GData.Extensions;
    
    [Cmdlet(VerbsCommon.Get, "OSAGCalendar")]
    public class OSAGetGCalendar : Cmdlet
    {
        /// <summary>
        /// Provides access to the OSA logging class
        /// </summary>
        private Logging logging = Logging.GetLogger("PowerShell");

        [Parameter(
            Mandatory = true)]
        public string Username { get; set; }

        [Parameter(
            Mandatory = true)]
        public string Password { get; set; }

        private DateTime fromDate = DateTime.MinValue;

        [Parameter(
            Mandatory = false)]
        public DateTime FromDate
        {
            get
            {
                return fromDate;
            }
            set
            {
                fromDate = value;
            }
        }

        private DateTime toDate = DateTime.MinValue;

        [Parameter(
            Mandatory = false)]
        public DateTime ToDate
        {
            get
            {
                return toDate;
            }
            set
            {
                toDate = value;
            }
        }

        private bool ? futureEvents = null;

        [Parameter(
            Mandatory = false)]
        public bool FutureEvents
        {
            get
            {
                if (futureEvents == null)
                {
                    return false;
                }
                return (bool)futureEvents;
            }
            set
            {
                futureEvents = value;
            }
        }

        protected override void ProcessRecord()
        {
            logging.AddToLog("Get-OSAGCalendar - ProcessRecord - Started", false);

            CalendarService service = new CalendarService("OSA");
            service.setUserCredentials(Username, Password);

            EventQuery query = new EventQuery();
            query.Uri = new Uri("http://www.google.com/calendar/feeds/" +
                  Username + "/private/full");

            if (FromDate != DateTime.MinValue)
            {
                query.StartTime = fromDate;
            }

            query.RecurrenceStart = DateTime.Now;

            if (ToDate != DateTime.MinValue)
            {
                query.EndTime = ToDate;
            }

            if (FutureEvents)
            {
                logging.AddToLog("Only looking for future events", false);
                query.FutureEvents = true;
            }
            else
            {
                query.FutureEvents = false;
            }

            query.SortOrder = CalendarSortOrder.ascending;
            query.ExtraParameters = "orderby=starttime";

            // Tell the service to query:
            EventFeed calFeed = service.Query(query);
            
            List<CalendarEvent> events = new List<CalendarEvent>();

            foreach (var entry in calFeed.Entries)
            {
               logging.AddToLog("Found Entry: " + entry.ToString(), false);
               EventEntry eventEntry = entry as Google.GData.Calendar.EventEntry;
               if (eventEntry != null)
               {                   
                   if (!eventEntry.Status.Value.Contains("event.canceled"))
                   {
                       logging.AddToLog("Entry is an EventEntry", false);
                       CalendarEvent c = new CalendarEvent();
                       c.Title = eventEntry.Title.Text;
                       c.Content = eventEntry.Content.Content;

                       if (eventEntry.Times.Count > 0)
                       {
                           c.Start = eventEntry.Times[0].StartTime;
                           c.End = eventEntry.Times[0].EndTime;
                       }

                       events.Add(c);
                   }
                }
            }
            WriteObject(events);
        }
    }

    public class CalendarEvent
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
