using Application.Interfaces.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class FileEmailService : IEmailService
    {
        private readonly ILogger<FileEmailService> _logger;
        // Path to store emails - utilizing the artifacts brain directory as requested
        private readonly string _emailDirectory = "/home/przydan/.gemini/antigravity/brain/emails";

        public FileEmailService(ILogger<FileEmailService> logger)
        {
            _logger = logger;
            if (!Directory.Exists(_emailDirectory))
            {
                Directory.CreateDirectory(_emailDirectory);
            }
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var fileName = $"Email_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid()}.txt";
            var filePath = Path.Combine(_emailDirectory, fileName);
            
            var emailContent = $"To: {to}\nSubject: {subject}\nDate: {DateTime.Now}\n\n{body}";

            await File.WriteAllTextAsync(filePath, emailContent);
            _logger.LogInformation($"Email sent to {to}, saved at {filePath}");
        }
    }
}
