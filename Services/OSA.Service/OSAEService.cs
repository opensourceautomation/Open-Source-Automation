namespace OSAE.Service
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceProcess;
    using System.Diagnostics;

    #endregion

    /// <summary>
    /// The primary server used in the OSA infrastructure to process information
    /// </summary>
    partial class OSAEService : ServiceBase
    {
        #region Member Variables
        /// <summary>
        /// Used when generating messages to identify where the message came from
        /// </summary>
        private const string sourceName = "OSAE Service";
              
        private ServiceHost sHost;
        private WCF.WCFService wcfService;
        private OSAEPluginCollection plugins = new OSAEPluginCollection();
        private OSAEPluginCollection masterPlugins = new OSAEPluginCollection();

        private bool goodConnection = false;

        /// <summary>
        /// Provides access to logging
        /// </summary>
        Logging logging = Logging.GetLogger(sourceName);

        private bool running = true;
        
        /// <summary>
        /// Timer used to periodically check if plugins are still running
        /// </summary>
        private System.Timers.Timer checkPlugins = new System.Timers.Timer();

        #endregion

        /// <summary>
        /// The Main Thread: This is where your Service is Run.
        /// </summary>
        static void Main(string[] args) 
        {          
            if (args.Length > 0)
            {
                string pattern = Common.MatchPattern(args[0]);
                Logging.AddToLog("Processing command: " + args[0] + ", Named Script: " + pattern, true, "OSACL");
                if (pattern != string.Empty)
                {
                    OSAEScriptManager.RunPatternScript(pattern, "", "OSACL");
                }
            }
            else
            {
                ServiceBase.Run(new OSAEService());
            }
            
        }
        
        /// <summary>
        /// Public Constructor for WindowsService.
        /// - Put all of your Initialization code here.
        /// </summary>
        public OSAEService()
        {
            logging.AddToLog("Service Starting", true);

            InitialiseOSAInEventLog();

            // These Flags set whether or not to handle that specific
            // type of event. Set to true if you need it, false otherwise.
            
            this.CanStop = true;
            this.CanShutdown = true;
        }

        #region Standard Windows Service Methods

        /// <summary>
        /// The service control manager has requested us to start
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
//#if (DEBUG)
//            Debugger.Launch(); //<-- Simple form to debug a web services 
//#endif
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptions);
            try
            {
                Common.InitialiseLogFolder();
                DeleteStoreFiles();
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error getting registry settings and/or deleting logs: " + ex.Message, true);
            }

            logging.AddToLog("OnStart", true);

            logging.AddToLog("Removing Orphaned methods", true);
            OSAEMethodManager.ClearMethodQueue();

            Common.CreateComputerObject(sourceName);
            CreateServiceObject();

            // Start the WCF service so messages can be sent 
            // and received by the service
            StartWCFService();

            // Start the threads that monitor the plugin 
            // updates check the method queue and so on
            StartThreads();
        }                            

        /// <summary>
        /// The service control manager has requested us to stop
        /// </summary>
        protected override void OnStop()
        {
            logging.AddToLog("OnStop Invoked", false);

            ShutDownSystems();
        }        

        /// <summary>
        /// The service control manager has requested us to shitdown
        /// </summary>
        protected override void OnShutdown() 
        {
            logging.AddToLog("OnShutdown Invoked", false);

            ShutDownSystems();
        }

        void UnhandledExceptions(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            logging.AddToLog("UnhandledExceptions caught : " + e.Message + " - InnerException: " + e.InnerException.Message, true);
        }
        #endregion
    }
}
