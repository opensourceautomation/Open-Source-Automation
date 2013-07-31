namespace OSAE.Zwave
{
    using System;
    using System.Collections.Generic;
    using OpenZWaveDotNet;
    using OSAE;

    public class Zwave : OSAEPluginBase
    {
        static private Logging logging = Logging.GetLogger("ZWave");
        static private ManagedControllerStateChangedHandler m_controllerStateChangedHandler = new ManagedControllerStateChangedHandler(Zwave.MyControllerStateChangedHandler);
        static private ZWManager m_manager = null;
        ZWOptions m_options = null;
        UInt32 m_homeId = 0;
        ZWNotification m_notification = null;
        List<Node> m_nodeList = new List<Node>();
        string pName;

        public override void RunInterface(string pluginName)
        {
            pName = pluginName;
            int poll = 60;
            if (OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Polling Interval").Value != string.Empty)
                poll = Int32.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Polling Interval").Value);

            string port = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Port").Value;

            logging.AddToLog("Port: " + port, true);
            try
            {
                if (port != "")
                {
                    // Create the Options
                    m_options = new ZWOptions();
                    m_options.Create(Common.ApiPath + @"\Plugins\ZWave\config\", Common.ApiPath + @"\Plugins\ZWave\", @"");

                    // Add any app specific options here...
                    m_options.AddOptionBool("ConsoleOutput", false);
                    m_options.AddOptionBool("IntervalBetweenPolls", true);
                    m_options.AddOptionInt("PollInterval", poll);


                    // Lock the options
                    m_options.Lock();

                    // Create the OpenZWave Manager
                    m_manager = new ZWManager();
                    m_manager.Create();
                    m_manager.OnNotification += new ManagedNotificationsHandler(NotificationHandler);

                    // Add a driver
                    m_manager.AddDriver(@"\\.\COM" + port);

                    //logging.AddToLog("Setting poll interval: " + poll.ToString(), true);
                    //m_manager.SetPollInterval(poll);
                    logging.AddToLog(Common.ApiPath + @"\Plugins\ZWave\Config", true);
                    logging.AddToLog("Zwave plugin initialized", true);
                }

            }
            catch (Exception ex)
            {
                logging.AddToLog("Error initalizing plugin: " + ex.Message, true);
            }
        }

        public override void ProcessCommand(OSAEMethod method)
        {
            logging.AddToLog("Found Command: " + method.MethodName + " | param1: " + method.Parameter1 + " | param2: " + method.Parameter2 + " | obj: " + method.ObjectName + " | addr: " + method.Address , false);
            //process command
            try
            {
                if (method.Address.Length > 0)
                {
                    
                    int address;
                    byte instance = 0;
                    byte nid;
                    if (int.TryParse(method.Address.Substring(1), out address))
                    {
                        nid = (byte)address;
                    }
                    else
                    {
                        nid = (byte)Int32.Parse(method.Address.Substring(1).Split('-')[0]);
                        instance = (byte)Int32.Parse(method.Address.Substring(1).Split('-')[1]);
                    }
                    Node node = GetNode(m_homeId, nid);
                    OSAEObject obj = OSAEObjectManager.GetObjectByAddress("Z" + nid.ToString());

                    if (method.MethodName == "NODE NEIGHBOR UPDATE")
                    {
                        logging.AddToLog("Requesting Node Neighbor Update: " + obj.Name, true);
                        m_manager.OnControllerStateChanged += m_controllerStateChangedHandler;
                        if (!m_manager.BeginControllerCommand(m_homeId, ZWControllerCommand.RequestNodeNeighborUpdate, false, nid))
                        {
                            logging.AddToLog("Request Node Neighbor Update Failed: " + obj.Name, true);
                            m_manager.OnControllerStateChanged -= m_controllerStateChangedHandler;
                        }
                    }
                    else if (method.MethodName == "ENABLE POLLING")
                        enablePolling(nid);
                    else if(method.MethodName == "ON")
                    {
                        int val = 255;
                        if (method.Parameter1 != "")
                            val = Int32.Parse(method.Parameter1);
                        Value v = new Value();
                        foreach (Value value in node.Values)
                        {
                            if ((obj.BaseType == "BINARY SWITCH" && value.Label == "Switch") || obj.BaseType == "MULTILEVEL SWITCH" && value.Label == "Level")
                            {
                                v = value;
                            }
                        }

                        //m_manager.SetNodeOn(m_homeId, nid);

                        if(OSAEObjectManager.GetObjectByAddress("Z" + nid.ToString()).BaseType == "BINARY SWITCH")
                            m_manager.SetValue(v.ValueID, true);
                        else if(OSAEObjectManager.GetObjectByAddress("Z" + nid.ToString()).BaseType == "MULTILEVEL SWITCH")
                            m_manager.SetValue(v.ValueID, (byte)val);

                        OSAEObjectStateManager.ObjectStateSet(method.ObjectName, "ON", pName);
                        logging.AddToLog("Turned on: " + method.ObjectName, false);
                    }
                    else if(method.MethodName == "OFF")
                    {
                        Value v = new Value();
                        foreach (Value value in node.Values)
                        {
                            if ((obj.BaseType == "BINARY SWITCH" && value.Label == "Switch") || obj.BaseType == "MULTILEVEL SWITCH" && value.Label == "Level")
                            {
                                v = value;
                            }
                        }

                        if (OSAEObjectManager.GetObjectByAddress("Z" + nid.ToString()).BaseType == "BINARY SWITCH")
                            m_manager.SetValue(v.ValueID, false);
                        else if (OSAEObjectManager.GetObjectByAddress("Z" + nid.ToString()).BaseType == "MULTILEVEL SWITCH")
                            m_manager.SetValue(v.ValueID, (byte)0);

                        //m_manager.SetNodeOff(m_homeId, nid);
                        OSAEObjectStateManager.ObjectStateSet(method.ObjectName, "OFF", pName);
                        logging.AddToLog("Turned off: " + method.ObjectName, false);
                    }
                    else
                    {
                        foreach (Value value in node.Values)
                        {
                            if (value.Label == method.MethodName)
                            {
                                if (method.Parameter1 != "")
                                {
                                    if (value.Type == ZWValueID.ValueType.String)
                                        m_manager.SetValue(value.ValueID, method.Parameter1);
                                    else if (value.Type == ZWValueID.ValueType.List)
                                        m_manager.SetValueListSelection(value.ValueID, method.Parameter1);
                                    else if (value.Type == ZWValueID.ValueType.Int)
                                        m_manager.SetValue(value.ValueID, Int32.Parse(method.Parameter1));
                                    else if (value.Type == ZWValueID.ValueType.Byte)
                                        m_manager.SetValue(value.ValueID, (byte)Int32.Parse(method.Parameter1));
                                    else if (value.Type == ZWValueID.ValueType.Bool)
                                    {
                                        if (method.Parameter1 == "TRUE")
                                            m_manager.SetValue(value.ValueID, true);
                                        else
                                            m_manager.SetValue(value.ValueID, false);
                                    }
                                    else if (value.Type == ZWValueID.ValueType.Decimal)
                                        m_manager.SetValue(value.ValueID, Convert.ToSingle(method.Parameter1));

                                    logging.AddToLog("Set " + method.MethodName + " to " + method.Parameter1 + ": " + method.ObjectName, false);
                                }
                                else if(value.Type == ZWValueID.ValueType.Button)
                                {
                                    if (value.Label == method.MethodName)
                                    {
                                        m_manager.PressButton(value.ValueID);
                                        m_manager.ReleaseButton(value.ValueID);
                                    }
                                }
                            }
                        }
                        
                    }
                }
                else
                {
                    #region Controller Commands
                    try
                    {
                        byte nid = 0xff;
                        if (method.Parameter1 != "")
                            nid = (byte)Int32.Parse(method.Parameter1.Substring(1));

                        switch (method.MethodName)
                        {
                            //case "ADD CONTROLLER":
                            //    m_manager.OnControllerStateChanged += m_controllerStateChangedHandler;
                            //    if (!m_manager.BeginControllerCommand(m_homeId, ZWControllerCommand.AddController, false, nid))
                            //    {
                            //        logging.AddToLog("Add Controller Failed", true);
                            //        m_manager.OnControllerStateChanged -= m_controllerStateChangedHandler;
                            //    }
                            //    //osae.MethodQueueAdd(osae.GetPluginName("GUI CLIENT", osae.ComputerName), "POPUP MESSAGE", "Put the target controller into receive configuration mode.\nThe PC Z-Wave Controller must be within 2m of the controller being added.", "");
                            //    break;
                            //case "REMOVE CONTROLLER":
                            //    m_manager.OnControllerStateChanged += m_controllerStateChangedHandler;
                            //    if (!m_manager.BeginControllerCommand(m_homeId, ZWControllerCommand.RemoveController, false, nid))
                            //    {
                            //        logging.AddToLog("Remove Controller Failed", true);
                            //        m_manager.OnControllerStateChanged -= m_controllerStateChangedHandler;
                            //    }
                            //    //osae.MethodQueueAdd(osae.GetPluginName("GUI CLIENT", osae.ComputerName), "POPUP MESSAGE", "Put the target controller into receive configuration mode.\nThe PC Z-Wave Controller must be within 2m of the controller being removed.", "");
                            //    break;
                            case "ADD DEVICE":
                                m_manager.OnControllerStateChanged += m_controllerStateChangedHandler;
                                if (!m_manager.BeginControllerCommand(m_homeId, ZWControllerCommand.AddDevice, false, nid))
                                {
                                    logging.AddToLog("Add Device Failed", true);
                                    m_manager.OnControllerStateChanged -= m_controllerStateChangedHandler;
                                }
                                break;
                            case "REMOVE DEVICE":
                                m_manager.OnControllerStateChanged += m_controllerStateChangedHandler;
                                if (m_manager.BeginControllerCommand(m_homeId, ZWControllerCommand.RemoveDevice, false, nid))
                                {
                                    OSAEObjectManager.ObjectDelete(OSAEObjectManager.GetObjectByAddress("Z" + nid.ToString()).Name);
                                }
                                else
                                {
                                    logging.AddToLog("Remove Device Failed", true);
                                    m_manager.OnControllerStateChanged -= m_controllerStateChangedHandler;
                                }
                                break;
                            case "REMOVE FAILED NODE":
                                m_manager.OnControllerStateChanged += m_controllerStateChangedHandler;
                                if (m_manager.BeginControllerCommand(m_homeId, ZWControllerCommand.RemoveFailedNode, false, nid))
                                {
                                    OSAEObjectManager.ObjectDelete(OSAEObjectManager.GetObjectByAddress("Z" + nid.ToString()).Name);
                                }
                                else
                                {
                                    logging.AddToLog("Remove Failed Node Failed: Z" + nid.ToString(), true);
                                    m_manager.OnControllerStateChanged -= m_controllerStateChangedHandler;
                                }
                                break;
                            case "RESET CONTROLLER":
                                logging.AddToLog("Resetting Controller", true);
                                m_manager.ResetController(m_homeId);
                                //DataSet ds = osae.GetObjectsByType("ZWAVE DIMMER");
                                //foreach (DataRow dr in ds.Tables[0].Rows)
                                //    osae.ObjectDelete(dr["object_name"].ToString());
                                //ds = osae.GetObjectsByType("ZWAVE BINARY SWITCH");
                                //foreach (DataRow dr in ds.Tables[0].Rows)
                                //    osae.ObjectDelete(dr["object_name"].ToString());
                                //ds = osae.GetObjectsByType("ZWAVE THERMOSTAT");
                                //foreach (DataRow dr in ds.Tables[0].Rows)
                                //    osae.ObjectDelete(dr["object_name"].ToString());
                                break;
                            case "NODE NEIGHBOR UPDATE":
                                logging.AddToLog("Requesting Node Neighbor Update: Z" + nid.ToString(), true);
                                m_manager.OnControllerStateChanged += m_controllerStateChangedHandler;
                                if (!m_manager.BeginControllerCommand(m_homeId, ZWControllerCommand.RequestNodeNeighborUpdate, false, nid))
                                {
                                    logging.AddToLog("Request Node Neighbor Update Failed: Z" + nid.ToString(), true);
                                    m_manager.OnControllerStateChanged -= m_controllerStateChangedHandler;
                                }
                                break;
                            case "NETWORK UPDATE":
                                logging.AddToLog("Requesting Network Update", true);
                                m_manager.OnControllerStateChanged += m_controllerStateChangedHandler;
                                if (!m_manager.BeginControllerCommand(m_homeId, ZWControllerCommand.RequestNetworkUpdate, false, nid))
                                {
                                    logging.AddToLog("Request Network Update Failed: Z" + nid.ToString(), true);
                                    m_manager.OnControllerStateChanged -= m_controllerStateChangedHandler;
                                }
                                break;
                            case "ENABLE POLLING":
                                enablePolling(nid);
                                break;
                        }

                    }
                    catch (Exception ex)
                    {
                        logging.AddToLog("Controller command failed (" + method.MethodName + "): " + ex.Message + " -- " + ex.StackTrace
                            + " -- " + ex.InnerException, true);
                    }
                    #endregion
                }

            }
            catch (Exception ex)
            {
                logging.AddToLog("Error Processing Command - " + ex.Message + " -" + ex.InnerException, true);
            }

        }

        public override void Shutdown()
        {
            m_manager.RemoveDriver(@"\\.\COM" + OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Port").Value);
            m_manager = null;
        }

        public void NotificationHandler(ZWNotification notification)
        {
            m_notification = notification;
            NotificationHandler();
            m_notification = null;
        }

        private void NotificationHandler()
        {
            Node node2 = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());

            logging.AddToLog(" --- ", true);
            logging.AddToLog("Notification: " + m_notification.GetType().ToString() + " | Node: " + node2.ID.ToString(), true);
            switch (m_notification.GetType())
            {
                #region ValueAdded
                case ZWNotification.Type.ValueAdded:
                    {

                        Node node = GetNode(m_homeId, m_notification.GetNodeId());
                        logging.AddToLog("ValueAdded start: node:" + node.ID.ToString(), true);
                        Value value = new Value();
                        ZWValueID vid = m_notification.GetValueID();
                        value.ValueID = vid;
                        value.Label = m_manager.GetValueLabel(vid);
                        value.Genre = vid.GetGenre();
                        value.Index = vid.GetIndex().ToString();
                        value.Type = vid.GetType();
                        value.CommandClassID = vid.GetCommandClassId().ToString();
                        node.AddValue(value);

                        string objType = m_manager.GetNodeProductName(m_homeId, node.ID);
                        string propType;

                        if (!string.IsNullOrEmpty(objType))
                        {
                            if (m_manager.IsValueReadOnly(value.ValueID))
                            {
                                if (value.Genre == ZWValueID.ValueGenre.User)
                                {
                                    if (value.Label == "Sensor")
                                    {
                                        OSAEObjectTypeManager.ObjectTypeStateAdd("ON", "On", objType);
                                        OSAEObjectTypeManager.ObjectTypeStateAdd("OFF", "Off", objType);
                                        OSAEObjectTypeManager.ObjectTypeEventAdd("ON", "On", objType);
                                        OSAEObjectTypeManager.ObjectTypeEventAdd("OFF", "Off", objType);
                                        OSAEObjectTypeManager.ObjectTypeEventAdd("ALARM", "Alarm", objType);
                                    }
                                    else
                                    {

                                        if (value.Type == ZWValueID.ValueType.Bool)
                                            propType = "Boolean";
                                        else if (value.Type == ZWValueID.ValueType.Byte || value.Type == ZWValueID.ValueType.Int)
                                            propType = "Integer";
                                        else
                                            propType = "String";

                                        OSAEObjectTypeManager.ObjectTypePropertyAdd(value.Label, propType, "", objType, false);
                                        OSAEObjectTypeManager.ObjectTypeEventAdd(value.Label, value.Label, objType);
                                    }
                                }
                            }
                            else
                            {
                                if (value.Genre == ZWValueID.ValueGenre.User)
                                {
                                    if (value.Label == "Switch" || value.Label == "Level")
                                    {
                                        OSAEObjectTypeManager.ObjectTypeStateAdd("ON", "On", objType);
                                        OSAEObjectTypeManager.ObjectTypeStateAdd("OFF", "Off", objType);
                                        OSAEObjectTypeManager.ObjectTypeEventAdd("ON", "On", objType);
                                        OSAEObjectTypeManager.ObjectTypeEventAdd("OFF", "Off", objType);
                                        OSAEObjectTypeManager.ObjectTypeMethodAdd("ON", "On", objType, "", "", "", "");
                                        OSAEObjectTypeManager.ObjectTypeMethodAdd("OFF", "Off", objType, "", "", "", "");
                                        if(value.Label == "Level")
                                            OSAEObjectTypeManager.ObjectTypePropertyAdd("Level", "Integer", "", objType, false);
                                    }
                                    else
                                    {
                                        if (value.Type == ZWValueID.ValueType.Byte || value.Type == ZWValueID.ValueType.Decimal || value.Type == ZWValueID.ValueType.Int)
                                        {
                                            OSAEObjectTypeManager.ObjectTypeMethodAdd(value.Label, value.Label, objType, "Value", "", "", "");
                                        }
                                        else if (value.Type == ZWValueID.ValueType.Button)
                                        {
                                            OSAEObjectTypeManager.ObjectTypeMethodAdd(value.Label, value.Label, objType, "", "", "", "");
                                        }
                                    }
                                }
                            }
                        }
                        

                        logging.AddToLog("ValueAdded: node:" + node.ID + " | type: " + value.Type
                            + " | genre: " + value.Genre + " | cmdClsID:" + value.CommandClassID
                            + " | index: " + value.Index + " | instance: " + vid.GetInstance().ToString()
                            + " | readOnly: " + m_manager.IsValueReadOnly(value.ValueID).ToString()
                            + " | value: " + value.Val + " | label: " + m_manager.GetValueLabel(vid), true);
                        break;
                    }
                #endregion

                #region ValueRemoved
                case ZWNotification.Type.ValueRemoved:
                    {
                        try
                        {
                            logging.AddToLog("ValueRemoved: ", true);
                            Node node = GetNode(m_homeId, m_notification.GetNodeId());
                            ZWValueID vid = m_notification.GetValueID();
                            Value val = node.GetValue(vid);
                            node.RemoveValue(val);
                        }
                        catch (Exception ex)
                        {
                            logging.AddToLog("ValueRemoved error: " + ex.Message, true);
                        }
                        break;
                    }
                #endregion

                #region ValueChanged
                case ZWNotification.Type.ValueChanged:
                    {
                        try
                        {
                            Node node = GetNode(m_homeId, m_notification.GetNodeId());
                            logging.AddToLog("ValueChanged start: node:" + node.ID.ToString(), false);
                            ZWValueID vid = m_notification.GetValueID();
                            Value value = node.GetValue(vid);
                            logging.AddToLog("value:" + value.Val, false);
                            OSAEObject nodeObject = OSAEObjectManager.GetObjectByAddress("Z" + m_notification.GetNodeId());
                            string v;
                            m_manager.GetValueAsString(vid, out v);
                            value.Val = v;

                            if (m_manager.IsValueReadOnly(value.ValueID))
                            {
                                if (value.Genre == ZWValueID.ValueGenre.User)
                                {
                                    if (value.Label == "Sensor")
                                    {
                                        if (value.Val == "False")
                                            OSAEObjectStateManager.ObjectStateSet(nodeObject.Name, "OFF", "ZWave");
                                        else
                                            OSAEObjectStateManager.ObjectStateSet(nodeObject.Name, "ON", "ZWave");
                                    }
                                    else if (value.Label == "Alarm Level")
                                    {
                                        if (value.Val == "255")
                                            logging.EventLogAdd(nodeObject.Name, "ALARM");
                                    }
                                    else
                                    {
                                        OSAEObjectPropertyManager.ObjectPropertySet(nodeObject.Name, value.Label, value.Val, "ZWave");
                                        logging.AddToLog("Set property " + value.Label + " of " + nodeObject.Name + " to: " + value.Val.ToString(), false);
                                    }
                                }
                            }
                            else
                            {
                                if (value.Genre == ZWValueID.ValueGenre.User)
                                {
                                    if (value.Label == "Switch")
                                    {
                                        if (value.Val == "False")
                                            OSAEObjectStateManager.ObjectStateSet(nodeObject.Name, "OFF", "ZWave");
                                        else
                                            OSAEObjectStateManager.ObjectStateSet(nodeObject.Name, "ON", "ZWave");
                                    }
                                    else if (value.Label == "Level")
                                    {
                                        if (value.Val == "0")
                                            OSAEObjectStateManager.ObjectStateSet(nodeObject.Name, "OFF", "ZWave");
                                        else
                                            OSAEObjectStateManager.ObjectStateSet(nodeObject.Name, "ON", "ZWave");
                                        OSAEObjectPropertyManager.ObjectPropertySet(nodeObject.Name, value.Label, value.Val, "ZWave");
                                        logging.AddToLog("Set property " + value.Label + " of " + nodeObject.Name + " to: " + value.Val.ToString(), false);
                                    }
                                    else
                                    {
                                        OSAEObjectPropertyManager.ObjectPropertySet(nodeObject.Name, value.Label, value.Val, "ZWave");
                                        logging.AddToLog("Set property " + value.Label + " of " + nodeObject.Name + " to: " + value.Val.ToString(), false);
                                    }
                                }
                            }

                            logging.AddToLog("ValueChanged: " + ((nodeObject != null) ? nodeObject.Name : "Object Not In OSA") + " | node:"
                                + node.ID + " | nodelabel: " + node.Label + " | type: " + value.Type
                                + " | genre: " + value.Genre + " | cmdClsID:" + value.CommandClassID
                                + " | readOnly: " + m_manager.IsValueReadOnly(value.ValueID) + " | value: " + value.Val + " | label: " + value.Label, false);

                        }
                        catch (Exception ex)
                        {
                            logging.AddToLog("ValueChanged error: " + ex.Message, true);
                        }
                        break;
                    }
                #endregion

                #region Group
                case ZWNotification.Type.Group:
                    {
                        Node node = GetNode(m_homeId, m_notification.GetNodeId());
                        logging.AddToLog("Group: " + node.ID, true);
                        break;
                    }
                #endregion

                #region NodeAdded
                case ZWNotification.Type.NodeAdded:
                    {
                        // Add the new node to our list
                        Node node = new Node();
                        node.ID = m_notification.GetNodeId();
                        node.HomeID = m_homeId;
                        node.Label = m_manager.GetNodeType(m_homeId, node.ID);
                        m_nodeList.Add(node);

                        logging.AddToLog("NodeAdded: " + node.ID.ToString(), true);
                        break;
                    }
                #endregion

                #region NodeRemoved
                case ZWNotification.Type.NodeRemoved:
                    {
                        foreach (Node node in m_nodeList)
                        {
                            if (node.ID == m_notification.GetNodeId())
                            {
                                m_nodeList.Remove(node);
                                break;
                            }
                        }
                        logging.AddToLog("NodeRemoved: " + m_notification.GetNodeId(), true);
                        break;
                    }
                #endregion

                #region NodeProtocolInfo
                case ZWNotification.Type.NodeProtocolInfo:
                    {
                        Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());

                        if (node != null)
                        {
                            node.Label = m_manager.GetNodeType(m_homeId, node.ID);
                        }

                        logging.AddToLog("NodeProtocolInfo: node: " + node.ID + " | " + node.Label, true);

                        break;
                    }
                #endregion

                #region NodeNaming
                case ZWNotification.Type.NodeNaming:
                    {
                        Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());
                        if (node != null)
                        {
                            node.Manufacturer = m_manager.GetNodeManufacturerName(m_homeId, node.ID);
                            node.Product = m_manager.GetNodeProductName(m_homeId, node.ID);

                            // Create object type dynamically
                            if (OSAEObjectManager.GetObjectByAddress("Z" + node.ID.ToString()) == null)
                            {
                                string baseType = "ZWAVE DEVICE";
                                switch (node.Label)
                                {
                                    case "Binary Switch":
                                    case "Binary Power Switch":
                                        {
                                            baseType = "BINARY SWITCH";
                                            break;
                                        }
                                    case "Multilevel Switch":
                                    case "Multilevel Scene Switch":
                                        {
                                            baseType = "MULTILEVEL SWITCH";
                                            break;
                                        }
                                    case "General Thermostat V2":
                                        {
                                            baseType = "THERMOSTAT";
                                            break;
                                        }
                                }

                                OSAEObjectTypeManager.ObjectTypeAdd(node.Product, node.Label, pName, baseType,  0, 0, 0, 1);
                                OSAEObjectTypeManager.ObjectTypeMethodAdd("NODE NEIGHBOR UPDATE", "Node Neighbor Update", node.Product, "", "", "", "");
                                OSAEObjectTypeManager.ObjectTypePropertyAdd("Home ID", "String", "", node.Product, false);
                                OSAEObjectTypeManager.ObjectTypePropertyAdd("Poll", "Boolean", "", node.Product, false);

                                string propType;
                                foreach (Value v in node.Values)
                                {
                                    if (m_manager.IsValueReadOnly(v.ValueID))
                                    {
                                        if (v.Type == ZWValueID.ValueType.Bool)
                                            propType = "Boolean";
                                        else if (v.Type == ZWValueID.ValueType.Byte || v.Type == ZWValueID.ValueType.Int)
                                            propType = "Integer";
                                        else
                                            propType = "String";

                                        OSAEObjectTypeManager.ObjectTypePropertyAdd(v.Label, propType, "", node.Product, false);
                                    }
                                    else
                                    {
                                        if (v.Genre == ZWValueID.ValueGenre.User)
                                        {
                                            if (v.Label == "Switch" || v.Label == "Level")
                                            {
                                                OSAEObjectTypeManager.ObjectTypeStateAdd("ON", "On", node.Product);
                                                OSAEObjectTypeManager.ObjectTypeStateAdd("OFF", "Off", node.Product);
                                                OSAEObjectTypeManager.ObjectTypeEventAdd("ON", "On", node.Product);
                                                OSAEObjectTypeManager.ObjectTypeEventAdd("OFF", "Off", node.Product);
                                                OSAEObjectTypeManager.ObjectTypeMethodAdd("ON", "On", node.Product, "", "", "", "");
                                                OSAEObjectTypeManager.ObjectTypeMethodAdd("OFF", "Off", node.Product, "", "", "", "");
                                            }
                                            else
                                            {
                                                if (v.Type == ZWValueID.ValueType.Byte || v.Type == ZWValueID.ValueType.Decimal || v.Type == ZWValueID.ValueType.Int)
                                                {
                                                    OSAEObjectTypeManager.ObjectTypeMethodAdd(v.Label, v.Label, node.Product, "Value", "", "", "");
                                                }
                                                else if (v.Type == ZWValueID.ValueType.Button)
                                                {
                                                    OSAEObjectTypeManager.ObjectTypeMethodAdd(v.Label, v.Label, node.Product, "", "", "", "");
                                                }
                                            }
                                        }
                                    }
                                }


                                OSAEObjectManager.ObjectAdd(node.Product + " - Z" + node.ID.ToString(), node.Product, node.Product, "Z" + node.ID.ToString(), "", true);
                                OSAEObjectPropertyManager.ObjectPropertySet(node.Product + " - Z" + node.ID.ToString(), "Home ID", m_homeId.ToString(), pName);
                            }

                        }

                        logging.AddToLog("NodeNaming: Manufacturer: " + node.Manufacturer + " | Product: " + node.Product, true);
                        break;
                    }
                #endregion

                #region NodeNew
                case ZWNotification.Type.NodeNew:
                    {
                        Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());
                        if (node != null)
                        {
                            node.Manufacturer = m_manager.GetNodeManufacturerName(m_homeId, node.ID);
                            node.Product = m_manager.GetNodeProductName(m_homeId, node.ID);
                        }

                        logging.AddToLog("NodeNew: Manufacturer: " + node.Manufacturer + " | Product: " + node.Product, true);
                        break;
                    }
                #endregion

                #region NodeEvent
                case ZWNotification.Type.NodeEvent:
                    {
                        try
                        {
                            Node node = GetNode(m_homeId, m_notification.GetNodeId());
                            if (node != null)
                            {
                                node.Label = m_manager.GetNodeType(m_homeId, node.ID);
                            }
                            logging.AddToLog("GetEvent:" + m_notification.GetEvent().ToString(), false);
                            logging.AddToLog("node.Label:" + node.Label, false);

                            ZWValueID vid = m_notification.GetValueID();
                            Value value = node.GetValue(vid);
                            OSAEObject nodeObject = OSAEObjectManager.GetObjectByAddress("Z" + m_notification.GetNodeId());
                            string v;
                            m_manager.GetValueAsString(vid, out v);
                            value.Val = v;

                            if (m_notification.GetEvent() > 0)
                                OSAEObjectStateManager.ObjectStateSet(nodeObject.Name, "ON", "ZWave");
                            else
                                OSAEObjectStateManager.ObjectStateSet(nodeObject.Name, "OFF", "ZWave");

                            logging.AddToLog("NodeEvent: " + ((nodeObject != null) ? nodeObject.Name : "Object Not In OSA") + " | node:" + node.ID + " | type: " + value.Type
                            + " | genre: " + value.Genre + " | cmdClsID:" + value.CommandClassID
                            + " | value: " + value.Val + " | label: " + value.Label, false);
                        }
                        catch (Exception ex)
                        {
                            logging.AddToLog("Error in NodeEvent: " + ex.Message, true);
                        }

                        break;
                    }
                #endregion

                #region PollingDisabled
                case ZWNotification.Type.PollingDisabled:
                    {
                        break;
                    }
                #endregion

                #region PollingEnabled
                case ZWNotification.Type.PollingEnabled:
                    {
                        logging.AddToLog("Polling Enabled: " + OSAEObjectManager.GetObjectByAddress("Z" + m_notification.GetNodeId().ToString()).Name, true);
                        break;
                    }
                #endregion

                #region DriverReady
                case ZWNotification.Type.DriverReady:
                    {
                        m_homeId = m_notification.GetHomeId();
                        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Home ID", m_homeId.ToString(), pName);
                        logging.AddToLog("Driver Ready.  Home ID: " + m_homeId.ToString(), true);
                        break;
                    }
                #endregion

                #region DriverReset
                case ZWNotification.Type.DriverReset:
                    {
                        m_homeId = m_notification.GetHomeId();
                        logging.AddToLog("Driver Reset.  Home ID: " + m_homeId.ToString(), true);
                        break;
                    }
                #endregion

                #region NodeQueriesComplete
                case ZWNotification.Type.NodeQueriesComplete:
                    {
                        Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());


                        logging.AddToLog("Node Queries Complete | " + node.ID + " | " + m_manager.GetNodeProductName(m_homeId, node.ID), true);
                        break;
                    }
                #endregion

                #region EssentialNodeQueriesComplete
                case ZWNotification.Type.EssentialNodeQueriesComplete:
                    {
                        Node node = GetNode(m_homeId, m_notification.GetNodeId());
                        logging.AddToLog("Essential Node Queries Completee | " + node.ID + " | " + m_manager.GetNodeProductName(m_homeId, node.ID), true);
                        break;
                    }
                #endregion

                #region AllNodesQueried
                case ZWNotification.Type.AllNodesQueried:
                    {
                        logging.AddToLog("All nodes queried", true);
                        foreach (Node n in m_nodeList)
                        {
                            OSAEObject obj = OSAEObjectManager.GetObjectByAddress("Z" + n.ID.ToString());
                            if (obj != null)
                            {
                                if (OSAEObjectPropertyManager.GetObjectPropertyValue(OSAEObjectManager.GetObjectByAddress("Z" + n.ID.ToString()).Name, "Poll").Value == "TRUE")
                                    enablePolling(n.ID);
                            }
                        }
                        break;
                    }
                #endregion

                #region AwakeNodesQueried
                case ZWNotification.Type.AwakeNodesQueried:
                    {
                        logging.AddToLog("Awake nodes queried (but some sleeping nodes have not been queried)", true);
                        foreach (Node n in m_nodeList)
                        {
                            OSAEObject obj = OSAEObjectManager.GetObjectByAddress("Z" + n.ID.ToString());
                            if (obj != null)
                            {
                                if (OSAEObjectPropertyManager.GetObjectPropertyValue(OSAEObjectManager.GetObjectByAddress("Z" + n.ID.ToString()).Name, "Poll").Value == "TRUE")
                                {
                                    logging.AddToLog("Enabling polling for: " + obj.Name, true);
                                    enablePolling(n.ID);
                                }
                            }
                        }
                        break;
                    }
                #endregion
            }
            logging.AddToLog(" --- ", true);
        }

        public static void MyControllerStateChangedHandler(ZWControllerState state)
        {
            // Handle the controller state notifications here.
            bool complete = false;
            switch (state)
            {
                case ZWControllerState.Waiting:
                    {
                        logging.AddToLog("Waiting...", true);
                        break;
                    }
                case ZWControllerState.InProgress:
                    {
                        // Tell the user that the controller has been found and the adding process is in progress.
                        logging.AddToLog("Please wait...", true);
                        break;
                    }
                case ZWControllerState.Completed:
                    {
                        // Tell the user that the controller has been successfully added.
                        // The command is now complete
                        logging.AddToLog("Command Completed OK.", true);
                        complete = true;
                        break;
                    }
                case ZWControllerState.Failed:
                    {
                        // Tell the user that the controller addition process has failed.
                        // The command is now complete
                        logging.AddToLog("Command Failed.", true);
                        complete = true;
                        break;
                    }
                case ZWControllerState.NodeOK:
                    {
                        logging.AddToLog("Node has not failed.", true);
                        complete = true;
                        break;
                    }
                case ZWControllerState.NodeFailed:
                    {
                        logging.AddToLog("Node has failed.", true);
                        complete = true;
                        break;
                    }
            }


            if (complete)
            {
                logging.AddToLog("Removing event handler", true);
                // Remove the event handler
                m_manager.OnControllerStateChanged -= m_controllerStateChangedHandler;
            }
        }


        private Node GetNode(UInt32 homeId, Byte nodeId)
        {
            foreach (Node node in m_nodeList)
            {
                if ((node.ID == nodeId) && (node.HomeID == homeId))
                {
                    return node;
                }
            }

            return new Node();
        }

        private void enablePolling(byte nid)
        {
            logging.AddToLog("Attempting to Enable Polling: " + OSAEObjectManager.GetObjectByAddress("Z" + nid.ToString()).Name, true);
            try
            {
                Node n = GetNode(m_homeId, nid);
                List<ZWValueID> zv = new List<ZWValueID>();
                switch (n.Label)
                {
                    case "Binary Switch":
                    case "Binary Power Switch":
                        foreach (Value v in n.Values)
                        {
                            if (v.Label == "Switch")
                                zv.Add(v.ValueID);
                        }
                        break;
                    case "Multilevel Switch":
                    case "Multilevel Power Switch":
                        foreach (Value v in n.Values)
                        {
                            if ((v.Genre == ZWValueID.ValueGenre.User && v.Label == "Level") || v.Label == "Power")
                                zv.Add(v.ValueID);
                        }
                        break;
                    case "General Thermostat V2":
                        foreach (Value v in n.Values)
                        {
                            if (v.Label == "Temperature")
                                zv.Add(v.ValueID);
                        }
                        break;
                    case "Routing Multilevel Sensor":
                        if (m_manager.GetNodeProductName(m_homeId, n.ID) == "Smart Energy Switch")
                        {
                            foreach (Value v in n.Values)
                            {
                                if (v.Label == "Power")
                                    zv.Add(v.ValueID);
                            }
                        }
                        break;
                }
                foreach (ZWValueID zwv in zv)
                {
                    if (m_manager.EnablePoll(zwv))
                        logging.AddToLog("Enable Polling Succeeded", true);
                    else
                        logging.AddToLog("Enable Polling Failed", true);
                }
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error attempting to enable polling: " + ex.Message, true);
            }
        }
    }

}
