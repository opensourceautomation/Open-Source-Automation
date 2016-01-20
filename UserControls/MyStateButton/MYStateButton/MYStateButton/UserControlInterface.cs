using System;
using System.Data;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSAE;
using PluginInterface;

namespace MYStateButton
{
    public class UserControlInterface : IPlugin
    {
        public UserControlInterface()
        {
            // TODO: Add any constructor logic here. 
            // For most Custom UserControl Plugins, this is Not required.
        }

        //Declarations of all our internal plugin variables
        // Change the following values to match your User Control's Information

        // Change to match your Plugin Name. MUST be in all Uppercase!
        string myName = "MYSTATEBUTTON";

        // Change to a new description for your plugin
        string myDescription = "Custom User Control For State Button";

        // Change to the correct Author of the plugin
        string myAuthor = "Kirk";

        // Change to match your Version of the Plugin. Should Match the OSA version built upon!
        string myVersion = "0.4.8";

        #region DO NOT CHANGE
        IPluginHost myHost = null;
        System.Windows.Controls.UserControl AddCtrlInterface;
        System.Windows.Controls.UserControl MainCtrl;
        public string Description { get { return myDescription; } }
        public string Author { get { return myAuthor; } }
        public IPluginHost Host { get { return myHost; } set { myHost = value; } }
        public string Name { get { return myName; } }
        public System.Windows.Controls.UserControl CtrlInterface { get { return AddCtrlInterface; } }
        public System.Windows.Controls.UserControl mainCtrl { get { return MainCtrl; } }
        public string Version { get { return myVersion; } }
        public void InitializeAddCtrl(string screen, string pluginName,string user, string obj) { AddCtrlInterface = new AddNewControl(screen, myName, user, obj); }
        public void InitializeMainCtrl(OSAEObject obj, string appName, string cuser)
        {
            MainCtrl = new CustomUserControl(obj, myName, appName, cuser);
        }

        public void Dispose()
        {
            //Put any cleanup code in here for when the program is stopped
        }
        #endregion
    }
}
