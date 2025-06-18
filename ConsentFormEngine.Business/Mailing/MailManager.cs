using Microsoft.Extensions.Configuration;
using ConsentFormEngine.Core.Mailing;
using System.Net;
using System.Net.Mail;

namespace ConsentFormEngine.Business.Mailing
{
    public class MailManager : IMailService
    {
        private readonly MailSettings _mailSettings;
        public MailManager(IConfiguration configuration)
        {
            const string configurationSection = "MailSettings";
            _mailSettings =
                configuration.GetSection(configurationSection).Get<MailSettings>()
                ?? throw new NullReferenceException($"\"{configurationSection}\" section cannot found in configuration.");
        }

        public void SendMail(Mail mail)
        {
            try
            {
                using MailMessage mm = new();
                mm.From = new System.Net.Mail.MailAddress(".com", "Bilgilendirme");
                mm.Subject = mail.Subject;
                mm.Body = string.IsNullOrEmpty(mail.TextBody) ? mail.HtmlBody.TrimEnd() : mail.TextBody.TrimEnd();

                mm.IsBodyHtml = true;
                mm.To.Clear();
                mm.Bcc.Clear();
                mm.CC.Clear();

                if (string.IsNullOrEmpty(mail.ToList) == false)
                {
                    if (mail.ToList.Contains(";"))
                    {
                        foreach (string extraMail in mail.ToList.Split(';'))
                        {
                            mm.To.Add(extraMail);
                        }
                    }
                    else
                    {
                        mm.To.Add(mail.ToList);
                    }
                }

                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient
                {
                    EnableSsl = false
                };


                smtp.Host = _mailSettings.Server;
                NetworkCredential NetworkCred = new NetworkCredential(_mailSettings.UserName, _mailSettings.Password);
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = Convert.ToInt32(_mailSettings.Port);


                if (!string.IsNullOrEmpty(mail.Attachments))
                {
                    foreach (string attachmentPath in mail.Attachments.Split(';'))
                    {
                        if (File.Exists(attachmentPath))
                        {
                            mm.Attachments.Add(new Attachment(attachmentPath));
                        }
                    }
                }
                smtp.Timeout = 5 * 60 * 1000;

                smtp.Send(mm);
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task SendEmailAsync(Mail mail)
        {
            using MailMessage mm = new();
            mm.From = new System.Net.Mail.MailAddress(".com", "");
            mm.Subject = mail.Subject;
            mm.Body = string.IsNullOrEmpty(mail.TextBody) ? mail.HtmlBody.TrimEnd() : mail.TextBody.TrimEnd();
            mm.IsBodyHtml = true;
            mm.To.Clear();
            mm.Bcc.Clear();
            mm.CC.Clear();

            if (mail.ToList.Contains(";"))
            {
                foreach (string extraMail in mail.ToList.Split(';'))
                {
                    mm.To.Add(extraMail);
                }
            }

            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient
            {
                EnableSsl = false
            };

            smtp.Host = _mailSettings.Server;
            NetworkCredential NetworkCred = new NetworkCredential(_mailSettings.UserName, _mailSettings.Password);
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = NetworkCred;
            smtp.Port = Convert.ToInt32(_mailSettings.Port);

            if (!string.IsNullOrEmpty(mail.Attachments))
            {
                foreach (string attachmentPath in mail.Attachments.Split(';'))
                {
                    if (File.Exists(attachmentPath))
                    {
                        mm.Attachments.Add(new Attachment(attachmentPath));
                    }
                }
            }
            smtp.Timeout = 60 * 1 * 1000;

            await smtp.SendMailAsync(mm);
        }
    }
}
