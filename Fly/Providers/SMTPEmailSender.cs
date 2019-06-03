using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Web;


namespace Fly.Providers
{
    public class SMTPEmailSender
    {
        SmtpClient client;
        //public log4net.ILog logger;
        Queue<MailMessage> _messages = new Queue<MailMessage>();
        public string AdminEmail = "";

        public SMTPEmailSender()
        {
            //logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            client = new SmtpClient("smtp.gmail.com", 587);
            
            //client.Credentials = new System.Net.NetworkCredential("rabbit201919@gmail.com", "RabbitP@$$w0rd");
            client.Credentials = new System.Net.NetworkCredential("willsuccessmail@gmail.com", "P@$$w0rd");

            AdminEmail = "Rabbit";
            client.EnableSsl = true;
        }
        //public SMTPEmailSender(string hostName, int portNumber, string email, string password)
        //{
        //    //Init Logger
        //    logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //    client = new SmtpClient(hostName, portNumber);
        //    client.Credentials = new System.Net.NetworkCredential(email, password);
        //}

        public void SendInvitationEmail(string from, string displayName, string to, string subject, string body, bool isBodyHtml, string reployTo = "", string isSSL = "", string senderUserName = "", string ccMails = "")
        {
            if (string.IsNullOrEmpty(displayName)) displayName = "Maharatona";
            MailMessage msg = new MailMessage();
            msg.Body = body.Replace("&lt;", "<").Replace("&gt;", ">");//"<div style='direction:rtl;'>" + body.Replace("&lt;", "<").Replace("&gt;", ">") + "<p><strong>مع وافر الاحترام والتقدير</strong></p></div>";
            msg.Subject = subject;
            msg.From = new MailAddress(from, senderUserName);

            msg.To.Add(to);
            if (!string.IsNullOrEmpty(reployTo)) msg.ReplyToList.Add(new MailAddress(reployTo));
            msg.IsBodyHtml = isBodyHtml;
            client.EnableSsl = isSSL == "1" ? true : false;
            if (!string.IsNullOrEmpty(ccMails))
            {
                foreach (string item in ccMails.Split(',').ToArray())
                {
                    msg.CC.Add(new MailAddress(item));
                }
            }
            //   msg.CC.Add(new MailAddress("ahashish@it-blocks.com"));

            Object state = to;

            lock (_messages)
            {
                _messages.Enqueue(msg);
                if (_messages.Count == 1)
                {
                    ThreadPool.QueueUserWorkItem(SendEmailInternal);
                }
            }
        }


        /// <summary>
        /// Send forgetten password to specific email
        /// </summary>
        /// <param name="Body"></param>
        /// <param name="Subject"></param>
        /// <param name="SenderMail"></param>
        /// <param name="Reciver"></param>
        public void SendForgettenPassword(string Body, string Subject, string SenderMail, string Reciver)
        {

            SmtpClient smtpClient = new SmtpClient();
            MailMessage mailMessage = new MailMessage();

            mailMessage.To.Add(Reciver);
            mailMessage.Subject = Subject;
            mailMessage.Body = Body;
            mailMessage.IsBodyHtml = true;

            smtpClient.Send(mailMessage);

        }

        protected virtual void SendEmailInternal(object state)
        {
            while (true)
            {
                MailMessage msg;
                lock (_messages)
                {
                    if (_messages.Count == 0)
                        return;
                    msg = _messages.Dequeue();
                    try
                    {
                        client.Send(msg);

                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        public bool SendEmail(string from, string to, string subject, string body, bool isBodyHtml, string isSSL = "")
        {
            try
            {
                MailMessage msg = new MailMessage();
                msg.Subject = subject;
                //if (!string.IsNullOrEmpty(from))
                //{
                msg.From = new MailAddress("willsuccessmail@gmail.com", "Rabbit");
                //}
                //else
                //{
                //    msg.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["emailAdmin"], "Maharatona");
                //}
                msg.To.Add(to);
                msg.Body = body;
                msg.IsBodyHtml = isBodyHtml;
                //  client.EnableSsl = isSSL == "1" ? true : false;
                //  msg.CC.Add(new MailAddress("ahashish@it-blocks.com"));
                client.Send(msg);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool SendEmail(string from, string to, string cc, string bcc, string subject, string body, bool isBodyHtml)
        {
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(from);
            ConvertFromSemiColonSeparatedEmailsToMailAddressCollection(msg.To, to);
            ConvertFromSemiColonSeparatedEmailsToMailAddressCollection(msg.Bcc, bcc);
            msg.Subject = subject;
            msg.Body = body;
            msg.IsBodyHtml = isBodyHtml;
            try
            {
                client.Send(msg);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static void ConvertFromSemiColonSeparatedEmailsToMailAddressCollection(MailAddressCollection emailsCollection, string emails)
        {
            if (emails == null) return;

            string[] emailsSeparated = emails.Split(',', ';');

            foreach (var email in emailsSeparated)
            {
                if (!string.IsNullOrWhiteSpace(email))
                    emailsCollection.Add(new MailAddress(email.Trim()));
            }
        }
    }
}