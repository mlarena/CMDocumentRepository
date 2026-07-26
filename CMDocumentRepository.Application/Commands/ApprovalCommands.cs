using CMDocumentRepository.Application.DTOs;
using MediatR;

namespace CMDocumentRepository.Application.Commands;

public record SendForApprovalCommand : IRequest<bool>
{
    public Guid DocumentId { get; init; }
    public List<Guid> ApproverIds { get; init; } = new();
    public bool IsSequential { get; init; } = true;
    public Guid SentBy { get; init; }
}

public record ApproveDocumentCommand : IRequest<bool>
{
    public Guid ApprovalId { get; init; }
    public string? Comment { get; init; }
    public Guid ApproverId { get; init; }
}

public record RejectDocumentCommand : IRequest<bool>
{
    public Guid ApprovalId { get; init; }
    public string Comment { get; init; } = string.Empty;
    public Guid ApproverId { get; init; }
}

public record RequestReworkCommand : IRequest<bool>
{
    public Guid ApprovalId { get; init; }
    public string? Comment { get; init; }
    public Guid ApproverId { get; init; }
}
