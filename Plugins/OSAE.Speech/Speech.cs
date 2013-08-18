namespace OSAE.Speech
{
    using System;
    using System.Speech.Synthesis;

    public class SPEECH : OSAEPluginBase
    {
        /// <summary>
        /// Provides access to logging
        /// </summary>
        Logging logging = Logging.GetLogger("SPEECH");

        SpeechSynthesizer oSpeech = new SpeechSynthesizer();
        WMPLib.WindowsMediaPlayer wmPlayer = new WMPLib.WindowsMediaPlayer();
        String gAppName = "";
        String gSelectedVoice = "";
        public override void RunInterface(string pluginName)
        {
            gAppName = pluginName;
            logging.AddToLog("Speech Client's Object Name: " + gAppName, true);
            Load_Settings();
            oSpeech.Speak("speech client started");

        }

        public override void ProcessCommand(OSAEMethod method)
        {          
            string sMethod = method.MethodName;
            string sParam1 = method.Parameter1;
            string sParam2 = method.Parameter2;

            logging.AddToLog("Received Command to: " + sMethod + " (" + sParam1 + ", " + sParam2 + ")", true);

            if (sMethod == "SPEAK")
            {
                string sText = Common.PatternParse(sParam1);
                OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "Speaking", "TRUE", gAppName);
                oSpeech.Speak(sText);
                logging.AddToLog("Said " + sText, true);
                OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "Speaking", "FALSE", gAppName);
            }
            else if (sMethod == "SPEAKFROM")
            {
                logging.AddToLog("--Speak From Object: " + sParam1 + " and pick From list: " + sParam2, true);
                string sText = OSAEObjectPropertyManager.ObjectPropertyArrayGetRandom(sParam1, sParam2).ToString();
                sText = Common.PatternParse(sText);
                OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "Speaking", "TRUE", gAppName);
                oSpeech.Speak(sText);
                OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "Speaking", "FALSE", gAppName);
                logging.AddToLog("Said " + sText, true);
            }
            else if (sMethod == "PLAY")
            {
                wmPlayer.URL = sParam1;
                wmPlayer.controls.play();
                logging.AddToLog("Played " + sParam1, true);
            }
            else if (sMethod == "PLAYFROM")
            {
                string sFile = OSAEObjectPropertyManager.ObjectPropertyArrayGetRandom(sParam1, sParam2).ToString();
                wmPlayer.URL = sFile;
                wmPlayer.controls.play();
                logging.AddToLog("Played " + sFile, true);
            }
            else if (sMethod == "STOP")
            {
                wmPlayer.controls.stop();
                logging.AddToLog("Stopped",true);
            }
            else if (sMethod == "PAUSE")
            {
                wmPlayer.controls.pause();
                logging.AddToLog("Paused",true);
            }
            else if (sMethod == "SETVOICE")
            {
                gSelectedVoice = sParam1;
                oSpeech.SelectVoice(gSelectedVoice);
                logging.AddToLog("Voice Set to " + gSelectedVoice, true);
            }
            else if (sMethod == "SETTTSVOLUME")
            {
                if (Convert.ToInt16(sParam1) > 0 && Convert.ToInt16(sParam1) <= 100)
                {
                    oSpeech.Volume = Convert.ToInt16(sParam1);
                    OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "TTS Volume", sParam1, "SPEECH");
                    logging.AddToLog("TTS Volume Set to " + Convert.ToInt16(sParam1), true);
                }
            }
            else if (sMethod == "SETTTSRATE")
            {
                // Load the speech rate, which must be -10 to 10, and set it to 0 if it is not valid.
                Int16 iTTSRate = Convert.ToInt16(sParam1);
                if (iTTSRate < -10 || iTTSRate > 10)
                {
                    iTTSRate = 0;
                    logging.AddToLog("TTS Rate was invalid! I changed it to " + iTTSRate.ToString(), true);
                }
                OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "TTS Rate", iTTSRate.ToString(), gAppName);
                logging.AddToLog("TTS Rate Set to " + iTTSRate.ToString(), true);
                oSpeech.Rate = iTTSRate;
            }
        }

        private void AddToLog(string message,Boolean always)
        {
            try
            {
                logging.AddToLog(message, always);
            }
            catch 
            {
            }
        }

        private void Load_Settings()
        {        
            try
            {
                gSelectedVoice = OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Voice").Value;
                OSAEObjectPropertyManager.ObjectPropertyArrayDeleteAll(gAppName, "Voices");
                foreach (System.Speech.Synthesis.InstalledVoice i in oSpeech.GetInstalledVoices())
                {
                    if (gSelectedVoice == "")
                    {
                        gSelectedVoice = i.VoiceInfo.Name;
                        OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "Voice", gSelectedVoice, "SPEECH");
                        logging.AddToLog("Default Voice Set to " + gSelectedVoice, true);
                    }
                    logging.AddToLog("Adding Voice: " + i.VoiceInfo.Name, false);
                    OSAEObjectPropertyManager.ObjectPropertyArrayAdd(gAppName, "Voices", i.VoiceInfo.Name, "Voice");
                }

                if (gSelectedVoice != "")
                {
                    oSpeech.SelectVoice(gSelectedVoice);
                    logging.AddToLog("Current Voice Set to " + gSelectedVoice, true);
                }

                // Load the speech rate, which must be -10 to 10, and set it to 0 if it is not valid.
                Int16 iTTSRate = Convert.ToInt16(OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "TTS Rate").Value);
                if (iTTSRate < -10 || iTTSRate > 10)
                {
                    iTTSRate = 0;
                    OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "TTS Rate", iTTSRate.ToString(), gAppName);
                    logging.AddToLog("TTS Rate was invalid! I changed it to " + iTTSRate.ToString(), true);
                }
                logging.AddToLog("TTS Rate Set to " + iTTSRate.ToString(), true);
                oSpeech.Rate = iTTSRate;
                

                Int16 iTTSVolume = Convert.ToInt16(OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "TTS Volume").Value);
                if (iTTSVolume > -11 && iTTSVolume <= 11)
                {
                    oSpeech.Rate = iTTSVolume;
                    logging.AddToLog("TTS Rate Set to " + iTTSVolume.ToString(), true);
                }
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error in Load_Voices!", true);
                logging.AddToLog("- " + ex.Message.ToString(), true);
            }
        }

        public override void Shutdown()
        {
            AddToLog("Recieved Shutdown Order.   All I do is display this...", true);
        }
    }
}
