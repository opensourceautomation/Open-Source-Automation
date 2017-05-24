using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenZWaveDotNet;
using OSAE;

namespace OSAE.Zwave
{
    public class Zwave : OSAEPluginBase
    {
        private static OSAE.General.OSAELog Log;
        static private ManagedControllerStateChangedHandler m_controllerStateChangedHandler = new ManagedControllerStateChangedHandler(Zwave.MyControllerStateChangedHandler);
        static private ZWManager m_manager = null;
        ZWOptions m_options = null;
        UInt32 m_homeId = 0;
        ZWNotification m_notification = null;
        List<Node> m_nodeList = new List<Node>();
        string pName;
        private bool isShuttingDown = false;

        public override async void RunInterface(string pluginName)
        {
            pName = pluginName;
            Log = new General.OSAELog(pName);
            await StartOpenzwaveAsync();
        }

        public override void ProcessCommand(OSAEMethod method)
        {
            Log.Debug("Found Command: name: " + method.MethodName + " | label: " + method.MethodLabel + " | param1: " + method.Parameter1 + " | param2: " + method.Parameter2 + " | obj: " + method.ObjectName + " | addr: " + method.Address );
            //process command
            try
            {
                if (method.Address.Length > 0)
                {
                    
                    int address;
                    byte instance = 0;
                    byte nid;
                    if (int.TryParse(method.Address.Substring(1), out address))
                        nid = (byte)address;
                    else
                    {
                        nid = (byte)Int32.Parse(method.Address.Substring(1).Split('-')[0]);
                        instance = (byte)Int32.Parse(method.Address.Substring(1).Split('-')[1]);
                    }
                    Node node = GetNode(m_homeId, nid);
                    OSAEObject obj = OSAEObjectManager.GetObjectByAddress("Z" + nid.ToString());

                    if (method.MethodName == "NODE NEIGHBOR UPDATE")
                    {
                        Log.Info("Requesting Node Neighbor Update: " + obj.Name);
                        m_manager.OnControllerStateChanged += m_controllerStateChangedHandler;
                        if (!m_manager.RequestNodeNeighborUpdate(m_homeId, nid))
                        {
                            Log.Info("Request Node Neighbor Update Failed: " + obj.Name);
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
                                v = value;
                        }

                        //m_manager.SetNodeOn(m_homeId, nid);

                        if(OSAEObjectManager.GetObjectByAddress("Z" + nid.ToString()).BaseType == "BINARY SWITCH")
                            m_manager.SetValue(v.ValueID, true);
                        else if(OSAEObjectManager.GetObjectByAddress("Z" + nid.ToString()).BaseType == "MULTILEVEL SWITCH")
                            m_manager.SetValue(v.ValueID, (byte)val);

                        OSAEObjectStateManager.ObjectStateSet(method.ObjectName, "ON", pName);
                        Log.Debug("Turned on: " + method.ObjectName);
                    }
                    else if(method.MethodName == "OFF")
                    {
                        Value v = new Value();
                        foreach (Value value in node.Values)
                        {
                            if ((obj.BaseType == "BINARY SWITCH" && value.Label == "Switch") || obj.BaseType == "MULTILEVEL SWITCH" && value.Label == "Level")
                                v = value;
                        }

                        if (OSAEObjectManager.GetObjectByAddress("Z" + nid.ToString()).BaseType == "BINARY SWITCH")
                            m_manager.SetValue(v.ValueID, false);
                        else if (OSAEObjectManager.GetObjectByAddress("Z" + nid.ToString()).BaseType == "MULTILEVEL SWITCH")
                            m_manager.SetValue(v.ValueID, (byte)0);

                        //m_manager.SetNodeOff(m_homeId, nid);
                        OSAEObjectStateManager.ObjectStateSet(method.ObjectName, "OFF", pName);
                        Log.Debug("Turned off: " + method.ObjectName);
                    }
                    else
                    {
                        foreach (Value value in node.Values)
                        {
                            if (value.Label.ToUpper() == method.MethodName.ToUpper())
                            {
                                if (method.Parameter1 != "")
                                {
                                    if (value.Type == ZWValueID.ValueType.String)
                                        m_manager.SetValue(value.ValueID, method.Parameter1);
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

                                    Log.Debug("Set " + method.MethodName + " to " + method.Parameter1 + ": " + method.ObjectName);
                                }
                                else if (value.Type == ZWValueID.ValueType.Button)
                                {
                                    m_manager.PressButton(value.ValueID);
                                    m_manager.ReleaseButton(value.ValueID);
                                }
                            }
                            else
                            {
                                String[] split;
                                split = method.MethodLabel.Split('-');
                                if (value.Label.ToUpper() == split[0].Trim().ToUpper())
                                {
                                    if (value.Type == ZWValueID.ValueType.List)
                                    {
                                        m_manager.SetValueListSelection(value.ValueID, split[1].Trim());
                                        Log.Info("Set " + value.Label + " to " + split[1].Trim());
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
                            //        this.Log.Info("Add Controller Failed", true);
                            //        m_manager.OnControllerStateChanged -= m_controllerStateChangedHandler;
                            //    }
                            //    //osae.MethodQueueAdd(osae.GetPluginName("GUI CLIENT", osae.ComputerName), "POPUP MESSAGE", "Put the target controller into receive configuration mode.\nThe PC Z-Wave Controller must be within 2m of the controller being added.", "");
                            //    break;
                            //case "REMOVE CONTROLLER":
                            //    m_manager.OnControllerStateChanged += m_controllerStateChangedHandler;
                            //    if (!m_manager.BeginControllerCommand(m_homeId, ZWControllerCommand.RemoveController, false, nid))
                            //    {
                            //        this.Log.Info("Remove Controller Failed", true);
                            //        m_manager.OnControllerStateChanged -= m_controllerStateChangedHandler;
                            //    }
                            //    //osae.MethodQueueAdd(osae.GetPluginName("GUI CLIENT", osae.ComputerName), "POPUP MESSAGE", "Put the target controller into receive configuration mode.\nThe PC Z-Wave Controller must be within 2m of the controller being removed.", "");
                            //    break;
                            case "ADD DEVICE":
                                m_manager.OnControllerStateChanged += m_controllerStateChangedHandler;
                                if (!m_manager.AddNode(m_homeId, false))
                                {
                                    Log.Info("Add Device Failed");
                                    m_manager.OnControllerStateChanged -= m_controllerStateChangedHandler;
                                }
                                break;
                            case "REMOVE DEVICE":
                                m_manager.OnControllerStateChanged += m_controllerStateChangedHandler;
                                if (m_manager.RemoveNode(m_homeId))
                                    OSAEObjectManager.ObjectDelete(OSAEObjectManager.GetObjectByAddress("Z" + nid.ToString()).Name);
                                else
                                {
                                    Log.Info("Remove Device Failed");
                                    m_manager.OnControllerStateChanged -= m_controllerStateChangedHandler;
                                }
                                break;
                            case "REMOVE FAILED NODE":
                                m_manager.OnControllerStateChanged += m_controllerStateChangedHandler;
                                if (m_manager.RemoveFailedNode(m_homeId, nid))
                                    OSAEObjectManager.ObjectDelete(OSAEObjectManager.GetObjectByAddress("Z" + nid.ToString()).Name);
                                else
                                {
                                    Log.Info("Remove Failed Node Failed: Z" + nid.ToString());
                                    m_manager.OnControllerStateChanged -= m_controllerStateChangedHandler;
                                }
                                break;
                            case "RESET CONTROLLER":
                                Log.Info("Resetting Controller");
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
                                Log.Info("Requesting Node Neighbor Update: Z" + nid.ToString());
                                m_manager.OnControllerStateChanged += m_controllerStateChangedHandler;
                                if (!m_manager.RequestNodeNeighborUpdate(m_homeId, nid))
                                {
                                    Log.Info("Request Node Neighbor Update Failed: Z" + nid.ToString());
                                    m_manager.OnControllerStateChanged -= m_controllerStateChangedHandler;
                                }
                                break;
                            case "NETWORK UPDATE":
                                Log.Info("Requesting Network Update");
                                m_manager.OnControllerStateChanged += m_controllerStateChangedHandler;
                                if (!m_manager.RequestNetworkUpdate(m_homeId, nid))
                                {
                                    Log.Info("Request Network Update Failed: Z" + nid.ToString());
                                    m_manager.OnControllerStateChanged -= m_controllerStateChangedHandler;
                                }
                                break;
                            case "ENABLE POLLING":
                                enablePolling(nid);
                                break;
                        }

                    }
                    catch (Exception ex)
                    { Log.Error("Controller command failed (" + method.MethodName + ")", ex); }
                    #endregion
                }

            }
            catch (Exception ex)
            { Log.Error("Error Processing Command - " + ex.Message + " -" + ex.InnerException); }
        }

        public override void Shutdown()
        {
            StopOpenzwave();
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

            Log.Info("Notification: " + m_notification.GetType().ToString() + " | Node: " + node2.ID.ToString());
            switch (m_notification.GetType())
            {
                #region ValueAdded
                case ZWNotification.Type.ValueAdded:
                    {

                        Node node = GetNode(m_homeId, m_notification.GetNodeId());
                        Log.Info("ValueAdded start: node:" + node.ID.ToString());
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

                        Log.Info("ValueAdded: objType: " + objType + " | node: " + node.ID + " | type: " + value.Type
                            + " | genre: " + value.Genre + " | cmdClsID:" + value.CommandClassID
                            + " | index: " + value.Index + " | instance: " + vid.GetInstance().ToString()
                            + " | readOnly: " + m_manager.IsValueReadOnly(value.ValueID).ToString()
                            + " | value: " + value.Val + " | label: " + m_manager.GetValueLabel(vid));

                        if (!string.IsNullOrEmpty(objType))
                        {
                            if (m_manager.IsValueReadOnly(value.ValueID))
                            {
                                Log.Debug("Found read only value");
                                if (value.Genre == ZWValueID.ValueGenre.User)
                                {
                                    if (value.Label == "Sensor")
                                    {
                                        OSAEObjectTypeManager.ObjectTypeStateAdd(objType, "ON", "On", "On");
                                        OSAEObjectTypeManager.ObjectTypeStateAdd(objType, "OFF", "Off", "Off");
                                        OSAEObjectTypeManager.ObjectTypeEventAdd(objType, "ON", "On", "On");
                                        OSAEObjectTypeManager.ObjectTypeEventAdd(objType, "OFF", "Off", "Off");
                                        OSAEObjectTypeManager.ObjectTypeEventAdd(objType, "ALARM", "Alarm", "Alarm");
                                    }
                                    else
                                    {

                                        if (value.Type == ZWValueID.ValueType.Bool)
                                            propType = "Boolean";
                                        else if (value.Type == ZWValueID.ValueType.Byte || value.Type == ZWValueID.ValueType.Int)
                                            propType = "Integer";
                                        else
                                            propType = "String";

                                        OSAEObjectTypeManager.ObjectTypePropertyAdd(objType, value.Label, propType,"", "", false,false, value.Label);
                                        OSAEObjectTypeManager.ObjectTypeEventAdd(objType, value.Label, value.Label, value.Label);
                                    }
                                }
                            }
                            else
                            {
                                Log.Debug("Found writable value");
                                if (value.Genre == ZWValueID.ValueGenre.User || value.Genre == ZWValueID.ValueGenre.Config)
                                {
                                    if (value.Label == "Switch" || value.Label == "Level")
                                    {
                                        OSAEObjectTypeManager.ObjectTypeStateAdd(objType, "ON", "On", "On");
                                        OSAEObjectTypeManager.ObjectTypeStateAdd(objType, "OFF", "Off", "Off");
                                        OSAEObjectTypeManager.ObjectTypeEventAdd(objType, "ON", "On", "On");
                                        OSAEObjectTypeManager.ObjectTypeEventAdd(objType, "OFF", "Off", "Off");
                                        OSAEObjectTypeManager.ObjectTypeMethodAdd(objType, "ON", "On", "", "", "", "", "Turm On");
                                        OSAEObjectTypeManager.ObjectTypeMethodAdd(objType, "OFF", "Off", "", "", "", "", "Turn Off");
                                        if (value.Label == "Level")
                                            OSAEObjectTypeManager.ObjectTypeMethodAdd(objType, "ON", "On", "Level", "", "", "", "Turm On");
                                        else
                                            OSAEObjectTypeManager.ObjectTypeMethodAdd(objType, "ON", "On", "", "", "", "", "Turm Off");
                                    }
                                    else
                                    {
                                        Log.Debug("Value is not a Switch or a Level");
                                        if (value.Type == ZWValueID.ValueType.Byte || value.Type == ZWValueID.ValueType.Decimal || value.Type == ZWValueID.ValueType.Int)
                                        {
                                            Log.Debug("Adding method: " + value.Label);
                                            OSAEObjectTypeManager.ObjectTypeMethodAdd(objType, value.Label, "Set " + value.Label, "Value", "", "", "", "Set " + value.Label);
                                            Log.Debug("Adding property: " + value.Label);
                                        }
                                        else if (value.Type == ZWValueID.ValueType.Button)
                                            OSAEObjectTypeManager.ObjectTypeMethodAdd(objType, value.Label, value.Label, "", "", "", "", value.Label);
                                        else if (value.Type == ZWValueID.ValueType.List)
                                        {
                                            String[] options;
                                            if (m_manager.GetValueListItems(value.ValueID, out options))
                                            {
                                                foreach (string option in options)
                                                    OSAEObjectTypeManager.ObjectTypeMethodAdd(objType, value.Label + " - " + option, value.Label + " - " + option, "", "", "", "", value.Label + " - " + option);

                                                OSAEObjectTypeManager.ObjectTypePropertyAdd(objType, value.Label, "String", "", "", false, false, value.Label);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        
                        break;
                    }
                #endregion

                #region ValueRemoved
                case ZWNotification.Type.ValueRemoved:
                    {
                        try
                        {
                            Log.Info("ValueRemoved: ");
                            Node node = GetNode(m_homeId, m_notification.GetNodeId());
                            ZWValueID vid = m_notification.GetValueID();
                            Value val = node.GetValue(vid);
                            node.RemoveValue(val);
                        }
                        catch (Exception ex)
                        { Log.Error("ValueRemoved error ", ex); }
                        break;
                    }
                #endregion

                #region ValueChanged
                case ZWNotification.Type.ValueChanged:
                    {
                        try
                        {
                            Node node = GetNode(m_homeId, m_notification.GetNodeId());
                            Log.Debug("ValueChanged start: node:" + node.ID.ToString());
                            ZWValueID vid = m_notification.GetValueID();
                            Value value = node.GetValue(vid);
                            
                            OSAEObject nodeObject = OSAEObjectManager.GetObjectByAddress("Z" + m_notification.GetNodeId());
                            string v;
                            m_manager.GetValueAsString(vid, out v);
                            value.Val = v;
                            
                            Log.Debug("value:" + value.Val);
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
                                            OSAEObjectManager.EventTrigger(nodeObject.Name, "ALARM", source: "ZWave");

                                        OSAEObjectPropertyManager.ObjectPropertySet(nodeObject.Name, value.Label, value.Val, "ZWave");
                                        Log.Debug("Set property " + value.Label + " of " + nodeObject.Name + " to: " + value.Val.ToString());
                                    }
                                    else
                                    {
                                        OSAEObjectPropertyManager.ObjectPropertySet(nodeObject.Name, value.Label, value.Val, "ZWave");
                                        Log.Debug("Set property " + value.Label + " of " + nodeObject.Name + " to: " + value.Val.ToString());
                                    }
                                }
                            }
                            else
                            {
                                if (value.Genre == ZWValueID.ValueGenre.User || value.Genre == ZWValueID.ValueGenre.Config)
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
                                        Log.Debug("Set property " + value.Label + " of " + nodeObject.Name + " to: " + value.Val.ToString());
                                    }
                                    else
                                    {
                                        OSAEObjectPropertyManager.ObjectPropertySet(nodeObject.Name, value.Label, value.Val, "ZWave");
                                        Log.Debug("Set property " + value.Label + " of " + nodeObject.Name + " to: " + value.Val.ToString());
                                    }
                                }
                            }

                            Log.Debug("ValueChanged: " + ((nodeObject != null) ? nodeObject.Name : "Object Not In OSA") + " | node:"
                                + node.ID + " | nodelabel: " + node.Label + " | type: " + value.Type
                                + " | genre: " + value.Genre + " | cmdClsID:" + value.CommandClassID
                                + " | readOnly: " + m_manager.IsValueReadOnly(value.ValueID) + " | value: " + value.Val + " | label: " + value.Label);

                        }
                        catch (Exception ex)
                        { Log.Error("ValueChanged error: " + ex.Message); }
                        break;
                    }
                #endregion

                #region Group
                case ZWNotification.Type.Group:
                    {
                        Node node = GetNode(m_homeId, m_notification.GetNodeId());
                        Log.Info("Group: " + node.ID);
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

                        Log.Info("NodeAdded: " + node.ID.ToString());
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
                        Log.Info("NodeRemoved: " + m_notification.GetNodeId());
                        break;
                    }
                #endregion

                #region NodeProtocolInfo
                case ZWNotification.Type.NodeProtocolInfo:
                    {
                        Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());

                        if (node != null) node.Label = m_manager.GetNodeType(m_homeId, node.ID);

                        Log.Info("NodeProtocolInfo: node: " + node.ID + " | " + node.Label);
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
                                    case "Multilevel Power Switch":
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

                                OSAEObjectTypeManager.ObjectTypeAdd(node.Product, node.Label, pName, baseType,  false, false, false, true, node.Label);
                                OSAEObjectTypeManager.ObjectTypeMethodAdd(node.Product, "NODE NEIGHBOR UPDATE", "Node Neighbor Update", "", "", "", "", "Node Neighbor Update");
                                OSAEObjectTypeManager.ObjectTypePropertyAdd(node.Product, "Home ID", "String", "", "", false, false, "Home ID");
                                OSAEObjectTypeManager.ObjectTypePropertyAdd(node.Product, "Poll", "Boolean", "", "", false, false, "Poll");

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

                                        OSAEObjectTypeManager.ObjectTypePropertyAdd(node.Product, v.Label, propType, "", "", false, false, v.Label);
                                    }
                                    else
                                    {
                                        if (v.Genre == ZWValueID.ValueGenre.User)
                                        {
                                            if (v.Label == "Switch" || v.Label == "Level")
                                            {
                                                OSAEObjectTypeManager.ObjectTypeStateAdd(node.Product, "ON", "On", "On");
                                                OSAEObjectTypeManager.ObjectTypeStateAdd(node.Product, "OFF", "Off", "Off");
                                                OSAEObjectTypeManager.ObjectTypeEventAdd(node.Product, "ON", "On", "On");
                                                OSAEObjectTypeManager.ObjectTypeEventAdd(node.Product, "OFF", "Off", "Off");
                                                OSAEObjectTypeManager.ObjectTypeMethodAdd(node.Product, "ON", "On", "", "", "", "", "Turn On");
                                                OSAEObjectTypeManager.ObjectTypeMethodAdd(node.Product, "OFF", "Off", "", "", "", "", "Turn Off");
                                            }
                                            else
                                            {
                                                if (v.Type == ZWValueID.ValueType.Byte || v.Type == ZWValueID.ValueType.Decimal || v.Type == ZWValueID.ValueType.Int)
                                                    OSAEObjectTypeManager.ObjectTypeMethodAdd(node.Product, v.Label, v.Label, "Value", "", "", "", v.Label);
                                                else if (v.Type == ZWValueID.ValueType.Button)
                                                    OSAEObjectTypeManager.ObjectTypeMethodAdd(node.Product, v.Label, v.Label, "", "", "", "", v.Label);
                                            }
                                        }
                                    }
                                }
                                OSAEObjectManager.ObjectAdd(node.Product + " - Z" + node.ID.ToString(),"", node.Product, node.Product, "Z" + node.ID.ToString(), "", 30, true);
                                OSAEObjectPropertyManager.ObjectPropertySet(node.Product + " - Z" + node.ID.ToString(), "Home ID", m_homeId.ToString(), pName);
                            }
                        }
                        Log.Info("NodeNaming: Manufacturer: " + node.Manufacturer + " | Product: " + node.Product);
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

                        Log.Info("NodeNew: Manufacturer: " + node.Manufacturer + " | Product: " + node.Product);
                        break;
                    }
                #endregion

                #region NodeEvent
                case ZWNotification.Type.NodeEvent:
                    {
                        try
                        {
                            Node node = GetNode(m_homeId, m_notification.GetNodeId());
                            if (node != null) node.Label = m_manager.GetNodeType(m_homeId, node.ID);
                            Log.Debug("GetEvent:" + m_notification.GetEvent().ToString());
                            Log.Debug("node.Label:" + node.Label);

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

                            Log.Debug("NodeEvent: " + ((nodeObject != null) ? nodeObject.Name : "Object Not In OSA") + " | node:" + node.ID + " | type: " + value.Type
                            + " | genre: " + value.Genre + " | cmdClsID:" + value.CommandClassID + " | value: " + value.Val + " | label: " + value.Label);
                        }
                        catch (Exception ex)
                        { Log.Error("Error in NodeEvent", ex); }
                        break;
                    }
                #endregion

                #region PollingDisabled
                case ZWNotification.Type.PollingDisabled:
                    { break; }
                #endregion

                #region PollingEnabled
                case ZWNotification.Type.PollingEnabled:
                    {
                        Log.Info("Polling Enabled: " + OSAEObjectManager.GetObjectByAddress("Z" + m_notification.GetNodeId().ToString()).Name);
                        break;
                    }
                #endregion

                #region DriverReady
                case ZWNotification.Type.DriverReady:
                    {
                        m_homeId = m_notification.GetHomeId();
                        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Home ID", m_homeId.ToString(), pName);
                        Log.Info("Driver Ready.  Home ID: " + m_homeId.ToString());
                        break;
                    }
                #endregion

                #region DriverReset
                case ZWNotification.Type.DriverReset:
                    {
                        m_homeId = m_notification.GetHomeId();
                        Log.Info("Driver Reset.  Home ID: " + m_homeId.ToString());
                        break;
                    }
                #endregion

                #region NodeQueriesComplete
                case ZWNotification.Type.NodeQueriesComplete:
                    {
                        Node node = GetNode(m_notification.GetHomeId(), m_notification.GetNodeId());
                        m_manager.AddAssociation(m_homeId, node.ID, 1, m_manager.GetControllerNodeId(m_homeId));

                        Log.Info("Node Queries Complete | " + node.ID + " | " + m_manager.GetNodeProductName(m_homeId, node.ID));
                        break;
                    }
                #endregion

                #region EssentialNodeQueriesComplete
                case ZWNotification.Type.EssentialNodeQueriesComplete:
                    {
                        Node node = GetNode(m_homeId, m_notification.GetNodeId());
                        Log.Info("Essential Node Queries Completee | " + node.ID + " | " + m_manager.GetNodeProductName(m_homeId, node.ID));
                        break;
                    }
                #endregion

                #region AllNodesQueried
                case ZWNotification.Type.AllNodesQueried:
                    {
                        Log.Info("All nodes queried");
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
                        Log.Info("Awake nodes queried (but some sleeping nodes have not been queried)");
                        foreach (Node n in m_nodeList)
                        {
                            OSAEObject obj = OSAEObjectManager.GetObjectByAddress("Z" + n.ID.ToString());
                            if (obj != null)
                            {
                                if (OSAEObjectPropertyManager.GetObjectPropertyValue(OSAEObjectManager.GetObjectByAddress("Z" + n.ID.ToString()).Name, "Poll").Value == "TRUE")
                                {
                                    Log.Info("Enabling polling for: " + obj.Name);
                                    enablePolling(n.ID);
                                }
                            }
                        }
                        break;
                    }
                #endregion
            }
        }

