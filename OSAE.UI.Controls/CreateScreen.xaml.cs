
namespace OSAE.UI.Controls
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Interaction logic for CreateScreen.xaml
    /// </summary>
    public partial class CreateScreen : UserControl
    {
        private string screenName;

        public CreateScreen()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show("Please specify a name for the screen");
                return;
            }

            if (string.IsNullOrEmpty(txtPath.Text))
            {
                MessageBox.Show("Please specify an image for the screen");
                return;
            }

            string fileName = Path.GetFileName(txtPath.Text).Split('.')[0];
            string ext = Path.GetFileName(txtPath.Text).Split('.')[1];

            OSAEImageManager imgMgr = new OSAEImageManager();

            int imgID = 0;
            byte[] byt;

            if (ext.ToLower() == "jpg" || ext.ToLower() == "jpeg")
            {
                byt = imgMgr.getJPGFromImageControl((BitmapImage)imgScreen.Source);
                imgID = imgMgr.AddImage(fileName, ext, byt);
            }
            else if (ext.ToLower() == "png")
            {
                byt = imgMgr.getPNGFromImageControl((BitmapImage)imgScreen.Source);
                imgID = imgMgr.AddImage(fileName, ext, byt);
            }

            screenName = "Screen - " + txtName.Text;
            OSAEObjectManager.ObjectAdd(screenName, screenName, "SCREEN", "", screenName, true);
            OSAEObjectPopertyManager.ObjectPropertySet(screenName, "Background Image", fileName, "GUI");

            OnLoadScreen();

            NotifyParentFinished();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            NotifyParentFinished();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name 
            dlg.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                "Portable Network Graphic (*.png)|*.png"; // Filter files by extension 

            if (dlg.ShowDialog() == true)
            {
                imgScreen.Source = new BitmapImage(new Uri(dlg.FileName));
                txtPath.Text = dlg.FileName;
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

        public event EventHandler LoadScreen;
        protected virtual void OnLoadScreen()
        {
            if (LoadScreen != null) LoadScreen(screenName, EventArgs.Empty);
        }
    }
}
