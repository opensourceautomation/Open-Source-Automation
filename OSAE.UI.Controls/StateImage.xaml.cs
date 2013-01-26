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
    /// Interaction logic for StateImage.xaml
    /// </summary>
    public partial class StateImage : UserControl
    {
        OSAE osae = new OSAE("GUI");

        public string ObjectName { get; set; }
        public string ObjectState { get; set; }
        public string ObjectStateTime { get; set; }
        public OSAEObject screenObject { get; set; }
        public Point Location;

        private OSAEObject representedObject;

        public StateImage(OSAEObject sObject, OSAEObject osaObject)
        {
            InitializeComponent();

            screenObject = sObject;
            ObjectName = osaObject.Name;
            representedObject = osaObject;
            Image.Tag = ObjectName;
            Image.MouseLeftButtonUp += new MouseButtonEventHandler(State_Image_MouseLeftButtonUp);

            ObjectStateTime = osaObject.LastUpd;
            ObjectState = osaObject.State.Value;

            string stateMatch = "";
            string imgPath;
            foreach (ObjectProperty p in screenObject.Properties)
            {
                if (p.Value.ToLower() == representedObject.State.Value.ToLower())
                {
                    stateMatch = p.Name.Substring(0, p.Name.LastIndexOf(' '));
                }
            }

            imgPath = osae.APIpath + screenObject.Property(stateMatch + " Image").Value;

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

        public void Update()
        {
            string sStateMatch = "";

            foreach (ObjectProperty p in screenObject.Properties)
            {
                if (p.Value.ToLower() == ObjectState.ToLower())
                {
                    sStateMatch = p.Name.Substring(0, p.Name.LastIndexOf(' '));
                }
            }
            String imagePath = screenObject.Property(sStateMatch + " Image").Value;
            if (File.Exists(osae.APIpath + imagePath))
            {
                imagePath = osae.APIpath + imagePath;
            }

            if (File.Exists(imagePath))
            {
                
                byte[] byteArray = File.ReadAllBytes(imagePath);
                var imageStream = new MemoryStream(byteArray);


                this.Dispatcher.Invoke((Action)(() =>
                {
                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = imageStream;
                    bitmapImage.EndInit();
                    Image.Source = bitmapImage;
                }));
            }
        }

        private void State_Image_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            bool iResults = false;

            if (ObjectState == "ON")
            {
                List<string> methods = representedObject.Methods;
                foreach (string m in methods)
                {
                    if (m == "ON")
                        iResults = true;
                }
                if (iResults)
                    osae.MethodQueueAdd(ObjectName, "OFF", "", "");
                else
                    osae.ObjectStateSet(ObjectName, "OFF");
            }
            else
            {
                List<string> methods = representedObject.Methods;
                foreach (string m in methods)
                {
                    if (m == "OFF")
                        iResults = true;
                }
                if (iResults)
                    osae.MethodQueueAdd(ObjectName, "ON", "", "");
                else
                    osae.ObjectStateSet(ObjectName, "ON");
            }
            
        }

        
    }
}
