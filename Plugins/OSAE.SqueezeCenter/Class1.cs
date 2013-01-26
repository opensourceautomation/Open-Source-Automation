using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Speech.Synthesis;
using System.Speech.AudioFormat;

namespace OSAE.SqueezeboxServer
{
    public class SqueezeboxServer : OSAEPluginBase
    {
        private string sbsAddress = "localhost";
        private int sbsPort = 9090;
        private string ttsSave = "";
        private string ttsPlay = "";
        private SqueezeboxServerAPI sbs = new SqueezeboxServerAPI();
        OSAE osae = new OSAE("Squeezebox Server");
               
        public override void ProcessCommand(OSAEMethod method)
        {
            //Process incomming command
            osae.AddToLog("Process command: " + method.MethodName, false);
            osae.AddToLog("Process parameter1: " + method.Parameter1, false);
            osae.AddToLog("Process parameter2: " + method.Parameter2, false);
            osae.AddToLog("Address: " + method.Address, false);

            switch (method.MethodName)
            {
                case "PLAY":
                    if (method.Parameter1.Trim() == string.Empty)
                        sbs.Play(method.Address);
                    else
                        sbs.PlaylistPlay(method.Address, method.Parameter1);
                    osae.ObjectStateSet(osae.GetObjectByAddress(method.Address).Name, "PLAYING");
                    break;

                case "STOP":
                    sbs.StopPlayer(method.Address);
                    osae.ObjectStateSet(osae.GetObjectByAddress(method.Address).Name, "STOPPED");
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
                    osae.ObjectStateSet(osae.GetObjectByAddress(method.Address).Name, "PAUSED");
                    break;

                case "TTS":
                    TextToSpeech(method.Parameter1);
                    sbs.PlaylistPlay(method.Address, ttsPlay);
                    break;
                
                case "TTSLIST":
                    DataSet list = osae.ObjectPropertyArrayGetAll(method.Parameter1, method.Parameter2);
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
                    string listItem = osae.ObjectPropertyArrayGetRandom(method.Parameter1, method.Parameter2);
                    TextToSpeech(listItem);
                    sbs.PlaylistPlay(method.Address, ttsPlay);
                    break;
            }
        }

        public override void RunInterface(string pName)
        {

            osae.AddToLog("Initializing Plugin", true);
            osae.ObjectTypeUpdate("SQUEEZEBOX", "SQUEEZEBOX", "Squeezebox", pName, "SQUEEZEBOX", 0, 0, 0, 1);

            sbsAddress = osae.GetObjectPropertyValue(pName, "Server Address").Value;
            sbsPort = Int32.Parse(osae.GetObjectPropertyValue(pName, "CLI Port").Value);
            ttsSave = osae.GetObjectPropertyValue(pName, "TTS Save Path").Value;
            ttsPlay = osae.GetObjectPropertyValue(pName, "TTS Play Path").Value;

            osae.AddToLog("address: " + sbsAddress, true);
            osae.AddToLog("port: " + sbsPort, true);
            sbs.mHost = sbsAddress;
            sbs.mPort = sbsPort;
            StringCollection players = sbs.GetPlayers();
            List<OSAEObject> objects = osae.GetObjectsByType("SQUEEZEBOX");
            osae.AddToLog("Found " + sbs.GetPlayerCount().ToString() + " players", true);
            foreach (string player in players)
            {
                osae.AddToLog("Found player: " + player, true);
                string[] sb = player.Split(' ');
                bool found = false;
                foreach (OSAEObject obj in objects)
                {
                    if (obj.Address == sb[0])
                    {
                        osae.AddToLog("Found matching object: " + obj.Name, true);
                        found = true;
                    }
                }

                if (!found)
                {
                    osae.AddToLog("No object found.  Adding to OSA", true);
                    osae.ObjectAdd(sb[1], sb[1], "SQUEEZEBOX", sb[0], "", true);
                }

            }
        }
        
        public override void Shutdown()
        {
            //Nothing to clean up...
        }
        
        public void TextToSpeech(string text)
        {
            osae.AddToLog("Creating wav file of: " + text, false);
            SpeechAudioFormatInfo synthFormat = new SpeechAudioFormatInfo(44100, AudioBitsPerSample.Sixteen, AudioChannel.Stereo);
            SpeechSynthesizer speechEngine = new SpeechSynthesizer();

            osae.AddToLog("setting output: " + ttsSave, false);
            speechEngine.SetOutputToWaveFile(ttsSave, synthFormat);
            osae.AddToLog("speaking", false);
            speechEngine.Speak(text);
            speechEngine.Dispose();
        }
    }
}
