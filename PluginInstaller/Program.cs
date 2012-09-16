using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace PluginInstaller
{
    static class Program
    {
        /// <summary> 
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string filename;

            if (args != null && args.Length == 1)
            {
                if (File.Exists(Path.GetFullPath(args[0])))
                {
                    filename = Path.GetFileName(Path.GetFullPath(args[0]));


                    if (filename.EndsWith("osapp", StringComparison.Ordinal))
                    {
                        // its a plugin package
                        PluginInstallerHelper.InstallPlugin(Path.GetFullPath(args[0]));
                    }
                }
            }
            else
            {
                Application.Run(new PluginInstaller(args));
            }
            
        }
    }
}
