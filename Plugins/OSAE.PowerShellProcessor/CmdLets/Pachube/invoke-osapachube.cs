namespace OSAE.PowerShellTools
{
    using PachubeDataAccess;
    using System.Management.Automation;

    [Cmdlet(VerbsLifecycle.Invoke, "OSAPachube")]
    public class OSAInvokePachube : Cmdlet
    {
        [Parameter(Mandatory = true)]
        public string Key { get; set; }

        [Parameter(Mandatory = true)]
        public string Value { get; set; }

        [Parameter(Mandatory = true)]
        public int Feed { get; set; }

        protected override void ProcessRecord()
        {
            Account a = new Account(Key);
            Feed f = new Feed(Feed, a);
            StringFeedItem i = new StringFeedItem(Key, Value);
            f.Post(i);
        } 
    }
}
