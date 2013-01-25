using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Net;
using System.Xml;
using System.Data;
using System.Text.RegularExpressions;

namespace OSAE.RSS
{
    public class RSS : OSAEPluginBase
    {
        string pName;
        OSAE osae = new OSAE("RSS");
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
            osae.AddToLog("Running Interface!", true);
            Clock = new System.Timers.Timer();
            Clock.Interval = Int32.Parse(osae.GetObjectProperty(pName, "Update Interval")) * 60000;
            osae.AddToLog(updateInterval.ToString(), true);
            Clock.Elapsed += new ElapsedEventHandler(Timer_Tick);
            osae.AddToLog(Clock.Interval.ToString(), true);
            Clock.Start();
            
            osae.AddToLog("Starting timer", true);
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
            osae.AddToLog("Timer Tick", false);
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

            osae.AddToLog("Trying to get all feed Urls:" + pName, false);
            
                DataSet ds = new DataSet();
                ds = osae.ObjectPropertyArrayGetAll(pName, "Feeds");
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        osae.AddToLog("Fetching feed: " + dr["item_name"].ToString(), false);
                        WebClient webClient = new WebClient();
                        string strSource = webClient.DownloadString(dr["item_name"].ToString());
                        webClient.Dispose();

                        XmlDocument xml = new XmlDocument();
                        xml.LoadXml(strSource);

                        string feedTitle = xml.SelectSingleNode("/rss/channel/title").InnerText;
                        osae.AddToLog("Feeds title: " + feedTitle, false);
                        if (osae.GetObjectProperty(pName, feedTitle) == "")
                        {
                            osae.AddToLog("Adding property to object type", false);
                            osae.ObjectTypePropertyAdd(feedTitle, "List", "", "RSS", false);
                        }
                        osae.ObjectPropertyArrayDeleteAll(pName, feedTitle);
                        osae.AddToLog("Cleared feed data", false);

                        XmlNodeList xnList = xml.SelectNodes("/rss/channel/item");
                        foreach (XmlNode xn in xnList)
                        {
                            string content = xn.SelectSingleNode("title").InnerText + " - " + xn.SelectSingleNode("description").InnerText;
                            content = Regex.Replace(content, "<.*?>", string.Empty);
                            osae.AddToLog("Added item to feed data: " + content, false);
                            osae.ObjectPropertyArrayAdd(pName, feedTitle, content, "");
                        }
                    }
                    catch(Exception ex)
                    {
                        osae.AddToLog("Error: " + ex.Message, true);
                    }
                }

            
        }
    }
}
