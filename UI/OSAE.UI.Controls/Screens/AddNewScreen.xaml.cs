namespace OSAE.UI.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Win32;
    using System.IO;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Interaction logic for AddNewScreen.xaml
    /// </summary>
    public partial class AddNewScreen : UserControl
    {
        /// <summary>
        /// OSAE API to interact with OSA DB
        /// </summary>
        private OSAE osae = new OSAE("OSAE.UI.Controls");

        /// <summary>
        /// Default constructor
        /// </summary>
        public AddNewScreen()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Allow the user to select an image from disk
        /// </summary>
        /// <param name="sender">Standard sender argument see MSDN</param>
        /// <param name="e">Standard RoutedEventArgs argument see MSDN</param>
        private void selectImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();            
            dlg.DefaultExt = ".jpg"; 

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                imageTextBox.Text = dlg.FileName;
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(dlg.FileName);
                image.EndInit();

                screenImage.Source = image;
            }
        }

        /// <summary>
        /// Validates if the user has filled in all the required fields
        /// </summary>
        /// <returns>true if all required fields filled in false otherwise</returns>
        private bool ValidateForm()
        {
            bool mandatoryFieldsCompleted = true;

            if (string.IsNullOrEmpty(nameTextBox.Text))
            {
                mandatoryFieldsCompleted = false;
            }

            if (string.IsNullOrEmpty(imageTextBox.Text))
            {
                mandatoryFieldsCompleted = false;
            }

            return mandatoryFieldsCompleted;
        }

        /// <summary>
        /// Add the new screen to the DB
        /// </summary>
        /// <param name="sender">Standard sender argument see MSDN</param>
        /// <param name="e">Standard RoutedEventArgs argument see MSDN</param>
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateForm())
            {
                // TODO deal with screen being in API path

                //osae.ObjectAdd(sName, sName, "SCREEN", "", sName, True);
                //osae.ObjectPropertySet(sName, "Background Image", txtScreenImage.Text);
                NotifyParentFinished();
            }
            else
            {
                MessageBox.Show("Not all the mandatory fields have been completed", "Fields Incomplete", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Cancel adding a new screen
        /// </summary>
        /// <param name="sender">Standard sender argument see MSDN</param>
        /// <param name="e">Standard RoutedEventArgs argument see MSDN</param>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
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
