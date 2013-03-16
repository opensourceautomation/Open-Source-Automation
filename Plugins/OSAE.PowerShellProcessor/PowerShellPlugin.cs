namespace OSAE.PowerShellProcessor
{
    using System;
    using System.Data;
    using System.Management.Automation.Runspaces;

    class PowerShellPlugin : OSAEPluginBase
    {
        Logging logging = Logging.GetLogger("PowerShell Plugin");
        string pName;

        /// <summary>
        /// A Command to be processed bu the plugin
        /// </summary>
        /// <param name="method"></param>
        public override void ProcessCommand(OSAEMethod method)
        {
            string script = OSAEScriptManager.GetScript(method.Parameter1);

            logging.AddToLog("running script: " + script, false);
 
            RunScript(script);            
        }

        private void RunScript(string script)
        {
            Runspace runspace = RunspaceFactory.CreateRunspace();

            runspace.Open();

            Pipeline pipeline = runspace.CreatePipeline();
            pipeline.Commands.AddScript(script);

            pipeline.Commands.Add("Out-String");

            pipeline.Invoke();

            runspace.Close();   
        }

        /// <summary>
        /// Set up the powershell plugin ready to process commands
        /// </summary>
        /// <param name="pluginName"></param>
        public override void RunInterface(string pluginName)
        {
            logging.AddToLog("Running Interface!", true);
            pName = pluginName;
        }

        /// <summary>
        /// Shutsdown the Powershell Plugin
        /// </summary>
        public override void Shutdown()
        {
        }
    }
}
