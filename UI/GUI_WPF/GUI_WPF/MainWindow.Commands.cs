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
    public partial class MainWindow
    {
        RoutedCommand eventsControl = new RoutedCommand();

        public void InnitiateCommandBindings()
        {
            CommandBinding cb = new CommandBinding(eventsControl, EventsControl);
            this.CommandBindings.Add(cb); 
        }

        private void EventsControl(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Hey, I'm some help.");
        } 
    }
}
