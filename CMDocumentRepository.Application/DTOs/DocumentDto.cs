using CMDocumentRepository.Domain.Enums;

namespace CMDocumentRepository.Application.DTOs;

public record DocumentDto
{
    public Guid Id { get; init; }
    public string DocumentNumber { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public Guid CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public Guid DocumentTypeId { get; init; }
    public string DocumentTypeName { get; init; } = string.Empty;
    public decimal Version { get; init; }
    public DocumentStatus Status { get; init; }
    public Guid CreatedBy { get; init; }
    public string CreatorName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public Guid? ApprovedBy { get; init; }
    public string? ApprovedByName { get; init; }
    public DateTime? ApprovedAt { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidUntil { get; init; }
    public string FilePath { get; init; } = string.Empty;
    public long FileSize { get; init; }
    public string FileExtension { get; init; } = string.Empty;
    public bool IsDeleted { get; init; }
    public DateTime? DeletedAt { get; init; }
}

public record CreateDocumentDto
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public Guid CategoryId { get; init; }
    public Guid DocumentTypeId { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidUntil { get; init; }
    public Stream? File { get; init; }
    public string? FileName { get; init; }
}

public record UpdateDocumentDto
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public Guid CategoryId { get; init; }
    public Guid DocumentTypeId { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidUntil { get; init; }
    public Stream? File { get; init; }
    public string? FileName { get; init; }
    public string? ChangeComment { get; init; }
}

public record DocumentVersionDto
{
    public Guid Id { get; init; }
    public Guid DocumentId { get; init; }
    public decimal VersionNumber { get; init; }
    public string FilePath { get; init; } = string.Empty;
    public long FileSize { get; init; }
    public Guid CreatedBy { get; init; }
    public string CreatorName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public string? ChangeComment { get; init; }
}
