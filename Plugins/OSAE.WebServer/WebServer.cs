namespace OSAE.WebServer
{
    using System;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.ServiceProcess;

    public class WebServer : OSAEPluginBase
    {
        //OSAELog
        private OSAE.General.OSAELog Log;// = new General.OSAELog();

        /// <summary>
        /// Holds the plugin name
        /// </summary>
        string pName;

        /// <summary>
        /// Standard OSA Plugin architecture see Wiki Docs
        /// </summary>
        /// <param name="method">Standard Architecture parameter see Wiki Docs</param>
        public override void ProcessCommand(OSAEMethod method)
        {
            // No commands to process
        }

        /// <summary>
        /// Standard OSA Plugin architecture see Wiki Docs
        /// </summary>
        /// <param name="method">Standard Architecture parameter see Wiki Docs</param>
        public override void RunInterface(string pluginName)
        {
            pName = pluginName;
            Log = new General.OSAELog(pName);
            OwnTypes();
        }

        public void OwnTypes()
        {
            //Added the follow to automatically own Speech Base types that have no owner.
            OSAEObjectType oType = OSAEObjectTypeManager.ObjectTypeLoad("WEB SERVER");

            if (oType.OwnedBy == "")
            {
                OSAEObjectTypeManager.ObjectTypeUpdate(oType.Name, oType.Name, oType.Description, pName, oType.BaseType, oType.Owner, oType.SysType, oType.Container, oType.HideRedundant, oType.Tooltip);
                Log.Info("Web Server Plugin took ownership of the WEB SERVER Object Type.");
            }
            else
                Log.Info("Web Server Plugin correctly owns the WEB SERVER Object Type.");
        }

        /// <summary>
        /// Standard OSA Plugin architecture see Wiki Docs
        /// </summary>
        /// <param name="method">Standard Architecture parameter see Wiki Docs</param>
        public override void Shutdown()
        {
            Log.Info("Web server is stopping!");
        }
    }
}
