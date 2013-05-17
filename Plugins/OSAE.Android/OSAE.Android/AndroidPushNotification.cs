
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Collections.Specialized;
using OSAE;


public class AndroidPushNotification
{

    private Logging logging = Logging.GetLogger("Android");

    public AndroidPushNotification()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public string SendNotification(string deviceId, string message)
    {
        string GoogleAppID = "AIzaSyAXrHDNsYhU-nQowJzLB-YeMyOG74jjjVs";
        var SENDER_ID = "824659990707";
        var value = message;
        WebRequest tRequest;
        tRequest = WebRequest.Create("https://android.googleapis.com/gcm/send");
        tRequest.Method = "post";
        tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
        tRequest.Headers.Add(string.Format("Authorization: key={0}", GoogleAppID));

        tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));

        //string postData = "collapse_key=score_update&time_to_live=108&delay_while_idle=1&data.message=" + value + "&data.time=" + System.DateTime.Now.ToString() + "registration_id=" + deviceId + "";
        string postData = "time_to_live=108&data.message=" + value + "&data.time=" + System.DateTime.Now.ToString() + "registration_id=" + deviceId + "";

        log("postdata = " + postData, false);

        Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
        tRequest.ContentLength = byteArray.Length;

        Stream dataStream = tRequest.GetRequestStream();
        dataStream.Write(byteArray, 0, byteArray.Length);
        
        //StreamReader tReadertmp = new StreamReader(dataStream);
        //String sResponseFromServertmp = tReadertmp.ReadToEnd();
        //tReadertmp.Close();
        //log("requeststream = " + sResponseFromServertmp, false);
        dataStream.Close();


        WebResponse tResponse = tRequest.GetResponse();
        
        dataStream = tResponse.GetResponseStream();

        StreamReader tReader = new StreamReader(dataStream);

        String sResponseFromServer = tReader.ReadToEnd();

        
        tReader.Close();
        dataStream.Close();
        tResponse.Close();
        return sResponseFromServer;
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
