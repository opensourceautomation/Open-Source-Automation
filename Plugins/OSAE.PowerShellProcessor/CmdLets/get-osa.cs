namespace OSAE.PowerShellTools
{
    using System.Management.Automation;

    [Cmdlet(VerbsCommon.Get, "OSA")]
    public class OSAPS : Cmdlet
    {
        /// <summary>
        /// Provides access to the OSA logging class
        /// </summary>
        private Logging logging = Logging.GetLogger("PowerShell");

        [Parameter(
            Mandatory = true,
            HelpMessage = "The name of the object to get")]
        public string Name { get; set; }

        protected override void ProcessRecord()
        {
            logging.AddToLog("Get-OSA - ProcessRecord - Started", false);
            OSAEObject obj = OSAEObjectManager.GetObjectByName(Name);
            WriteObject(obj);
        }
    }
}
