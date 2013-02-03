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
using System.IO;

namespace OSAE.UI.Controls
{
    /// <summary>
    /// Interaction logic for NavigationImage.xaml
    /// </summary>
    public partial class NavigationImage : UserControl
    {
        public string screenName { get; set; }
        public Point Location;
        public OSAEObject screenObject { get; set; }

        private string imgPath;
        private OSAE osae = new OSAE("GUI");

        public NavigationImage(string Name, string path, OSAEObject sObj)
        {
            InitializeComponent();
            screenObject = sObj;
            screenName = Name;
            imgPath = Common.ApiPath + path;

            Image.Tag = screenName;
            if (File.Exists(imgPath))
            {
                byte[] byteArray = File.ReadAllBytes(imgPath);
                var imageStream = new MemoryStream(byteArray);
                var bitmapImage = new BitmapImage();

                bitmapImage.BeginInit();
                bitmapImage.StreamSource = imageStream;
                bitmapImage.EndInit();
                Image.Source = bitmapImage;
                Image.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                Image.Source = null;
                Image.Visibility = System.Windows.Visibility.Hidden;
            }

        }
    }
}
