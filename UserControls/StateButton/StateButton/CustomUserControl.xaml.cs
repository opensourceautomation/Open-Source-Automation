using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Data;
using OSAE;

namespace StateButton
{
    /// <summary>
    /// Interaction logic for CustomUserControl.xaml
    /// </summary>
    public partial class CustomUserControl : UserControl
    {
        public OSAEObject screenObject { get; set; }
        public Point Location;
        public string Author { get { return ""; } } // replaace from control description file
        public string Version { get { return ""; } }
        public string _controlname;
        public int ControlWidth;
        public int ControlHeight;
        public string objName;
        public string CurState;
        public string State1Name, State1Label, State2Name, State2Label;
        public DateTime LastUpdated;
        public DateTime LastStateChange;
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
            DataSet ds = OSAEObjectStateManager.ObjectStateListGet(objName);
            if (ds.Tables[0].Rows.Count > 0)
            {
                State1Name = ds.Tables[0].Rows[0]["state_name"].ToString();
                State1Label = ds.Tables[0].Rows[0]["state_label"].ToString();
            }
            if (ds.Tables[0].Rows.Count > 1)
            {
                State2Name = ds.Tables[0].Rows[1]["state_name"].ToString();
                State2Label = ds.Tables[0].Rows[1]["state_label"].ToString();
            }
            CurState = OSAEObjectStateManager.GetObjectStateValue(objName).Value;
            LastStateChange = OSAEObjectStateManager.GetObjectStateValue(objName).LastStateChange;
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
                    btnState.Background = brush;
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
                    btnState.Foreground = brush;
                }
                catch (Exception)
                { }
            }
            if (iFontSize != "")
            {
                try
                { btnState.FontSize = Convert.ToDouble(iFontSize); }
                catch (Exception)
                { }
            }
            if (sFontName != "")
            {
                try
                { btnState.FontFamily = new FontFamily(sFontName); }
                catch (Exception)
                { }
            }
            //propLabel.Content = sPrefix + sPropertyValue + sSuffix;


            Update_Button();
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
            OSAEObjectState stateCurrent = OSAEObjectStateManager.GetObjectStateValue(objName);
            LastStateChange = stateCurrent.LastStateChange;
            if (CurState != stateCurrent.Value) 
            {
                CurState = stateCurrent.Value;
                Update_Button();
            }
        }

        private void btnState_Click(object sender, RoutedEventArgs e)
        {
            if (currentUser == "") return;

            if (CurState == State1Name)
            {
                OSAEMethodManager.MethodQueueAdd(objName, State2Name, "0", "", currentUser);
                OSAEObjectStateManager.ObjectStateSet(objName, State2Name, currentUser);
            }
            else
            {
                OSAEMethodManager.MethodQueueAdd(objName, State1Name, "0", "", currentUser);
                OSAEObjectStateManager.ObjectStateSet(objName, State1Name, currentUser);
            }
        }

        private void Update_Button()
        {
            if (CurState == State1Name)
            {
                btnState.Content = objName + " " + State1Label;
                ToolTip = "Click to Set " + objName + " to " + State2Label;
            }
            else
            {
                btnState.Content = objName + " " + State2Label;
                ToolTip = "Click to Set " + objName + " to " + State1Label;
            }
        }
    }
}
