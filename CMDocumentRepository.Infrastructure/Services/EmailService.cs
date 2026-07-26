using CMDocumentRepository.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CMDocumentRepository.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body)
    {
        _logger.LogInformation("Email отправлен: {To}, Тема: {Subject}", to, subject);
        return Task.CompletedTask;
    }

    public Task SendApprovalNotificationAsync(string approverEmail, string documentNumber, string documentTitle, string actionUrl)
    {
        _logger.LogInformation(
            "Уведомление о согласовании: {Email}, Документ: {Number} - {Title}",
            approverEmail, documentNumber, documentTitle);
        return Task.CompletedTask;
    }

    public Task SendApprovalResultAsync(string authorEmail, string documentNumber, string documentTitle, string status, string? comment)
    {
        _logger.LogInformation(
            "Результат согласования: {Email}, Документ: {Number} - {Title}, Статус: {Status}",
            authorEmail, documentNumber, documentTitle, status);
        return Task.CompletedTask;
    }
}
