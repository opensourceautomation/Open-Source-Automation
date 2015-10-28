using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OSAE;
using PluginInterface;

namespace UserControlTemplate
{
    /// <summary>
    /// Interaction logic for CustomUserControl.xaml
    /// </summary>
    public partial class CustomUserControl : UserControl
    {
        // This is where most of the work is done for creating a Custom User Control
        
        // 1st, Go to Design Mode on the CustomUserControl.xaml file to create your Control.
        // 2nd, Change this file to include code that populates, updates and runs your control.
        

        // Uncomment this line to add OSAE General Logging. Not all controls need Logging.
        // private OSAE.General.OSAELog Log = new OSAE.General.OSAELog();

        // Do Not remove these:
        public OSAEObject screenObject = new OSAEObject();
        public Point Location;
        public string _controlname;
        

        // Code to Initialize your custom User Control
        public CustomUserControl(OSAEObject sObj, string ControlName)
        {
            InitializeComponent();
            _controlname = ControlName;
            screenObject = sObj;

            //var imgsWidth = OSAEObjectPropertyManager.GetObjectPropertyValue(sObj.Property("Object Name").Value, "Width").Value;
            //var imgsHeight = OSAEObjectPropertyManager.GetObjectPropertyValue(sObj.Property("Object Name").Value, "Height").Value;

        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            // Add any Cleanup code needed here
            
        }

    }
}
