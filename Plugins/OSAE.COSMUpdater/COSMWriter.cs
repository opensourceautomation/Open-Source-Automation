using System;
using System.IO;
using System.Net;
using System.Text;

namespace OSAE.COSMUpdater
{
    public class COSMWriter 
    {
        Logging logging = Logging.GetLogger("COSM");
        const string baseUri = "http://api.cosm.com/v2/feeds/";

        public void Send(string apiKey, string feedId, string dataStream, string sample)
        {
            try
            {
                var request = CreateRequest(apiKey, feedId, dataStream, sample);
                request.Timeout = 60 * 1000;
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    HandleResponse(response);
                }
            }
            catch (Exception e)
            {
                logging.AddToLog("COSMWriter.Send Exception " + e.Message, true);
            }
        }
        private HttpWebRequest CreateRequest(string apiKey, string feedId, string dataStream, string sample)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(sample);

            var FullURL = string.Format("{0}{1}/datastreams/{2}", baseUri, feedId, dataStream);
            logging.AddToLog("COSMWriter.CreateRequest URL : " + FullURL, false);
            var request = (HttpWebRequest)WebRequest.Create(FullURL);

            // request line
            request.Method = "PUT";

            // request headers
            request.ContentLength = buffer.Length;
            request.ContentType = "text/csv";
            request.Headers.Add("X-ApiKey", apiKey);

            // request body
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(buffer, 0, buffer.Length);
            }

            return request;
        }

        private void HandleResponse(HttpWebResponse response)
        {
            logging.AddToLog("COSMWriter.HandleResponse Status code from COSM : " + response.StatusCode, false);
        }
    }
}
