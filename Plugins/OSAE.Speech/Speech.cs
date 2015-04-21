namespace OSAE.Speech
{
    using System;
    using System.Speech.Synthesis;
    using System.Threading;

    public class SPEECH : OSAEPluginBase
    {
        private OSAE.General.OSAELog Log = new General.OSAELog();

        SpeechSynthesizer oSpeech = new SpeechSynthesizer();
        WMPLib.WindowsMediaPlayer wmPlayer = new WMPLib.WindowsMediaPlayer();
        String gAppName = "";
        String gSelectedVoice = "";
        Boolean gDebug = false;

        public override void RunInterface(string pluginName)
        {
            gAppName = pluginName;
            OwnTypes();
            Load_Settings();
            oSpeech.Speak("speech client started");
        }

        public void OwnTypes()
        {
            //Added the follow to automatically own Speech Base types that have no owner.
            OSAEObjectType oType = OSAEObjectTypeManager.ObjectTypeLoad("SPEECH");

            if (oType.OwnedBy == "")
            {
                OSAEObjectTypeManager.ObjectTypeUpdate(oType.Name, oType.Name, oType.Description, gAppName, oType.BaseType, oType.Owner, oType.SysType, oType.Container, oType.HideRedundant);
                Log.Info("Speech Plugin took ownership of the Speech Object Type.");
            }
            else
            {
                Log.Info("The Speech Plugin correctly owns the Speech Object Type.");
            }
        }

        public override void ProcessCommand(OSAEMethod method)
        {          
            string sMethod = method.MethodName;
            string sParam1 = method.Parameter1;
            string sParam2 = method.Parameter2;

            if (gDebug) Log.Debug("Received Command to: " + sMethod + " (" + sParam1 + ", " + sParam2 + ")");

            if (sMethod == "SPEAK")
            {
                string sText = Common.PatternParse(sParam1);
                OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "Speaking", "TRUE", gAppName);
                oSpeech.Speak(sText);
                Log.Info("Said " + sText);
                Thread.Sleep(500);
                OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "Speaking", "FALSE", gAppName);
            }
            else if (sMethod == "SPEAKFROM")
            {
                Log.Info("--Speak From Object: " + sParam1 + " and pick From list: " + sParam2);
                string sText = OSAEObjectPropertyManager.ObjectPropertyArrayGetRandom(sParam1, sParam2).ToString();
                sText = Common.PatternParse(sText);
                OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "Speaking", "TRUE", gAppName);
                oSpeech.Speak(sText);
                Log.Info("Said " + sText);
                Thread.Sleep(500);
                OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "Speaking", "FALSE", gAppName);
            }
            else if (sMethod == "PLAY")
            {
                wmPlayer.URL = sParam1;
                wmPlayer.controls.play();
                Log.Info("Played " + sParam1);
            }
            else if (sMethod == "PLAYFROM")
            {
                string sFile = OSAEObjectPropertyManager.ObjectPropertyArrayGetRandom(sParam1, sParam2).ToString();
                wmPlayer.URL = sFile;
                wmPlayer.controls.play();
                Log.Info("Played " + sFile);
            }
            else if (sMethod == "STOP")
            {
                wmPlayer.controls.stop();
                Log.Info("Stopped");
            }
            else if (sMethod == "PAUSE")
            {
                wmPlayer.controls.pause();
                Log.Info("Paused");
            }
            else if (sMethod == "SETVOICE")
            {
                gSelectedVoice = sParam1;
                oSpeech.SelectVoice(gSelectedVoice);
                Log.Info("Voice Set to " + gSelectedVoice);
            }
            else if (sMethod == "SETTTSVOLUME")
            {
                if (Convert.ToInt16(sParam1) > 0 && Convert.ToInt16(sParam1) <= 100)
                {
                    oSpeech.Volume = Convert.ToInt16(sParam1);
                    OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "TTS Volume", sParam1, "SPEECH");
                    Log.Info("TTS Volume Set to " + Convert.ToInt16(sParam1));
                }
            }
            else if (sMethod == "SETTTSRATE")
            {
                // Load the speech rate, which must be -10 to 10, and set it to 0 if it is not valid.
                Int16 iTTSRate = Convert.ToInt16(sParam1);
                if (iTTSRate < -10 || iTTSRate > 10)
                {
                    iTTSRate = 0;
                    Log.Info("TTS Rate was invalid! I changed it to " + iTTSRate.ToString());
                }
                OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "TTS Rate", iTTSRate.ToString(), gAppName);
                Log.Info("TTS Rate Set to " + iTTSRate.ToString());
                oSpeech.Rate = iTTSRate;
            }
        }

        private void Load_Settings()
        {        
            try
            {
                try
                {
                    gDebug = Convert.ToBoolean(OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Debug").Value);
                }
                catch
                {
                    Log.Error("I think the Debug property is missing from the Speech object type!");
                }
                Log.Info("Debug Mode Set to " + gDebug);
                gSelectedVoice = OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Voice").Value;
                bool VoiceValid = false;
                string aValidVoice = "";
                OSAEObjectPropertyManager.ObjectPropertyArrayDeleteAll(gAppName, "Voices");
                foreach (System.Speech.Synthesis.InstalledVoice i in oSpeech.GetInstalledVoices())
                {
                    if (aValidVoice == "") aValidVoice = i.VoiceInfo.Name;
                    if (gSelectedVoice == "")
                    {
                        gSelectedVoice = i.VoiceInfo.Name;
                        OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "Voice", gSelectedVoice, "SPEECH");
                        Log.Info("Default Voice Set to " + gSelectedVoice);
                    }
                    Log.Info("Adding Voice: " + i.VoiceInfo.Name);
                    if (gSelectedVoice == i.VoiceInfo.Name) VoiceValid = true;
                    OSAEObjectPropertyManager.ObjectPropertyArrayAdd(gAppName, "Voices", i.VoiceInfo.Name, "Voice");
                }
                if (VoiceValid != true) gSelectedVoice = aValidVoice;

                if (gSelectedVoice != "")
                {
                    oSpeech.SelectVoice(gSelectedVoice);
                    Log.Info("Current Voice Set to " + gSelectedVoice);
                }
                

                // Load the speech rate, which must be -10 to 10, and set it to 0 if it is not valid.
                Int16 iTTSRate = 0;
                try
                {
                    iTTSRate = Convert.ToInt16(OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "TTS Rate").Value);
                }
                catch
                {
                    OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "TTS Rate", iTTSRate.ToString(), gAppName);
                    Log.Info("TTS Rate was invalid! I changed it to " + iTTSRate.ToString());
                }
                if (iTTSRate < -10 || iTTSRate > 10)
                {
                    iTTSRate = 0;
                    OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "TTS Rate", iTTSRate.ToString(), gAppName);
                    Log.Info("TTS Rate was invalid! I changed it to " + iTTSRate.ToString());
                }
                Log.Info("TTS Rate Set to " + iTTSRate.ToString());
                oSpeech.Rate = iTTSRate;
                Int16 iTTSVolume = 0;
                try
                {
                    iTTSVolume = Convert.ToInt16(OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "TTS Volume").Value);
                }
                catch
                {
                    OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "TTS Volume", iTTSVolume.ToString(), gAppName);
                    Log.Info("TTS Volume was invalid! I changed it to " + iTTSVolume.ToString());
                }
                if (iTTSVolume < -10 || iTTSVolume > 10)
                {
                    iTTSVolume = 0;
                    OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "TTS Volume", iTTSVolume.ToString(), gAppName);
                    Log.Info("TTS Volume was invalid! I changed it to " + iTTSVolume.ToString());
                }
                oSpeech.Rate = iTTSVolume;
                Log.Info("TTS Volume Set to " + iTTSVolume.ToString());
            }
            catch (Exception ex)
            {
                Log.Error("Error in Load_Settings!", ex);
            }
        }

        public override void Shutdown()
        {
            this.Log.Info("Recieved Shutdown Order.");
        }
    }
}
