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
    public partial class ctrlChangeScreen : UserControl
    {      
        OSAEImageManager imgMgr = new OSAEImageManager();
        public ctrlChangeScreen()
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
                OSAEObjectCollection screens = OSAEObjectManager.GetObjectsByBaseType("SCREEN");
                List<OSAEImage> images = new List<OSAEImage>();
                foreach (OSAEObject obj in screens)
                {
                    string scrnName = obj.Property("Background Image").Value;
                    OSAEImage img = imgMgr.GetImage(scrnName);
                    img.Name = obj.Name;
                    images.Add(img);
                }

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

        public event EventHandler LoadScreen;
        protected virtual void OnLoadScreen(string screen)
        {
            if (LoadScreen != null) LoadScreen(screen, EventArgs.Empty);
        }

        private void Image_MouseLeftButtonUp_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image img = (Image)sender;
            OnLoadScreen((string)img.Tag);
            NotifyParentFinished();
        }
    }
}
