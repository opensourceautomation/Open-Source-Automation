namespace Manager_WPF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using System.IO;
    using ICSharpCode.SharpZipLib.Zip;
    using OSAE;

    /// <summary>
    /// Interaction logic for InstallPlugin.xaml
    /// </summary>
    public partial class InstallPlugin : Window
    {
        #region variables

        private string filename = string.Empty;
        private int s = 5;
        private PluginDescription desc = new PluginDescription();
        private Image MyPluginImage = null;
        public bool install = false;

        #endregion

        public InstallPlugin(string Sendfilename)
        {
            InitializeComponent();

            filename = Sendfilename;

            // Unzip osapp to find plugin description
            if (!UnzipOSA(filename))
                this.DialogResult = true;

            // Show Plugin Image
            string exePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (File.Exists(exePath + "/tempDir/Screenshot.jpg"))
            {
                // load the image, specify CacheOption so the file is not locked
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(exePath + "/tempDir/Screenshot.jpg");
                image.EndInit();

                pluginImage.Stretch = Stretch.Fill;
                pluginImage.Source = image;
            }

            // popuplate Plugin Information in GUI
            lblPlugin.Content = string.Format("Install {0} {1} {2}?", desc.Name, desc.Version, desc.Status);
            lblAuthor.Content = string.Format("by {0}", desc.Author);
            lblDescription.Content = desc.Description;
        }

        #region methods

        private bool UnzipOSA(string PluginPackagePath)
        {
            string exePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (Directory.Exists(exePath + "/tempDir/"))
            {
                Directory.Delete(exePath + "/tempDir/", true);
            }

            string tempfolder = exePath + "/tempDir/";
            string zipFileName = System.IO.Path.GetFullPath(PluginPackagePath);
            string DescPath = null;

            FastZip fastZip = new FastZip();

            fastZip.ExtractZip(zipFileName, tempfolder, null);
            // find all included plugin descriptions and install the plugins
            List<string> osapdFiles = new List<string>();
            List<string> sqlFiles = new List<string>();

            string[] pluginFile = Directory.GetFiles(tempfolder, "*.osapd", SearchOption.TopDirectoryOnly);
            osapdFiles.AddRange(pluginFile);
            string[] sqlFile = Directory.GetFiles(tempfolder, "*.sql", SearchOption.TopDirectoryOnly);
            sqlFiles.AddRange(sqlFile);

            if (osapdFiles.Count == 0)
            {
                MessageBox.Show("No plugin description files found.");
                return false;
            }

            if (osapdFiles.Count > 1)
            {
                MessageBox.Show("More than one plugin description file found.");
                return false;
            }
            if (osapdFiles.Count == 1)
            {

                DescPath = osapdFiles[0];
            }


            if (!string.IsNullOrEmpty(DescPath))
            {
                desc.Deserialize(DescPath);
                return true;
            }
            return false;
        }

        #endregion

        #region events

        private void btnInstall_Click(object sender, RoutedEventArgs e)
        {
            install = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            install = false;
            this.Close();
        }

        #endregion
    }
}
