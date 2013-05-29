using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using ICSharpCode.SharpZipLib.Zip;

namespace PluginDescriptionEditor
{
    public partial class EditorForm : Form
    {
        string filename;
        string folder;
        bool descchanged;

        public EditorForm(string[] args)
        {
            InitializeComponent();

            if (args != null && args.Length == 1)
            {
                if (File.Exists(Path.GetFullPath(args[0])))
                {
                    filename = Path.GetFileName(Path.GetFullPath(args[0]));
                    folder = Path.GetDirectoryName(Path.GetFullPath(args[0]));

                    if (filename.EndsWith("osapd", StringComparison.Ordinal))
                    {
                        // its a plugin description

                        LoadFile(Path.GetFullPath(args[0]));
                    }
                }
            }

            this.saveFileDialog.InitialDirectory = folder;

            this.openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();

        }

        private void LoadFile(string file)
        {
            string[] sa = Directory.GetFiles(folder, "*.dll", SearchOption.AllDirectories);
            this.MainFileComboBox.Items.Clear();
            foreach (string s in sa)
            {
                string rs = GetRelativePath(folder, s);
                this.MainFileComboBox.Items.Add(rs);
            }

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(ReadFromFile(file));
            this.NameTextBox.Text = xml.SelectSingleNode("//plugin-name").InnerText;
            this.PluginIdTextBox.Text = xml.SelectSingleNode("//plugin-id").InnerText;
            this.VersionTextBox.Text = xml.SelectSingleNode("//plugin-version").InnerText;
            this.StateComboBox.SelectedItem = xml.SelectSingleNode("//plugin-state").InnerText; 
            this.AuthorTextBox.Text = xml.SelectSingleNode("//author").InnerText;
            this.DescriptionTextBox.Text = xml.SelectSingleNode("//short-description").InnerText;
            this.DestinationFolderTextBox.Text = xml.SelectSingleNode("//destination-folder").InnerText;
            this.MainFileComboBox.SelectedItem = xml.SelectSingleNode("//main-file").InnerText;
            this.WikiUrlTextBox.Text = xml.SelectSingleNode("//wiki-url").InnerText;

            XmlNode temp = xml.SelectSingleNode("//additional-assemblies");
            XmlNodeList childNodes = temp.ChildNodes;

            foreach (XmlNode t in childNodes)
                AdtlAssembliesListView.Items.Add(t.InnerText);

            //Load x64 files if any...
            if ((temp = xml.SelectSingleNode("//x64-additional-assemblies")) != null)
            {
                childNodes = temp.ChildNodes;

                foreach (XmlNode t in childNodes)
                    AdtlAssembliesListView.Items.Add(t.InnerText);
            }

            FormText();
        }


        /// <summary>
        /// Returns a relative path.
        /// E.g. C:\folder and C:\folder\x\y.c would return x\y.c
        /// E.g. C:\folder and C:\y.c would return ..\y.c
        /// </summary>
        /// <param name="relativeTo">The absolute reference to which the path is relative to.</param>
        /// <param name="absolutePath">The absolute path which will be made relative to the other paramater.</param>
        /// <returns></returns>
        private static string GetRelativePath(string relativeTo, string absolutePath)
        {
            string[] absoluteDirectories = relativeTo.Split(Path.DirectorySeparatorChar);
            string[] relativeDirectories = absolutePath.Split(Path.DirectorySeparatorChar);

            //Get the shortest of the two paths
            int length = absoluteDirectories.Length < relativeDirectories.Length ? absoluteDirectories.Length : relativeDirectories.Length;

            //Use to determine where in the loop we exited
            int lastCommonRoot = -1;
            int index;

            //Find common root
            for (index = 0; index < length; index++)
                if (absoluteDirectories[index] == relativeDirectories[index])
                    lastCommonRoot = index;
                else
                    break;

            //If we didn't find a common prefix then throw
            if (lastCommonRoot == -1)
                return relativeTo;

            //Build up the relative path
            StringBuilder relativePath = new StringBuilder();

            //Add on the ..
            for (index = lastCommonRoot + 1; index < absoluteDirectories.Length; index++)
                if (absoluteDirectories[index].Length > 0)
                    relativePath.Append(".." + Path.DirectorySeparatorChar);

            //Add on the folders
            for (index = lastCommonRoot + 1; index < relativeDirectories.Length - 1; index++)
                relativePath.Append(relativeDirectories[index] + Path.DirectorySeparatorChar);
            relativePath.Append(relativeDirectories[relativeDirectories.Length - 1]);

            return relativePath.ToString();
        }

