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

        //OSAELog
        private OSAE.General.OSAELog Log = new General.OSAELog();

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
            
            this.Log.Info("Connecting to server...");
            connect();
        }

        public override void ProcessCommand(OSAEMethod method)
        {
            try
            {
                //basically just need to send parameter two to the contact in parameter one with sendMessage();
                //Process incomming command
                string to = "";
                this.Log.Debug("Process command: " + method.MethodName);
                this.Log.Debug("Message: " + method.Parameter2);
                OSAEObjectProperty prop = OSAEObjectPropertyManager.GetObjectPropertyValue(method.Parameter1, "JabberID");

                if(prop != null)
                    to = prop.Value;
                else
                    to = method.Parameter1;

                if (to == "")
                    to = method.Parameter1;

                this.Log.Debug("To: " + to);

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
                this.Log.Error("Error processing command ", ex);
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

            this.Log.Debug(String.Format("OnMessage from:{0} type:{1}", msg.From.Bare, msg.Type.ToString()));
            this.Log.Debug("Message: " + msg.Body);
            string pattern = Common.MatchPattern(msg.Body);
          //  if (pattern != string.Empty)
           // {
                //OSAEScriptManager.RunPatternScript(pattern, msg.From.Bare, "Jabber");
           // }             
        }

        void xmppCon_OnClose(object sender)
        {
            this.Log.Info("OnClose Connection closed");
            if (!shuttingDown)
            {
                this.Log.Info("Connection closed unexpectedly.  Attempting reconnect...");
                connect();
            }
        }

        void xmppCon_OnError(object sender, Exception ex)
        {
            this.Log.Info("OnError");
        }

        void xmppCon_OnAuthError(object sender, agsXMPP.Xml.Dom.Element e)
        {
            this.Log.Info("OnAuthError");
        }

        void xmppCon_OnPresence(object sender, agsXMPP.protocol.client.Presence pres)
        {
            this.Log.Debug(String.Format("Received Presence from:{0} show:{1} status:{2}", pres.From.ToString(), pres.Show.ToString(), pres.Status));

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
            this.Log.Info(String.Format("Received Contact {0}", item.Jid.Bare));

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
            this.Log.Info("OnRosterEnd");

            // Send our own presence to teh server, so other epople send us online
            // and the server sends us the presences of our contacts when they are
            // available
            xmppCon.SendMyPresence();
        }

        void xmppCon_OnRosterStart(object sender)
        {
            this.Log.Info("OnRosterStart");
        }

        void xmppCon_OnLogin(object sender)
        {
            this.Log.Info("OnLogin");
        }
        #endregion

        private void connect()
        {
            Jid jidUser = new Jid(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Username").Value);

            xmppCon.Username = jidUser.User;
            xmppCon.Server = jidUser.Server;
            xmppCon.Password = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Password").Value;
            xmppCon.AutoResolveConnectServer = true;
            this.Log.Info("Connecting to: " + xmppCon.Server + " as user: " + xmppCon.Username);

            try
            {
                xmppCon.Open();
            }
            catch (Exception ex)
            {
                this.Log.Error("Error connecting ", ex);
            }
        }

        private void sendMessage(string message, string contact)
        {
            this.Log.Debug("Sending message: '" + message + "' to " + contact);
            // Send a message
            agsXMPP.protocol.client.Message msg = new agsXMPP.protocol.client.Message();
            msg.Type = agsXMPP.protocol.client.MessageType.chat;
            msg.To = new Jid(contact);
            msg.Body = message;

            xmppCon.Send(msg);
        }

    }
}
