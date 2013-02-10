namespace LogViewer
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using OSAE;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Standard event don't interfere with it if we don't have to
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();                
        }

        /// <summary>
        /// See what logs are available and add them to the list box
        /// </summary>
        private void GetLogs()
        {
            object previouslySelected = null;

            if (this.logsListBox.SelectedItem != null)
            {
                previouslySelected = this.logsListBox.SelectedItem;
            }

            this.logsListBox.Items.Clear();

            if (Directory.Exists(Common.ApiPath + @"\Logs"))
            {
                string[] fileList = Directory.GetFiles(Common.ApiPath + @"\Logs");

                foreach (string file in fileList)
                {
                    this.logsListBox.Items.Add(Path.GetFileNameWithoutExtension(file));
                }
            }
            else
            {
                this.fileContentTextBox.Text = "Log directory doesn't exist - looking for logs in: " + Common.ApiPath + @"\Logs";
            }

            if (previouslySelected != null)
            {
                if (this.logsListBox.Items.Contains(previouslySelected))
                {
                    this.logsListBox.SelectedItem = previouslySelected;
                }
            }
        }

        /// <summary>
        /// User has selected a different log load the contentx into the text box
        /// </summary>
        /// <param name="sender">Standard object parameter see MSDN for details</param>
        /// <param name="e">Standard SelectionChangedEventArgs parameter see MSDN for details</param>
        private void LogsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.LoadLogContent();
        }

        /// <summary>
        /// Loads the content of the log
        /// </summary>
        private void LoadLogContent()
        {
            if (this.logsListBox.SelectedItem == null)
            {                
                return;
            }

            this.fileContentTextBox.Text = string.Empty;
            try
            {
                this.fileContentTextBox.Text = File.ReadAllText(Common.ApiPath + @"\Logs\" + this.logsListBox.SelectedItem.ToString() + ".log");

                // TODO Set scroll position to end of log content

                // TODO create a file system watcher
                //CreateFileWatcher(osae.APIpath + @"\Logs\" + logsListBox.SelectedItem.ToString() + ".log");
            }
            catch (Exception ex)
            {
                this.fileContentTextBox.Text = "Failed to read the log file detail : \r\n\r\n" + ex.Message;
            }
        }

        /// <summary>
        /// The window has finished loading go and load in the available logs
        /// </summary>
        /// <param name="sender">Standard object parameter see MSDN for details</param>
        /// <param name="e">Standard RoutedEventArgs parameter see MSDN for details</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.GetLogs();
        }

        /// <summary>
        /// Create a file watcher so we can update the file when new entries appear in the log
        /// </summary>
        /// <param name="path">The file to watch</param>
        private void CreateFileWatcher(string path)
        {
            // Create a new FileSystemWatcher and set its properties. 
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = System.IO.Path.GetDirectoryName(path);             
            
            /* Watch for changes in LastAccess and LastWrite times, and  
               the renaming of files or directories. */
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = System.IO.Path.GetFileName(path);            

            // Add event handlers. 
            watcher.Changed += new FileSystemEventHandler(OnChanged);

            // Begin watching. 
            watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// The contents of the file has changed
        /// </summary>
        /// <param name="source">Standard object parameter see MSDN for details</param>
        /// <param name="e">Standard FileSystemEventArgs parameter see MSDN for details</param>
        private static void OnChanged(object source, FileSystemEventArgs e)
        {            
            // fileContentTextBox.Text = File.ReadAllText(osae.APIpath + @"\Logs\" + logsListBox.SelectedItem.ToString() + ".log");
        }

        /// <summary>
        /// Check to see if there are any new logs files
        /// </summary>
        /// <param name="sender">Standard object parameter see MSDN for details</param>
        /// <param name="e">Standard RoutedEventArgs parameter see MSDN for details</param>
        private void CheckLogsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.GetLogs();            
        }

        /// <summary>
        /// Reload the content of the log
        /// </summary>
        /// <param name="sender">Standard object parameter see MSDN for details</param>
        /// <param name="e">Standard RoutedEventArgs parameter see MSDN for details</param>
        private void LoadLogMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.LoadLogContent();
        } 

    }
}
