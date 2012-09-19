using System;
using System.AddIn;
using System.Collections.Generic;
using agsXMPP;
using OpenSourceAutomation;

namespace OSAE.Jabber
{
    [AddIn("Jabber", Version = "0.3.7")]
    public class Jabber : IOpenSourceAutomationAddInv2
    {
        OSAE osae = new OSAE("Jabber");
        XmppClientConnection xmppCon = new XmppClientConnection();
        string pName;
        bool shuttingDown = false;

        #region OSAPlugin Methods
        public void RunInterface(string pluginName)
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
            
            osae.AddToLog("Connecting to server...", true);
            connect();
        }

        public void ProcessCommand(OSAEMethod method)
        {
            try
            {
                //basically just need to send parameter two to the contact in parameter one with sendMessage();
                //Process incomming command
                string to = "";
                osae.AddToLog("Process command: " + method.MethodName, false);
                osae.AddToLog("Message: " + method.Parameter2, false);
                osae.AddToLog("To: " + method.Parameter1, false);
                ObjectProperty prop = osae.GetObjectPropertyValue(method.Parameter1, "JabberID");
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
                        sendMessage(osae.PatternParse(method.Parameter2), to);
                        break;

                    case "SEND FROM LIST":
                        sendMessage(osae.PatternParse(osae.ObjectPropertyArrayGetRandom("Speech List", method.Parameter2)), to);
                        break;
                }
            }
            catch (Exception ex)
            {
                osae.AddToLog("Error processing command: " +ex.Message, true);
            }
        }

        public void Shutdown()
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

            osae.AddToLog(String.Format("OnMessage from:{0} type:{1}", msg.From.Bare, msg.Type.ToString()), false);
            osae.AddToLog("Message: " + msg.Body, false);
            string pattern = osae.MatchPattern(msg.Body);
            if(pattern != "")
                osae.MethodQueueAdd("Script Processor", "NAMED SCRIPT", pattern, msg.From.Bare);

  
        }

        void xmppCon_OnClose(object sender)
        {
            osae.AddToLog("OnClose Connection closed", true);
            if (!shuttingDown)
            {
                osae.AddToLog("Connection closed unexpectedly.  Attempting reconnect...", true);
                connect();
            }
        }

        void xmppCon_OnError(object sender, Exception ex)
        {
            osae.AddToLog("OnError", true);
        }

        void xmppCon_OnAuthError(object sender, agsXMPP.Xml.Dom.Element e)
        {
            osae.AddToLog("OnAuthError", true);
        }

        void xmppCon_OnPresence(object sender, agsXMPP.protocol.client.Presence pres)
        {
            osae.AddToLog(String.Format("Received Presence from:{0} show:{1} status:{2}", pres.From.ToString(), pres.Show.ToString(), pres.Status), false);
            List<OSAEObject> objects = osae.GetObjectsByType("PERSON");

            foreach (OSAEObject oObj in objects)
            {
                OSAEObject obj = osae.GetObjectByName(oObj.Name);
                if (osae.GetObjectPropertyValue(obj.Name, "JabberID").Value == pres.From.Bare)
                {
                    if (pres.Show.ToString() == "away")
                        osae.ObjectPropertySet(obj.Name, "JabberStatus", "Idle");
                    else if (pres.Show.ToString() == "NONE")
                        osae.ObjectPropertySet(obj.Name, "JabberStatus", "Online");
                    break;
                }
            }
        }

        void xmppCon_OnRosterItem(object sender, agsXMPP.protocol.iq.roster.RosterItem item)
        {
            bool found = false;
            osae.AddToLog(String.Format("Received Contact {0}", item.Jid.Bare), true);
            List<OSAEObject> objects = osae.GetObjectsByType("PERSON");

            foreach (OSAEObject oObj in objects)
            {
                OSAEObject obj = osae.GetObjectByName(oObj.Name);
                if (osae.GetObjectPropertyValue(obj.Name, "JabberID").Value == item.Jid.Bare)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                osae.ObjectAdd(item.Jid.Bare, item.Jid.Bare, "PERSON", "", "",true);
                osae.ObjectPropertySet(item.Jid.Bare, "JabberID", item.Jid.Bare);
            }
        }

        void xmppCon_OnRosterEnd(object sender)
        {
            osae.AddToLog("OnRosterEnd", true);

            // Send our own presence to teh server, so other epople send us online
            // and the server sends us the presences of our contacts when they are
            // available
            xmppCon.SendMyPresence();
        }

        void xmppCon_OnRosterStart(object sender)
        {
            osae.AddToLog("OnRosterStart", true);
        }

        void xmppCon_OnLogin(object sender)
        {
            osae.AddToLog("OnLogin", true);
        }
        #endregion

        private void connect()
        {
            osae.AddToLog("Connecting to server", true);
            Jid jidUser = new Jid(osae.GetObjectPropertyValue(pName, "Username").Value);

            xmppCon.Username = jidUser.User;
            xmppCon.Server = jidUser.Server;
            xmppCon.Password = osae.GetObjectPropertyValue(pName, "Password").Value;
            xmppCon.AutoResolveConnectServer = true;

            try
            {
                xmppCon.Open();
            }
            catch (Exception ex)
            {
                osae.AddToLog("Error connecting: " + ex.Message, true);
            }
        }

        private void sendMessage(string message, string contact)
        {
            osae.AddToLog("Sending message: '" + message + "' to " + contact, false);
            // Send a message
            agsXMPP.protocol.client.Message msg = new agsXMPP.protocol.client.Message();
            msg.Type = agsXMPP.protocol.client.MessageType.chat;
            msg.To = new Jid(contact);
            msg.Body = message;

            xmppCon.Send(msg);
        }

    }
}
