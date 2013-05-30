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

        public AddNewCameraViewer(string screen)
        {
            InitializeComponent();
            currentScreen = screen;
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
            if (ValidateForm())
            {
                string sName = currentScreen + " - " + objectsComboBox.Text;
                OSAEObjectManager.ObjectAdd(sName, sName, "CONTROL CAMERA VIEWER", "", currentScreen, true);
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Object Name", objectsComboBox.Text, "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "X", "100", "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "Y", "100", "GUI");
                OSAEObjectPropertyManager.ObjectPropertySet(sName, "ZOrder", "1", "GUI");

                OSAEScreenControlManager.ScreenObjectAdd(currentScreen, objectsComboBox.Text, sName);


                NotifyParentFinished();
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

    }
}