        public static void MyControllerStateChangedHandler(ZWControllerState state)
        {
            // Handle the controller state notifications here.
            bool complete = false;
            switch (state)
            {
                case ZWControllerState.Waiting:
                    {
                        Log.Info("Waiting...");
                        break;
                    }
                case ZWControllerState.InProgress:
                    {
                        // Tell the user that the controller has been found and the adding process is in progress.
                        Log.Info("Please wait...");
                        break;
                    }
                case ZWControllerState.Completed:
                    {
                        // Tell the user that the controller has been successfully added.
                        // The command is now complete
                        Log.Info("Command Completed OK.");
                        complete = true;
                        break;
                    }
                case ZWControllerState.Failed:
                    {
                        // Tell the user that the controller addition process has failed.
                        // The command is now complete
                        Log.Info("Command Failed.");
                        complete = true;
                        break;
                    }
                case ZWControllerState.NodeOK:
                    {
                        Log.Info("Node has not failed.");
                        complete = true;
                        break;
                    }
                case ZWControllerState.NodeFailed:
                    {
                        Log.Info("Node has failed.");
                        complete = true;
                        break;
                    }
            }


            if (complete)
            {
                Log.Info("Removing event handler");
                // Remove the event handler
                m_manager.OnControllerStateChanged -= m_controllerStateChangedHandler;
            }
        }


