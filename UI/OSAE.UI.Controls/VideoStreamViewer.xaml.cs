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
        private OSAE.General.OSAELog Log;// = new General.OSAELog();     
        string newData;
        double imgWidth = 400;
        double imgHeight = 300;
        double imgRatio;
        public double ControlWidth;
        public double ControlHeight;
        string streamURI;

        public VideoStreamViewer(string url, OSAEObject obj, string appName)
        {
            InitializeComponent();
            Log = new General.OSAELog(appName);
            screenObject = obj;
            _mjpeg = new MjpegDecoder();
            _mjpeg.FrameReady += mjpeg_FrameReady;
            _mjpeg.Error += _mjpeg_Error;
            imgWidth = Convert.ToDouble(OSAEObjectPropertyManager.GetObjectPropertyValue(obj.Property("Object Name").Value, "Width").Value);
            imgHeight = Convert.ToDouble(OSAEObjectPropertyManager.GetObjectPropertyValue(obj.Property("Object Name").Value, "Height").Value);
            ControlWidth = Convert.ToDouble(OSAEObjectPropertyManager.GetObjectPropertyValue(obj.Name, "Width").Value);
            imgRatio = ControlWidth / imgWidth;
            ControlHeight = Convert.ToDouble(imgHeight * imgRatio);
            streamURI = OSAEObjectPropertyManager.GetObjectPropertyValue(obj.Property("Object Name").Value, "Stream Address").Value;
          //  if (imgWidth > 0) { imgWidth = imgWidth); }
           // if (imgHeight > 0) { imgHeight = Convert.ToDouble(imgHeight); }
            this.Width = ControlWidth;
            this.Height = ControlHeight;
            
            image.Width = ControlWidth;
            image.Height = ControlHeight;
            if (streamURI == null)
            {
                Log.Error("Stream Path Not Found: " + streamURI);
                message.Content = "Can Not Open: " + streamURI;
            }
            else
            {
                streamURI = renameingSys(streamURI);
                Log.Info("Streaming: " + streamURI);
                _mjpeg.ParseStream(new Uri(streamURI));
            }
        }

        public string renameingSys(string fieldData)
        {
            newData = fieldData.Replace("http://", "");
            while (newData.IndexOf("[") != -1)
            {
                int ss = newData.IndexOf("[");
                int es = newData.IndexOf("]");
                string renameProperty = newData.Substring(ss + 1, (es - ss) - 1);
                string getProperty = OSAEObjectPropertyManager.GetObjectPropertyValue(screenObject.Property("Object Name").Value, renameProperty).Value;
                // log any errors
                if (getProperty.Length > 0)
                    newData = newData.Replace("[" + renameProperty + "]", getProperty);
                else
                    Log.Error("Property has NO data");

                if (getProperty == null) Log.Error("Property: NOT FOUND");
            }
            newData = @"http://" + newData;
            return newData;
        }

        private void _mjpeg_Error(object sender, ErrorEventArgs e)
        {
            Log.Error("Error parsing stream:" + e.Message);
        }
 
        private void mjpeg_FrameReady(object sender, FrameReadyEventArgs e)
        {
            image.Source = e.BitmapImage;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _mjpeg.StopStream();
            Log.Info("Stopping stream:" + streamURI);
        }
    }
}
