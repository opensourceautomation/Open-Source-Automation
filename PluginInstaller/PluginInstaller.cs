using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;


namespace PluginInstaller
{
    public partial class PluginInstaller : Form
    {
        List<PluginDescription> pluginList = new List<PluginDescription>();

        public PluginInstaller(string[] args)
        {

            InitializeComponent();
            GetInstalledPlugins();

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                PluginInstallerHelper.InstallPlugin(openFileDialog1.FileName);
                GetInstalledPlugins();
            }
        
        }

        public void GetInstalledPlugins()
        {
            PluginListView.BeginUpdate();

            PluginListView.Items.Clear();

            List<string> files = new List<string>();

            string[] rfiles;
            rfiles = Directory.GetFiles("AddIns/", "*.osapd", SearchOption.AllDirectories);

            files.AddRange(rfiles);

            foreach (string file in files)
            {
                PluginDescription desc = PluginInstallerHelper.Deserialize(file);
                ListViewItem lvi = new ListViewItem(desc.PluginName);
                lvi.SubItems.Add(desc.PluginVersion + " " + desc.PluginState);

                lvi.SubItems.Add(desc.Description);

                lvi.Tag = desc;

                PluginListView.Items.Add(lvi);

                //string[] row1 = { desc.PluginVersion + " " + desc.PluginState, desc.Description };
                //PluginListView.Items.Add(desc.PluginName).SubItems.AddRange(row1);

            }
            PluginListView.EndUpdate();
            this.Refresh();
        }

        private void PluginListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PluginListView.SelectedIndices.Count > 0)
            {
                toolStripButton2.Enabled = true;
                PluginListView.ContextMenuStrip = this.contextMenuStrip2;
            }
            else
            {
                toolStripButton2.Enabled = false;
                PluginListView.ContextMenuStrip = this.contextMenuStrip1;
            }
        
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (PluginListView.SelectedIndices.Count > 0)
            {
                PluginDescription desc = (PluginDescription)PluginListView.SelectedItems[0].Tag;

                if (MessageBox.Show(
                    string.Format(desc.PluginName),
                   "Delete plugin?",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, 0) == DialogResult.Yes)
                {
                    bool b = PluginInstallerHelper.UninstallPlugin(desc);

                    if (b == true)
                    {
                        MessageBox.Show("Package uninstalled.");
                    }
                    else
                    {
                        MessageBox.Show("Package was not uninstalled.");
                    }

                    GetInstalledPlugins();
                }
            }
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
