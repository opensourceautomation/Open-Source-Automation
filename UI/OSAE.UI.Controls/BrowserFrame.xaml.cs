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

namespace OSAE.UI.Controls
{
    /// <summary>
    /// Interaction logic for BrowserFrame.xaml
    /// </summary>
    public partial class BrowserFrame : UserControl
    {
        public OSAEObject screenObject { get; set; }
        public Point Location;
        
        public string StateMatch;
        public string CurState;

        public string ObjectName;

        public BrowserFrame(OSAEObject sObject)
        {
            InitializeComponent();
                     screenObject = sObject;
            try
            {

                Uri uriproperty = new Uri(screenObject.Property("URI").Value);
                wbBrowser.Source = uriproperty;
            }
            catch (Exception ex)
            {
               
            }
        }
    }
}
