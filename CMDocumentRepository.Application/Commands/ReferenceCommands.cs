using CMDocumentRepository.Application.DTOs;
using MediatR;

namespace CMDocumentRepository.Application.Commands;

public record CreateDocumentTypeCommand : IRequest<DocumentTypeDto>
{
    public string Name { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string? Description { get; init; }
}

public record UpdateDocumentTypeCommand : IRequest<DocumentTypeDto>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}

public record DeleteDocumentTypeCommand : IRequest<bool>
{
    public Guid Id { get; init; }
}

public record CreateCategoryCommand : IRequest<CategoryDto>
{
    public string Name { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string? Description { get; init; }
    public Guid? ParentCategoryId { get; init; }
}

public record UpdateCategoryCommand : IRequest<CategoryDto>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}

public record DeleteCategoryCommand : IRequest<bool>
{
    public Guid Id { get; init; }
}

public record SetDocumentPermissionCommand : IRequest<bool>
{
    public Guid DocumentId { get; init; }
    public Guid UserId { get; init; }
    public bool CanRead { get; init; } = true;
    public bool CanEdit { get; init; }
    public bool CanApprove { get; init; }
    public bool CanDelete { get; init; }
}

public record RemoveDocumentPermissionCommand : IRequest<bool>
{
    public Guid DocumentId { get; init; }
    public Guid UserId { get; init; }
}
