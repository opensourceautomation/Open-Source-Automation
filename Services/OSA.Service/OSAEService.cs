namespace OSAE.Service
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceProcess;
    using System.Diagnostics;
    using NetworkCommsDotNet;
    using log4net.Config;
    using log4net;
    using System.Reflection;

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
              
        private OSAEPluginCollection plugins = new OSAEPluginCollection();
        private OSAEPluginCollection masterPlugins = new OSAEPluginCollection();

        private bool goodConnection = false;

        private bool running = true;
        
        /// <summary>
        /// Timer used to periodically check if plugins are still running
        /// </summary>
        //private System.Timers.Timer checkPlugins = new System.Timers.Timer();

        //OSAELog
        private OSAE.General.OSAELog Log = new General.OSAELog("OSAE Service");
        #endregion


        /// <summary>
        /// The Main Thread: This is where your Service is Run.
        /// </summary>
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                string pattern = Common.MatchPattern(args[0]);
                //this.Log.Info("Processing command: " + args[0] + ", Named Script: " + pattern);
                if (pattern != string.Empty)
                {
                    OSAEScriptManager.RunPatternScript(pattern, "", "OSACL");
                }
            }
            else
            {
                //Debugger.Launch();
                ServiceBase.Run(new OSAEService());
            }
        }
        
        /// <summary>
        /// Public Constructor for WindowsService.
        /// - Put all of your Initialization code here.
        /// </summary>
        public OSAEService()
        {
            this.Log.Info("================");
            this.Log.Info("Service Starting");
            this.Log.Info("================");
            
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
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptions);

            var dbConnectionStatus = Common.TestConnection();
            if (dbConnectionStatus.Success)
            {
                this.Log.Info("Verified successful connection to database.");
            }
            else
            {
                this.Log.Fatal("Unable to connect to database: " + dbConnectionStatus.CaughtException.Message);
                return;
            }

            try
            {
                Common.InitialiseLogFolder();
                DeleteStoreFiles();
            }
            catch (Exception ex)
            {
                this.Log.Fatal("Error getting registry settings and/or deleting logs: " + ex.Message, ex);
            }

            this.Log.Info("OnStart");
            
            this.Log.Info("Removing Orphaned Methods");
            OSAEMethodManager.ClearMethodQueue();

            Common.CreateComputerObject(sourceName);
            CreateServiceObject();

            OSAE.OSAESql.RunSQL("SET GLOBAL event_scheduler = ON;");

            // Start the network service so messages can be  
            // received by the service
            StartNetworkListener();


            // Start the threads that monitor the plugin 
            // updates check the method queue and so on
            StartThreads();
        }                            

        /// <summary>
        /// The service control manager has requested us to stop
        /// </summary>
        protected override void OnStop()
        {
            this.Log.Info("OnStop Invoked");
            NetworkComms.Shutdown();
            ShutDownSystems();
            OSAE.General.OSAELog.FlushBuffers();
        }        

        /// <summary>
        /// The service control manager has requested us to shitdown
        /// </summary>
        protected override void OnShutdown() 
        {
            this.Log.Info("OnShutdown Invoked");
            ShutDownSystems();
        }

        void UnhandledExceptions(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            this.Log.Fatal("UnhandledExceptions caught : " + e.Message, e);
        }
        #endregion
    }
}
