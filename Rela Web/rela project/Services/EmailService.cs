using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Net.Mail;
using System.Net;
using System.Configuration;
using System.Runtime.Remoting.Messaging;
using System.Net.Mime;

namespace Rela_project.Services
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            #region formatter
            string text = string.Format("Please click on this link to {0}: {1}", message.Subject, message.Body);
            string html = "Please confirm your account by clicking this link: <a href=\"" + message.Body + "\">link</a><br/>";
            #endregion
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(ConfigurationManager.AppSettings["mailAccount"], "Rela project");
            msg.To.Add(new MailAddress(message.Destination));
            msg.Subject = message.Subject;
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text,null, MediaTypeNames.Text.Html));
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", Convert.ToInt32(587));
            System.Net.NetworkCredential credential = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["mailAccount"],ConfigurationManager.AppSettings["mailPassword"]);
            smtpClient.Credentials = credential;
            smtpClient.EnableSsl = true;
            try
            {
                smtpClient.SendMailAsync(msg);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                return Task.FromResult(0);
            }
        }
    }
}