using CMDocumentRepository.Domain.Common;
using CMDocumentRepository.Domain.Enums;

namespace CMDocumentRepository.Domain.Entities;

public class Approval : BaseEntity
{
    public Guid DocumentId { get; set; }
    public Guid ApproverId { get; set; }
    public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;
    public string? Comment { get; set; }
    public int OrderNumber { get; set; }
    public DateTime? ApprovedAt { get; set; }

    public Document Document { get; set; } = null!;
    public User Approver { get; set; } = null!;
}
