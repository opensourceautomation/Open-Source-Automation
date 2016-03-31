#define OSAESERVICECONTROLLER

namespace OSAE.Service
{
    using System;
    using System.Timers;
    using NetworkCommsDotNet;
    using System.Windows.Forms;

    /// <summary>
    /// The primary server used in the OSA infrastructure to process information
    /// </summary>
#if OSAESERVICECONTROLLER
    partial class OSAEService : OSAEServiceBase
#else
    partial class OSAEService : ServiceBase
#endif
    {
        /// <summary>
        /// Used when generating messages to identify where the message came from
        /// </summary>

        private OSAEPluginCollection plugins = new OSAEPluginCollection();
        private OSAEPluginCollection masterPlugins = new OSAEPluginCollection();
        private bool running = true;
        private string serviceObject = "";
        private static System.Timers.Timer checkLog;
        private OSAE.General.OSAELog Log;

        /// <summary>
        /// The Main Thread: This is where your Service is Run.
        /// </summary>
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                // replace with Speech emualation!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                // string pattern = Common.MatchPattern(args[0],"");
                // Log.Info("Processing command: " + args[0] + ", Named Script: " + pattern);
                // if (pattern != string.Empty)
                // {
                //     OSAEScriptManager.RunPatternScript(pattern, "", "OSACL");
                // }
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
            serviceObject = CheckServiceObject();
            if (serviceObject == null)
            {
                Log = new General.OSAELog("Faulted Service");
                Log.Fatal("Failed to retrieve Service's Object!");
            }
            //else
             //   OSAE.OSAEObjectStateManager.ObjectStateSet(serviceObject, "ON", serviceObject);

            Log = new General.OSAELog(serviceObject);
            Log.Info("Service Starting");

            Common.CheckComputerObject(serviceObject);
            OSAEObject obj = OSAEObjectManager.GetObjectByName(serviceObject);
            OSAEObjectManager.ObjectUpdate(serviceObject, serviceObject, obj.Address, obj.Description, obj.Type, obj.Address, Common.ComputerName, obj.MinTrustLevel, obj.Enabled);

            InitialiseOSAInEventLog();

            // These Flags set whether or not to handle that specific type of event. Set to true if you need it, false otherwise.

            CanStop = true;
            CanShutdown = true;
        }

        #region Standard Windows Service Methods

        /// <summary>
        /// The service control manager has requested us to start
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            Log.Debug("OnStart subroutine Starting...");
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptions);

            var dbConnectionStatus = Common.TestConnection();
            if (dbConnectionStatus.Success)
                Log.Info("Verified successful connection to database.");
            else
            {
                Log.Fatal("Unable to connect to database: " + dbConnectionStatus.CaughtException.Message);
                return;
            }

            try
            {
                Common.InitialiseLogFolder();
                DeleteStoreFiles();
            }
            catch (Exception ex)
            { Log.Fatal("Error getting registry settings and/or deleting logs: " + ex.Message, ex); }

            Log.Info("Removing Orphaned Methods");
            OSAEMethodManager.ClearMethodQueue();

            try
            {
                OSAE.OSAESql.RunSQL("SET GLOBAL event_scheduler = ON;");
            }
            catch (Exception ex)
            { Log.Fatal("Error setting the event scheduler: " + ex.Message, ex); }

            checkLog = new System.Timers.Timer(60000);
            checkLog.Elapsed += checkLogEvent;
            checkLog.Enabled = true;

            // Start the network service so messages can be  
            // received by the service
            StartNetworkListener();
            
            // Start the threads that monitor the plugin 
            // updates check the method queue and so on
            StartThreads(serviceObject);
            
        }

        /// <summary>
        /// The service control manager has requested us to stop
        /// </summary>
        protected override void OnStop()
        {
            Log.Info("OnStop Invoked");
            OSAE.OSAEObjectStateManager.ObjectStateSet(serviceObject, "OFF", serviceObject);
            Log.Info("Set my Object to Stopped");
            NetworkComms.Shutdown();
            ShutDownSystems();
            OSAE.General.OSAELog.FlushBuffers();
        }

        /// <summary>
        /// The service control manager has requested us to shitdown
        /// </summary>
        protected override void OnShutdown()
        {
            Log.Info("OnShutdown Invoked");
            ShutDownSystems();
        }

        void UnhandledExceptions(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            Log.Fatal("UnhandledExceptions caught : " + e.Message, e);
        }

        private void checkLogEvent(Object source, ElapsedEventArgs e)
        {
            Log.PruneLogs();
        }

        #endregion
    }
}
