namespace OSAE.PowerShellTools 
{
    using System;
    using System.Management.Automation;

    [Cmdlet(VerbsLifecycle.Invoke, "OSALog")]
    public class OSALOG : Cmdlet
    {
        /// <summary>
        /// Provides access to the OSA logging class
        /// </summary>
        private Logging logging = Logging.GetLogger("PowerShell");

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
                    logging.AddToLog(Message, true);
                }
                else
                {
                    Logging.AddToLog(Message, true, Log);
                }
            }
            catch (Exception exc)
            {
                logging.AddToLog("An error occured while trying to run the command invoke-osalog, details: \r\n" + exc.Message, true);
            }
        }      
    }     
}
