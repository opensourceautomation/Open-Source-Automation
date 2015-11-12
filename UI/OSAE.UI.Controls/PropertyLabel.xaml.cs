namespace OSAE.UI.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using OSAE;

    /// <summary>
    /// Interaction logic for PropertyLabel.xaml
    /// </summary>
    public partial class PropertyLabel : UserControl
    {
        public OSAEObject screenObject { get; set; }
        public Point Location;
        public DateTime LastUpdated;
        
        private string ObjectName;
        private string PropertyName;
        private string LastState;
        private DateTime LastStateChange;

        public PropertyLabel(OSAEObject sObj)
        {
            InitializeComponent();
            screenObject = sObj;
            ObjectName = screenObject.Property("Object Name").Value;
            PropertyName = screenObject.Property("Property Name").Value;

            string sPropertyValue;
            if (string.Equals(PropertyName, "STATE", StringComparison.CurrentCultureIgnoreCase))
            {
                sPropertyValue = OSAEObjectStateManager.GetObjectStateValue(ObjectName).StateLabel;
                LastState = sPropertyValue;
                LastStateChange = OSAEObjectStateManager.GetObjectStateValue(ObjectName).LastStateChange;
                TimeSpan newSpan = (DateTime.Now - LastStateChange);
                sPropertyValue += " (" + newSpan.ToString(@"dd\ hh\:mm\:ss") + ")";
            }
            else
            {
                sPropertyValue = OSAEObjectPropertyManager.GetObjectPropertyValue(ObjectName, PropertyName).Value;
            }
            string sBackColor = screenObject.Property("Back Color").Value;
            string sForeColor = screenObject.Property("Fore Color").Value;
            string sPrefix = screenObject.Property("Prefix").Value;
            string sSuffix = screenObject.Property("Suffix").Value;
            string iFontSize = screenObject.Property("Font Size").Value;
            string sFontName = screenObject.Property("Font Name").Value;

            if (sPropertyValue != "")
            {
                if (sBackColor != "")
                {
                    try
                    {
                        BrushConverter conv = new BrushConverter();
                        SolidColorBrush brush = conv.ConvertFromString(sBackColor) as SolidColorBrush;
                        propLabel.Background = brush;
                    }
                    catch (Exception)
                    {}
                }
                if (sForeColor != "")
                {
                    try
                    {
                        BrushConverter conv = new BrushConverter();
                        SolidColorBrush brush = conv.ConvertFromString(sForeColor) as SolidColorBrush;
                        propLabel.Foreground = brush;
                    }
                    catch (Exception)
                    {}
                }
                if (iFontSize != "")
                {
                    try
                    {
                        propLabel.FontSize = Convert.ToDouble(iFontSize);
                    }
                    catch (Exception)
                    {}
                }
                if (sFontName != "")
                {
                    try
                    {
                        propLabel.FontFamily = new FontFamily(sFontName);
                    }
                    catch (Exception)
                    {}
                }
                propLabel.Content = sPrefix + sPropertyValue + sSuffix;
            }
            else
                propLabel.Content = "";
        }

        public void Update(string type)
        {

                string sPropertyValue;
                if (type == "Full")
                {
                    if (string.Equals(PropertyName, "STATE", StringComparison.CurrentCultureIgnoreCase))
                     {
                        sPropertyValue = OSAEObjectStateManager.GetObjectStateValue(ObjectName).StateLabel;
                        LastState = sPropertyValue;
                        LastStateChange = OSAEObjectStateManager.GetObjectStateValue(ObjectName).LastStateChange;
                        TimeSpan newSpan = (DateTime.Now - LastStateChange);
                        sPropertyValue += " (" + newSpan.ToString(@"dd\ hh\:mm\:ss") + ")";
                        string sPrefix = screenObject.Property("Prefix").Value;
                        string sSuffix = screenObject.Property("Suffix").Value;
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                             propLabel.Content = sPrefix + sPropertyValue + sSuffix;
                        }));
                    }
                    else
                    {
                        sPropertyValue = OSAEObjectPropertyManager.GetObjectPropertyValue(ObjectName, PropertyName).Value;
                        string sPrefix = screenObject.Property("Prefix").Value;
                        string sSuffix = screenObject.Property("Suffix").Value;
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            propLabel.Content = sPrefix + sPropertyValue + sSuffix;
                        }));
                    }
                }
                else if (type == "Refresh")
                {
                    if (string.Equals(PropertyName, "STATE", StringComparison.CurrentCultureIgnoreCase))
                    {
                        sPropertyValue = LastState;
                        TimeSpan newSpan = (DateTime.Now - LastStateChange);
                        sPropertyValue += " (" + newSpan.ToString(@"dd\ hh\:mm\:ss") + ")";
                        string sPrefix = screenObject.Property("Prefix").Value;
                        string sSuffix = screenObject.Property("Suffix").Value;
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            propLabel.Content = sPrefix + sPropertyValue + sSuffix;
                        }));
                    }
                }
           
        }
    }
}
