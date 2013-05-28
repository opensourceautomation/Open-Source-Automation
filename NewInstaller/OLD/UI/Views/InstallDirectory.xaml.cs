using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OSA.Bootstrapper
{
    /// <summary>
    /// Interaction logic for InstallDirectory.xaml
    /// </summary>
    public partial class InstallDirectory : System.Windows.Controls.UserControl
    {
        public InstallDirectory()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.SelectedPath = "C:\\";

            DialogResult result = folderDialog.ShowDialog();
            //if (result.ToString() == "OK")
                //textBox2.Text = folderDialog.SelectedPath;

        }
    }
}
