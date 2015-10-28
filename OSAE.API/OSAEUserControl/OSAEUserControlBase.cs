namespace OSAE
{
    using System;

    /// <summary>
    /// Defines the interface common to all untrusted user controls.
    /// </summary>
    public abstract class OSAEUserControlBase : MarshalByRefObject
    {

        /// <summary>
        /// This is used to return an instance of Add UserControl Screen.
        /// NO alteration or additions are required.
        /// This must be implemented.
        /// </summary>
        System.Windows.Controls.UserControl CtrlInterface { set; get; }

        //// <summary>
        /// This is used to return an instance of the actual UserControl.
        /// NO alteration or additions are required.
        /// This must be implemented.
        /// </summary>
        System.Windows.Controls.UserControl mainCtrl { set; get; }

        /// <summary>
        /// This method is involked to initialize the Add UserControl screen before displaying.
        /// NO alteration or additions are required.
        /// This must be implemented.
        /// </summary>
        public abstract void InitializeAddCtrl(String screen);

        /// <summary>
        /// This method is involked to initialize the actual UserControl screen before displaying.
        /// NO alteration or additions are required.
        /// This must be implemented.
        /// </summary>
        public abstract void InitializeMainCtrl(OSAE.OSAEObject obj);

        /// <summary>
        /// This method is invoked to clean up any unused variables or perform closing actions. 
        /// and innitiate any long running tasks. Not all plugins will need to use this method
        /// however they must implement it.
        /// </summary>
        public abstract void Dispose();
    }
}