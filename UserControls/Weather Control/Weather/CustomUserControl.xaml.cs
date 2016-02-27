using System;
using System.Net;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OSAE;
using PluginInterface;

namespace OSAE.Weather_Control
{
    /// <summary>
    /// Interaction logic for WeatherControl.xaml
    /// </summary>
    public partial class CustomUserControl : UserControl
    {
        OSAEObject weatherObj;
        public Point Location;
        public string _controlname;
        public int ControlWidth;
        public int ControlHeight;
        public OSAEObject screenObject = new OSAEObject();
        public string CurState;
        public string CurStateLabel;
        public DateTime LastUpdated;
        public DateTime LastStateChange;
        public string objName;
        string sMode = "Max";
        private string gAppName = "";
        private string currentUser;

        public CustomUserControl(OSAEObject sObj, string ControlName, string appName, string user)
        {
            InitializeComponent();
            gAppName = appName;
            currentUser = user;
            _controlname = ControlName;
            screenObject = sObj;
            objName = sObj.Property("Object Name").Value;

            try
            {
                ControlWidth = Convert.ToInt32(OSAEObjectPropertyManager.GetObjectPropertyValue(screenObject.Name, "Width").Value);
                ControlHeight = Convert.ToInt32(OSAEObjectPropertyManager.GetObjectPropertyValue(screenObject.Name, "Height").Value);
            }
            catch (Exception ex)
            { }

            string sBackColor = screenObject.Property("Back Color").Value;
            string sForeColor = screenObject.Property("Fore Color").Value;
            string iFontSize = screenObject.Property("Font Size").Value;
            string sFontName = screenObject.Property("Font Name").Value;
            if (sBackColor != "")
            {
                try
                {
                    BrushConverter conv = new BrushConverter();
                    SolidColorBrush brush = conv.ConvertFromString(sBackColor) as SolidColorBrush;
                    this.Background = brush;
                }
                catch (Exception)
                { }
            }
            if (sForeColor != "")
            {
                try
                {
                    BrushConverter conv = new BrushConverter();
                    SolidColorBrush brush = conv.ConvertFromString(sForeColor) as SolidColorBrush;
                    lblCurTemp.Foreground = brush;
                    lblConditions.Foreground = brush;
                    lblDay1.Foreground = brush;
                    lblDay2.Foreground = brush;
                    lblDay3.Foreground = brush;
                    lblDay4.Foreground = brush;
                    lblDay5.Foreground = brush;
                    lblForcast.Foreground = brush;
                }
                catch (Exception)
                { }
            }
            if (iFontSize != "")
            {
                try
                {
                    lblConditions.FontSize = Convert.ToDouble(iFontSize);
                    lblDay1.FontSize = Convert.ToDouble(iFontSize);
                    lblDay2.FontSize = Convert.ToDouble(iFontSize);
                    lblDay3.FontSize = Convert.ToDouble(iFontSize);
                    lblDay4.FontSize = Convert.ToDouble(iFontSize);
                    lblDay5.FontSize = Convert.ToDouble(iFontSize);
                    lblForcast.FontSize = Convert.ToDouble(iFontSize);
                }
                catch (Exception)
                { }
            }
            if (sFontName != "")
            {
                try
                {
                    lblConditions.FontFamily = new FontFamily(sFontName);
                    lblDay1.FontFamily = new FontFamily(sFontName);
                    lblDay2.FontFamily = new FontFamily(sFontName);
                    lblDay3.FontFamily = new FontFamily(sFontName);
                    lblDay4.FontFamily = new FontFamily(sFontName);
                    lblDay5.FontFamily = new FontFamily(sFontName);
                    lblForcast.FontFamily = new FontFamily(sFontName);
                }
                catch (Exception)
                { }
            }

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(timMain_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 30, 0);
            dispatcherTimer.Start();
            Load_All_Weather();
        }

        public void setLocation(double X, double Y)
        {
            Location.X = X;
            Location.Y = Y;
        }

