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
    using NetworkCommsDotNet;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon MyNotifyIcon;
        ServiceController myService = new ServiceController();

        //OSAELog
        private OSAE.General.OSAELog Log = new OSAE.General.OSAELog();

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

            try
            {
                this.Log.Debug("Starting UDP listener");
                NetworkComms.AppendGlobalIncomingPacketHandler<string>("Commmand", CommandMessageReceived);
                //Start listening for incoming UDP data
                UDPConnection.StartListening(true);
                this.Log.Debug("UPD Listener started");
            }
            catch (Exception ex)
            {
                this.Log.Error("Error starting listener", ex);
            }
            
        }
        
        private void CheckService(object sender, EventArgs e)
        {            
            string svcStatus = myService.Status.ToString();

            if (svcStatus == "Running")
            {
                setLabel(Brushes.Green, "RUNNING");
                setButton("Stop", true);
                starting = false;
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, new Action(delegate
                {
                    mnuInstall.IsEnabled = true;
                    mnuInstall.ToolTip = "You are clear to Install Plugins";
                }));
            }
            else if (svcStatus == "Stopped" && !starting)
            {
                setLabel(Brushes.Red, "STOPPED");
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, new Action(delegate
                {
                    mnuInstall.IsEnabled = false;
                    mnuInstall.ToolTip = "Service Must be Running";
                }));

                foreach (PluginDescription pd in pluginList)
                {
                    if (pd.Enabled)
                        pd.Status = "Stopped";
                }
                setButton("Start", true);
                if (!clicked)
                {
                    this.Log.Info("Service died.  Attempting to restart.");
                    clicked = false;
                    System.Threading.Thread.Sleep(5000);
                    Thread m_WorkerThreadStart = new Thread(new ThreadStart(this.StartService));
                    m_WorkerThreadStart.Start();
                }
                
            }
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
                this.Log.Error("Error starting service", ex);
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
                this.Log.Error("Error stopping service", ex);
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
                this.Log.Info("Manager Closing");
                Clock.Stop();
                Clock = null;
                this.Log.Info("Timer stopped");
                NetworkComms.Shutdown();
            }
            catch(Exception ex)
            {
                this.Log.Error("Error closing Manager", ex);
            }
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
            Process.Start("http://localhost:8081/default.aspx");
        }

        private void hypGUI_Click(object sender, RoutedEventArgs e)
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
                this.Log.Info("Plugin file selected: " + dlg.FileName + ".  Installing...");
                // Open Plugin Package 
                PluginInstallerHelper pInst = new PluginInstallerHelper();
                pInst.InstallPlugin(dlg.FileName);
            }
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
                this.Log.Info("CMDLINE received: " + param[0].Trim() + " - " + param[1].Trim());
                Process pr = new Process();
                pr.StartInfo.FileName = param[0].Trim();
                pr.StartInfo.Arguments = param[1].Trim();
                pr.Start();
            }
        }
    }
}
