namespace OSAE.UI.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Data;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Navigation;
    using System.Windows.Shapes;

    /// <summary>
    /// Interaction logic for AddControlBrowser.xaml
    /// </summary>
    public partial class AddControlBrowser : UserControl
    {
        private string currentScreen;
        string sOriginalName = "";
        string sWorkingName = "";
        string sMode = "";

        public AddControlBrowser(string screen, string controlName = "")
        {
            InitializeComponent();
            currentScreen = screen;

            //Check if controlName was passed in, if so, goto edit mode
            if (controlName != "")
            {
                //Let's validate the controlName and then call a Pre-Load of its properties
                DataSet dsScreenControl = OSAESql.RunSQL("SELECT COUNT(object_name) FROM osae_v_object where object_name = '" + controlName + "'");
                if (dsScreenControl.Tables[0].Rows[0][0].ToString() == "1")
                {
                    // We have a hit, this is an Update call, se call the preload
                    sMode = "Update";
                    sOriginalName = controlName;
                    txtName.Text = controlName;
                    LoadCurrentScreenObject(controlName);
                }
            }

            if (controlName == "")
            {
                //Let's create a new name
                sWorkingName = currentScreen + " - New Browser";
                DataSet dsScreenControl = OSAESql.RunSQL("SELECT COUNT(object_name) FROM osae_v_object where object_name = '" + sWorkingName + "'");
                int iCount = 0;

                while (dsScreenControl.Tables[0].Rows[0][0].ToString() == "1")
                {
                    // We have a duplicate name, we must get a unique name
                    iCount += 1;
                    sWorkingName = currentScreen + " - New Object " + iCount;
                    dsScreenControl = OSAESql.RunSQL("SELECT COUNT(object_name) FROM osae_v_object where object_name = '" + sWorkingName + "'");
                }
                sMode = "Add";
                controlName = sWorkingName;
                txtName.Text = controlName;
                LoadCurrentScreenObject(controlName);
            }

            txtName.Text = currentScreen + " - Browser";
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {


            OSAEObjectManager.ObjectAdd(txtName.Text, txtName.Text, txtName.Text, "CONTROL BROWSER", "", currentScreen, true);

            OSAEObjectPropertyManager.ObjectPropertySet(txtName.Text, "URI", txtURI.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(txtName.Text, "Width", txtWidth.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(txtName.Text, "Height", txtHeight.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(txtName.Text, "ZOrder", "1", "GUI");

            OSAEScreenControlManager.ScreenObjectAdd(currentScreen, "", txtName.Text);

            NotifyParentFinished();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            NotifyParentFinished();
        }

        /// <summary>
        /// Let the hosting contol know that we are done
        /// </summary>
        /// <remarks>At present it tells the parent to close, this could later be altered to have a event that fires to
        /// the parent allowing them to decide what to do when the control is finished. If the control is being hosted in
        /// an element host this will have no affect as the parent is the element host and not the form.</remarks>
        private void NotifyParentFinished()
        {
            // Get the window hosting us so we can ask it to close
            Window parentWindow = Window.GetWindow(this);
            parentWindow.Close();
        }

        private void LoadCurrentScreenObject(string controlName)
        {
            txtURI.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "URI").Value;
            txtX.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "X").Value;
            txtY.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Y").Value;
            txtHeight.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Height").Value;
            txtWidth.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Width").Value;
        }




    }
}
