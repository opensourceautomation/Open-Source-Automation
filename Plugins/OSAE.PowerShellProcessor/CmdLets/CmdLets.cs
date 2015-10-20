namespace OSAE.PowerShellTools
{
    using System;  // Windows PowerShell assembly.
    using System.Collections.ObjectModel;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;
    using System.Text;    

    [Cmdlet(VerbsCommon.Set, "OSA")]
    public class OSAPSSet : Cmdlet
    {
        //OSAELog
        private OSAE.General.OSAELog Log = new General.OSAELog();

        /// <summary>
        /// Used in logs to determine where an event occured from or where log messages should go to
        /// </summary>
        private const string source = "PowerShell CmdLet";

        [Parameter(
            Mandatory = true,
            HelpMessage = "The name of the object that has the property to set")]
        public string Name { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The name of the property to set")]
        [Alias("Prop")]
        public string Property { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The value to set the property to")]
        public string Value { get; set; }
                
        protected override void ProcessRecord()
        {
            this.Log.Debug("Set-OSA - ProcessRecord - Started");
            OSAEObject obj = OSAEObjectManager.GetObjectByName(Name);
            OSAEObjectPropertyManager.ObjectPropertySet(Name, Property, Value, source);

            WriteObject(true);
        }
    }    

    [Cmdlet(VerbsCommon.Show, "OSA")]
    public class OSAPSShow : Cmdlet
    {
        //OSAELog
        private OSAE.General.OSAELog Log = new General.OSAELog();

        [Parameter(Mandatory = true)]
        public string Name { get; set; }
        
        protected override void ProcessRecord()
        {
            this.Log.Debug("Show-OSA - ProcessRecord - Started");
            OSAEObject obj = OSAEObjectManager.GetObjectByName(Name);

            WriteObject("Name: " + obj.Name);
            WriteObject("State: " + obj.State.Value);
            WriteObject("Description: " + obj.Description);
            WriteObject("Container: " + obj.Container);
            WriteObject("Address: " + obj.Address);

            if (obj.Enabled == 0)
            {
                WriteObject("Enabled: FALSE");
            }
            else
            {
                WriteObject("Enabled: TRUE");
            }

            WriteObject("Base Type: " + obj.BaseType);
            WriteObject("Type: " + obj.Type);

            foreach (OSAEMethod method in obj.Methods)
            {
                WriteObject("Method: " + method.MethodName);
            }

            foreach (OSAEObjectProperty prop in obj.Properties)
            {
                WriteObject("Property (" + prop.DataType + "): " + prop.Name + " Value = " + prop.Value);
            }

            WriteObject("Updated: " + obj.LastUpd);
        }
    }

    [Cmdlet(VerbsLifecycle.Invoke, "OSAScript")]
    public class OSAPSInvokeScript : Cmdlet
    {
        //OSAELog
        private OSAE.General.OSAELog Log = new General.OSAELog();

        [Parameter(Mandatory = true)]
        public string Name { get; set; }

        [Parameter(Mandatory = false)]
        [Alias("P2")]
        public string Parameter2 { get; set; }

        [Parameter(Mandatory = false)]
        public string Nested { get; set; }

        protected override void ProcessRecord()
        {
            if (!string.IsNullOrEmpty(Nested) & Nested == "TRUE")
            {
                RunNested();
            }
            else
            {
                RunNormal();
            }           
        }

        private void RunNormal()
        {
            Pipeline pipeline = null;
            Runspace runspace = null;

            try
            {
                RunspaceConfiguration runConfig = RunspaceConfiguration.Create();

                PSSnapInException psEx = null;
                string script = OSAEScriptManager.GetScriptByName(Name);
                runConfig.AddPSSnapIn("OSA", out psEx);
                runspace = RunspaceFactory.CreateRunspace(runConfig);
                runspace.ThreadOptions = PSThreadOptions.UseCurrentThread;

                runspace.Open();

                runspace.SessionStateProxy.SetVariable("parameter2", Parameter2);

                pipeline = runspace.CreatePipeline();
                pipeline.Commands.AddScript(script);
                pipeline.Commands.Add("Out-String");

                Collection<PSObject> results = pipeline.Invoke();

                StringBuilder stringBuilder = new StringBuilder();
                foreach (PSObject obj in results)
                {
                    stringBuilder.AppendLine(obj.ToString());
                }

                this.Log.Debug("Script return: \r\n" + stringBuilder.ToString());

            }
            catch (Exception ex)
            {
                this.Log.Error("An error occured while trying to run the script, details", ex);
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

        private void RunNested()
        {
            Pipeline pipeline = null;

            try
            {
                pipeline = Runspace.DefaultRunspace.CreateNestedPipeline();
                string script = OSAEScriptManager.GetScriptByName(Name);
                Command psCommand = new Command(script, true);               
                pipeline.Commands.Add(psCommand);

                pipeline.Commands.Add("Out-String");

                Collection<PSObject> results = pipeline.Invoke();

                StringBuilder stringBuilder = new StringBuilder();
                foreach (PSObject obj in results)
                {
                    stringBuilder.AppendLine(obj.ToString());
                }

                this.Log.Debug("Script return: \r\n" + stringBuilder.ToString());
            }
            catch (Exception ex)
            {
                this.Log.Error("An error occured while trying to run the script, details", ex);
            }
            finally
            {
                if (pipeline != null)
                {
                    pipeline.Dispose();
                }
            }
        }
    }
}
