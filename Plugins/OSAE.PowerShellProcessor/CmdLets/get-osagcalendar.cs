namespace OSAE.PowerShellProcessor.CmdLets
{

    using Google.GData.Calendar;
    using System;
    using System.Collections.Generic;
    using System.Management.Automation;
    using Google.GData.Client;
    using Google.GData.Extensions;

    [Cmdlet(VerbsCommon.Get, "OSAGCalendar")]
    public class OSAGetGCalendar : Cmdlet
    {
        //OSAELog
        private OSAE.General.OSAELog Log = new General.OSAELog("POWERSHELL");

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
            get { return fromDate; }
            set { fromDate = value; }
        }

        private DateTime toDate = DateTime.MinValue;

        [Parameter(
            Mandatory = false)]
        public DateTime ToDate
        {
            get { return toDate; }
            set { toDate = value; }
        }

        private bool ? futureEvents = null;

        [Parameter(
            Mandatory = false)]
        public bool FutureEvents
        {
            get
            {
                if (futureEvents == null) return false;
                return (bool)futureEvents;
            }
            set { futureEvents = value; }
        }

        protected override void ProcessRecord()
        {
            Log.Debug("Get-OSAGCalendar - ProcessRecord - Started");

            CalendarService service = new CalendarService("OSA");
            service.setUserCredentials(Username, Password);

            EventQuery query = new EventQuery();
            query.Uri = new Uri("http://www.google.com/calendar/feeds/" + Username + "/private/full");

            if (FromDate != DateTime.MinValue) query.StartTime = fromDate;

            query.RecurrenceStart = DateTime.Now;

            if (ToDate != DateTime.MinValue) query.EndTime = ToDate;

            if (FutureEvents)
            {
                Log.Debug("Only looking for future events");
                query.FutureEvents = true;
            }
            else
                query.FutureEvents = false;

            query.SortOrder = CalendarSortOrder.ascending;
            query.ExtraParameters = "orderby=starttime";

            // Tell the service to query:
            EventFeed calFeed = service.Query(query);
            
            List<CalendarEvent> events = new List<CalendarEvent>();

            foreach (var entry in calFeed.Entries)
            {
               Log.Debug("Found Entry: " + entry.ToString());
               EventEntry eventEntry = entry as Google.GData.Calendar.EventEntry;
               if (eventEntry != null)
               {                   
                   if (!eventEntry.Status.Value.Contains("event.canceled"))
                   {
                       Log.Debug("Entry is an EventEntry");
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
