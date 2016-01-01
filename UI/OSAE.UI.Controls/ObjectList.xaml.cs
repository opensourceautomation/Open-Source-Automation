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
    public partial class ObjectList : UserControl
    {
        public Point Location;
        public string _CurrentUser = "";
        public string _AppName = "";
        public string _ScreenLocation = "";
        private OSAEImageManager imgMgr = new OSAEImageManager();
        public ObjectList(string appName)
        {
            InitializeComponent();
            userGrid.Height = 25;
            _AppName = appName;

            //  treeNode = new TreeNode("Linux");
            //  treeView1.Nodes.Add(treeNode);
            //
            // Create two child nodes and put them in an array.
            // ... Add the third node, and specify these as its children.
            //
            //  TreeNode node2 = new TreeNode("C#");
            //  TreeNode node3 = new TreeNode("VB.NET");


            
            tvObjects.Items.Add("House");


        }

  
    }
}