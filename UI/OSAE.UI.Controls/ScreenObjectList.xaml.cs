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
using System.IO;
using OSAE;
using System.Data;

namespace OSAE.UI.Controls
{
    /// <summary>
    /// Interaction logic for NavigationImage.xaml
    /// </summary>
    public partial class ScreenObjectList : UserControl
    {
        public Point Location;
        public string currentUser = "";
        public string _AppName = "";
        public string _ScreenLocation = "";
        public string currentProperty = "";
        public string currentPropertyType = "";
        public string currentScreen = "";
        public ScreenObjectList(string screen,string user)
        {
            InitializeComponent();
            currentUser = user;
            currentScreen = screen;
            OSAEObjectCollection _objs = OSAEObjectManager.GetObjectsByContainer(currentScreen);
            foreach (OSAEObject _obj in _objs)
            {
                lbControls.Items.Add(_obj.Name);
            }
        }

        private void lbControls_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataSet dsControls = OSAESql.RunSQL("SELECT property_name, property_value, property_datatype, object_property_id, DATE_FORMAT(last_updated,'%m/%d %h:%i:%s %p') as last_updated,source_name, trust_level,interest_level,property_object_type FROM osae_v_object_property where object_name='" + lbControls.SelectedValue.ToString() + "' ORDER BY property_name");
            dgProperties.ItemsSource = dsControls.Tables[0].DefaultView;
            // dgProperties.SelectedIndex = 0;

        }

        private void dgProperties_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
            if (row != null)
            {

                dataGrid.Columns[2].Visibility = System.Windows.Visibility.Visible;
                DataGridCell RowColumn = dataGrid.Columns[1].GetCellContent(row).Parent as DataGridCell;
                txtValue.Text = ((TextBlock)RowColumn.Content).Text;

                RowColumn = dataGrid.Columns[0].GetCellContent(row).Parent as DataGridCell;
                currentProperty = ((TextBlock)RowColumn.Content).Text;

                RowColumn = dataGrid.Columns[2].GetCellContent(row).Parent as DataGridCell;
                currentPropertyType = ((TextBlock)RowColumn.Content).Text;
                if (currentPropertyType.ToUpper() == "BOOLEAN")
                {
                    ddlPropValue.Visibility = System.Windows.Visibility.Visible;
                    txtValue.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    ddlPropValue.Visibility = System.Windows.Visibility.Hidden;
                    txtValue.Visibility = System.Windows.Visibility.Visible;
                }          
            }
            else
            {
                txtValue.Text = "";
                currentProperty = "";
                currentPropertyType = "";
            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string value = "";
            if (currentPropertyType.ToUpper() == "BOOLEAN")
                value = ddlPropValue.SelectedValue.ToString();
            else
            {
                if (ddlPropValue.IsVisible)
                    value = ddlPropValue.SelectedValue.ToString();
                else
                    value = txtValue.Text;
            }
            OSAEObjectPropertyManager.ObjectPropertySet(lbControls.SelectedValue.ToString(), currentProperty, value, currentUser);
            DataSet dsControls = OSAESql.RunSQL("SELECT property_name, property_value, property_datatype, object_property_id, DATE_FORMAT(last_updated,'%m/%d %h:%i:%s %p') as last_updated,source_name, trust_level,interest_level,property_object_type FROM osae_v_object_property where object_name='" + lbControls.SelectedValue.ToString() + "' ORDER BY property_name");
            dgProperties.ItemsSource = dsControls.Tables[0].DefaultView;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}