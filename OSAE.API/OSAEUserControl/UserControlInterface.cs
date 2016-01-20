using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OSAE;

namespace PluginInterface
{
    using System;
    using System.Windows;

    public interface IPlugin
    {
        IPluginHost Host { get; set; }
        string Name { get; }
        string Description { get; }
        string Author { get; }
        string Version { get; }

        System.Windows.Controls.UserControl CtrlInterface { get; }
        System.Windows.Controls.UserControl mainCtrl { get; }

        void InitializeAddCtrl(string screen, string pluginname, string user, string obj="");
        void InitializeMainCtrl(OSAE.OSAEObject obj, string appName, string cuser);

        void Dispose();
    }

    public interface IPluginHost
    {
        void Feedback(string Feedback, IPlugin Plugin);
    }
}
