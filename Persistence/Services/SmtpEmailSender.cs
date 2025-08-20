using Microsoft.Extensions.Options;
using StajTakipUygulaması.Application.Interfaces;
using StajTakipUygulaması.Models;
using System.Net;
using System.Net.Mail;

namespace StajTakipUygulaması.Infrastructure.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly EmailOptions _opt;
        public SmtpEmailSender(IOptions<EmailOptions> opt) => _opt = opt.Value;

        public async Task SendAsync(string to, string subject, string bodyHtml)
        {
            if (string.IsNullOrWhiteSpace(to)) return;

            using var msg = new MailMessage
            {
                From = new MailAddress(_opt.FromAddress, _opt.FromName),
                Subject = subject,
                Body = bodyHtml,
                IsBodyHtml = true
            };
            msg.To.Add(to);

            using var smtp = new SmtpClient(_opt.Host, _opt.Port)
            {
                EnableSsl = _opt.EnableSsl,
                UseDefaultCredentials = _opt.UseDefaultCredentials,
                Credentials = _opt.UseDefaultCredentials
                    ? CredentialCache.DefaultNetworkCredentials
                    : new NetworkCredential(_opt.UserName, _opt.Password)
            };

            await smtp.SendMailAsync(msg);
        }
    }
}
