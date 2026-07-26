using CMDocumentRepository.Application.DTOs;
using CMDocumentRepository.Domain.Enums;
using CMDocumentRepository.Domain.Interfaces;
using MediatR;

namespace CMDocumentRepository.Application.Queries;

public record GetPagedDocumentsQuery : IRequest<PagedResult<DocumentDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? Keyword { get; init; }
    public DocumentStatus? Status { get; init; }
    public Guid? CategoryId { get; init; }
    public Guid? DocumentTypeId { get; init; }
    public Guid? CreatedBy { get; init; }
}

public class GetPagedDocumentsQueryHandler : IRequestHandler<GetPagedDocumentsQuery, PagedResult<DocumentDto>>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IDocumentTypeRepository _typeRepository;
    private readonly IUserRepository _userRepository;

    public GetPagedDocumentsQueryHandler(
        IDocumentRepository documentRepository,
        ICategoryRepository categoryRepository,
        IDocumentTypeRepository typeRepository,
        IUserRepository userRepository)
    {
        _documentRepository = documentRepository;
        _categoryRepository = categoryRepository;
        _typeRepository = typeRepository;
        _userRepository = userRepository;
    }

    public async Task<PagedResult<DocumentDto>> Handle(GetPagedDocumentsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _documentRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.Keyword,
            request.Status,
            request.CategoryId,
            request.DocumentTypeId,
            request.CreatedBy);

        var dtos = new List<DocumentDto>();
        foreach (var doc in items)
        {
            var category = await _categoryRepository.GetByIdAsync(doc.CategoryId);
            var type = await _typeRepository.GetByIdAsync(doc.DocumentTypeId);
            var creator = await _userRepository.GetByIdAsync(doc.CreatedBy);

            dtos.Add(new DocumentDto
            {
                Id = doc.Id,
                DocumentNumber = doc.DocumentNumber,
                Title = doc.Title,
                Description = doc.Description,
                CategoryId = doc.CategoryId,
                CategoryName = category?.Name ?? string.Empty,
                DocumentTypeId = doc.DocumentTypeId,
                DocumentTypeName = type?.Name ?? string.Empty,
                Version = doc.Version,
                Status = doc.Status,
                CreatedBy = doc.CreatedBy,
                CreatorName = creator != null ? $"{creator.LastName} {creator.FirstName}" : string.Empty,
                CreatedAt = doc.CreatedAt,
                UpdatedAt = doc.UpdatedAt,
                ValidFrom = doc.ValidFrom,
                ValidUntil = doc.ValidUntil,
                FilePath = doc.FilePath,
                FileSize = doc.FileSize,
                FileExtension = doc.FileExtension,
                IsDeleted = doc.IsDeleted
            });
        }

        return PagedResult<DocumentDto>.Create(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}
