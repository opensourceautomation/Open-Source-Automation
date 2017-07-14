namespace Manager_WPF
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.ServiceProcess;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Navigation;
    using OSAE;
    using NetworkCommsDotNet;
    using System.Net;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon MyNotifyIcon;
        //ServiceController myService = new ServiceController();

        private OSAE.General.OSAELog Log = new OSAE.General.OSAELog("Manager-" + Common.ComputerName);

        private BindingList<PluginDescription> pluginList = new BindingList<PluginDescription>();
        System.Timers.Timer Clock = new System.Timers.Timer();
        private bool clicked = true;
        private bool webclicked = true;
        private bool MySQLclicked = true;
        private bool starting = false;
        private bool webstarting = false;
        private bool MySQLstarting = false;
        private bool UWSenabled = false;
        private bool isCient = false;
        private const string Unique = "OSAE Manager";

        [STAThread]
        public static void Main(string[] args)
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                string filename;

                if (args != null && args.Length == 1)
                {
                    if (System.IO.File.Exists(System.IO.Path.GetFullPath(args[0])))
                    {
                        filename = System.IO.Path.GetFileName(System.IO.Path.GetFullPath(args[0]));

                        if (filename.EndsWith("osapp", StringComparison.Ordinal))
                        {
                            // its a plugin package
                            PluginInstallerHelper pInst = new PluginInstallerHelper();
                            pInst.InstallPlugin(System.IO.Path.GetFullPath(args[0]));
                        }
                    }
                }
                else
                {
                    var application = new App();

                    application.InitializeComponent();
                    application.Run();
                }
                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            MyNotifyIcon = new System.Windows.Forms.NotifyIcon();
            MyNotifyIcon.Icon = Properties.Resources.icon;
            MyNotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(MyNotifyIcon_MouseDoubleClick);
            MyNotifyIcon.MouseDown += new System.Windows.Forms.MouseEventHandler(MyNotifyIcon_MouseDown);
            MyNotifyIcon.Visible = true;
            MyNotifyIcon.Text = "Manager";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ServiceController myService = new ServiceController();
            ServiceController[] services = ServiceController.GetServices();
            UWSenabled = false;
            if (isCient == true)
            {
                foreach (ServiceController service in services)
                {
                    if (service.ServiceName == "UltiDev Web Server Pro") UWSenabled = true;
                }
            }

            try
            {
                myService.ServiceName = "OSAE";
                string svcStatus = myService.Status.ToString();
                if (svcStatus == "Running")
                {
                    lbl_isRunning.Content = "RUNNING";
                    btnService.Content = "Stop";
                }
                else if (svcStatus == "Stopped")
                {
                    lbl_isRunning.Content = "STOPPED";
                    btnService.Content = "Start";
                }
            }
            catch
            {
                try
                {
                    myService.ServiceName = "OSAE Client";
                    string svcStatus = myService.Status.ToString();
                    isCient = true;
                    if (svcStatus == "Running")
                    {
                        lbl_isRunning.Content = "RUNNING";
                        btnService.Content = "Stop";
                    }
                    else if (svcStatus == "Stopped")
                    {
                        lbl_isRunning.Content = "STOPPED";
                        btnService.Content = "Start";
                    }
                }
                catch
                {
                    lbl_isRunning.Content = "NOT INSTALLED";
                    btnService.IsEnabled = false;
                }
            }

            if (!UWSenabled || isCient == true)
            {
                btnWebService.Visibility = Visibility.Hidden;
                lblUWS.Visibility = Visibility.Hidden;
                lbl_isWebRunning.Visibility = Visibility.Hidden;
            }

            if (isCient == true)
            {
                btnMySQLService.Visibility = Visibility.Hidden;
                lblMySQL.Visibility = Visibility.Hidden;
                lbl_isMySQLRunning.Visibility = Visibility.Hidden;
            }

            Clock.Interval = 2000;
            Clock.Elapsed += new System.Timers.ElapsedEventHandler(CheckService);
            Clock.Start();

            try
            {
                Log.Debug("Starting UDP listener");
                NetworkComms.AppendGlobalIncomingPacketHandler<string>("Manager", ManagerMessageReceived);
                UDPConnection.StartListening(new IPEndPoint(IPAddress.Any, 10052));
                Log.Debug("UPD Listener started");
            }
            catch (Exception ex)
            { Log.Error("Error starting listener", ex); }
        }

        private void CheckService(object sender, EventArgs e)
        {
            CheckOSAEService();

            if (UWSenabled == true) CheckWebService();
            if (isCient != true) CheckMySQLService();

            /*
            string svcStatus = myService.Status.ToString();
            if (svcStatus == "Running")
            {
                setLabel(Brushes.Green, "RUNNING");
                setButton("Stop", true);
                starting = false;
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, new Action(delegate
                {
                    mnuInstall.IsEnabled = true;
                    mnuInstall.ToolTip = "You are clear to Install Plugins";
                }));
            }
            else if (svcStatus == "Stopped" && !starting)
            {
                setLabel(Brushes.Red, "STOPPED");
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, new Action(delegate
                {
                    mnuInstall.IsEnabled = false;
                    mnuInstall.ToolTip = "Service Must be Running";
                }));

                foreach (PluginDescription pd in pluginList)
                {
                    if (pd.Enabled) pd.Status = "Stopped";
                }
                setButton("Start", true);
                if (!clicked)
                {
                    Log.Info("Service died.  Attempting to restart.");
                    clicked = false;
                    System.Threading.Thread.Sleep(5000);
                    Thread m_WorkerThreadStart = new Thread(new ThreadStart(StartService));
                    m_WorkerThreadStart.Start();
                }
            }
            */
        }

        private void CheckWebService()
        {
            try
            {
                ServiceController hiService = new ServiceController();
                hiService.ServiceName = "UWS HiPriv Services";
                string hisvcStatus = hiService.Status.ToString();

                ServiceController loService = new ServiceController();
                loService.ServiceName = "UWS LoPriv Services";
                string losvcStatus = loService.Status.ToString();

                ServiceController wsService = new ServiceController();
                wsService.ServiceName = "UltiDev Web Server Pro";
                string wssvcStatus = wsService.Status.ToString();

                if (hisvcStatus == "Running" && losvcStatus == "Running" && wssvcStatus == "Running")
                {
                    setWebLabel(Brushes.Green, "RUNNING");
                    setWebButton("Stop", true);
                    webstarting = false;
                }
                else
                {
                    setWebLabel(Brushes.Red, "STOPPED");
                    setWebButton("Start", true);

                    if (!webclicked)
                    {
                        Log.Info("Web Services died.  Attempting to restart.");
                        webclicked = false;
                        System.Threading.Thread.Sleep(5000);
                        Thread m_WorkerThreadStart = new Thread(new ThreadStart(this.StartWebService));
                        m_WorkerThreadStart.Start();
                    }
                }
            }
            catch (Exception ex)
            { Log.Error("Error checking Web Services", ex); }
        }

        private void CheckMySQLService()
        {
            try
            {
                ServiceController hiService = new ServiceController();
                hiService.ServiceName = "MySQL";
                string svcStatus = hiService.Status.ToString();

                if (svcStatus == "Running")
                {
                    setMySQLLabel(Brushes.Green, "RUNNING");
                    setMySQLButton("Stop", true);

                    MySQLstarting = false;
                }
                else
                {
                    setMySQLLabel(Brushes.Red, "STOPPED");
                    setMySQLButton("Start", true);
                    if (!MySQLclicked)
                    {
                        Log.Info("MySQL Services died.  Attempting to restart.");
                        MySQLclicked = false;
                        System.Threading.Thread.Sleep(5000);
                        Thread m_WorkerThreadStart = new Thread(new ThreadStart(this.StartMySQLService));
                        m_WorkerThreadStart.Start();
                    }
                }
            }
            catch (Exception ex)
            { Log.Error("Error checking MySQL Services", ex); }
        }

        private void CheckOSAEService()
        {
            ServiceController osaService = new ServiceController();
            try
            {
                osaService.ServiceName = "OSAE";
                string svcStatus = osaService.Status.ToString();
                isCient = false;
                if (svcStatus == "Running")
                {
                    setLabel(Brushes.Green, "RUNNING");
                    setButton("Stop", true);
                    setInstallButton(true);
                    starting = false;
                }
                else
                {
                    setLabel(Brushes.Red, "STOPPED");
                    setButton("Start", true);
                    setInstallButton(false);
                    if (!clicked)
                    {
                        Log.Info("OSAE Services died.  Attempting to restart.");
                        clicked = false;
                        System.Threading.Thread.Sleep(5000);
                        Thread m_WorkerThreadStart = new Thread(new ThreadStart(this.StartService));
                        m_WorkerThreadStart.Start();
                    }
                }
            }
            catch
            {
                try
                {
                    osaService.ServiceName = "OSAE Client";
                    string svcStatus = osaService.Status.ToString();
                    isCient = true;
                    if (svcStatus == "Running")
                    {
                        setLabel(Brushes.Green, "RUNNING");
                        setButton("Stop", true);
                        setInstallButton(true);
                        starting = false;
                    }
                    else
                    {
                        setLabel(Brushes.Red, "STOPPED");
                        setButton("Start", true);
                        setInstallButton(false);
                        if (!clicked)
                        {
                            Log.Info("OSAE Services died.  Attempting to restart.");
                            clicked = false;
                            System.Threading.Thread.Sleep(5000);
                            Thread m_WorkerThreadStart = new Thread(new ThreadStart(this.StartService));
                            m_WorkerThreadStart.Start();
                        }
                    }
                }
                catch
                {
                    lbl_isRunning.Content = "NOT INSTALLED";
                    btnService.IsEnabled = false;
                }
            }
        }

        private void StartService()
        {
            try
            {
                System.TimeSpan ts = new TimeSpan(0, 0, 30);
                ServiceController osaService = new ServiceController();
                if (isCient == true) osaService.ServiceName = "OSAE Client";
                else osaService.ServiceName = "OSAE";
                osaService.Start();
                osaService.WaitForStatus(ServiceControllerStatus.Running, ts);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error starting service.  " + ex);
                Log.Error("Error starting service", ex);
                starting = false;
            }
        }

        private void StopService()
        {
            try
            {
                System.TimeSpan ts = new TimeSpan(0, 0, 30);
                ServiceController osaService = new ServiceController();
                if (isCient == true) osaService.ServiceName = "OSAE Client";
                else osaService.ServiceName = "OSAE";
                osaService.Stop();
                osaService.WaitForStatus(ServiceControllerStatus.Stopped, ts);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error stopping service.  " + ex);
                Log.Error("Error stopping service", ex);
            }
        }

        private void StartWebService()
        {
            System.TimeSpan ts = new TimeSpan(0, 0, 30);

            Log.Info("Checking for UWS HiPriv Services...");
            try
            {
                ServiceController hiService = new ServiceController();
                hiService.ServiceName = "UWS HiPriv Services";
                string svcStatus = hiService.Status.ToString();
                if (svcStatus == "Stopped")
                {
                    hiService.Start();
                    hiService.WaitForStatus(ServiceControllerStatus.Running, ts);
                    Log.Info("Started UWS Priv Services");
                }
            }
            catch (Exception ex)
            { Log.Error("Error checking for UWS Server!", ex); }

            ts = new TimeSpan(0, 0, 30);
            try
            {
                ServiceController loService = new ServiceController();
                loService.ServiceName = "UWS LoPriv Services";
                string svcStatus = loService.Status.ToString();
                if (svcStatus == "Stopped")
                {
                    loService.Start();
                    loService.WaitForStatus(ServiceControllerStatus.Running, ts);
                    Log.Info("Started UWS LoPriv Services");
                }

            }
            catch (Exception ex)
            { Log.Error("Error checking for UWS Server!", ex); }

            ts = new TimeSpan(0, 0, 30);
            try
            {
                ServiceController wsService = new ServiceController();
                wsService.ServiceName = "UltiDev Web Server Pro";
                string svcStatus = wsService.Status.ToString();
                if (svcStatus == "Stopped")
                {
                    wsService.Start();
                    wsService.WaitForStatus(ServiceControllerStatus.Running, ts);
                    Log.Info("Started UWS Web Server");
                }
            }
            catch (Exception ex)
            { Log.Error("Error checking for UWS Server!", ex); }
        }

        private void StopWebService()
        {
            System.TimeSpan ts = new TimeSpan(0, 0, 30);

            Log.Info("Checking for UWS HiPriv Services...");
            try
            {
                ServiceController hiService = new ServiceController();
                hiService.ServiceName = "UWS HiPriv Services";
                string svcStatus = hiService.Status.ToString();
                if (svcStatus == "Running")
                {
                    hiService.Stop();
                    hiService.WaitForStatus(ServiceControllerStatus.Running, ts);
                    Log.Info("Started UWS Priv Services");
                }
            }
            catch (Exception ex)
            { Log.Error("Error checking for UWS Server!", ex); }

            ts = new TimeSpan(0, 0, 30);
            try
            {
                ServiceController loService = new ServiceController();
                loService.ServiceName = "UWS LoPriv Services";
                string svcStatus = loService.Status.ToString();
                if (svcStatus == "Running")
                {
                    loService.Stop();
                    loService.WaitForStatus(ServiceControllerStatus.Running, ts);
                    Log.Info("Started UWS LoPriv Services");
                }
            }
            catch (Exception ex)
            { Log.Error("Error checking for UWS Server!", ex); }

            ts = new TimeSpan(0, 0, 30);
            try
            {
                ServiceController wsService = new ServiceController();
                wsService.ServiceName = "UltiDev Web Server Pro";
                string svcStatus = wsService.Status.ToString();
                if (svcStatus == "Running")
                {
                    wsService.Stop();
                    wsService.WaitForStatus(ServiceControllerStatus.Running, ts);
                    Log.Info("Started UWS UWS Server");
                }
            }
            catch (Exception ex)
            { Log.Error("Error checking for UWS Server!", ex); }
        }

        private void StartMySQLService()
        {
            System.TimeSpan ts = new TimeSpan(0, 0, 30);

            Log.Info("Checking for MySQL Services...");
            try
            {
                ServiceController hiService = new ServiceController();
                hiService.ServiceName = "MySQL";
                string svcStatus = hiService.Status.ToString();
                if (svcStatus == "Stopped")
                {
                    hiService.Start();
                    hiService.WaitForStatus(ServiceControllerStatus.Running, ts);
                    Log.Info("Started MySQL Services");
                }
            }
            catch (Exception ex)
            { Log.Error("Error checking for MySQL Service!", ex); }
        }

        private void StopMySQLService()
        {
            System.TimeSpan ts = new TimeSpan(0, 0, 30);

            Log.Info("Checking for MySQL Services...");
            try
            {
                ServiceController hiService = new ServiceController();
                hiService.ServiceName = "MySQL";
                string svcStatus = hiService.Status.ToString();
               // if (svcStatus == "Running")
               // {
                    hiService.Stop();
                    hiService.WaitForStatus(ServiceControllerStatus.Running, ts);
                    Log.Info("Stopped MySQL Services");
               // }
            }
            catch (Exception ex)
            { Log.Error("Error checking for MySQL Service!", ex); }
        }

        private void setButton(string text, bool enabled)
        {
            btnService.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, new Action(delegate
                {
                    btnService.IsEnabled = enabled;
                    btnService.Content = text;
                }));
        }

        private void setWebButton(string text, bool enabled)
        {
            btnWebService.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, new Action(delegate
            {
                btnWebService.IsEnabled = enabled;
                btnWebService.Content = text;
            }));
        }

        private void setInstallButton(bool enabled)
        {
            mnuInstall.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, new Action(delegate
            {
                if (enabled)
                    {
                    mnuInstall.IsEnabled = true;
                    mnuInstall.ToolTip = "You are clear to Install Plugins";
                }
                else
                {
                    mnuInstall.IsEnabled = false;
                    mnuInstall.ToolTip = "Service Must be Running";
                }
            }));
        }

        private void setMySQLButton(string text, bool enabled)
        {
            btnMySQLService.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, new Action(delegate
            {
                btnMySQLService.IsEnabled = enabled;
                btnMySQLService.Content = text;
            }));
        }

        private void setLabel(Brush color, string text)
        {
            lbl_isRunning.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, new Action(delegate
                {
                    lbl_isRunning.Foreground = color;
                    lbl_isRunning.Content = text;
                }));
        }

        private void setWebLabel(Brush color, string text)
        {
            lbl_isWebRunning.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, new Action(delegate
            {
                lbl_isWebRunning.Foreground = color;
                lbl_isWebRunning.Content = text;
            }));
        }

        private void setMySQLLabel(Brush color, string text)
        {
            lbl_isMySQLRunning.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, new Action(delegate
            {
                lbl_isMySQLRunning.Foreground = color;
                lbl_isMySQLRunning.Content = text;
            }));
        }

        void HandleRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
            e.Handled = true;
        }
                
        private void btnService_Click(object sender, RoutedEventArgs e)
        {
            if (btnService.Content.ToString() == "Stop") clicked = true;

            setButton(btnService.Content.ToString(), false);
            if (btnService.Content.ToString() == "Stop")
            {
                //client.Close();
                setLabel(Brushes.Red, "STOPPING...");
                foreach (PluginDescription pd in pluginList)
                {
                    if (pd.Enabled) pd.Status = "Stopping...";
                }
                Thread m_WorkerThreadStop = new Thread(new ThreadStart(this.StopService));
                m_WorkerThreadStop.Start(); 
            }
            else if (btnService.Content.ToString() == "Start")
            {
                starting = true;
                setLabel(Brushes.Green, "STARTING...");
                foreach (PluginDescription pd in pluginList)
                {
                    if (pd.Enabled) pd.Status = "Starting...";
                }
                System.TimeSpan ts = new TimeSpan(0, 0, 30);
                Thread m_WorkerThreadStart = new Thread(new ThreadStart(this.StartService));
                m_WorkerThreadStart.Start(); 
            }            
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                Log.Info("Manager Closing");
                Clock.Stop();
                Clock = null;
                Log.Info("Timer stopped");
                NetworkComms.Shutdown();
            }
            catch(Exception ex)
            { Log.Error("Error closing Manager", ex); }
        }

        void MyNotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                ShowInTaskbar = false;
            else if (WindowState == WindowState.Normal)
                ShowInTaskbar = true;
        }

        void MyNotifyIcon_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                ContextMenu menu = (ContextMenu)this.FindResource("NotifierContextMenu");
                menu.IsOpen = true;
            }
        }

        private void Menu_Manager(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        private void Menu_GUI(object sender, RoutedEventArgs e)
        {
            Process.Start(Common.ApiPath + "\\OSAE.Screens.exe");
        }

        private void hypWebUI_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://localhost:8081/default.aspx");
        }

        private void hypScreens_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Common.ApiPath + "\\OSAE.Screens.exe");
        }

        private void InstallPlugin_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = string.Empty; // Default file name 
            dlg.DefaultExt = ".osapp"; // Default file extension 
            dlg.Filter = "Open Source Automation Plugin Pakages (.osapp)|*.osapp"; // Filter files by extension 

            // Show open file dialog box 
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                Log.Info("Plugin file selected: " + dlg.FileName + ".  Installing...");
                // Open Plugin Package 
                PluginInstallerHelper pInst = new PluginInstallerHelper();
                pInst.InstallPlugin(dlg.FileName);
            }

        }

        private void InstallWebPlugin()
        {
            using (WebClient wc = new WebClient())
            {
                wc.DownloadFile("http://opensourceautomation.com/dl.php?plugin_id=e733a7e3-25e3-40be-b976-f8fba026fd2b&plugin_version=0.4.7&plugin_state=beta", @".\temp.osapp");
            }
            PluginInstallerHelper pInst = new PluginInstallerHelper();
            pInst.InstallPlugin(".\\temp.osapp");
        }

        private void hypLog_Click(object sender, RoutedEventArgs e)
        {
            LogWindow l = new LogWindow();
            l.ShowDialog();
        }

        private void CommandMessageReceived(PacketHeader header, Connection connection, string message)
        {
            string[] param = message.Split('|');
            if (param[2].Trim() == Common.ComputerName)
            {
                Log.Info("CMDLINE received: " + param[0].Trim() + " - " + param[1].Trim());
                Process pr = new Process();
                pr.StartInfo.FileName = param[0].Trim();
                pr.StartInfo.Arguments = param[1].Trim();
                pr.Start();
            }
        }
      
        private void btnWebService_Click(object sender, RoutedEventArgs e)
        {
            if (btnWebService.Content.ToString() == "Stop")
                webclicked = true;

            setWebButton(btnWebService.Content.ToString(), false);
            if (btnWebService.Content.ToString() == "Stop")
            {
                //client.Close();
                setWebLabel(Brushes.Red, "STOPPING...");
                Thread m_WorkerThreadStop = new Thread(new ThreadStart(this.StopWebService));
                m_WorkerThreadStop.Start();
            }
            else if (btnWebService.Content.ToString() == "Start")
            {
                webstarting = true;
                setWebLabel(Brushes.Green, "STARTING...");

                System.TimeSpan ts = new TimeSpan(0, 0, 30);
                Thread m_WorkerThreadStart = new Thread(new ThreadStart(this.StartWebService));
                m_WorkerThreadStart.Start();
            }
        }

        private void btnMySQLService_Click(object sender, RoutedEventArgs e)
        {
            if (btnMySQLService.Content.ToString() == "Stop")
                MySQLclicked = true;

            setMySQLButton(btnMySQLService.Content.ToString(), false);
            if (btnMySQLService.Content.ToString() == "Stop")
            {
                //client.Close();
                setMySQLLabel(Brushes.Red, "STOPPING...");
                Thread m_WorkerThreadStop = new Thread(new ThreadStart(this.StopMySQLService));
                m_WorkerThreadStop.Start();
            }
            else if (btnMySQLService.Content.ToString() == "Start")
            {
                MySQLstarting = true;
                setMySQLLabel(Brushes.Green, "STARTING...");

                System.TimeSpan ts = new TimeSpan(0, 0, 30);
                Thread m_WorkerThreadStart = new Thread(new ThreadStart(this.StartMySQLService));
                m_WorkerThreadStart.Start();
            }
        }

        private void ManagerMessageReceived(PacketHeader header, Connection connection, string message)
        {
            Log.Info("<- Manager: " + message);
            string[] items = message.Split('|');
            //ServiceName + " | " + MethodName, new IPEndPoint(IPAddress.Broadcast, 10051));

            if (items[0].Trim() == "SERVICE-" + Common.ComputerName)
            {
                if (items[1].Trim() == "ON") StartService();
                else if (items[1].Trim() == "OFF") StopService();
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Common.ApiPath + "\\OSAE.Voice.exe");
        }
    }
}
