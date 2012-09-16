using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.AddIn;
using OpenSourceAutomation;
using Tamir.SharpSsh;

namespace OSAE.SSH
{
    [AddIn("SSH", Version="0.3.1")]
    public class SSH : IOpenSourceAutomationAddIn
    {
        OSAE osae = new OSAE("SSH");
        string pName;
        string server = "";
        string username = "";
        string password = "";

        public void ProcessCommand(System.Data.DataTable table)
        {
            System.Data.DataRow row = table.Rows[0];
            //process command
            try
            {
                string[] tmp = row["parameter_1"].ToString().Split('/');
                server = tmp[0];
                username = tmp[1];
                password = tmp[2];
                string command = row["parameter_2"].ToString();
                osae.AddToLog("Sending command: " + command + " | " + server + " | " + username + " | " + password, false);
                SshExec ssh = new SshExec(server, username, password);
                ssh.Connect();
                
                string response = ssh.RunCommand(command);
                osae.AddToLog("Response: " + response, false);
                ssh.Close();
            }
            catch (Exception ex)
            {
                osae.AddToLog("Error Sending command - " + ex.Message + " -" + ex.InnerException, true);
            }
        }

        public void RunInterface(string pluginName)
        {
            pName = pluginName;
        }

        public void Shutdown()
        {
            
        }
    }
}
