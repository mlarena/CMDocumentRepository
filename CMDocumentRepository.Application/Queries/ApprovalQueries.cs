using CMDocumentRepository.Application.DTOs;
using MediatR;

namespace CMDocumentRepository.Application.Queries;

public record GetApprovalsByDocumentQuery : IRequest<List<ApprovalDto>>
{
    public Guid DocumentId { get; init; }
}

public record GetPendingApprovalsQuery : IRequest<List<ApprovalDto>>
{
    public Guid ApproverId { get; init; }
}

public record GetMyApprovalsQuery : IRequest<List<ApprovalDto>>
{
    public Guid ApproverId { get; init; }
}

public record GetAvailableApproversQuery : IRequest<List<UserDto>>
{
}
