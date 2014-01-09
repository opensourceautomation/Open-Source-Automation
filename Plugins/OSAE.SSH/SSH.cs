using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tamir.SharpSsh;

namespace OSAE.SSH
{
    public class SSH : OSAEPluginBase
    {
        //OSAELog
        private OSAE.General.OSAELog Log = new General.OSAELog("SSH");
        
        string pName;
        string server = "";
        string username = "";
        string password = "";

        public override void ProcessCommand(OSAEMethod method)
        {
            try
            {
                string[] tmp = method.Parameter1.Split('/');
                server = tmp[0];
                username = tmp[1];
                password = tmp[2];
                string command = method.Parameter2;
                this.Log.Debug("Sending command: " + command + " | " + server + " | " + username + " | " + password);
                SshExec ssh = new SshExec(server, username, password);
                ssh.Connect();
                
                string response = ssh.RunCommand(command);
                this.Log.Debug("Response: " + response);
                ssh.Close();
            }
            catch (Exception ex)
            {
                this.Log.Error("Error Sending command", ex);
            }
        }

        public override void RunInterface(string pluginName)
        {
            pName = pluginName;
        }

        public override void Shutdown()
        {
            
        }
    }
}
