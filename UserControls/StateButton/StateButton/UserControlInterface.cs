using OSAE;
using PluginInterface;

namespace StateButton
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
        string myName = "STATEBUTTON";

        // Change to a new description for your plugin
        string myDescription = "Custom User Control For State Button";

        #region DO NOT CHANGE
        IPluginHost myHost = null;
        System.Windows.Controls.UserControl AddCtrlInterface;
        System.Windows.Controls.UserControl MainCtrl;
        public string Description { get { return myDescription; } }
        public string Author { get { return ""; } }
        public IPluginHost Host { get { return myHost; } set { myHost = value; } }
        public string Name { get { return myName; } }
        public System.Windows.Controls.UserControl CtrlInterface { get { return AddCtrlInterface; } }
        public System.Windows.Controls.UserControl mainCtrl { get { return MainCtrl; } }
        public string Version { get { return ""; } }
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