        private Node GetNode(UInt32 homeId, Byte nodeId)
        {
            foreach (Node node in m_nodeList)
            {
                if ((node.ID == nodeId) && (node.HomeID == homeId)) return node;
            }
            return new Node();
        }

        private void enablePolling(byte nid)
        {
            Log.Info("Attempting to Enable Polling: " + OSAEObjectManager.GetObjectByAddress("Z" + nid.ToString()).Name);
            try
            {
                Node n = GetNode(m_homeId, nid);
                ZWValueID zv = null;
                switch (n.Label)
                {
                    case "Toggle Switch":
                    case "Binary Toggle Switch":
                    case "Binary Switch":
                    case "Binary Power Switch":
                    case "Binary Scene Switch":
                    case "Binary Toggle Remote Switch":
                        foreach (Value v in n.Values)
                        {
                            if (v.Label == "Switch") m_manager.EnablePoll(v.ValueID);
                        }
                        break;
                    case "Multilevel Toggle Remote Switch":
                    case "Multilevel Remote Switch":
                    case "Multilevel Toggle Switch":
                    case "Multilevel Switch":
                    case "Multilevel Power Switch":
                    case "Multilevel Scene Switch":
                    case "Multiposition Motor":
                    case "Motor Control Class A":
                    case "Motor Control Class B":
                    case "Motor Control Class C":
                        foreach (Value v in n.Values)
                        {
                            if (v.Genre == ZWValueID.ValueGenre.User && v.Label == "Level") m_manager.EnablePoll(v.ValueID);
                        }
                        break;
                    case "General Thermostat V2":
                    case "Heating Thermostat":
                    case "General Thermostat":
                    case "Setback Schedule Thermostat":
                    case "Setpoint Thermostat":
                    case "Setback Thermostat":
                        foreach (Value v in n.Values)
                        {
                            if (v.Label == "Temperature") m_manager.EnablePoll(v.ValueID);
                            if (v.Label == "Fan State") m_manager.EnablePoll(v.ValueID);
                            if (v.Label == "Operating State") m_manager.EnablePoll(v.ValueID);
                            if (v.Label == "Mode") m_manager.EnablePoll(v.ValueID);
                        }
                        break;
                    case "Static PC Controller":
                    case "Static Controller":
                    case "Portable Remote Controller":
                    case "Portable Installer Tool":
                    case "Static Scene Controller":
                    case "Static Installer Tool":
                        break;
                    case "Secure Keypad Door Lock":
                    case "Advanced Door Lock":
                    case "Door Lock":
                    case "Entry Control":
                        foreach (Value v in n.Values)
                        {
                            if (v.Genre == ZWValueID.ValueGenre.User && v.Label == "Basic") m_manager.EnablePoll(v.ValueID);
                        }
                        break;
                    case "Alarm Sensor":
                    case "Basic Routing Alarm Sensor":
                    case "Routing Alarm Sensor":
                    case "Basic Zensor Alarm Sensor":
                    case "Zensor Alarm Sensor":
                    case "Advanced Zensor Alarm Sensor":
                    case "Basic Routing Smoke Sensor":
                    case "Routing Smoke Sensor":
                    case "Basic Zensor Smoke Sensor":
                    case "Zensor Smoke Sensor":
                    case "Advanced Zensor Smoke Sensor":
                    case "Routing Binary Sensor":
                        foreach (Value v in n.Values)
                        {
                            if (v.Genre == ZWValueID.ValueGenre.User && v.Label == "Basic") m_manager.EnablePoll(v.ValueID);
                        }
                        break;
                }
            }
            catch (Exception ex)
            { Log.Error("Error attempting to enable polling", ex); }
        }
        
