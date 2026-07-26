using CMDocumentRepository.Application.DTOs;
using MediatR;

namespace CMDocumentRepository.Application.Queries;

public record GetAuditLogsQuery : IRequest<List<AuditLogDto>>
{
    public Guid? UserId { get; init; }
    public string? EntityType { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public int PageSize { get; init; } = 100;
}
