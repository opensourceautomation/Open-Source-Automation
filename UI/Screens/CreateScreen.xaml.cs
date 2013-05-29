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

namespace GUI2
{
    /// <summary>
    /// Interaction logic for CreateScreen.xaml
    /// </summary>
    public partial class CreateScreen : Window
    {
        private MainWindow m_mainwindow = null;

        public CreateScreen(MainWindow parent)
        {
            InitializeComponent();
            m_mainwindow = parent;

            uc_Child.LoadScreen += new EventHandler(Load_Screen);
        }

        protected void Load_Screen(object sender, EventArgs e)
        {
            m_mainwindow.Load_Screen((string)sender);
        }
    }
}
