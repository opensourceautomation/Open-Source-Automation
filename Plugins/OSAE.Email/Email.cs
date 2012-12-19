using System;
using System.AddIn;
using System.Net;
using System.Net.Mail;
using OpenSourceAutomation;

namespace OSAE.Email
{
    [AddIn("Email", Version = "0.3.8")]
    public class Email : IOpenSourceAutomationAddInv2
    {
        string pName;
        OSAE osae = new OSAE("Email");
        
        public void ProcessCommand(OSAEMethod method)
        {
            //process command
            try
            {                
                string to = string.Empty;
                string parameter2 = string.Empty;
                string subject = string.Empty;
                string body = string.Empty;
                ObjectProperty prop = osae.GetObjectPropertyValue(method.Parameter1, "Email Address");
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
                MailAddress mailAddress = new MailAddress(osae.GetObjectPropertyValue(pName, "From Address").Value);
                mailMsg.From = mailAddress;

                // Subject and Body
                mailMsg.Subject = "Message from OSAE";
                mailMsg.Body = osae.PatternParse(method.Parameter2);
                parameter2 = osae.PatternParse(method.Parameter2);

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
                SmtpClient smtpClient = new SmtpClient(osae.GetObjectPropertyValue(pName, "SMTP Server").Value, Int32.Parse(osae.GetObjectPropertyValue(pName, "SMTP Port").Value));
                if (osae.GetObjectPropertyValue(pName, "ssl").Value == "TRUE")
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
                smtpClient.Credentials = new NetworkCredential(osae.GetObjectPropertyValue(pName, "Username").Value, osae.GetObjectPropertyValue(pName, "Password").Value);
                
                osae.AddToLog("to: " + mailMsg.To, true);
                osae.AddToLog("from: " + mailMsg.From, true);
                osae.AddToLog("subject: " + mailMsg.Subject, true);
                osae.AddToLog("body: " + mailMsg.Body, true);
                osae.AddToLog("smtpServer: " + osae.GetObjectPropertyValue(pName, "SMTP Server").Value, true);
                osae.AddToLog("smtpPort: " + osae.GetObjectPropertyValue(pName, "SMTP Port").Value, true);
                osae.AddToLog("username: " + osae.GetObjectPropertyValue(pName, "Username").Value, true);
                osae.AddToLog("password: " + osae.GetObjectPropertyValue(pName, "Password").Value, true);
                osae.AddToLog("ssl: " + osae.GetObjectPropertyValue(pName, "ssl").Value, true);

                smtpClient.Send(mailMsg);
            }
            catch (Exception ex)
            {
                osae.AddToLog("Error Sending email - " + ex.Message + " -" + ex.InnerException, true);
            }
        }

        public void RunInterface(string pluginName)
        {
            pName = pluginName;
            //No constant processing
        }

        /// <summary>
        /// Interface implementation, this plugin does not perform any actions on shutdown
        /// </summary>
        public void Shutdown()
        {
        }
    }
}
