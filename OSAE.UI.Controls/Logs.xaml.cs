namespace OSAE.UI.Controls
{
    using System;
    using System.Data;
    using System.Windows.Controls;
    using System.Windows.Threading;
    
    /// <summary>
    /// Interaction logic for the logs control
    /// </summary>
    public partial class Logs : UserControl
    {        
        Logging logging = Logging.GetLogger();

        /// <summary>
        /// Timer to allow reload of log data after given period
        /// </summary>
        private DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();  

        /// <summary>
        /// Initializes the control
        /// </summary>
        public Logs()
        {
            this.InitializeComponent();
            this.dispatcherTimer.Tick += new EventHandler(this.dispatcherTimer_Tick);
            this.dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
            this.dispatcherTimer.Start();
            this.LoadLoags();
        }

        /// <summary>
        /// Load the logs from the DB and updates the DataGrid
        /// </summary>
        public void LoadLoags()
        {
            DataSet dataSet = OSAESql.RunSQL("SELECT log_time,object_name,event_label,parameter_1,parameter_2,from_object_name FROM osae_v_event_log ORDER BY log_time DESC, object_name LIMIT 1000");
            this.logDataGrid.ItemsSource = dataSet.Tables[0].DefaultView;
        }

        /// <summary>
        /// Mehod called when the timer fires
        /// </summary>
        /// <param name="sender">Standard sender argument see MSDN</param>
        /// <param name="e">Standard EventArgs argument see MSDN</param>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            this.LoadLoags();
        }        

        /// <summary>
        /// Clear out the event log
        /// </summary>
        /// <param name="sender">Standard sender argument see MSDN</param>
        /// <param name="e">Standard RoutedEventArgs argument see MSDN</param>
        private void clearLogButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.logging.EventLogClear();
            this.LoadLoags();
        }

        /// <summary>
        /// Start the timer going when the mouse enters the data grid
        /// </summary>
        /// <param name="sender">Standard sender argument see MSDN</param>
        /// <param name="e">Standard MouseEventArgs argument see MSDN</param>
        private void logDataGrid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.dispatcherTimer.Start();
        }

        /// <summary>
        /// Stops the time when the mouse leaves the data grid
        /// </summary>
        /// <param name="sender">Standard sender argument see MSDN</param>
        /// <param name="e">Standard MouseEventArgs argument see MSDN</param>
        private void logDataGrid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.dispatcherTimer.Stop();
        }
    }
}