        static string ReadFromFile(string filename)
        {
            StreamReader SR;
            string S;
            SR = File.OpenText(filename);
            S = SR.ReadToEnd();
            SR.Close();
            return S;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.NameTextBox.Text = "";
            this.PluginIdTextBox.Text = "";
            this.VersionTextBox.Text = "";
            this.StateComboBox.SelectedItem = null;
            this.AuthorTextBox.Text = "";
            this.ShortDescriptionTextBox.Text = "";
            this.DestinationFolderTextBox.Text = "";
            this.MainFileComboBox.SelectedItem = null;
            this.WikiUrlTextBox.Text = "";

            filename = string.Empty;
            folder = Directory.GetCurrentDirectory();
            FormText();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // we discard changes
            if (false)//(descchanged)
            {
                DialogResult r;

                r = MessageBox.Show("You have unsaved changes. Save current configuration?",
                    "Warning",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1,
                    0);

                if (r == DialogResult.Cancel)
                {
                    return;
                }
                if (r == DialogResult.Yes)
                {
                    // save
                    saveToolStripMenuItem_Click(null, null);
                }
            }

            // and load 
            if (this.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // then deserialize the file
                try
                {
                    filename = Path.GetFileName(openFileDialog.FileName);
                    folder = Path.GetDirectoryName(openFileDialog.FileName);
                    LoadFile(Path.GetFullPath(openFileDialog.FileName));
                    
                    descchanged = false;
                    FormText();
                }
                catch
                {
                    MessageBox.Show("The file could not be read.",
                        "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1,
                        0
                    );
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckDescription())
            {
                // if invalid filename, we gosub Save As ...
                if (string.IsNullOrEmpty(filename))
                {
                    saveAsToolStripMenuItem1_Click(sender, e);
                    return;
                }

                // we serialize the data to file.
                try
                {
                    SaveFile(Path.Combine(folder, filename));
                    descchanged = false;
                    FormText();
                }
                catch
                {
                    MessageBox.Show("The file could not be written.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                System.ComponentModel.CancelEventArgs ce = e as System.ComponentModel.CancelEventArgs;
                if (ce != null)
                    ce.Cancel = true;
            }
        }

        private void saveAsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (this.saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                filename = Path.GetFileName(this.saveFileDialog.FileName);
                folder = Path.GetDirectoryName(this.saveFileDialog.FileName);
                this.MainFileComboBox.Items.Clear();
                string[] sa = Directory.GetFiles(folder, "*.dll", SearchOption.AllDirectories);
                foreach (string s in sa)
                {
                    string rs = GetRelativePath(folder, s);
                    this.MainFileComboBox.Items.Add(rs);
                }
                saveToolStripMenuItem_Click(sender, e);
            }
            else
            {
                System.ComponentModel.CancelEventArgs ce = e as System.ComponentModel.CancelEventArgs;
                if (ce != null)
                    ce.Cancel = true;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (descchanged)
            {
                DialogResult r;

                r = MessageBox.Show("You have unsaved changes. Save current configuration?", "Warning",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1,
                    0);

                if (r == DialogResult.Cancel)
                {
                    return;
                }
                if (r == DialogResult.Yes)
                {
                    // save
                    saveToolStripMenuItem_Click(null, null);
                }
            }
            // and exit the application
            Application.Exit();
        }

        // Updates the title of the form
        private void FormText()
        {
            string star = string.Empty;
            string minus = string.Empty;

            if (!string.IsNullOrEmpty(filename))
                minus = "- ";
            if (descchanged == true)
                star = " *";

            this.Text = "OSA Plugin Description Editor " + minus + filename + star;
        }

        public static string PackagePlugin(string PluginDescriptionFilePath, string Extension)
        {
            string tempfile = Path.GetTempFileName();
            string zipFileName = Path.Combine(Path.GetDirectoryName(PluginDescriptionFilePath), Path.GetFileNameWithoutExtension(PluginDescriptionFilePath)) + Extension;
            string sourceDirectory = Path.GetDirectoryName(PluginDescriptionFilePath);

            FastZip fastZip = new FastZip();
            try
            {
                fastZip.CreateZip(tempfile, sourceDirectory, true, "");

                if (File.Exists(zipFileName))
                    File.Delete(zipFileName);
                File.Move(tempfile, zipFileName);

                return zipFileName;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Log");
            }
            return null;
        }

        private void createPackageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            string package = string.Empty;

            // check: safe file needed? (this could also add or remove a osapd file)
            System.ComponentModel.CancelEventArgs e2 = new System.ComponentModel.CancelEventArgs(false);
            saveToolStripMenuItem_Click(null, e2);
            if (e2.Cancel)
                return;

            // check: only one .osapd
            int mopdCount = 0;
            
            string[] sa0 = Directory.GetFiles(folder, "*.osapd", SearchOption.TopDirectoryOnly);
            mopdCount = Math.Max(mopdCount, sa0.Length);
            
            if (mopdCount != 1)
            {
                string errortext = string.Empty;

                if (mopdCount == 0)
                    errortext += "The root folder contains no plugin description file.";

                if (mopdCount > 1)
                    errortext += "The root folder contains multiple plugin description files of the same type.";


                errortext += Environment.NewLine +
                            "The package was not created.";

                MessageBox.Show(errortext,
                        "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1,
                        0);
                return;
            }


            string s = Path.GetFileNameWithoutExtension(filename) + ".osapp";
            string[] sa3 = Directory.GetFiles(folder, s, SearchOption.AllDirectories);
            if (sa3.Length > 0)
            {
                DialogResult dr = MessageBox.Show(
                    string.Format(
                            "The folder contains a plugin package with the same name as the one to be created." +
                            Environment.NewLine +
                            "The existing package {0} will be deleted." +
                            Environment.NewLine +
                            Environment.NewLine +
                            "Delete existing package?",
                        s
                    ),
                    "Warning",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2, 0
                    );
                if (dr == DialogResult.OK)
                    File.Delete(sa3[0]);
            }




            if (!string.IsNullOrEmpty(folder) && !string.IsNullOrEmpty(filename))
                package = PackagePlugin(Path.Combine(folder, filename), ".osapp");

            if (!string.IsNullOrEmpty(package))
                MessageBox.Show("Package created:" + Environment.NewLine + package, "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, 0);
            else
                MessageBox.Show("The package was not created:" + Environment.NewLine + package, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, 0);

        

        }

        private bool CheckDescription()
        {
            return true;
        }

        private void SaveFile(string path)
        {
            StreamWriter sw = new StreamWriter(path);

            sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sw.WriteLine("<plugin-description xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" version=\"3.0\">");

            sw.WriteLine("<plugin-name>" + this.NameTextBox.Text + "</plugin-name>");
            sw.WriteLine("<plugin-id>" + this.PluginIdTextBox.Text + "</plugin-id>");
            sw.WriteLine("<plugin-version>" + this.VersionTextBox.Text + "</plugin-version>");
            sw.WriteLine("<plugin-state>" + this.StateComboBox.Text + "</plugin-state>");
            sw.WriteLine("<short-description>" + this.DescriptionTextBox.Text + "</short-description>");
            sw.WriteLine("<author>" + this.AuthorTextBox.Text + "</author>");
            sw.WriteLine("<destination-folder>" + this.DestinationFolderTextBox.Text + "</destination-folder>");
            sw.WriteLine("<main-file>" + this.MainFileComboBox.SelectedItem + "</main-file>");
            sw.WriteLine("<wiki-url>" + this.WikiUrlTextBox.Text + "</wiki-url>");
            sw.WriteLine("<additional-assemblies>");

            foreach (ListViewItem assembly in AdtlAssembliesListView.Items)
            {
                if(Path.GetExtension(assembly.Text)!=".x64")
                    sw.WriteLine("<name>" + assembly.Text + "</name>");
            }

            sw.WriteLine("</additional-assemblies>");

            //Now write the 64bit files
            sw.WriteLine("<x64-additional-assemblies>");

            foreach (ListViewItem assembly in AdtlAssembliesListView.Items)
            {
                if (Path.GetExtension(assembly.Text) == ".x64")
                    sw.WriteLine("<name>" + assembly.Text + "</name>");
            }
            sw.WriteLine("</x64-additional-assemblies>");
            sw.WriteLine("</plugin-description>");

            sw.Close();

        }

        private void GenerateIDButton_Click(object sender, EventArgs e)
        {
            
            this.PluginIdTextBox.Text = Guid.NewGuid().ToString();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog_Add.ShowDialog() == DialogResult.OK)
            {
                // then deserialize the file
                AdtlAssembliesListView.Items.Add(Path.GetFileName(openFileDialog_Add.FileName));
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (AdtlAssembliesListView.SelectedIndices.Count > 0)
            {
                foreach(ListViewItem lvi in AdtlAssembliesListView.SelectedItems)
                {
                    AdtlAssembliesListView.Items.Remove(lvi);
                }

                
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog_Add.ShowDialog() == DialogResult.OK)
            {
                //Check if it already has the .x64 extension
                string file = Path.GetFileName(openFileDialog_Add.FileName);
                string filename = string.Empty;
                if (!(Path.GetExtension(file) == ".x64"))//if not, then add it
                    filename = Path.GetFileName(file) + ".x64";
                else
                    filename = file;
                                
                //Make sure we have already done a saveAs so we know the target directory
                if (string.IsNullOrEmpty(folder))
                {
                    System.ComponentModel.CancelEventArgs e2 = new System.ComponentModel.CancelEventArgs(false);
                    saveAsToolStripMenuItem1_Click(null, e2);
                    if (e2.Cancel)
                        return;
                }
                //Make sure this file is in the target directory (incase we picked it from an external directory)
                if (Directory.GetFiles(folder, filename, SearchOption.TopDirectoryOnly).Length == 0)
                {
                    //copy the file 
                    File.Copy(openFileDialog_Add.FileName, folder + "/" + filename);
                }
                
                AdtlAssembliesListView.Items.Add(Path.GetFileName(filename));
            }
        }
    }
}