        private void Load_All_Weather()
        {
            weatherObj = OSAEObjectManager.GetObjectByName("Weather");
            lblCurTemp.Content = weatherObj.Property("Temp").Value + "°";
            lblConditions.Content = weatherObj.Property("Today Forecast").Value;

            LoadLows();
            LoadHighs();
            LoadDayLabels();
            LoadDaySummaryLabels();
            LoadNightSummaryLabels();
            LoadDates();
            LoadImageControls();

            lblForcast.Text = "";
        }
        private void LoadNightSummaryLabels()
        {
            imgDay1Night.Tag = weatherObj.Property("Night1 Summary").Value;
            imgDay2Night.Tag = weatherObj.Property("Night2 Summary").Value;
            imgDay3Night.Tag = weatherObj.Property("Night3 Summary").Value;
            imgDay4Night.Tag = weatherObj.Property("Night4 Summary").Value;
            imgDay5Night.Tag = weatherObj.Property("Night5 Summary").Value;
        }
        private void LoadDaySummaryLabels()
        {
            imgDay1Day.Tag = weatherObj.Property("Day1 Summary").Value;
            imgDay2Day.Tag = weatherObj.Property("Day2 Summary").Value;
            imgDay3Day.Tag = weatherObj.Property("Day3 Summary").Value;
            imgDay4Day.Tag = weatherObj.Property("Day4 Summary").Value;
            imgDay5Day.Tag = weatherObj.Property("Day5 Summary").Value;
        }
        private void LoadDayLabels()
        {
            lblDay1.Tag = weatherObj.Property("Day1 Forecast").Value;
            lblDay2.Tag = weatherObj.Property("Day2 Forecast").Value;
            lblDay3.Tag = weatherObj.Property("Day3 Forecast").Value;
            lblDay4.Tag = weatherObj.Property("Day4 Forecast").Value;
            lblDay5.Tag = weatherObj.Property("Day5 Forecast").Value;
        }
        private void LoadHighs()
        {
            lblTodayHi.Content = string.Format("{0}°", weatherObj.Property("Day1 High").Value);
            lblDay1Hi.Content = string.Format("{0}°", weatherObj.Property("Day1 High").Value);
            lblDay2Hi.Content = string.Format("{0}°", weatherObj.Property("Day2 High").Value);
            lblDay3Hi.Content = string.Format("{0}°", weatherObj.Property("Day3 High").Value);
            lblDay4Hi.Content = string.Format("{0}°", weatherObj.Property("Day4 High").Value);
            lblDay5Hi.Content = string.Format("{0}°", weatherObj.Property("Day5 High").Value);
        }
        private void LoadLows()
        {
            lblTodayLo.Content = string.Format("{0}°", weatherObj.Property("Night1 Low").Value);
            lblDay1Lo.Content = string.Format("{0}°", weatherObj.Property("Night1 Low").Value);
            lblDay2Lo.Content = string.Format("{0}°", weatherObj.Property("Night2 Low").Value);
            lblDay3Lo.Content = string.Format("{0}°", weatherObj.Property("Night3 Low").Value);
            lblDay4Lo.Content = string.Format("{0}°", weatherObj.Property("Night4 Low").Value);
            lblDay5Lo.Content = string.Format("{0}°", weatherObj.Property("Night5 Low").Value);
        }
        private void LoadDates()
        {
            lblDay1.Content = DateTime.Now.AddDays(0).DayOfWeek;
            lblDay2.Content = DateTime.Now.AddDays(1).DayOfWeek;
            lblDay3.Content = DateTime.Now.AddDays(2).DayOfWeek;
            lblDay4.Content = DateTime.Now.AddDays(3).DayOfWeek;
            lblDay5.Content = DateTime.Now.AddDays(4).DayOfWeek;
        }
        private void LoadImageControls()
        {
            LoadImages("Today Image", imgTodayDay);
            LoadImages("Tonight Image", imgTodayNight);
            LoadImages("Day1 Image", imgDay1Day);
            LoadImages("Day2 Image", imgDay2Day);
            LoadImages("Day3 Image", imgDay3Day);
            LoadImages("Day4 Image", imgDay4Day);
            LoadImages("Day5 Image", imgDay5Day);
            LoadImages("Night1 Image", imgDay1Night);
            LoadImages("Night2 Image", imgDay2Night);
            LoadImages("Night3 Image", imgDay3Night);
            LoadImages("Night4 Image", imgDay4Night);
            LoadImages("Night5 Image", imgDay5Night);
        }

        private void LoadImages(string key, System.Windows.Controls.Image imageBox)
        {
            dynamic imageName = weatherObj.Property(key).Value;
            if (string.IsNullOrEmpty(imageName))
                return;

            Uri url = new Uri(imageName);
            string path = string.Format("{0}\\images\\Weather\\{1}", Common.ApiPath, System.IO.Path.GetFileName(url.LocalPath));
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
            if (File.Exists(path))
            {
                ImageSource imageSource = new BitmapImage(new Uri(path));
                imageBox.Source = imageSource;
            }
            else
            {
                try
                {
                    WebClient webClient = new WebClient();
                    webClient.DownloadFile(url.OriginalString, path);
                    ImageSource imageSource = new BitmapImage(new Uri(path));
                    imageBox.Source = imageSource;

                }
                catch { }
            }
        }

        private void Grid_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
        }

        private void imageHover(object sender, EventArgs e)
        {
            System.Windows.Controls.Image img = (System.Windows.Controls.Image)sender;
            lblForcast.Text = img.Tag.ToString();
        }

        private void imageLeave(object sender, EventArgs e)
        {
            lblForcast.Text = "";
        }


        private void timMain_Tick(object sender, EventArgs e)
        {
            Load_All_Weather();
        }

        private void imgTodayDay_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sMode == "Max")
            {
                sMode = "Min";
                this.Width = 85;
            }
            else
            {
                sMode = "Max";
                this.Width = 440;
            }
        }

        public void Update()
        {
            bool stateChanged = false;
            OSAEObjectState stateCurrent = OSAEObjectStateManager.GetObjectStateValue(objName);
            if (this.CurState != stateCurrent.Value) stateChanged = true;
            this.CurState = stateCurrent.Value;
            this.CurStateLabel = stateCurrent.StateLabel;
            this.LastStateChange = stateCurrent.LastStateChange;
            if (stateChanged)
            {
                // add update code here!
            }
        }
    }
}
