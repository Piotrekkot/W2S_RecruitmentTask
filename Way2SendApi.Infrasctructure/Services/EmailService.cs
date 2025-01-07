using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Way2SendApi.Infrastructure.Services.Interfaces;
using Way2SendApi.Infrastructure.Settings;

namespace Way2SendApi.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly SMTP _SMTP;

        public EmailService()
        {

        }

        public EmailService(IOptions<SMTP> SMTP)
        {
            _SMTP = SMTP.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_SMTP.UserName));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };
            email.Body = builder.ToMessageBody();

            using (var smtp = new SmtpClient())
            {
                await smtp.ConnectAsync(_SMTP.Host, _SMTP.Port, _SMTP.EnableSsl);
                await smtp.AuthenticateAsync(_SMTP.UserName, _SMTP.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
        }
    }
}
