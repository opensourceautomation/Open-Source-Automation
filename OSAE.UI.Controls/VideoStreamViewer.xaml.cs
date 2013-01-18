using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MjpegProcessor;

namespace OSAE.UI.Controls
{
    /// <summary>
    /// Interaction logic for VideoStreamViewer.xaml
    /// </summary>
    public partial class VideoStreamViewer : UserControl
    {
        MjpegDecoder _mjpeg;

        public VideoStreamViewer(string url)
        {
            InitializeComponent();
            _mjpeg = new MjpegDecoder();
            _mjpeg.FrameReady += mjpeg_FrameReady;
            _mjpeg.ParseStream(new Uri(url));
        }
 
        private void mjpeg_FrameReady(object sender, FrameReadyEventArgs e)
        {
            image.Source = e.BitmapImage;
        }
    }
}
