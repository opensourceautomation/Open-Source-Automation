namespace OSAE.PowerShellTools
{
    using System.Management.Automation;  // Windows PowerShell assembly.

    [Cmdlet(VerbsCommon.Get, "OSA")]
    public class OSAPS : Cmdlet
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

        [Parameter(Mandatory = false)]
        public string Property { get; set; }
      
        protected override void ProcessRecord()
        {
            logging.AddToLog("Get-OSA - ProcessRecord - Started", false);
            OSAEObject obj = OSAEObjectManager.GetObjectByName(Name);

            if (Property != string.Empty)
            {
                OSAEObjectProperty objProp = OSAEObjectPropertyManager.GetObjectPropertyValue(Name, Property);

                WriteObject(objProp);
            }
            else
            {
                WriteObject(obj);
            }
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
    public class OSAPSInovke : Cmdlet
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
            OSAEMethodManager.MethodQueueAdd(Name, Method, parameter1, parameter2, source);

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

        /// <summary>
        /// Used in logs to determine where an event occured from or where log messages should go to
        /// </summary>
        private const string source = "PowerShell CmdLet";

        [Parameter(Mandatory = true)]
        public string Name { get; set; }
        
        protected override void ProcessRecord()
        {
            logging.AddToLog("Show-OSA - ProcessRecord - Started", false);
            OSAEObject obj = OSAEObjectManager.GetObjectByName(Name);

            WriteObject("Name: " + obj.Name);
            WriteObject("State: " + obj.State);
            WriteObject("Description: " + obj.Description);
            WriteObject("Container: " + obj.Container);
            WriteObject("Address: " + obj.Address);
            WriteObject("Enabled: " + obj.Enabled);
            WriteObject("Base Type: " + obj.BaseType);
            WriteObject("Type: " + obj.Type);

            foreach (string method in obj.Methods)
            {
                WriteObject("Method: " + method);
            }

            foreach (OSAEObjectProperty prop in obj.Properties)
            {
                WriteObject("Property (" + prop.DataType + "): " + prop.Name + " Value = " + prop.Value);
            }

            WriteObject("Updated: " + obj.LastUpd);
        }
    }
}
