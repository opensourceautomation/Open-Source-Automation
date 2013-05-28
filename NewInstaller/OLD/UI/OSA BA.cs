namespace OSA.Bootstrapper
{
    using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
    using System;
    using System.Windows;
    using System.Windows.Threading;

    public class OSABA : BootstrapperApplication
    {

        // global dispatcher
        static public Dispatcher BootstrapperDispatcher { get; private set; }

        // entry point for our custom UI
        protected override void Run()
        {
            try
            {
                this.Engine.Log(LogLevel.Verbose, "Launching OSA BA UX");
                BootstrapperDispatcher = Dispatcher.CurrentDispatcher;

                //Form1 f = new Form1(this);
                //f.ShowDialog();
                MainViewModel viewModel = new MainViewModel(this);
                this.Engine.Detect();

                MainView view = new MainView();

                view.DataContext = viewModel;
                view.Closed += (sender, e) => BootstrapperDispatcher.InvokeShutdown();
                view.Show();

                Dispatcher.Run();
            }
            catch (Exception ex)
            {
                this.Engine.Log(LogLevel.Verbose, "Exception Caught by BA details:" + ex.Message);
                // change to use engine log
                MessageBox.Show("Exception" + ex.Message);
            }

            this.Engine.Quit(0);
        }
    }
}
