using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace RelaService
{
    class EmailService
    {
        private string message { get; set; }
        private string email { get; set; }
        private string subject { get; set; }

        public EmailService(string message, string email, string subject)
        {
            this.message = message;
            this.email = email;
            this.subject = subject;
        }

        public void sendMail()
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("Mail", "Rela project");
            mailMessage.To.Add(new MailAddress(email));
            mailMessage.Subject = subject;
            mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(message, null, MediaTypeNames.Text.Plain));
            mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(message, null, MediaTypeNames.Text.Html));
            mailMessage.Priority = MailPriority.High;

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", Convert.ToInt32("587"));
            NetworkCredential networkCredential = new NetworkCredential("Email", "Password");
            smtpClient.Credentials = networkCredential;
            smtpClient.EnableSsl = true;
            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }
        }

    }
}
