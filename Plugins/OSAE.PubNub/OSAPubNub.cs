namespace OSAE.OSAPubNub
{
    using Newtonsoft.Json.Linq;
    using PubNubMessaging.Core;
    using System;
    using System.Threading;

    public class OSAPUBNUB : OSAEPluginBase
    {
        private OSAE.General.OSAELog Log = new General.OSAELog();
        // PubNub publish and subscribe keys
        private Pubnub pubnub = new Pubnub("pub-c-45d27cdb-5675-4a00-8065-5e6347a9bd57", "sub-c-acd9fce4-4958-11e5-b316-0619f8945a4f");

        String gAppName = "";
        Boolean gDebug = false;

        public override void RunInterface(string pluginName)
        {
            gAppName = pluginName;

            OwnTypes();

            Load_Settings();

            SubscribePubNubMessages();

        }

        // Subscribe PubNub messages
        private void SubscribePubNubMessages()
        {
            pubnub.Subscribe<string>(
                "rpipb-vmsg",
                PubNubSubscribeSuccess,
                DisplaySubscribeConnectStatusMessage,
                PubNubError);
        }

        // Receive PubNub messages
        private void PubNubSubscribeSuccess(string publishResult)
        {
            Log.Info("Message: " + publishResult);

            JArray message = JArray.Parse(publishResult);

            string text = message[0]["speak"].ToString();

            Log.Info("Text Length: " + text.Length);

        }

        private void DisplaySubscribeConnectStatusMessage(string publishResult)
        {
            Log.Info("Connection: " + publishResult);
        }

        // We have some issue sending the message, simply display it on the console
        private void PubNubError(PubnubClientError error)
        {
            Log.Info("Error: " + error.ToString());
        }

        public void OwnTypes()
        {
            //Added the follow to automatically own PubNub Base types that have no owner.
            OSAEObjectType oType = OSAEObjectTypeManager.ObjectTypeLoad("PUBNUB");

            if (oType.OwnedBy == "")
            {
                OSAEObjectTypeManager.ObjectTypeUpdate(oType.Name, oType.Name, oType.Description, gAppName, oType.BaseType, oType.Owner, oType.SysType, oType.Container, oType.HideRedundant);
                Log.Info("PubNub Plugin took ownership of the PubNub Object Type.");
            }
            else
            {
                Log.Info("The PubNub Plugin correctly owns the PubNub Object Type.");
            }
        }

        public override void ProcessCommand(OSAEMethod method)
        {          
            string sMethod = method.MethodName;
            string sParam1 = method.Parameter1;
            string sParam2 = method.Parameter2;

            if (gDebug) Log.Debug("Received Command to: " + sMethod + " (" + sParam1 + ", " + sParam2 + ")");

            if (sMethod == "SENDMESSAGE")
            {
                string sText = Common.PatternParse(sParam1);
                OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "Speaking", "TRUE", gAppName);
                Thread.Sleep(500);
                OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "Speaking", "FALSE", gAppName);
            }
        }

        private void Load_Settings()
        {        
            try
            {
                try
                {
                    gDebug = Convert.ToBoolean(OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Debug").Value);
                }
                catch
                {
                    Log.Error("I think the Debug property is missing from the PubNub object type!");
                }
                Log.Info("Debug Mode Set to " + gDebug);
            }
            catch (Exception ex)
            {
                Log.Error("Error in Load_Settings!", ex);
            }




        }

        public override void Shutdown()
        {
            this.Log.Info("Recieved Shutdown Order.");
        }
    }
}
