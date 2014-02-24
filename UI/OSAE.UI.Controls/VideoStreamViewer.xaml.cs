using System;
using System.Windows;
using System.Windows.Controls;
using System.Net;
using MjpegProcessor;

namespace OSAE.UI.Controls
{
    /// <summary>
    /// Interaction logic for VideoStreamViewer.xaml
    /// </summary>
    public partial class VideoStreamViewer : UserControl
    {
        MjpegDecoder _mjpeg;
        public Point Location;
        public OSAEObject screenObject = new OSAEObject();
        //OSAELog
        private OSAE.General.OSAELog Log = new General.OSAELog();

        string CamipAddress;
        string CamPort;
        string UserName;
        string Password;
        double imgWidth = 400;
        double imgHeight = 300;

        public VideoStreamViewer(string url, OSAEObject obj)
        {
            InitializeComponent();
            screenObject = obj;
            _mjpeg = new MjpegDecoder();
            _mjpeg.FrameReady += mjpeg_FrameReady;
            _mjpeg.Error += _mjpeg_Error;
            CamipAddress = OSAEObjectPropertyManager.GetObjectPropertyValue(obj.Property("Object Name").Value, "IP Address").Value;
            CamPort = OSAEObjectPropertyManager.GetObjectPropertyValue(obj.Property("Object Name").Value, "Port").Value;         
            UserName = OSAEObjectPropertyManager.GetObjectPropertyValue(obj.Property("Object Name").Value, "Username").Value;
            Password = OSAEObjectPropertyManager.GetObjectPropertyValue(obj.Property("Object Name").Value, "Password").Value;
            var imgsWidth = OSAEObjectPropertyManager.GetObjectPropertyValue(obj.Property("Object Name").Value, "Width").Value;
            var imgsHeight = OSAEObjectPropertyManager.GetObjectPropertyValue(obj.Property("Object Name").Value, "Height").Value;
            string streamURI = OSAEObjectPropertyManager.GetObjectPropertyValue(obj.Property("Object Name").Value, "Stream Address").Value;
            if (imgsWidth != "") { imgWidth = Convert.ToDouble(imgsWidth); }
            if (imgsHeight != "") { imgHeight = Convert.ToDouble(imgsHeight); }
            this.Width = imgWidth;
            this.Height = imgHeight;
            image.Width = imgWidth;
            image.Height = imgHeight;
            if (streamURI == null)
            {
                this.Log.Info("Stream Path Not Found: " + streamURI);
                message.Content = "Can Not Open: " + streamURI;
            }
            else
            {
                streamURI = replaceFielddata(streamURI);
                _mjpeg.ParseStream(new Uri(streamURI));
            }
        }

        public string replaceFielddata(string fieldData)
        {
            string XmlData1 = fieldData.Replace("[address]", CamipAddress);
            string XmlData2 = XmlData1.Replace("[port]", CamPort);
            string XmlData3 = XmlData2.Replace("[username]", UserName);
            string XmlData4 = XmlData3.Replace("[password]", Password);
            XmlData4 = @"http://" + XmlData4;
            return XmlData4;
        }

        private void _mjpeg_Error(object sender, ErrorEventArgs e)
        {
            this.Log.Error("Error parsing stream:" + e.Message);
        }
 
        private void mjpeg_FrameReady(object sender, FrameReadyEventArgs e)
        {
            image.Source = e.BitmapImage;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _mjpeg.StopStream();
        }
    }
}
