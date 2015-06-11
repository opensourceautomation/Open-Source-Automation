
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
    /// Interaction logic for AddControlPropertyLabel.xaml
    /// </summary>
    public partial class AddControlPropertyLabel : UserControl
    {
        private string currentScreen;
        string sOriginalName = "";
        string sWorkingName = "";
        string sMode = "";

        public AddControlPropertyLabel(string screen, string controlName = "")
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
                    txtControlName.Text = controlName;
                    LoadCurrentScreenObject(controlName);
                }
            }

            if (controlName == "")
            {
                //Let's create a new name
                sWorkingName = currentScreen + " - New State Image";
                DataSet dsScreenControl = OSAESql.RunSQL("SELECT COUNT(object_name) FROM osae_v_object where object_name = '" + sWorkingName + "'");
                int iCount = 0;

                while (dsScreenControl.Tables[0].Rows[0][0].ToString() == "1")
                {
                    // We have a duplicate name, we must get a unique name
                    iCount += 1;
                    sWorkingName = currentScreen + " - New State Image" + iCount;
                    dsScreenControl = OSAESql.RunSQL("SELECT COUNT(object_name) FROM osae_v_object where object_name = '" + sWorkingName + "'");
                }
                sMode = "Add";
                controlName = sWorkingName;
                txtControlName.Text = controlName;

                LoadCurrentScreenObject(controlName);
            }
            Enable_Buttons();



        }

        /// <summary>
        /// Load the objects from the DB into the combo box
        /// </summary>
        private void LoadCurrentScreenObject(string controlName)
        {
            OSAEImageManager imgMgr = new OSAEImageManager();

            cboObject.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Object Name").Value;
            cboProperty.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Property Name").Value;

            txtFont.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Font Name").Value;
            txtSize.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Font Size").Value;
            cboForeColor.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Fore Color").Value;
            cboBackColor.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Back Color").Value;
            txtPrefix.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Prefix").Value;
            txtSuffix.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Suffix").Value;



            txtX.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "X").Value;
            txtY.Text = OSAEObjectPropertyManager.GetObjectPropertyValue(controlName, "Y").Value;

         //   OSAEObjectPropertyManager.ObjectPropertySet(sName, "Zorder", "1", "GUI");
        //    OSAEScreenControlManager.ScreenObjectAdd(currentScreen, cboObject.Text, sName);
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
            if (sMode == "Update" && sOriginalName == txtControlName.Text)
            {
                btnAdd.IsEnabled = false;
                btnUpdate.IsEnabled = true;
                btnDelete.IsEnabled = true;
            }
            //Now we handle Updates WITH name changes
            if (sMode == "Update" && sOriginalName != txtControlName.Text)
            {
                btnAdd.IsEnabled = true;
                btnUpdate.IsEnabled = true;
                btnDelete.IsEnabled = true;
            }
        }




        /// <summary>
        /// Load the screens from the DB into the combo box
        /// </summary>
        private void LoadObjects()
        {
            DataSet dataSet = OSAESql.RunSQL("SELECT object_name FROM osae_v_object order by object_name");
            cboObject.ItemsSource = dataSet.Tables[0].DefaultView;
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
                    cboForeColor.Items.Add(colorName);
                    cboBackColor.Items.Add(colorName);
                //}
            }
            //return the color list
            cboForeColor.Text = "Black";
            cboBackColor.Text = "White";

        }

        private void objectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataSet dataSet = OSAESql.RunSQL("select property_name from osae_v_object_property where object_name='" + (sender as ComboBox).SelectedValue.ToString() + "' Union select 'State' order by property_name");
            cboProperty.ItemsSource = dataSet.Tables[0].DefaultView;
        }

        private void txtFont_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                System.Windows.Forms.FontDialog dlgFont = null;
                dlgFont = new System.Windows.Forms.FontDialog();

                //dlgFont.Font = set your font here

                if (dlgFont.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtFont.Text = dlgFont.Font.FontFamily.Name;
                    txtSize.Text = dlgFont.Font.Size.ToString();
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            string sName = txtControlName.Text;
            OSAEObjectManager.ObjectAdd(sName, sName, sName, "CONTROL PROPERTY LABEL", "", currentScreen, true);
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Font Name", txtFont.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Font Size", txtSize.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Fore Color", cboForeColor.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Back Color", cboBackColor.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Object Name", cboObject.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Property Name", cboProperty.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Prefix", txtPrefix.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Suffix", txtSuffix.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "X", txtX.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Y", txtY.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Zorder", "1", "GUI");

            OSAEScreenControlManager.ScreenObjectAdd(currentScreen, cboObject.Text, sName);


            NotifyParentFinished();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            string sName = txtControlName.Text;
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Font Name", txtFont.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Font Size", txtSize.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Fore Color", cboForeColor.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Back Color", cboBackColor.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Object Name", cboObject.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Property Name", cboProperty.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Prefix", txtPrefix.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Suffix", txtSuffix.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "X", txtX.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Y", txtY.Text, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(sName, "Zorder", "1", "GUI");

            NotifyParentFinished();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            OSAEObjectManager.ObjectDelete(sOriginalName);
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
