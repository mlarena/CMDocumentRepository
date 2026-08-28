using CMDocumentRepository.Application.DTOs;
using MediatR;

namespace CMDocumentRepository.Application.Queries;

public record GetDocumentMetadataQuery : IRequest<DocumentMetadataDto>
{
    public Guid DocumentId { get; init; }
}

public record UpdateDocumentMetadataCommand : IRequest<bool>
{
    public Guid DocumentId { get; init; }
    public Dictionary<string, string?> Metadata { get; init; } = new();
}
