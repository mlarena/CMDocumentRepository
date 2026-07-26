namespace CMDocumentRepository.Domain.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendApprovalNotificationAsync(string approverEmail, string documentNumber, string documentTitle, string actionUrl);
    Task SendApprovalResultAsync(string authorEmail, string documentNumber, string documentTitle, string status, string? comment);
}
