using System;
using System.AddIn;
using System.Speech.Synthesis;
using OpenSourceAutomation;

namespace OSAE.Speech
{
    [AddIn("SPEECH", Version = "0.3.6")]
    public class SPEECH : IOpenSourceAutomationAddIn
    {
        OSAE OSAEApi = new OSAE("SPEECH");
        SpeechSynthesizer oSpeech = new SpeechSynthesizer();
        WMPLib.WindowsMediaPlayer wmPlayer = new WMPLib.WindowsMediaPlayer();
        String gAppName = string.Empty;
        String gSelectedVoice = string.Empty;

        public void RunInterface(string pluginName)
        {
            gAppName = pluginName;
            OSAEApi.AddToLog("Speech Client's Object Name: " + gAppName, true);
            Load_Settings();
            oSpeech.Speak("speech client started");
        }

        public void ProcessCommand(System.Data.DataTable table)
        {
            System.Data.DataRow row = table.Rows[0];
            string sMethod = row["method_name"].ToString();
            string sParam1 = row["parameter_1"].ToString();
            string sParam2 = row["parameter_2"].ToString();
            OSAEApi.AddToLog("Received Command to: " + sMethod + " (" + sParam1 + ", " + sParam2 + ")", true);
            if (sMethod == "SPEAK")
            {
                string sText = OSAEApi.PatternParse(sParam1);
                oSpeech.Speak(sText);
                OSAEApi.AddToLog("Said " + sText, true);
            }
            else if (sMethod == "SPEAKFROM")
            {
                OSAEApi.AddToLog("--Speak From Object: " + sParam1 + " and pick From list: " + sParam2, true);
                string sText = OSAEApi.ObjectPropertyArrayGetRandom(sParam1, sParam2).ToString();
                sText = OSAEApi.PatternParse(sText);
                oSpeech.Speak(sText);
                OSAEApi.AddToLog("Said " + sText, true);
            }
            else if (sMethod == "PLAY")
            {
                wmPlayer.URL = sParam1;
                wmPlayer.controls.play();
                OSAEApi.AddToLog("Played " + sParam1, true);
            }
            else if (sMethod == "PLAYFROM")
            {
                string sFile = OSAEApi.ObjectPropertyArrayGetRandom(sParam1, sParam2).ToString();
                wmPlayer.URL = sFile;
                wmPlayer.controls.play();
                OSAEApi.AddToLog("Played " + sFile, true);
            }
            else if (sMethod == "STOP")
            {
                wmPlayer.controls.stop();
                OSAEApi.AddToLog("Stopped",true);
            }
            else if (sMethod == "PAUSE")
            {
                wmPlayer.controls.pause();
                OSAEApi.AddToLog("Paused",true);
            }
            else if (sMethod == "SETVOICE")
            {
                gSelectedVoice = sParam1;
                oSpeech.SelectVoice(gSelectedVoice);
                OSAEApi.AddToLog("Voice Set to " + gSelectedVoice, true);
            }
            else if (sMethod == "SETTTSVOLUME")
            {
                if (Convert.ToInt16(sParam1) > 0 && Convert.ToInt16(sParam1) <= 100)
                {
                    oSpeech.Volume = Convert.ToInt16(sParam1);
                    OSAEApi.ObjectPropertySet(gAppName, "TTS Volume", sParam1);
                    OSAEApi.AddToLog("TTS Volume Set to " + Convert.ToInt16(sParam1), true);
                }
            }
            else if (sMethod == "SETTTSRATE")
            {
                if (Convert.ToInt16(sParam1) > -11 && Convert.ToInt16(sParam1) < 11)
                {
                    oSpeech.Rate = Convert.ToInt16(sParam1);
                    OSAEApi.ObjectPropertySet(gAppName, "TTS Rate", sParam1);
                    OSAEApi.AddToLog("TTS Rate Set to " + Convert.ToInt16(sParam1), true);
                }
            }
        }

        private void AddToLog(string message,Boolean always)
        {
            try
            {
                OSAEApi.AddToLog(message, always);
            }
            catch 
            {
            }
        }

        private void Load_Settings()
        {
            try
            {
                gSelectedVoice = OSAEApi.GetObjectPropertyValue(gAppName, "Voice").Value;
                OSAEApi.ObjectPropertyArrayDeleteAll(gAppName, "Voices");
                foreach (System.Speech.Synthesis.InstalledVoice i in oSpeech.GetInstalledVoices())
                {
                    if (gSelectedVoice == "")
                    {
                        gSelectedVoice = i.VoiceInfo.Name;
                        OSAEApi.ObjectPropertySet(gAppName, "Voice", gSelectedVoice);
                        OSAEApi.AddToLog("Default Voice Set to " + gSelectedVoice, true);
                    }
                    OSAEApi.AddToLog("Adding Voice: " + i.VoiceInfo.Name, false);
                    OSAEApi.ObjectPropertyArrayAdd(gAppName, "Voices", i.VoiceInfo.Name, "Voice");
                }

                if (gSelectedVoice != "")
                {
                    oSpeech.SelectVoice(gSelectedVoice);
                    OSAEApi.AddToLog("Current Voice Set to " + gSelectedVoice, true);
                }

                Int16 iTTSRate = Convert.ToInt16(OSAEApi.GetObjectPropertyValue(gAppName, "TTS Rate").Value);
                if (iTTSRate > 0 && iTTSRate <= 100)
                {
                    oSpeech.Rate = iTTSRate;
                    OSAEApi.AddToLog("TTS Rate Set to " + iTTSRate.ToString(), true);
                }

                Int16 iTTSVolume = Convert.ToInt16(OSAEApi.GetObjectPropertyValue(gAppName, "TTS Volume").Value);
                if (iTTSVolume > -11 && iTTSVolume <= 11)
                {
                    oSpeech.Rate = iTTSVolume;
                    OSAEApi.AddToLog("TTS Rate Set to " + iTTSVolume.ToString(), true);
                }
            }
            catch (Exception ex)
            {
                OSAEApi.AddToLog("Error in Load_Voices!", true);
                OSAEApi.AddToLog("- " + ex.Message.ToString(), true);
            }
        }

        public void Shutdown()
        {
            AddToLog("Recieved Shutdown Order.   All I do is display this...", true);
        }
    }
}
