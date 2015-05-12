
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
        OSAEImage imgNormalRaw = new OSAEImage();
        OSAEImage imgPressedRaw = new OSAEImage();
        string sOriginalName = "";
        string sWorkingName = "";
        string sMode = "";

        public AddControlClickImage(string screen,string controlName = "")
        {
            InitializeComponent();
            LoadObjects();
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
                    txtControlName.Text = controlName;
                    LoadCurrentScreenObject(controlName);
                }
            }

            if (controlName == "")
            {
                //Let's create a new name
                sWorkingName = currentScreen + " - New Object";
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
                txtControlName.Text = controlName;
                LoadCurrentScreenObject(controlName);
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


            OSAEImageManager imgMgr = new OSAEImageManager();
            try
            {
                imgNormalRaw = imgMgr.GetImage(OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Normal Image").Value);
                imgNormal.Source = LoadImage(imgNormalRaw.Data);
                Validate_Initial_Coordinates();
            }
            catch (Exception ex)
            {
            }

            try
            {
                imgPressedRaw = imgMgr.GetImage(OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Pressed Image").Value);
                imgPressed.Source = LoadImage(imgPressedRaw.Data);
                Validate_Initial_Coordinates();
            }
            catch (Exception ex)
            {
            }
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
            sWorkingName = txtControlName.Text;
            OSAEObjectManager.ObjectAdd(sWorkingName, sWorkingName, "CONTROL CLICK IMAGE", "", currentScreen, true);
            if (imgNormalRaw != null) OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Normal Image", imgNormalRaw.Name, "GUI");
            else OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Normal Image", "", "GUI");
            if (imgPressedRaw != null) OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Pressed Image", imgPressedRaw.Name, "GUI");
            else OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Pressed Image", "", "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Press Object Name", cboPressObject.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Press Method Name", cboPressMethod.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Press Param 1", txtPressParam1.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Press Param 2", txtPressParam2.Text, "GUI");

            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Press Script Name", cboPressScript.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Press Script Param 1", txtPressScriptParam1.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Press Script Param 2", txtPressScriptParam2.Text, "GUI");

            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Release Object Name", cboReleaseObject.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Release Method Name", cboReleaseMethod.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Release Param 1", txtReleaseParam1.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Release Param 2", txtReleaseParam2.Text, "GUI");

            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Release Script Name", cboReleaseScript.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Release Script Param 1", txtReleaseScriptParam1.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Release Script Param 2", txtReleaseScriptParam2.Text, "GUI");

            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "X", "100", "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Y", "100", "GUI");

            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Zorder", "1", "GUI");

            OSAEScreenControlManager.ScreenObjectAdd(currentScreen, "", sWorkingName);

            NotifyParentFinished();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            sWorkingName = txtControlName.Text;
            OSAE.OSAEObject obj = OSAEObjectManager.GetObjectByName(sOriginalName);
            //We call an object update here in case the Name was changed, then perform the updates against the New name
            OSAEObjectManager.ObjectUpdate(sOriginalName,sWorkingName,obj.Description,obj.Type,obj.Address,obj.Container,obj.Enabled);

            if (imgNormalRaw != null) OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Normal Image", imgNormalRaw.Name, "GUI");
            else OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Normal Image", "", "GUI");
            if (imgPressedRaw != null) OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Pressed Image", imgPressedRaw.Name, "GUI");
            else OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Pressed Image", "", "GUI");

            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Press Object Name", cboPressObject.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Press Method Name", cboPressMethod.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Press Param 1", txtPressParam1.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Press Param 2", txtPressParam2.Text, "GUI");

            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Press Script Name", cboPressScript.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Press Script Param 1", txtPressScriptParam1.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Press Script Param 2", txtPressScriptParam2.Text, "GUI");

            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Release Object Name", cboReleaseObject.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Release Method Name", cboReleaseMethod.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Release Param 1", txtReleaseParam1.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Release Param 2", txtReleaseParam2.Text, "GUI");

            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Release Script Name", cboReleaseScript.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Release Script Param 1", txtReleaseScriptParam1.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Release Script Param 2", txtReleaseScriptParam2.Text, "GUI");

            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "X", txtNormalX.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Y", txtNormalY.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sWorkingName, "Zorder", "1", "GUI");

            OSAEScreenControlManager.ScreenObjectAdd(currentScreen, "", sWorkingName);

            NotifyParentFinished();
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
                btnDelete.IsEnabled = true;
            }
        }

        private void txtControlName_TextChanged(object sender, TextChangedEventArgs e)
        {
            Enable_Buttons();
        }

        private void cboPressObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataSet dataSet = OSAESql.RunSQL("SELECT method_name FROM osae_v_object_method where object_name = '" + cboPressObject.SelectedValue + "' order by method_name");
            cboPressMethod.ItemsSource = dataSet.Tables[0].DefaultView;
        }

        private void cboReleaseObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataSet dataSet = OSAESql.RunSQL("SELECT method_name FROM osae_v_object_method where object_name = '" + cboReleaseObject.SelectedValue + "' order by method_name");
            cboReleaseMethod.ItemsSource = dataSet.Tables[0].DefaultView;
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
    }
}
