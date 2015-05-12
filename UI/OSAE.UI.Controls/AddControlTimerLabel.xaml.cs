namespace OSAE.UI.Controls
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Data;
    using System.Windows;
    using System.Windows.Controls;
    using System.Collections.Generic;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Interaction logic for AddControlStaticLabel.xaml
    /// </summary>
    public partial class AddControlTimerLabel : UserControl
    {
        private string currentScreen;
        string sOriginalName = "";
        string sOriginalObject = "";
        string sMode = "";

        public AddControlTimerLabel(string screen, string controlName = "")
        {
            InitializeComponent();
            currentScreen = screen;
            LoadObjects();
            LoadColors();

            //Check if controlName was passed in, if so, goto edit mode
            if (controlName != "")
            {
                //Let's validate the controlName and then call a Pre-Load of its properties
                DataSet dsScreenControl = OSAESql.RunSQL("SELECT COUNT(object_name) FROM osae_v_object where object_name = '" + controlName + "'");
                if (dsScreenControl.Tables[0].Rows[0][0].ToString() == "1")
                {
                    // We have a hit, this is an Update call, se call the preload
                    sMode = "Update";
                    sOriginalName = controlName;
                    lblName.Content = controlName;
                    LoadCurrentScreenObject(controlName);
                }
            }
            else   
            {
                sMode = "Add";
            }
            Enable_Buttons();
        }

        /// <summary>
        /// Load the screens from the DB into the combo box
        /// </summary>
        private void LoadObjects()
        {
            DataSet dataSet = OSAESql.RunSQL("SELECT object_name FROM osae_v_object order by object_name");
            objectComboBox.ItemsSource = dataSet.Tables[0].DefaultView;
        }

        private void LoadColors()
        {
            //create a generic list of strings
            //Dim colors As New List(Of String)()
            //get the color names from the Known color enum
            string[] colorNames = Enum.GetNames(typeof(KnownColor));
            //iterate thru each string in the colorNames array
            foreach (string colorName in colorNames)
            {
                //cast the colorName into a KnownColor
                //KnownColor. knownColor;// = (KnownColor)Enum.Parse(typeof(KnownColor), colorName);
                //check if the knownColor variable is a System color
                //if (knownColor > knownColor.)
                //{
                //add it to our list
                foreColorComboBox.Items.Add(colorName);
                backColorComboBox.Items.Add(colorName);
                //}
            }
            //return the color list
            foreColorComboBox.Text = "Black";
            backColorComboBox.Text = "White";
            txtFont.Text = "Arial";
            txtSize.Text = "8.5";
        }

        private void txtFont_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                System.Windows.Forms.FontDialog dlgFont = null;
                dlgFont = new System.Windows.Forms.FontDialog();

                if (dlgFont.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtFont.Text = dlgFont.Font.FontFamily.Name;
                    txtFont.FontFamily = new System.Windows.Media.FontFamily(dlgFont.Font.FontFamily.Name);
                    txtSize.Text = dlgFont.Font.Size.ToString();
                }
            }
            catch
            {
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            string sName = lblName.Content.ToString();
            OSAEObjectManager.ObjectAdd(sName, sName, "CONTROL TIMER LABEL", "", currentScreen, true);
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Font Name", txtFont.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Font Size", txtSize.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Fore Color", foreColorComboBox.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Back Color", backColorComboBox.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Object Name", objectComboBox.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "X", txtX.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Y", txtY.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Zorder", "1", "GUI");

            OSAEScreenControlManager.ScreenObjectAdd(currentScreen, objectComboBox.Text, sName);
            NotifyParentFinished();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            string sName = lblName.Content.ToString();
            OSAEObjectManager.ObjectUpdate(sOriginalName, sName, sName, "CONTROL TIMER LABEL", "", currentScreen, 1);
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Font Name", txtFont.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Font Size", txtSize.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Fore Color", foreColorComboBox.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Back Color", backColorComboBox.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Object Name", objectComboBox.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "X", txtX.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Y", txtY.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Zorder", "1", "GUI");
            if ((sOriginalObject != objectComboBox.Text) && (sOriginalName != sName))
            {
                OSAEScreenControlManager.ScreenObjectUpdate(currentScreen, objectComboBox.Text, sName);
            }
            NotifyParentFinished();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            OSAEObjectManager.ObjectDelete(sOriginalName);
            NotifyParentFinished();
        }

        private void objectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lblName.Content = "Screen - " + currentScreen.Replace("Screen - ","") + " - " + (sender as ComboBox).SelectedValue.ToString() + " (Off Timer)";
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

        private void LoadCurrentScreenObject(string controlName)
        {
            objectComboBox.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Object Name").Value;
            sOriginalObject = objectComboBox.Text;
            txtFont.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Font Name").Value;
            txtSize.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Font Size").Value;
            foreColorComboBox.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Fore Color").Value;
            backColorComboBox.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Back Color").Value;
            txtX.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "X").Value;
            txtY.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Y").Value;
        }

        private void Enable_Buttons()
        {
            //First Senerio is a New Control, not a rename or update.
            if (sMode == "Add")
            {
                btnAdd.IsEnabled = true;
                btnUpdate.IsEnabled = false;
                btnDelete.IsEnabled = false;
            }
            //Now we handle Updates with no name changes
            if (sMode == "Update" && sOriginalObject == objectComboBox.Text)
            {
                btnAdd.IsEnabled = false;
                btnUpdate.IsEnabled = true;
                btnDelete.IsEnabled = true;
            }
            //Now we handle Updates WITH name changes
            if (sMode == "Update" && sOriginalObject != objectComboBox.Text)
            {
                btnAdd.IsEnabled = true;
                btnUpdate.IsEnabled = true;
                btnDelete.IsEnabled = true;
            }
        }
    }
}
