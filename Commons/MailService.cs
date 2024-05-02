using Humanizer;
using KGQT.Models.temp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;
using System.Net.Mail;

namespace KGQT.Commons
{
    public class MailService
    {
        public MailService() { }    

        public static async Task<bool> SendMailAsync(MailSettings mailSettings,string to, string subject, string body)
        {
            var result = new DataReturnModel<object>();
            var email = new MimeMessage();
            email.Sender = new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail);
            email.From.Add(new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = body;
            email.Body = builder.ToMessageBody();

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                smtp.Connect(mailSettings.Host, mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(mailSettings.Mail, mailSettings.Password);
                await smtp.SendAsync(email);
                return true;
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Type = 2;
                result.Message = "Gửi mail không thành công!";
            }
            smtp.Disconnect(true);
            return false;

        }
    }
}
