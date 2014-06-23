using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using OSAE;
using System.Text;
using System.Web;
using System.Xml;


namespace OSAE.IPCamII
{
    public class IPCamII : OSAEPluginBase
    {
        private OSAE.General.OSAELog Log = new OSAE.General.OSAELog();
        string camName;
        string camStream;
        string camSnapShot;
        string camSloc;
        string camDegrees;
        string camOptional;
        string camIpAddress;
        string camPort;
        string camUser;
        string camPass;
        string sMethod;

        public override void RunInterface(string pluginName)
        {
            this.Log.Info("===================================================");
            this.Log.Info("IPCAMII Plugin Object: " + pluginName);
            this.Log.Info(pluginName + " is starting...");
        }

        public override void ProcessCommand(OSAEMethod method)
        {
            this.Log.Info("RECEIVED: " + method.ObjectName + " - " + method.MethodName);
            camName = method.ObjectName;
            OSAEObject obj = OSAEObjectManager.GetObjectByName(camName);
            camPort = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "Port").Value;
            camUser = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "Username").Value;
            camPass = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "Password").Value;
            camStream = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "Stream Address").Value;
           
            string camCommand = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, method.MethodName).Value;

            camSnapShot = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "camSnapShot").Value;

            camSloc = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "Save Location").Value;
            camDegrees = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "Degrees").Value;
            camSloc = camSloc + @"\";
            if (method.Parameter1 != "")
            {
                camDegrees = method.Parameter1;
            }
            if (method.Parameter2 != "")
            {
                camOptional = method.Parameter2;
            }
            sMethod = method.MethodName;
            WebClient wc = new WebClient();
            wc.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            if (sMethod == "SNAPSHOT")
            {
                string i = DateTime.Now.ToLongTimeString();
                string j = DateTime.Now.ToShortDateString();
                i = i.Replace(":", "_");
                j = j.Replace("/", "_");
                i = j + "_" + i;
                i = i.Replace(" ", "");
                try
                {
                    var URI = new Uri(replaceFielddata(camSnapShot));
                    wc.DownloadFile(URI, camSloc + camName + "_" + i + ".jpg");
                    this.Log.Info(camSloc + camName + "_" + i + ".jpg was created");
                }
                catch (Exception ex)
                {
                    this.Log.Info("ERROR: " + ex.Message);
                }
            }
            else if (sMethod == "SETUP")
            {
                try
                {
                    //System.Windows.Forms.MessageBox.Show("Test");
                }
                catch (Exception ex)
                {
                    this.Log.Info("ERROR: " + ex.Message);
                }
            }
            else 
            {
                try
                {
                    wc.UploadStringAsync(new Uri(replaceFielddata(camCommand)), "POST", "");
                    this.Log.Debug("SENT TO: " + method.ObjectName + ": "  + camCommand);
                }
                catch (Exception ex)
                {
                    this.Log.Info("ERROR: " + ex.Message);
                }
            }
        }

        public override void Shutdown()
        {
            this.Log.Info("IPCamII Plugin has STOPPED");
        }

        public string replaceFielddata(string fieldData)
        {
            string XmlData1 = fieldData.Replace("[address]", camIpAddress);
            string XmlData2 = XmlData1.Replace("[port]", camPort);
            string XmlData3 = XmlData2.Replace("[username]", camUser);
            string XmlData4 = XmlData3.Replace("[password]", camPass);
            string XmlData5 = XmlData4.Replace("[degrees]", camDegrees);
            string XmlData6 = XmlData5.Replace("[filename]", "camera");
            XmlData6 = @"http://" + XmlData6;
            return XmlData6;
        }

    }
}
