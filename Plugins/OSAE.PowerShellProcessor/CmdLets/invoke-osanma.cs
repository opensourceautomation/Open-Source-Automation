namespace OSAE.PowerShellTools
{
    using NMALib;
    using System;
    using System.Management.Automation;

    [Cmdlet(VerbsLifecycle.Invoke, "OSANMA")]
    public class OSANMA : Cmdlet
    {
        //OSAELog
        private OSAE.General.OSAELog Log = new General.OSAELog("POWERSHELL");

        [Parameter(Mandatory = true)]
        public string APIKey { get; set; }

        [Parameter(Mandatory = true)]
        public int Priority { get; set; }

        [Parameter(Mandatory = true)]
        public string Event { get; set; }

        [Parameter(Mandatory = true)]
        public string Description { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                Log.Info("Invoke-OSANMA - ProcessRecord - Started");

                var notification =
                    new NMANotification
                    {
                        Description = Description, Event = Event, Priority = (NMANotificationPriority)Priority
                    };

                NMAClientConfiguration config = new NMAClientConfiguration();
                config.ApplicationName = "OSA";
                config.ApiKeychain = APIKey;

                var client = new NMAClient(config);

                // Post the notification.
                client.PostNotification(notification);

                WriteObject(true);
            }
            catch (Exception exc)
            { Log.Error("An error occured while trying to run the command invoke-osanma, details", exc); }                
        }        
    }
}
