namespace OSAE.Email
{
    using System;
    using System.Net;
    using System.Net.Mail;

    public class Email : OSAEPluginBase
    {
        string gAppName;

        //OSAELog
        private OSAE.General.OSAELog Log = new General.OSAELog();
        
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
                if (prop != null)
                {
                    to = prop.Value;
                }

                if (to == string.Empty)
                {
                    to = method.Parameter1;
                }

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
                if (parameter2.Equals(string.Empty))
                {
                    throw new ArgumentOutOfRangeException("Message body missing.");
                }

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
                {
                    smtpClient.EnableSsl = true;
                }
                else
                {
                    smtpClient.EnableSsl = false;
                }

                smtpClient.Timeout = 10000;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Username").Value, OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Password").Value);
                
                this.Log.Info("to: " + mailMsg.To);
                this.Log.Info("from: " + mailMsg.From);
                this.Log.Info("subject: " + mailMsg.Subject);
                this.Log.Info("body: " + mailMsg.Body);
                this.Log.Info("smtpServer: " + OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "SMTP Server").Value);
                this.Log.Info("smtpPort: " + OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "SMTP Port").Value);
                this.Log.Info("username: " + OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Username").Value);
                this.Log.Info("password: " + OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Password").Value);
                this.Log.Info("ssl: " + OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "ssl").Value);

                smtpClient.Send(mailMsg);
            }
            catch (Exception ex)
            {
                Log.Error("Error Sending email" , ex);
            }
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
            this.Log.Info("*** Email Plugin is Starting ***");
            gAppName = pluginName;
            if (OSAEObjectManager.ObjectExists(gAppName))
                Log.Info("Found Email Plugin's Object (" + gAppName + ")");
            else
                Log.Info("Could Not Find Email Plugin's Object!!! (" + gAppName + ")");
        }
    }
}
