namespace OSAE.Email
{
    using System;
    using System.Net;
    using System.Net.Mail;
    using System.ComponentModel;

    public class Email : OSAEPluginBase
    {
        string gAppName;
        private OSAE.General.OSAELog Log;

        public override void ProcessCommand(OSAEMethod method)
        {
            //process command
            try
            {
                
                string parameter2 = string.Empty;
                string subject = string.Empty;
                string body = string.Empty;

                // To
                MailAddress to;
                OSAEObjectProperty prop = OSAEObjectPropertyManager.GetObjectPropertyValue(method.Parameter1, "Email Address");
                if (prop != null)
                {
                    if (prop.Value == string.Empty) to = new MailAddress(method.Parameter1);
                    else to = new MailAddress(prop.Value);
                }
                else
                    to = new MailAddress(method.Parameter1);

                // From
                MailAddress from = new MailAddress(OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "From Address").Value);

                MailMessage mailMsg = new MailMessage(from, to);

                // Subject and Body
                mailMsg.Subject = "Message from OSAE";
                mailMsg.Body = Common.PatternParse(method.Parameter2);
                mailMsg.BodyEncoding = System.Text.Encoding.UTF8;

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
                    mailMsg.SubjectEncoding = System.Text.Encoding.UTF8;

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
                Log.Info("smtpServer: " + smtpClient.Host);
                Log.Info("smtpPort: " + smtpClient.Port);
                Log.Info("username: " + OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Username").Value);
                Log.Info("password: " + OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Password").Value);
                Log.Info("ssl: " + smtpClient.EnableSsl);
                smtpClient.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
                string userState = "test message1";
                smtpClient.SendAsync(mailMsg, userState);

                //smtpClient.Send(mailMsg);
            }
            catch (Exception ex)
            { Log.Error("Error Sending email" , ex); }
        }

        public void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            String token = (string)e.UserState;

            if (e.Cancelled) Log.Info(token + "  Send canceled.");

            if (e.Error != null)
                Log.Info(token + " " +e.Error.ToString());
            else
                Log.Info("Message sent.");
        }

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
