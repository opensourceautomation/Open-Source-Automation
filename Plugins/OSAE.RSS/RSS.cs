namespace OSAE.RSS
{
    using System;
    using System.Data;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Timers;
    using System.Xml;

    public class RSS : OSAEPluginBase
    {
        string pName;
        Logging logging = Logging.GetLogger("RSS");
        System.Timers.Timer Clock;
        Thread updateThread;
        int updateInterval=60;

        public override void ProcessCommand(OSAEMethod method)
        {
            if (method.MethodName == "UPDATE")
                updateFeeds();
        }

        public override void RunInterface(string pluginName)
        { 
            pName = pluginName;
            logging.AddToLog("Running Interface!", true);
            Clock = new System.Timers.Timer();
            Clock.Interval = Int32.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Update Interval").Value) * 60000;
            logging.AddToLog(updateInterval.ToString(), true);
            Clock.Elapsed += new ElapsedEventHandler(Timer_Tick);
            logging.AddToLog(Clock.Interval.ToString(), true);
            Clock.Start();
            
            logging.AddToLog("Starting timer", true);
            this.updateThread = new Thread(new ThreadStart(updateFeeds));
            this.updateThread.Start();
        }

        public override void Shutdown()
        {
            updateThread.Abort();
            Clock.Stop();
        }


        public void Timer_Tick(object sender, EventArgs eArgs)
        {
            logging.AddToLog("Timer Tick", false);
            if (sender == Clock)
            {
                if (!updateThread.IsAlive)
                {
                    this.updateThread = new Thread(new ThreadStart(updateFeeds));
                    this.updateThread.Start();
                }
            }

        }

        public void updateFeeds()
        {

            logging.AddToLog("Trying to get all feed Urls:" + pName, false);
            
                DataSet ds = new DataSet();
                ds = OSAEObjectPropertyManager.ObjectPropertyArrayGetAll(pName, "Feeds");
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        logging.AddToLog("Fetching feed: " + dr["item_name"].ToString(), false);
                        WebClient webClient = new WebClient();
                        string strSource = webClient.DownloadString(dr["item_name"].ToString());
                        webClient.Dispose();

                        XmlDocument xml = new XmlDocument();
                        xml.LoadXml(strSource);

                        string feedTitle = xml.SelectSingleNode("/rss/channel/title").InnerText;
                        logging.AddToLog("Feeds title: " + feedTitle, false);
                        if (OSAEObjectPropertyManager.GetObjectPropertyValue(pName, feedTitle).Value == "")
                        {
                            logging.AddToLog("Adding property to object type", false);
                            OSAEObjectTypeManager.ObjectTypePropertyAdd(feedTitle, "List", "", "RSS", false);
                        }
                        OSAEObjectPropertyManager.ObjectPropertyArrayDeleteAll(pName, feedTitle);
                        logging.AddToLog("Cleared feed data", false);

                        XmlNodeList xnList = xml.SelectNodes("/rss/channel/item");
                        foreach (XmlNode xn in xnList)
                        {
                            string content = xn.SelectSingleNode("title").InnerText + " - " + xn.SelectSingleNode("description").InnerText;
                            content = Regex.Replace(content, "<.*?>", string.Empty);
                            logging.AddToLog("Added item to feed data: " + content, false);
                            OSAEObjectPropertyManager.ObjectPropertyArrayAdd(pName, feedTitle, content, "");
                        }
                    }
                    catch(Exception ex)
                    {
                        logging.AddToLog("Error: " + ex.Message, true);
                    }
                }

            
        }
    }
}
