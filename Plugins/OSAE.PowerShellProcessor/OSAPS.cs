namespace OSAE.PowerShellTools
{
    using System.Management.Automation;  // Windows PowerShell assembly.

    [Cmdlet(VerbsCommon.Get, "OSAPS")]
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

    [Cmdlet(VerbsCommon.Set, "OSAPS")]
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
            OSAEObject obj = OSAEObjectManager.GetObjectByName(Name);
            OSAEObjectPropertyManager.ObjectPropertySet(Name, Property, Value, source);

            WriteObject(true);
        }

    }

    [Cmdlet(VerbsLifecycle.Invoke, "OSAPS")]
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

        [Parameter(Mandatory = true)]
        public string Value { get; set; }
               
        protected override void ProcessRecord()
        {
            // OSAEMethodManager.MethodQueueAdd(name, Method, Value, ""); TODO

            WriteObject(true);
        }
    }

    [Cmdlet(VerbsCommon.Show, "OSAPS")]
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
