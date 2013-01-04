using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OSAE
{
    /// <summary>
    /// Defines the interface common to all untrusted plugins.
    /// </summary>
    public abstract class OSAEPluginBase : MarshalByRefObject
    {
        public abstract void RunInterface(string pluginName);

        public abstract void ProcessCommand(OSAEMethod method);

        public abstract void Shutdown();
        
    }

}