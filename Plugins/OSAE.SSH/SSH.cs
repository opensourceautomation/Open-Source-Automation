using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tamir.SharpSsh;

namespace OSAE.SSH
{
    public class SSH : OSAEPluginBase
    {
        Logging logging = Logging.GetLogger("SSH");
        
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
                logging.AddToLog("Sending command: " + command + " | " + server + " | " + username + " | " + password, false);
                SshExec ssh = new SshExec(server, username, password);
                ssh.Connect();
                
                string response = ssh.RunCommand(command);
                logging.AddToLog("Response: " + response, false);
                ssh.Close();
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error Sending command - " + ex.Message + " -" + ex.InnerException, true);
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
