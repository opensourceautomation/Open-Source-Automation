using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OSAE;

namespace OSAE
{
    public class OSAERecurringSchedule
    {
        public string Name { get; set; }
        public string Time { get; set; }
        public string Interval { get; set; }
        public string Object { get; set; }
        public string Method { get; set; }
        public string Param1 { get; set; }
        public string Param2 { get; set; }
        public string Script { get; set; }

        public string Date { get; set; }
        public string Minutes { get; set; }
        public string MonthDay { get; set; }

        public string Sunday { get; set; }
        public string Monday { get; set; }
        public string Tuesday { get; set; }
        public string Wednesday { get; set; }
        public string Thursday { get; set; }
        public string Friday { get; set; }
        public string Saturday { get; set; }

        public string Active { get; set; }
    }
}
