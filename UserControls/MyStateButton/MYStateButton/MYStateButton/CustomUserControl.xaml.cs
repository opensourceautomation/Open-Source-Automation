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

namespace MYStateButton
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
        public OSAEObject screenObject { get; set; }
        public Point Location;
        public string _controlname;
        public string CurState;
        public string CurStateLabel;
        public DateTime LastUpdated;
        public DateTime LastStateChange;
        public string objName;
        private string gAppName = "";
        private string currentUser;

        // Code to Initialize your custom User Control
        public CustomUserControl(OSAEObject sObj, string ControlName, string appName, string user)
        {
            InitializeComponent();
            gAppName = appName;
            currentUser = user;
            _controlname = ControlName;
            screenObject = sObj;
            objName = sObj.Property("Object Name").Value;
            CurState = OSAEObjectStateManager.GetObjectStateValue(objName).Value;
            LastStateChange = OSAEObjectStateManager.GetObjectStateValue(objName).LastStateChange;
            this.updateBut();
        }

        public void setLocation(double X, double Y)
        {
            Location.X = X;
            Location.Y = Y;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {

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
                this.updateBut();
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            string currentUser = OSAE.OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Current User").Value;
            if (currentUser == "") return;

                this.CurState = OSAEObjectStateManager.GetObjectStateValue(objName).ToString();
            if (this.CurState == "ON")
            {
                OSAEMethodManager.MethodQueueAdd(objName, "OFF", "0", "", currentUser);
                OSAEObjectStateManager.ObjectStateSet(objName, "OFF", currentUser);
                this.CurState = "OFF";
            }
            else
            {
                OSAEMethodManager.MethodQueueAdd(objName, "ON", "0", "", currentUser);
                OSAEObjectStateManager.ObjectStateSet(objName, "ON", currentUser);
                this.CurState = "ON";
            }
            this.updateBut();
        }

        private void updateBut()
        {
            if (this.CurState == "ON")
            {
                this.button1.Content = objName + " OFF";
            }
            else
            {
                this.button1.Content = objName + " ON";
            }
        }
    }
}
