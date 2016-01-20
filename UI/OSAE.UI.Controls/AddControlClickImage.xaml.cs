
namespace OSAE.UI.Controls
{
    using System;
    using System.IO;
    using System.Data;
    using System.Windows;
    using System.Windows.Controls;
    using System.Collections.Generic;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Interaction logic for AddControlClickImage.xaml
    /// </summary>
    public partial class AddControlClickImage : UserControl
    {
        private string currentScreen;
        private string currentUser;
        OSAEImage imgNormalRaw = new OSAEImage();
        OSAEImage imgPressedRaw = new OSAEImage();
        string sOriginalName = "";
        string sWorkingName = "";
        string sMode = "";

        public AddControlClickImage(string screen, string user,string controlName = "")
        {
            InitializeComponent();
            LoadObjects();
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
                    txtControlName.Text = controlName;
                    LoadCurrentScreenObject(controlName);
                }
            }

            if (controlName == "")
            {
                //Let's create a new name
                sWorkingName = currentScreen + " - New Click Image";
                DataSet dsScreenControl = OSAESql.RunSQL("SELECT COUNT(object_name) FROM osae_v_object where object_name = '" + sWorkingName + "'");
                int iCount = 0;

                while (dsScreenControl.Tables[0].Rows[0][0].ToString() == "1")
                {
                    // We have a duplicate name, we must get a unique name
                    iCount += 1;
                    sWorkingName = currentScreen + " - New Click Image " + iCount;
                    dsScreenControl = OSAESql.RunSQL("SELECT COUNT(object_name) FROM osae_v_object where object_name = '" + sWorkingName + "'");
                 }
                sMode = "Add";
                controlName = sWorkingName;
                txtControlName.Text = controlName;
                //LoadCurrentScreenObject(controlName);
            }
            Enable_Buttons();
        }



        /// <summary>
        /// Load the objects from the DB into the combo box
        /// </summary>
        private void LoadCurrentScreenObject(string controlName)
        {
            cboPressObject.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Press Object Name").Value;
            cboPressMethod.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Press Method Name").Value;
            txtPressParam1.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Press Method Param 1").Value;
            txtPressParam2.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Press Method Param 2").Value;
            cboPressScript.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Press Script Name").Value;
            txtPressScriptParam1.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Press Script Param 1").Value;
            txtPressScriptParam2.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Press Script Param 2").Value;
            cboReleaseObject.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Release Object Name").Value;
            cboReleaseMethod.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Release Method Name").Value;
            txtReleaseParam1.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Release Method Param 1").Value;
            txtReleaseParam2.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Release Method Param 2").Value;
            cboReleaseScript.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Release Script Name").Value;
            txtReleaseScriptParam1.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Release Script Param 1").Value;
            txtReleaseScriptParam2.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Release Script Param 2").Value;
            txtNormalX.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "X").Value;
            txtNormalY.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Y").Value;
            txtZOrder.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "ZOrder").Value;
            OSAEImageManager imgMgr = new OSAEImageManager();
            try
            {
                imgNormalRaw = imgMgr.GetImage(OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Normal Image").Value);
                imgNormal.Source = LoadImage(imgNormalRaw.Data);
                Validate_Initial_Coordinates();
            }
            catch { }

            try
            {
                imgPressedRaw = imgMgr.GetImage(OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Pressed Image").Value);
                imgPressed.Source = LoadImage(imgPressedRaw.Data);
                Validate_Initial_Coordinates();
            }
            catch { }
       }


        /// <summary>
        /// Load the objects from the DB into the combo box
        /// </summary>
        private void LoadObjects()
        {
            DataSet dataSet = OSAESql.RunSQL("SELECT object_name FROM osae_v_object order by object_name");
            cboPressObject.ItemsSource = dataSet.Tables[0].DefaultView;
            cboReleaseObject.ItemsSource = dataSet.Tables[0].DefaultView;
            DataSet dataSet2 = OSAESql.RunSQL("SELECT script_name FROM osae_script order by script_name");
            cboPressScript.ItemsSource = dataSet2.Tables[0].DefaultView;
            cboReleaseScript.ItemsSource = dataSet2.Tables[0].DefaultView;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (validateForm("Add"))
            {
                sWorkingName = txtControlName.Text;
                OSAEObjectManager.ObjectAdd(sWorkingName, sWorkingName, sWorkingName, "CONTROL CLICK IMAGE", "", currentScreen,50, true);
                if (imgNormalRaw != null) OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Normal Image", imgNormalRaw.Name, currentUser);
                else OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Normal Image", "", currentUser);
                if (imgPressedRaw != null) OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Pressed Image", imgPressedRaw.Name, currentUser);
                else OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Pressed Image", "", currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Press Object Name", cboPressObject.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Press Method Name", cboPressMethod.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Press Param 1", txtPressParam1.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Press Param 2", txtPressParam2.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Press Script Name", cboPressScript.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Press Script Param 1", txtPressScriptParam1.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Press Script Param 2", txtPressScriptParam2.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Release Object Name", cboReleaseObject.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Release Method Name", cboReleaseMethod.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Release Param 1", txtReleaseParam1.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Release Param 2", txtReleaseParam2.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Release Script Name", cboReleaseScript.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Release Script Param 1", txtReleaseScriptParam1.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Release Script Param 2", txtReleaseScriptParam2.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "X", "100", currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Y", "100", currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Zorder", txtZOrder.Text, currentUser);
                OSAEScreenControlManager.ScreenObjectAdd(currentScreen, "", sWorkingName);
                NotifyParentFinished();
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (validateForm("Update"))
            {
                sWorkingName = txtControlName.Text;
                OSAE.OSAEObject obj = OSAEObjectManager.GetObjectByName(sOriginalName);
                //We call an object update here in case the Name was changed, then perform the updates against the New name
                OSAEObjectManager.ObjectUpdate(sOriginalName, sWorkingName, obj.Alias, obj.Description, obj.Type, obj.Address, obj.Container, obj.MinTrustLevel, obj.Enabled);

                if (imgNormalRaw != null) OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Normal Image", imgNormalRaw.Name, currentUser);
                else OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Normal Image", "", currentUser);
                if (imgPressedRaw != null) OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Pressed Image", imgPressedRaw.Name, currentUser);
                else OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Pressed Image", "", currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Press Object Name", cboPressObject.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Press Method Name", cboPressMethod.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Press Param 1", txtPressParam1.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Press Param 2", txtPressParam2.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Press Script Name", cboPressScript.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Press Script Param 1", txtPressScriptParam1.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Press Script Param 2", txtPressScriptParam2.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Release Object Name", cboReleaseObject.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Release Method Name", cboReleaseMethod.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Release Param 1", txtReleaseParam1.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Release Param 2", txtReleaseParam2.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Release Script Name", cboReleaseScript.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Release Script Param 1", txtReleaseScriptParam1.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Release Script Param 2", txtReleaseScriptParam2.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "X", txtNormalX.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Y", txtNormalY.Text, currentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Zorder", txtZOrder.Text, currentUser);
                OSAEScreenControlManager.ScreenObjectUpdate(currentScreen, "", sWorkingName);
                NotifyParentFinished();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            OSAEObjectManager.ObjectDelete(sOriginalName);
            NotifyParentFinished();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            NotifyParentFinished();
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

        private void txtControlName_TextChanged(object sender, TextChangedEventArgs e)
        {
            Enable_Buttons();
        }

        private void cboPressObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataSet dataSet = OSAESql.RunSQL("SELECT method_name FROM osae_v_object_method where object_name = '" + cboPressObject.SelectedValue + "' order by method_name");
            cboPressMethod.IsEnabled = true;
            cboPressMethod.ItemsSource = dataSet.Tables[0].DefaultView;
            txtControlName.Text = currentScreen + " - " + cboPressObject.SelectedValue + " - Click Image";
        }

        private void cboReleaseObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataSet dataSet = OSAESql.RunSQL("SELECT method_name FROM osae_v_object_method where object_name = '" + cboReleaseObject.SelectedValue + "' order by method_name");
            cboReleaseMethod.IsEnabled = true;
            cboReleaseMethod.ItemsSource = dataSet.Tables[0].DefaultView;
            if(cboPressObject.Text == "")
            {
                txtControlName.Text = currentScreen + " - " + cboReleaseObject.SelectedValue + " - Click Image";
            }
        }

        private void SelectNormalImage_Click(object sender, RoutedEventArgs e)
        {
            selectImageWindow dlgSelectImage = new selectImageWindow();
            ctrlSelectImage siNormal = new ctrlSelectImage();
            siNormal.ImagePicked += siNormal_ImagePicked;
            dlgSelectImage.Width = siNormal.Width + 80;
            dlgSelectImage.Height = siNormal.Height + 80;
            dlgSelectImage.Content = siNormal;
            dlgSelectImage.Show();
        }

        private void SelectPressedImage_Click(object sender, RoutedEventArgs e)
        {
            selectImageWindow dlgSelectImage = new selectImageWindow();
            ctrlSelectImage siPressed = new ctrlSelectImage();
            siPressed.ImagePicked += siPressed_ImagePicked;
            dlgSelectImage.Width = siPressed.Width + 80;
            dlgSelectImage.Height = siPressed.Height + 80;
            dlgSelectImage.Content = siPressed;
            dlgSelectImage.Show();
        }

        private void Validate_Initial_Coordinates()
        {
            //If there is an image, make sure X & Y are not blank
            if (imgNormalRaw != null)
            {
                if (txtNormalX.Text == "")
                    txtNormalX.Text = "100";
                if (txtNormalY.Text == "")
                    txtNormalY.Text = "100";
            }
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
            parentWindow.DialogResult = true;
            parentWindow.Close();
        }

        protected void siNormal_ImagePicked(object sender, EventArgs e)
        {
            OSAEImageManager imgMgr = new OSAEImageManager();

            imgNormalRaw = imgMgr.GetImage((int)sender);
            imgNormal.Source = LoadImage(imgNormalRaw.Data);
            Validate_Initial_Coordinates();
        }

        protected void siPressed_ImagePicked(object sender, EventArgs e)
        {
            OSAEImageManager imgMgr = new OSAEImageManager();

            imgPressedRaw = imgMgr.GetImage((int)sender);
            imgPressed.Source = LoadImage(imgPressedRaw.Data);
            Validate_Initial_Coordinates();
        }


        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
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
                MessageBox.Show("You must enter a Control Name!");
                validate = false;
            }
            if (string.IsNullOrEmpty(txtNormalX.Text))
            {
                MessageBox.Show("X Can not be empty");
                validate = false;
            }
            if (string.IsNullOrEmpty(txtNormalY.Text))
            {
                MessageBox.Show("Y Can not be empty");
                validate = false;
            }
            if (string.IsNullOrEmpty(txtZOrder.Text))
            {
                MessageBox.Show("ZOrder can not be empty");
                validate = false;
            }
            if (string.IsNullOrEmpty(cboPressObject.Text) && string.IsNullOrEmpty(cboReleaseObject.Text) && string.IsNullOrEmpty(cboPressScript.Text) && string.IsNullOrEmpty(cboReleaseScript.Text))
            {
                MessageBox.Show("You must set an Object or Script");
                validate = false;
            }
            if(!string.IsNullOrEmpty(cboPressObject.Text) && string.IsNullOrEmpty(cboPressMethod.Text))
            {
                MessageBox.Show("You must set the Mouse Down Method!");
                validate = false;
            }
            if (!string.IsNullOrEmpty(cboReleaseObject.Text) && string.IsNullOrEmpty(cboReleaseMethod.Text))
            {
                MessageBox.Show("You must set the Mouse Release Method!");
                validate = false;
            }
            return validate;
        }

        private void cboPressMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lblAsk1.IsEnabled = true;
            lblDMP1.IsEnabled = true;
            txtPressParam1.IsEnabled = true;
            lblDMP2.IsEnabled = true;
            txtPressParam2.IsEnabled = true;
        }

        private void cboReleaseMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lblAsk2.IsEnabled = true;
            lblUMP1.IsEnabled = true;
            txtReleaseParam1.IsEnabled = true;
            lblUMP2.IsEnabled = true;
            txtReleaseParam2.IsEnabled = true;
        }

        private void cboPressScript_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lblAsk3.IsEnabled = true;
            lblDSP1.IsEnabled = true;
            txtPressScriptParam1.IsEnabled = true;
            lblDSP2.IsEnabled = true;
            txtPressScriptParam2.IsEnabled = true;
        }

        private void cboReleaseScript_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lblAsk4.IsEnabled = true;
            lblUSP1.IsEnabled = true;
            txtReleaseScriptParam1.IsEnabled = true;
            lblUSP2.IsEnabled = true;
            txtReleaseScriptParam2.IsEnabled = true;
        }
    }
}
