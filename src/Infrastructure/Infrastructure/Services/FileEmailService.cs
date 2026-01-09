using Application.Interfaces.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class FileEmailService : IEmailService
    {
        private readonly ILogger<FileEmailService> _logger;
        private readonly string _emailDirectory;

        public FileEmailService(ILogger<FileEmailService> logger)
        {
            _logger = logger;
            // Use temp directory for cross-platform compatibility
            _emailDirectory = Path.Combine(Path.GetTempPath(), "SystemRezerwacji", "emails");
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
            _logger.LogInformation("Email sent to {To}, saved at {FilePath}", to, filePath);
        }
    }
}
