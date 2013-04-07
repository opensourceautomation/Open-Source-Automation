using System;

namespace PachubeDataAccess
{
    public class DoubleFeedItem : IFeedItem
    {
        public string Id {get;set;}
        public double Value { get; set; }

        public DoubleFeedItem(string id, double val)
        {
            Id = id;
            Value = val;
        }

        public string ToJson(int FeedId)
        {
            return "{\r\n\t\"version\": \"1.0.0\",\r\n\t\"datastreams\":[\r\n\t\t{\r\n\t\t\t\"id\":\"" + this.Id + "\",\r\n\t\t\t\"current_value\": \"" + this.Value.ToString("F") + "\"\r\n\t\t}]\r\n}";
        }
    }
}
