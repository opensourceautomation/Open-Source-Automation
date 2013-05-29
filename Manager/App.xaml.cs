using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;


namespace Manager_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        
        #region ISingleInstanceApp Members

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            MainWindow.WindowState = WindowState.Normal;
            MainWindow.Activate();

            string filename;

            if (args != null && args.Count == 2)
            {
                if (System.IO.File.Exists(System.IO.Path.GetFullPath(args[1])))
                {
                    filename = System.IO.Path.GetFileName(System.IO.Path.GetFullPath(args[1]));


                    if (filename.EndsWith("osapp", StringComparison.Ordinal))
                    {
                        // its a plugin package
                        PluginInstallerHelper pInst = new PluginInstallerHelper();
                        pInst.InstallPlugin(System.IO.Path.GetFullPath(args[1]));
                    }
                }
            }

            return true;
        }

        #endregion
    }
}
