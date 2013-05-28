using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.ComponentModel;

namespace OSA.Bootstrapper
{

    public class MainViewModel : PropertyNotifyBase
    {
        private int progressPhases;
        private int progress;
        private int cacheProgress;
        private int executeProgress;
        private string message = "Starting";

        public string Message
        {
            get
            {
                return this.message;
            }

            set
            {
                //if (this.message != value)
                {
                    this.Bootstrapper.Engine.Log(LogLevel.Standard, "Message Property : " + value);
                    this.message = value;
                    this.OnPropertyChanged("Message");
                }
            }
        }

        //constructor
        public MainViewModel(BootstrapperApplication bootstrapper)
        {
            
            this.IsThinking = false;
            
            this.Bootstrapper = bootstrapper;
            this.Bootstrapper.ExecuteMsiMessage += this.ExecuteMsiMessage;
            this.Bootstrapper.ApplyComplete += this.OnApplyComplete;
            this.Bootstrapper.DetectPackageComplete += this.OnDetectPackageComplete;
            this.Bootstrapper.PlanComplete += this.OnPlanComplete;
            this.Bootstrapper.CacheAcquireProgress += this.CacheAcquireProgress;
            this.Bootstrapper.ExecuteProgress += this.ApplyExecuteProgress;
           
            
        }

        #region Properties

        public int Progress
        {
            get
            {
                return this.progress;
            }

            set
            {
                this.Bootstrapper.Engine.Log(LogLevel.Standard, "Progress Setter");
                if (this.progress != value)
                {
                    this.progress = value;
                    this.OnPropertyChanged("Progress");
                    
                    //base.OnPropertyChanged("Progress");
                }
            }
        }

        private bool installEnabled;
        public bool InstallEnabled
        {
            get { return installEnabled; }
            set
            {
                installEnabled = value;
                this.OnPropertyChanged("InstallEnabled");
            }
        }

        private bool uninstallEnabled;
        public bool UninstallEnabled
        {
            get { return uninstallEnabled; }
            set
            {
                uninstallEnabled = value;
                this.OnPropertyChanged("UninstallEnabled");
            }
        }

        private bool isThinking;
        public bool IsThinking
        {
            get { return isThinking; }
            set
            {
                isThinking = value;
                this.OnPropertyChanged("IsThinking");
            }
        }
        
        private string packageOrContainerId;
        public string PackageOrContainerId
        {
            get { return packageOrContainerId; }
            set
            {
                packageOrContainerId = value;
                this.OnPropertyChanged("PackageOrContainerId");
            }
        }

        private string currentAction = "Calculating";
        public string CurrentAction
        {
            get { return currentAction; }
            set
            {
                currentAction = value;
                this.OnPropertyChanged("CurrentAction");
            }
        }



        public BootstrapperApplication Bootstrapper { get; private set; }

        #endregion //Properties

        #region Methods

        private void ExecuteMsiMessage(object sender, ExecuteMsiMessageEventArgs e)
        {
            this.Bootstrapper.Engine.Log(LogLevel.Standard, "WPF ExecuteMsiMessage: " + e.Message);
            lock (this)
            {
                this.Message = e.Message;
                //e.Result = this.root.Canceled ? Result.Cancel : Result.Ok;
            }
        }

        private void CacheAcquireProgress(object sender, CacheAcquireProgressEventArgs e)
        {
            this.Bootstrapper.Engine.Log(LogLevel.Standard, "WPF CacheAcquireProgress");
            lock (this)
            {
                this.PackageOrContainerId = e.PackageOrContainerId;
                this.cacheProgress = e.OverallPercentage;
                this.Progress = (this.cacheProgress + this.executeProgress) / this.progressPhases;
                //e.Result = this.root.Canceled ? Result.Cancel : Result.Ok;
            }
        }

        private void InstallExecute()
        {
            IsThinking = true;
            Bootstrapper.Engine.Plan(LaunchAction.Install);
        }

        private void UninstallExecute()
        {
            IsThinking = true;
            Bootstrapper.Engine.Plan(LaunchAction.Uninstall);
        }

        private void ExitExecute()
        {
            OSABA.BootstrapperDispatcher.InvokeShutdown();
        }

        /// <summary>
        /// Method that gets invoked when the Bootstrapper ApplyComplete event is fired.
        /// This is called after a bundle installation has completed. Make sure we updated the view.
        /// </summary>
        private void OnApplyComplete(object sender, ApplyCompleteEventArgs e)
        {
            IsThinking = false;
            InstallEnabled = false;
            UninstallEnabled = false;
        }

        private void ApplyExecuteProgress(object sender, ExecuteProgressEventArgs e)
        {
            lock (this)
            {

                this.executeProgress = e.OverallPercentage;
                this.Progress = (this.cacheProgress + this.executeProgress) / 2; // always two phases if we hit execution.
               
                this.Bootstrapper.Engine.SendEmbeddedProgress(e.ProgressPercentage, this.Progress);
                this.Bootstrapper.Engine.Log(LogLevel.Standard, "ApplyExecuteProgress");
                //e.Result = this.root.Canceled ? Result.Cancel : Result.Ok;
            }
        }

        /// <summary>
        /// Method that gets invoked when the Bootstrapper DetectPackageComplete event is fired.
        /// Checks the PackageId and sets the installation scenario. The PackageId is the ID
        /// specified in one of the package elements (msipackage, exepackage, msppackage,
        /// msupackage) in the WiX bundle.
        /// </summary>
        private void OnDetectPackageComplete(object sender, DetectPackageCompleteEventArgs e)
        {
            if (e.PackageId == "OSAInstallerPackageId")
            {
                if (e.State == PackageState.Absent)
                    InstallEnabled = true;

                else if (e.State == PackageState.Present)
                    UninstallEnabled = true;
            }
        }

        /// <summary>
        /// Method that gets invoked when the Bootstrapper PlanComplete event is fired.
        /// If the planning was successful, it instructs the Bootstrapper Engine to 
        /// install the packages.
        /// </summary>
        private void OnPlanComplete(object sender, PlanCompleteEventArgs e)
        {
            if (e.Status >= 0)
                CurrentAction = "Planning Install";
                Bootstrapper.Engine.Apply(System.IntPtr.Zero);
        }

        #endregion //Methods

        #region RelayCommands

        private RelayCommand installCommand;
        public RelayCommand InstallCommand
        {
            get
            {
                if (installCommand == null)
                    installCommand = new RelayCommand(() => InstallExecute(), () => InstallEnabled == true);

                return installCommand;
            }
        }

        private RelayCommand uninstallCommand;
        public RelayCommand UninstallCommand
        {
            get
            {
                if (uninstallCommand == null)
                    uninstallCommand = new RelayCommand(() => UninstallExecute(), () => UninstallEnabled == true);

                return uninstallCommand;
            }
        }

        private RelayCommand exitCommand;
        public RelayCommand ExitCommand
        {
            get
            {
                if (exitCommand == null)
                    exitCommand = new RelayCommand(() => ExitExecute());

                return exitCommand;
            }
        }
        
        #endregion //RelayCommands

        
    }
}