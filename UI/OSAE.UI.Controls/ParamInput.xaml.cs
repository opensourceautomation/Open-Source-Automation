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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


    /// <summary>
    /// Interaction logic for ParamInput.xaml
    /// </summary>
    public partial class ParamInput : Window
    {
        string ObjectName;
        string MethodName;
        string Param1;
        string Param2;
        string ctrlType;
        public ParamInput(string ctlType, OSAEObject screenObject)
        {
            InitializeComponent();
            ctrlType = ctlType;
            if (ctrlType == "Method")
            {
                ObjectName = screenObject.Property("Object Name").Value; ;
                MethodName = screenObject.Property("Method Name").Value; ;
                Param1 = screenObject.Property("Param 1").Value; ;
                Param2 = screenObject.Property("Param 2").Value; ;
                if (Param1 == "[ASK]")
                {
                    inputParam1.Visibility = System.Windows.Visibility.Visible;
                    inputParam1Lab.Visibility = System.Windows.Visibility.Visible;
                    inputParam1.Text = Param1;
                }
                else
                {
                    inputParam1.Visibility = System.Windows.Visibility.Hidden;
                    inputParam1Lab.Visibility = System.Windows.Visibility.Hidden;
                }
                if (Param2 == "[ASK]")
                {
                    inputParam2.Visibility = System.Windows.Visibility.Visible;
                    inputParam2Lab.Visibility = System.Windows.Visibility.Visible;
                    inputParam2.Text = Param2;
                }
                else
                {
                    inputParam2.Visibility = System.Windows.Visibility.Hidden;
                    inputParam2Lab.Visibility = System.Windows.Visibility.Hidden;
                }
                BadInputLab.Visibility = System.Windows.Visibility.Visible;
                OKButt.IsEnabled = false;
                if (Param1 == "[ASK]")
                {
                    inputParam1.Focus();
                    inputParam1.SelectAll();
                }
                else
                {
                    inputParam2.Focus();
                    inputParam2.SelectAll();
                }
            }

            // TODO: use this for other parameter input
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ctrlType == "Method")
            {
               if (inputParam1.Text == "[ASK]" | inputParam2.Text == "[ASK]")
                   {
                       // invalid input
                       BadInputLab.Visibility = System.Windows.Visibility.Visible;
                       OKButt.IsEnabled = false;
                   }
                   else
                   {
                   
                   // Input Changed
                   BadInputLab.Visibility = System.Windows.Visibility.Hidden;
                   OKButt.IsEnabled = true; 
                   if (Param1 == "[ASK]")
                   {
                       Param1 = inputParam1.Text;
                   }
                   if (Param2 == "[ASK]")
                   {
                       Param2 = inputParam1.Text;
                   }
                }
                OSAEMethodManager.MethodQueueAdd(ObjectName, MethodName, Param1, Param2, "GUI");
                NotifyParentFinished();
            }
        }

        private void inputParam1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (inputParam1.Text != "[ASK]" && inputParam2.Text != "[ASK]")
            {
                BadInputLab.Visibility = System.Windows.Visibility.Hidden;
                OKButt.IsEnabled = true;
            }
            else
            {
                BadInputLab.Visibility = System.Windows.Visibility.Visible;
                OKButt.IsEnabled = false;
            }
        }

        private void inputParam2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (inputParam2.Text != "[ASK]" && inputParam1.Text != "[ASK]")
            {
                BadInputLab.Visibility = System.Windows.Visibility.Hidden;
                OKButt.IsEnabled = true;
            }
            else
            {
                BadInputLab.Visibility = System.Windows.Visibility.Visible;
                OKButt.IsEnabled = false;
            }
        }

        private void NotifyParentFinished()
        {
            // Get the window hosting us so we can ask it to close
            Window parentWindow = Window.GetWindow(this);
            parentWindow.Close();
        }
    }
}
