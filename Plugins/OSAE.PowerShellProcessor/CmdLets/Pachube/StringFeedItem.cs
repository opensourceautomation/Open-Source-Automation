using System;

namespace PachubeDataAccess
{
    public class StringFeedItem : IFeedItem
    {
        public string Id {get;set;}
        public string Value { get; set; }

        public StringFeedItem(string id, string val)
        {
            Id = id;
            Value = val;
        }

        public string ToJson(int FeedId)
        {
            return "{\r\n\t\"version\": \"1.0.0\",\r\n\t\"datastreams\":[\r\n\t\t{\r\n\t\t\t\"id\":\"" + this.Id + "\",\r\n\t\t\t\"current_value\": \"" + this.Value +"\"\r\n\t\t}]\r\n}";
        }
    }
}
