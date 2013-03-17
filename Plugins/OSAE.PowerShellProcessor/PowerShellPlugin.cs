namespace OSAE.PowerShellProcessor
{
    using System;
    using System.Collections.ObjectModel;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;
    using System.Text;

    /// <summary>
    /// Allows OSA to run scripts through PowerShell.
    /// </summary>
    class PowerShellPlugin : OSAEPluginBase
    {
        /// <summary>
        /// Provides access to the OSA logging functionality
        /// </summary>
        Logging logging = Logging.GetLogger("PowerShell Plugin");

        /// <summary>
        /// The plugin name
        /// </summary>
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

        /// <summary>
        /// Runs the content of the script parameter as a powershell script 
        /// </summary>
        /// <param name="script">The script to be run</param>
        private void RunScript(string script)
        {
            Pipeline pipeline = null;
            Runspace runspace = null;

            try
            {
                RunspaceConfiguration runConfig = RunspaceConfiguration.Create();
                
                PSSnapInException psEx = null;
                runConfig.AddPSSnapIn("OSAPS", out psEx);
                runspace = RunspaceFactory.CreateRunspace(runConfig);

                runspace.Open();

                pipeline = runspace.CreatePipeline();
                pipeline.Commands.AddScript(script);
                pipeline.Commands.Add("Out-String");

                Collection<PSObject> results = pipeline.Invoke();

                StringBuilder stringBuilder = new StringBuilder();
                foreach (PSObject obj in results)
                {
                    stringBuilder.AppendLine(obj.ToString());
                }

                logging.AddToLog("Script return: " + stringBuilder.ToString(), false);

            }
            catch (Exception ex)
            {
                logging.AddToLog("An error occured while trying to run the script, details: \r\n" + ex.Message, true);
            }
            finally
            {
                if (pipeline != null)
                {
                    pipeline.Dispose();
                }
                if (runspace != null)
                {
                    runspace.Close();
                    runspace.Dispose();
                }
            }
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
