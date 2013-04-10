using System;
using System.Net;
using System.IO;
using System.Threading;
using System.Diagnostics;
using OSAE;

namespace PachubeDataAccess
{
    public class Feed
    {
        public Account AssociatedAccount;
        public int Id;

        public Feed(int id, Account account)
        {
            Id = id;
            AssociatedAccount = account;
        }

        public string Get()
        {
            String result = "";
            try
            {                
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(AssociatedAccount.EventsUrl + this.Id.ToString());
                req.Method = "GET";
                req.ContentType = "application/json";
                req.UserAgent = "NetduinoPlus";
                req.Headers.Add("X-PachubeApiKey", AssociatedAccount.ApiKey);

                Logging.GetLogger("PowerShell").AddToLog("Pachube request details: " + AssociatedAccount.EventsUrl + this.Id.ToString() , false);

                using (WebResponse resp = req.GetResponse())
                {
                    using (StreamReader respStream = new StreamReader(resp.GetResponseStream()))
                    {
                        result = respStream.ReadToEnd();
                    }
                }
            }
            catch (Exception e)
            {
                Logging.GetLogger("PowerShell").AddToLog("Get Pachube data failed : " + e, false);
            }
            return result;
        }

        public string Post(IFeedItem item)
        {
            String result = "";
            
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(this.AssociatedAccount.EventsUrl + this.Id.ToString());
                req.Method = "PUT";
                req.ContentType = "application/json";
                req.UserAgent = "NetduinoPlus";
                req.Headers.Add("X-PachubeApiKey", this.AssociatedAccount.ApiKey);
                req.Timeout = 1000;
                if (this.AssociatedAccount.HttpProxy != null)
                    req.Proxy = this.AssociatedAccount.HttpProxy;

                string content = item.ToJson(this.Id);
                Logging.GetLogger("PowerShell").AddToLog("Post Content : " + content, false);


                byte[] postdata = System.Text.Encoding.UTF8.GetBytes(content);
                req.ContentLength = postdata.Length;

                using (Stream s = req.GetRequestStream())
                {
                    s.Write(postdata, 0, postdata.Length);
                }

                using (WebResponse resp = req.GetResponse())
                {
                    using (StreamReader respStream = new StreamReader(resp.GetResponseStream()))
                    {
                        result = respStream.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.GetLogger("PowerShell").AddToLog("Post Pachube data failed : " + ex, false);
            }
            finally
            {
                
            }
            return result;
        }        
    }
}
