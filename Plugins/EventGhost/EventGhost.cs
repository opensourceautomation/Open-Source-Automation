namespace OSAE.EventGhost
{
    using System;
    using System.Text;
    using System.Net;
    using System.Net.Sockets;

    public class EventGhost : OSAEPluginBase
    {
        /// <summary>
        /// Provides access to logging
        /// </summary>
       // Logging logging = Logging.GetLogger("EventGhost");
        private static OSAE.General.OSAELog Log;// = new General.OSAELog("EventGhost");

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
            string methodName = method.MethodName;
            string objectName = method.ObjectName;
            string combinedString = objectName + "_" + methodName;
           
            //get the called objects properties to see if there is a computer objet defined 
            OSAEObject calledObject = OSAEObjectManager.GetObjectByName(method.ObjectName);
            OSAEObjectPropertyCollection ComputerProperty = calledObject.Properties;
            OSAEObjectCollection eventGhostObjects = OSAEObjectManager.GetObjectsByType("Computer");
            OSAEObject computer = new OSAEObject();
            
            //if there are no properties defined then grab the first available Computer object     
            //logging.AddToLog("The Count of the Computer Properties is : " + ComputerProperty.Count, true);
            if (ComputerProperty.Count == 0)
            {
                         
                //get the last object in the list
                
                foreach (OSAEObject test in eventGhostObjects)
                    computer = test;

                //add log entry if mulptiple computer objects were dectected and no property was used to select which object
                if (eventGhostObjects.Count > 1)                                      
                    Log.Info("There are multiple eventghost computer objects detected.  " + computer.Name + " with an address of: " + OSAEObjectPropertyManager.GetObjectPropertyValue(computer.Name, "IPAddress") + ":" + OSAEObjectPropertyManager.GetObjectPropertyValue(computer.Name, "EventGhost Port") + " is being used to transmit the package.  Please add a property called 'Computer' with a value of the object name of the computer you wish to transmit the UDP packet to.");
            }
                // if the object type contains a property with ID "Computer" use that property to select the appropriate object
            else
            {
                OSAEObjectProperty whichComputer = OSAEObjectPropertyManager.GetObjectPropertyValue(calledObject.Name,"Computer");
                computer = eventGhostObjects.Find(whichComputer.Value);

                //check to see if a computer object was found if none print the logged property value and select another object to use
                if (computer == null){
                    foreach (OSAEObject test in eventGhostObjects)
                        computer = test;

                    if (eventGhostObjects.Count > 1)
                    {
                        Log.Info("A computer object was unable to be matched to " + whichComputer.Value + " please ensure that " + calledObject.Name + " contains a property called Computer with a value mathing the name of the appropriate computer object.");
                        Log.Info(computer.Name + " with an address of: " + computer.Address + ":" + OSAEObjectPropertyManager.GetObjectPropertyValue(computer.Name, "EventGhost Port") + " is being used to transmit the package.");
                    }
                }
            }

            //logging.AddToLog(address, true);
            //logging.AddToLog(objectName, true);
            //logging.AddToLog(combinedString, true);

            //get the server address indicated in the address for the object passed to the method
            string ipAddress = computer.Address;

            //check to make sure computer object has address, if not assume local host
            if (ipAddress.Equals("", StringComparison.Ordinal)) ipAddress = "Localhost";
            OSAEObjectProperty portAddress = OSAEObjectPropertyManager.GetObjectPropertyValue(computer.Name, "Port");
            //if no value for port use default value of 33333
            Log.Debug("The port being used is: " + portAddress.Value);
            int port = 0;
            //Log.Debug("The port being used is: " + portAddress.Value);

            if ((portAddress.Value).Equals("", StringComparison.Ordinal))
                port = 33333;
            else
            {
                port = Int32.Parse(portAddress.Value);
                Log.Debug("The port being used is: " + portAddress.Value);

            }
            //logging.AddToLog(port, true);
            //logging.AddToLog(ipAddress, true);
            IPAddress serverAddr = IPAddress.Parse(ipAddress);
            IPEndPoint endPoint = new IPEndPoint(serverAddr, port);
            Log.Debug("The ipEndPoint is:" + ipAddress + ":" + port);

            //send the updpacket
            UdpClient udp = new UdpClient();
            byte[] sendBytes = Encoding.ASCII.GetBytes(combinedString);
            udp.Send(sendBytes, sendBytes.Length, endPoint);

            
            ////if not value for port use default value of 33333
            //int port = 0;

            //if ((portAddress.Value).Equals("",StringComparison.Ordinal))
            //    port = 33333;
            //else
            //    port = Int32.Parse(portAddress.Value);

            ////logging.AddToLog(port, true);
            ////logging.AddToLog(ipAddress, true);
            //IPAddress serverAddr = IPAddress.Parse(ipAddress);
            //IPEndPoint endPoint = new IPEndPoint(serverAddr, port);

            ////send the updpacket
            //UdpClient udp = new UdpClient();
            //byte[] sendBytes = Encoding.ASCII.GetBytes(combinedString); 
            //udp.Send(sendBytes, sendBytes.Length, endPoint);          
        }

        /// <summary>
        /// OSA Plugin Interface - called on start up to allow plugin to do any tasks it needs
        /// </summary>
        /// <param name="pluginName">The name of the plugin from the system</param>
        public override void RunInterface(string pluginName)
        {
            pName = pluginName;
            Log = new General.OSAELog(pName);
            try
            {
                Log.Info("Starting EventGhost...");

                OwnTypes();

                // on startup ensure that there is at least one computer setup to receive eventghost commands
                OSAEObjectCollection computers = OSAEObjectManager.GetObjectsByType("COMPUTER");
                
                // check to make sure that the Compuer Objecttype has the needed property
                //add eventghost property to the computer object type
                // need to implement check before i use the method below
                // OSAEObjectTypeManager.ObjectTypePropertyAdd("EventGhost Port", "String", "", "Computer", false);
             
                foreach (OSAEObject acomputer in computers)
                {
                    string computerName = acomputer.Name;
                    string IPAddress = acomputer.Address;
                    string eventGhostPort = (OSAEObjectPropertyManager.GetObjectPropertyValue(computerName, "EventGhost Port").Value);

                    //if no port for property indicate that the default port of 333333 will be used
                    if (eventGhostPort.Equals("", StringComparison.Ordinal)) 
                        Log.Info("There is no port information for " + acomputer.Name + " the default port of 33333 will be used when this computer is called");

                    //check to see if computer has address, if no log that "local host will be used"
                    if ((acomputer.Address).Equals("", StringComparison.Ordinal))
                    Log.Info("There is no address information for " + acomputer.Name + "localhost will be used for this location");
                }
            }
            catch (Exception ex)
            { Log.Error("Error during RunInterface!", ex); }
        }

        public void OwnTypes()
        {
            //Added the follow to automatically own Speech Base types that have no owner.
            OSAEObjectType oType = OSAEObjectTypeManager.ObjectTypeLoad("EVENTGHOST");

            if (oType.OwnedBy == "")
            {
                OSAEObjectTypeManager.ObjectTypeUpdate(oType.Name, oType.Name, oType.Description, pName, oType.BaseType, oType.Owner, oType.SysType, oType.Container, oType.HideRedundant,oType.Tooltip);
                Log.Info("EventGhost Plugin took ownership of the EVENTGHOST Object Type.");
            }
            else
                Log.Info("EventGhost Plugin correctly owns the EVENTGHOST Object Type.");
        }

        /// <summary>
        /// OSA Plugin Interface - The plugin has been asked to shut down
        /// </summary>        
        public override void Shutdown()
        {
            Log.Info("Stopping EventGhost...");
            // Place any code required to shut down your plugin here leave empty if no action is required
        }
    }
}
