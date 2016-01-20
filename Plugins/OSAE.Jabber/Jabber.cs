using System;
using System.Collections.Generic;
using System.Data;
using agsXMPP;
using System.Threading;
using System.Speech.Recognition;

namespace OSAE.Jabber
{
    public class Jabber : OSAEPluginBase
    {
        XmppClientConnection xmppCon = new XmppClientConnection();
        SpeechRecognitionEngine oRecognizer = new SpeechRecognitionEngine();
        string gAppName;
        bool shuttingDown = false;
        bool gDebug = false;
        string gCurrentUser = "";
        string gCurrentAddress = "";
        string gAnswerObject = "";
        string gAnswerProperty = "";
        private OSAE.General.OSAELog Log = new General.OSAELog();
        private agsXMPP.protocol.client.Message oldMmsg;

        public override void RunInterface(string pluginName)
        {
            gAppName = pluginName;
            if (OSAEObjectManager.ObjectExists(gAppName))
                Log.Info("Found the Jabber plugin's Object (" + gAppName + ")");

            try
            {
                gDebug = Convert.ToBoolean(OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Debug").Value);
            }
            catch
            { Log.Info("The JABBER Object Type seems to be missing the Debug Property!"); }
            Log.Info("Debug Mode Set to " + gDebug);

            OwnTypes();

            try
            {
                oRecognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(oRecognizer_SpeechRecognized);
                //oRecognizer.AudioStateChanged += new EventHandler<AudioStateChangedEventArgs>(oRecognizer_StateChanged);
            }
            catch (Exception ex)
            { Log.Error("Unable to configure oRecognizer", ex); }

            oRecognizer = OSAEGrammar.Load_Direct_Grammar(oRecognizer);
            Log.Info("Load_Direct_Grammar completed");
            oRecognizer = OSAEGrammar.Load_Voice_Grammars(oRecognizer);
            Log.Info("Load_Voice_Grammars completed");
            oRecognizer = OSAEGrammar.Load_Text_Only_Grammars(oRecognizer);
            Log.Info("Load_Text_Only_Grammars completed");
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

            connect();
        }

        public void OwnTypes()
        {
            //Added the follow to automatically own Speech Base types that have no owner.
            OSAEObjectType oType = OSAEObjectTypeManager.ObjectTypeLoad("JABBER");

            if (oType.OwnedBy == "")
            {
                OSAEObjectTypeManager.ObjectTypeUpdate(oType.Name, oType.Name, oType.Description, gAppName, oType.BaseType, oType.Owner, oType.SysType, oType.Container, oType.HideRedundant);
                Log.Info("Jabber Plugin took ownership of the JABBER Object Type.");
            }
            else
                Log.Info("Jabber Plugin correctly owns the JABBER Object Type.");
        }

        public override void ProcessCommand(OSAEMethod method)
        {
            try
            {
                //basically just need to send parameter two to the contact in parameter one with sendMessage();
                //Process incomming command
                string to = "";
                if (gDebug) Log.Debug("Process command: " + method.MethodName);
                if (gDebug) Log.Debug("Message: " + method.Parameter2);
                OSAEObjectProperty prop = OSAEObjectPropertyManager.GetObjectPropertyValue(method.Parameter1, "JabberID");

                if (prop != null)
                    to = prop.Value;
                else
                    to = method.Parameter1;

                if (to == "") to = method.Parameter1;

                if (gDebug) Log.Debug("To: " + to);

                switch (method.MethodName)
                {
                    case "SEND MESSAGE":
                        sendMessage(Common.PatternParse(method.Parameter2), to);
                        break;

                    case "SEND QUESTION":
                        Ask_Question(to);
                        break;

                    case "SEND FROM LIST":
                        string speechList = method.Parameter2.Substring(0,method.Parameter2.IndexOf(","));
                        string listItem = method.Parameter2.Substring(method.Parameter2.IndexOf(",") + 1, method.Parameter2.Length - (method.Parameter2.IndexOf(",")+ 1));
                        if (gDebug) Log.Debug("List = " + speechList + "   Item=" + listItem);
                        sendMessage(Common.PatternParse(OSAEObjectPropertyManager.ObjectPropertyArrayGetRandom(speechList, listItem)), to);
                        break;
                }
            }
            catch (Exception ex)
            { Log.Error("Error in ProcessCommand!", ex); }
        }

        public void Ask_Question(string address)
        {
            //OSAEObjectProperty prop = OSAEObjectPropertyManager.GetObjectPropertyValue("Vaughn", "JabberID");
            DataSet dataset = new DataSet();
            dataset = OSAESql.RunSQL("SELECT Question,object_name,property_name,property_datatype,property_object_type,interest_level FROM osae_v_interests ORDER BY interest_level DESC LIMIT 1");
            if (dataset.Tables[0].Rows.Count > 0)
            {
                string sQuestion = dataset.Tables[0].Rows[0]["Question"].ToString();
                sendMessage(Common.PatternParse(sQuestion), address);
                //Unload All grammars
                oRecognizer.UnloadAllGrammars();
                //Load Answer grammer based on Property Object Type plus Unknown and None
                oRecognizer = OSAEGrammar.Load_Answer_Grammar(oRecognizer, dataset.Tables[0].Rows[0]["property_datatype"].ToString(), dataset.Tables[0].Rows[0]["property_object_type"].ToString());
                //set a AskingQuestionVariables so recognition can run an QuestionedAnswered routine
                gAnswerObject = dataset.Tables[0].Rows[0]["object_name"].ToString();
                gAnswerProperty = dataset.Tables[0].Rows[0]["property_name"].ToString();
                Log.Info("Asking: " + sQuestion + "  (" + gAnswerObject + "," + gAnswerProperty  + ")");
            }
        }

        public void Question_Answered(string answer)
        {
            oRecognizer.UnloadAllGrammars();
            oRecognizer = OSAEGrammar.Load_Direct_Grammar(oRecognizer);
            Log.Info("Load_Direct_Grammar completed");
            oRecognizer = OSAEGrammar.Load_Voice_Grammars(oRecognizer);
            Log.Info("Load_Voice_Grammars completed");
            oRecognizer = OSAEGrammar.Load_Text_Only_Grammars(oRecognizer);
            Log.Info("Load_Text_Only_Grammars completed");

            sendMessage(Common.PatternParse("Setting " + gAnswerObject + "'s " + gAnswerProperty + " to " + answer), gCurrentAddress);

            Log.Info(Common.PatternParse("Setting " + gAnswerObject + "'s " + gAnswerProperty + " to " + answer));
            OSAEObjectPropertyManager.ObjectPropertySet(gAnswerObject, gAnswerProperty, answer, gCurrentUser);
            //Trust is enforced in the storedProc, but maybe it can be checked here for better replies.
                gAnswerObject = "";
                gAnswerProperty = "";
        }

        public override void Shutdown()
        {
            shuttingDown = true;
            xmppCon.Close();
            Log.Info("Shutdown!");
        }

        void xmppCon_OnMessage(object sender, agsXMPP.protocol.client.Message msg)
        {
            try
            {
                // ignore empty messages (events)
                if (msg.Body == null || oldMmsg == msg || msg.Type == agsXMPP.protocol.client.MessageType.error) return;
                oldMmsg = msg;
                if (gDebug) Log.Debug("Searching for: " + msg.From.Bare);
                DataSet dsResults = new DataSet();  //Build a List of all Users to identify who is sending the message.
                dsResults = OSAESql.RunSQL("SELECT DISTINCT(object_name) FROM osae_v_object_property WHERE property_name = 'JabberID' and property_value = '" + msg.From.Bare + "' ORDER BY object_name");
                gCurrentUser = dsResults.Tables[0].Rows[0][0].ToString();
                if (gDebug) Log.Debug("gCurrentUser: " + dsResults.Tables[0].Rows[0][0].ToString());
                OSAEObjectPropertyManager.ObjectPropertySet(gCurrentUser, "Communication Method", "Jabber", gCurrentUser);
                gCurrentAddress = msg.From.Bare;
                if (msg.Body.EndsWith("?") || msg.Body.EndsWith("!") || msg.Body.EndsWith("."))
                    msg.Body = msg.Body.Substring(0, msg.Body.Length -1);

                RecognitionResult rr = oRecognizer.EmulateRecognize(msg.Body);
                if (rr == null)
                    if (gDebug) Log.Debug("INPUT: No Matching Pattern found!");
            }
            catch (Exception ex)
            { Log.Error("Error in _OnMessage!", ex); }
        }

        void xmppCon_OnClose(object sender)
        {
            Log.Info("OnClose Connection Closed");
            if (!shuttingDown)
            {
                Log.Info("Connection Closed unexpectedly. Attempting Reconnect in 5 seconds.");
                Thread.Sleep(5000);
                connect();
            }
        }

        void xmppCon_OnError(object sender, Exception ex)
        {
            Log.Error("OnError", ex);
        }

        void xmppCon_OnAuthError(object sender, agsXMPP.Xml.Dom.Element e)
        {
            Log.Error("OnAuthError");
        }

        void xmppCon_OnPresence(object sender, agsXMPP.protocol.client.Presence pres)
        {
            Log.Info(String.Format("Received Presence from: {0} show: {1} status: {2}", pres.From.ToString(), pres.Show.ToString(), pres.Status));

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
            OSAEObject obj = OSAEObjectManager.GetObjectByPropertyValue("JabberID", item.Jid.Bare);

            if (obj != null)
                Log.Info(String.Format("Received contact from {0} ({1})", obj.Name, item.Jid.Bare));
            else
            { 
                Log.Info(String.Format("Received NEW Contact {0}", item.Jid.Bare));
                OSAEObjectManager.ObjectAdd(item.Jid.Bare, "", "Discovered Jabber contact", "THING", "", "Unknown", 10, true);
                OSAEObjectPropertyManager.ObjectPropertySet(item.Jid.Bare, "JabberID", item.Jid.Bare, "Jabber");
            }


            //   }
            //            bool found = false;
            // OSAEObjectCollection objects = OSAEObjectManager.GetObjectsByType("PERSON");

            // foreach (OSAEObject oObj in objects)
            //  {
            //      OSAEObject obj = OSAEObjectManager.GetObjectByName(oObj.Name);
            //      if (OSAEObjectPropertyManager.GetObjectPropertyValue(obj.Name, "JabberID").Value == item.Jid.Bare)
            //     {
            //         found = true;
            //        Log.Info(String.Format("Received contact from {0} ({1})", obj.Name, item.Jid.Bare));
            //        break;
            //     }
            //  }

            //  if (!found)
            //   {
            //      Log.Info(String.Format("Received NEW Contact {0}", item.Jid.Bare));
            //      OSAEObjectManager.ObjectAdd(item.Jid.Bare, "", "Discovered Jabber contact", "PERSON", "", "Unknown", 50, true);
            //     OSAEObjectPropertyManager.ObjectPropertySet(item.Jid.Bare, "JabberID", item.Jid.Bare, "Jabber");
            //   }
        }

        void xmppCon_OnRosterEnd(object sender)
        {
            if (gDebug) Log.Debug("OnRosterEnd");

            // Send our own presence to teh server, so other epople send us online
            // and the server sends us the presences of our contacts when they are
            // available
            xmppCon.SendMyPresence();
        }

        void xmppCon_OnRosterStart(object sender)
        {
            if (gDebug) Log.Debug("OnRosterStart");
        }

        void xmppCon_OnLogin(object sender)
        {
            if (gDebug) Log.Debug("OnLogin");
        }

        private void connect()
        {
            Jid jidUser = new Jid(OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Username").Value);

            xmppCon.Username = jidUser.User;
            xmppCon.Server = jidUser.Server;
            xmppCon.Password = OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Password").Value;
            xmppCon.AutoResolveConnectServer = true;
            Log.Info("Connecting to: " + xmppCon.Server + " as user: " + xmppCon.Username);

            try
            {
                xmppCon.Open();
            }
            catch (Exception ex)
            { Log.Error("Error connecting: ", ex);}
        }

        private void sendMessage(string message, string contact)
        {
            if (gDebug) Log.Debug("OUTPUT: '" + message + "' to " + contact);
            // Send a message
            agsXMPP.protocol.client.Message msg = new agsXMPP.protocol.client.Message();
            msg.Type = agsXMPP.protocol.client.MessageType.chat;
            msg.To = new Jid(contact);
            msg.Body = message;

            xmppCon.Send(msg);
        }

        private void oRecognizer_SpeechRecognized(object sender, System.Speech.Recognition.SpeechRecognizedEventArgs e)
        {
            try
            {
                RecognitionResult result = e.Result;
                SemanticValue semantics = e.Result.Semantics;
                string scriptParameter = "";
                if (e.Result.Semantics.ContainsKey("PARAM1"))
                {
                    string temp = e.Result.Semantics["PARAM1"].Value.ToString().Replace("'s", "").Replace("'S", "");
                    if (temp.ToUpper() == "I" || temp.ToUpper() == "ME" || temp.ToUpper() == "MY") temp = gCurrentUser;
                    if (temp.ToUpper() == "YOU" || temp.ToUpper() == "YOUR") temp = "SYSTEM";
                    scriptParameter = temp;
                    if (e.Result.Semantics.ContainsKey("PARAM2"))
                    {
                        temp = e.Result.Semantics["PARAM2"].Value.ToString().Replace("'s", "").Replace("'S", "");
                        if (temp.ToUpper() == "I" || temp.ToUpper() == "ME" || temp.ToUpper() == "MY") temp = gCurrentUser;
                        if (temp.ToUpper() == "YOU" || temp.ToUpper() == "YOUR") temp = "SYSTEM";
                        scriptParameter += "," + temp;
                        if (e.Result.Semantics.ContainsKey("PARAM3"))
                        {
                            temp = e.Result.Semantics["PARAM3"].Value.ToString().Replace("'s", "").Replace("'S", "");
                            if (temp.ToUpper() == "I" || temp.ToUpper() == "ME" || temp.ToUpper() == "MY") temp = gCurrentUser;
                            if (temp.ToUpper() == "YOU" || temp.ToUpper() == "YOUR") temp = "SYSTEM";
                            scriptParameter += "," + temp;
                        }
                    }
                }
                // scriptParameter = scriptParameter.Replace();
                string sResults = "";
                if (e.Result.Semantics.ContainsKey("ANSWER"))
                    Question_Answered(e.Result.Semantics["ANSWER"].Value.ToString());
                else
                {
                    if (result.Grammar.Name.ToString() == "Direct Match")
                    {
                        Log.Debug("Searching for: " + sResults);
                        sResults = OSAEGrammar.SearchForMeaning(result.Text, scriptParameter, gCurrentUser);
                    }
                    else
                    {
                        Log.Debug("Searching for: " + sResults);
                        sResults = OSAEGrammar.SearchForMeaning(result.Grammar.Name.ToString(), scriptParameter, gCurrentUser);
                    }
                }
                Log.Info(gCurrentUser + ": " + e.Result.Text );
                Log.Info(" meaning = " + sResults);
            }
            catch (Exception ex)
            { Log.Error("Error in _SpeechRecognized!", ex); }
        }

    }
}
