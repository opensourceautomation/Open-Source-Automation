
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
    /// Interaction logic for AddControlMethodImage.xaml
    /// </summary>
    public partial class AddControlMethodImage : UserControl
    {
        private string currentScreen;

        public AddControlMethodImage(string screen)
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

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            string fileName = Path.GetFileName(txtImagePath.Text).Split('.')[0];
            string ext = Path.GetFileName(txtImagePath.Text).Split('.')[1];

            OSAEImageManager imgMgr = new OSAEImageManager();

            int imgID = 0;
            byte[] byt;

            if (ext.ToLower() == "jpg" || ext.ToLower() == "jpeg")
            {
                byt = imgMgr.getJPGFromImageControl((BitmapImage)imgMethod.Source);
                imgID = imgMgr.AddImage(fileName, ext, byt);
            }
            else if (ext.ToLower() == "png")
            {
                byt = imgMgr.getPNGFromImageControl((BitmapImage)imgMethod.Source);
                imgID = imgMgr.AddImage(fileName, ext, byt);
            }

            string sName = currentScreen + " - " + objectComboBox.Text;
            OSAEObjectManager.ObjectAdd(sName, sName, "CONTROL METHOD IMAGE", "", currentScreen, true);
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Object Name", objectComboBox.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Method Name", methodComboBox.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Param 1", txtParam1.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Param 2", txtParam2.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Image", fileName, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "X", "100", "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Y", "100", "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Zorder", "1", "GUI");

            OSAEScreenControlManager.ScreenObjectAdd(currentScreen, objectComboBox.Text, sName);

            NotifyParentFinished();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {

        }

        private void objectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataSet dataSet = OSAESql.RunSQL("SELECT method_name FROM osae_v_object_method where object_name = '" + objectComboBox.SelectedValue + "' order by method_name");
            methodComboBox.ItemsSource = dataSet.Tables[0].DefaultView;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name 
            dlg.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                "Portable Network Graphic (*.png)|*.png"; // Filter files by extension 

            if (dlg.ShowDialog() == true)
            {
                imgMethod.Source = new BitmapImage(new Uri(dlg.FileName));
                txtImagePath.Text = dlg.FileName;
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
            parentWindow.Close();
        }
    }
}
