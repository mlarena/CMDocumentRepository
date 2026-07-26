using CMDocumentRepository.Application.DTOs;
using CMDocumentRepository.Domain.Enums;
using MediatR;

namespace CMDocumentRepository.Application.Queries;

public record GetDocumentByIdQuery : IRequest<DocumentDto?>
{
    public Guid Id { get; init; }
}

public record GetDocumentByNumberQuery : IRequest<DocumentDto?>
{
    public string DocumentNumber { get; init; } = string.Empty;
}

public record GetAllDocumentsQuery : IRequest<List<DocumentDto>>
{
    public DocumentStatus? Status { get; init; }
    public Guid? CategoryId { get; init; }
    public Guid? DocumentTypeId { get; init; }
    public Guid? CreatedBy { get; init; }
}

public record SearchDocumentsQuery : IRequest<List<DocumentDto>>
{
    public string? Keyword { get; init; }
    public DocumentStatus? Status { get; init; }
    public Guid? CategoryId { get; init; }
    public Guid? DocumentTypeId { get; init; }
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
}

public record GetDocumentVersionsQuery : IRequest<List<DocumentVersionDto>>
{
    public Guid DocumentId { get; init; }
}

public record GetMyDocumentsQuery : IRequest<List<DocumentDto>>
{
    public Guid UserId { get; init; }
}

public record GetDocumentsForApprovalQuery : IRequest<List<DocumentDto>>
{
    public Guid UserId { get; init; }
}

public record GetDeletedDocumentsQuery : IRequest<List<DocumentDto>>
{
    public Guid? UserId { get; init; }
}