        private Task StartOpenzwaveAsync()
        {
            if (isShuttingDown)
            {
                Log.Info("ZWave driver cannot start because it is still shutting down");
                return Task.FromResult(0);
            }

            try
            {
                string port = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Port").Value;
                if (port != "")
                {
                    // Create the Options
                    m_options = new ZWOptions();
                    m_options.Create(Common.ApiPath + @"\Plugins\ZWave\config\", Common.ApiPath + @"\Plugins\ZWave\", @"");
                    // Lock the options
                    m_options.Lock();

                    // Create the OpenZWave Manager
                    m_manager = new ZWManager();
                    m_manager.Create();
                    m_manager.OnNotification += new ManagedNotificationsHandler(NotificationHandler);

                    // Add a driver
                    m_manager.AddDriver(@"\\.\COM" + port);

                    int poll = 60;
                    if (OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Polling Interval").Value != string.Empty)
                        poll = Int32.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Polling Interval").Value);
                    {
                        Log.Info("Setting poll interval: " + poll.ToString());
                        m_manager.SetPollInterval(poll * 1000, true);
                    }

                    Log.Info(Common.ApiPath + @"\Plugins\ZWave\Config");
                    Log.Info("Zwave plugin initialized");
                }
            }
            catch (Exception e)
            { Log.Error("Error initalizing plugin", e); }

            return Task.FromResult(0);
        }

        private void StopOpenzwave()
        {
            if (!isShuttingDown)
            {
                isShuttingDown = true;
                
                if (m_manager != null)
                {
                    m_manager.OnNotification -= NotificationHandler;
                    m_manager.RemoveDriver(@"\\.\COM" + OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Port").Value);
                    m_manager.Destroy();
                    m_manager = null;
                }

                if (m_options != null)
                {
                    m_options.Destroy();
                    m_options = null;
                }

                isShuttingDown = false;
            }
        }
    }
}
