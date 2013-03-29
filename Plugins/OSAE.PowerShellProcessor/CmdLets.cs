namespace OSAE.PowerShellTools
{
    using System;  // Windows PowerShell assembly.
    using System.Collections.ObjectModel;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;
    using System.Text;

    [Cmdlet(VerbsCommon.Get, "OSA")]
    public class OSAPS : Cmdlet
    {
        /// <summary>
        /// Provides access to the OSA logging class
        /// </summary>
        private Logging logging = Logging.GetLogger("PowerShell");

        [Parameter(Mandatory = true)]
        public string Name { get; set; }
               
        protected override void ProcessRecord()
        {
            logging.AddToLog("Get-OSA - ProcessRecord - Started", false);
            OSAEObject obj = OSAEObjectManager.GetObjectByName(Name);
            WriteObject(obj);
        }
    }

    [Cmdlet(VerbsCommon.Set, "OSA")]
    public class OSAPSSet : Cmdlet
    {
        /// <summary>
        /// Provides access to the OSA logging class
        /// </summary>
        private Logging logging = Logging.GetLogger("PowerShell");

        /// <summary>
        /// Used in logs to determine where an event occured from or where log messages should go to
        /// </summary>
        private const string source = "PowerShell CmdLet";

        [Parameter(Mandatory = true)]
        public string Name { get; set; }

        [Parameter(Mandatory = true)]
        public string Property { get; set; }

        [Parameter(Mandatory = true)]
        public string Value { get; set; }
                
        protected override void ProcessRecord()
        {
            logging.AddToLog("Set-OSA - ProcessRecord - Started", false);
            OSAEObject obj = OSAEObjectManager.GetObjectByName(Name);
            OSAEObjectPropertyManager.ObjectPropertySet(Name, Property, Value, source);

            WriteObject(true);
        }
    }

    [Cmdlet(VerbsLifecycle.Invoke, "OSA")]
    public class OSAPSInvoke : Cmdlet
    {
        /// <summary>
        /// Provides access to the OSA logging class
        /// </summary>
        private Logging logging = Logging.GetLogger("PowerShell");

        [Parameter(Mandatory = true)]
        public string Name { get; set; }

        [Parameter(Mandatory = true)]
        public string Method { get; set; }

        private string parameter1 = string.Empty;
        
        [Parameter(Mandatory = false)]
        public string Parameter1 
        {
            get
            {
                return parameter1;
            }
            set
            {
                parameter1 = value;
            }
        }

        private string parameter2 = string.Empty;

        [Parameter(Mandatory = false)]
        public string Parameter2
        {
            get
            {
                return parameter2;
            }
            set
            {
                parameter2 = value;
            }
        }
               
        protected override void ProcessRecord()
        {
            logging.AddToLog("Invoke-OSA - ProcessRecord - Started", false);
            OSAEMethodManager.MethodQueueAdd(Name, Method, parameter1, parameter2, "PowerShell");

            WriteObject(true);
        }
    }

    [Cmdlet(VerbsCommon.Show, "OSA")]
    public class OSAPSShow : Cmdlet
    {
        /// <summary>
        /// Provides access to the OSA logging class
        /// </summary>
        private Logging logging = Logging.GetLogger("PowerShell");

        [Parameter(Mandatory = true)]
        public string Name { get; set; }
        
        protected override void ProcessRecord()
        {
            logging.AddToLog("Show-OSA - ProcessRecord - Started", false);
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
        /// <summary>
        /// Provides access to the OSA logging class
        /// </summary>
        private Logging logging = Logging.GetLogger("PowerShell");

        [Parameter(Mandatory = true)]
        public string Name { get; set; }

        [Parameter(Mandatory = false)]
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
            }
        }
    }
}
