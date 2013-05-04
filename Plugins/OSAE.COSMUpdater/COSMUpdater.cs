using System;
using System.Collections.Generic;
using System.Threading;

namespace OSAE.COSMUpdater
{
    
    public class COSMUpdater : OSAEPluginBase
    {

        string pName = string.Empty;
        int pollInterval;
        bool enabled = false;
        Thread backgroundThread;
        Logging logging = Logging.GetLogger("COSM");
        List<string> RealMonitorItems = new List<string>();
        COSMWriter cosmTest;
        public override void ProcessCommand(OSAEMethod method)
        {
            // COSMUPDATER.Run Method.RELOADITEMS

            switch (method.MethodName.ToUpper())
            {
                case "OFF":
                    logging.AddToLog("COSMUpdater Stopped", true);
                    enabled = false;
                    break;
                case "ON":
                    logging.AddToLog("COSMUpdater Started", true);
                    enabled = true;
                    break;
                case "WRITEDATA":
                    logging.AddToLog("COSMUpdater DataWrite Forced", true);
                    WriteData();
                    break;
                case "RELOADITEMS":
                    logging.AddToLog("COSMUpdater ReloadItems", true);
                    GetCurrentList();
                    break;
                default:
                    logging.AddToLog(string.Format("COSMUpdater got method of {0} but it is not implemented", method.MethodName), true);
                    break;
            }
        }

        public override void RunInterface(string pluginName)
        {

            logging.AddToLog("Loading COSMUpdater (0.1.1)...", true);
            cosmTest = new COSMWriter();
            pName = pluginName;
            try
            {
                pollInterval = int.Parse("0" + OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "PollRate").Value);
                if (!ReportAndCheckUsage()) return;
                SetUpPoller();
                enabled = true;
                logging.AddToLog("Started COSMUpdater (0.1.1)...", true);
            }
            catch (Exception ex)
            {
                logging.AddToLog("COSMUpdater Exception in RunInterface. " + ex.Message, true);
            }
        }

        private void GetCurrentList()
        {
            RealMonitorItems.Clear();
            foreach (var item in GetMonitorItems())
            {
                var osaObj = OSAEObjectPropertyManager.GetObjectPropertyValue(item.Name, "OSAObject");
                var osaProp = OSAEObjectPropertyManager.GetObjectPropertyValue(item.Name, "OSAObjectProperty");
                var cosmFeed = OSAEObjectPropertyManager.GetObjectPropertyValue(item.Name, "COSMFeedID");
                var cosmDataStream = OSAEObjectPropertyManager.GetObjectPropertyValue(item.Name, "COSMDataStream");
                var osaCurrValue = OSAEObjectPropertyManager.GetObjectPropertyValue(osaObj.Value, osaProp.Value);
                if (osaCurrValue == null || osaCurrValue.Value.Trim() == string.Empty)
                {
                    logging.AddToLog(string.Format("*** ERROR {0} monitoring OSA Object {1} Property {2} and sending to COSM Feed {3} and DataStream {4} not found or value is empty", item.Name, osaObj.Value, osaProp.Value, cosmFeed.Value, cosmDataStream.Value), true);
                }
                else
                {
                    logging.AddToLog(string.Format("{0} monitoring OSA Object {1} Property {2} and sending to COSM Feed {3} and DataStream {4} and Current Value of {5}", item.Name, osaObj.Value, osaProp.Value, cosmFeed.Value, cosmDataStream.Value, osaCurrValue.Value), true);
                    RealMonitorItems.Add(item.Name);
                }
            }
        }

        private OSAEObjectCollection GetMonitorItems()
        {
            return OSAEObjectManager.GetObjectsByType("COSMITEM");
        }

        private bool ReportAndCheckUsage()
        {
            bool lValid = true;
            try
            {
                logging.AddToLog("COSMUpdater pollInterval=" + pollInterval, true);
                if (pollInterval < 30)
                {
                    lValid = false;
                    logging.AddToLog("COSMUpdater pollInterval must be greater than or equal to 30 to prevent flooding COSM with updates", true);
                }
                logging.AddToLog("COSMUpdater has been setup with the following items to monitor", true);
                GetCurrentList();
                if (!lValid) logging.AddToLog("COSMUpdater is not started. No updates will be processed.", true);
            }
            catch (Exception ex)
            {
                logging.AddToLog("COSMUpdater Exception in ReportAndCheckUsage. " + ex.Message, true);
            }
            return lValid;
        }

        private void SetUpPoller()
        {
            backgroundThread = new Thread(new ThreadStart(DoWork)) { Name = "Poller" };
            backgroundThread.Start();
        }
        private void DoWork()
        {
            while (true)
            {
                if (enabled)
                {
                    logging.AddToLog("Running COSMUpdater polling logic", false);
                    WriteData();
                }
                else
                {
                    logging.AddToLog("COSMUpdater polling logic currently disabled", false);
                }
                Thread.Sleep(pollInterval * 1000);
            }
        }

        private void WriteData()
        {
            foreach (var item in RealMonitorItems)
            {
                var osaObj = OSAEObjectPropertyManager.GetObjectPropertyValue(item, "OSAObject");
                var osaProp = OSAEObjectPropertyManager.GetObjectPropertyValue(item, "OSAObjectProperty");
                var cosmFeed = OSAEObjectPropertyManager.GetObjectPropertyValue(item, "COSMFeedID");
                var cosmDataStream = OSAEObjectPropertyManager.GetObjectPropertyValue(item, "COSMDataStream");
                var cosmAPIKey = OSAEObjectPropertyManager.GetObjectPropertyValue(item, "COSMAPIKey");

                var osaCurrValue = OSAEObjectPropertyManager.GetObjectPropertyValue(osaObj.Value, osaProp.Value);
                if (osaCurrValue == null || osaCurrValue.Value.Trim() == string.Empty)
                {
                    logging.AddToLog(string.Format("*** ERROR {0} monitoring OSA Object {1} Property {2} and sending to COSM Feed {3} and DataStream {4} not found or value is empty", item, osaObj.Value, osaProp.Value, cosmFeed.Value, cosmDataStream.Value), true);
                }
                else
                {
                    logging.AddToLog(string.Format("Sending {0} monitoring OSA Object {1} Property {2} and sending to COSM Feed {3} and DataStream {4} and Current Value of {5}", item, osaObj.Value, osaProp.Value, cosmFeed.Value, cosmDataStream.Value, osaCurrValue.Value), false);
                }
                var timestamp = DateTime.UtcNow.ToString("s") + "Z";
                var bulk = string.Format("{0},{1}", timestamp, osaCurrValue.Value);
                cosmTest.Send(cosmAPIKey.Value, cosmFeed.Value, cosmDataStream.Value, bulk);
            }
        }

        public override void Shutdown()
        {
            logging.AddToLog("Running COSMUpdater shutdown logic", true);
        }
    }
}

