using CMDocumentRepository.Application.DTOs;
using MediatR;

namespace CMDocumentRepository.Application.Commands;

public record CreateDocumentCommand : IRequest<DocumentDto>
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public Guid CategoryId { get; init; }
    public Guid DocumentTypeId { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidUntil { get; init; }
    public Stream? File { get; init; }
    public string? FileName { get; init; }
    public Guid CreatedBy { get; init; }
}

public record UpdateDocumentCommand : IRequest<DocumentDto>
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public Guid CategoryId { get; init; }
    public Guid DocumentTypeId { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidUntil { get; init; }
    public Stream? File { get; init; }
    public string? FileName { get; init; }
    public string? ChangeComment { get; init; }
    public Guid UpdatedBy { get; init; }
}

public record DeleteDocumentCommand : IRequest<bool>
{
    public Guid Id { get; init; }
    public Guid DeletedBy { get; init; }
}

public record RestoreDocumentCommand : IRequest<bool>
{
    public Guid Id { get; init; }
}

public record PermanentDeleteDocumentCommand : IRequest<bool>
{
    public Guid Id { get; init; }
}

public record SetDocumentStatusCommand : IRequest<bool>
{
    public Guid Id { get; init; }
    public CMDocumentRepository.Domain.Enums.DocumentStatus Status { get; init; }
    public Guid? ApprovedBy { get; init; }
}
