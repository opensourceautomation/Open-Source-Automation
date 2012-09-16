using System.Net.Sockets;
using System.Web;
using System.Diagnostics;
using System.Collections.Specialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OSAE.SqueezeboxServer
{

    public class SqueezeboxServerAPI
    {
        //class to control a Squeezebox Server
        //Work in progress - contact visual@pcw.co.uk for comments or improvements

        public string mHost = "localhost";
        public int mPort = 9090;

        public SqueezeboxServerAPI()
        {

        }

        public SqueezeboxServerAPI(string Hostval)
        {
            mHost = Hostval;
        }

        public SqueezeboxServerAPI(string Hostval, int portval)
        {

            //TODO check that host and port are valid

            mHost = Hostval;

            mPort = portval;
        }

        public StringCollection GetPlayers()
        {
            StringCollection @params = new StringCollection();
            @params.Add("0");
            @params.Add(GetPlayerCount().ToString());
            StringCollection result = new StringCollection();
            string s = SendCommand("players", @params);

            int iPos = s.IndexOf("playerid:");

            while (iPos > -1)
            {
                string sName = null;
                string sID = null;
                s = s.Substring(iPos + 9);

                //get the index of the next space
                int iLen = s.IndexOf(" ");
                sID = s.Substring(0, iLen);

                int iPosName = s.IndexOf("name:");

                if (iPosName > -1)
                {
                    s = s.Substring(iPosName + 5);
                    iLen = s.IndexOf("model:");
                    sName = s.Substring(0, iLen);
                }

                result.Add(sID + " " + sName);

                iPos = s.IndexOf("playerid:");
            }

            return result;
        }

        public string SetVolume(string playerID, int iVol)
        {
            if (iVol < 0 | iVol > 100)
            {
                return "Illegal volume";
            }
            StringCollection @params = new StringCollection();
            @params.Add(iVol.ToString());
            return SendCommand(playerID, "mixer volume", @params);
        }

        public int GetPlayerCount()
        {
            string s = SendCommand("player count ?");
            s = s.Replace("player count", "").Trim();
            if (s != string.Empty)
            {
                return int.Parse(s);
            }
            else
            {
                return 0;
            }
        }

        public string ShowMessage(string playerID, string sMessage, int duration)
        {
            if (playerID == "" || sMessage == "")
            {
                return "";
            }

            StringCollection @params = new StringCollection();
            @params.Add("line2:" + sMessage);
            @params.Add("duration:" + duration.ToString());
            @params.Add("centered:1");
            @params.Add("font:huge");


            return this.SendCommand(playerID, "show", @params);
        }

        public string StopPlayer(string playerID)
        {
            return SendCommand(playerID, "stop", null);
        }

        public string PausePlayer(string playerID)
        {
            return SendCommand(playerID, "pause", null);
        }

        public string Play(string playerID)
        {
            return SendCommand(playerID, "play", null);
        }

        public string Next(string playerID)
        {
            return SendCommand(playerID, "playlist index +1", null);
        }

        public string Previous(string playerID)
        {
            return SendCommand(playerID, "playlist index -1", null);
        }

        public string PlaylistPlay(string playerID, string path)
        {
            StringCollection @params = new StringCollection();
            @params.Add(path);
            return SendCommand(playerID, "playlist play", @params);
        }

        public string GetSongInfo(string playerID, int trackid)
        {

            StringCollection @params = new StringCollection();
            @params.Add("0");
            @params.Add("100");
            // all the info

            return this.SendCommand(playerID, "songinfo", @params);
        }

        private string EncodeString(string s)
        {
            s = HttpUtility.UrlPathEncode(s);
            // UrlEncode replaces spaces with +, not what we want
            return s;
        }

        private string SendCommand(string cmdVal, StringCollection @params)
        {
            return SendCommand("", cmdVal, @params);
        }

        private string SendCommand(string cmdVal)
        {
            return SendCommand("", cmdVal, null);
        }
     
        private string SendCommand(string playerid, string cmdval, StringCollection @params)
        {

            string cmd = cmdval;

            if (playerid != string.Empty)
            {
                cmd = EncodeString(playerid) + " " + cmd;
            }

            if ((@params != null))
            {
                foreach (string str in @params)
                {
                    cmd = cmd + " " + EncodeString(str);

                }
            }

            //add the linefeed
            cmd = cmd + "\n";

            string ReturnVal = string.Empty;

            TcpClient client = default(TcpClient);

            try
            {

                //mHost is the server, mPort the port number
                client = new TcpClient(mHost, mPort);

                byte[] data = System.Text.Encoding.ASCII.GetBytes(cmd);
                NetworkStream stream = client.GetStream();

                // Send the message. 
                stream.Write(data, 0, data.Length);

                // Now read in result into a byte buffer.
                byte[] bytes = null;

                int BytesRead = 0;

                do
                {
                    bytes = new byte[257];
                    BytesRead = stream.Read(bytes, 0, bytes.Length);

                    // Read can return anything from 0 to numBytesToRead.
                    ReturnVal = ReturnVal + System.Text.Encoding.ASCII.GetString(bytes, 0, BytesRead);
                }
                while (stream.DataAvailable);

                ReturnVal = HttpUtility.UrlDecode(ReturnVal);

                // Clean up.
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                Debug.WriteLine("ArgumentNullException: " + e.Message);
            }
            catch (SocketException e)
            {
                Debug.WriteLine("SocketException: " + e.Message);
            }


            return ReturnVal;
        }


    }
}
