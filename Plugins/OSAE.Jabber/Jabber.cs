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
        string gSystemName = "SYSTEM";
        private OSAE.General.OSAELog Log;// = new General.OSAELog();
        private agsXMPP.protocol.client.Message oldMmsg;

        public override void RunInterface(string pluginName)
        {
            gAppName = pluginName;
            Log = new General.OSAELog(gAppName);

            try
            {
                gDebug = Convert.ToBoolean(OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Debug").Value);
            }
            catch
            { Log.Info("The JABBER Object Type seems to be missing the Debug Property!"); }
            Log.Info("Debug Mode Set to " + gDebug);

            OwnTypes();

            OSAE.OSAEObject tempAlias = OSAE.OSAEObjectManager.GetObjectByName(gSystemName);
            if (tempAlias.Alias.Length > 0) gSystemName = tempAlias.Alias;

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
                OSAEObjectTypeManager.ObjectTypeUpdate(oType.Name, oType.Name, oType.Description, gAppName, oType.BaseType, oType.Owner, oType.SysType, oType.Container, oType.HideRedundant, oType.Tooltip);
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
                string to = "";
                if (gDebug) Log.Debug("Process command: " + method.MethodName);
                if (gDebug) Log.Debug("Message: " + method.Parameter2);
                OSAEObjectProperty prop = OSAEObjectPropertyManager.GetObjectPropertyValue(method.Parameter1, "JabberID");

                if (prop != null) to = prop.Value;
                else to = method.Parameter1;

                if (to == "") to = method.Parameter1;

                switch (method.MethodName)
                {
                    case "SEND MESSAGE":
                        sendMessage(method.Parameter1, to, Common.PatternParse(method.Parameter2));
                        break;

                    case "SEND QUESTION":
                        Ask_Question(to);
                        break;

                    case "SEND FROM LIST":
                        string speechList = method.Parameter2.Substring(0,method.Parameter2.IndexOf(","));
                        string listItem = method.Parameter2.Substring(method.Parameter2.IndexOf(",") + 1, method.Parameter2.Length - (method.Parameter2.IndexOf(",")+ 1));
                        if (gDebug) Log.Debug("List = " + speechList + "   Item=" + listItem);
                        sendMessage(method.Parameter1, to, Common.PatternParse(OSAEObjectPropertyManager.ObjectPropertyArrayGetRandom(speechList, listItem)));
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
                sendMessage(gCurrentUser, Common.PatternParse(sQuestion), address);
                //Unload All grammars
                oRecognizer.UnloadAllGrammars();
                //Load Answer grammer based on Property Object Type plus Unknown and None
                oRecognizer = OSAEGrammar.Load_Answer_Grammar(oRecognizer, dataset.Tables[0].Rows[0]["property_datatype"].ToString(), dataset.Tables[0].Rows[0]["property_object_type"].ToString());
                //set a AskingQuestionVariables so recognition can run an QuestionedAnswered routine
                gAnswerObject = dataset.Tables[0].Rows[0]["object_name"].ToString();
                gAnswerProperty = dataset.Tables[0].Rows[0]["property_name"].ToString();
                Log.Info("-> Asking: " + sQuestion + "  (" + gAnswerObject + "," + gAnswerProperty  + ")");
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

            sendMessage(gCurrentUser,Common.PatternParse("Setting " + gAnswerObject + "'s " + gAnswerProperty + " to " + answer), gCurrentAddress);

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

                string[] jIDRaw = msg.From.ToString().Split('/');
                string jID = jIDRaw[0];
                OSAEObjectCollection objects = OSAEObjectManager.GetObjectsByPropertyValue("JabberID", jID);
                if (objects != null)
                {
                    foreach (OSAEObject oObj in objects)
                    {
                        if (oObj.Alias.Length > 0) gCurrentUser = oObj.Alias;
                        else gCurrentUser = oObj.Name;
                    }
                }
                else
                    Log.Info("Message from Unknown address: " + jID);

                if (gDebug) Log.Debug("Current User set to: " + gCurrentUser);
                OSAEObjectPropertyManager.ObjectPropertySet(gCurrentUser, "Communication Method", "Jabber", gAppName);
                gCurrentAddress = jID;
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
            string[] jIDRaw = pres.From.ToString().Split('/');
            string jID = jIDRaw[0];
            OSAEObjectCollection objects = OSAEObjectManager.GetObjectsByPropertyValue("JabberID", jID);
            if (objects != null)
            {
                foreach (OSAEObject oObj in objects)
                {
                    Log.Info(String.Format("Received Presence from: {0} show: {1} status: {2}", oObj.Name, pres.Show.ToString(), pres.Status));
                    if (pres.Show.ToString() == "away")
                        OSAEObjectPropertyManager.ObjectPropertySet(oObj.Name, "JabberStatus", "Idle", "Jabber");
                    else if (pres.Show.ToString() == "NONE")
                        OSAEObjectPropertyManager.ObjectPropertySet(oObj.Name, "JabberStatus", "Online", "Jabber");
                    break;
                }
            }
            else
                Log.Info(String.Format("No object found with address of: {0}",jID, pres.Show.ToString(), pres.Status));

        }

        void xmppCon_OnRosterItem(object sender, agsXMPP.protocol.iq.roster.RosterItem item)
        {
            string[] jIDRaw = item.Jid.Bare.ToString().Split('/');
            string jID = jIDRaw[0];
            OSAEObjectCollection objects = OSAEObjectManager.GetObjectsByPropertyValue("JabberID", jID);
            if (objects == null)
            { 
                bool dupName = OSAEObjectManager.ObjectExists(item.Name);
                if (dupName)
                    Log.Info(String.Format("Found Object {0} for {1}", item.Name, jID));
                else
                {
                    Log.Info(String.Format("Creating new Object {0} for {1}", item.Name, jID));
                    OSAEObjectManager.ObjectAdd(item.Name, "", "Discovered Jabber contact", "PERSON", "", "Unknown", 10, false);
                }
                Log.Info(String.Format("Updating JabberID for {0}", item.Name));
                OSAEObjectPropertyManager.ObjectPropertySet(item.Name, "JabberID", jID, gAppName);
            }
            else
                Log.Info(String.Format("Found Object {0} for {1}", item.Name, jID));
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
                if (gDebug) Log.Debug("ID: " + xmppCon.MyJID.ToString());
                if (gDebug) Log.Debug("Authenticated: " + xmppCon.Authenticated.ToString());
            }
            catch (Exception ex)
            { Log.Error("Error connecting: ", ex);}
        }

        private void sendMessage(string name, string address, string message)
        {
            Log.Info("-> " + name + ": " + message + " (" + address + ")");
            // Send a message
            agsXMPP.protocol.client.Message msg = new agsXMPP.protocol.client.Message
            {
                Type = agsXMPP.protocol.client.MessageType.chat,
                To = new Jid(address),
                Body = message
            };

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
                    if (temp.ToUpper() == "YOU" || temp.ToUpper() == "YOUR") temp = gSystemName;
                    scriptParameter = temp;
                    if (e.Result.Semantics.ContainsKey("PARAM2"))
                    {
                        temp = e.Result.Semantics["PARAM2"].Value.ToString().Replace("'s", "").Replace("'S", "");
                        if (temp.ToUpper() == "I" || temp.ToUpper() == "ME" || temp.ToUpper() == "MY") temp = gCurrentUser;
                        if (temp.ToUpper() == "YOU" || temp.ToUpper() == "YOUR") temp = gSystemName;
                        scriptParameter += "," + temp;
                        if (e.Result.Semantics.ContainsKey("PARAM3"))
                        {
                            temp = e.Result.Semantics["PARAM3"].Value.ToString().Replace("'s", "").Replace("'S", "");
                            if (temp.ToUpper() == "I" || temp.ToUpper() == "ME" || temp.ToUpper() == "MY") temp = gCurrentUser;
                            if (temp.ToUpper() == "YOU" || temp.ToUpper() == "YOUR") temp = gSystemName;
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
                        Log.Debug("Search for Meaning of: " + result.Text);
                        sResults = OSAEGrammar.SearchForMeaning(result.Text, scriptParameter, gCurrentUser);
                    }
                    else
                    {
                        Log.Debug("Search for Meaning of: " + result.Grammar.Name.ToString());
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
