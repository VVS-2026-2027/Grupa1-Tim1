using Microsoft.AspNetCore.Identity.UI.Services;

namespace OCULIS.Services.Email
{
    public class EmailService : IEmailSender
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _logger.LogInformation("Email poslan na {Email}. Naslov: {Subject}. Sadržaj: {Message}",
                email, subject, htmlMessage);
            return Task.CompletedTask;
        }
    }
}
