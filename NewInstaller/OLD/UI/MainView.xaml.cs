namespace OSA.Bootstrapper
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Description for MainView.
    /// </summary>
    public partial class MainView : Window
    {      
        Welcome welcomeView = new Welcome();
        MySQLOptions mySqlView = new MySQLOptions();
        InstallDirectory installDir = new InstallDirectory();
        OSAInstallType it = new OSAInstallType();
        Install installControl;

        /// <summary>
        /// Initializes a new instance of the MainView class.
        /// </summary>
        public MainView()
        {
            InitializeComponent();
            installControl = new Install((MainViewModel)this.DataContext);
            controlContent.Content = welcomeView;
            //this.BackButton.Visibility = System.Windows.Visibility.Collapsed;
            
            //view.Closed += (sender, e) => BootstrapperDispatcher.InvokeShutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (((UserControl)controlContent.Content).Name)
            {
                case "WelcomeControl" :
                    controlContent.Content = it;
                    this.BackButton.IsEnabled = true;
                    break;
                case "installType":
                    controlContent.Content = installDir;
                    break;
                case "installDir" :
                    controlContent.Content = mySqlView;
                    break;
                case "MySqlOptionsControl":
                    controlContent.Content = installControl;
                    Next.IsEnabled = false;
                    ((MainViewModel)this.DataContext).InstallEnabled = true;
                    break;
            }        
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            switch (((UserControl)controlContent.Content).Name)
            {
                case "WelcomeControl":
                    controlContent.Content = it;
                    break;
                case "installType":
                    this.BackButton.IsEnabled = false;
                    controlContent.Content = welcomeView;
                    break;
                case "installDir":
                    controlContent.Content = it;
                    break;
                case "MySqlOptionsControl":
                    controlContent.Content = installDir;                   
                    break;
                case "installControl":
                    controlContent.Content = mySqlView;
                    ((MainViewModel)this.DataContext).InstallEnabled = false;
                    Next.IsEnabled = true;
                    break;
            }  
        }  
    }
}