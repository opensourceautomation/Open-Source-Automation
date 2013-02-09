namespace OSAE.UI.Controls
{
    using System.Data;
    using System.Windows;
    using System.Windows.Controls;
    using System.Collections.Generic; 

    /// <summary>
    /// Interaction logic for AddNewObject control
    /// </summary>
    public partial class AddNewObject : UserControl
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public AddNewObject()
        {
            InitializeComponent();
            LoadObjectTypes();
            LoadContainers();
        }

        /// <summary>
        /// Load the object types from the DB into the combo box
        /// </summary>
        private void LoadObjectTypes()
        {
            DataSet dataSet = OSAESql.RunSQL("SELECT object_type FROM osae_v_object_type WHERE base_type<>'CONTROL' ORDER BY object_type");
            objectTypesComboBox.ItemsSource = dataSet.Tables[0].DefaultView;
        }

        /// <summary>
        /// Load the available containers into the containers combo box
        /// </summary>
        private void LoadContainers()
        {
            DataSet dataSet = OSAESql.RunSQL("SELECT object_name FROM osae_v_object WHERE container=1 ORDER BY object_name");
            containersComboBox.ItemsSource = dataSet.Tables[0].DefaultView;
        }

        /// <summary>
        /// Add the new object to the system
        /// </summary>
        /// <param name="sender">Standard sender argument see MSDN</param>
        /// <param name="e">Standard RoutedEventArgs argument see MSDN</param>
        private void addButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ValidateForm())
            {
                OSAEObjectManager.ObjectAdd(
                    nameTextBox.Text,
                    descriptionTextBox.Text,
                    objectTypesComboBox.SelectedValue.ToString(),
                    addressTextBox.Text,
                    (containersComboBox.SelectedIndex == -1) ? "" : containersComboBox.SelectedValue.ToString(),
                    true);
            }
            else
            {
                MessageBox.Show("Not all the mandatory fields have been completed", "Fields Incomplete", MessageBoxButton.OK, MessageBoxImage.Error);
                NotifyParentFinished();
            }
        }

        /// <summary>
        /// Validates if the user has filled in all the required fields
        /// </summary>
        /// <returns>true if all required fields filled in false otherwise</returns>
        private bool ValidateForm()
        {
            bool mandatoryFieldsCompleted = true;

            if (nameTextBox.Text.Length == 0)
            {
                mandatoryFieldsCompleted = false;
            }

            if (objectTypesComboBox.SelectedIndex == -1)
            {
                mandatoryFieldsCompleted = false;
            }

            return mandatoryFieldsCompleted;        
        }

        /// <summary>
        /// Close the form as user has cancelled
        /// </summary>
        /// <param name="sender">Standard sender argument see MSDN</param>
        /// <param name="e">Standard RoutedEventArgs argument see MSDN</param>
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

        /// <summary>
        /// Gives a default value to the description field when the focus to the name text box is lost
        /// </summary>
        /// <param name="sender">Standard sender argument see MSDN</param>
        /// <param name="e">Standard RoutedEventArgs argument see MSDN</param>
        private void nameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(descriptionTextBox.Text))
            {
                descriptionTextBox.Text = nameTextBox.Text;
            }
        }
    }
}
