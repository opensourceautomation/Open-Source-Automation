using System;
using System.Collections.Generic;
using agsXMPP;

namespace OSAE.Jabber
{
    public class Jabber : OSAEPluginBase
    {
        XmppClientConnection xmppCon = new XmppClientConnection();
        string pName;
        bool shuttingDown = false;

        /// <summary>
        /// Provides access to logging
        /// </summary>
        Logging logging = Logging.GetLogger("Jabber");

        #region OSAPlugin Methods
        public override void RunInterface(string pluginName)
        {
            pName = pluginName;
            // Subscribe to Events
            xmppCon.OnLogin += new ObjectHandler(xmppCon_OnLogin);
            xmppCon.OnRosterStart += new ObjectHandler(xmppCon_OnRosterStart);
            xmppCon.OnRosterEnd += new ObjectHandler(xmppCon_OnRosterEnd);
            xmppCon.OnRosterItem += new XmppClientConnection.RosterHandler(xmppCon_OnRosterItem);
            xmppCon.OnPresence += new agsXMPP.protocol.client.PresenceHandler(xmppCon_OnPresence);
            xmppCon.OnAuthError += new XmppElementHandler(xmppCon_OnAuthError);
            xmppCon.OnError += new ErrorHandler(xmppCon_OnError);
            xmppCon.OnClose += new ObjectHandler(xmppCon_OnClose);
            xmppCon.OnMessage += new agsXMPP.protocol.client.MessageHandler(xmppCon_OnMessage);
            
            logging.AddToLog("Connecting to server...", true);
            connect();
        }

        public override void ProcessCommand(OSAEMethod method)
        {
            try
            {
                //basically just need to send parameter two to the contact in parameter one with sendMessage();
                //Process incomming command
                string to = "";
                logging.AddToLog("Process command: " + method.MethodName, false);
                logging.AddToLog("Message: " + method.Parameter2, false);
                logging.AddToLog("To: " + method.Parameter1, false);
                OSAEObjectProperty prop = OSAEObjectPropertyManager.GetObjectPropertyValue(method.Parameter1, "JabberID");
                if(prop != null)
                    to = prop.Value;
                    if (to == "")
                        to = method.Parameter1;
                else
                    to = method.Parameter1;

                if (to == "")
                    to = method.Parameter1;

                switch (method.MethodName)
                {
                    case "SEND MESSAGE":
                        sendMessage(Common.PatternParse(method.Parameter2), to);
                        break;

                    case "SEND FROM LIST":
                        sendMessage(Common.PatternParse(OSAEObjectPropertyManager.ObjectPropertyArrayGetRandom("Speech List", method.Parameter2)), to);
                        break;
                }
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error processing command: " +ex.Message, true);
            }
        }

        public override void Shutdown()
        {
            shuttingDown = true;
            xmppCon.Close();
        }
        #endregion

        #region Events
        void xmppCon_OnMessage(object sender, agsXMPP.protocol.client.Message msg)
        {

            // ignore empty messages (events)
            if (msg.Body == null)
                return;

            logging.AddToLog(String.Format("OnMessage from:{0} type:{1}", msg.From.Bare, msg.Type.ToString()), false);
            logging.AddToLog("Message: " + msg.Body, false);
            string pattern = Common.MatchPattern(msg.Body);
          //  if (pattern != string.Empty)
           // {
                //OSAEScriptManager.RunPatternScript(pattern, msg.From.Bare, "Jabber");
           // }             
        }

        void xmppCon_OnClose(object sender)
        {
            logging.AddToLog("OnClose Connection closed", true);
            if (!shuttingDown)
            {
                logging.AddToLog("Connection closed unexpectedly.  Attempting reconnect...", true);
                connect();
            }
        }

        void xmppCon_OnError(object sender, Exception ex)
        {
            logging.AddToLog("OnError", true);
        }

        void xmppCon_OnAuthError(object sender, agsXMPP.Xml.Dom.Element e)
        {
            logging.AddToLog("OnAuthError", true);
        }

        void xmppCon_OnPresence(object sender, agsXMPP.protocol.client.Presence pres)
        {
            logging.AddToLog(String.Format("Received Presence from:{0} show:{1} status:{2}", pres.From.ToString(), pres.Show.ToString(), pres.Status), false);

            OSAEObjectCollection objects = OSAEObjectManager.GetObjectsByType("PERSON");

            foreach (OSAEObject oObj in objects)
            {
                OSAEObject obj = OSAEObjectManager.GetObjectByName(oObj.Name);

                if (OSAEObjectPropertyManager.GetObjectPropertyValue(obj.Name, "JabberID").Value == pres.From.Bare)
                {
                    if (pres.Show.ToString() == "away")
                        OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "JabberStatus", "Idle", "Jabber");
                    else if (pres.Show.ToString() == "NONE")
                        OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "JabberStatus", "Online", "Jabber");
                    break;
                }
            }
        }

        void xmppCon_OnRosterItem(object sender, agsXMPP.protocol.iq.roster.RosterItem item)
        {
            bool found = false;
            logging.AddToLog(String.Format("Received Contact {0}", item.Jid.Bare), true);

            OSAEObjectCollection objects = OSAEObjectManager.GetObjectsByType("PERSON");

            foreach (OSAEObject oObj in objects)
            {
                OSAEObject obj = OSAEObjectManager.GetObjectByName(oObj.Name);
                if (OSAEObjectPropertyManager.GetObjectPropertyValue(obj.Name, "JabberID").Value == item.Jid.Bare)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                OSAEObjectManager.ObjectAdd(item.Jid.Bare, item.Jid.Bare, "PERSON", "", "", true);
                OSAEObjectPropertyManager.ObjectPropertySet(item.Jid.Bare, "JabberID", item.Jid.Bare, "Jabber");
            }
        }

        void xmppCon_OnRosterEnd(object sender)
        {
            logging.AddToLog("OnRosterEnd", true);

            // Send our own presence to teh server, so other epople send us online
            // and the server sends us the presences of our contacts when they are
            // available
            xmppCon.SendMyPresence();
        }

        void xmppCon_OnRosterStart(object sender)
        {
            logging.AddToLog("OnRosterStart", true);
        }

        void xmppCon_OnLogin(object sender)
        {
            logging.AddToLog("OnLogin", true);
        }
        #endregion

        private void connect()
        {
            logging.AddToLog("Connecting to server", true);
            Jid jidUser = new Jid(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Username").Value);

            xmppCon.Username = jidUser.User;
            xmppCon.Server = jidUser.Server;
            xmppCon.Password = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Password").Value;
            xmppCon.AutoResolveConnectServer = true;

            try
            {
                xmppCon.Open();
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error connecting: " + ex.Message, true);
            }
        }

        private void sendMessage(string message, string contact)
        {
            logging.AddToLog("Sending message: '" + message + "' to " + contact, false);
            // Send a message
            agsXMPP.protocol.client.Message msg = new agsXMPP.protocol.client.Message();
            msg.Type = agsXMPP.protocol.client.MessageType.chat;
            msg.To = new Jid(contact);
            msg.Body = message;

            xmppCon.Send(msg);
        }

    }
}
