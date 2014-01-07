namespace OSAE.Speech
{
    using System;
    using System.Speech.Synthesis;

    public class SPEECH : OSAEPluginBase
    {
        //OSAELog
        private OSAE.General.OSAELog Log = new General.OSAELog("Speech");

        SpeechSynthesizer oSpeech = new SpeechSynthesizer();
        WMPLib.WindowsMediaPlayer wmPlayer = new WMPLib.WindowsMediaPlayer();
        String gAppName = "";
        String gSelectedVoice = "";
        public override void RunInterface(string pluginName)
        {
            gAppName = pluginName;
            this.Log.Info("Speech Client's Object Name: " + gAppName);
            Load_Settings();
            oSpeech.Speak("speech client started");

        }

        public override void ProcessCommand(OSAEMethod method)
        {          
            string sMethod = method.MethodName;
            string sParam1 = method.Parameter1;
            string sParam2 = method.Parameter2;

            this.Log.Info("Received Command to: " + sMethod + " (" + sParam1 + ", " + sParam2 + ")");

            if (sMethod == "SPEAK")
            {
                string sText = Common.PatternParse(sParam1);
                OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "Speaking", "TRUE", gAppName);
                oSpeech.Speak(sText);
                this.Log.Info("Said " + sText);
                OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "Speaking", "FALSE", gAppName);
            }
            else if (sMethod == "SPEAKFROM")
            {
                this.Log.Info("--Speak From Object: " + sParam1 + " and pick From list: " + sParam2);
                string sText = OSAEObjectPropertyManager.ObjectPropertyArrayGetRandom(sParam1, sParam2).ToString();
                sText = Common.PatternParse(sText);
                OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "Speaking", "TRUE", gAppName);
                oSpeech.Speak(sText);
                OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "Speaking", "FALSE", gAppName);
                this.Log.Info("Said " + sText);
            }
            else if (sMethod == "PLAY")
            {
                wmPlayer.URL = sParam1;
                wmPlayer.controls.play();
                this.Log.Info("Played " + sParam1);
            }
            else if (sMethod == "PLAYFROM")
            {
                string sFile = OSAEObjectPropertyManager.ObjectPropertyArrayGetRandom(sParam1, sParam2).ToString();
                wmPlayer.URL = sFile;
                wmPlayer.controls.play();
                this.Log.Info("Played " + sFile);
            }
            else if (sMethod == "STOP")
            {
                wmPlayer.controls.stop();
                this.Log.Info("Stopped");
            }
            else if (sMethod == "PAUSE")
            {
                wmPlayer.controls.pause();
                this.Log.Info("Paused");
            }
            else if (sMethod == "SETVOICE")
            {
                gSelectedVoice = sParam1;
                oSpeech.SelectVoice(gSelectedVoice);
                this.Log.Info("Voice Set to " + gSelectedVoice);
            }
            else if (sMethod == "SETTTSVOLUME")
            {
                if (Convert.ToInt16(sParam1) > 0 && Convert.ToInt16(sParam1) <= 100)
                {
                    oSpeech.Volume = Convert.ToInt16(sParam1);
                    OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "TTS Volume", sParam1, "SPEECH");
                    this.Log.Info("TTS Volume Set to " + Convert.ToInt16(sParam1));
                }
            }
            else if (sMethod == "SETTTSRATE")
            {
                // Load the speech rate, which must be -10 to 10, and set it to 0 if it is not valid.
                Int16 iTTSRate = Convert.ToInt16(sParam1);
                if (iTTSRate < -10 || iTTSRate > 10)
                {
                    iTTSRate = 0;
                    this.Log.Info("TTS Rate was invalid! I changed it to " + iTTSRate.ToString());
                }
                OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "TTS Rate", iTTSRate.ToString(), gAppName);
                this.Log.Info("TTS Rate Set to " + iTTSRate.ToString());
                oSpeech.Rate = iTTSRate;
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
                        this.Log.Info("Default Voice Set to " + gSelectedVoice);
                    }
                    this.Log.Info("Adding Voice: " + i.VoiceInfo.Name);
                    OSAEObjectPropertyManager.ObjectPropertyArrayAdd(gAppName, "Voices", i.VoiceInfo.Name, "Voice");
                }

                if (gSelectedVoice != "")
                {
                    oSpeech.SelectVoice(gSelectedVoice);
                    this.Log.Info("Current Voice Set to " + gSelectedVoice);
                }

                // Load the speech rate, which must be -10 to 10, and set it to 0 if it is not valid.
                Int16 iTTSRate = Convert.ToInt16(OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "TTS Rate").Value);
                if (iTTSRate < -10 || iTTSRate > 10)
                {
                    iTTSRate = 0;
                    OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "TTS Rate", iTTSRate.ToString(), gAppName);
                    this.Log.Info("TTS Rate was invalid! I changed it to " + iTTSRate.ToString());
                }
                this.Log.Info("TTS Rate Set to " + iTTSRate.ToString());
                oSpeech.Rate = iTTSRate;
                

                Int16 iTTSVolume = Convert.ToInt16(OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "TTS Volume").Value);
                if (iTTSVolume > -11 && iTTSVolume <= 11)
                {
                    oSpeech.Rate = iTTSVolume;
                    this.Log.Info("TTS Rate Set to " + iTTSVolume.ToString());
                }
            }
            catch (Exception ex)
            {
                this.Log.Error("Error in Load_Voices!", ex);
            }
        }

        public override void Shutdown()
        {
            this.Log.Info("Recieved Shutdown Order.   All I do is display this...");
        }
    }
}
