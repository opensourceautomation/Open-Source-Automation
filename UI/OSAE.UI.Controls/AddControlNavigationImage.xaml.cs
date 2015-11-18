
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
    /// Interaction logic for AddControlNavigationImage.xaml
    /// </summary>
    public partial class AddControlNavigationImage : UserControl
    {
        private string currentScreen;
        OSAEImage img = new OSAEImage();
        string sOriginalName = "";
        string sWorkingName = "";
        string sMode = "";

        public AddControlNavigationImage(string screen, string controlName = "")
        {
            InitializeComponent();
            currentScreen = screen;
            LoadScreens();
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
                sWorkingName = currentScreen + " - Nav - New Nav Image";
                DataSet dsScreenControl = OSAESql.RunSQL("SELECT COUNT(object_name) FROM osae_v_object where object_name = '" + sWorkingName + "'");
                int iCount = 0;

                while (dsScreenControl.Tables[0].Rows[0][0].ToString() == "1")
                {
                    // We have a duplicate name, we must get a unique name
                    iCount += 1;
                    sWorkingName = currentScreen + " - Nav - New Nav Image " + iCount;
                    dsScreenControl = OSAESql.RunSQL("SELECT COUNT(object_name) FROM osae_v_object where object_name = '" + sWorkingName + "'");
                }
                sMode = "Add";
                controlName = sWorkingName;
                txtName.Text = controlName;
                //LoadCurrentScreenObject(controlName);
            }
            Enable_Buttons();
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


        private void LoadCurrentScreenObject(string controlName)
        {
            cboScreens.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Screen").Value;
            txtX.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "X").Value;
            txtY.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Y").Value;
            txtZOrder.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "ZOrder").Value;

            OSAEImageManager imgMgr = new OSAEImageManager();
            try
            {
                img = imgMgr.GetImage(OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Image").Value);
                imgScreen.Source = LoadImage(img.Data);
                Validate_Initial_Coordinates();
            }
            catch (Exception ex)
            {
            }
        }


        /// <summary>
        /// Load the screens from the DB into the combo box
        /// </summary>
        private void LoadScreens()
        {
            DataSet dataSet = OSAESql.RunSQL("SELECT object_name FROM osae_v_object where object_type = 'SCREEN' order by object_name");
            cboScreens.ItemsSource = dataSet.Tables[0].DefaultView;
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            selectImageWindow dlgSelectImage = new selectImageWindow();
            ctrlSelectImage si = new ctrlSelectImage();
            si.ImagePicked += si_ImagePicked;
            dlgSelectImage.Width = si.Width + 80;
            dlgSelectImage.Height = si.Height + 80;
            dlgSelectImage.Content = si;
            dlgSelectImage.Show();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            NotifyParentFinished();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (validateForm("Add"))
            {
                string sName = txtName.Text;
                OSAEObjectManager.ObjectAdd(sName, sName, sName, "CONTROL NAVIGATION IMAGE", "", currentScreen, true);
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Image", img.Name, "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Screen", cboScreens.Text, "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "X", "100", "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Y", "100", "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Zorder", txtZOrder.Text, "GUI");
                OSAEScreenControlManager.ScreenObjectAdd(currentScreen, cboScreens.Text, sName);
                NotifyParentFinished();
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (validateForm("Update"))
            {
                sWorkingName = txtName.Text;
                OSAE.OSAEObject obj = OSAEObjectManager.GetObjectByName(sOriginalName);
                //We call an object update here in case the Name was changed, then perform the updates against the New name
                OSAEObjectManager.ObjectUpdate(sOriginalName, sWorkingName, obj.Alias, obj.Description, obj.Type, obj.Address, obj.Container, obj.Enabled);
                string sName = txtName.Text;
                OSAEObjectManager.ObjectAdd(sName, sName, sName, "CONTROL NAVIGATION IMAGE", "", currentScreen, true);
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Image", img.Name, "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Screen", cboScreens.Text, "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "X", txtX.Text, "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Y", txtY.Text, "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Zorder", txtZOrder.Text, "GUI");
                OSAEScreenControlManager.ScreenObjectUpdate(currentScreen, cboScreens.Text, sName);
                NotifyParentFinished();
            }
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

        protected void si_ImagePicked(object sender, EventArgs e)
        {
            OSAEImageManager imgMgr = new OSAEImageManager();

            img = imgMgr.GetImage((int)sender);
            imgScreen.Source = LoadImage(img.Data);
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

        private void Validate_Initial_Coordinates()
        {
            //If there is an image, make sure X & Y are not blank
            if (img != null)
            {
                if (txtX.Text == "")
                    txtX.Text = "100";
                if (txtY.Text == "")
                    txtY.Text = "100";
            }
        }

        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            Enable_Buttons();
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
                MessageBox.Show("Please specify a name for the control");
                validate = false;
            }
            if (img.Data == null)
            {
                MessageBox.Show("Please specify an image for the control");
                validate = false;
            }
            if (string.IsNullOrEmpty(cboScreens.Text))
            {
                MessageBox.Show("Please specify a target for the control");
                validate = false;
            }

            return validate;
        }
        
    }
}
