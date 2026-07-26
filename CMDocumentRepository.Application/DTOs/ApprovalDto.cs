using CMDocumentRepository.Domain.Enums;

namespace CMDocumentRepository.Application.DTOs;

public record ApprovalDto
{
    public Guid Id { get; init; }
    public Guid DocumentId { get; init; }
    public string DocumentNumber { get; init; } = string.Empty;
    public string DocumentTitle { get; init; } = string.Empty;
    public Guid ApproverId { get; init; }
    public string ApproverName { get; init; } = string.Empty;
    public ApprovalStatus Status { get; init; }
    public string? Comment { get; init; }
    public int OrderNumber { get; init; }
    public DateTime? ApprovedAt { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record SendForApprovalDto
{
    public Guid DocumentId { get; init; }
    public List<Guid> ApproverIds { get; init; } = new();
    public bool IsSequential { get; init; } = true;
}

public record ApproveDocumentDto
{
    public Guid ApprovalId { get; init; }
    public string? Comment { get; init; }
}

public record RejectDocumentDto
{
    public Guid ApprovalId { get; init; }
    public string Comment { get; init; } = string.Empty;
}
