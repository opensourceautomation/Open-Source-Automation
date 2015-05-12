#define OSAESERVICECONTROLLER

namespace OSAE.Service
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceProcess;
    using System.Diagnostics;
    using System.Timers;
    using NetworkCommsDotNet;
    using log4net.Config;
    using log4net;
    using System.Reflection;
    using System.Windows.Forms;

    #endregion

    /// <summary>
    /// The primary server used in the OSA infrastructure to process information
    /// </summary>
#if OSAESERVICECONTROLLER
    partial class OSAEService : OSAEServiceBase
#else
    partial class OSAEService : ServiceBase
#endif
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
        /// Timer used to periodically check log to prune
        /// </summary>
        private static System.Timers.Timer checkLog;

        //OSAELog
        private OSAE.General.OSAELog Log = new General.OSAELog();
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
#if OSAESERVICECONTROLLER
                if (Environment.UserInteractive)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new OSAEServiceController(new OSAEService(), "OSAE Service Controller"));
                }
                else
                OSAEServiceBase.Run(new OSAEService());
#else
                ServiceBase.Run(new OSAEService());
#endif
            }
        }

        /// <summary>
        /// Public Constructor for WindowsService.
        /// - Put all of your Initialization code here.
        /// </summary>
        public OSAEService()
        {
            this.Log.Info("Service Starting");

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

            checkLog = new System.Timers.Timer(60000);
            checkLog.Elapsed += checkLogEvent;
            checkLog.Enabled = true;

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

        private void checkLogEvent(Object source, ElapsedEventArgs e)
        {
            this.Log.PruneLogs();
        }

        #endregion
    }
}
