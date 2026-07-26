using CMDocumentRepository.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CMDocumentRepository.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly string _uploadPath;
    private readonly long _maxFileSize;
    private readonly string[] _allowedExtensions;
    private readonly string[] _blockedExtensions;

    public FileService(IConfiguration configuration)
    {
        _uploadPath = configuration["FileStorage:UploadPath"] ?? "uploads";
        _maxFileSize = long.Parse(configuration["FileStorage:MaxFileSizeMB"] ?? "50") * 1024 * 1024;
        _allowedExtensions = configuration.GetSection("FileStorage:AllowedExtensions").Get<string[]>()
            ?? new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".jpg", ".png" };
        _blockedExtensions = configuration.GetSection("FileStorage:BlockedExtensions").Get<string[]>()
            ?? new[] { ".exe", ".bat", ".cmd", ".com", ".ps1", ".vbs", ".js", ".jar", ".app", ".dmg", ".msi", ".sh" };

        if (!Directory.Exists(_uploadPath))
            Directory.CreateDirectory(_uploadPath);
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, Guid documentId)
    {
        var sanitized = SanitizeFileName(fileName);
        var extension = Path.GetExtension(sanitized).ToLowerInvariant();
        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(sanitized);
        var uniqueName = $"{documentId}_{sanitized}";

        var filePath = Path.Combine(_uploadPath, uniqueName);
        using var fileStreamOutput = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(fileStreamOutput);

        return filePath;
    }

    public async Task<string> SaveFileVersionAsync(Stream fileStream, string fileName, Guid documentId, decimal versionNumber)
    {
        var sanitized = SanitizeFileName(fileName);
        var versionDir = Path.Combine(_uploadPath, $"{documentId}.history");

        if (!Directory.Exists(versionDir))
            Directory.CreateDirectory(versionDir);

        var versionFileName = $"v{versionNumber.ToString("F1").Replace(".", "_")}_{sanitized}";
        var filePath = Path.Combine(versionDir, versionFileName);

        using var fileStreamOutput = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(fileStreamOutput);

        return filePath;
    }

    public Task DeleteFileAsync(string filePath)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);
        return Task.CompletedTask;
    }

    public Task<Stream> GetFileStreamAsync(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Файл не найден", filePath);

        Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return Task.FromResult(stream);
    }

    public Task<bool> FileExistsAsync(string filePath)
    {
        return Task.FromResult(File.Exists(filePath));
    }

    public string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(fileName.Where(c => !invalidChars.Contains(c)).ToArray());
        sanitized = sanitized.Replace(' ', '_');
        return sanitized;
    }

    public bool IsAllowedExtension(string extension)
    {
        extension = extension.ToLowerInvariant();
        if (_blockedExtensions.Contains(extension))
            return false;
        return _allowedExtensions.Contains(extension);
    }
}
