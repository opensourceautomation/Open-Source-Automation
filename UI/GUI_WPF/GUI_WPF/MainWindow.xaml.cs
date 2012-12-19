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
using System.Windows.Navigation;
using System.Windows.Shapes;
using OSAE.UI.Controls;
using System.Diagnostics;
using System.Threading;

namespace GUI_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Thread.Sleep(2000);
            InitializeComponent();
            InnitiateCommandBindings();
        }

        private void controlsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string s = ((ListBoxItem)controlsListBox.SelectedItem).Name;

            switch (s)
            {
                case "Objects" :
                    ChangeMainPageContent(new ComingSoon());
                    break;
                case "Events":
                    ChangeMainPageContent(new Events());
                    break;                
                case "DBImages":                                    
                    ChangeMainPageContent(new ComingSoon());
                    break;
                case "Logs":
                    ChangeMainPageContent(new Logs());
                    break;
            }
        }

        private void ChangeMainPageContent(UIElement newControl)
        {
            mainDockPanel.Children.Clear();
            mainDockPanel.Children.Add(newControl);
        }

        private void PluginManager_Click(object sender, RoutedEventArgs e)
        {
             Manager_WPF.MainWindow mainWindow = new Manager_WPF.MainWindow();
             mainWindow.ShowDialog();
        }

        private void OpenWiki_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.opensourceautomation.com/wiki/index.php?title=Main_Page");
        }

        private void OpenForum_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.opensourceautomation.com/phpBB3/");
        }
    }
}
