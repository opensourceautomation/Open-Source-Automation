namespace OSAE.PowerShellTools 
{
    using System;
    using System.Management.Automation;

    [Cmdlet(VerbsLifecycle.Invoke, "OSALog")]
    public class OSALOG : Cmdlet
    {
        //OSAELog
        private OSAE.General.OSAELog Log2 = new General.OSAELog("Powershell");

        [Parameter(Mandatory = true)]
        public string Message { get; set; }

        [Parameter(Mandatory = false)]
        public string Log { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                if (string.IsNullOrEmpty(Log))
                {
                    this.Log2.Debug(Message);
                }
                else
                {
                    this.Log2.Debug(Message);
                }
            }
            catch (Exception exc)
            {
                this.Log2.Error("An error occured while trying to run the command invoke-osalog, details", exc);
            }
        }      
    }     
}
