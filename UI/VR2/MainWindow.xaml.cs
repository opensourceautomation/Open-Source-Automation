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
using System.Speech.Recognition.SrgsGrammar;
using System.Data;
using System.IO;
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
        List<string> wakeList = new List<string>();
        String gSleepPattern = "Thanks";
        List<string> sleepList = new List<string>();
        List<string> userList = new List<string>();
        String gSpeechPlugin = "";
        String gUser = "";
        Boolean gAppClosing = false;
        Boolean gRunningManual = false;
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
            }
            catch (Exception ex)
            {
                AddToLog("Unable to configure oRecognizer");
                AddToLog("Error: " + ex.Message);
            }
           
            Load_Settings();
            
            //Load_User_Grammer(); //So nothing is recognized until we have a user object
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

                AddToLog("Finished Loading...");
                AddToLog("_______________________________________________");
                AddToLog("Who are you?");

                Thread t1 = new Thread(delegate()
                {
                    oRecognizer.SetInputToDefaultAudioDevice();
                    oRecognizer.RecognizeAsync(RecognizeMode.Multiple);
                    while (!gAppClosing)
                    {
                        Thread.Sleep(333);
                    }
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
            {
                AddToLog("Error in Load Settings: " + ex.Message);
            }

            
        }

        /*
        private void Load_Grammer()
        {
            List<string> grammerList = new List<string>();
            DataSet dsResults = new DataSet();
            Choices myChoices = new Choices();
            try
            {
                //Load all unique patterns with no place-holders into a single grammer, our main one.
                dsResults = OSAESql.RunSQL("SELECT `match` FROM osae_pattern_match WHERE UPPER(`match`) NOT LIKE '%[OBJECT]%' AND UPPER(`match`) NOT LIKE '%[STATE]%' AND UPPER(`match`) NOT LIKE '%[PRONOUN]%' ORDER BY `match`");
                //grammerList.Add(gWakePhrase);
                //grammerList.Add(gSleepPhrase);
                for (int i = 0; i < dsResults.Tables[0].Rows.Count; i++)
                {
                    string sTemp = dsResults.Tables[0].Rows[i][0].ToString();

                    if (!string.IsNullOrEmpty(sTemp))
                    {
                        SemanticResultKey srk = new SemanticResultKey(sTemp, sTemp);
                        //gb_GrammarBuilder.Append(srk);
                        myChoices.Add(srk);
                        //grammerList.Add(sTemp);
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


            //Choices myChoices = new Choices(grammerList.ToArray());
            GrammarBuilder builder = new GrammarBuilder(myChoices);
            Grammar gram = new Grammar(builder);
            gram.Name = "Direct Match";
            oRecognizer.LoadGrammar(gram);
            AddToLog("Grammer Load Completed (" + grammerList.Count + " unique items)");
            }
            catch (Exception ex)
            {
                AddToLog("I could Not build the Grammer set!");
                AddToLog("Error: " + ex.Message);
            }
        }
        
        private void Load_Grammer_With_OT_Substitutions()
        {
            Choices nounPrecedentChoices = new Choices(new string[] { "a", "an", "the" });
            DataSet dsResults = new DataSet();

            #region Build a List of all Objects

            List<string> objectFullList = new List<string>();
            //Get All objects
            dsResults = OSAESql.RunSQL("SELECT object_name FROM osae_v_object ORDER BY object_name");
            for (int i = 0; i < dsResults.Tables[0].Rows.Count; i++)
            {
                string grammer = dsResults.Tables[0].Rows[i][0].ToString();
                if (!string.IsNullOrEmpty(grammer)) objectFullList.Add(grammer);
            }
            Choices objectFullChoices = new Choices(objectFullList.ToArray());
            #endregion

            #region Build a List of all Possessive Objects

            List<string> objectPossessiveList = new List<string>();
            //Get All objects
            dsResults = OSAESql.RunSQL("SELECT CONCAT(object_name,'''s') as object_name FROM osae_v_object ORDER BY object_name");
            for (int i = 0; i < dsResults.Tables[0].Rows.Count; i++)
            {
                string grammer = dsResults.Tables[0].Rows[i][0].ToString();
                if (!string.IsNullOrEmpty(grammer)) objectPossessiveList.Add(grammer);
            }
            Choices objectPossessiveChoices = new Choices(objectPossessiveList.ToArray());
            #endregion

            #region Build a List of all Containers

            List<string> containerList = new List<string>();
            //Get All containers
            dsResults = OSAESql.RunSQL("SELECT object_name FROM osae_v_object WHERE container=1 ORDER BY object_name");
            for (int i = 0; i < dsResults.Tables[0].Rows.Count; i++)
            {
                string grammer = dsResults.Tables[0].Rows[i][0].ToString();
                if (!string.IsNullOrEmpty(grammer)) containerList.Add(grammer);
            }
            Choices containerChoices = new Choices(containerList.ToArray());

            #endregion
                
            #region Build a List of all Object Types

            List<string> objectTypeList = new List<string>();
            dsResults = OSAESql.RunSQL("SELECT object_type FROM osae_v_object_type WHERE base_type NOT IN ('CONTROL','SCREEN') ORDER BY object_type");
            for (int i = 0; i < dsResults.Tables[0].Rows.Count; i++)
            {
                string grammer = dsResults.Tables[0].Rows[i][0].ToString();
                if (!string.IsNullOrEmpty(grammer)) objectTypeList.Add(grammer);
            }
            Choices objectTypeChoices = new Choices(objectTypeList.ToArray());

            #endregion

            #region [Object] [State]
            // Is OBJECT STATE?
            // Is [NP] OBJECT STATE?
            // OBJECT is STATE
            // [NP] OBJECT is STATE
                GrammarBuilder gbAreYouState = new GrammarBuilder("Are");
                SemanticResultKey srk = new SemanticResultKey("PARAM1", "you");
                gbAreYouState.Append(srk);
                
                GrammarBuilder gbAmIState = new GrammarBuilder("Am I");
                srk = new SemanticResultKey("PARAM1", "I");
                gbAmIState.Append(srk);

                //builder.Append(objectChoices);

                foreach (string ot in objectTypeList)
                {
                    List<string> objectList = new List<string>();
                    List<string> stateList = new List<string>();

                    GrammarBuilder gbIsObjectState = new GrammarBuilder("Is");
                    GrammarBuilder gbIsNPObjectState = new GrammarBuilder("Is");
                    gbIsNPObjectState.Append(nounPrecedentChoices);
                    
                    GrammarBuilder gbNPObjectIsState = new GrammarBuilder(nounPrecedentChoices);
                    
                    //Get All objects of the current Object Type
                    dsResults = OSAESql.RunSQL("SELECT object_name FROM osae_v_object WHERE object_type='" + ot + "' ORDER BY object_name");
                    for (int i = 0; i < dsResults.Tables[0].Rows.Count; i++)
                    {
                        string grammer = dsResults.Tables[0].Rows[i][0].ToString();
                        if (!string.IsNullOrEmpty(grammer)) objectList.Add(grammer);
                    }
                    if (objectList.Count > 0)  // Only bother with this object type if there are objects using it
                    {
                        Choices objectChoices = new Choices(objectList.ToArray());
                        srk = new SemanticResultKey("PARAM1", objectChoices);

                        gbIsObjectState.Append(srk);
                        gbIsNPObjectState.Append(srk);
                        GrammarBuilder gbObjectIsState = new GrammarBuilder(srk);
                        gbNPObjectIsState.Append(srk);
                        gbObjectIsState.Append("is");
                        gbNPObjectIsState.Append("is");


                        //Now the the appropriate states                    
                        dsResults = OSAESql.RunSQL("SELECT DISTINCT(state_label) FROM osae_v_object_type_state WHERE object_type='" + ot + "'");
                        for (int i = 0; i < dsResults.Tables[0].Rows.Count; i++)
                        {
                            string grammer = dsResults.Tables[0].Rows[i][0].ToString();
                            if (!string.IsNullOrEmpty(grammer)) stateList.Add(grammer);
                        }
                        dsResults = OSAESql.RunSQL("SELECT DISTINCT(state_name) FROM osae_v_object_type_state WHERE object_type='" + ot + "' AND UPPER(state_name) != UPPER(state_label)");
                        for (int i = 0; i < dsResults.Tables[0].Rows.Count; i++)
                        {
                            string grammer = dsResults.Tables[0].Rows[i][0].ToString();
                            if (!string.IsNullOrEmpty(grammer)) stateList.Add(grammer);
                        }
                        if (stateList.Count > 0)
                        {
                            Choices stateChoices = new Choices(stateList.ToArray());
                            srk = new SemanticResultKey("PARAM2", stateChoices);
                            if (ot == "PERSON")
                            {
                                gbAmIState.Append(srk);
                                Grammar gAmIState = new Grammar(gbAmIState);
                                gAmIState.Name = "Is [OBJECT] [STATE]";
                                oRecognizer.LoadGrammar(gAmIState);
                            }
                            else if (ot == "SYSTEM")
                            {
                                gbAreYouState.Append(srk);
                                Grammar gAreYouState = new Grammar(gbAreYouState);
                                gAreYouState.Name = "Is [OBJECT] [STATE]";
                                oRecognizer.LoadGrammar(gAreYouState);
                            }

                            gbIsObjectState.Append(srk);
                            Grammar gIsObjectState = new Grammar(gbIsObjectState);
                            gIsObjectState.Name = "Is [OBJECT] [STATE]";
                            oRecognizer.LoadGrammar(gIsObjectState);

                            gbIsNPObjectState.Append(srk);
                            Grammar gIsNPObjectState = new Grammar(gbIsNPObjectState);
                            gIsNPObjectState.Name = "[OBJECT] is [STATE]";
                            oRecognizer.LoadGrammar(gIsNPObjectState);

                            gbObjectIsState.Append(srk);
                            Grammar gObjectIsState = new Grammar(gbObjectIsState);
                            gObjectIsState.Name = "[OBJECT] is [STATE]";
                            oRecognizer.LoadGrammar(gObjectIsState);

                            gbNPObjectIsState.Append(srk);
                            Grammar gNPObjectIsState = new Grammar(gbNPObjectIsState);
                            gNPObjectIsState.Name = "[OBJECT] is [STATE]";
                            oRecognizer.LoadGrammar(gNPObjectIsState);
                        }
                    }
                }
                AddToLog("Grammer 'is {NP} [OBJECT] [STATE]' Loaded.");
                AddToLog("Grammer '{NP} [OBJECT] is [STATE]' Loaded.");
            #endregion

            #region What is [OBJECT]'s [PROPERTY]
                //What is OBJECT's PROPERTY
                //What is NP OBJECT's PROPERTY
                //What is my PROPERTY
                //What is your PROPERTY

                GrammarBuilder gbWhatIsMyProperty = new GrammarBuilder("What is");
                srk = new SemanticResultKey("PARAM1", "my");
                gbWhatIsMyProperty.Append(srk);
                GrammarBuilder gbWhatIsYourProperty = new GrammarBuilder("What is");
                srk = new SemanticResultKey("PARAM1", "your");
                gbWhatIsYourProperty.Append(srk);

                foreach (string ot in objectTypeList)
                {
                    GrammarBuilder gbWhatIsObjectProperty = new GrammarBuilder("What is");
                    GrammarBuilder gbWhatIsNPObjectProperty = new GrammarBuilder("What is");
                    gbWhatIsNPObjectProperty.Append(nounPrecedentChoices);

                    List<string> objectList = new List<string>();
                    List<string> propertyList = new List<string>();

                    //Get All objects of the current Object Type
                    dsResults = OSAESql.RunSQL("SELECT CONCAT(object_name,'''s') as object_name FROM osae_v_object WHERE object_type='" + ot + "' ORDER BY object_name");
                    for (int i = 0; i < dsResults.Tables[0].Rows.Count; i++)
                    {
                        string grammer = dsResults.Tables[0].Rows[i][0].ToString();
                        if (!string.IsNullOrEmpty(grammer)) objectList.Add(grammer);
                    }
                    if (objectList.Count > 0)  // Only bother with this object type if there are objects using it
                    {
                        Choices objectChoices = new Choices(objectList.ToArray());
                        srk = new SemanticResultKey("PARAM1", objectChoices);
                        gbWhatIsObjectProperty.Append(srk);
                        gbWhatIsNPObjectProperty.Append(srk);

                        //Now the the appropriate properties                    
                        dsResults = OSAESql.RunSQL("SELECT DISTINCT(property_name) FROM osae_v_object_type_property WHERE object_type='" + ot + "'");
                        for (int i = 0; i < dsResults.Tables[0].Rows.Count; i++)
                        {
                            string grammer = dsResults.Tables[0].Rows[i][0].ToString();
                            if (!string.IsNullOrEmpty(grammer)) propertyList.Add(grammer);
                        }
                        if (propertyList.Count > 0)
                        {
                            Choices propertyChoices = new Choices(propertyList.ToArray());
                            srk = new SemanticResultKey("PARAM2", propertyChoices);
                            if (ot == "PERSON")
                            {
                                gbWhatIsMyProperty.Append(srk);
                                Grammar gWhatIsMyProperty = new Grammar(gbWhatIsMyProperty);
                                gWhatIsMyProperty.Name = "What is [OBJECT] [PROPERTY]";
                                oRecognizer.LoadGrammar(gWhatIsMyProperty);
                            }
                            else if (ot == "SYSTEM")
                            {
                                gbWhatIsYourProperty.Append(srk);
                                Grammar gWhatIsYourProperty = new Grammar(gbWhatIsYourProperty);
                                gWhatIsYourProperty.Name = "What is [OBJECT] [PROPERTY]";
                                oRecognizer.LoadGrammar(gWhatIsYourProperty);
                            }
                            gbWhatIsObjectProperty.Append(srk);
                            Grammar gWhatIsObjectProperty = new Grammar(gbWhatIsObjectProperty);
                            gWhatIsObjectProperty.Name = "What is [OBJECT] [PROPERTY]";
                            oRecognizer.LoadGrammar(gWhatIsObjectProperty);

                            gbWhatIsNPObjectProperty.Append(srk);
                            Grammar gWhatIsNPObjectProperty = new Grammar(gbWhatIsNPObjectProperty);
                            gWhatIsNPObjectProperty.Name = "What is [OBJECT] [PROPERTY]";
                            oRecognizer.LoadGrammar(gWhatIsNPObjectProperty);
                            
                        }
                    }
                }
                AddToLog("Grammer 'Whatis {NP} [OBJECT] [PROPERTY]' Loaded.");
            #endregion

            #region [OBJECT]'s [PROPERTY] is [VALUE]
                //OBJECT's PROPERTY is [VALUE]
                
                foreach (string ot in objectTypeList)
                {
                    List<string> objectList = new List<string>();
                    

                    GrammarBuilder gbObjectPropertyIs = new GrammarBuilder();

                    //Get All objects of the current Object Type
                    dsResults = OSAESql.RunSQL("SELECT CONCAT(object_name,'''s') as object_name FROM osae_v_object WHERE object_type='" + ot + "' ORDER BY object_name");
                    for (int i = 0; i < dsResults.Tables[0].Rows.Count; i++)
                    {
                        string grammer = dsResults.Tables[0].Rows[i][0].ToString();
                        if (!string.IsNullOrEmpty(grammer)) objectList.Add(grammer);
                    }
                    if (objectList.Count > 0)  // Only bother with this object type if there are objects using it
                    {
                        Choices objectChoices = new Choices(objectList.ToArray());
                        srk = new SemanticResultKey("PARAM1", objectChoices);
                        gbObjectPropertyIs.Append(srk);

                        //Now the the appropriate properties                    
                        DataSet dsPropType = OSAESql.RunSQL("SELECT DISTINCT(property_datatype),property_object_type FROM osae_v_object_type_property WHERE object_type='" + ot + "' ORDER BY property_datatype");
                        foreach (DataRow drType in dsPropType.Tables[0].Rows)
                        {
                            List<string> propertyList = new List<string>();
                            DataSet dsPropName = OSAESql.RunSQL("SELECT DISTINCT(property_name) FROM osae_v_object_type_property WHERE object_type='" + ot + "' AND property_datatype='" + drType["property_datatype"].ToString() + "' ORDER BY property_datatype");
                            foreach (DataRow drName in dsPropName.Tables[0].Rows)
                            {
                                propertyList.Add(drName["property_name"].ToString());
                            }
                            Choices propertyChoices = new Choices(propertyList.ToArray());
                                if (drType["property_datatype"].ToString().ToUpper() == "STRING")
                                {
                                    GrammarBuilder dictation = new GrammarBuilder();
                                    dictation.AppendDictation();
                                                                        
                                    srk = new SemanticResultKey("PARAM2", propertyChoices);
                                    gbObjectPropertyIs.Append(srk);
                                    gbObjectPropertyIs.Append("is");
                                    gbObjectPropertyIs.Append(new SemanticResultKey("PARAM3", dictation));
                                    Grammar gObjectPropertyIs = new Grammar(gbObjectPropertyIs);
                                    gObjectPropertyIs.Name = "[OBJECT] [PROPERTY] is [VALUE]";
                                    oRecognizer.LoadGrammar(gObjectPropertyIs);
                                }
                                else if (drType["property_datatype"].ToString().ToUpper() == "OBJECT")
                                {
                                    srk = new SemanticResultKey("PARAM2", propertyChoices);
                                    gbObjectPropertyIs.Append(srk);
                                    gbObjectPropertyIs.Append("is");
                                    gbObjectPropertyIs.Append(new SemanticResultKey("PARAM3", objectFullChoices));
                                    Grammar gObjectPropertyIs = new Grammar(gbObjectPropertyIs);
                                    gObjectPropertyIs.Name = "[OBJECT] [PROPERTY] is [VALUE]";
                                    oRecognizer.LoadGrammar(gObjectPropertyIs);
                                }
                                else if (drType["property_datatype"].ToString().ToUpper() == "OBJECT TYPE")
                                {
                                    List<string> propertyOTList = new List<string>();
                                    DataSet dsPropObjectType = OSAESql.RunSQL("SELECT DISTINCT(object_name) FROM osae_v_object WHERE object_type='" + drType["property_object_type"].ToString() + "' ORDER BY object_name");
                                    foreach (DataRow drName in dsPropObjectType.Tables[0].Rows)
                                    {
                                        propertyOTList.Add(drName["object_name"].ToString());
                                    }
                                    Choices propertyOTChoices = new Choices(propertyOTList.ToArray());
                                    srk = new SemanticResultKey("PARAM2", propertyChoices);
                                    gbObjectPropertyIs.Append(srk);
                                    gbObjectPropertyIs.Append("is");

                                    gbObjectPropertyIs.Append(new SemanticResultKey("PARAM3", propertyOTChoices));
                                    Grammar gObjectPropertyIs = new Grammar(gbObjectPropertyIs);
                                    gObjectPropertyIs.Name = "[OBJECT] [PROPERTY] is [VALUE]";
                                    oRecognizer.LoadGrammar(gObjectPropertyIs);
                                }                        
                        }
                    }
                }
            #endregion

            #region [OBJECT] [CONTAINER]
                // OBJECT is in CONTAINER
                // np OBJECT is in CONTAINER
                // OBJECT is in np CONTAINER
                // np OBJECT is in np CONTAINER
                // I am in CONTAINER
                // I am in np CONTAINER
                // You are in CONTAINER
                // You are in np CONTAINER


                // is OBJECT in CONTAINER
                // is np OBJECT is in CONTAINER
                // is OBJECT in np CONTAINER
                // is np OBJECT in np CONTAINER
                // am I in CONTAINER
                // am I in NP CONTAINER
                // are you in CONTAINER
                // are you in np CONTAINER

                // OBJECT is in CONTAINER
                GrammarBuilder gb_GrammarBuilder = new GrammarBuilder();
                srk = new SemanticResultKey("PARAM1", objectFullChoices);
                gb_GrammarBuilder.Append(srk);
                gb_GrammarBuilder.Append("is in");
                srk = new SemanticResultKey("PARAM2", containerChoices);
                gb_GrammarBuilder.Append(srk);
                Grammar g_Grammar = new Grammar(gb_GrammarBuilder);
                g_Grammar.Name = "[OBJECT] is in [CONTAINER]";
                oRecognizer.LoadGrammar(g_Grammar);

                // np OBJECT is in CONTAINER
                gb_GrammarBuilder = new GrammarBuilder(nounPrecedentChoices);
                srk = new SemanticResultKey("PARAM1", objectFullChoices);
                gb_GrammarBuilder.Append(srk);
                gb_GrammarBuilder.Append("is in");
                srk = new SemanticResultKey("PARAM2", containerChoices);
                gb_GrammarBuilder.Append(srk);
                g_Grammar = new Grammar(gb_GrammarBuilder);
                g_Grammar.Name = "[OBJECT] is in [CONTAINER]";
                oRecognizer.LoadGrammar(g_Grammar);

                // OBJECT is in np CONTAINER
                gb_GrammarBuilder = new GrammarBuilder();
                srk = new SemanticResultKey("PARAM1", objectFullChoices);
                gb_GrammarBuilder.Append(srk);
                gb_GrammarBuilder.Append("is in");
                gb_GrammarBuilder.Append(nounPrecedentChoices);
                srk = new SemanticResultKey("PARAM2", containerChoices);
                gb_GrammarBuilder.Append(srk);
                g_Grammar = new Grammar(gb_GrammarBuilder);
                g_Grammar.Name = "[OBJECT] is in [CONTAINER]";
                oRecognizer.LoadGrammar(g_Grammar);

                // np OBJECT is in np CONTAINER
                gb_GrammarBuilder = new GrammarBuilder(nounPrecedentChoices);
                srk = new SemanticResultKey("PARAM1", objectFullChoices);
                gb_GrammarBuilder.Append(srk);
                gb_GrammarBuilder.Append("is in");
                gb_GrammarBuilder.Append(nounPrecedentChoices);
                srk = new SemanticResultKey("PARAM2", containerChoices);
                gb_GrammarBuilder.Append(srk);
                g_Grammar = new Grammar(gb_GrammarBuilder);
                g_Grammar.Name = "[OBJECT] is in [CONTAINER]";
                oRecognizer.LoadGrammar(g_Grammar);

                // I am in CONTAINER
                gb_GrammarBuilder = new GrammarBuilder();
                srk = new SemanticResultKey("PARAM1", "I");
                gb_GrammarBuilder.Append(srk);
                gb_GrammarBuilder.Append("am in");
                srk = new SemanticResultKey("PARAM2", containerChoices);
                gb_GrammarBuilder.Append(srk);
                g_Grammar = new Grammar(gb_GrammarBuilder);
                g_Grammar.Name = "[OBJECT] is in [CONTAINER]";
                oRecognizer.LoadGrammar(g_Grammar);

                // I am in np CONTAINER
                gb_GrammarBuilder = new GrammarBuilder();
                srk = new SemanticResultKey("PARAM1", "I");
                gb_GrammarBuilder.Append(srk);
                gb_GrammarBuilder.Append("am in");
                gb_GrammarBuilder.Append(nounPrecedentChoices);
                srk = new SemanticResultKey("PARAM2", containerChoices);
                gb_GrammarBuilder.Append(srk);
                g_Grammar = new Grammar(gb_GrammarBuilder);
                g_Grammar.Name = "[OBJECT] is in [CONTAINER]";
                oRecognizer.LoadGrammar(g_Grammar);

                // You are in CONTAINER
                gb_GrammarBuilder = new GrammarBuilder();
                srk = new SemanticResultKey("PARAM1", "you");
                gb_GrammarBuilder.Append(srk);
                gb_GrammarBuilder.Append("are in");
                srk = new SemanticResultKey("PARAM2", containerChoices);
                gb_GrammarBuilder.Append(srk);
                g_Grammar = new Grammar(gb_GrammarBuilder);
                g_Grammar.Name = "[OBJECT] is in [CONTAINER]";
                oRecognizer.LoadGrammar(g_Grammar);

                // You are in np CONTAINER
                gb_GrammarBuilder = new GrammarBuilder();
                srk = new SemanticResultKey("PARAM1", "you");
                gb_GrammarBuilder.Append(srk);
                gb_GrammarBuilder.Append("are in");
                gb_GrammarBuilder.Append(nounPrecedentChoices);
                srk = new SemanticResultKey("PARAM2", containerChoices);
                gb_GrammarBuilder.Append(srk);
                g_Grammar = new Grammar(gb_GrammarBuilder);
                g_Grammar.Name = "[OBJECT] is in [CONTAINER]";
                oRecognizer.LoadGrammar(g_Grammar);



                        
                // is OBJECT in CONTAINER
                gb_GrammarBuilder = new GrammarBuilder("is");
                srk = new SemanticResultKey("PARAM1", objectFullChoices);
                gb_GrammarBuilder.Append(srk);
                gb_GrammarBuilder.Append("in");
                srk = new SemanticResultKey("PARAM2", containerChoices);
                gb_GrammarBuilder.Append(srk);
                g_Grammar = new Grammar(gb_GrammarBuilder);
                g_Grammar.Name = "Is [OBJECT] in [CONTAINER]";
                oRecognizer.LoadGrammar(g_Grammar);

                // is np OBJECT is in CONTAINER
                gb_GrammarBuilder = new GrammarBuilder("is");
                gb_GrammarBuilder.Append(nounPrecedentChoices);
                srk = new SemanticResultKey("PARAM1", objectFullChoices);
                gb_GrammarBuilder.Append(srk);
                gb_GrammarBuilder.Append("in");
                srk = new SemanticResultKey("PARAM2", containerChoices);
                gb_GrammarBuilder.Append(srk);
                g_Grammar = new Grammar(gb_GrammarBuilder);
                g_Grammar.Name = "Is [OBJECT] in [CONTAINER]";
                oRecognizer.LoadGrammar(g_Grammar);

                // is OBJECT in np CONTAINER
                gb_GrammarBuilder = new GrammarBuilder("is");
                srk = new SemanticResultKey("PARAM1", objectFullChoices);
                gb_GrammarBuilder.Append(srk);
                gb_GrammarBuilder.Append("is in");
                gb_GrammarBuilder.Append(nounPrecedentChoices);
                srk = new SemanticResultKey("PARAM2", containerChoices);
                gb_GrammarBuilder.Append(srk);
                g_Grammar = new Grammar(gb_GrammarBuilder);
                g_Grammar.Name = "Is [OBJECT] in [CONTAINER]";
                oRecognizer.LoadGrammar(g_Grammar);

                // is np OBJECT in np CONTAINER
                gb_GrammarBuilder = new GrammarBuilder("is");
                gb_GrammarBuilder.Append(nounPrecedentChoices);
                srk = new SemanticResultKey("PARAM1", objectFullChoices);
                gb_GrammarBuilder.Append(srk);
                gb_GrammarBuilder.Append("in");
                gb_GrammarBuilder.Append(nounPrecedentChoices);
                srk = new SemanticResultKey("PARAM2", containerChoices);
                gb_GrammarBuilder.Append(srk);
                g_Grammar = new Grammar(gb_GrammarBuilder);
                g_Grammar.Name = "Is [OBJECT] in [CONTAINER]";
                oRecognizer.LoadGrammar(g_Grammar);

                // am I in CONTAINER
                gb_GrammarBuilder = new GrammarBuilder("Am");
                srk = new SemanticResultKey("PARAM1", "I");
                gb_GrammarBuilder.Append(srk);
                gb_GrammarBuilder.Append("in");
                srk = new SemanticResultKey("PARAM2", containerChoices);
                gb_GrammarBuilder.Append(srk);
                g_Grammar = new Grammar(gb_GrammarBuilder);
                g_Grammar.Name = "Is [OBJECT] in [CONTAINER]";
                oRecognizer.LoadGrammar(g_Grammar);

                // am I in NP CONTAINER
                gb_GrammarBuilder = new GrammarBuilder("Am");
                srk = new SemanticResultKey("PARAM1", "I");
                gb_GrammarBuilder.Append(srk);
                gb_GrammarBuilder.Append("in");
                gb_GrammarBuilder.Append(nounPrecedentChoices);
                srk = new SemanticResultKey("PARAM2", containerChoices);
                gb_GrammarBuilder.Append(srk);
                g_Grammar = new Grammar(gb_GrammarBuilder);
                g_Grammar.Name = "Is [OBJECT] in [CONTAINER]";
                oRecognizer.LoadGrammar(g_Grammar);

                // are you in CONTAINER
                gb_GrammarBuilder = new GrammarBuilder("Are");
                srk = new SemanticResultKey("PARAM1", "you");
                gb_GrammarBuilder.Append(srk);
                gb_GrammarBuilder.Append("in");
                srk = new SemanticResultKey("PARAM2", containerChoices);
                gb_GrammarBuilder.Append(srk);
                g_Grammar = new Grammar(gb_GrammarBuilder);
                g_Grammar.Name = "Is [OBJECT] in [CONTAINER]";
                oRecognizer.LoadGrammar(g_Grammar);

                // are you in np CONTAINER
                gb_GrammarBuilder = new GrammarBuilder("Are");
                srk = new SemanticResultKey("PARAM1", "you");
                gb_GrammarBuilder.Append(srk);
                gb_GrammarBuilder.Append("in");
                gb_GrammarBuilder.Append(nounPrecedentChoices);
                srk = new SemanticResultKey("PARAM2", containerChoices);
                gb_GrammarBuilder.Append(srk);
                g_Grammar = new Grammar(gb_GrammarBuilder);
                g_Grammar.Name = "Is [OBJECT] in [CONTAINER]";
                oRecognizer.LoadGrammar(g_Grammar);

                AddToLog("Grammer 'is [OBJECT] in [CONTAINER]' Loaded.");



                #endregion

            #region [OBJECT] [OBJECT TYPE]
                // is OBJECT np OBJECT TYPE
                // is np OBJECT np OBJECT TYPE
                // am I np OBJECT TYPE
                // are you np OBJECT TYPE

                // is OBJECT np OBJECT TYPE
                gb_GrammarBuilder = new GrammarBuilder("is");
                srk = new SemanticResultKey("PARAM1", objectFullChoices);
                gb_GrammarBuilder.Append(srk);
                gb_GrammarBuilder.Append(nounPrecedentChoices);
                srk = new SemanticResultKey("PARAM2", objectTypeChoices);
                gb_GrammarBuilder.Append(srk);
                g_Grammar = new Grammar(gb_GrammarBuilder);
                g_Grammar.Name = "Is [OBJECT] [OBJECT TYPE]";
                oRecognizer.LoadGrammar(g_Grammar);

                // is np OBJECT np OBJECT TYPE
                gb_GrammarBuilder = new GrammarBuilder("is");
                gb_GrammarBuilder.Append(nounPrecedentChoices);
                srk = new SemanticResultKey("PARAM1", objectFullChoices);
                gb_GrammarBuilder.Append(srk);
                gb_GrammarBuilder.Append(nounPrecedentChoices);
                srk = new SemanticResultKey("PARAM2", objectTypeChoices);
                gb_GrammarBuilder.Append(srk);
                g_Grammar = new Grammar(gb_GrammarBuilder);
                g_Grammar.Name = "Is [OBJECT] [OBJECT TYPE]";
                oRecognizer.LoadGrammar(g_Grammar);

                // am I np OBJECT TYPE
                gb_GrammarBuilder = new GrammarBuilder("am");
                srk = new SemanticResultKey("PARAM1", "I");
                gb_GrammarBuilder.Append(srk);
                gb_GrammarBuilder.Append(nounPrecedentChoices);
                srk = new SemanticResultKey("PARAM2", objectTypeChoices);
                gb_GrammarBuilder.Append(srk);
                g_Grammar = new Grammar(gb_GrammarBuilder);
                g_Grammar.Name = "Is [OBJECT] [OBJECT TYPE]";
                oRecognizer.LoadGrammar(g_Grammar);

                // are you np OBJECT TYPE
                gb_GrammarBuilder = new GrammarBuilder("are");
                srk = new SemanticResultKey("PARAM1", "you");
                gb_GrammarBuilder.Append(srk);
                gb_GrammarBuilder.Append(nounPrecedentChoices);
                srk = new SemanticResultKey("PARAM2", objectTypeChoices);
                gb_GrammarBuilder.Append(srk);
                g_Grammar = new Grammar(gb_GrammarBuilder);
                g_Grammar.Name = "Is [OBJECT] [OBJECT TYPE]";
                oRecognizer.LoadGrammar(g_Grammar);
            #endregion

            #region Where/What is [OBJECT]
                //Where is OBJECT
                //Where is NP OBJECT
                //Where am I
                //Where are You

                //What is OBJECT
                //What is NP OBJECT
                //What am I
                //What are You
                
                //Where is OBJECT
                GrammarBuilder gb_Single = new GrammarBuilder("Where is");
                srk = new SemanticResultKey("PARAM1", objectFullChoices);
                gb_Single.Append(srk);
                Grammar g_Single = new Grammar(gb_Single);
                g_Single.Name = "Where is [OBJECT]";
                oRecognizer.LoadGrammar(g_Single);

                //Where is NP OBJECT
                gb_Single = new GrammarBuilder("Where is");
                gb_Single.Append(nounPrecedentChoices);
                srk = new SemanticResultKey("PARAM1", objectFullChoices);
                gb_Single.Append(srk);
                g_Single = new Grammar(gb_Single);
                g_Single.Name = "Where is [OBJECT]";
                oRecognizer.LoadGrammar(g_Single);

                //Where am I
                gb_Single = new GrammarBuilder("Where am");
                srk = new SemanticResultKey("PARAM1", "I");
                gb_Single.Append(srk);
                g_Single = new Grammar(gb_Single);
                g_Single.Name = "Where is [OBJECT]";
                oRecognizer.LoadGrammar(g_Single);

                //Where are you
                gb_Single = new GrammarBuilder("Where are");
                srk = new SemanticResultKey("PARAM1", "you");
                gb_Single.Append(srk);
                g_Single = new Grammar(gb_Single);
                g_Single.Name = "Where is [OBJECT]";
                oRecognizer.LoadGrammar(g_Single);


                //What is OBJECT
                gb_Single = new GrammarBuilder("What is");
                srk = new SemanticResultKey("PARAM1", objectFullChoices);
                gb_Single.Append(srk);
                g_Single = new Grammar(gb_Single);
                g_Single.Name = "What is [OBJECT]";
                oRecognizer.LoadGrammar(g_Single);

                //What is NP OBJECT
                gb_Single = new GrammarBuilder("What is");
                gb_Single.Append(nounPrecedentChoices);
                srk = new SemanticResultKey("PARAM1", objectFullChoices);
                gb_Single.Append(srk);
                g_Single = new Grammar(gb_Single);
                g_Single.Name = "What is [OBJECT]";
                oRecognizer.LoadGrammar(g_Single);

                //What am I
                gb_Single = new GrammarBuilder("What am");
                srk = new SemanticResultKey("PARAM1", "I");
                gb_Single.Append(srk);
                g_Single = new Grammar(gb_Single);
                g_Single.Name = "What is [OBJECT]";
                oRecognizer.LoadGrammar(g_Single);

                //What are you
                gb_Single = new GrammarBuilder("What are");
                srk = new SemanticResultKey("PARAM1", "you");
                gb_Single.Append(srk);
                g_Single = new Grammar(gb_Single);
                g_Single.Name = "What is [OBJECT]";
                oRecognizer.LoadGrammar(g_Single);
            #endregion

            #region Who is [PRONOUN]
                //Who am I
                //Who are you

                //Who am I
                gb_Single = new GrammarBuilder("Who am");
                srk = new SemanticResultKey("PARAM1", "I");
                gb_Single.Append(srk);
                g_Single = new Grammar(gb_Single);
                g_Single.Name = "Who is [PERSON]";
                oRecognizer.LoadGrammar(g_Single);

                //Who are you
                gb_Single = new GrammarBuilder("Who are");
                srk = new SemanticResultKey("PARAM1", "you");
                gb_Single.Append(srk);
                g_Single = new Grammar(gb_Single);
                g_Single.Name = "Who is [PERSON]";
                oRecognizer.LoadGrammar(g_Single);
            #endregion

        }

      
        private void Load_User_Grammer()
        {
            //User

            DataSet dsResults = new DataSet();
            try
            {
                //Load all users
                dsResults = OSAESql.RunSQL("SELECT object_name FROM osae_v_object WHERE base_type='PERSON' ORDER BY object_name");
                for (int i = 0; i < dsResults.Tables[0].Rows.Count; i++)
                {
                    userList.Add(dsResults.Tables[0].Rows[i][0].ToString());
                }
            }
            catch (Exception ex)
            {
                AddToLog("Error Loading User Grammer!");
                AddToLog("Error: " + ex.Message);
            }
            Choices userChoices = new Choices(userList.ToArray());

            try
            {
                GrammarBuilder builder = new GrammarBuilder("This is");
                SemanticResultKey srk = new SemanticResultKey("PARAM1", userChoices);
                builder.Append(srk);
                Grammar gram = new Grammar(builder);
                gram.Name = "This is [OBJECT]";
                oRecognizer.LoadGrammar(gram);
                AddToLog("User Grammer 'This is User' Load Completed (" + userList.Count + " items with place-holders)");
            }
            catch (Exception ex)
            {
                AddToLog("I could Not build the 'This is User' Grammer set!");
                AddToLog("Error: " + ex.Message);
            }

            try
            {
                GrammarBuilder builder = new GrammarBuilder("I am");
                SemanticResultKey srk = new SemanticResultKey("PARAM1", userChoices);
                builder.Append(srk);
                Grammar gram = new Grammar(builder);
                gram.Name = "This is [OBJECT]";
                oRecognizer.LoadGrammar(gram);
                AddToLog("Grammer 'I am User' Load Completed (" + userList.Count + " items with place-holders)");
            }
            catch (Exception ex)
            {
                AddToLog("I could Not build the 'I am User' Grammer set!");
                AddToLog("Error: " + ex.Message);
            }
        }
         */

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
                { return; }

                if (sInput.StartsWith("This is [OBJECT]"))
                {
                    AddToLog("Heard: " + sRaw);
                    if (scriptParamaters != "")
                    {
                        gUser = scriptParamaters;
                        AddToLog("I am talking to " + gUser);
                        lblObjectTalking.Content = "I am talking to " + gUser;
                        OSAEObjectPropertyManager.ObjectPropertySet(gUser, "Communication Method", "Speech", gUser);
                        string sText = OSAEGrammar.SearchForMeaning(sInput, scriptParamaters, gUser);
                        //Load_Grammer();
                        //Load_Grammer_With_OT_Substitutions();
                        oRecognizer = OSAEGrammar.Load_Direct_Grammar(oRecognizer);
                        oRecognizer = OSAEGrammar.Load_OSA_Grammar(oRecognizer);
                    }
                }

                // gSpeechPlugin;
                String temp = OSAEObjectPropertyManager.GetObjectPropertyValue(gSpeechPlugin, "Speaking").Value.ToString().ToLower();

                if (temp.ToLower() == "false")
                {
                    if ((gVRMuted == false) || (sleepList.Contains(sInput)))
                    {
                        try
                        {
                            string sLogEntry = "Heard: " + sRaw;
                            //string sText = OSAE.Common.MatchPattern(sInput,gUser);
                            //string sText = MatchPattern(sInput,gUser);
                            string sText = OSAEGrammar.SearchForMeaning(sInput, scriptParamaters, gUser);

                            if (sText.Length > 0) 
                                sLogEntry += ".  Ran: " + sText;

                            AddToLog(sLogEntry);
                        }
                        catch {}
                    }
                }
            }
            catch (Exception ex)
            { AddToLog("Error in _SpeechRecognized: " + ex.Message); }
        }

        private void oRecognizer_StateChanged(object sender, System.Speech.Recognition.AudioStateChangedEventArgs e)
        {
            /*
            //System.Speech.Recognition.AudioState state = oRecognizer.AudioState;
            try
            {
                if (oRecognizer.AudioState == 0 && gRunningManual == false)
                {
                    oRecognizer.RecognizeAsync();
                }
            }
            catch (Exception ex)
            {
                AddToLog("Error trying to Restart Recognition!");
                AddToLog("Errord: " + ex.Message);
            }
             */

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

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            gRunningManual = true;
            oRecognizer.RecognizeAsyncCancel();
            while (oRecognizer.AudioState != 0)
            { }
            RecognitionResult rr = oRecognizer.EmulateRecognize(txtInput.Text);
            txtInput.Text = "";
            gRunningManual = false;
            //oRecognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        /*
        private static string MatchPattern(string str, string ScriptParameter, string sUser)
        {
            //MOVE TO API


            DataSet dataset = new DataSet();
            dataset = OSAESql.RunSQL("SELECT pattern FROM osae_v_pattern_match WHERE `match`='" + str.Replace("'", "''") + "'");
            if (dataset.Tables[0].Rows.Count > 0)
            {
                //Since we have a match, lets execute the scripts
                OSAEScriptManager.RunPatternScript(dataset.Tables[0].Rows[0]["pattern"].ToString(), ScriptParameter, sUser);
                return dataset.Tables[0].Rows[0]["pattern"].ToString();
            }
            else
            {
                return "Sorry!";
            }
        }
         */
    }
}
