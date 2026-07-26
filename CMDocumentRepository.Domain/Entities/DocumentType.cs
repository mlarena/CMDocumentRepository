using CMDocumentRepository.Domain.Common;

namespace CMDocumentRepository.Domain.Entities;

public class DocumentType : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystem { get; set; }
}
