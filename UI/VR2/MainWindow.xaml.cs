using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Speech;
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
        Boolean gVRMuted = true;
        Boolean gVREnabled = false;
        String gWakePattern = "VR Wake";
        List<string> wakeList = new List<string>();
        String gSleepPattern = "Thanks";
        List<string> sleepList = new List<string>();
        String gSpeechPlugin = "";
        Boolean gAppClosing = false;
        private System.Windows.Forms.NotifyIcon MyNotifyIcon;

        public MainWindow()
        {
            InitializeComponent();
            MyNotifyIcon = new System.Windows.Forms.NotifyIcon();
            MyNotifyIcon.Icon = Properties.Resources.icon;
            MyNotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(MyNotifyIcon_MouseDoubleClick);
            MyNotifyIcon.Visible = true;
            MyNotifyIcon.Text = "VR";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
          
            Load_App_Name();
            try
            {
                oRecognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(oRecognizer_SpeechRecognized);
                oRecognizer.AudioStateChanged += new EventHandler<AudioStateChangedEventArgs>(oRecognizer_StateChanged);
                AudioState state = oRecognizer.AudioState;  // This shows AudioState.Stopped
            }
            catch (Exception ex)
            {
                AddToLog("Unable to configure oRecognizer");
                AddToLog("Error: " + ex.Message);
            }
           
            Load_Settings();
            Load_Grammer();
            Load_Grammer_With_Substitutions();
            try
            {
                oRecognizer.SetInputToDefaultAudioDevice();
                AddToLog("Recognizer Settings: "); 
                AddToLog("--  Level: " + oRecognizer.AudioLevel.ToString());
                AddToLog("--  End Silence Timeout: " + oRecognizer.EndSilenceTimeout);
                AddToLog("--  Recognition Babble Timeout: " + oRecognizer.BabbleTimeout); 
                AddToLog("Audio Format Settings: "); 
                AddToLog("--  EncodingFormat: " + oRecognizer.AudioFormat.EncodingFormat);
                AddToLog("--  Channel Count: " + oRecognizer.AudioFormat.ChannelCount);
                AddToLog("--  Bits Per Sample: " + oRecognizer.AudioFormat.BitsPerSample);
                AddToLog("--  Samples Per Second: " + oRecognizer.AudioFormat.SamplesPerSecond);
                AddToLog("--  Average Bytes Per Second: " + oRecognizer.AudioFormat.AverageBytesPerSecond);
                gSpeechPlugin = OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Can Hear this Plugin").Value.ToString();
                AddToLog("--  I will ignore speech from: " + gSpeechPlugin);
                Thread t1 = new Thread(delegate()
                {
                    oRecognizer.SetInputToDefaultAudioDevice();
                    oRecognizer.RecognizeAsync();
                    while (!gAppClosing)
                    {
                        Thread.Sleep(333);
                    }
                });
                t1.Start();

                //oRecognizer.RecognizeAsync();

               AddToLog("Finished Loading, Recognition Started...");
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
            this.WindowState = WindowState.Normal;
        }

        private void AddToLog(string message)
        {
            try
            {
                txtLog.Text += message;
                txtLog.Text += Environment.NewLine;
                txtLog.ScrollToEnd();
            }
            catch (Exception ex)
            {
                //AddToLog("Error in AddToLog: " + ex.Message);
            }
        }

        private void Load_App_Name()
        {
            try
            {
                gAppName = PluginManager.GetPluginName("VR CLIENT", OSAE.Common.ComputerName);
                if (gAppName == "")
                {
                    gAppName = "VR CLIENT-" + OSAE.Common.ComputerName;
                    OSAEObjectManager.ObjectAdd(gAppName, gAppName, "VR CLIENT", "", "SYSTEM", true);
                    OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "Computer Name", OSAE.Common.ComputerName, "VR");
                }
                OSAEObjectStateManager.ObjectStateSet(gAppName, "ON", OSAE.Common.ComputerName);
            }
            catch (Exception ex)
            {
                AddToLog("Error messaging host: " + ex.Message);
            }
        }

        private void Load_Settings()
        {
            try
            {
                AddToLog("OSA Settings:");
                AddToLog("--  Object Name: " + gAppName);
                String temp = OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "VR Input Muted").Value.ToString();
                if (temp == "FALSE")
                {
                    gVRMuted = false;
                    lblStatus.Content = "I am awake";
                }
                else
                {
                    gVRMuted = true;
                    lblStatus.Content = "I am asleep";
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
            {
                AddToLog("Error in Load Settings: " + ex.Message);
            }
        }

        private void Load_Grammer()
        {
            List<string> grammerList = new List<string>();
            DataSet dsResults = new DataSet();
            try
            {
                //Load all unique patterns with no place-holders into a single grammer, our main one.
                dsResults = OSAESql.RunSQL("SELECT `match` FROM osae_pattern_match WHERE UPPER(`match`) NOT LIKE '%[OBJECT]%' AND UPPER(`match`) NOT LIKE '%[STATE]%' ORDER BY `match`");
                //grammerList.Add(gWakePhrase);
                //grammerList.Add(gSleepPhrase);
                for (int i = 0; i < dsResults.Tables[0].Rows.Count; i++)
                {
                    string grammer = dsResults.Tables[0].Rows[i][0].ToString();

                    if (!string.IsNullOrEmpty(grammer))
                    {
                        grammerList.Add(grammer);
                    }
                }
            }
            catch (Exception ex)
            {
                AddToLog("Error getting Grammer set from the DB!");
                AddToLog("Error: " + ex.Message);
            }
            try
            {
            Choices myChoices = new Choices(grammerList.ToArray());
            GrammarBuilder builder = new GrammarBuilder(myChoices);
            Grammar gram = new Grammar(builder);
            oRecognizer.LoadGrammar(gram);
            AddToLog("Grammer Load Completed (" + grammerList.Count + " unique items)");
            }
            catch (Exception ex)
            {
                AddToLog("I could Not build the Grammer set!");
                AddToLog("Error: " + ex.Message);
            }
        }

        private void Load_Grammer_With_Substitutions()
        {
            List<string> objectList = new List<string>();
            List<string> stateList = new List<string>();
            List<string> propertyList = new List<string>();
            DataSet dsResults = new DataSet();
            try
            {
                //Build a List of all Objects to use for substitutions
                dsResults = OSAESql.RunSQL("SELECT object_name FROM osae_v_object WHERE base_type NOT IN ('CONTROL','SCREEN','PLUGIN') ORDER BY object_name");
                for (int i = 0; i < dsResults.Tables[0].Rows.Count; i++)
                {
                    string grammer = dsResults.Tables[0].Rows[i][0].ToString();

                    if (!string.IsNullOrEmpty(grammer))
                    {
                        objectList.Add(grammer);
                    }
                }
            }
            catch (Exception ex)
            {
                AddToLog("Error getting Object Set set from the DB!");
                AddToLog("Error: " + ex.Message);
            }
            Choices objectChoices = new Choices(objectList.ToArray());

            try
            {
                //Build a List of all States to use for substitutions
                dsResults = OSAESql.RunSQL("SELECT DISTINCT(state_label) FROM osae_v_object_state ORDER BY state_label");
                for (int i = 0; i < dsResults.Tables[0].Rows.Count; i++)
                {
                    string grammer = dsResults.Tables[0].Rows[i][0].ToString();

                    if (!string.IsNullOrEmpty(grammer))
                    {
                        stateList.Add(grammer);
                    }
                }
            }
            catch (Exception ex)
            {
                AddToLog("Error getting Object Set set from the DB!");
                AddToLog("Error: " + ex.Message);
            }
            Choices stateChoices = new Choices(stateList.ToArray());

            try
            {
                //Build a List of all Properties to use for substitutions
                dsResults = OSAESql.RunSQL("SELECT DISTINCT(property_name) FROM osae_v_object_property ORDER BY property_name");
                for (int i = 0; i < dsResults.Tables[0].Rows.Count; i++)
                {
                    string grammer = dsResults.Tables[0].Rows[i][0].ToString();

                    if (!string.IsNullOrEmpty(grammer))
                    {
                        propertyList.Add(grammer);
                    }
                }
            }
            catch (Exception ex)
            {
                AddToLog("Error getting Property Set set from the DB!");
                AddToLog("Error: " + ex.Message);
            }
            Choices propertyChoices = new Choices(propertyList.ToArray());


            //Each Place Holder Pattern must be build my hand right now, but it should be able to be done progammatically

            //Is [Object] [State]
            try
            {
                GrammarBuilder builder = new GrammarBuilder("Is");
                builder.Append(objectChoices);
                builder.Append(stateChoices);
                Grammar gram = new Grammar(builder);
                oRecognizer.LoadGrammar(gram);
                AddToLog("Grammer 'is [OBJECT] [STATE]' Load Completed (" + objectList.Count + " items with place-holders)");
            }
            catch (Exception ex)
            {
                AddToLog("I could Not build the 'Is [OBJECT] [STATE]' Grammer set!");
                AddToLog("Error: " + ex.Message);
            }


            //[Object] is [State]
            try
            {
                GrammarBuilder builder = new GrammarBuilder(objectChoices);
                builder.Append("is");
                builder.Append(stateChoices);
                Grammar gram = new Grammar(builder);
                oRecognizer.LoadGrammar(gram);
                AddToLog("Grammer '[OBJECT] is [STATE]' Load Completed (" + objectList.Count + " items with place-holders)");
            }
            catch (Exception ex)
            {
                AddToLog("I could Not build the '[OBJECT] is [STATE]' Grammer set!");
                AddToLog("Error: " + ex.Message);
            }

            //[OBJECT] is in the [OBJECT]
            try
            {
                GrammarBuilder builder = new GrammarBuilder(objectChoices);
                builder.Append("is in the");
                builder.Append(objectChoices);
                Grammar gram = new Grammar(builder);
                oRecognizer.LoadGrammar(gram);
                AddToLog("Grammer '[OBJECT] is in the [OBJECT]' Load Completed (" + objectList.Count + " items with place-holders)");
            }
            catch (Exception ex)
            {
                AddToLog("I could Not build the '[OBJECT] is in the [OBJECT]' Grammer set!");
                AddToLog("Error: " + ex.Message);
            }

            //is [OBJECT] is in the [OBJECT]
            try
            {
                GrammarBuilder builder = new GrammarBuilder("is");
                builder.Append(objectChoices);
                builder.Append("in the");
                builder.Append(objectChoices);
                Grammar gram = new Grammar(builder);
                oRecognizer.LoadGrammar(gram);
                AddToLog("Grammer 'is [OBJECT] is in the [OBJECT]' Load Completed (" + objectList.Count + " items with place-holders)");
            }
            catch (Exception ex)
            {
                AddToLog("I could Not build the 'is [OBJECT] is in the [OBJECT]' Grammer set!");
                AddToLog("Error: " + ex.Message);
            }

            //Where is [OBJECT]
            try
            {
                GrammarBuilder builder = new GrammarBuilder("Where is");
                builder.Append(objectChoices);
                Grammar gram = new Grammar(builder);
                oRecognizer.LoadGrammar(gram);
                AddToLog("Grammer 'Where is' Load Completed (" + objectList.Count + " items with place-holders)");
            }
            catch (Exception ex)
            {
                AddToLog("I could Not build the 'Where is' Grammer set!");
                AddToLog("Error: " + ex.Message);
            }

            //What is [OBJECT]'s [PROPERTY]
            try
            {
                GrammarBuilder builder = new GrammarBuilder("What is");
                builder.Append(objectChoices);
                builder.Append("'s");
                builder.Append(propertyChoices);
                Grammar gram = new Grammar(builder);
                oRecognizer.LoadGrammar(gram);
                AddToLog("Grammer 'What is' Load Completed (" + objectList.Count + " items with place-holders)");
            }
            catch (Exception ex)
            {
                AddToLog("I could Not build the 'What is' Grammer set!");
                AddToLog("Error: " + ex.Message);
            }
        }


        private void oRecognizer_SpeechRecognized(object sender, System.Speech.Recognition.SpeechRecognizedEventArgs e)
        {
            ProcessInput(e.Result.Text);
        }

        private void ProcessInput(string sInput)
        {
            DataSet dsResults = new DataSet();
            String sPattern = "";
            try
            {
               // if ((sInput.ToUpper() == gWakePattern.ToUpper()) & (gVRMuted == true))
                if (wakeList.Contains(sInput) && gVRMuted == true)
                {
                    gVRMuted = false;
                    lblStatus.Content = "I am awake";
                }
                else if (sleepList.Contains(sInput) && gVRMuted == false)
                {
                    gVRMuted = true;
                    lblStatus.Content = "I am sleeping";
                }
                else if ((sInput.ToUpper() == gSleepPattern.ToUpper()) & (gVRMuted == true))
                {
                    return;
                }
                
                
                // gSpeechPlugin;
                String temp = OSAEObjectPropertyManager.GetObjectPropertyValue(gSpeechPlugin, "Speaking").Value.ToString().ToLower();

                if (temp.ToLower() == "true")
                {
                    try
                    {
                        if (chkShowIgnored.IsChecked == true)
                        AddToLog("Ignored Speech because TTS was talking.");
                    }
                    catch (Exception ex)
                    {
                    }
                }
                else
                {
                    if ((gVRMuted == false) || (sleepList.Contains(sInput)))
                    {
                        try
                        {
                            string sLogEntry = "Heard: " + sInput;
                            //string sText = OSAE.Common.MatchPattern(sInput);
                            string sText = MatchPattern(sInput);
                            if (sText.Length > 0)
                            {
                                sLogEntry += ".  Ran: " + sText;
                            }
                            AddToLog(sLogEntry);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddToLog("Error in _SpeechRecognized: " + ex.Message);
            }
        }


        private void oRecognizer_StateChanged(object sender, System.Speech.Recognition.AudioStateChangedEventArgs e)
        {
            System.Speech.Recognition.AudioState state = oRecognizer.AudioState;
            lblAudioState.Content = "I hear " + state.ToString();
            try
            {
                if (oRecognizer.AudioState == 0)
                    oRecognizer.RecognizeAsync();
            }
            catch (Exception ex)
            {
                AddToLog("Error trying to Restart Recognition!");
                AddToLog("Errord: " + ex.Message);
            }
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            ProcessInput(txtInput.Text);
        }


       // WARNING the follow code is only used for testing.   Matching is done in the API!!!!!!!!!!!!!!!!!!!!!!!!!!
       // When working on the API Matching, copy the code here, change the call above to point to this code for debugging
        public static string MatchPattern(string str)
        {
           string ScriptParameter = "";
            try
            {
                str = str.TrimEnd('?','.','!');
                DataSet dataset = new DataSet();
                //command.CommandText = "SELECT pattern FROM osae_v_pattern WHERE `match`=@Name";
                //command.Parameters.AddWithValue("@Name", str);
                dataset = OSAESql.RunSQL("SELECT pattern FROM osae_v_pattern_match WHERE `match`='" + str.Replace("'", "''") + "'");

                if (dataset.Tables[0].Rows.Count > 0)
                {

                    //Since we have a match, lets execute the scripts
                    OSAEScriptManager.RunPatternScript(dataset.Tables[0].Rows[0]["pattern"].ToString(), "", "Jabber");
                    return dataset.Tables[0].Rows[0]["pattern"].ToString();
                }
                else
                {
                    //Replace Words with place holders and retry the pattern match
                    //example  "Please turn the main light on" becomes "Please turn the [OBJECT] [STATE]"

                    //Step 1: Break the Input into an Array to Query the Words for DB matches
                    str = str.ToUpper();
                    
                    string[] words = str.Split(' ');

                    DataSet dsObjects = new DataSet();
                    foreach (String word in words)
                    {
                        dsObjects = OSAE.Common.ObjectNamesStartingWith(word);
                        foreach (DataRow dr in dsObjects.Tables[0].Rows)
                        {
                            if (str.IndexOf(dr["object_name"].ToString().ToUpper()) > -1)
                            //return "Found " + dr["object_name"].ToString();
                            {
                                str = str.Replace(dr["object_name"].ToString().ToUpper(), "[OBJECT]");
                                if (ScriptParameter.Length > 1)
                                {
                                    ScriptParameter = ScriptParameter + ",";
                                }
                                ScriptParameter += dr["object_name"].ToString();
                                //Determine if the Object is Possessive, which would be followed by a Property
                                if (str.IndexOf("[OBJECT] 'S") > -1)
                                {
                                    //Here We have found our Possessive Object, so we need to look for an appropriate property afterwards
                                    //So we are going to retrieve a property list and compare it to the start of theremainder of the string

                                    DataSet dsProperties = new DataSet();
                                    dsProperties = OSAEObjectPropertyManager.ObjectPropertyListGet(dr["object_name"].ToString());
                                    foreach (DataRow drProperty in dsProperties.Tables[0].Rows)
                                    {
                                        //Here we need to break the string into words to avoid partial matches
                                        int objectStartLoc = str.IndexOf("[OBJECT]'s");
                                        string strNewSearch = str.Substring(objectStartLoc + 15);
                                        if (strNewSearch.IndexOf(drProperty["property_name"].ToString().ToUpper())> -1)
                                        {
                                            str = str.Replace("[OBJECT] 'S " + drProperty["property_name"].ToString().ToUpper(), "[OBJECT] 'S [PROPERTY]");
                                            //str = str.Replace(drState["state_label"].ToString().ToUpper(), "[STATE]");
                                            ScriptParameter += "," + drProperty["property_name"].ToString();
                                        }
                                    }
                                }

                                //Here We have found our Object, so we need to look for an appropriate state afterwards
                                //So we are going to retrieve a state list and compare it to the remainder of the string
                                DataSet dsStates = new DataSet();
                                dsStates = OSAEObjectStateManager.ObjectStateListGet(dr["object_name"].ToString());
                                foreach (DataRow drState in dsStates.Tables[0].Rows)
                                {
                                    //Here we need to break the string into words to avoid partial matches
                                    string replacementString = "";
                                    string[] wordArray = str.Split(new Char[] { ' ' });
                                    foreach (string w in wordArray)
                                    {
                                        if (replacementString.Length > 1)
                                        {
                                            replacementString = replacementString + " ";
                                        }
                                        if (drState["state_label"].ToString().ToUpper() == w || drState["state_name"].ToString().ToUpper() == w)
                                        {
                                            replacementString = replacementString + "[STATE]";
                                            //str = str.Replace(drState["state_label"].ToString().ToUpper(), "[STATE]");
                                            ScriptParameter += "," + drState["state_name"].ToString();
                                        }
                                        else
                                        {
                                            replacementString = replacementString + w;
                                        }
                                    }
                                        //Now that we have replaced the Object and State, Lets check for a match again
                                        //DataSet dataset = new DataSet();
                                        //command.CommandText = "SELECT pattern FROM osae_v_pattern WHERE `match`=@Name";
                                        //command.Parameters.AddWithValue("@Name", str);
                                        //dataset = OSAESql.RunQuery(command);
                                    replacementString = replacementString.Replace(" 'S", "'S");
                                    dataset = OSAESql.RunSQL("SELECT pattern FROM osae_v_pattern_match WHERE `match`='" + replacementString.Replace("'", "''") + "'");
                                        if (dataset.Tables[0].Rows.Count > 0)
                                        {
                                            //return dataset.Tables[0].Rows[0]["pattern"].ToString();
                                            //Since we have a match, lets execute the scripts
                                            OSAEScriptManager.RunPatternScript(dataset.Tables[0].Rows[0]["pattern"].ToString(), ScriptParameter, "Jabber");
                                            return dataset.Tables[0].Rows[0]["pattern"].ToString();
                                        }
                                        //break;
                                }
                                  //break;
                            }
                        }
                    }
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Logging.GetLogger().AddToLog("API - MatchPattern error: " + ex.Message, true);
                return string.Empty;
            }

        }
        







    }


}
