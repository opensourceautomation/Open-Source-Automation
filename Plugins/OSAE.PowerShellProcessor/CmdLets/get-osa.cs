namespace OSAE.PowerShellTools
{
    using System.Management.Automation;

    [Cmdlet(VerbsCommon.Get, "OSA")]
    public class OSAPS : Cmdlet
    {
        //OSAELog
        private OSAE.General.OSAELog Log = new General.OSAELog();

        [Parameter(
            Mandatory = true,
            HelpMessage = "The name of the object to get")]
        public string Name { get; set; }

        protected override void ProcessRecord()
        {
            this.Log.Debug("Get-OSA - ProcessRecord - Started");
            OSAEObject obj = OSAEObjectManager.GetObjectByName(Name);
            WriteObject(obj);
        }
    }
}
