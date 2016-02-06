using System;
using System.Collections.Generic;
using System.Windows;
using System.Speech.Recognition;
using System.Data;
using System.Threading;
using System.ComponentModel; 
using OSAE;
namespace VR2

{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SpeechRecognitionEngine oRecognizer = new SpeechRecognitionEngine();
        String gAppName = "";
        Boolean gVRMuted = false;
        Boolean gVREnabled = true;
        String gWakePattern = "VR Wake";
        String gSleepPattern = "Thanks";
        List<string> wakeList = new List<string>();
        List<string> sleepList = new List<string>();
        List<string> userList = new List<string>();
        String gSpeechPlugin = "";
        String gUser = "";
        Boolean gAppClosing = false;
        private System.Windows.Forms.NotifyIcon MyNotifyIcon;

        public MainWindow()
        {
            InitializeComponent();
            MyNotifyIcon = new System.Windows.Forms.NotifyIcon();
            MyNotifyIcon.Icon = Properties.Resources.icon;
            MyNotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(MyNotifyIcon_MouseDoubleClick);
            MyNotifyIcon.Visible = true;
            MyNotifyIcon.Text = "OSAE.Voice";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Load_App_Name();
            try
            {
                oRecognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(oRecognizer_SpeechRecognized);
                oRecognizer.AudioStateChanged += new EventHandler<AudioStateChangedEventArgs>(oRecognizer_StateChanged);
            }
            catch (Exception ex)
            {
                AddToLog("Unable to configure oRecognizer");
                AddToLog("Error: " + ex.Message);
            }
           
            Load_Settings();
            
            oRecognizer = OSAEGrammar.Load_User_Grammar(oRecognizer);
            try
            {
                gSpeechPlugin = OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Can Hear this Plugin").Value.ToString();
                if (gSpeechPlugin == "")
                {
                    gSpeechPlugin = "Speech";
                    OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "Can Hear this Plugin", "Speech","VR Client");
                }
                AddToLog("--  I will ignore speech from: " + gSpeechPlugin);

                oRecognizer = OSAEGrammar.Load_Direct_Grammar(oRecognizer);
                oRecognizer = OSAEGrammar.Load_Voice_Grammars(oRecognizer);
                SaveGrammars();

                AddToLog("Finished Loading...");
                AddToLog("_______________________________________________");
                AddToLog("Who are you?");

                Thread t1 = new Thread(delegate()
                {
                    oRecognizer.SetInputToDefaultAudioDevice();
                    oRecognizer.RecognizeAsync(RecognizeMode.Multiple);
                    while (!gAppClosing)
                        Thread.Sleep(333);
                });
                t1.Start();
            }
            catch (Exception ex)
            {
                    AddToLog("Unable to set Default Audio Device.  Check Sound Card.");
                    AddToLog("Error: " + ex.Message);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            OSAEObjectStateManager.ObjectStateSet(gAppName, "OFF", OSAE.Common.ComputerName);
            gAppClosing = true;
        }

        void MyNotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        private void AddToLog(string message)
        {
            try
            {
                txtLog.Text += DateTime.Now.ToString("hh:mm:ss") + "  " + message;
                txtLog.Text += Environment.NewLine;
                txtLog.ScrollToEnd();
            }
            catch { }
        }

        private void Load_App_Name()
        {
            try
            {
                gAppName = "VR CLIENT-" + OSAE.Common.ComputerName;
                bool found = OSAEObjectManager.ObjectExists(gAppName);
                if (!found)
                {
                    OSAEObjectManager.ObjectAdd(gAppName, "", gAppName, "VR CLIENT", "", OSAE.Common.ComputerName, 30, true);
                    AddToLog("Object Name: " + gAppName + " not found, so I created it.");
                }

                OSAEObjectStateManager.ObjectStateSet(gAppName, "ON", gAppName);
            }
            catch (Exception ex)
            { AddToLog("Error messaging host: " + ex.Message); }
        }

