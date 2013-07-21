using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace OSAE.UI.Controls
{
    /// <summary>
    /// Interaction logic for StateImage.xaml
    /// </summary>
    public partial class StateImage : UserControl
    {
        public OSAEObject screenObject { get; set; }
        public Point Location;
        public DateTime LastUpdated;
        public DateTime LastStateChange;
        
        public string StateMatch;
        public string CurState;

        public string ObjectName;
        private OSAEImageManager imgMgr = new OSAEImageManager();

        public StateImage(OSAEObject sObject)
        {
            InitializeComponent();

            screenObject = sObject;
            try
            {

                ObjectName = screenObject.Property("Object Name").Value;
                CurState = OSAEObjectStateManager.GetObjectStateValue(ObjectName).Value;
                LastStateChange = OSAEObjectStateManager.GetObjectStateValue(ObjectName).LastStateChange;

                Image.Tag = ObjectName;
                Image.MouseLeftButtonUp += new MouseButtonEventHandler(State_Image_MouseLeftButtonUp);

                foreach (OSAEObjectProperty p in screenObject.Properties)
                {
                    if (p.Value.ToLower() == CurState.ToLower())
                    {
                        StateMatch = p.Name.Substring(0, p.Name.LastIndexOf(' '));
                    }
                }

                string imgName = screenObject.Property(StateMatch + " Image").Value;
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
            catch (Exception ex)
            {
               
            }

        }

        public void Update()
        {
            try
            {
                CurState = OSAEObjectStateManager.GetObjectStateValue(ObjectName).Value;
                LastStateChange = OSAEObjectStateManager.GetObjectStateValue(ObjectName).LastStateChange;
            }
            catch (Exception ex)
            {

            }
            foreach (OSAEObjectProperty p in screenObject.Properties)
            {
                if (p.Value.ToLower() == CurState.ToLower())
                {
                    StateMatch = p.Name.Substring(0, p.Name.LastIndexOf(' '));
                }
            }

            Location.X = Double.Parse(screenObject.Property(StateMatch + " X").Value);
            Location.Y = Double.Parse(screenObject.Property(StateMatch + " Y").Value);

            string imgName = screenObject.Property(StateMatch + " Image").Value;
            OSAEImage img = imgMgr.GetImage(imgName);

            if (img.Data != null)
            {
                var imageStream = new MemoryStream(img.Data);


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
            if (CurState == "ON")
            {
                OSAEMethodManager.MethodQueueAdd(ObjectName, "OFF", "", "", "GUI");
                OSAEObjectStateManager.ObjectStateSet(ObjectName, "OFF", "GUI");
            }
            else
            {
                OSAEMethodManager.MethodQueueAdd(ObjectName, "ON", "", "", "GUI");
                OSAEObjectStateManager.ObjectStateSet(ObjectName, "ON", "GUI");
            }        
        }            
    }
}