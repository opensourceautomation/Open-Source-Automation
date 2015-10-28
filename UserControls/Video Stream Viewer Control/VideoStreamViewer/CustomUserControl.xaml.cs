using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OSAE;
using PluginInterface;
using MjpegProcessor;

namespace VideoStreamViewer
{
    /// <summary>
    /// Interaction logic for CustomUserControl.xaml
    /// </summary>
    public partial class CustomUserControl : UserControl
    {

        // add OSAE General Logging. Not all controls need Logging.
        private OSAE.General.OSAELog Log = new OSAE.General.OSAELog();

        // Create a Screen Object:
        public OSAEObject screenObject = new OSAEObject();

        // Set any initial values
        MjpegDecoder _mjpeg;
        public Point Location;
        public string _controlname;
        string newData;
        double imgWidth = 400;
        double imgHeight = 300;
        string streamURI;

        // Code to Initialize your custom User Control
        public CustomUserControl(OSAEObject sObj, string ControlName)
        {
            InitializeComponent();
            _controlname = ControlName;
            screenObject = sObj;
            _mjpeg = new MjpegDecoder();
            _mjpeg.FrameReady += mjpeg_FrameReady;
            _mjpeg.Error += _mjpeg_Error;
            var imgsWidth = OSAEObjectPropertyManager.GetObjectPropertyValue(sObj.Property("Object Name").Value, "Width").Value;
            var imgsHeight = OSAEObjectPropertyManager.GetObjectPropertyValue(sObj.Property("Object Name").Value, "Height").Value;
            streamURI = OSAEObjectPropertyManager.GetObjectPropertyValue(sObj.Property("Object Name").Value, "Stream Address").Value;
            if (imgsWidth != "") { imgWidth = Convert.ToDouble(imgsWidth); }
            if (imgsHeight != "") { imgHeight = Convert.ToDouble(imgsHeight); }
            this.Width = imgWidth;
            this.Height = imgHeight;
            image.Width = imgWidth;
            image.Height = imgHeight;
            if (streamURI == null)
            {
                this.Log.Error("Stream Path Not Found: " + streamURI);
                message.Content = "Can Not Open: " + streamURI;
            }
            else
            {
                streamURI = renameingSys(streamURI);
                this.Log.Info("Streaming: " + streamURI);
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
                {
                    newData = newData.Replace("[" + renameProperty + "]", getProperty);
                }
                else
                {
                    this.Log.Error("Property has NO data");
                }
                if (getProperty == null)
                {
                    this.Log.Error("Property: NOT FOUND");
                }
            }
            newData = @"http://" + newData;
            return newData;
        }

        private void _mjpeg_Error(object sender, MjpegProcessor.ErrorEventArgs e)
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
            this.Log.Info("Stopping stream:" + streamURI);
        }

    }
}
