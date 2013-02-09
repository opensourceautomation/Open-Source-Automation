namespace Manager_WPF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.ServiceModel;
    using System.ServiceProcess;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using OSAE;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Single, UseSynchronizationContext = false)]
    public partial class MainWindow : Window, WCFServiceReference.IWCFServiceCallback
    {
        private System.Windows.Forms.NotifyIcon MyNotifyIcon;
        ServiceController myService = new ServiceController();
        WCFServiceReference.WCFServiceClient wcfObj;
        OSAE osae = null;

        /// <summary>
        /// Used to get access to the logging facility
        /// </summary>
        private Logging logging = Logging.GetLogger("Manager_WPF");

        private BindingList<PluginDescription> pluginList = new BindingList<PluginDescription>();
        System.Timers.Timer Clock = new System.Timers.Timer();
        private bool clicked = true;
        private bool starting = false;
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
                            PluginInstallerHelper.InstallPlugin(System.IO.Path.GetFullPath(args[0]));
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
            //imgUpdate.Source = (ImageSource)FindResource("upgrade.png");

            // Test the connection to the DB is valid before we start else the UI will
            // hang when it tries to connect
            if (!Common.TestConnection())
            {
                MessageBox.Show("The OSA DB could not be contacted, Please ensure the correct address is specified and the DB is available");
                return;
            }

            // don't initialise this until we know we can get a connection
            osae = new OSAE("Manager_WPF");

            loadPlugins();

            if (connectToService())
            {
                Thread thread = new Thread(() => messageHost("info", "connected"));
                thread.Start();
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

            Clock.Interval = 1000;
            Clock.Elapsed += new System.Timers.ElapsedEventHandler(CheckService);
            Clock.Start();

            //dgLocalPlugins.SelectedIndex = 0;
        }

        private bool connectToService()
        {
            try
            {
                EndpointAddress ep = new EndpointAddress("net.tcp://" + Common.DBConnection + ":8731/WCFService/");
                InstanceContext context = new InstanceContext(this);
                wcfObj = new WCFServiceReference.WCFServiceClient(context, "NetTcpBindingEndpoint", ep);
                wcfObj.Subscribe();
                logging.AddToLog("Connected to Service", true);
                //reloadPlugins();
                return true;
            }
            catch (Exception ex)
            {
                logging.AddToLog("Unable to connect to service.  Is it running? - " + ex.Message, true);
                return false;
            }
        }

        private void messageHost(string msgType, string message)
        {
            try
            {
                if (wcfObj.State == CommunicationState.Opened)
                    wcfObj.messageHost(msgType, message, osae.ComputerName);
                else
                {
                    if (connectToService())
                        wcfObj.messageHost(msgType, message, osae.ComputerName);
                }
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error messaging host: " + ex.Message, true);
            }
        }

        private void dgLocalPlugins_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                gbPluginInfo.Visibility = System.Windows.Visibility.Visible;
                DataGrid dg = (DataGrid)sender;
                PluginDescription p = (PluginDescription)dg.SelectedItems[0];

                lblAuthor.Content = "by " + p.Author;
                txbkDescription.Text = p.Description;
                lblPluginName.Content = p.Type;
                lblVersion.Content = p.Version;

                if (p.WikiUrl != string.Empty)
                {
                    hypWiki.NavigateUri = new Uri(p.WikiUrl);
                    txblWiki.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    txblWiki.Visibility = System.Windows.Visibility.Hidden;
                }

                if (p.Upgrade != string.Empty)
                {
                    imgUpdate.Visibility = System.Windows.Visibility.Visible;
                    Uri u = new Uri("http://www.opensourceautomation.com/plugin_details.php?pid=" + p.ID);
                    hypUpdate.NavigateUri = u;
                }
                else
                {
                    imgUpdate.Visibility = System.Windows.Visibility.Hidden;
                }

                string pluginPath = Common.ApiPath + "\\Plugins\\" + p.Path + "\\";
                string[] paths = System.IO.Directory.GetFiles(pluginPath, "Screenshot*");

                logging.AddToLog("Plugin path: " + pluginPath, true);
                logging.AddToLog("paths length: " + paths.Length.ToString(), true);
                if (paths.Length > 0)
                {
                    // load the image, specify CacheOption so the file is not locked
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = new Uri(paths[0]);
                    image.EndInit();

                    imgPlugin.Stretch = Stretch.Fill;
                    imgPlugin.Source = image;
                }
                else
                {
                    imgPlugin.Source = null;
                }
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error loading details: " + ex.Message, true);
            }
        }

        private void CheckService(object sender, EventArgs e)
        {            
            string svcStatus = myService.Status.ToString();

            if (svcStatus == "Running")
            {
                setLabel(Brushes.Green, "RUNNING");
                setButton("Stop", true);
                if (wcfObj == null || wcfObj.State == CommunicationState.Closed || wcfObj.State == CommunicationState.Faulted)
                {
                    if (connectToService())
                    {
                        Thread thread = new Thread(() => messageHost("info", "connected"));
                        thread.Start();
                    }
                }
                starting = false;
            }
            else if (svcStatus == "Stopped" && !starting)
            {
                setLabel(Brushes.Red, "STOPPED");
                foreach (PluginDescription pd in pluginList)
                {
                    if (pd.Enabled)
                        pd.Status = "Stopped";
                }
                setButton("Start", true);
                if (!clicked)
                {
                    logging.AddToLog("Service died.  Attempting to restart.", true);
                    clicked = false;
                    System.Threading.Thread.Sleep(5000);
                    Thread m_WorkerThreadStart = new Thread(new ThreadStart(this.StartService));
                    m_WorkerThreadStart.Start();
                }
                
            }
        }

        private void loadPlugins()
        {
            pluginList = new BindingList<PluginDescription>();
            List<string> osapdFiles = new List<string>();
            string[] pluginFile = Directory.GetFiles(Common.ApiPath + "\\Plugins", "*.osapd", SearchOption.AllDirectories);
            osapdFiles.AddRange(pluginFile);

            foreach (string path in osapdFiles)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    PluginDescription desc = new PluginDescription();
                    OSAEObjectManager objectManager = new OSAEObjectManager();

                    desc.Deserialize(path);
                    desc.Status = "Stopped";
                    desc.Enabled = false;
                    List<OSAEObject> objs = objectManager.GetObjectsByType(desc.Type);
                    foreach (OSAEObject o in objs)
                    {
                        if (ObjectPopertiesManager.GetObjectPropertyValue(o.Name, "Computer Name").Value == Common.ComputerName || desc.Type == o.Name)
                        {
                            desc.Name = o.Name;
                            if (o.Enabled == 1)
                                desc.Enabled = true;
                            desc.Status = o.State.Value;
                        }
                    }
                    pluginList.Add(desc);
                    logging.AddToLog("Plugin found: Name:" + desc.Name + " Desc ID: " + desc.ID, true);
                }
            }            
            dgLocalPlugins.ItemsSource = pluginList;
        }

        private void StartService()
        {
            try
            {
                System.TimeSpan ts = new TimeSpan(0, 0, 30);
                myService.Start();
                myService.WaitForStatus(ServiceControllerStatus.Running, ts);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error stopping service.  Make sure you are running Manager as Administrator");
                logging.AddToLog("Error starting service: " + ex.Message, true);
                starting = false;
            }
        }

        private void StopService()
        {
            try
            {
                System.TimeSpan ts = new TimeSpan(0, 0, 30);

                myService.Stop();
                myService.WaitForStatus(ServiceControllerStatus.Stopped, ts);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error stopping service.  Make sure you are running Manager as Administrator");
                logging.AddToLog("Error stopping service: " + ex.Message, true);
            }
        }

        private void setButton(string text, bool enabled)
        {
            btnService.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, new Action(delegate
                {
                    this.btnService.IsEnabled = enabled;
                    this.btnService.Content = text;
                }));
        }

        private void setLabel(Brush color, string text)
        {
            lbl_isRunning.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, new Action(delegate
                {
                    this.lbl_isRunning.Foreground = color;
                    this.lbl_isRunning.Content = text;
                }));
        }

        void HandleRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
            e.Handled = true;
        }

        public void OnMessageReceived(string msgType, string message, string from, DateTime timestamp)
        {
            logging.AddToLog("Message received: " + msgType + " - " + message, false);
            //this.Invoke((MethodInvoker)delegate
            //{
                switch (msgType)
                {
                    case "plugin":
                        string[] split = message.Split('|');
                        bool enabled = false;
                        if (split[1].Trim() == "True")
                        {
                            enabled = true;
                        }

                        foreach (PluginDescription plugin in pluginList)
                        {
                            if ((plugin.Type == split[5].Trim() && Common.ComputerName == split[6].Trim()) || plugin.Name == split[0].Trim())
                            {
                                plugin.Status = split[3].Trim();
                                plugin.Enabled = enabled;
                                plugin.Name = split[0].Trim();
                                if (split[4].Trim() != "")
                                    plugin.Upgrade = split[4].Trim();
                                else
                                {
                                    plugin.Upgrade = string.Empty;
                                }
                                logging.AddToLog("updated plugin: " + plugin.Name + "|" + plugin.Version + "|" + plugin.Upgrade + "|" + plugin.Status + "| " + plugin.Enabled.ToString(), true);
                                break;
                            }
                        }
                        break;
                    case "command":
                        string[] param = message.Split('|');
                        if (param[2].Trim() == Common.ComputerName)
                        {
                            Process pr = new Process();
                            pr.StartInfo.FileName = param[0].Trim();
                            pr.StartInfo.Arguments = param[1].Trim();
                            pr.Start();
                        }
                        break;
                    case "service":
                        string[] serv = message.Split('|');
                        if (serv[0] != serv[1] && txblNewVersion.Visibility == Visibility.Hidden)
                        {
                            ShowNewVersion("Version " + serv[1] + " Available");
                        }

                        break;
                }
            //});
        }
        
        private void btnService_Click(object sender, RoutedEventArgs e)
        {
            if (btnService.Content.ToString() == "Stop")
            {
                clicked = true;
            }

            setButton(btnService.Content.ToString(), false);
            if (btnService.Content.ToString() == "Stop")
            {
                //client.Close();
                setLabel(Brushes.Red, "STOPPING...");
                foreach (PluginDescription pd in pluginList)
                {
                    if(pd.Enabled)
                        pd.Status = "Stopping...";
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
                    if (pd.Enabled)
                    {
                        pd.Status = "Starting...";
                    }
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
                logging.AddToLog("Closing", true);
                Clock.Stop();
                Clock = null;
                logging.AddToLog("Timer stopped", true);
                wcfObj.Unsubscribe();
            }
            catch
            { }
        }

        void MyNotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                this.ShowInTaskbar = true;
            }
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
            this.WindowState = WindowState.Normal;
        }

        private void Menu_GUI(object sender, RoutedEventArgs e)
        {
            Process.Start(Common.ApiPath + "\\OSAE.GUI.exe");
        }

        private void hypSettings_Click(object sender, RoutedEventArgs e)
        {
            Settings s = new Settings();
            s.ShowDialog();
        }

        private void hypGUI_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Common.ApiPath + "\\OSAE.GUI.exe");
        }

        void OnChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                PluginDescription pd = (PluginDescription)dgLocalPlugins.SelectedItem;

                logging.AddToLog("checked: " + pd.Name, true);

                if (wcfObj.State == CommunicationState.Opened)
                {
                    Thread thread = new Thread(() => messageHost("plugin", "ENABLEPLUGIN|" + pd.Name + "|True"));
                    thread.Start();
                    logging.AddToLog("Sending message: " + "ENABLEPLUGIN|" + pd.Name + "|True", true);
                    if (myService.Status == ServiceControllerStatus.Running)
                    {
                        foreach (PluginDescription plugin in pluginList)
                        {
                            if (plugin.Name == pd.Name && plugin.Name != null)
                            {
                                plugin.Status = "Starting...";
                            }
                        }
                    }
                }

                OSAEObjectManager objectManager = new OSAEObjectManager();
                OSAEObject obj = objectManager.GetObjectByName(pd.Name);
                objectManager.ObjectUpdate(obj.Name, obj.Name, obj.Description, obj.Type, obj.Address, obj.Container, 1);                
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error enabling plugin: " + ex.Message, true);
            }
        }

        void OnUnchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                PluginDescription pd = (PluginDescription)dgLocalPlugins.SelectedItem;
                logging.AddToLog("unchecked: " + pd.Name, true);

                if (wcfObj.State == CommunicationState.Opened)
                {
                    Thread thread = new Thread(() => messageHost("plugin", "ENABLEPLUGIN|" + pd.Name + "|False"));
                    thread.Start();
                    logging.AddToLog("Sending message: " + "ENABLEPLUGIN|" + pd.Name + "|False", true);

                    if (myService.Status == ServiceControllerStatus.Running)
                    {
                        foreach (PluginDescription plugin in pluginList)
                        {
                            if (plugin.Name == pd.Name && plugin.Name != null)
                            {
                                plugin.Status = "Stopping...";
                            }
                        }
                    }
                }

                OSAEObjectManager objectManager = new OSAEObjectManager();
                OSAEObject obj = objectManager.GetObjectByName(pd.Name);
                objectManager.ObjectUpdate(obj.Name, obj.Name, obj.Description, obj.Type, obj.Address, obj.Container, 0);                
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error disabling plugin: " + ex.Message, true);
            }
        }

        void ShowNewVersion(string label)
        {
            if (hypNewVersion.Dispatcher.CheckAccess() == false)
            {
                hypNewVersion.Dispatcher.Invoke(
                    System.Windows.Threading.DispatcherPriority.Normal,
                    new Action(
                      delegate()
                      {
                          Run run = new Run(label);
                          hypNewVersion.Inlines.Clear();
                          hypNewVersion.Inlines.Add(run);
                          txblNewVersion.Visibility = Visibility.Visible;
                      }
                  ));
            }
            else
            {
                Run run = new Run(label);
                hypNewVersion.Inlines.Clear();
                hypNewVersion.Inlines.Add(run);
                txblNewVersion.Visibility = Visibility.Visible;
            }
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
                // Open Plugin Package 
                PluginInstallerHelper.InstallPlugin(dlg.FileName);
                loadPlugins();
            }
        }

        private void hypLog_Click(object sender, RoutedEventArgs e)
        {
            LogWindow l = new LogWindow();
            l.ShowDialog();
        }
    }
}
