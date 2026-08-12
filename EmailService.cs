using System.Net;
using System.Net.Mail;

namespace ProjectManagementAPI.Services
{
    public interface IEmailService
    {
        Task SendOtpEmail(string toEmail, string otpCode, string purpose);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendOtpEmail(string toEmail, string otpCode, string purpose)
        {
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderPassword = _configuration["EmailSettings:SenderPassword"];
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);

            var subject = purpose == "Registration"
                ? "Verify your ProjectFlow account"
                : "Reset your ProjectFlow password";

            var body = $@"
                <h2>Your verification code</h2>
                <p>Use the code below to continue. This code expires in 5 minutes.</p>
                <h1 style='letter-spacing: 5px;'>{otpCode}</h1>
                <p>If you didn't request this, you can safely ignore this email.</p>";

            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true
            };

            var message = new MailMessage(senderEmail, toEmail, subject, body)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(message);
        }
    }
}