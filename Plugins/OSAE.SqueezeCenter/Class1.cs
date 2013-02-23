using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;

namespace OSAE.SqueezeboxServer
{
    public class SqueezeboxServer : OSAEPluginBase
    {
        private string sbsAddress = "localhost";
        private int sbsPort = 9090;
        private string ttsSave = "";
        private string ttsPlay = "";
        private SqueezeboxServerAPI sbs = new SqueezeboxServerAPI();
        Logging logging = Logging.GetLogger("Squeezebox Server");

        private string pName = null;
               
        public override void ProcessCommand(OSAEMethod method)
        {
            //Process incomming command
            logging.AddToLog("Process command: " + method.MethodName, false);
            logging.AddToLog("Process parameter1: " + method.Parameter1, false);
            logging.AddToLog("Process parameter2: " + method.Parameter2, false);
            logging.AddToLog("Address: " + method.Address, false);

            switch (method.MethodName)
            {
                case "PLAY":
                    if (method.Parameter1.Trim() == string.Empty)
                        sbs.Play(method.Address);
                    else
                        sbs.PlaylistPlay(method.Address, method.Parameter1);
                    OSAEObjectStateManager.ObjectStateSet(OSAEObjectManager.GetObjectByAddress(method.Address).Name, "PLAYING", pName);
                    break;

                case "STOP":
                    sbs.StopPlayer(method.Address);
                    OSAEObjectStateManager.ObjectStateSet(OSAEObjectManager.GetObjectByAddress(method.Address).Name, "STOPPED", pName);
                    break;

                case "NEXT":
                    sbs.Next(method.Address);
                    break;

                case "PREV":
                    sbs.Previous(method.Address);
                    break;

                case "SHOW":
                    sbs.ShowMessage(method.Address, method.Parameter1, Int32.Parse(method.Parameter2));
                    break;

                case "PAUSE":
                    sbs.PausePlayer(method.Address);
                    OSAEObjectStateManager.ObjectStateSet(OSAEObjectManager.GetObjectByAddress(method.Address).Name, "PAUSED", pName);
                    break;

                case "TTS":
                    TextToSpeech(method.Parameter1);
                    sbs.PlaylistPlay(method.Address, ttsPlay);
                    break;
                
                case "TTSLIST":
                    DataSet list = OSAEObjectPropertyManager.ObjectPropertyArrayGetAll(method.Parameter1, method.Parameter2);
                    string tts = "";
                    int count = 1;
                    foreach(DataRow item in list.Tables[0].Rows)
                    {
                        tts += "  RSS item number " + count.ToString() + ".  " + item["item_name"].ToString();
                        count++;
                    }
                    TextToSpeech(tts);
                    sbs.PlaylistPlay(method.Address, ttsPlay);
                    break;
                
                case "TTSLISTRAND":
                    string listItem = OSAEObjectPropertyManager.ObjectPropertyArrayGetRandom(method.Parameter1, method.Parameter2);
                    TextToSpeech(listItem);
                    sbs.PlaylistPlay(method.Address, ttsPlay);
                    break;
            }
        }

        public override void RunInterface(string pluginName)
        {
            pName = pluginName;

            logging.AddToLog("Initializing Plugin", true);
            OSAEObjectTypeManager.ObjectTypeUpdate("SQUEEZEBOX", "SQUEEZEBOX", "Squeezebox", pName, "SQUEEZEBOX", 0, 0, 0, 1);

            sbsAddress = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Server Address").Value;
            sbsPort = Int32.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "CLI Port").Value);
            ttsSave = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "TTS Save Path").Value;
            ttsPlay = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "TTS Play Path").Value;

            logging.AddToLog("address: " + sbsAddress, true);
            logging.AddToLog("port: " + sbsPort, true);
            sbs.mHost = sbsAddress;
            sbs.mPort = sbsPort;
            StringCollection players = sbs.GetPlayers();
            OSAEObjectCollection objects = OSAEObjectManager.GetObjectsByType("SQUEEZEBOX");
            logging.AddToLog("Found " + sbs.GetPlayerCount().ToString() + " players", true);
            foreach (string player in players)
            {
                logging.AddToLog("Found player: " + player, true);
                string[] sb = player.Split(' ');
                bool found = false;
                foreach (OSAEObject obj in objects)
                {
                    if (obj.Address == sb[0])
                    {
                        logging.AddToLog("Found matching object: " + obj.Name, true);
                        found = true;
                    }
                }

                if (!found)
                {
                    logging.AddToLog("No object found.  Adding to OSA", true);
                    OSAEObjectManager.ObjectAdd(sb[1], sb[1], "SQUEEZEBOX", sb[0], "", true);
                }

            }
        }
        
        public override void Shutdown()
        {
            //Nothing to clean up...
        }
        
        public void TextToSpeech(string text)
        {
            logging.AddToLog("Creating wav file of: " + text, false);
            SpeechAudioFormatInfo synthFormat = new SpeechAudioFormatInfo(44100, AudioBitsPerSample.Sixteen, AudioChannel.Stereo);
            SpeechSynthesizer speechEngine = new SpeechSynthesizer();

            logging.AddToLog("setting output: " + ttsSave, false);
            speechEngine.SetOutputToWaveFile(ttsSave, synthFormat);
            logging.AddToLog("speaking", false);
            speechEngine.Speak(text);
            speechEngine.Dispose();
        }
    }
}
