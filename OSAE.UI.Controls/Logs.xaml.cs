namespace OSAE.UI.Controls
{
    using System;
    using System.Data;
    using System.Windows.Controls;
    using System.Windows.Threading;
    
    /// <summary>
    /// Interaction logic for Logs.xaml
    /// </summary>
    public partial class Logs : UserControl
    {        
        /// <summary>
        /// OSAE API to interact with OSA DB
        /// </summary>
        OSAE osae = new OSAE("OSAE.UI.Controls");

        /// <summary>
        /// Timer to allow reload of log data after given period
        /// </summary>
        DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();  

        /// <summary>
        /// Initializes the control
        /// </summary>
        public Logs()
        {
            InitializeComponent();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
            dispatcherTimer.Start(); 
            LoadLoags();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            LoadLoags();
        } 

        /// <summary>
        /// Load the logs from the DB and updates the DataGrid
        /// </summary>
        public void LoadLoags()
        {
            DataSet dataSet = osae.RunSQL("SELECT log_time,object_name,event_label,parameter_1,parameter_2,from_object_name FROM osae_v_event_log ORDER BY log_time DESC, object_name LIMIT 1000");
            logDataGrid.ItemsSource = dataSet.Tables[0].DefaultView;            
        }

        /// <summary>
        /// Clear out the event log
        /// </summary>
        /// <param name="sender">Standard sender argument see MSDN</param>
        /// <param name="e">Standard RoutedEventArgs argument see MSDN</param>
        private void clearLogButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            osae.EventLogClear();
            LoadLoags();
        }

        private void logDataGrid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            dispatcherTimer.Start();
        }

        private void logDataGrid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            dispatcherTimer.Stop();
        }
    }
}
