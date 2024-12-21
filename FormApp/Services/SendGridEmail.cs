using Formix.Helper;
using Formix.Services.Interfaces;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Formix.Services
{
    public class SendGridEmail : ISendGridEmail
    {
        private readonly IOptions<AuthMessageSenderOptions> _options;

        public SendGridEmail(IOptions<AuthMessageSenderOptions> options)
        {
            _options = options;
        }
        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            if(_options.Value.ApiKey != null)
            { 
                await Execute(_options.Value.ApiKey, subject, message, toEmail);
            }
        }
        private async Task Execute(string apiKey, string subject, string message, string toEmail)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage
            {
                From = new EmailAddress("yura.shabunya.work@gmail.com", "Formix"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(toEmail));
            var response = await client.SendEmailAsync(msg);
        }
    }
}
