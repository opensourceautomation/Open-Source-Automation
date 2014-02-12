namespace OSAE.UI.Controls
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
    using System.Windows.Navigation;
    using System.Windows.Shapes;

    /// <summary>
    /// Interaction logic for AddControlBrowser.xaml
    /// </summary>
    public partial class AddControlBrowser : UserControl
    {
        private string currentScreen;
        public AddControlBrowser(string screen)
        {
            InitializeComponent();
            currentScreen = screen;
            txtName.Text = currentScreen + " - Browser";
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {


            OSAEObjectManager.ObjectAdd(txtName.Text, txtName.Text, "CONTROL BROWSER", "", currentScreen, true);

            OSAEObjectPropertyManager.ObjectPropertySet(txtName.Text, "URI", txtURI.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(txtName.Text, "Width", txtWidth.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(txtName.Text, "Height", txtHeight.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(txtName.Text, "ZOrder", "1", "GUI");

            OSAEScreenControlManager.ScreenObjectAdd(currentScreen, "", txtName.Text);

            NotifyParentFinished();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            NotifyParentFinished();
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
    }
}
