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
        //OSAELog
        private OSAE.General.OSAELog Log = new General.OSAELog();

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
                string script = "";

                int scriptId;
                if (int.TryParse(method.Parameter1, out scriptId))
                {
                    script = OSAEScriptManager.GetScript(method.Parameter1);
                }
                else
                {
                    script = OSAEScriptManager.GetScriptByName(method.Parameter1);                    
                }                

                this.Log.Debug("running script: " + script);

                if(!string.IsNullOrEmpty(script))
                {                    
                    RunScript(script, method);
                }
            }
            catch (Exception exc)
            {
                this.Log.Error("Error Processing Command ", exc);
            }
        }

        /// <summary>
        /// Set up the powershell plugin ready to process commands
        /// </summary>
        /// <param name="pluginName"></param>
        public override void RunInterface(string pluginName)
        {
            this.Log.Info("Running Interface!");
            pName = pluginName;

            if (PluginRegistered())
            {
                this.Log.Debug("Powershell Plugin already registered");
            }
            else
            {
                this.Log.Debug("Powershell Plugin needs registering");
                Register(false);
            }
            OwnTypes();
        }

        public void OwnTypes()
        {
            //Added the follow to automatically own Speech Base types that have no owner.
            OSAEObjectType oType = OSAEObjectTypeManager.ObjectTypeLoad("POWERSHELL");

            if (oType.OwnedBy == "")
            {
                OSAEObjectTypeManager.ObjectTypeUpdate(oType.Name, oType.Name, oType.Description, pName, oType.BaseType, oType.Owner, oType.SysType, oType.Container, oType.HideRedundant);
                Log.Info("Powershell Plugin took ownership of the POWERSHELL Object Type.");
            }
            else
            {
                Log.Info("Powershell Plugin correctly owns the POWERSHELL Object Type.");
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
        private void RunScript(string script, OSAEMethod method)
        {
            Pipeline pipeline = null;
            Runspace runspace = null;

            try
            {
                RunspaceConfiguration runConfig = RunspaceConfiguration.Create();                
                
                PSSnapInException psEx = null;
                
                runConfig.AddPSSnapIn("OSA", out psEx);                
                runspace = RunspaceFactory.CreateRunspace(runConfig);
                runspace.ThreadOptions = PSThreadOptions.UseCurrentThread;
               
                runspace.Open();
                
                runspace.SessionStateProxy.SetVariable("parameter2", method.Parameter2);
                
                pipeline = runspace.CreatePipeline();
                pipeline.Commands.AddScript(script);
                pipeline.Commands.Add("Out-String");
          
                Collection<PSObject> results = pipeline.Invoke();

                StringBuilder stringBuilder = new StringBuilder();
                foreach (PSObject obj in results)
                {
                    stringBuilder.AppendLine(obj.ToString());
                }

                this.Log.Debug("Script return: " + stringBuilder.ToString());

            }
            catch (Exception ex)
            {
                this.Log.Error("An error occured while trying to run the script, details",  ex);
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
            this.Log.Debug("Registering Poweshell Plugin");
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
                this.Log.Debug("Powershell Plugin successfully registered");
            }
            else
            {
                this.Log.Debug("Powershell Plugin failed to register");
            }
        }        
    }
}
