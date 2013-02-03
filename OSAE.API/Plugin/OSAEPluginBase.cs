namespace OSAE
{
    using System;

    /// <summary>
    /// Defines the interface common to all untrusted plugins.
    /// </summary>
    public abstract class OSAEPluginBase : MarshalByRefObject
    {
        /// <summary>
        /// This method is invoked to allow the plugin to innitialise any required variables 
        /// and innitiate any long running tasks. Not all plugins will need to use this method
        /// however they must implement it.
        /// </summary>
        /// <param name="pluginName">The name of the plugin</param>
        public abstract void RunInterface(string pluginName);

        /// <summary>
        /// A command has been passed to the plugin to be processed.
        /// Not all plugins will support commands but the method must be implemented
        /// </summary>
        /// <param name="method">The information required to process the command</param>
        public abstract void ProcessCommand(OSAEMethod method);

        /// <summary>
        /// When the method on the plugin is invoked it should implement
        /// all that is requierd to shutdown the plugin
        /// </summary>
        public abstract void Shutdown();

        public override Object InitializeLifetimeService()
        {            
            return null;
        }
    }

}