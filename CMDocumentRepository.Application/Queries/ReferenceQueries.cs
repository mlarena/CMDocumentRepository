using CMDocumentRepository.Application.DTOs;
using MediatR;

namespace CMDocumentRepository.Application.Queries;

public record GetAllDocumentTypesQuery : IRequest<List<DocumentTypeDto>> { }

public record GetDocumentTypeByIdQuery : IRequest<DocumentTypeDto?>
{
    public Guid Id { get; init; }
}

public record GetAllCategoriesQuery : IRequest<List<CategoryDto>> { }

public record GetCategoryByIdQuery : IRequest<CategoryDto?>
{
    public Guid Id { get; init; }
}

public record GetDocumentPermissionsQuery : IRequest<List<DocumentPermissionDto>>
{
    public Guid DocumentId { get; init; }
}

public record GetMyPermissionsQuery : IRequest<List<DocumentPermissionDto>>
{
    public Guid UserId { get; init; }
}
