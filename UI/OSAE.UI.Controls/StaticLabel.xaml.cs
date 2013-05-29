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

namespace OSAE.UI.Controls
{
    /// <summary>
    /// Interaction logic for StaticLabel.xaml
    /// </summary>
    public partial class StaticLabel : UserControl
    {
        public OSAEObject screenObject { get; set; }
        public Point Location;
        public DateTime LastUpdated;
        

        public StaticLabel(OSAEObject sObj)
        {
            InitializeComponent();
            screenObject = sObj;

            string sValue = screenObject.Property("Value").Value; ;
            string sBackColor = screenObject.Property("Background Color").Value;
            string sForeColor = screenObject.Property("Fore Color").Value;
            string iFontSize = screenObject.Property("Font Size").Value;
            string sFontName = screenObject.Property("Font Name").Value;

            if (sValue != "")
            {
                if (sBackColor != "")
                {
                    try
                    {
                        BrushConverter conv = new BrushConverter();
                        SolidColorBrush brush = conv.ConvertFromString(sBackColor) as SolidColorBrush;
                        staticLabel.Background = brush;
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
                        staticLabel.Foreground = brush;
                    }
                    catch (Exception myerror)
                    {
                    }
                }
                if (iFontSize != "")
                {
                    try
                    {
                        staticLabel.FontSize = Convert.ToDouble(iFontSize);
                    }
                    catch (Exception myerror)
                    {
                    }
                }
                staticLabel.Content = sValue;
            }
            else
            {
                staticLabel.Content = "";
            }

        }
    }
}
