using CMDocumentRepository.Domain.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace CMDocumentRepository.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly string _host;
    private readonly int _port;
    private readonly bool _useSsl;
    private readonly string _userName;
    private readonly string _password;
    private readonly string _fromName;
    private readonly string _fromAddress;

    public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _host = configuration["Smtp:Host"] ?? "localhost";
        _port = int.Parse(configuration["Smtp:Port"] ?? "25");
        _useSsl = bool.Parse(configuration["Smtp:UseSsl"] ?? "false");
        _userName = configuration["Smtp:UserName"] ?? "";
        _password = configuration["Smtp:Password"] ?? "";
        _fromName = configuration["Smtp:FromName"] ?? "CMDocumentRepository";
        _fromAddress = configuration["Smtp:FromAddress"] ?? "noreply@cmdocument.local";
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_fromName, _fromAddress));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync(_host, _port,
                _useSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.Auto);

            if (!string.IsNullOrEmpty(_userName))
                await client.AuthenticateAsync(_userName, _password);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email отправлен: {To}, Тема: {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка отправки email: {To}. Сообщение будет обработано.", to);
        }
    }

    public async Task SendApprovalNotificationAsync(string approverEmail, string documentNumber, string documentTitle, string actionUrl)
    {
        var subject = string.Format("[Согласование] Документ {0}", documentNumber);
        var htmlLink = string.Format("<a href=\"{0}\">Открыть для согласования</a>", actionUrl);
        var body = string.Format(
            "<html><body>" +
            "<h2>Требуется согласование документа</h2>" +
            "<p><b>Номер:</b> {0}</p>" +
            "<p><b>Название:</b> {1}</p>" +
            "<p><b>Действие:</b> {2}</p>" +
            "</body></html>",
            documentNumber, documentTitle, htmlLink);

        await SendEmailAsync(approverEmail, subject, body);
    }

    public async Task SendApprovalResultAsync(string authorEmail, string documentNumber, string documentTitle, string status, string? comment)
    {
        var subject = string.Format("[Результат] Документ {0} - {1}", documentNumber, status);
        var commentBlock = !string.IsNullOrEmpty(comment)
            ? string.Format("<p><b>Комментарий:</b> {0}</p>", comment)
            : "";
        var body = string.Format(
            "<html><body>" +
            "<h2>Результат согласования документа</h2>" +
            "<p><b>Номер:</b> {0}</p>" +
            "<p><b>Название:</b> {1}</p>" +
            "<p><b>Статус:</b> {2}</p>" +
            "{3}" +
            "</body></html>",
            documentNumber, documentTitle, status, commentBlock);

        await SendEmailAsync(authorEmail, subject, body);
    }

    public async Task SendDocumentExpiryNotificationAsync(string recipientEmail, string documentNumber, string documentTitle, DateTime expiryDate)
    {
        var daysLeft = (expiryDate.Date - DateTime.Now.Date).Days;
        string urgency;
        if (daysLeft <= 1)
            urgency = "\u26a0\ufe0f Срочно";
        else if (daysLeft <= 3)
            urgency = "\u26a0 Внимание";
        else
            urgency = "\u2139\ufe0f Информация";

        var subject = string.Format("{0} Срок действия документа {1} истекает", urgency, documentNumber);
        var body = string.Format(
            "<html><body>" +
            "<h2>Уведомление о сроке действия документа</h2>" +
            "<p><b>Номер:</b> {0}</p>" +
            "<p><b>Название:</b> {1}</p>" +
            "<p><b>Срок действия до:</b> {2:dd.MM.yyyy}</p>" +
            "<p><b>Осталось дней:</b> {3}</p>" +
            "</body></html>",
            documentNumber, documentTitle, expiryDate, daysLeft);

        await SendEmailAsync(recipientEmail, subject, body);
    }
}