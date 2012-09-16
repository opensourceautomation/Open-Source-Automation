using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip;
using System.ServiceProcess;

namespace PluginInstaller
{
    partial class Gui_Install : Form
    {
        #region variables
        private string filename = string.Empty;
        private int s = 5;
        private PluginDescription desc;
        private Image MyPluginImage = null;
        #endregion

        public Gui_Install(string Sendfilename)
        {
            InitializeComponent();

            filename = Sendfilename;

            // Unzip osapp to find plugin description
            if (!UnzipOSA(filename))
                this.DialogResult = DialogResult.Cancel;

            //hide progress_bar
            progressBar_frm_gui_install.Visible = false;


            // Show Plugin Image
            string exePath = Path.GetDirectoryName(Application.ExecutablePath);
            if (File.Exists(exePath + "/tempDir/Screenshot.jpg"))
            {
                MyPluginImage = Image.FromFile(exePath + "/tempDir/Screenshot.jpg");
                ImagePictureBox_frm_gui_install.Image = MyPluginImage;
            }

            // popuplate Plugin Information in GUI
            label_gui_install_statut.Text = string.Format("Install {0} {1} {2}?", desc.PluginName, desc.PluginVersion, desc.PluginState);
            label_gui_install_author.Text = string.Format("by {0}", desc.PluginAuthor);
            richTextBox_gui_install.Text = desc.Description;


        }

        #region methods

        private bool UnzipOSA(string PluginPackagePath)
        {
            string exePath = Path.GetDirectoryName(Application.ExecutablePath);
            if (Directory.Exists(exePath + "/tempDir/"))
            {
                Directory.Delete(exePath + "/tempDir/", true);
            }

            string tempfolder = exePath + "/tempDir/";
            string zipFileName = Path.GetFullPath(PluginPackagePath);
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
                desc = PluginInstallerHelper.Deserialize(DescPath);
                return true;
            }
            return false;
        }

        #endregion

        private void button_frm_gui_install_Click_1(object sender, EventArgs e)
        {
            //ImagePictureBox_frm_gui_install.Image.Dispose();
            // Hide RichTextBox
            richTextBox_gui_install.Visible = false;
            // Redraw Form
            Gui_Install.ActiveForm.Size = new System.Drawing.Size(414, 162);
            //show ProgressBar
            progressBar_frm_gui_install.Visible = true;
            ImagePictureBox_frm_gui_install.Image.Dispose();
            this.DialogResult = DialogResult.OK;
        }

        private void button_frm_gui_cancel_Click_1(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }


    }
}