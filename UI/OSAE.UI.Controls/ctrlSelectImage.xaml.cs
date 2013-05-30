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
    /// Interaction logic for ChangeScreen.xaml
    /// </summary>
    public partial class ctrlSelectImage : UserControl
    {      
        OSAEImageManager imgMgr = new OSAEImageManager();
        public ctrlSelectImage()
        {
            InitializeComponent();

            BindImages();
        }

        /// <summary>
        /// Bind Image in List Box Control
        /// </summary>
        private void BindImages()
        {
            try
            {
                List<OSAEImage> images = imgMgr.GetImages();
                
                // Check List Object Count
                if (images.Count > 0)
                {
                    // Bind Data in List Box Control.
                    LsImageGallery.DataContext = images;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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

        public event EventHandler ImagePicked;
        protected virtual void OnImagePicked(int imgID)
        {
            if (ImagePicked != null) ImagePicked(imgID, EventArgs.Empty);
        }

        private void Image_MouseLeftButtonUp_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image img = (Image)sender;
            OnImagePicked(Int32.Parse(img.Tag.ToString()));
            NotifyParentFinished();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //Configure open file dialog box 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name 
            dlg.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                "Portable Network Graphic (*.png)|*.png"; // Filter files by extension 

            if (dlg.ShowDialog() == true)
            {
                BitmapImage img = new BitmapImage(new Uri(dlg.FileName));

                string fileName = Path.GetFileName(dlg.FileName).Split('.')[0];
                string ext = Path.GetFileName(dlg.FileName).Split('.')[1];

                OSAEImageManager imgMgr = new OSAEImageManager();

                int imgID = 0;
                byte[] byt;

                if (ext.ToLower() == "jpg" || ext.ToLower() == "jpeg")
                {
                    byt = imgMgr.getJPGFromImageControl(img);
                    imgID = imgMgr.AddImage(fileName, ext, byt);
                }
                else if (ext.ToLower() == "png")
                {
                    byt = imgMgr.getPNGFromImageControl(img);
                    imgID = imgMgr.AddImage(fileName, ext, byt);
                }
                else if (ext.ToLower() == "gif")
                {
                    byt = imgMgr.getGIFFromImageControl(img);
                    imgID = imgMgr.AddImage(fileName, ext, byt);
                }

                BindImages();
            }
        }
    }

    public class selectImageWindow : Window
    {
        public int imgID;
    }
}
