using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
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
        public string CurStateLabel;
        public string CurLevel = "";
        public int LightLevel = 100;

        public string ObjectName;
        public string SliderMethod;
        private OSAEImageManager imgMgr = new OSAEImageManager();
        private OSAEImage img1;
        private OSAEImage img2;
        private OSAEImage img3;
        private OSAEImage img4;
        private int imageFrames = 0;
        private int currentFrame = 0;
        private int frameDelay = 100;
        private bool repeatAnimation;
        private Boolean sliderVisible = false;
        private Boolean updatingSlider = false; 
        private DispatcherTimer timer = new DispatcherTimer();

        public StateImage(OSAEObject sObject)
        {
            InitializeComponent();

            screenObject = sObject;
          //  try
          //  {

                ObjectName = screenObject.Property("Object Name").Value;
                SliderMethod = screenObject.Property("Slider Method").Value;
                CurState = OSAEObjectStateManager.GetObjectStateValue(ObjectName).Value;
                try
                {
                    LightLevel = Convert.ToUInt16(OSAEObjectPropertyManager.GetObjectPropertyValue(ObjectName, "Light Level").Value);
                }
                catch (Exception ex)
                {
                    LightLevel = 100;
                }

                LastStateChange = OSAEObjectStateManager.GetObjectStateValue(ObjectName).LastStateChange;
                Image.ToolTip = ObjectName + "\n" + CurState + " since: " + LastStateChange;

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
                string imgName2 = screenObject.Property(StateMatch + " Image 2").Value;
                string imgName3 = screenObject.Property(StateMatch + " Image 3").Value;
                string imgName4 = screenObject.Property(StateMatch + " Image 4").Value;
                try
                {
                    repeatAnimation = Convert.ToBoolean(screenObject.Property("Repeat Animation").Value);
                }
                catch (Exception ex)
                {
                    OSAEObjectPropertyManager.ObjectPropertySet(screenObject.Name, "Repeat Animation", "TRUE", "GUI");
                    repeatAnimation = true;
                }
                try
                {
                    frameDelay = Convert.ToInt16(screenObject.Property("Frame Delay").Value);
                }
                catch (Exception ex)
                {
                    frameDelay = 100;
                    OSAEObjectPropertyManager.ObjectPropertySet(screenObject.Name, "Frame Delay", "100", "GUI");
                }
                img1 = imgMgr.GetImage(imgName);
                if (img1 != null)
                {
                    var imageStream = new MemoryStream(img1.Data);
                    var bitmapImage = new BitmapImage();

                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = imageStream;
                    bitmapImage.EndInit();

                    Image.Source = bitmapImage;
                    Image.Visibility = System.Windows.Visibility.Visible;
                    
                    imageFrames = 1;
                    currentFrame = 1;
                }
                else
                {
                    Image.Source = null;
                    Image.Visibility = System.Windows.Visibility.Hidden;
                }

                sliderVisible = Convert.ToBoolean(screenObject.Property("Show Slider").Value);

                if (sliderVisible)
                {
                    sldSlider.Visibility = System.Windows.Visibility.Visible;
                    CurLevel = OSAEObjectPropertyManager.GetObjectPropertyValue(ObjectName, "Level").Value;
                    sldSlider.Value = Convert.ToUInt16( CurLevel);
                }

                else
                    sldSlider.Visibility = System.Windows.Visibility.Hidden;

                // Primary Frame is loaded, load up additional frames for the time to display.
                img2 = imgMgr.GetImage(imgName2);
                if (img2 != null)
                    imageFrames = 2;
                img3 = imgMgr.GetImage(imgName3);
                if (img3 != null)
                    imageFrames = 3;
                img4 = imgMgr.GetImage(imgName4);
                if (img4 != null)
                    imageFrames = 4;
        //    }
         //   catch (Exception ex)
         //   {
         //       MessageBox.Show(ex.Message, "StateImage Load");
          //  }

            timer.Interval = TimeSpan.FromMilliseconds(frameDelay);
            timer.Tick += this.timer_Tick;

            if (imageFrames > 1) timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (currentFrame < imageFrames)
                currentFrame += 1;
            else if (currentFrame == imageFrames)
                currentFrame = 1;
            var imageStream = new MemoryStream(img1.Data);;
            switch (currentFrame)
            {
                case 1:
                    imageStream = new MemoryStream(img1.Data);
                    break;
                case 2:
                    imageStream = new MemoryStream(img2.Data);
                    break;
                case 3:
                    imageStream = new MemoryStream(img3.Data);
                    break;
                case 4:
                    imageStream = new MemoryStream(img4.Data);
                    break;
            }
            var bitmapImage = new BitmapImage();

            bitmapImage.BeginInit();
            bitmapImage.StreamSource = imageStream;
            bitmapImage.EndInit();
            Image.Source = bitmapImage;
            Image.Visibility = System.Windows.Visibility.Visible;
        }


        public void Update()
        {
            try
            {
                CurState = OSAEObjectStateManager.GetObjectStateValue(ObjectName).Value;
                CurStateLabel = OSAEObjectStateManager.GetObjectStateValue(ObjectName).StateLabel;
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

            try
            {
                //Location.X = Double.Parse(screenObject.Property(StateMatch + " X").Value);
               //Location.Y = Double.Parse(screenObject.Property(StateMatch + " Y").Value);
                timer.Stop();
                Location.X = Double.Parse(OSAE.OSAEObjectPropertyManager.GetObjectPropertyValue(screenObject.Name, StateMatch + " X").Value);
                Location.Y = Double.Parse(OSAE.OSAEObjectPropertyManager.GetObjectPropertyValue(screenObject.Name, StateMatch + " Y").Value);

                try
                {
                    LightLevel = Convert.ToUInt16(OSAEObjectPropertyManager.GetObjectPropertyValue(ObjectName, "Light Level").Value);
                }
                catch (Exception ex)
                {
                    LightLevel = 100;
                }

                string imgName = screenObject.Property(StateMatch + " Image").Value;
                img1 = imgMgr.GetImage(imgName);
                imgName = screenObject.Property(StateMatch + " Image 2").Value;
                img2 = imgMgr.GetImage(imgName);
                imgName = screenObject.Property(StateMatch + " Image 3").Value;
                img3 = imgMgr.GetImage(imgName);
                imgName = screenObject.Property(StateMatch + " Image 4").Value;
                img4 = imgMgr.GetImage(imgName);
                if (img1 != null)
                {
                    var imageStream = new MemoryStream(img1.Data);
                    imageFrames = 1;
                    this.Dispatcher.Invoke((Action)(() =>
                {
                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = imageStream;
                    bitmapImage.EndInit();
                    Image.Source = bitmapImage;
                    
                    if (sliderVisible && updatingSlider == false)
                    {
                        CurLevel = OSAEObjectPropertyManager.GetObjectPropertyValue(ObjectName, "Level").Value;
                        sldSlider.ToolTip = CurLevel + "%";
                        sldSlider.Value = Convert.ToUInt16(CurLevel);
                    }
                    if (CurLevel != "")
                        Image.ToolTip = ObjectName + "\n" + CurStateLabel + " (" + CurLevel + "%) since: " + LastStateChange;

                    else
                        Image.ToolTip = ObjectName + "\n" + CurStateLabel + " since: " + LastStateChange;
                    }));
                }
                if (img2 != null)
                    imageFrames = 2;
                if (img3 != null)
                    imageFrames = 3;
                if (img4 != null)
                    imageFrames = 4;
                if (imageFrames > 1)
                {
                    timer.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"StateImage Update");
            }
        }

        private void State_Image_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {             
            if (CurState == "ON")
            {
                OSAEMethodManager.MethodQueueAdd(ObjectName, "OFF", "0", "", "GUI");
                OSAEObjectStateManager.ObjectStateSet(ObjectName, "OFF", "GUI");
            }
            else
            {
                OSAEMethodManager.MethodQueueAdd(ObjectName, "ON", "100", "", "GUI");
                OSAEObjectStateManager.ObjectStateSet(ObjectName, "ON", "GUI");
            }        
        }

        private void Slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (updatingSlider) return;

        }

        private void Slider_MouseDown(object sender, MouseButtonEventArgs e)
        {
            updatingSlider = true;
        }

        private void Slider_MouseUp(object sender, MouseButtonEventArgs e)
        {
            OSAEMethodManager.MethodQueueAdd(ObjectName, SliderMethod, Convert.ToUInt16(sldSlider.Value).ToString(), "", "GUI");
            updatingSlider = false;
        }        
    }
}