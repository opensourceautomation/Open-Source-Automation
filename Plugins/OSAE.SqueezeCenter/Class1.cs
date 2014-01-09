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
        //OSAELog
        private OSAE.General.OSAELog Log = new General.OSAELog("Squeezebox Server");

        private string pName = null;
               
        public override void ProcessCommand(OSAEMethod method)
        {
            //Process incomming command
            this.Log.Debug("Process command: " + method.MethodName);
            this.Log.Debug("Process parameter1: " + method.Parameter1);
            this.Log.Debug("Process parameter2: " + method.Parameter2);
            this.Log.Debug("Address: " + method.Address);

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

            this.Log.Info("Initializing Plugin");
            OSAEObjectTypeManager.ObjectTypeUpdate("SQUEEZEBOX", "SQUEEZEBOX", "Squeezebox", pName, "SQUEEZEBOX", 0, 0, 0, 1);

            sbsAddress = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Server Address").Value;
            sbsPort = Int32.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "CLI Port").Value);
            ttsSave = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "TTS Save Path").Value;
            ttsPlay = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "TTS Play Path").Value;

            this.Log.Info("address: " + sbsAddress);
            this.Log.Info("port: " + sbsPort);
            sbs.mHost = sbsAddress;
            sbs.mPort = sbsPort;
            StringCollection players = sbs.GetPlayers();
            OSAEObjectCollection objects = OSAEObjectManager.GetObjectsByType("SQUEEZEBOX");
            this.Log.Info("Found " + sbs.GetPlayerCount().ToString() + " players");
            foreach (string player in players)
            {
                this.Log.Info("Found player: " + player);
                string[] sb = player.Split(' ');
                bool found = false;
                foreach (OSAEObject obj in objects)
                {
                    if (obj.Address == sb[0])
                    {
                        this.Log.Info("Found matching object: " + obj.Name);
                        found = true;
                    }
                }

                if (!found)
                {
                    this.Log.Info("No object found.  Adding to OSA");
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
            this.Log.Debug("Creating wav file of: " + text);
            SpeechAudioFormatInfo synthFormat = new SpeechAudioFormatInfo(44100, AudioBitsPerSample.Sixteen, AudioChannel.Stereo);
            SpeechSynthesizer speechEngine = new SpeechSynthesizer();

            this.Log.Debug("setting output: " + ttsSave);
            speechEngine.SetOutputToWaveFile(ttsSave, synthFormat);
            this.Log.Debug("speaking");
            speechEngine.Speak(text);
            speechEngine.Dispose();
        }
    }
}
