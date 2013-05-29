
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
        OSAEImage onImg = new OSAEImage();
        OSAEImage offImg = new OSAEImage();

        public AddControlStateImage(string screen)
        {
            InitializeComponent();
            currentScreen = screen;
            LoadObjects();
        }

        /// <summary>
        /// Load the objects from the DB into the combo box
        /// </summary>
        private void LoadObjects()
        {
            DataSet dataSet = OSAESql.RunSQL("SELECT object_name FROM osae_v_object order by object_name");
            objectComboBox.ItemsSource = dataSet.Tables[0].DefaultView;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            selectImageWindow dlgSelectImage = new selectImageWindow();
            ctrlSelectImage si = new ctrlSelectImage();
            si.ImagePicked += si_OnImagePicked;
            dlgSelectImage.Width = si.Width + 80;
            dlgSelectImage.Height = si.Height + 80;
            dlgSelectImage.Content = si;
            dlgSelectImage.Show();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            selectImageWindow dlgSelectImage = new selectImageWindow();
            ctrlSelectImage si = new ctrlSelectImage();
            si.ImagePicked += si_OffImagePicked;
            dlgSelectImage.Width = si.Width + 80;
            dlgSelectImage.Height = si.Height + 80;
            dlgSelectImage.Content = si;
            dlgSelectImage.Show();
        }

        private void objectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            
            string sName = currentScreen + " - " + objectComboBox.Text;
            OSAEObjectManager.ObjectAdd(sName, sName, "CONTROL STATE IMAGE", "", currentScreen, true);
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Object Name", objectComboBox.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 1 Name", "ON", "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 1 Image", onImg.Name, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 1 X", "100", "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 1 Y", "100", "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 2 Name", "OFF", "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 2 Image", offImg.Name, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 2 X", "100", "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "State 2 Y", "100", "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Zorder", "1", "GUI");

            OSAEScreenControlManager.ScreenObjectAdd(currentScreen, objectComboBox.Text, sName);

            NotifyParentFinished();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
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

        protected void si_OnImagePicked(object sender, EventArgs e)
        {
            OSAEImageManager imgMgr = new OSAEImageManager();

            onImg = imgMgr.GetImage((int)sender);
            imgState1.Source = LoadImage(onImg.Data);
        }

        protected void si_OffImagePicked(object sender, EventArgs e)
        {
            OSAEImageManager imgMgr = new OSAEImageManager();

            offImg = imgMgr.GetImage((int)sender);
            imgState2.Source = LoadImage(offImg.Data);
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
