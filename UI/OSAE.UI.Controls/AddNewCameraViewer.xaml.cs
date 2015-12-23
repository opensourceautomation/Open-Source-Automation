using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Collections.Generic; 

namespace OSAE.UI.Controls
{
    /// <summary>
    /// Interaction logic for AddNewCameraViewer.xaml
    /// </summary>
    public partial class AddNewCameraViewer : UserControl
    {
        private string currentScreen;
        private string currentUser;
        string sOriginalName = "";
        string sWorkingName = "";
        string sMode = "";

        public AddNewCameraViewer(string screen, string user, string controlName = "")
        {
            InitializeComponent();
            currentScreen = screen;
            currentUser = user;
            LoadObjects();

            if (controlName != "")
            {
                //Let's validate the controlName and then call a Pre-Load of its properties
                DataSet dsScreenControl = OSAESql.RunSQL("SELECT COUNT(object_name) FROM osae_v_object where object_name = '" + controlName + "'");
                if (dsScreenControl.Tables[0].Rows[0][0].ToString() == "1")
                {
                    // We have a hit, this is an Update call, se call the preload
                    sMode = "Update";
                    sOriginalName = controlName;
                    txtControlName.Text = controlName;
                    LoadCurrentScreenObject(controlName);
                }
            }

            if (controlName == "")
            {
                //Let's create a new name
                sWorkingName = currentScreen + " - New Camera Viewer";
                DataSet dsScreenControl = OSAESql.RunSQL("SELECT COUNT(object_name) FROM osae_v_object where object_name = '" + sWorkingName + "'");
                int iCount = 0;

                while (dsScreenControl.Tables[0].Rows[0][0].ToString() == "1")
                {
                    // We have a duplicate name, we must get a unique name
                    iCount += 1;
                    sWorkingName = currentScreen + " - New Camera Viewer" + iCount;
                    dsScreenControl = OSAESql.RunSQL("SELECT COUNT(object_name) FROM osae_v_object where object_name = '" + sWorkingName + "'");
                }
                sMode = "Add";
                controlName = sWorkingName;
                txtControlName.Text = controlName;

                // LoadCurrentScreenObject(controlName);
            }
            Enable_Buttons();
        }

        /// <summary>
        /// Load the object types from the DB into the combo box
        /// </summary>
        private void LoadObjects()
        {
            DataSet dataSet = OSAESql.RunSQL("SELECT object_name FROM osae_v_object WHERE object_Type = 'IP CAMERA' ORDER BY object_name");
            objectsComboBox.ItemsSource = dataSet.Tables[0].DefaultView;
        }

        private void LoadCurrentScreenObject(string controlName)
        {
            objectsComboBox.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Object Name").Value;
            txtX.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "X").Value;
            txtY.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Y").Value;
            txtZOrder.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "ZOrder").Value;
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (validateForm("Add"))
            {
                string sName = txtControlName.Text;
                OSAEObjectManager.ObjectAdd(sName, "", sName, "CONTROL CAMERA VIEWER", "", currentScreen, 50, true);
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Object Name", objectsComboBox.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "X", txtX.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Y", txtY.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "ZOrder", txtZOrder.Text, currentUser);
                OSAEScreenControlManager.ScreenObjectAdd(currentScreen, objectsComboBox.Text, sName);
                NotifyParentFinished();
            }
            else
            {
                MessageBox.Show("Not all the mandatory fields have been completed", "Fields Incomplete", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (validateForm("Update"))
            {
                string sName = txtControlName.Text;
                OSAEObjectManager.ObjectUpdate(sOriginalName, sName, "", sName, "CONTROL CAMERA VIEWER", "", currentScreen, 50, 1);
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Object Name", objectsComboBox.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "X", txtX.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Y", txtY.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "ZOrder", txtZOrder.Text, currentUser);
                OSAEScreenControlManager.ScreenObjectAdd(currentScreen, objectsComboBox.Text, sName);
                NotifyParentFinished();
            }
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
            if (sMode == "Update" && sOriginalName == txtControlName.Text)
            {
                btnAdd.IsEnabled = false;
                btnUpdate.IsEnabled = true;
                btnDelete.IsEnabled = true;
            }
            //Now we handle Updates WITH name changes
            if (sMode == "Update" && sOriginalName != txtControlName.Text)
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
            if (mthd == "Add" || sOriginalName != txtControlName.Text)
            {
                try
                {
                    OSAEObject oExist = OSAEObjectManager.GetObjectByName(txtControlName.Text);
                    if (oExist != null)
                    {
                        MessageBox.Show("Control name already exist. Please Change!");
                        validate = false;
                    }
                }
                catch { }
            }
            if (string.IsNullOrEmpty(txtControlName.Text))
            {
                MessageBox.Show("You must enter a name for this control!");
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
                MessageBox.Show("ZOrder can not be empty");
                validate = false;
            }

            return validate;
        }

      

        private void cancelbutton_Click(object sender, RoutedEventArgs e)
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

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            OSAEObjectManager.ObjectDelete(sOriginalName);
            NotifyParentFinished();
        }

    }
}
