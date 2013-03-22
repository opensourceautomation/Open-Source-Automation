namespace OSAE.PowerShellProcessor
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Configuration.Install;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;
    using System.Text;

    /// <summary>
    /// Allows OSA to run scripts through PowerShell.
    /// </summary>
    public class PowerShellPlugin : OSAEPluginBase
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
            try
            {
                string script = OSAEScriptManager.GetScript(method.Parameter1);

                logging.AddToLog("running script: " + script, false);

                RunScript(script);
            }
            catch (Exception exc)
            {
                logging.AddToLog("Error Processing Command: " + exc.Message, true);
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

            if (PluginRegistered())
            {
                logging.AddToLog("Powershell Plugin already registered", false);
            }
            else
            {
                logging.AddToLog("Powershell Plugin needs registering", false);
                Register(false);
            }

        }

        private bool PluginRegistered()
        {
            // load PowerShell
            Runspace runSpace;

            var rsConfig = RunspaceConfiguration.Create();
            runSpace = RunspaceFactory.CreateRunspace(rsConfig);
            runSpace.Open();

            using (var ps = PowerShell.Create())
            {
                ps.Runspace = runSpace;
                ps.AddCommand("Get-PSSnapin");
                ps.AddParameter("Registered");
                ps.AddParameter("Name", "OSA");
                var result = ps.Invoke();
                if (result.Count == 0)
                {
                    return false;                    
                }
                else
                {
                    return true;                    
                }
            }
        }

        /// <summary>
        /// Shutsdown the Powershell Plugin
        /// </summary>
        public override void Shutdown()
        {
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
                
                runConfig.AddPSSnapIn("OSA", out psEx);
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

                logging.AddToLog("Script return: \r\n" + stringBuilder.ToString(), false);

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

        private void Register(bool undo)
        {
            logging.AddToLog("Registering Poweshell Plugin", false);
            var core = Common.ApiPath + @"\Plugins\PowerShell\OSAE.PowerShellProcessor.dll";
            using (var install = new AssemblyInstaller(core, null))
            {
                
                IDictionary state = new Hashtable();
                install.UseNewContext = true;
                try
                {
                    if (undo)
                    {
                        install.Uninstall(state);
                    }
                    else
                    {
                        install.Install(state);
                        install.Commit(state);
                    }
                }
                catch
                {
                    install.Rollback(state);
                }
            }

            if (PluginRegistered())
            {
                logging.AddToLog("Powershell Plugin successfully registered", false);
            }
            else
            {
                logging.AddToLog("Powershell Plugin failed to register", false);
            }
        }        
    }
}
