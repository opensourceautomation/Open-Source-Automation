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
    using OSAE;

    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        OSAE osae = new OSAE("Manager_WPF");

        public Settings()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblDBVersion.Content = osae.GetObjectPropertyValue("SYSTEM", "DB Version").Value;
            txbxZipcode.Text = osae.GetObjectPropertyValue("SYSTEM", "ZIP Code").Value;
            if (osae.GetObjectPropertyValue("SYSTEM", "Debug").Value == "TRUE")
                cbDebugging.SelectedItem = cbDebugging.Items[0];
            else
                cbDebugging.SelectedItem = cbDebugging.Items[1];
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem cbi = (ComboBoxItem)cbDebugging.SelectedItem;
            ObjectPopertiesManager.ObjectPropertySet("SYSTEM", "Debug", cbi.Content.ToString(), "Manager_WPF");
            ObjectPopertiesManager.ObjectPropertySet("SYSTEM", "ZIP Code", txbxZipcode.Text, "Manager_WPF");
            this.Close();
        }
    }
}
