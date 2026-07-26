namespace CMDocumentRepository.Application.DTOs;

public record DocumentTypeDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsSystem { get; init; }
}

public record CreateDocumentTypeDto
{
    public string Name { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string? Description { get; init; }
}

public record CategoryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string? Description { get; init; }
    public Guid? ParentCategoryId { get; init; }
    public string? ParentCategoryName { get; init; }
    public List<CategoryDto> SubCategories { get; init; } = new();
}

public record CreateCategoryDto
{
    public string Name { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string? Description { get; init; }
    public Guid? ParentCategoryId { get; init; }
}

public record DocumentPermissionDto
{
    public Guid Id { get; init; }
    public Guid DocumentId { get; init; }
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public bool CanRead { get; init; }
    public bool CanEdit { get; init; }
    public bool CanApprove { get; init; }
    public bool CanDelete { get; init; }
}

public record SetDocumentPermissionDto
{
    public Guid DocumentId { get; init; }
    public Guid UserId { get; init; }
    public bool CanRead { get; init; } = true;
    public bool CanEdit { get; init; }
    public bool CanApprove { get; init; }
    public bool CanDelete { get; init; }
}

public record AuditLogDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty;
    public string EntityType { get; init; } = string.Empty;
    public Guid? EntityId { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record PaginatedResult<T>
{
    public List<T> Items { get; init; } = new();
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPages;
}
