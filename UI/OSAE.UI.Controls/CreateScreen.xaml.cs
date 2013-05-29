
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
        OSAEImage img = new OSAEImage();

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

            if (img == null)
            {
                MessageBox.Show("Please specify an image for the screen");
                return;
            }


            screenName = "Screen - " + txtName.Text;
            OSAEObjectManager.ObjectAdd(screenName, screenName, "SCREEN", "", screenName, true);
            OSAEObjectPropertyManager.ObjectPropertySet(screenName, "Background Image", img.Name, "GUI");

            OnLoadScreen();

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

        public event EventHandler LoadScreen;
        protected virtual void OnLoadScreen()
        {
            if (LoadScreen != null) LoadScreen(screenName, EventArgs.Empty);
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
