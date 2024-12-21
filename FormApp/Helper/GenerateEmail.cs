using Formix.Services.Interfaces;

namespace Formix.Helper
{
    public class GenerateEmail
    {
        private readonly ISendGridEmail _sendGridEmail;

        public GenerateEmail(ISendGridEmail sendGridEmail)
        {
            _sendGridEmail = sendGridEmail;
        }
        public async Task<string> CodeAsync(string email)
        {
            var rand = new Random();
            var code = rand.Next(100000, 999999).ToString();
            await _sendGridEmail.SendEmailAsync(email, "Email Confirmation",
                $"Dear user,</br></br>" +
            $"Please enter the following verification code into the <strong>FormiX</strong> application to continue:</br>" +
            $"<h2 style='color: #6A0DAD;'>{code}</h2></br>" +
            $"If you did not request this code, please ignore this message.");
            return code;
        }
        public async Task LinkAsync(string email, string callbackUrl)
        {
            await _sendGridEmail.SendEmailAsync(email, "Reset Email Confirmation",
            $"Dear user,</br></br>" +
            $"We received a request to reset your password for the <strong>FormiX</strong> application.</br>" +
            $"You can reset your password by clicking on the link below:</br></br>" +
            $"<a href=\"{callbackUrl}\" style=\"color: #6A0DAD; text-decoration: none; font-weight: bold;\">Reset Password</a></br></br>" +
            $"If you did not request a password reset, please ignore this message or contact support for assistance.");
        }
    }
}
