namespace OSAE.Email
{
    using System;
    using System.Net;
    using System.Net.Mail;

    public class Email : OSAEPluginBase
    {
        string pName;

        /// <summary>
        /// Provides access to logging
        /// </summary>
        Logging logging = Logging.GetLogger("Email");
        
        public override void ProcessCommand(OSAEMethod method)
        {
            //process command
            try
            {                
                string to = string.Empty;
                string parameter2 = string.Empty;
                string subject = string.Empty;
                string body = string.Empty;
                ObjectProperty prop = ObjectPopertiesManager.GetObjectPropertyValue(method.Parameter1, "Email Address");
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
                MailAddress mailAddress = new MailAddress(ObjectPopertiesManager.GetObjectPropertyValue(pName, "From Address").Value);
                mailMsg.From = mailAddress;

                // Subject and Body
                mailMsg.Subject = "Message from OSAE";
                mailMsg.Body = Common.PatternParse(method.Parameter2);
                parameter2 = Common.PatternParse(method.Parameter2);

                // Make sure there is a body of text.
                if (parameter2.Equals(""))
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

                if (subject.Equals(""))
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
                SmtpClient smtpClient = new SmtpClient(ObjectPopertiesManager.GetObjectPropertyValue(pName, "SMTP Server").Value, Int32.Parse(ObjectPopertiesManager.GetObjectPropertyValue(pName, "SMTP Port").Value));
                if (ObjectPopertiesManager.GetObjectPropertyValue(pName, "ssl").Value == "TRUE")
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
                smtpClient.Credentials = new NetworkCredential(ObjectPopertiesManager.GetObjectPropertyValue(pName, "Username").Value, ObjectPopertiesManager.GetObjectPropertyValue(pName, "Password").Value);
                
                logging.AddToLog("to: " + mailMsg.To, true);
                logging.AddToLog("from: " + mailMsg.From, true);
                logging.AddToLog("subject: " + mailMsg.Subject, true);
                logging.AddToLog("body: " + mailMsg.Body, true);
                logging.AddToLog("smtpServer: " + ObjectPopertiesManager.GetObjectPropertyValue(pName, "SMTP Server").Value, true);
                logging.AddToLog("smtpPort: " + ObjectPopertiesManager.GetObjectPropertyValue(pName, "SMTP Port").Value, true);
                logging.AddToLog("username: " + ObjectPopertiesManager.GetObjectPropertyValue(pName, "Username").Value, true);
                logging.AddToLog("password: " + ObjectPopertiesManager.GetObjectPropertyValue(pName, "Password").Value, true);
                logging.AddToLog("ssl: " + ObjectPopertiesManager.GetObjectPropertyValue(pName, "ssl").Value, true);

                smtpClient.Send(mailMsg);
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error Sending email - " + ex.Message + " -" + ex.InnerException, true);
            }
        }

        /// <summary>
        /// Interface implementation, this plugin does not perform any actions on shutdown
        /// </summary>
        public override void Shutdown()
        {
            
        }


        public override void RunInterface(string pluginName)
        {
            logging.AddToLog("Starting...", true);
            pName = pluginName;
            //No constant processing
        }
    }
}
