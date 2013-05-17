
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.IO;
using OSAE;

namespace OSAE.Android
{
    
    public class GCMSender
    {
        private const string GCM_URI = "https://android.googleapis.com/gcm/send";

        private Logging logging = Logging.GetLogger("Android");

        #region Properties
        public string DeviceToken { get; set; }
        public string APIKey { get; set; }
        public string ResultJSON { get; private set; }
        #endregion

        HttpWebRequest gcmRequest = null;
        HttpWebResponse gcmResponse = null;

        public GCMSender()
        {
            // do nothing
        }

        public GCMSender(string deviceToken, string apiKey)
        {
            DeviceToken = deviceToken;
            APIKey = apiKey;
        }

        public string Send(string message)
        {
            // Escape condition
            if (DeviceToken == null || APIKey == null)
            {
                return "[ERROR] Device Token or API Key has not been set";
            }

            InitGCMClient();
            PostPayload(message);

            try
            {
                gcmResponse = gcmRequest.GetResponse() as HttpWebResponse;
            }
            catch (WebException we)
            {
                return "[ERROR] There is a problem within processing GCM message \n" + we.Message;
            }
            ResultJSON = ReadResponse(gcmResponse);

            return ResultJSON;
        }

        private string ReadResponse(HttpWebResponse response)
        {
            StreamReader responseReader = new StreamReader(response.GetResponseStream());
            return responseReader.ReadToEnd();
        }

        private void InitGCMClient()
        {
            gcmRequest = WebRequest.Create(GCM_URI) as HttpWebRequest;
            gcmRequest.ContentType = "application/json";
            gcmRequest.UserAgent = "Android GCM Message Sender Client 1.0";
            gcmRequest.Method = "POST";

            // Credential info
            gcmRequest.Headers.Add("Authorization", "key=" + APIKey);
        }

        private void PostPayload(string message)
        {
            // Ready
            string payloadString = AssembleJSONPayload(DeviceToken, message);
            byte[] payloadByte = Encoding.UTF8.GetBytes(payloadString);
            gcmRequest.ContentLength = payloadByte.Length;

            // Go
            using (Stream payloadStream = gcmRequest.GetRequestStream())
            {    
                payloadStream.Write(payloadByte, 0, payloadByte.Length);
                payloadStream.Close();
            }
        }

        private string AssembleJSONPayload(string gcmDeviceToken, string gcmBody)
        {
            string payloadFormatJSON =
                "{{" +
                    "\"registration_ids\" : [\"{0}\"]," +
                    "\"data\" : {{" +
                        " {1} " +
                    "}}" +
                "}}";

            string payload = string.Format(payloadFormatJSON, gcmDeviceToken, gcmBody);
            Debug.WriteLine("payload : " + payload);

            return payload;
        }

        private void log(String message, bool alwaysLog)
        {
            try
            {
                logging.AddToLog(message, alwaysLog);
            }
            catch (IOException ex)
            {
                //do nothing
            }


        }
    }

}
