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
        public string currentScreen;
        private Window parentWindow;

        public AddNewCameraViewer()
        {
            InitializeComponent();

            LoadObjects();
        }

        /// <summary>
        /// Load the object types from the DB into the combo box
        /// </summary>
        private void LoadObjects()
        {
            DataSet dataSet = OSAESql.RunSQL("SELECT object_name FROM osae_v_object WHERE object_Type = 'IP CAMERA' ORDER BY object_name");
            objectsComboBox.ItemsSource = dataSet.Tables[0].DefaultView;
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            OSAEObjectManager objectManager = new OSAEObjectManager();

            if (ValidateForm())
            {
                string sName = currentScreen + " - " + objectsComboBox.Text;
                objectManager.ObjectAdd(sName, sName, "CONTROL CAMERA VIEWER", "", currentScreen, true);
                ObjectPopertiesManager.ObjectPropertySet(sName, "Object Name", objectsComboBox.Text, "Weather");
                ObjectPopertiesManager.ObjectPropertySet(sName, "X", "100", "Weather");
                ObjectPopertiesManager.ObjectPropertySet(sName, "Y", "100", "Weather");

                OSAESql.RunSQL("CALL osae_sp_screen_object_add('" + currentScreen + "','" + objectsComboBox.Text + "','" + sName + "')");


                HwndSource source = (HwndSource)PresentationSource.FromVisual(sender as Button);
                System.Windows.Forms.Control ctl = System.Windows.Forms.Control.FromChildHandle(source.Handle);
                ctl.FindForm().Close();
            }
            else
            {
                MessageBox.Show("Not all the mandatory fields have been completed", "Fields Incomplete", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateForm()
        {
            bool mandatoryFieldsCompleted = true;

            if (objectsComboBox.SelectedIndex == -1)
            {
                mandatoryFieldsCompleted = false;
            }

            return mandatoryFieldsCompleted;
        }

      

        private void cancelbutton_Click(object sender, RoutedEventArgs e)
        {
            HwndSource source = (HwndSource)PresentationSource.FromVisual(sender as Button);
            System.Windows.Forms.Control ctl = System.Windows.Forms.Control.FromChildHandle(source.Handle);
            ctl.FindForm().Close();
        }

    }
}
