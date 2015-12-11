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

namespace OSAE.UI.Controls
{
    /// <summary>
    /// Interaction logic for NavigationImage.xaml
    /// </summary>
    public partial class UserSelector : UserControl
    {
        public Point Location;
        public string _CurrentUser = "";
        private OSAEImageManager imgMgr = new OSAEImageManager();
        private string _pwbuff = "";
        private string _usersPIN = "";

        public UserSelector()
        {
            InitializeComponent();
            cboUsers.Items.Add("Log In/Out");

            OSAEObjectCollection userList = OSAEObjectManager.GetObjectsByType("PERSON");
            foreach (OSAE.OSAEObject obj in userList)
            {
                cboUsers.Items.Add (obj.Name);
            }
            cboUsers.SelectedIndex = 1;
            cboUsers.Background = new SolidColorBrush(Colors.Yellow);

            // string imgName = screenObject.Property("Image").Value;
            // OSAEImage img = imgMgr.GetImage(imgName);

            //  if (img != null)
            // DataSet dataSet = OSAESql.RunSQL("SELECT state_name FROM osae_v_object_state where object_name = '" + cboObject.SelectedValue + "' order by state_name");
            //cboState1.ItemsSource = dataSet.Tables[0].DefaultView;

        }

        private void cboUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _CurrentUser = "";
            _pwbuff = "";
            _usersPIN = "";
            lblPIN.Content = "";
            if (cboUsers.SelectedIndex == 1)
            {
                userGrid.Height = 25;
                cboUsers.Background = new SolidColorBrush(Colors.Yellow);
            }
            else
            {
                //Load user's PIN
                _usersPIN = OSAE.OSAEObjectPropertyManager.GetObjectPropertyValue(cboUsers.SelectedItem.ToString(), "PIN").Value;
                if (_usersPIN == "")
                {
                    userGrid.Height = 25;
                    cboUsers.Background = new SolidColorBrush(Colors.Green);
                    _CurrentUser = cboUsers.SelectedItem.ToString();
                }
                else
                    userGrid.Height = 184;
            }
        }

        private void Number_Pressed(string _number)
        {
            _pwbuff += _number;
            if (_pwbuff == _usersPIN)
            {
                userGrid.Height = 25;
                cboUsers.Background = new SolidColorBrush(Colors.Green);
                _CurrentUser = cboUsers.SelectedItem.ToString();
            }
            lblPIN.Content  += "* ";
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Number_Pressed("1");
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Number_Pressed("2");
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            Number_Pressed("3");
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            Number_Pressed("4");
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            Number_Pressed("5");
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            Number_Pressed("6");
        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            Number_Pressed("7");
        }

        private void button8_Click(object sender, RoutedEventArgs e)
        {
            Number_Pressed("8");
        }

        private void button9_Click(object sender, RoutedEventArgs e)
        {
            Number_Pressed("9");
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            _pwbuff = "";
            lblPIN.Content = "";
        }

        private void button0_Click(object sender, RoutedEventArgs e)
        {
            Number_Pressed("0");
        }

        private void buttonGo_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}