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
        private OSAE.General.OSAELog Log;
        System.Timers.Timer Clock;
        Thread updateThread;
        int updateInterval=60;

        public override void ProcessCommand(OSAEMethod method)
        {
            if (method.MethodName == "UPDATE") updateFeeds();
        }

        public override void RunInterface(string pluginName)
        { 
            pName = pluginName;
            Log = new General.OSAELog(pName);
            Log.Info("Running Interface!");
            Clock = new System.Timers.Timer();
            Clock.Interval = Int32.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Update Interval").Value) * 60000;
            Log.Info(updateInterval.ToString());
            Clock.Elapsed += new ElapsedEventHandler(Timer_Tick);
            Log.Info(Clock.Interval.ToString());
            Clock.Start();
            
            Log.Info("Starting timer");
            updateThread = new Thread(new ThreadStart(updateFeeds));
            updateThread.Start();
        }

        public override void Shutdown()
        {
            updateThread.Abort();
            Clock.Stop();
        }


        public void Timer_Tick(object sender, EventArgs eArgs)
        {
            Log.Debug("Timer Tick");
            if (sender == Clock)
            {
                if (!updateThread.IsAlive)
                {
                    updateThread = new Thread(new ThreadStart(updateFeeds));
                    updateThread.Start();
                }
            }
        }

        public void updateFeeds()
        {
            Log.Debug("Trying to get all feed Urls:" + pName);
            DataSet ds = new DataSet();
            ds = OSAEObjectPropertyManager.ObjectPropertyArrayGetAll(pName, "Feeds");
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                try
                {
                    Log.Debug("Fetching feed: " + dr["item_name"].ToString());
                    WebClient webClient = new WebClient();
                    string strSource = webClient.DownloadString(dr["item_name"].ToString());
                    webClient.Dispose();

                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(strSource);

                    string feedTitle = xml.SelectSingleNode("/rss/channel/title").InnerText;
                    Log.Debug("Feeds title: " + feedTitle);
                    if (OSAEObjectPropertyManager.GetObjectPropertyValue(pName, feedTitle).Value == "")
                    {
                        Log.Debug("Adding property to object type");
                        OSAEObjectTypeManager.ObjectTypePropertyAdd(feedTitle, "List", "","", "RSS", false, false, "RSS List");
                    }
                    OSAEObjectPropertyManager.ObjectPropertyArrayDeleteAll(pName, feedTitle);
                    Log.Debug("Cleared feed data");

                    XmlNodeList xnList = xml.SelectNodes("/rss/channel/item");
                    foreach (XmlNode xn in xnList)
                    {
                        string content = xn.SelectSingleNode("title").InnerText + " - " + xn.SelectSingleNode("description").InnerText;
                        content = Regex.Replace(content, "<.*?>", string.Empty);
                        Log.Debug("Added item to feed data: " + content);
                        OSAEObjectPropertyManager.ObjectPropertyArrayAdd(pName, feedTitle, content, "");
                    }
                }
                catch(Exception ex)
                { Log.Error("Error", ex); }
            }
        }
    }
}
