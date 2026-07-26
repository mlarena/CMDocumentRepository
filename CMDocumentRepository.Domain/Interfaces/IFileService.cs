namespace CMDocumentRepository.Domain.Interfaces;

public interface IFileService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, Guid documentId);
    Task<string> SaveFileVersionAsync(Stream fileStream, string fileName, Guid documentId, decimal versionNumber);
    Task DeleteFileAsync(string filePath);
    Task<Stream> GetFileStreamAsync(string filePath);
    Task<bool> FileExistsAsync(string filePath);
    string SanitizeFileName(string fileName);
    bool IsAllowedExtension(string extension);
}
