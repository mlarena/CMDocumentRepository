using CMDocumentRepository.Domain.Common;

namespace CMDocumentRepository.Domain.Entities;

public class DocumentPermission : BaseEntity
{
    public Guid DocumentId { get; set; }
    public Guid UserId { get; set; }
    public bool CanRead { get; set; } = true;
    public bool CanEdit { get; set; }
    public bool CanApprove { get; set; }
    public bool CanDelete { get; set; }

    public Document Document { get; set; } = null!;
    public User User { get; set; } = null!;
}
