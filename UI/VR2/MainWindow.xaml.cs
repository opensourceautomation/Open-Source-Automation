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
using OSAE;
namespace VR2

{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Logging OSAEApi = Logging.GetLogger("VR2");
        SpeechRecognitionEngine oRecognizer = new SpeechRecognitionEngine();
        String gAppName = "";
        Boolean gVRMuted = true;
        Boolean gVREnabled = false;
        String gWakePhrase = "Computer";
        String gSleepPhrase = "Thank You";
        String gSpeechPlugin = "";
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
                oRecognizer.RecognizeAsync();

               AddToLog("Finished Loading, Recognition Started...");
            }
            catch (Exception ex)
            {
                    AddToLog("Unable to set Default Audio Device.  Check Sound Card.");
                    AddToLog("Error: " + ex.Message);
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                this.ShowInTaskbar = true;
            }
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
                AddToLog("Error in AddToLog: " + ex.Message);
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
                temp = OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "VR Wake Phrase").Value.ToString();
                gWakePhrase = temp;
                AddToLog("--  VR Wake Phrase: " + gWakePhrase);

                temp = "";
                temp = OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "VR Sleep Phrase").Value.ToString();
                gSleepPhrase = temp;
                AddToLog("--  VR Sleep Phrase: " + gSleepPhrase);
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
                
                dsResults = OSAESql.RunSQL("SELECT `match` FROM osae_pattern_match ORDER BY `match`");
                grammerList.Add(gWakePhrase);
                grammerList.Add(gSleepPhrase);
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
            AddToLog("Grammer Load Completed (" + grammerList.Count + " items)");
            }
            catch (Exception ex)
            {
                AddToLog("I could Not build the Grammer set!");
                AddToLog("Error: " + ex.Message);
            }
        }

        private void oRecognizer_SpeechRecognized(object sender, System.Speech.Recognition.SpeechRecognizedEventArgs e)
        {
            DataSet dsResults = new DataSet();
            String sPattern = "";
            try
            {
                if ((e.Result.Text == gWakePhrase) & (gVRMuted == true))
                {
                    gVRMuted = false;
                    lblStatus.Content = "I am awake";
                }
                else if ((e.Result.Text == gSleepPhrase) & (gVRMuted == false))
                {
                    gVRMuted = true;
                    lblStatus.Content = "I am sleeping";
                }
               // gSpeechPlugin;
                String temp = OSAEObjectPropertyManager.GetObjectPropertyValue(gSpeechPlugin, "Speaking").Value.ToString().ToLower();

                if (temp.ToLower() == "true")
                {
                    try
                    {
                        AddToLog("Ignored Speech because TTS was talking.");
                    }
                    catch (Exception ex)
                    {
                    }
                }
                else
                {
                    if ((gVRMuted == false) || (e.Result.Text == gSleepPhrase) || (e.Result.Text == gSleepPhrase))
                    {
                        try
                        {
                            string sText = OSAE.Common.MatchPattern(e.Result.Text);

                            //OSAEScriptManager.RunPatternScript(sText, "", "VR"); 
                            AddToLog("Heard: " + e.Result.Text + ", Ran: " + sText);
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
    }
}
