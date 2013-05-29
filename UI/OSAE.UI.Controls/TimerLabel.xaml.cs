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
using System.Timers;

namespace OSAE.UI.Controls
{
    /// <summary>
    /// Interaction logic for TimerLabel.xaml
    /// </summary>
    public partial class TimerLabel : UserControl
    {
        public OSAEObject screenObject { get; set; }
        public Point Location;
        public DateTime LastUpdated;
        
        private string ObjectName;
        private string CurrentState;
        private int OffTimer;
        private int TimeInState;

        TimeSpan span;
        System.Timers.Timer timer = new System.Timers.Timer();

        public TimerLabel(OSAEObject sObj)
        {
            InitializeComponent();
            screenObject = sObj;            

            ObjectName = screenObject.Property("Object Name").Value;
            OSAEObject timerObj = OSAEObjectManager.GetObjectByName(ObjectName);
            if (timerObj.Property("OFF TIMER").Value != "")
                OffTimer = Int32.Parse(timerObj.Property("OFF TIMER").Value);
            else
                OffTimer = 0;
            CurrentState = timerObj.State.Value;
            TimeInState = (int)timerObj.State.TimeInState;

            string sValue;
            string sBackColor = screenObject.Property("Back Color").Value;
            string sForeColor = screenObject.Property("Fore Color").Value;
            string iFontSize = screenObject.Property("Font Size").Value;
            string sFontName = screenObject.Property("Font Name").Value;

            if (CurrentState == "OFF")
                sValue = "OFF";
            else
            {
                span = TimeSpan.FromSeconds(OffTimer - TimeInState); //Or TimeSpan.FromSeconds(seconds); (see Jakob C´s answer)
                sValue = span.ToString(@"mm\:ss");
            }

            if (sValue != "")
            {
                if (sBackColor != "")
                {
                    try
                    {
                        BrushConverter conv = new BrushConverter();
                        SolidColorBrush brush = conv.ConvertFromString(sBackColor) as SolidColorBrush;
                        timerLabel.Background = brush;
                    }
                    catch (Exception myerror)
                    {
                        
                    }
                }
                if (sForeColor != "")
                {
                    try
                    {
                        BrushConverter conv = new BrushConverter();
                        SolidColorBrush brush = conv.ConvertFromString(sForeColor) as SolidColorBrush;
                        timerLabel.Foreground = brush;
                    }
                    catch (Exception myerror)
                    {
                    }
                }
                if (iFontSize != "")
                {
                    try
                    {
                        timerLabel.FontSize = Convert.ToDouble(iFontSize);
                    }
                    catch (Exception myerror)
                    {
                    }
                }
                timerLabel.Content = sValue;
            }
            else
            {
                timerLabel.Content = "";
            }

            timer.Interval = 1000;
            timer.Enabled = true;
            timer.Elapsed += new ElapsedEventHandler(timer_tick);
        }

        public void Update()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                string sValue;
                OSAEObjectState os = OSAEObjectStateManager.GetObjectStateValue(ObjectName);
                CurrentState = os.Value;

                TimeSpan ts = DateTime.Now - LastUpdated;
                TimeInState = (int)ts.TotalSeconds;
                if (os.Value == "OFF")
                    sValue = "OFF";
                else
                {
                    span = TimeSpan.FromSeconds(OffTimer - TimeInState); //Or TimeSpan.FromSeconds(seconds); (see Jakob C´s answer)
                    sValue = span.ToString(@"mm\:ss");
                }
                timerLabel.Content = sValue;
            }));
        }

        private void timer_tick(object source, EventArgs e)
        {
            if (CurrentState != "OFF")
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    span = span.Subtract(new TimeSpan(0, 0, 1));
                    timerLabel.Content = span.ToString(@"mm\:ss");
                }));
            }
        }
    }
}
