using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Timers;

namespace OSAE.UI.Controls
{
    /// <summary>
    /// Interaction logic for TimerLabel.xaml
    /// </summary>
    public partial class TimerLabel : UserControl
    {
        public OSAEObject screenObject { get; set; }
        public string screenName { get; set; }
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
            screenName = Name;

            ObjectName = screenObject.Property("Object Name").Value;

            OSAEObjectState os = OSAEObjectStateManager.GetObjectStateValue(ObjectName);
            CurrentState = os.Value;

            OffTimer = Convert.ToInt16(OSAEObjectPropertyManager.GetObjectPropertyValue(ObjectName,"OFF TIMER").Value);
            TimeInState = (int)os.TimeInState;

            string sValue;
            string sBackColor = screenObject.Property("Back Color").Value;
            string sForeColor = screenObject.Property("Fore Color").Value;
            string iFontSize = screenObject.Property("Font Size").Value;
            string sFontName = screenObject.Property("Font Name").Value;

            if (CurrentState == "OFF")
                sValue = os.StateLabel;
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
                    catch { }
                }
                if (sForeColor != "")
                {
                    try
                    {
                        BrushConverter conv = new BrushConverter();
                        SolidColorBrush brush = conv.ConvertFromString(sForeColor) as SolidColorBrush;
                        timerLabel.Foreground = brush;
                    }
                    catch { }
                }
                if (iFontSize != "")
                {
                    try
                    { timerLabel.FontSize = Convert.ToDouble(iFontSize); }
                    catch { }
                }
                timerLabel.Content = sValue;
            }
            else
                timerLabel.Content = "";

            timer.Interval = 1000;
            timer.Enabled = true;
            timer.Elapsed += new ElapsedEventHandler(timer_tick);
        }

        public void Update()
        {
            Dispatcher.Invoke((Action)(() =>
            {
                string sValue;
                OSAEObjectState os = OSAEObjectStateManager.GetObjectStateValue(ObjectName);
                CurrentState = os.Value;

                TimeSpan ts = DateTime.Now - LastUpdated;
                TimeInState = (int)ts.TotalSeconds;
                if (os.Value == "OFF")
                {
                    sValue = os.StateLabel;
                    timerLabel.Content = sValue;
                }
                else
                {
                    span = TimeSpan.FromSeconds(OffTimer - TimeInState); //Or TimeSpan.FromSeconds(seconds); (see Jakob C´s answer)
                    sValue = span.ToString(@"mm\:ss");
                }
            }));
        }

        private void timer_tick(object source, EventArgs e)
        {
            if (CurrentState != "OFF")
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    span = span.Subtract(new TimeSpan(0, 0, 1));
                    timerLabel.Content = span.ToString(@"mm\:ss");
                }));
            }
        }
    }
}
