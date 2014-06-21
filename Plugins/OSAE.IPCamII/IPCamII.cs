using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
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
        string ptzUP;
        string ptzDOWN;
        string ptzLEFT;
        string ptzRIGHT;
        string ptzIN;
        string ptzOUT;
        string ptzFOCUSIN;
        string ptzFOCUSOUT;
        string ptzPRESET1;
        string ptzPRESET2;
        string ptzPRESET3;
        string ptzPRESET4;
        string ptzPRESET5;
        string ptzPRESET6;
        string CUSTOM1;
        string CUSTOM2;
        string CUSTOM3;
        string CUSTOM4;
        string CUSTOM5;
        string CUSTOM6;
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
            this.Log.Info("IPCAMII Plugin Found");
            this.Log.Info("===================================================");
        }

        public override void ProcessCommand(OSAEMethod method)
        {
            this.Log.Info("Received Command: " + method.MethodName + " | to Cam: " + method.ObjectName);
            camName = method.ObjectName;
            // OSAEObject obj = OSAEObjectManager.GetObjectByName(camName);
            camIpAddress = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "IP Address").Value;
            camPort = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "Port").Value;
            camUser = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "Username").Value;
            camPass = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "Password").Value;
            camStream = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "Stream Address").Value;
            camSnapShot = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "camSnapShot").Value;
            ptzUP = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "ptzUP").Value;
            ptzDOWN = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "ptzDOWN").Value;
            ptzLEFT = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "ptzLEFT").Value;
            ptzRIGHT = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "ptzRIGHT").Value;
            ptzIN = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "ptzIN").Value;
            ptzOUT = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "ptzOUT").Value;
            ptzFOCUSIN = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "ptzFOCUSIN").Value;
            ptzFOCUSOUT = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "ptzFOCUSOUT").Value;
            ptzPRESET1 = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "ptzPRESET1").Value;
            ptzPRESET2 = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "ptzPRESET2").Value;
            ptzPRESET3 = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "ptzPRESET3").Value;
            ptzPRESET4 = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "ptzPRESET4").Value;
            ptzPRESET5 = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "ptzPRESET5").Value;
            ptzPRESET6 = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "ptzPRESET6").Value;
            CUSTOM1 = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "CUSTOM1").Value;
            CUSTOM2 = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "CUSTOM2").Value;
            CUSTOM3 = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "CUSTOM3").Value;
            CUSTOM4 = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "CUSTOM4").Value;
            CUSTOM5 = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "CUSTOM5").Value;
            CUSTOM6 = OSAEObjectPropertyManager.GetObjectPropertyValue(camName, "CUSTOM6").Value;
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
            if (sMethod == "UP")
            {
                try
                {
                    wc.UploadStringAsync(new Uri(replaceFielddata(ptzUP)), "POST", "");
                }
                catch (Exception ex)
                {
                    this.Log.Info("An error occurred!!!: " + ex.Message);
                }
            }
            else if (sMethod == "DOWN")
            {
                try
                {
                    wc.UploadStringAsync(new Uri(replaceFielddata(ptzDOWN)), "POST", "");
                }
                catch (Exception ex)
                {
                    this.Log.Info("An error occurred!!!: " + ex.Message);
                }
            }
            else if (sMethod == "LEFT")
            {
                try
                {
                    wc.UploadStringAsync(new Uri(replaceFielddata(ptzLEFT)), "POST", "");
                }
                catch (Exception ex)
                {
                    this.Log.Info("An error occurred!!!: " + ex.Message);
                }
            }
            else if (sMethod == "RIGHT")
            {
                try
                {
                    wc.UploadStringAsync(new Uri(replaceFielddata(ptzRIGHT)), "POST", "");
                }
                catch (Exception ex)
                {
                    this.Log.Info("An error occurred!!!: " + ex.Message);
                }
            }
            else if (sMethod == "IN")
            {
                try
                {
                    wc.UploadStringAsync(new Uri(replaceFielddata(ptzIN)), "POST", "");
                }
                catch (Exception ex)
                {
                    this.Log.Info("An error occurred!!!: " + ex.Message);
                }
            }
            else if (sMethod == "OUT")
            {
                try
                {
                    wc.UploadStringAsync(new Uri(replaceFielddata(ptzOUT)), "POST", "");
                }
                catch (Exception ex)
                {
                    this.Log.Info("An error occurred!!!: " + ex.Message);
                }
            }
            else if (sMethod == "FOCUSIN")
            {
                try
                {
                    wc.UploadStringAsync(new Uri(replaceFielddata(ptzFOCUSIN)), "POST", "");
                }
                catch (Exception ex)
                {
                    this.Log.Info("An error occurred!!!: " + ex.Message);
                }
            }
            else if (sMethod == "FOCUSOUT")
            {
                try
                {
                    wc.UploadStringAsync(new Uri(replaceFielddata(ptzFOCUSOUT)), "POST", "");
                }
                catch (Exception ex)
                {
                    this.Log.Info("An error occurred!!!: " + ex.Message);
                }
            }
            else if (sMethod == "PRESET1")
            {
                try
                {
                    wc.UploadStringAsync(new Uri(replaceFielddata(ptzPRESET1)), "POST", "");
                }
                catch (Exception ex)
                {
                    this.Log.Info("An error occurred!!!: " + ex.Message);
                }
            }
            else if (sMethod == "PRESET2")
            {
                try
                {
                    wc.UploadStringAsync(new Uri(replaceFielddata(ptzPRESET2)), "POST", "");
                }
                catch (Exception ex)
                {
                    this.Log.Info("An error occurred!!!: " + ex.Message);
                }
            }
            else if (sMethod == "PRESET3")
            {
                try
                {
                    wc.UploadStringAsync(new Uri(replaceFielddata(ptzPRESET3)), "POST", "");
                }
                catch (Exception ex)
                {
                    this.Log.Info("An error occurred!!!: " + ex.Message);
                }
            }
            else if (sMethod == "PRESET4")
            {
                try
                {
                    wc.UploadStringAsync(new Uri(replaceFielddata(ptzPRESET4)), "POST", "");
                }
                catch (Exception ex)
                {
                    this.Log.Info("An error occurred!!!: " + ex.Message);
                }
            }
            else if (sMethod == "PRESET5")
            {
                try
                {
                    wc.UploadStringAsync(new Uri(replaceFielddata(ptzPRESET5)), "POST", "");
                }
                catch (Exception ex)
                {
                    this.Log.Info("An error occurred!!!: " + ex.Message);
                }
            }
            else if (sMethod == "PRESET6")
            {
                try
                {
                    wc.UploadStringAsync(new Uri(replaceFielddata(ptzPRESET6)), "POST", "");
                }
                catch (Exception ex)
                {
                    this.Log.Info("An error occurred!!!: " + ex.Message);
                }
            }
            else if (sMethod == "CUSTOM1")
            {
                try
                {
                    wc.UploadStringAsync(new Uri(replaceFielddata(CUSTOM1)), "POST", "");
                }
                catch (Exception ex)
                {
                    this.Log.Info("An error occurred!!!: " + ex.Message);
                }
            }
            else if (sMethod == "CUSTOM2")
            {
                try
                {
                    wc.UploadStringAsync(new Uri(replaceFielddata(CUSTOM2)), "POST", "");
                }
                catch (Exception ex)
                {
                    this.Log.Info("An error occurred!!!: " + ex.Message);
                }
            }
            else if (sMethod == "CUSTOM3")
            {
                try
                {
                    wc.UploadStringAsync(new Uri(replaceFielddata(CUSTOM3)), "POST", "");
                }
                catch (Exception ex)
                {
                    this.Log.Info("An error occurred!!!: " + ex.Message);
                }
            }
            else if (sMethod == "CUSTOM4")
            {
                try
                {
                    wc.UploadStringAsync(new Uri(replaceFielddata(CUSTOM4)), "POST", "");
                }
                catch (Exception ex)
                {
                    this.Log.Info("An error occurred!!!: " + ex.Message);
                }
            }
            else if (sMethod == "CUSTOM5")
            {
                try
                {
                    wc.UploadStringAsync(new Uri(replaceFielddata(CUSTOM5)), "POST", "");
                }
                catch (Exception ex)
                {
                    this.Log.Info("An error occurred!!!: " + ex.Message);
                }
            }
            else if (sMethod == "CUSTOM6")
            {
                try
                {
                    wc.UploadStringAsync(new Uri(replaceFielddata(CUSTOM6)), "POST", "");
                }
                catch (Exception ex)
                {
                    this.Log.Info("An error occurred!!!: " + ex.Message);
                }
            }
            else if (sMethod == "SNAPSHOT")
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
                    this.Log.Info("An error occurred!!!: " + ex.Message);
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
                    this.Log.Info("An error occurred!!!: " + ex.Message);
                }
            }
            this.Log.Info("===================================================");
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
