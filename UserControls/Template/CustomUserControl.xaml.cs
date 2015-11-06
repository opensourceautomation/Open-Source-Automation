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
        // See each section below!

        #region Logging
        // Uncomment this line to add OSAE General Logging. Not all controls need Logging.
        // private OSAE.General.OSAELog Log = new OSAE.General.OSAELog();
        #endregion

        #region Required Properties DO NOT DELETE
        // Do Not remove these:
        public OSAEObject screenObject = new OSAEObject();
        public Point Location;
        public string _controlname;
        public string CurState;
        public string CurStateLabel;
        public DateTime LastUpdated;
        public DateTime LastStateChange;
        public string objName;
        
        // Add any additional properties needed here...

        #endregion

        #region Constructor
        // Code to Initialize your custom User Control
        public CustomUserControl(OSAEObject sObj, string ControlName)
        {
            InitializeComponent();
            _controlname = ControlName;
            screenObject = sObj;
            objName = sObj.Property("Object Name").Value;
            CurState = OSAEObjectStateManager.GetObjectStateValue(objName).Value;
            LastStateChange = OSAEObjectStateManager.GetObjectStateValue(objName).LastStateChange;

            // Retreive any other information needed from the database here....


            // Execute the refreshControl function. See below!
            refreshControl();
        }
        #endregion

        #region refreshControl
        private void refreshControl()
        {
            // This code should populate or refresh the screen object with old/new information retieved
            // For Example:
            
            // if (this.CurState == "ON")
            // {
            //     this.button1.Content = objName + " OFF";
            // }
            // else
            // {
            //     this.button1.Content = objName + " ON";
            // }
        }
        #endregion

        #region Update
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
                // Code that executes when the associated object has changed
                refreshControl();
            } 
            
        }
        #endregion

        #region Unloaded
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            // Add any Cleanup code needed here
        }
        #endregion

    }
}
