namespace OSAE.Rest
{
    using OSAERest;
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Description;
    using System.ServiceModel.Web;
    using System.Security.Cryptography;
    using System.Web;

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Rest : OSAEPluginBase
    {
        //OSAELog
        private OSAE.General.OSAELog Log;// = new General.OSAELog();

        /// <summary>
        /// Hosts the web service
        /// </summary>
        private WebServiceHost serviceHost = null;

        /// <summary>
        /// Plugin name
        /// </summary>
        string pName;

        /// <summary>
        /// OSA Plugin Interface - Commands the be processed by the plugin
        /// </summary>
        /// <param name="method">Method containging the command to run</param>
        public override void ProcessCommand(OSAEMethod method)
        {
            Log.Info("Processing Method: " + method.MethodLabel);
            //No commands to process
            if(method.MethodLabel == "GenerateApiKey")
            {
                OSAEObjectPropertyManager.ObjectPropertySet("Rest", "APIKEY", OSAESecurity.GenerateAPIKey(), "Rest");
                Log.Debug("New APIKEY Generated");
            }
            if (method.MethodLabel == "GenerateSecurityKey")
            {
                OSAEObjectPropertyManager.ObjectPropertySet("System", "SecurityKey", OSAESecurity.GenerateSecurityKey(), "Rest");
                Log.Debug("New SecurityKey Generated");
            }
            if (method.MethodLabel == "GenerateCurrentAuthKey")
            {
                Log.Info("currentAPIAuthKey: " + OSAESecurity.generateCurrentAuthKey(method.Parameter1));
            }
        }

        /// <summary>
        /// OSA Plugin Interface - called on start up to allow plugin to do any tasks it needs
        /// </summary>
        /// <param name="pluginName">The name of the plugin from the system</param>
        public override void RunInterface(string pluginName)
        {
            pName = pluginName;
            Log = new General.OSAELog(pName);
            OwnTypes();
            try
            {
                this.Log.Info("Starting Rest Interface");

                bool showHelp = bool.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Show Help").Value);
                int restPort = 8732;

                if (!OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "REST Port").Id.Equals(String.Empty))
                {
                    try
                    {
                        restPort = int.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "REST Port").Value);
                        Log.Info("Rest Port read in as: " + restPort);
                    }
                    catch (FormatException ex)
                    { Log.Error("Error pulling REST port from property (not a valid number)", ex); }
                    catch (OverflowException ex)
                    { Log.Error("Error pulling REST port from property (too large)", ex); }
                    catch (ArgumentNullException ex)
                    { Log.Error("Error pulling REST port from property (null)", ex); }
                }

                String restUrl = "http://localhost:"+restPort.ToString()+"/api";
                serviceHost = new WebServiceHost(typeof(OSAERest.api), new Uri(restUrl));
                
                WebHttpBinding binding = new WebHttpBinding(WebHttpSecurityMode.None); 
                binding.CrossDomainScriptAccessEnabled = true;
                
                var endpoint = serviceHost.AddServiceEndpoint(typeof(IRestService), binding, "");

                ServiceDebugBehavior sdb = serviceHost.Description.Behaviors.Find<ServiceDebugBehavior>();
                sdb.HttpHelpPageEnabled = false;
                if (showHelp) serviceHost.Description.Endpoints[0].Behaviors.Add(new WebHttpBehavior { HelpEnabled = true });


                checkAPIKey();
                checkSecurityKey();
                serviceHost.Open();                                
            }
            catch (Exception ex)
            { Log.Error("Error starting RESTful web service", ex); }
        }

        public void OwnTypes()
        {
            //Added the follow to automatically own Speech Base types that have no owner.
            OSAEObjectType oType = OSAEObjectTypeManager.ObjectTypeLoad("REST");

            if (oType.OwnedBy == "")
            {
                OSAEObjectTypeManager.ObjectTypeUpdate(oType.Name, oType.Name, oType.Description, pName, oType.BaseType, oType.Owner, oType.SysType, oType.Container, oType.HideRedundant, oType.Tooltip);
                Log.Info("Jabber Plugin took ownership of the REST Object Type.");
            }
            else
                Log.Info("Jabber Plugin correctly owns the REST Object Type.");
        }

        /// <summary>
        /// OSA Plugin Interface - The plugin has been asked to shut down
        /// </summary>        
        public override void Shutdown()
        {
            serviceHost.Close();
        }

        private void checkAPIKey()
        {
            try
            {
                Log.Info("Checking if REST API Key exist");
                string cRAK = OSAE.OSAEObjectPropertyManager.GetObjectPropertyValue("Rest", "APIKEY").Value;
                if (cRAK == " " || cRAK == "" || cRAK.Length <32)
                {
                    Log.Info("REST API Key found, but is not valid.");
                    Log.Info("Creating a new REST API Key...");
                    OSAEObjectPropertyManager.ObjectPropertySet("Rest", "APIKEY", OSAESecurity.GenerateAPIKey(), "Rest");
                }
                else
                {
                    Log.Info("REST API Key found.");
                }
            }
            catch
            {
                Log.Info("REST API Key does not exist..");
                Log.Info("Creating Rest Plugin property: APIKEY.");
                OSAEObjectTypeManager.ObjectTypePropertyAdd("Rest", "APIKEY", "String", "", "", false, true, "This is the Rest Plugin API Key. This property is used by remote clients and devices to encrypt and decrypt user information for authentication.");
                Log.Info("Generating a new REST API Key...");
                OSAEObjectPropertyManager.ObjectPropertySet("Rest", "APIKEY", OSAESecurity.GenerateAPIKey(), "Rest");
            }
        }

        private void checkSecurityKey()
        {
            try
            {
                Log.Info("Checking if System Security Key exist");
                string cSSK = OSAE.OSAEObjectPropertyManager.GetObjectPropertyValue("System", "SecurityKey").Value;
                if (cSSK == " " || cSSK == "" || cSSK.Length < 16)
                {
                    Log.Info("System Security Key found, but is not valid.");
                    Log.Info("Generating a new Security Key...");
                    OSAEObjectPropertyManager.ObjectPropertySet("System", "SecurityKey", OSAESecurity.GenerateSecurityKey(), "Rest");
                }
                else
                {
                    Log.Info("System Security Key found.");
                }
            }
            catch
            {
                Log.Info("System Security Key does not exist..");
                Log.Info("Creating System object property: SecurityKey.");
                OSAEObjectTypeManager.ObjectTypePropertyAdd("System", "SecurityKey", "String", "", "", false, true, "This key is used in HTTP and Rest authorization. Must be 16 characters.");
                Log.Info("Generating a new Security Key...");
                OSAEObjectPropertyManager.ObjectPropertySet("System", "SecurityKey", OSAESecurity.GenerateSecurityKey(), "Rest");
            }
        }
    }
}
