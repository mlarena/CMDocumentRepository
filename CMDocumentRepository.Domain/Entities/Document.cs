using System.Text.Json;
using CMDocumentRepository.Domain.Common;
using CMDocumentRepository.Domain.Enums;

namespace CMDocumentRepository.Domain.Entities;

public class Document : BaseEntity
{
    public string DocumentNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public Guid DocumentTypeId { get; set; }
    public decimal Version { get; set; } = 1.0m;
    public DocumentStatus Status { get; set; } = DocumentStatus.Draft;
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FileExtension { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    public JsonDocument? Metadata { get; set; }

    public Category Category { get; set; } = null!;
    public DocumentType DocumentType { get; set; } = null!;
    public User Creator { get; set; } = null!;
    public ICollection<DocumentVersion> Versions { get; set; } = new List<DocumentVersion>();
    public ICollection<Approval> Approvals { get; set; } = new List<Approval>();
    public ICollection<DocumentPermission> Permissions { get; set; } = new List<DocumentPermission>();
}
