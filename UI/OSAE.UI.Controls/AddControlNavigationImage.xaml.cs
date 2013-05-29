
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

        public AddControlNavigationImage(string screen)
        {
            InitializeComponent();
            currentScreen = screen;
            LoadScreens();
        }

        /// <summary>
        /// Load the screens from the DB into the combo box
        /// </summary>
        private void LoadScreens()
        {
            DataSet dataSet = OSAESql.RunSQL("SELECT object_name FROM osae_v_object where object_type = 'SCREEN' order by object_name");
            screenComboBox.ItemsSource = dataSet.Tables[0].DefaultView;
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
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show("Please specify a name for the control");
                return;
            }

            if (img == null)
            {
                MessageBox.Show("Please specify an image for the control");
                return;
            }

            if (string.IsNullOrEmpty(screenComboBox.Text))
            {
                MessageBox.Show("Please specify a target for the control");
                return;
            }


            string sName = "Screen - Nav - " + txtName.Text;
            OSAEObjectManager.ObjectAdd(sName, sName, "CONTROL NAVIGATION IMAGE", "", currentScreen, true);
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Image", img.Name, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Screen", screenComboBox.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "X", "100", "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Y", "100", "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Zorder", "1", "GUI");

            OSAEScreenControlManager.ScreenObjectAdd(currentScreen, screenComboBox.Text, sName);

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
