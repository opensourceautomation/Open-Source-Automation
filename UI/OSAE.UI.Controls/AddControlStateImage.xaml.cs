
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
    /// Interaction logic for AddControlStateImage.xaml
    /// </summary>
    public partial class AddControlStateImage : UserControl
    {
        private string currentScreen;
        OSAEImage State1Img1 = new OSAEImage();
        OSAEImage State1Img2 = new OSAEImage();
        OSAEImage State1Img3 = new OSAEImage();
        OSAEImage State1Img4 = new OSAEImage();

        OSAEImage State2Img1 = new OSAEImage();
        OSAEImage State2Img2 = new OSAEImage();
        OSAEImage State2Img3 = new OSAEImage();
        OSAEImage State2Img4 = new OSAEImage();
        
        string sOriginalName = "";
        string sWorkingName = "";
        string sMode = "";

        public AddControlStateImage(string screen, string controlName = "")
        {
            InitializeComponent();
            currentScreen = screen;
            LoadObjects();

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
                sWorkingName = currentScreen + " - New State Image";
                DataSet dsScreenControl = OSAESql.RunSQL("SELECT COUNT(object_name) FROM osae_v_object where object_name = '" + sWorkingName + "'");
                int iCount = 0;

                while (dsScreenControl.Tables[0].Rows[0][0].ToString() == "1")
                {
                    // We have a duplicate name, we must get a unique name
                    iCount += 1;
                    sWorkingName = currentScreen + " - New State Image" + iCount;
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
            OSAEImageManager imgMgr = new OSAEImageManager();
            cboObject.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Object Name").Value;
            cboState1.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "State 1 Name").Value;
            try
            {
                State1Img1 = imgMgr.GetImage(OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "State 1 Image").Value);
                imgState1Img1.Source = LoadImage(State1Img1.Data);
                Validate_Initial_Coordinates();
            }
            catch
            {
            }
            try
            {
                State1Img2 = imgMgr.GetImage(OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "State 1 Image 2").Value);
                imgState1Img2.Source = LoadImage(State1Img2.Data);
                Validate_Initial_Coordinates();
            }
            catch
            {
            }
            try
            {
                State1Img3 = imgMgr.GetImage(OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "State 1 Image 3").Value);
                imgState1Img3.Source = LoadImage(State1Img3.Data);
                Validate_Initial_Coordinates();
            }
            catch 
            {
            }
            try
            {
                State1Img4 = imgMgr.GetImage(OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "State 1 Image 4").Value);
                imgState1Img4.Source = LoadImage(State1Img4.Data);
                Validate_Initial_Coordinates();
            }
            catch 
            {
            }

            txtState1X.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "State 1 X").Value;
            txtState1Y.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "State 1 Y").Value;
            txtZOrder.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "ZOrder").Value;

            cboState2.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "State 2 Name").Value;
            try
            {
                State2Img1 = imgMgr.GetImage(OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "State 2 Image").Value);
                imgState2Img1.Source = LoadImage(State2Img1.Data);
                Validate_Initial_Coordinates();
            }
            catch
            {
            }
            try
            {
                State2Img2 = imgMgr.GetImage(OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "State 2 Image 2").Value);
                imgState2Img2.Source = LoadImage(State2Img2.Data);
                Validate_Initial_Coordinates();
            }
            catch 
            {
            }
            try
            {
                State2Img3 = imgMgr.GetImage(OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "State 2 Image 3").Value);
                imgState2Img3.Source = LoadImage(State2Img3.Data);
                Validate_Initial_Coordinates();
            }
            catch 
            {
            }
            try
            {
                State2Img4 = imgMgr.GetImage(OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "State 2 Image 4").Value);
                imgState2Img4.Source = LoadImage(State2Img4.Data);
                Validate_Initial_Coordinates();
            }
            catch 
            {
            }

            txtState2X.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "State 2 X").Value;
            txtState2Y.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "State 2 Y").Value;

            cboSliderMethod.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Slider Method").Value;

            try
            {
                chkRepeat.IsChecked = Convert.ToBoolean(OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Repeat Animation").Value);
            }
            catch 
            {
                chkRepeat.IsChecked = true;
            }
            try
            {
                chkSlider.IsChecked = Convert.ToBoolean(OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Show Slider").Value);
            }
            catch
            {
                chkSlider.IsChecked = false;
            }
            try
            {
                txtDelay.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Frame Delay").Value;
            }
            catch 
            {
                txtDelay.Text = "500";
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


        private void Validate_Initial_Coordinates()
        {
            //If there is an image, make sure X & Y are not blank
            if (imgState1Img1 != null)
            {
                if (txtState1X.Text == "")
                    txtState1X.Text = "100";
                if (txtState1Y.Text == "")
                    txtState1Y.Text = "100";
            }

            if (imgState2Img1 != null)
            {
                if (txtState2X.Text == "")
                    txtState2X.Text = "100";
                if (txtState2Y.Text == "")
                    txtState2Y.Text = "100";
            }
        }


        /// <summary>
        /// Load the objects from the DB into the combo box
        /// </summary>
        private void LoadObjects()
        {
            DataSet dataSet = OSAESql.RunSQL("SELECT object_name FROM osae_v_object order by object_name");
            cboObject.ItemsSource = dataSet.Tables[0].DefaultView;
        }

        private void btnLoadS1I1_Click(object sender, RoutedEventArgs e)
        {
            selectImageWindow dlgSelectImage = new selectImageWindow();
            ctrlSelectImage si = new ctrlSelectImage();
            si.ImagePicked += si_S1I1ImagePicked;
            dlgSelectImage.Width = si.Width + 80;
            dlgSelectImage.Height = si.Height + 80;
            dlgSelectImage.Content = si;
            dlgSelectImage.Show();
        }
        private void btnLoadS1I1_RightClick(object sender, RoutedEventArgs e)
        {
            State1Img1 = null;
            imgState1Img1.Source = null;
        }

        private void btnLoadS1I2_Click(object sender, RoutedEventArgs e)
        {
            selectImageWindow dlgSelectImage = new selectImageWindow();
            ctrlSelectImage si = new ctrlSelectImage();
            si.ImagePicked += si_S1I2ImagePicked;
            dlgSelectImage.Width = si.Width + 80;
            dlgSelectImage.Height = si.Height + 80;
            dlgSelectImage.Content = si;
            dlgSelectImage.Show();
        }
        private void btnLoadS1I2_RightClick(object sender, RoutedEventArgs e)
        {
            State1Img2 = null;
            imgState1Img2.Source = null;
        }

        private void btnLoadS1I3_Click(object sender, RoutedEventArgs e)
        {
            selectImageWindow dlgSelectImage = new selectImageWindow();
            ctrlSelectImage si = new ctrlSelectImage();
            si.ImagePicked += si_S1I3ImagePicked;
            dlgSelectImage.Width = si.Width + 80;
            dlgSelectImage.Height = si.Height + 80;
            dlgSelectImage.Content = si;
            dlgSelectImage.Show();
        }
        private void btnLoadS1I3_RightClick(object sender, RoutedEventArgs e)
        {
            State1Img3 = null;
            imgState1Img3.Source = null;
        }

        private void btnLoadS1I4_Click(object sender, RoutedEventArgs e)
        {
            selectImageWindow dlgSelectImage = new selectImageWindow();
            ctrlSelectImage si = new ctrlSelectImage();
            si.ImagePicked += si_S1I4ImagePicked;
            dlgSelectImage.Width = si.Width + 80;
            dlgSelectImage.Height = si.Height + 80;
            dlgSelectImage.Content = si;
            dlgSelectImage.Show();
        }
        private void btnLoadS1I4_RightClick(object sender, RoutedEventArgs e)
        {
            State1Img4 = null;
            imgState1Img4.Source = null;
        }

        private void btnLoadS2I1_Click(object sender, RoutedEventArgs e)
        {
            selectImageWindow dlgSelectImage = new selectImageWindow();
            ctrlSelectImage si = new ctrlSelectImage();
            si.ImagePicked += si_S2I1ImagePicked;
            dlgSelectImage.Width = si.Width + 80;
            dlgSelectImage.Height = si.Height + 80;
            dlgSelectImage.Content = si;
            dlgSelectImage.Show();
        }
        private void btnLoadS2I1_RightClick(object sender, RoutedEventArgs e)
        {
            State2Img1 = null;
            imgState2Img1.Source = null;
        }

        private void btnLoadS2I2_Click(object sender, RoutedEventArgs e)
        {
            selectImageWindow dlgSelectImage = new selectImageWindow();
            ctrlSelectImage si = new ctrlSelectImage();
            si.ImagePicked += si_S2I2ImagePicked;
            dlgSelectImage.Width = si.Width + 80;
            dlgSelectImage.Height = si.Height + 80;
            dlgSelectImage.Content = si;
            dlgSelectImage.Show();
        }
        private void btnLoadS2I2_RightClick(object sender, RoutedEventArgs e)
        {
            State2Img2 = null;
            imgState2Img2.Source = null;
        }

        private void btnLoadS2I3_Click(object sender, RoutedEventArgs e)
        {
            selectImageWindow dlgSelectImage = new selectImageWindow();
            ctrlSelectImage si = new ctrlSelectImage();
            si.ImagePicked += si_S2I3ImagePicked;
            dlgSelectImage.Width = si.Width + 80;
            dlgSelectImage.Height = si.Height + 80;
            dlgSelectImage.Content = si;
            dlgSelectImage.Show();
        }
        private void btnLoadS2I3_RightClick(object sender, RoutedEventArgs e)
        {
            State2Img3 = null;
            imgState2Img3.Source = null;
        }

        private void btnLoadS2I4_Click(object sender, RoutedEventArgs e)
        {
            selectImageWindow dlgSelectImage = new selectImageWindow();
            ctrlSelectImage si = new ctrlSelectImage();
            si.ImagePicked += si_S2I4ImagePicked;
            dlgSelectImage.Width = si.Width + 80;
            dlgSelectImage.Height = si.Height + 80;
            dlgSelectImage.Content = si;
            dlgSelectImage.Show();
        }
        private void btnLoadS2I4_RightClick(object sender, RoutedEventArgs e)
        {
            State2Img4 = null;
            imgState2Img4.Source = null;
        }


        private void cboObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataSet dataSet = OSAESql.RunSQL("SELECT state_name FROM osae_v_object_state where object_name = '" + cboObject.SelectedValue + "' order by state_name");
            cboState1.ItemsSource = dataSet.Tables[0].DefaultView;
            cboState2.ItemsSource = dataSet.Tables[0].DefaultView;
            DataSet dataSet2 = OSAESql.RunSQL("SELECT method_name FROM osae_v_object_method where object_name = '" + cboObject.SelectedValue + "' order by method_name");
            cboSliderMethod.ItemsSource = dataSet2.Tables[0].DefaultView;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (validateForm("Add"))
            {
                string sName = txtControlName.Text;
                OSAEObjectManager.ObjectAdd(sName, sName, sName, "CONTROL STATE IMAGE", "", currentScreen, true);
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Object Name", cboObject.Text, "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 1 Name", cboState1.Text, "GUI");
                if (State1Img1 != null) OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 1 Image", State1Img1.Name, "GUI");
                else OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 1 Image", "", "GUI");
                if (State1Img2 != null) OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 1 Image 2", State1Img2.Name, "GUI");
                else OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 1 Image 2", "", "GUI");
                if (State1Img3 != null) OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 1 Image 3", State1Img3.Name, "GUI");
                else OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 1 Image 3", "", "GUI");
                if (State1Img4 != null) OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 1 Image 4", State1Img4.Name, "GUI");
                else OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 1 Image 4", "", "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 1 X", txtState1X.Text, "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 1 Y", txtState1Y.Text, "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 2 Name", cboState2.Text, "GUI");
                if (State2Img1 != null) OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 2 Image", State2Img1.Name, "GUI");
                else OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 2 Image", "", "GUI");
                if (State2Img2 != null) OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 2 Image 2", State2Img2.Name, "GUI");
                else OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 2 Image 2", "", "GUI");
                if (State2Img3 != null) OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 2 Image 3", State2Img3.Name, "GUI");
                else OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 2 Image 3", "", "GUI");
                if (State2Img4 != null) OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 2 Image 4", State2Img4.Name, "GUI");
                else OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 2 Image 4", "", "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 2 X", txtState2X.Text, "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 2 Y", txtState2Y.Text, "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "ZOrder", txtZOrder.Text, "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Repeat Animation", chkRepeat.IsChecked.ToString(), "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Frame Delay", txtDelay.Text, "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Show Slider", chkSlider.IsChecked.ToString(), "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Slider Method", cboSliderMethod.Text, "GUI");
                OSAEScreenControlManager.ScreenObjectAdd(currentScreen, cboObject.Text, sName);
                NotifyParentFinished();
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (validateForm("Update"))
            {
                string sName = txtControlName.Text;
                //OSAEObjectManager.ObjectAdd(sName, sName, sName, "CONTROL STATE IMAGE", "", currentScreen, true);
                OSAEObjectManager.ObjectUpdate(sOriginalName, sName, sName, sName, "CONTROL STATE IMAGE", "", currentScreen, 1);
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Object Name", cboObject.Text, "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 1 Name", cboState1.Text, "GUI");
                if (State1Img1 != null) OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 1 Image", State1Img1.Name, "GUI");
                else OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 1 Image", "", "GUI");
                if (State1Img2 != null) OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 1 Image 2", State1Img2.Name, "GUI");
                else OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 1 Image 2", "", "GUI");
                if (State1Img3 != null) OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 1 Image 3", State1Img3.Name, "GUI");
                else OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 1 Image 3", "", "GUI");
                if (State1Img4 != null) OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 1 Image 4", State1Img4.Name, "GUI");
                else OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 1 Image 4", "", "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 1 X", txtState1X.Text, "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 1 Y", txtState1Y.Text, "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 2 Name", cboState2.Text, "GUI");
                if (State2Img1 != null) OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 2 Image", State2Img1.Name, "GUI");
                else OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 2 Image", "", "GUI");
                if (State2Img2 != null) OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 2 Image 2", State2Img2.Name, "GUI");
                else OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 2 Image 2", "", "GUI");
                if (State2Img3 != null) OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 2 Image 3", State2Img3.Name, "GUI");
                else OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 2 Image 3", "", "GUI");
                if (State2Img4 != null) OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 2 Image 4", State2Img4.Name, "GUI");
                else OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 2 Image 4", "", "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 2 X", txtState2X.Text, "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 2 Y", txtState2Y.Text, "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "ZOrder", txtZOrder.Text, "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Repeat Animation", chkRepeat.IsChecked.ToString(), "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Frame Delay", txtDelay.Text, "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Show Slider", chkSlider.IsChecked.ToString(), "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Slider Method", cboSliderMethod.Text, "GUI");
                OSAEScreenControlManager.ScreenObjectUpdate(currentScreen, cboObject.Text, sName);
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

        protected void si_S1I1ImagePicked(object sender, EventArgs e)
        {
            OSAEImageManager imgMgr = new OSAEImageManager();

            State1Img1 = imgMgr.GetImage((int)sender);
            imgState1Img1.Source = LoadImage(State1Img1.Data);
            imgState1Img1.ToolTip = "Width:" + imgState1Img1.Width + " Height:" + imgState1Img1.Height;
            Validate_Initial_Coordinates();
        }

        protected void si_S1I2ImagePicked(object sender, EventArgs e)
        {
            OSAEImageManager imgMgr = new OSAEImageManager();

            State1Img2 = imgMgr.GetImage((int)sender);
            imgState1Img2.Source = LoadImage(State1Img2.Data);
            Validate_Initial_Coordinates();
        }

        protected void si_S1I3ImagePicked(object sender, EventArgs e)
        {
            OSAEImageManager imgMgr = new OSAEImageManager();

            State1Img3 = imgMgr.GetImage((int)sender);
            imgState1Img3.Source = LoadImage(State1Img3.Data);
            Validate_Initial_Coordinates();
        }

        protected void si_S1I4ImagePicked(object sender, EventArgs e)
        {
            
            OSAEImageManager imgMgr = new OSAEImageManager();

            State1Img4 = imgMgr.GetImage((int)sender);
            imgState1Img4.Source = LoadImage(State1Img4.Data);
            Validate_Initial_Coordinates();
        }


        protected void si_S2I1ImagePicked(object sender, EventArgs e)
        {
            OSAEImageManager imgMgr = new OSAEImageManager();

            State2Img1 = imgMgr.GetImage((int)sender);
            imgState2Img1.Source = LoadImage(State2Img1.Data);
            Validate_Initial_Coordinates();
        }

        protected void si_S2I2ImagePicked(object sender, EventArgs e)
        {
            OSAEImageManager imgMgr = new OSAEImageManager();

            State2Img2 = imgMgr.GetImage((int)sender);
            imgState2Img2.Source = LoadImage(State2Img2.Data);
            Validate_Initial_Coordinates();
        }

        protected void si_S2I3ImagePicked(object sender, EventArgs e)
        {
            OSAEImageManager imgMgr = new OSAEImageManager();

            State2Img3 = imgMgr.GetImage((int)sender);
            imgState2Img3.Source = LoadImage(State2Img3.Data);
            Validate_Initial_Coordinates();
        }

        protected void si_S2I4ImagePicked(object sender, EventArgs e)
        {
            OSAEImageManager imgMgr = new OSAEImageManager();

            State2Img4 = imgMgr.GetImage((int)sender);
            imgState2Img4.Source = LoadImage(State2Img4.Data);
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

        private void txtControlName_TextChanged(object sender, TextChangedEventArgs e)
        {
            Enable_Buttons();
        }

        private void chkSlider_Checked(object sender, RoutedEventArgs e)
        {
            cboSliderMethod.IsEnabled = true;
        }

        private void chkSlider_Unchecked(object sender, RoutedEventArgs e)
        {
            cboSliderMethod.IsEnabled = false;
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
                catch {}
            }
            if (string.IsNullOrEmpty(txtControlName.Text))
            {
                MessageBox.Show("You must enter a Name for this control!");
                validate = false;
            }
            if (string.IsNullOrEmpty(cboObject.Text))
            {
                MessageBox.Show("You must choose an associated object!");
                validate = false;
            }
            if(string.IsNullOrEmpty(cboState1.Text))
            {
                MessageBox.Show("You must set the State 1 property!");
                validate = false;
            }
            if(string.IsNullOrEmpty(cboState2.Text))
            {
                MessageBox.Show("You must set the State 2 property!");
                validate = false;
            }
            if(imgState1Img1.Source == null)
            {
                MessageBox.Show("You must select Image 1 for State 1!");
                validate = false;
            }
            if (imgState2Img1.Source == null)
            {
                MessageBox.Show("You must select Image 1 for State 2!");
                validate = false;
            }
            if (string.IsNullOrEmpty(txtState1X.Text))
            {
                MessageBox.Show("State 1 X Can not be empty");
                validate = false;
            }
            if (string.IsNullOrEmpty(txtState1Y.Text))
            {
                MessageBox.Show("State 1 Y Can not be empty");
                validate = false;
            }
            if (string.IsNullOrEmpty(txtState2X.Text))
            {
                MessageBox.Show("State 2 X Can not be empty");
                validate = false;
            }
            if (string.IsNullOrEmpty(txtState2Y.Text))
            {
                MessageBox.Show("State 2 Y Can not be empty");
                validate = false;
            }
            if (string.IsNullOrEmpty(txtZOrder.Text))
            {
                MessageBox.Show("ZOrder can not be empty");
                validate = false;
            }
            return validate;
        }

    }
}
