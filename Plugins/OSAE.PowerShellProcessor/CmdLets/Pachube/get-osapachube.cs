namespace OSAE.PowerShellTools
{
    using PachubeDataAccess;
    using System.Management.Automation;

    [Cmdlet(VerbsCommon.Get, "OSAPachube")]
    public class OSAGetPachube : Cmdlet
    {
        [Parameter(Mandatory = true)]
        public string Key { get; set; }

        [Parameter(Mandatory = true)]
        public int Feed { get; set; }

        protected override void ProcessRecord()
        {
            Account a = new Account(Key);
            Feed f = new Feed(Feed, a);
            WriteObject(f.Get());
        }    
    }
}
