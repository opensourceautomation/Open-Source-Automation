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

        public VideoStreamViewer(string url, OSAEObject obj)
        {
            InitializeComponent();
            screenObject = obj;
            _mjpeg = new MjpegDecoder();
            _mjpeg.FrameReady += mjpeg_FrameReady;
            
            
            _mjpeg.ParseStream(new Uri(url), "Admin", "123456");
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
