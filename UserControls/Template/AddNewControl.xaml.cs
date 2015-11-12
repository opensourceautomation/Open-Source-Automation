using System;
using System.Data;
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
using OSAE;

namespace UserControlTemplate
{
    /// <summary>
    /// Interaction logic for AddNewControl.xaml
    /// </summary>
    public partial class AddNewControl : UserControl
    {
        // Set the Default name to use when a user adds this to a screen
        string defaultNewName = "New YourControlName";

        // Set the Default Object Type to Associate with this Custom UserControl
        // Example, X-10 RELAY, IP CAMERA, DIMMER, THERMOSTAT
        // Must be all UPPERCASE!
        string defaultAssocObject = "OSAE OBJECT TYPE";

        // Set the Default X Coordinate
        string defaultX = "100";

        // Set the Default Y Coordinate
        string defaultY = "100";

        // Set the Default ZOrder
        string defaultZ = "1";

        // Set the Default Dropdown Caption
        string defaultCaption = "Choose an IP Camera Object below";

        #region DO NOT CHANGE THIS CODE UNLESS YOU REVISED THE CONTROL
        Boolean hasParams = false;
        List<objParams> oParams = new List<objParams>();
        string osaeControlType = "USER CONTROL";
        public string currentScreen;
        private Window parentWindow;
        private objParams selectedParam;
        public string _controlName;
        string sOriginalName = "";
        string sWorkingName = "";
        string sMode = "";
        string _pluginName;

        public AddNewControl(string screen, string pluginName, string controlName="")
        {
            InitializeComponent();
            AssocObj.Content = defaultCaption;
            currentScreen = screen;
            LoadObjects();
            _pluginName = pluginName;

            if (controlName != "") 
            {
                //Let's validate the controlName and then call a Pre-Load of its properties
                DataSet dsScreenControl = OSAESql.RunSQL("SELECT COUNT(object_name) FROM osae_v_object where object_name = '" + controlName + "'");
                if (dsScreenControl.Tables[0].Rows[0][0].ToString() == "1")
                {
                    // We have a hit, this is an Update call, se call the preload
                    sMode = "Update";
                    sOriginalName = controlName;
                    cntrlName.Text = controlName;
                    LoadCurrentScreenObject(controlName);
                }
            }
            
            if (controlName == "")
            {
                // This is a New Custom UserControl.
                sWorkingName = currentScreen + " - " + defaultNewName;
                DataSet dsScreenControl = OSAESql.RunSQL("SELECT COUNT(object_name) FROM osae_v_object where object_name = '" + sWorkingName + "'");
                int iCount = 0;

                while (dsScreenControl.Tables[0].Rows[0][0].ToString() == "1")
                {
                    // We have a duplicate name, we must get a unique name
                    iCount += 1;
                    // Edit the Text in the next line with a Default Name
                    sWorkingName = currentScreen + " - " + defaultNewName + " " + iCount;
                    dsScreenControl = OSAESql.RunSQL("SELECT COUNT(object_name) FROM osae_v_object where object_name = '" + sWorkingName + "'");
                }
                sMode = "Add";
                controlName = sWorkingName;
                cntrlName.Text = controlName;
                cntrlX.Text = defaultX;
                cntrlY.Text = defaultY;
                cntrlZOrder.Text = defaultZ;
                getExtraParams(controlName);
            }
            Enable_Buttons();
        }

