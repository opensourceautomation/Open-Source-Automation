
namespace OSAE.UI.Controls
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Data;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Interaction logic for CreateScreen.xaml
    /// </summary>
    public partial class AddControlScreen : UserControl
    {
        public string currentScreen;
        OSAEImage img = new OSAEImage();
        private OSAEImageManager imgMgr = new OSAEImageManager();
        string sOriginalName = "";
        string sWorkingName = "";
        string sMode = "";
        string gHostGUI;

        public AddControlScreen(string screen)
        {
            InitializeComponent();
            currentScreen = screen;

            //Check if Screen Name was passed in, if so, goto edit mode
            if (currentScreen != "")
            {
                //Let's validate the Screen Name and then call a Pre-Load of its properties
                DataSet dsScreenControl = OSAESql.RunSQL("SELECT COUNT(object_name) FROM osae_v_object where object_name = '" + currentScreen + "'");
                if (dsScreenControl.Tables[0].Rows[0][0].ToString() == "1")
                {
                    // We have a hit, this is an Update call, se call the preload
                    sMode = "Update";
                    sOriginalName = currentScreen;
                    txtScreenName.Text = currentScreen;
                    LoadCurrentScreenObject(currentScreen);
                }
            }

            if (currentScreen == "")
            {
                //Let's create a new name
                sWorkingName = "Screen - New Screen";
                DataSet dsScreenControl = OSAESql.RunSQL("SELECT COUNT(object_name) FROM osae_v_object where object_name = '" + sWorkingName + "'");
                int iCount = 0;

                while (dsScreenControl.Tables[0].Rows[0][0].ToString() == "1")
                {
                    // We have a duplicate name, we must get a unique name
                    iCount += 1;
                    sWorkingName = "Screen - New Screen" + iCount;
                    dsScreenControl = OSAESql.RunSQL("SELECT COUNT(object_name) FROM osae_v_object where object_name = '" + sWorkingName + "'");
                }
                sMode = "Add";
                //currentScreen = sWorkingName.Replace("Screen - ","");
                //txtScreenName.Text = currentScreen;
                //LoadCurrentScreenObject(currentScreen);
            }
            Enable_Buttons();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (validateForm("Add"))
            {
                string tempName = txtScreenName.Text;
                OSAEObjectManager.ObjectAdd(tempName, tempName, tempName, "SCREEN", "", tempName, true);
                OSAEObjectPropertyManager.ObjectPropertySet(tempName, "Background Image", img.Name, "GUI");
                currentScreen = txtScreenName.Text;
                NotifyParentFinished();
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (validateForm("Update"))
            {
                string tempName = txtScreenName.Text;
                OSAE.OSAEObject obj = OSAEObjectManager.GetObjectByName(sOriginalName);
                //We call an object update here in case the Name was changed, then perform the updates against the New name
                OSAEObjectManager.ObjectUpdate(sOriginalName, tempName, obj.Alias, obj.Description, obj.Type, obj.Address, obj.Container, obj.Enabled);
                OSAEObjectPropertyManager.ObjectPropertySet(tempName, "Background Image", img.Name, "GUI");
                NotifyParentFinished();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            //read in the Container/Object ID to hack this delete until it gets made into a Proc/API call
            DataSet dsScreenControlID = OSAESql.RunSQL("SELECT object_id FROM osae_object where object_name = '" + currentScreen + "'");
            string iObjectID = dsScreenControlID.Tables[0].Rows[0][0].ToString();
            //Delete the controls on the screen, then the screen, then load a valid screen
            DataSet dsTemp = OSAESql.RunSQL("DELETE FROM osae_object WHERE container_id = " + iObjectID);
            dsTemp = OSAESql.RunSQL("DELETE FROM osae_object WHERE object_name ='" + currentScreen + "'");
            OSAEObjectCollection screens = OSAEObjectManager.GetObjectsByType("SCREEN");
            if (screens.Count > 0)
                currentScreen = screens[0].Name;
            else
                currentScreen = "";
            NotifyParentFinished();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            NotifyParentFinished();
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

        //public event EventHandler LoadScreen;
        //protected virtual void OnLoadScreen()
       // {
       //     if (LoadScreen != null) LoadScreen(currentScreen, EventArgs.Empty);
      //  }

        protected void si_ImagePicked(object sender, EventArgs e)
        {
           // OSAEImageManager imgMgr = new OSAEImageManager();

            img = imgMgr.GetImage((int)sender);
            imgScreen.Source = LoadImage(img.Data);
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
            if (sMode == "Update" && sOriginalName == txtScreenName.Text)
            {
                btnAdd.IsEnabled = false;
                btnUpdate.IsEnabled = true;
                btnDelete.IsEnabled = true;
            }
            //Now we handle Updates WITH name changes
            if (sMode == "Update" && sOriginalName != txtScreenName.Text)
            {
                btnAdd.IsEnabled = true;
                btnUpdate.IsEnabled = true;
                btnDelete.IsEnabled = false;
            }
        }


        private void txtScreenName_TextChanged(object sender, TextChangedEventArgs e)
        {
            Enable_Buttons();
        }

        private void LoadCurrentScreenObject(string screenName)
        {
            OSAEImageManager imgMgr = new OSAEImageManager();
            try
            {
                img = imgMgr.GetImage(OSAEObjectPropertyManager.GetObjectPropertyValue(screenName, "Background Image").Value);
                if (img != null)
                    imgScreen.Source = LoadImage(img.Data);
            }
            catch { }
         }

        private bool validateForm(string mthd)
        {
            bool validate = true;
            // Does this object already exist
            if (mthd == "Add" || sOriginalName != txtScreenName.Text)
            {
                try
                {
                    OSAEObject oExist = OSAEObjectManager.GetObjectByName(txtScreenName.Text);
                    if (oExist != null)
                    {
                        MessageBox.Show("Control name already exist. Please Change!");
                        validate = false;
                    }
                }
                catch { }
            }
            if (string.IsNullOrEmpty(txtScreenName.Text))
            {
                MessageBox.Show("You must enter a Name for this Control!");
                validate = false;
            }
            if (img == null)
            {
                MessageBox.Show("Please specify an image for the screen");
                validate = false;
            }
         
            return validate;
        }
    }
}
