using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Speech.Synthesis;
using System.Speech.AudioFormat;
using System.AddIn;
using OpenSourceAutomation;

namespace OSAE.SqueezeboxServer
{
    [AddIn("Squeezebox Server", Version = "0.3.4")]
    public class SqueezeboxServer : IOpenSourceAutomationAddIn
    {
        private string sbsAddress = "localhost";
        private int sbsPort = 9090;
        private string ttsSave = "";
        private string ttsPlay = "";
        private SqueezeboxServerAPI sbs = new SqueezeboxServerAPI();
        OSAE osae = new OSAE("Squeezebox Server");
               
        public void ProcessCommand(System.Data.DataTable table)
        {
            System.Data.DataRow row = table.Rows[0];
            //Process incomming command
            osae.AddToLog("Process command: " + row["method_name"], false);
            osae.AddToLog("Process parameter1: " + row["parameter_1"], false);
            osae.AddToLog("Process parameter2: " + row["parameter_2"], false);
            osae.AddToLog("Address: " + row["address"], false);

            switch (row["method_name"].ToString())
            {
                case "PLAY":
                    if (row["parameter_1"].ToString().Trim() == string.Empty)
                        sbs.Play(row["address"].ToString());
                    else
                        sbs.PlaylistPlay(row["address"].ToString(), row["parameter_1"].ToString());
                    osae.ObjectStateSet(osae.GetObjectByAddress(row["address"].ToString()).Name, "PLAYING");
                    break;

                case "STOP":
                    sbs.StopPlayer(row["address"].ToString());
                    osae.ObjectStateSet(osae.GetObjectByAddress(row["address"].ToString()).Name, "STOPPED");
                    break;

                case "NEXT":
                    sbs.Next(row["address"].ToString());
                    break;

                case "PREV":
                    sbs.Previous(row["address"].ToString());
                    break;

                case "SHOW":
                    sbs.ShowMessage(row["address"].ToString(), row["parameter_1"].ToString(), Int32.Parse(row["parameter_2"].ToString()));
                    break;

                case "PAUSE":
                    sbs.PausePlayer(row["address"].ToString());
                    osae.ObjectStateSet(osae.GetObjectByAddress(row["address"].ToString()).Name, "PAUSED");
                    break;

                case "TTS":
                    TextToSpeech(row["parameter_1"].ToString());
                    sbs.PlaylistPlay(row["address"].ToString(), ttsPlay);
                    break;
                
                case "TTSLIST":
                    DataSet list = osae.ObjectPropertyArrayGetAll(row["parameter_1"].ToString(), row["parameter_2"].ToString());
                    string tts = "";
                    int count = 1;
                    foreach(DataRow item in list.Tables[0].Rows)
                    {
                        tts += "  RSS item number " + count.ToString() + ".  " + item["item_name"].ToString();
                        count++;
                    }
                    TextToSpeech(tts);
                    sbs.PlaylistPlay(row["address"].ToString(), ttsPlay);
                    break;
                
                case "TTSLISTRAND":
                    string listItem = osae.ObjectPropertyArrayGetRandom(row["parameter_1"].ToString(), row["parameter_2"].ToString());
                    TextToSpeech(listItem);
                    sbs.PlaylistPlay(row["address"].ToString(), ttsPlay);
                    break;
            }
        }

        public void RunInterface(string pName)
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
        
        public void Shutdown()
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
