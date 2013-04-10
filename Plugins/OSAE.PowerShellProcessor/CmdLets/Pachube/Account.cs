using System;
using System.Net;

namespace PachubeDataAccess
{
    public class Account
    {
        public string ApiKey;
        public string BaseUrl = "http://api.pachube.com/v2/";
        public WebProxy HttpProxy = null;

        public Account(string key)
        {
            ApiKey = key;
        }

        public string EventsUrl
        {
            get
            {
                return BaseUrl + "feeds/";
            }
            private set
            { }
        }
    }
}
