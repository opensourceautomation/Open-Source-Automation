namespace OSAE.Email
{
    using System;
    using System.Net;
    using System.Net.Mail;

    public class Email : OSAEPluginBase
    {
        string gAppName;

        private OSAE.General.OSAELog Log;// = new General.OSAELog();
        
        public override void ProcessCommand(OSAEMethod method)
        {
            //process command
            try
            {                
                string to = string.Empty;
                string parameter2 = string.Empty;
                string subject = string.Empty;
                string body = string.Empty;
                OSAEObjectProperty prop = OSAEObjectPropertyManager.GetObjectPropertyValue(method.Parameter1, "Email Address");

                if (prop != null) to = prop.Value;
                if (to == string.Empty) to = method.Parameter1;

                // To
                MailMessage mailMsg = new MailMessage();
                mailMsg.To.Add(to);

                // From
                MailAddress mailAddress = new MailAddress(OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "From Address").Value);
                mailMsg.From = mailAddress;

                // Subject and Body
                mailMsg.Subject = "Message from OSAE";
                mailMsg.Body = Common.PatternParse(method.Parameter2);
                parameter2 = Common.PatternParse(method.Parameter2);

                // Make sure there is a body of text.
                if (parameter2.Equals(string.Empty)) throw new ArgumentOutOfRangeException("Message body missing.");

                // See if there is a subject.
                // Opening delimiter in first char is good indication of subject.
                if (parameter2[0] == ':')
                {
                    // Find clossing delimiter
                    int i = parameter2.IndexOf(':', 1);
                    if (i != -1)
                    {
                        subject = parameter2.Substring(1, i - 1);
                        body = parameter2.Substring(i + 1, parameter2.Length - i - 1);
                    }
                }

                if (subject.Equals(string.Empty))
                {
                    mailMsg.Subject = "Message from OSAE";
                    mailMsg.Body = parameter2;
                }
                else
                {
                    mailMsg.Subject = subject;
                    mailMsg.Body = body;
                }              

                // Init SmtpClient and send
                SmtpClient smtpClient = new SmtpClient(OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "SMTP Server").Value, int.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "SMTP Port").Value));
                if (OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "ssl").Value == "TRUE")
                    smtpClient.EnableSsl = true;
                else
                    smtpClient.EnableSsl = false;

                smtpClient.Timeout = 20000;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Username").Value, OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Password").Value);
                
                Log.Info("to: " + mailMsg.To);
                Log.Info("from: " + mailMsg.From);
                Log.Info("subject: " + mailMsg.Subject);
                Log.Info("body: " + mailMsg.Body);
                Log.Info("smtpServer: " + OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "SMTP Server").Value);
                Log.Info("smtpPort: " + OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "SMTP Port").Value);
                Log.Info("username: " + OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Username").Value);
                Log.Info("password: " + OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Password").Value);
                Log.Info("ssl: " + OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "ssl").Value);

                smtpClient.Send(mailMsg);
            }
            catch (Exception ex)
            { Log.Error("Error Sending email" , ex); }
        }

        /// <summary>
        /// Interface implementation, this plugin does not perform any actions on shutdown
        /// </summary>
        public override void Shutdown()
        {
            Log.Info("*** Shutting Down ***");
        }

        public override void RunInterface(string pluginName)
        {
            gAppName = pluginName;
            Log = new General.OSAELog(gAppName);

            Log.Info("Email Plugin is Starting...");

            OwnTypes();
        }

        public void OwnTypes()
        {
            //Added the follow to automatically own Speech Base types that have no owner.
            OSAEObjectType oType = OSAEObjectTypeManager.ObjectTypeLoad("EMAIL");

            if (oType.OwnedBy == "")
            {
                OSAEObjectTypeManager.ObjectTypeUpdate(oType.Name, oType.Name, oType.Description, gAppName, oType.BaseType, oType.Owner, oType.SysType, oType.Container, oType.HideRedundant);
                Log.Info("Email Plugin took ownership of the EMAIL Object Type.");
            }
            else
                Log.Info("EMail Plugin correctly owns the EMAIL Object Type.");
        }
    }
}