        private void Load_Settings()
        {
            try
            {
                AddToLog("OSA Settings:");
                AddToLog("--  Object Name: " + gAppName);
                string temp = OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "VR Input Muted").Value.ToString();
                if (temp == "FALSE")
                {
                    gVRMuted = false;
                    lblStatus.Content = "I am awake.";
                }
                else
                {
                    gVRMuted = true;
                    lblStatus.Content = "I am sleeping.  Zzzz";
                }
                AddToLog("--  VR Muted: " + gVRMuted);

                temp = "";
                temp = OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "VR Enabled").Value.ToString();
                if (temp == "FALSE")
                    gVREnabled = false;
                else
                    gVREnabled = true;
                AddToLog("--  VR Enabled: " + gVREnabled);

                temp = "";
                temp = OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "VR Wake Pattern").Value.ToString();
                gWakePattern = temp;
                AddToLog("--  VR Wake Pattern: " + gWakePattern);
                DataSet dsWakeResults = new DataSet();
                try
                {
                    //Load all unique patterns with no place-holders into a single grammer, our main one.
                    dsWakeResults = OSAESql.RunSQL("SELECT `match` FROM osae_v_pattern_match WHERE pattern ='" + gWakePattern + "' ORDER BY `match`");
                    for (int i = 0; i < dsWakeResults.Tables[0].Rows.Count; i++)
                    {
                        wakeList.Add(dsWakeResults.Tables[0].Rows[i][0].ToString());
                    }
                }
                catch (Exception ex)
                {
                    AddToLog("Error getting Wake matches from the DB!");
                    AddToLog("Error: " + ex.Message);
                }

                temp = "";
                temp = OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "VR Sleep Pattern").Value.ToString();
                gSleepPattern = temp;
                AddToLog("--  VR Sleep Pattern: " + gSleepPattern);
                DataSet dsSleepResults = new DataSet();
                try
                {
                    //Load all unique patterns with no place-holders into a single grammer, our main one.
                    dsSleepResults = OSAESql.RunSQL("SELECT `match` FROM osae_v_pattern_match WHERE pattern='" + gSleepPattern + "' ORDER BY `match`");
                    for (int i = 0; i < dsSleepResults.Tables[0].Rows.Count; i++)
                    {
                        sleepList.Add(dsSleepResults.Tables[0].Rows[i][0].ToString());
                    }
                }
                catch (Exception ex)
                {
                    AddToLog("Error getting Sleep matches from the DB!");
                    AddToLog("Error: " + ex.Message);
                }
            }
            catch (Exception ex)
            { AddToLog("Error in Load Settings: " + ex.Message); }
        }

        private void oRecognizer_SpeechRecognized(object sender, System.Speech.Recognition.SpeechRecognizedEventArgs e)
        {
              RecognitionResult result = e.Result;
              SemanticValue semantics = e.Result.Semantics;
              string scriptParameter = "";
              if (e.Result.Semantics.ContainsKey("PARAM1"))
              {
                  string temp = e.Result.Semantics["PARAM1"].Value.ToString().Replace("'s", "").Replace("'S", "");
                  if (temp.ToUpper() == "I" || temp.ToUpper() == "ME" || temp.ToUpper() == "MY") temp = gUser;
                  if (temp.ToUpper() == "YOU" || temp.ToUpper() == "YOUR") temp = "SYSTEM";
                  scriptParameter = temp;
                  if (e.Result.Semantics.ContainsKey("PARAM2"))
                  {
                      temp = e.Result.Semantics["PARAM2"].Value.ToString().Replace("'s", "").Replace("'S", "");
                      if (temp.ToUpper() == "I" || temp.ToUpper() == "ME" || temp.ToUpper() == "MY") temp = gUser;
                      if (temp.ToUpper() == "YOU" || temp.ToUpper() == "YOUR") temp = "SYSTEM";
                      scriptParameter += "," + temp;
                      if (e.Result.Semantics.ContainsKey("PARAM3"))
                      {
                          temp = e.Result.Semantics["PARAM3"].Value.ToString().Replace("'s", "").Replace("'S", "");
                          if (temp.ToUpper() == "I" || temp.ToUpper() == "ME" || temp.ToUpper() == "MY") temp = gUser;
                          if (temp.ToUpper() == "YOU" || temp.ToUpper() == "YOUR") temp = "SYSTEM";
                          scriptParameter += "," + temp;
                      }
                  }
              }
             // scriptParameter = scriptParameter.Replace();


              if (result.Grammar.Name.ToString() == "Direct Match")
                  ProcessInput(result.Text,result.Text, scriptParameter);
              else
                  ProcessInput(result.Text, result.Grammar.Name.ToString(), scriptParameter);
        }

        private void ProcessInput(string sRaw,string sInput,string scriptParamaters)
        {
            DataSet dsResults = new DataSet();
            try
            {
                if (wakeList.Contains(sInput) && gVRMuted == true)
                {
                    gVRMuted = false;
                    lblStatus.Content = "I am awake.";
                }
                else if (sleepList.Contains(sInput) && gVRMuted == false)
                {
                    gVRMuted = true;
                    lblStatus.Content = "I am sleeping. Zzzz";
                }
                else if (sleepList.Contains(sInput) && gVRMuted == true)
                    return;

                // gSpeechPlugin;
                string temp = OSAEObjectPropertyManager.GetObjectPropertyValue(gSpeechPlugin, "Speaking").Value.ToString().ToLower();

                if (temp.ToLower() == "false")
                {
                    if ((gVRMuted == false) || (sleepList.Contains(sInput)))
                    {
                        if (sInput.StartsWith("This is [OBJECT]"))
                        {
                            AddToLog("Heard: " + sRaw);
                            if (scriptParamaters != "")
                            {
                                if (gUser == scriptParamaters) return;
                                gUser = scriptParamaters;
                                AddToLog("I am talking to " + gUser);
                                lblObjectTalking.Content = "I am talking to " + gUser;
                                string sText = OSAEGrammar.SearchForMeaning(sInput, scriptParamaters, gUser);
                            }
                        }

                        if (gUser == "")
                        {
                            AddToLog("I must know who I am talking with.");
                            return;
                        }

                        try
                        {
                            string sLogEntry = "Heard: " + sRaw;
                            //string sText = OSAE.Common.MatchPattern(sInput,gUser);
                            //string sText = MatchPattern(sInput,gUser);
                            string sText = OSAEGrammar.SearchForMeaning(sInput, scriptParamaters, gUser);

                            if (sText.Length > 0)
                            {
                                sLogEntry += ".  Ran: " + sText;
                                OSAEObjectPropertyManager.ObjectPropertySet(gUser, "Communication Method", "Speech", gUser);
                            }
                            AddToLog(sLogEntry);
                        }
                        catch {}
                    }
                }
            }
            catch (Exception ex)
            { AddToLog("Error in _SpeechRecognized: " + ex.Message); }
        }

        private void SaveGrammars()
        {
           // string qualifier;
            List<Grammar> grammars = new List<Grammar>(oRecognizer.Grammars);
            AddToLog(grammars.Count + " Grammas loaded.");
         //   foreach (Grammar g in grammars)
         //   {
         //    qualifier = (g.Enabled) ? "enabled" : "disabled";

            //foreach (sr r in g)
            //   { AddToLog(String.Format("Grammar {0} is loaded and is {1}.", g.Name, g.RuleName)); }
            // }

        }

        private void oRecognizer_StateChanged(object sender, System.Speech.Recognition.AudioStateChangedEventArgs e)
        {
            switch (oRecognizer.AudioState.ToString())
            {
                case "Stopped":  
                    lblAudioState.Content = "I hear silence.";
                    break;
                case "Speech":
                    // gSpeechPlugin;
                    String temp = OSAEObjectPropertyManager.GetObjectPropertyValue(gSpeechPlugin, "Speaking").Value.ToString().ToLower();

                    if (temp.ToLower() == "true")
                        lblAudioState.Content = "I hear myself.";
                    else
                        lblAudioState.Content = "I hear talking.";
                    break;
                case "Silence":
                    lblAudioState.Content = "I hear silence.";
                    break;
            }
        }
    }
}
