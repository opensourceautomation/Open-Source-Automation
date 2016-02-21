using System;
using System.Windows;
using System.Windows.Controls;

namespace OSAE.UI.Controls
{
    /// <summary>
    /// Interaction logic for BrowserFrame.xaml
    /// </summary>
    public partial class BrowserFrame : UserControl
    {
        public OSAEObject screenObject { get; set; }
        public Point Location;
        public double ControlWidth;
        public double ControlHeight;
        public string ObjectName;

        public BrowserFrame(OSAEObject sObject)
        {
            InitializeComponent();
            screenObject = sObject;
            try
            {
                Uri uriproperty = new Uri(screenObject.Property("URI").Value);
                wbBrowser.Source = uriproperty;
            }
            catch { }
        }
    }
}
