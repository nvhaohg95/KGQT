using Humanizer;
using KGQT.Business;
using KGQT.Models.temp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;
using Serilog;
using System.Net.Mail;
using ILogger = Serilog.ILogger;
namespace KGQT.Commons
{
    public class MailService
    {
        private static readonly ILogger _log = Log.ForContext(typeof(AccountBusiness));
        public MailService() { }    

        public static async Task<bool> SendMailAsync(MailSettings mailSettings,string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.Sender = new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail);
            email.From.Add(new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = body;
            email.Body = builder.ToMessageBody();

            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    smtp.Connect(mailSettings.Host, mailSettings.Port, SecureSocketOptions.StartTls);
                    smtp.Authenticate(mailSettings.Mail, mailSettings.Password);
                    await smtp.SendAsync(email);
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error("Lỗi gửi mail", ex.Message);
                    smtp.Disconnect(true);
                    return false;
                }
            }
        }
    }
}
