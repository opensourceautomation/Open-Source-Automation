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
        private OSAE.General.OSAELog Log;
        //Logging logging = Logging.GetLogger("COSM");
        List<string> RealMonitorItems = new List<string>();
        COSMWriter cosmTest;
        public override void ProcessCommand(OSAEMethod method)
        {
            // COSMUPDATER.Run Method.RELOADITEMS

            switch (method.MethodName.ToUpper())
            {
                case "OFF":
                    Log.Info("COSMUpdater Stopped");
                    enabled = false;
                    break;
                case "ON":
                    Log.Info("COSMUpdater Started");
                    enabled = true;
                    break;
                case "WRITEDATA":
                    Log.Info("COSMUpdater DataWrite Forced");
                    WriteData();
                    break;
                case "RELOADITEMS":
                    Log.Info("COSMUpdater ReloadItems");
                    GetCurrentList();
                    break;
                default:
                    Log.Info(string.Format("COSMUpdater got method of {0} but it is not implemented", method.MethodName));
                    break;
            }
        }

        public override void RunInterface(string pluginName)
        {

            Log.Info("Loading COSMUpdater..");
            cosmTest = new COSMWriter();
            pName = pluginName;
            Log = new General.OSAELog(pName);
            try
            {
                pollInterval = int.Parse("0" + OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "PollRate").Value);
                if (!ReportAndCheckUsage()) return;
                SetUpPoller();
                enabled = true;
                Log.Info("Started COSMUpdater...");
            }
            catch (Exception ex)
            { Log.Error("COSMUpdater Exception in RunInterface. ", ex); }
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
                    Log.Info(string.Format("*** ERROR {0} monitoring OSA Object {1} Property {2} and sending to COSM Feed {3} and DataStream {4} not found or value is empty", item.Name, osaObj.Value, osaProp.Value, cosmFeed.Value, cosmDataStream.Value));
                else
                {
                    Log.Info(string.Format("{0} monitoring OSA Object {1} Property {2} and sending to COSM Feed {3} and DataStream {4} and Current Value of {5}", item.Name, osaObj.Value, osaProp.Value, cosmFeed.Value, cosmDataStream.Value, osaCurrValue.Value));
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
                Log.Info("COSMUpdater pollInterval=" + pollInterval);
                if (pollInterval < 30)
                {
                    lValid = false;
                    Log.Info("COSMUpdater pollInterval must be greater than or equal to 30 to prevent flooding COSM with updates");
                }
                Log.Info("COSMUpdater has been setup with the following items to monitor");
                GetCurrentList();
                if (!lValid) Log.Info("COSMUpdater is not started. No updates will be processed.");
            }
            catch (Exception ex)
            {
                Log.Error("COSMUpdater Exception in ReportAndCheckUsage.", ex);
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
                    Log.Debug("Running COSMUpdater polling logic");
                    WriteData();
                }
                else
                    Log.Debug("COSMUpdater polling logic currently disabled");

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
                    Log.Info(string.Format("*** ERROR {0} monitoring OSA Object {1} Property {2} and sending to COSM Feed {3} and DataStream {4} not found or value is empty", item, osaObj.Value, osaProp.Value, cosmFeed.Value, cosmDataStream.Value));
                else
                    Log.Debug(string.Format("Sending {0} monitoring OSA Object {1} Property {2} and sending to COSM Feed {3} and DataStream {4} and Current Value of {5}", item, osaObj.Value, osaProp.Value, cosmFeed.Value, cosmDataStream.Value, osaCurrValue.Value));

                var timestamp = DateTime.UtcNow.ToString("s") + "Z";
                var bulk = string.Format("{0},{1}", timestamp, osaCurrValue.Value);
                cosmTest.Send(cosmAPIKey.Value, cosmFeed.Value, cosmDataStream.Value, bulk);
            }
        }

        public override void Shutdown()
        {
            Log.Info("Running COSMUpdater shutdown logic");
        }
    }
}

