using CMDocumentRepository.Domain.Common;

namespace CMDocumentRepository.Domain.Entities;

public class DocumentVersion : BaseEntity
{
    public Guid DocumentId { get; set; }
    public decimal VersionNumber { get; set; }
    public bool IsMajorVersion { get; set; } = false;
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public Guid CreatedBy { get; set; }
    public string? ChangeComment { get; set; }

    public Document Document { get; set; } = null!;
    public User Creator { get; set; } = null!;
}
