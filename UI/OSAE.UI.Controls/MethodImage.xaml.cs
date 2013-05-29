using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace OSAE.UI.Controls
{
    /// <summary>
    /// Interaction logic for MethodImage.xaml
    /// </summary>
    public partial class MethodImage : UserControl
    {
        public Point Location;
        public OSAEObject screenObject { get; set; }

        private string ObjectName;
        private string MethodName;
        private string Param1;
        private string Param2;
        private OSAEImageManager imgMgr = new OSAEImageManager();

        public MethodImage(OSAEObject sObj)
        {
            InitializeComponent();
            screenObject = sObj;
            ObjectName = screenObject.Property("Object Name").Value; ;
            MethodName = screenObject.Property("Method Name").Value; ;
            Param1 = screenObject.Property("Param 1").Value; ;
            Param2 = screenObject.Property("Param 2").Value; ;

            Image.Tag = ObjectName + " - " + MethodName;
            Image.MouseLeftButtonUp += new MouseButtonEventHandler(Method_Image_MouseLeftButtonUp);

            string imgName = screenObject.Property("Image").Value;
            OSAEImage img = imgMgr.GetImage(imgName);

            if (img.Data != null)
            {
                var imageStream = new MemoryStream(img.Data);
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

        private void Method_Image_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            OSAEMethodManager.MethodQueueAdd(ObjectName, MethodName, Param1, Param2, "GUI");
        }     
    }
}