        private void LoadCurrentScreenObject(string controlName)
        {
            objectsComboBox.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Object Name").Value;
            cntrlX.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "X").Value;
            cntrlY.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Y").Value;
            cntrlZOrder.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "ZOrder").Value;
            getExtraParams(controlName);
        }

        /// <summary>
        /// Loads extra Parmeters in to edit screen
        /// </summary>
        private void getExtraParams(string controlName)
        {
            OSAEObjectPropertyCollection uc = OSAEObjectPropertyManager.GetObjectProperties(controlName);
            foreach (OSAEObjectProperty p in uc)
            {
                if (p.Name != "Control Type" & p.Name != "Object Name" & p.Name != "X" & p.Name != "Y" & p.Name != "ZOrder")
                {
                    objParams oP = new objParams();
                    oP.Name = p.Name;
                    oP.Value = p.Value;
                    oParams.Add(oP);
                    hasParams = true;
                    paramList.Items.Add(oP.Name);
                }
            }
        }

        /// <summary>
        /// Load the object types from the DB into the combo box
        /// </summary>
        private void LoadObjects()
        {
            DataSet dataSet = OSAESql.RunSQL("SELECT object_name FROM osae_v_object WHERE object_Type = '"+ defaultAssocObject + "' ORDER BY object_name");
            objectsComboBox.ItemsSource = dataSet.Tables[0].DefaultView;
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateForm("Add"))
            {
                string sName = cntrlName.Text;
                OSAEObjectManager.ObjectAdd(sName, sName, sName, osaeControlType + " " + _pluginName, "", currentScreen, true);
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Control Type", osaeControlType +" " + _pluginName , "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Object Name", objectsComboBox.Text, "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "X", "100", "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Y", "100", "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "ZOrder", "1", "GUI");
                if (hasParams == true)
                {
                    foreach (objParams op in oParams)
                    {
                        OSAEObjectPropertyManager.ObjectPropertySet(sName, op.Name, op.Value, "GUI");
                    }
                }
                OSAEScreenControlManager.ScreenObjectAdd(currentScreen, objectsComboBox.Text, sName);
                NotifyParentFinished();
            }
            else
            {
                // Do not save until feilds are correct
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateForm("Update"))
            {
                sWorkingName = cntrlName.Text;
                OSAE.OSAEObject obj = OSAEObjectManager.GetObjectByName(sOriginalName);
                //We call an object update here in case the Name was changed, then perform the updates against the New name
                OSAEObjectManager.ObjectUpdate(sOriginalName, sWorkingName, obj.Alias, obj.Description, obj.Type, obj.Address, obj.Container, obj.Enabled);
                string sName = cntrlName.Text;
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Control Type", osaeControlType + " " + _pluginName, "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Object Name", objectsComboBox.Text, "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "X", cntrlX.Text, "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Y", cntrlY.Text, "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "ZOrder", cntrlZOrder.Text, "GUI");
                if (hasParams == true)
                {
                    foreach (objParams op in oParams)
                    {
                        OSAEObjectPropertyManager.ObjectPropertySet(sName, op.Name, op.Value, "GUI");
                    }
                }
                OSAEScreenControlManager.ScreenObjectUpdate(currentScreen, objectsComboBox.Text, sName);
                NotifyParentFinished();
            }
            else
            {
                // Do not save until feilds are correct
            }
        }

        private bool ValidateForm(string mthd)
        {
            bool validate = true;
            // Does this object already exist
            if (mthd == "Add" || sOriginalName != cntrlName.Text)
            {
                try
                {
                    OSAEObject oExist = OSAEObjectManager.GetObjectByName(cntrlName.Text);
                    if (oExist != null)
                    {
                        MessageBox.Show("Control name already exist. Please Change!");
                        validate = false;
                    }
                }
                catch { }
            }
            if (string.IsNullOrEmpty(cntrlName.Text))
            {
                MessageBox.Show("Please specify a name for the control");
                validate = false;
            }
            if (objectsComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Please Choose a Object to Associate");
                validate = false;
            }
            if (string.IsNullOrEmpty(cntrlX.Text))
            {
                MessageBox.Show("X Can not be empty");
                validate = false;
            }
            if (string.IsNullOrEmpty(cntrlY.Text))
            {
                MessageBox.Show("Y Can not be empty");
                validate = false;
            }
            if (string.IsNullOrEmpty(cntrlZOrder.Text))
            {
                MessageBox.Show("ZOrder can not be empty");
                validate = false;
            }
            if (hasParams == true)
            {
                foreach (objParams op in oParams)
                {
                    if (string.IsNullOrEmpty(op.Value))
                    {
                        MessageBox.Show("An additioanl Parameter is empty");
                        validate = false;
                    }
                }
            }

            return validate;
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

        private void Enable_Buttons()
        {
            //First Senerio is a New Control, not a rename or update.
            if (sMode == "Add")
            {
                addButton.IsEnabled = true;
                btnUpdate.IsEnabled = false;
                btnDelete.IsEnabled = false;
            }
            //Now we handle Updates with no name changes
            if (sMode == "Update" && sOriginalName == cntrlName.Text)
            {
                addButton.IsEnabled = false;
                btnUpdate.IsEnabled = true;
                btnDelete.IsEnabled = true;
            }
            //Now we handle Updates WITH name changes
            if (sMode == "Update" && sOriginalName != cntrlName.Text)
            {
                addButton.IsEnabled = true;
                btnUpdate.IsEnabled = true;
                btnDelete.IsEnabled = false;
            }
            if (hasParams == true)
            {
                paramList.IsEnabled = true;
                paramValue.IsEnabled = true;
                addParam.IsEnabled = true;
            }
            else
            {
                paramList.IsEnabled = false;
                paramValue.IsEnabled = false;
                addParam.IsEnabled = false;
            }
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

        private void paramList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int pInd = paramList.SelectedIndex;
            selectedParam = oParams[pInd];
            paramValue.Text = selectedParam.Value;
        }

        private void saveParmValue_Click(object sender, RoutedEventArgs e)
        {
            selectedParam.Value = paramValue.Text;
        }
        
    }

    public class objParams
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    #endregion
}
