﻿using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using Poppin.Configuration;
using Poppin.Interfaces;
using Poppin.Models.Identity;
using System;
using System.Web;

namespace Poppin.Services
{
	public class SmtpService : ISmtpService
    {
        private readonly Office365Settings _appSettings;
        private readonly IHttpContextAccessor httpContextAccessor;

        public SmtpService(IOptions<Office365Settings> settings, IHttpContextAccessor hca)
        {
            _appSettings = settings.Value;
            httpContextAccessor = hca;
        }

        public void SendConfirmationEmail(User user, string token)
        {
            var hostUrl = HostUrl();
            var mailMessage = new MimeMessage();
            var htmlToken = HttpUtility.UrlEncode(token);
            var link = $"{hostUrl}/account/confirm-email/{user.Id}?t={htmlToken}";

            mailMessage.Sender = MailboxAddress.Parse(_appSettings.Sender);
            mailMessage.To.Add(MailboxAddress.Parse(user.Email));
            mailMessage.Subject = "Confirm Your Poppin Account";
            mailMessage.Body = new TextPart(TextFormat.Html) { Text = $"Click here to confirm: <a href=\"{link}\">CONFIRM</a>&nbsp;<br> or enter this URL in the address bar of your browser: {link}" };

            SendEmail(mailMessage);
        }

        public void SendPasswordResetEmail(User user, string token)
        {
            var hostUrl = HostUrl();
            var mailMessage = new MimeMessage();
            var htmlToken = HttpUtility.UrlEncode(token);
            var link = $"{hostUrl}/account/reset-password/{user.Id}?t={htmlToken}";

            mailMessage.Sender = MailboxAddress.Parse(_appSettings.Sender);
            mailMessage.To.Add(MailboxAddress.Parse(user.Email));
            mailMessage.Subject = "Reset Your Poppin Password";
            mailMessage.Body = new TextPart(TextFormat.Html) { Text = $"Click here to reset your password: <a href=\"{link}\">RESET PASSWORD</a> or enter this URL in the address bar of your browser: {link}. If you did not request this action, ignore this message." };

            SendEmail(mailMessage);
        }

        public void SendPasswordConfirmationEmail(User user)
        {
            var hostUrl = HostUrl();
            var mailMessage = new MimeMessage();

            mailMessage.Sender = MailboxAddress.Parse(_appSettings.Sender);
            mailMessage.To.Add(MailboxAddress.Parse(user.Email));
            mailMessage.Subject = "Reset Your Poppin Password";
            mailMessage.Body = new TextPart(TextFormat.Html) { Text = $"Your password was recently updated. If you did not request this action, you should update your Poppin password immediately. <a href=\"{hostUrl}/account/forgot-password\">CLICK HERE</a> or go to {hostUrl}/account/forgot-password" };

            SendEmail(mailMessage);
        }

        private void SendEmail(MimeMessage mailMessage)
        {
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

        private string HostUrl()
								{
            var request = httpContextAccessor.HttpContext.Request;
            return request.Scheme + "://" + request.Host.Value;
								}
    }
}
