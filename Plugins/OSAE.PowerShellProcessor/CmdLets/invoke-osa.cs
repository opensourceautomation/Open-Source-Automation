namespace OSAE.PowerShellTools
{
    using System.Management.Automation;

    [Cmdlet(VerbsLifecycle.Invoke, "OSA")]
    public class OSAPSInvoke : Cmdlet
    {
        //OSAELog
        private OSAE.General.OSAELog Log = new General.OSAELog();

        [Parameter(Mandatory = true)]
        public string Name { get; set; }

        [Parameter(Mandatory = true)]
        public string Method { get; set; }

        private string parameter1 = string.Empty;

        [Parameter(Mandatory = false)]
        [Alias("P1")]
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
        [Alias("P2")]
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
            this.Log.Debug("Invoke-OSA - ProcessRecord - Started");
            OSAEMethodManager.MethodQueueAdd(Name, Method, parameter1, parameter2, "PowerShell");

            WriteObject(true);
        }
    }
}
