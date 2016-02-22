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
        private string currentUser;
        string sOriginalName = "";
        string sWorkingName = "";
        string sMode = "";

        public AddControlBrowser(string screen, string user, string controlName = "")
        {
            InitializeComponent();
            currentScreen = screen;
            currentUser = user;
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
                sWorkingName = currentScreen + " - Browser";
                DataSet dsScreenControl = OSAESql.RunSQL("SELECT COUNT(object_name) FROM osae_v_object where object_name = '" + sWorkingName + "'");
                int iCount = 0;

                while (dsScreenControl.Tables[0].Rows[0][0].ToString() == "1")
                {
                    // We have a duplicate name, we must get a unique name
                    iCount += 1;
                    sWorkingName = currentScreen + " - Browser " + iCount;
                    dsScreenControl = OSAESql.RunSQL("SELECT COUNT(object_name) FROM osae_v_object where object_name = '" + sWorkingName + "'");
                }
                sMode = "Add";
                controlName = sWorkingName;
                txtName.Text = controlName;
                //LoadCurrentScreenObject(controlName);
            }
            Enable_Buttons();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (validateForm("Add"))
            {
                string sName = txtName.Text;
                OSAEObjectManager.ObjectAdd(sName, "", sName, "CONTROL BROWSER", "", currentScreen, 30, true);
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "URI", txtURI.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Width", txtWidth.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Height", txtHeight.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "X", txtX.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Y", txtY.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "ZOrder", txtZOrder.Text, currentUser);
                OSAEScreenControlManager.ScreenObjectAdd(currentScreen, "", txtName.Text);
                NotifyParentFinished();
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (validateForm("Update"))
            {
                string sName = txtName.Text;
                OSAEObjectManager.ObjectUpdate(sOriginalName, sName, sName, sName, "CONTROL BROWSER", "", currentScreen, 30, true);
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "URI", txtURI.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Width", txtWidth.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Height", txtHeight.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "X", txtX.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Y", txtY.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "ZOrder", txtZOrder.Text, currentUser);
                OSAEScreenControlManager.ScreenObjectAdd(currentScreen, "", txtName.Text);
                NotifyParentFinished();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            NotifyParentFinished();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            OSAEObjectManager.ObjectDelete(sOriginalName);
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
            txtZOrder.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "ZOrder").Value;
            txtHeight.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Height").Value;
            txtWidth.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Width").Value;
        }

        private void Enable_Buttons()
        {
            //First Senerio is a New Control, not a rename or update.
            if (sMode == "Add")
            {
                btnAdd.IsEnabled = true;
                btnUpdate.IsEnabled = false;
                btnDelete.IsEnabled = false;
            }
            //Now we handle Updates with no name changes
            if (sMode == "Update" && sOriginalName == txtName.Text)
            {
                btnAdd.IsEnabled = false;
                btnUpdate.IsEnabled = true;
                btnDelete.IsEnabled = true;
            }
            //Now we handle Updates WITH name changes
            if (sMode == "Update" && sOriginalName != txtName.Text)
            {
                btnAdd.IsEnabled = true;
                btnUpdate.IsEnabled = true;
                btnDelete.IsEnabled = false;
            }
        }

        private bool validateForm(string mthd)
        {
            bool validate = true;
            // Does this object already exist
            if (mthd == "Add" || sOriginalName != txtName.Text)
            {
                try
                {
                    OSAEObject oExist = OSAEObjectManager.GetObjectByName(txtName.Text);
                    if (oExist != null)
                    {
                        MessageBox.Show("Control name already exist. Please Change!");
                        validate = false;
                    }
                }
                catch { }
            }
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show("You must enter a Name for this control!");
                validate = false;
            }
            if (string.IsNullOrEmpty(txtURI.Text))
            {
                MessageBox.Show("You must enter a URI for this Brower!");
                validate = false;
            }
            if (string.IsNullOrEmpty(txtX.Text))
            {
                MessageBox.Show("X Can not be empty");
                validate = false;
            }
            if (string.IsNullOrEmpty(txtY.Text))
            {
                MessageBox.Show("Y Can not be empty");
                validate = false;
            }
            if (string.IsNullOrEmpty(txtZOrder.Text))
            {
                MessageBox.Show("ZOrder Can not be empty");
                validate = false;
            }
            return validate;
        }

        private void txtURI_TextChanged(object sender, TextChangedEventArgs e)
        {
            string cURL = txtURI.Text.Replace("http://", ""); 
            cURL = cURL.Replace("www.", "");
            cURL = cURL.Replace(".com", "");
            txtName.Text = currentScreen + " - " + cURL;
        }
    }
}
