using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using Poppin.Configuration;
using Poppin.Interfaces;
using Poppin.Models.Identity;
using System;

namespace Poppin.Services
{
    public class SmtpService : ISmtpService
    {
        private readonly Office365Settings _appSettings;

        public SmtpService(IOptions<Office365Settings> settings)
        {
            _appSettings = settings.Value;
        }

        public void SendConfirmationEmail(User user, string token)
        {
            var mailMessage = new MimeMessage();
            mailMessage.Sender = MailboxAddress.Parse(_appSettings.Sender);
            mailMessage.To.Add(MailboxAddress.Parse(user.Email));
            mailMessage.Subject = "Confirm Your Poppin Account";
            mailMessage.Body = new TextPart(TextFormat.Html) { Text = "Click here to confirm: <a href=\"https://getpopp.in/account/confirm-email/" + user.Id + "?t=" + token + "\">CONFIRM</a>"};

            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_appSettings.SmtpServerAddress, 587, SecureSocketOptions.StartTls);
                    client.Authenticate(_appSettings.UserName, _appSettings.Password);
                    client.Send(mailMessage);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }

        public void SendPasswordResetEmail(User user, string token)
        {
            var mailMessage = new MimeMessage();
            mailMessage.Sender = MailboxAddress.Parse(_appSettings.Sender);
            mailMessage.To.Add(MailboxAddress.Parse(user.Email));
            mailMessage.Subject = "Reset Your Poppin Password";
            mailMessage.Body = new TextPart(TextFormat.Html) { Text = "Click here to reset your password: <a href=\"https://getpopp.in/account/reset-password/" + user.Id + "?t=" + token + "\">RESET PASSWORD</a>" };

            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_appSettings.SmtpServerAddress, _appSettings.SmtpServerPort, SecureSocketOptions.StartTls);
                    client.Authenticate(_appSettings.UserName, _appSettings.Password);
                    client.Send(mailMessage);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }
    }
}
