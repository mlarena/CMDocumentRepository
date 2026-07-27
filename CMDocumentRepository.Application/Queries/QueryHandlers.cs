using CMDocumentRepository.Application.DTOs;
using CMDocumentRepository.Domain.Enums;
using CMDocumentRepository.Domain.Interfaces;
using MediatR;

namespace CMDocumentRepository.Application.Queries;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IUserRepository _repository;
    public GetUserByIdQueryHandler(IUserRepository repository) => _repository = repository;

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(request.Id);
        if (user == null) return null;

        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            MiddleName = user.MiddleName,
            Role = user.Role,
            IsActive = user.IsActive,
            IsLocked = user.IsLocked,
            LockedUntil = user.LockedUntil,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        };
    }
}

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserDto>>
{
    private readonly IUserRepository _repository;
    public GetAllUsersQueryHandler(IUserRepository repository) => _repository = repository;

    public async Task<List<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _repository.GetAllAsync();

        if (request.Role.HasValue)
            users = users.Where(u => u.Role == request.Role.Value);

        if (request.IsActive.HasValue)
            users = users.Where(u => u.IsActive == request.IsActive.Value);

        return users.Select(u => new UserDto
        {
            Id = u.Id,
            UserName = u.UserName,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            MiddleName = u.MiddleName,
            Role = u.Role,
            IsActive = u.IsActive,
            IsLocked = u.IsLocked,
            CreatedAt = u.CreatedAt,
            LastLoginAt = u.LastLoginAt
        }).ToList();
    }
}

public class GetUserByUserNameQueryHandler : IRequestHandler<GetUserByUserNameQuery, UserDto?>
{
    private readonly IUserRepository _repository;
    public GetUserByUserNameQueryHandler(IUserRepository repository) => _repository = repository;

    public async Task<UserDto?> Handle(GetUserByUserNameQuery request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByUserNameAsync(request.UserName);
        if (user == null) return null;

        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            MiddleName = user.MiddleName,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}

public class GetDocumentByIdQueryHandler : IRequestHandler<GetDocumentByIdQuery, DocumentDto?>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IDocumentTypeRepository _typeRepository;
    private readonly IUserRepository _userRepository;

    public GetDocumentByIdQueryHandler(
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

    public async Task<DocumentDto?> Handle(GetDocumentByIdQuery request, CancellationToken cancellationToken)
    {
        var doc = await _documentRepository.GetByIdAsync(request.Id);
        if (doc == null) return null;

        var category = await _categoryRepository.GetByIdAsync(doc.CategoryId);
        var type = await _typeRepository.GetByIdAsync(doc.DocumentTypeId);
        var creator = await _userRepository.GetByIdAsync(doc.CreatedBy);

        return new DocumentDto
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
            ApprovedBy = doc.ApprovedBy,
            ApprovedAt = doc.ApprovedAt,
            ValidFrom = doc.ValidFrom,
            ValidUntil = doc.ValidUntil,
            FilePath = doc.FilePath,
            FileSize = doc.FileSize,
            FileExtension = doc.FileExtension,
            IsDeleted = doc.IsDeleted,
            DeletedAt = doc.DeletedAt
        };
    }
}

public class GetAllDocumentsQueryHandler : IRequestHandler<GetAllDocumentsQuery, List<DocumentDto>>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IDocumentTypeRepository _typeRepository;
    private readonly IUserRepository _userRepository;

    public GetAllDocumentsQueryHandler(
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

    public async Task<List<DocumentDto>> Handle(GetAllDocumentsQuery request, CancellationToken cancellationToken)
    {
        var docs = await _documentRepository.SearchAsync(
            string.Empty, request.Status, request.CategoryId, request.DocumentTypeId, null, null);

        var result = new List<DocumentDto>();
        foreach (var doc in docs)
        {
            var category = await _categoryRepository.GetByIdAsync(doc.CategoryId);
            var type = await _typeRepository.GetByIdAsync(doc.DocumentTypeId);
            var creator = await _userRepository.GetByIdAsync(doc.CreatedBy);

            result.Add(new DocumentDto
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

        return result;
    }
}

public class SearchDocumentsQueryHandler : IRequestHandler<SearchDocumentsQuery, List<DocumentDto>>
{
    private readonly ISearchService _searchService;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IDocumentTypeRepository _typeRepository;
    private readonly IUserRepository _userRepository;

    public SearchDocumentsQueryHandler(
        ISearchService searchService,
        ICategoryRepository categoryRepository,
        IDocumentTypeRepository typeRepository,
        IUserRepository userRepository)
    {
        _searchService = searchService;
        _categoryRepository = categoryRepository;
        _typeRepository = typeRepository;
        _userRepository = userRepository;
    }

    public async Task<List<DocumentDto>> Handle(SearchDocumentsQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Keyword))
            return new List<DocumentDto>();

        var docs = await _searchService.FullTextSearchAsync(
            request.Keyword,
            request.Status,
            request.CategoryId,
            request.DocumentTypeId,
            request.DateFrom,
            request.DateTo);

        var result = new List<DocumentDto>();
        foreach (var doc in docs)
        {
            var category = await _categoryRepository.GetByIdAsync(doc.CategoryId);
            var type = await _typeRepository.GetByIdAsync(doc.DocumentTypeId);
            var creator = await _userRepository.GetByIdAsync(doc.CreatedBy);

            result.Add(new DocumentDto
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
                ValidFrom = doc.ValidFrom,
                ValidUntil = doc.ValidUntil,
                FilePath = doc.FilePath,
                FileSize = doc.FileSize,
                FileExtension = doc.FileExtension
            });
        }

        return result;
    }
}

public class GetDocumentByNumberQueryHandler : IRequestHandler<GetDocumentByNumberQuery, DocumentDto?>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IDocumentTypeRepository _typeRepository;
    private readonly IUserRepository _userRepository;

    public GetDocumentByNumberQueryHandler(
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

    public async Task<DocumentDto?> Handle(GetDocumentByNumberQuery request, CancellationToken cancellationToken)
    {
        var doc = await _documentRepository.GetByNumberAsync(request.DocumentNumber);
        if (doc == null) return null;

        var category = await _categoryRepository.GetByIdAsync(doc.CategoryId);
        var type = await _typeRepository.GetByIdAsync(doc.DocumentTypeId);
        var creator = await _userRepository.GetByIdAsync(doc.CreatedBy);

        return new DocumentDto
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
            ValidFrom = doc.ValidFrom,
            ValidUntil = doc.ValidUntil,
            FilePath = doc.FilePath,
            FileSize = doc.FileSize,
            FileExtension = doc.FileExtension
        };
    }
}

public class GetDocumentVersionsQueryHandler : IRequestHandler<GetDocumentVersionsQuery, List<DocumentVersionDto>>
{
    private readonly IDocumentVersionRepository _versionRepository;
    private readonly IUserRepository _userRepository;

    public GetDocumentVersionsQueryHandler(IDocumentVersionRepository versionRepository, IUserRepository userRepository)
    {
        _versionRepository = versionRepository;
        _userRepository = userRepository;
    }

    public async Task<List<DocumentVersionDto>> Handle(GetDocumentVersionsQuery request, CancellationToken cancellationToken)
    {
        var versions = await _versionRepository.GetByDocumentIdAsync(request.DocumentId);

        return versions.Select(v => new DocumentVersionDto
        {
            Id = v.Id,
            DocumentId = v.DocumentId,
            VersionNumber = v.VersionNumber,
            FilePath = v.FilePath,
            FileSize = v.FileSize,
            CreatedBy = v.CreatedBy,
            CreatedAt = v.CreatedAt,
            ChangeComment = v.ChangeComment
        }).ToList();
    }
}

public class GetMyDocumentsQueryHandler : IRequestHandler<GetMyDocumentsQuery, List<DocumentDto>>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IDocumentTypeRepository _typeRepository;
    private readonly IUserRepository _userRepository;

    public GetMyDocumentsQueryHandler(
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

    public async Task<List<DocumentDto>> Handle(GetMyDocumentsQuery request, CancellationToken cancellationToken)
    {
        var docs = await _documentRepository.GetByCreatorAsync(request.UserId);

        var result = new List<DocumentDto>();
        foreach (var doc in docs)
        {
            var cat = await _categoryRepository.GetByIdAsync(doc.CategoryId);
            var typ = await _typeRepository.GetByIdAsync(doc.DocumentTypeId);
            var creator = await _userRepository.GetByIdAsync(doc.CreatedBy);

            result.Add(new DocumentDto
            {
                Id = doc.Id,
                DocumentNumber = doc.DocumentNumber,
                Title = doc.Title,
                Description = doc.Description,
                CategoryId = doc.CategoryId,
                CategoryName = cat?.Name ?? "—",
                DocumentTypeId = doc.DocumentTypeId,
                DocumentTypeName = typ?.Name ?? "—",
                Version = doc.Version,
                Status = doc.Status,
                CreatedBy = doc.CreatedBy,
                CreatorName = creator != null ? $"{creator.LastName} {creator.FirstName}" : "—",
                CreatedAt = doc.CreatedAt,
                ValidFrom = doc.ValidFrom,
                ValidUntil = doc.ValidUntil,
                FilePath = doc.FilePath,
                FileSize = doc.FileSize,
                FileExtension = doc.FileExtension
            });
        }
        return result;
    }
}

public class GetDocumentsForApprovalQueryHandler : IRequestHandler<GetDocumentsForApprovalQuery, List<DocumentDto>>
{
    private readonly IApprovalRepository _approvalRepository;
    private readonly IDocumentRepository _documentRepository;

    public GetDocumentsForApprovalQueryHandler(IApprovalRepository approvalRepository, IDocumentRepository documentRepository)
    {
        _approvalRepository = approvalRepository;
        _documentRepository = documentRepository;
    }

    public async Task<List<DocumentDto>> Handle(GetDocumentsForApprovalQuery request, CancellationToken cancellationToken)
    {
        var pendingApprovals = await _approvalRepository.GetPendingByApproverAsync(request.UserId);
        var result = new List<DocumentDto>();

        foreach (var approval in pendingApprovals)
        {
            var doc = await _documentRepository.GetByIdAsync(approval.DocumentId);
            if (doc != null)
            {
                result.Add(new DocumentDto
                {
                    Id = doc.Id,
                    DocumentNumber = doc.DocumentNumber,
                    Title = doc.Title,
                    Status = doc.Status,
                    CreatedAt = doc.CreatedAt,
                    FileExtension = doc.FileExtension
                });
            }
        }

        return result;
    }
}

public class GetDeletedDocumentsQueryHandler : IRequestHandler<GetDeletedDocumentsQuery, List<DocumentDto>>
{
    private readonly IDocumentRepository _documentRepository;

    public GetDeletedDocumentsQueryHandler(IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<List<DocumentDto>> Handle(GetDeletedDocumentsQuery request, CancellationToken cancellationToken)
    {
        var docs = await _documentRepository.GetAllAsync();
        var deleted = docs.Where(d => d.IsDeleted).ToList();

        return deleted.Select(doc => new DocumentDto
        {
            Id = doc.Id,
            DocumentNumber = doc.DocumentNumber,
            Title = doc.Title,
            Status = doc.Status,
            CreatedAt = doc.CreatedAt,
            DeletedAt = doc.DeletedAt,
            IsDeleted = true
        }).ToList();
    }
}

public class GetApprovalsByDocumentQueryHandler : IRequestHandler<GetApprovalsByDocumentQuery, List<ApprovalDto>>
{
    private readonly IApprovalRepository _approvalRepository;
    private readonly IUserRepository _userRepository;

    public GetApprovalsByDocumentQueryHandler(IApprovalRepository approvalRepository, IUserRepository userRepository)
    {
        _approvalRepository = approvalRepository;
        _userRepository = userRepository;
    }

    public async Task<List<ApprovalDto>> Handle(GetApprovalsByDocumentQuery request, CancellationToken cancellationToken)
    {
        var approvals = await _approvalRepository.GetByDocumentIdAsync(request.DocumentId);

        return approvals.Select(a => new ApprovalDto
        {
            Id = a.Id,
            DocumentId = a.DocumentId,
            ApproverId = a.ApproverId,
            Status = a.Status,
            Comment = a.Comment,
            OrderNumber = a.OrderNumber,
            ApprovedAt = a.ApprovedAt,
            CreatedAt = a.CreatedAt
        }).ToList();
    }
}

public class GetPendingApprovalsQueryHandler : IRequestHandler<GetPendingApprovalsQuery, List<ApprovalDto>>
{
    private readonly IApprovalRepository _approvalRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IUserRepository _userRepository;

    public GetPendingApprovalsQueryHandler(
        IApprovalRepository approvalRepository,
        IDocumentRepository documentRepository,
        IUserRepository userRepository)
    {
        _approvalRepository = approvalRepository;
        _documentRepository = documentRepository;
        _userRepository = userRepository;
    }

    public async Task<List<ApprovalDto>> Handle(GetPendingApprovalsQuery request, CancellationToken cancellationToken)
    {
        var approvals = await _approvalRepository.GetPendingByApproverAsync(request.ApproverId);
        var result = new List<ApprovalDto>();

        foreach (var a in approvals)
        {
            var doc = await _documentRepository.GetByIdAsync(a.DocumentId);
            var approver = await _userRepository.GetByIdAsync(a.ApproverId);

            result.Add(new ApprovalDto
            {
                Id = a.Id,
                DocumentId = a.DocumentId,
                DocumentNumber = doc?.DocumentNumber ?? string.Empty,
                DocumentTitle = doc?.Title ?? string.Empty,
                ApproverId = a.ApproverId,
                ApproverName = approver != null ? $"{approver.LastName} {approver.FirstName}" : string.Empty,
                Status = a.Status,
                Comment = a.Comment,
                OrderNumber = a.OrderNumber,
                ApprovedAt = a.ApprovedAt,
                CreatedAt = a.CreatedAt
            });
        }

        return result;
    }
}

public class GetMyApprovalsQueryHandler : IRequestHandler<GetMyApprovalsQuery, List<ApprovalDto>>
{
    private readonly IApprovalRepository _approvalRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IUserRepository _userRepository;

    public GetMyApprovalsQueryHandler(
        IApprovalRepository approvalRepository,
        IDocumentRepository documentRepository,
        IUserRepository userRepository)
    {
        _approvalRepository = approvalRepository;
        _documentRepository = documentRepository;
        _userRepository = userRepository;
    }

    public async Task<List<ApprovalDto>> Handle(GetMyApprovalsQuery request, CancellationToken cancellationToken)
    {
        var approvals = await _approvalRepository.GetByApproverIdAsync(request.ApproverId);
        var result = new List<ApprovalDto>();

        foreach (var a in approvals)
        {
            var doc = await _documentRepository.GetByIdAsync(a.DocumentId);

            result.Add(new ApprovalDto
            {
                Id = a.Id,
                DocumentId = a.DocumentId,
                DocumentNumber = doc?.DocumentNumber ?? string.Empty,
                DocumentTitle = doc?.Title ?? string.Empty,
                ApproverId = a.ApproverId,
                Status = a.Status,
                Comment = a.Comment,
                OrderNumber = a.OrderNumber,
                ApprovedAt = a.ApprovedAt,
                CreatedAt = a.CreatedAt
            });
        }

        return result;
    }
}

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskDto?>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;

    public GetTaskByIdQueryHandler(ITaskRepository taskRepository, IUserRepository userRepository)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
    }

    public async Task<TaskDto?> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.Id);
        if (task == null) return null;

        var creator = await _userRepository.GetByIdAsync(task.CreatedBy);
        string? assigneeName = null;
        if (task.AssigneeId.HasValue)
        {
            var assignee = await _userRepository.GetByIdAsync(task.AssigneeId.Value);
            if (assignee != null) assigneeName = $"{assignee.LastName} {assignee.FirstName}";
        }

        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Priority = task.Priority,
            Status = task.Status,
            AssigneeId = task.AssigneeId,
            AssigneeName = assigneeName,
            CreatedBy = task.CreatedBy,
            CreatorName = creator != null ? $"{creator.LastName} {creator.FirstName}" : string.Empty,
            CreatedAt = task.CreatedAt,
            DueDate = task.DueDate,
            OrderNumber = task.OrderNumber
        };
    }
}

public class GetAllTasksQueryHandler : IRequestHandler<GetAllTasksQuery, List<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;

    public GetAllTasksQueryHandler(ITaskRepository taskRepository, IUserRepository userRepository)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
    }

    public async Task<List<TaskDto>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
    {
        var tasks = await _taskRepository.GetAllAsync();

        if (request.Status.HasValue)
            tasks = tasks.Where(t => t.Status == request.Status.Value);
        if (request.AssigneeId.HasValue)
            tasks = tasks.Where(t => t.AssigneeId == request.AssigneeId.Value);
        if (request.Priority.HasValue)
            tasks = tasks.Where(t => t.Priority == request.Priority.Value);

        return tasks.Select(t => new TaskDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Priority = t.Priority,
            Status = t.Status,
            AssigneeId = t.AssigneeId,
            CreatedBy = t.CreatedBy,
            CreatedAt = t.CreatedAt,
            DueDate = t.DueDate,
            OrderNumber = t.OrderNumber
        }).ToList();
    }
}

public class GetTasksByStatusQueryHandler : IRequestHandler<GetTasksByStatusQuery, List<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;

    public GetTasksByStatusQueryHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<List<TaskDto>> Handle(GetTasksByStatusQuery request, CancellationToken cancellationToken)
    {
        var tasks = await _taskRepository.GetByStatusAsync(request.Status);

        return tasks.Select(t => new TaskDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Priority = t.Priority,
            Status = t.Status,
            AssigneeId = t.AssigneeId,
            CreatedBy = t.CreatedBy,
            CreatedAt = t.CreatedAt,
            DueDate = t.DueDate,
            OrderNumber = t.OrderNumber
        }).ToList();
    }
}

public class GetMyTasksQueryHandler : IRequestHandler<GetMyTasksQuery, List<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;

    public GetMyTasksQueryHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<List<TaskDto>> Handle(GetMyTasksQuery request, CancellationToken cancellationToken)
    {
        var tasks = await _taskRepository.GetByAssigneeAsync(request.UserId);

        return tasks.Select(t => new TaskDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Priority = t.Priority,
            Status = t.Status,
            AssigneeId = t.AssigneeId,
            CreatedBy = t.CreatedBy,
            CreatedAt = t.CreatedAt,
            DueDate = t.DueDate,
            OrderNumber = t.OrderNumber
        }).ToList();
    }
}

public class GetAllDocumentTypesQueryHandler : IRequestHandler<GetAllDocumentTypesQuery, List<DocumentTypeDto>>
{
    private readonly IDocumentTypeRepository _repository;
    public GetAllDocumentTypesQueryHandler(IDocumentTypeRepository repository) => _repository = repository;

    public async Task<List<DocumentTypeDto>> Handle(GetAllDocumentTypesQuery request, CancellationToken cancellationToken)
    {
        var types = await _repository.GetAllAsync();
        return types.Select(t => new DocumentTypeDto
        {
            Id = t.Id,
            Name = t.Name,
            Code = t.Code,
            Description = t.Description,
            IsSystem = t.IsSystem
        }).ToList();
    }
}

public class GetDocumentTypeByIdQueryHandler : IRequestHandler<GetDocumentTypeByIdQuery, DocumentTypeDto?>
{
    private readonly IDocumentTypeRepository _repository;
    public GetDocumentTypeByIdQueryHandler(IDocumentTypeRepository repository) => _repository = repository;

    public async Task<DocumentTypeDto?> Handle(GetDocumentTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var type = await _repository.GetByIdAsync(request.Id);
        if (type == null) return null;

        return new DocumentTypeDto
        {
            Id = type.Id,
            Name = type.Name,
            Code = type.Code,
            Description = type.Description,
            IsSystem = type.IsSystem
        };
    }
}

public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryDto>>
{
    private readonly ICategoryRepository _repository;
    public GetAllCategoriesQueryHandler(ICategoryRepository repository) => _repository = repository;

    public async Task<List<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _repository.GetRootCategoriesAsync();
        return categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Code = c.Code,
            Description = c.Description,
            ParentCategoryId = c.ParentCategoryId
        }).ToList();
    }
}

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
{
    private readonly ICategoryRepository _repository;
    public GetCategoryByIdQueryHandler(ICategoryRepository repository) => _repository = repository;

    public async Task<CategoryDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _repository.GetByIdAsync(request.Id);
        if (category == null) return null;

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Code = category.Code,
            Description = category.Description,
            ParentCategoryId = category.ParentCategoryId
        };
    }
}

public class GetDocumentPermissionsQueryHandler : IRequestHandler<GetDocumentPermissionsQuery, List<DocumentPermissionDto>>
{
    private readonly IDocumentPermissionRepository _permissionRepository;
    private readonly IUserRepository _userRepository;

    public GetDocumentPermissionsQueryHandler(IDocumentPermissionRepository permissionRepository, IUserRepository userRepository)
    {
        _permissionRepository = permissionRepository;
        _userRepository = userRepository;
    }

    public async Task<List<DocumentPermissionDto>> Handle(GetDocumentPermissionsQuery request, CancellationToken cancellationToken)
    {
        var permissions = await _permissionRepository.GetByDocumentIdAsync(request.DocumentId);

        return permissions.Select(p => new DocumentPermissionDto
        {
            Id = p.Id,
            DocumentId = p.DocumentId,
            UserId = p.UserId,
            CanRead = p.CanRead,
            CanEdit = p.CanEdit,
            CanApprove = p.CanApprove,
            CanDelete = p.CanDelete
        }).ToList();
    }
}

public class GetMyPermissionsQueryHandler : IRequestHandler<GetMyPermissionsQuery, List<DocumentPermissionDto>>
{
    private readonly IDocumentPermissionRepository _permissionRepository;

    public GetMyPermissionsQueryHandler(IDocumentPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task<List<DocumentPermissionDto>> Handle(GetMyPermissionsQuery request, CancellationToken cancellationToken)
    {
        var permissions = await _permissionRepository.GetByUserIdAsync(request.UserId);

        return permissions.Select(p => new DocumentPermissionDto
        {
            Id = p.Id,
            DocumentId = p.DocumentId,
            UserId = p.UserId,
            CanRead = p.CanRead,
            CanEdit = p.CanEdit,
            CanApprove = p.CanApprove,
            CanDelete = p.CanDelete
        }).ToList();
    }
}

public class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, List<AuditLogDto>>
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IUserRepository _userRepository;

    public GetAuditLogsQueryHandler(IAuditLogRepository auditLogRepository, IUserRepository userRepository)
    {
        _auditLogRepository = auditLogRepository;
        _userRepository = userRepository;
    }

    public async Task<List<AuditLogDto>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Domain.Entities.AuditLog> logs;

        if (request.UserId.HasValue)
        {
            logs = await _auditLogRepository.GetByUserIdAsync(request.UserId.Value);
        }
        else if (!string.IsNullOrEmpty(request.EntityType))
        {
            logs = await _auditLogRepository.GetByEntityTypeAsync(request.EntityType);
        }
        else if (request.FromDate.HasValue || request.ToDate.HasValue)
        {
            logs = await _auditLogRepository.GetByDateRangeAsync(
                request.FromDate ?? DateTime.MinValue,
                request.ToDate ?? DateTime.MaxValue);
        }
        else
        {
            logs = await _auditLogRepository.GetAllAsync();
        }

        var result = new List<AuditLogDto>();
        foreach (var log in logs.OrderByDescending(l => l.CreatedAt).Take(request.PageSize))
        {
            var user = await _userRepository.GetByIdAsync(log.UserId);
            result.Add(new AuditLogDto
            {
                Id = log.Id,
                UserId = log.UserId,
                UserName = user?.UserName ?? "—",
                Action = log.Action,
                EntityType = log.EntityType,
                EntityId = log.EntityId,
                CreatedAt = log.CreatedAt
            });
        }

        return result;
    }
}

public class GetGanttDataQueryHandler : IRequestHandler<GetGanttDataQuery, GanttDataDto>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly ITaskRepository _taskRepository;

    public GetGanttDataQueryHandler(IDocumentRepository documentRepository, ITaskRepository taskRepository)
    {
        _documentRepository = documentRepository;
        _taskRepository = taskRepository;
    }

    public async Task<GanttDataDto> Handle(GetGanttDataQuery request, CancellationToken cancellationToken)
    {
        var items = new List<GanttItemDto>();

        // Документы с заданными сроками действия
        var docs = await _documentRepository.GetAllAsync();
        foreach (var doc in docs.Where(d => !d.IsDeleted && d.ValidFrom.HasValue && d.ValidUntil.HasValue))
        {
            var progress = doc.Status switch
            {
                DocumentStatus.Active => 100,
                DocumentStatus.Approved => 100,
                DocumentStatus.Archived => 100,
                DocumentStatus.Draft => 0,
                DocumentStatus.Rejected => 0,
                _ => 50
            };

            items.Add(new GanttItemDto
            {
                Id = doc.Id,
                Name = doc.Title,
                Type = "document",
                Start = doc.ValidFrom!.Value,
                End = doc.ValidUntil!.Value,
                Progress = progress,
                Url = $"/Document/Details/{doc.Id}",
                Status = doc.Status.ToString()
            });
        }

        // Задачи с дедлайном
        var tasks = await _taskRepository.GetAllAsync();
        foreach (var task in tasks.Where(t => t.DueDate.HasValue))
        {
            var start = task.CreatedAt;
            var end = task.DueDate!.Value;
            if (end <= start) end = start.AddDays(1);

            var progress = task.Status switch
            {
                AppTaskStatus.Done => 100,
                AppTaskStatus.Review => 75,
                AppTaskStatus.InProgress => 50,
                _ => 0
            };

            items.Add(new GanttItemDto
            {
                Id = task.Id,
                Name = task.Title,
                Type = "task",
                Start = start,
                End = end,
                Progress = progress,
                Url = $"/Task/Details/{task.Id}",
                Status = task.Status.ToString()
            });
        }

        return new GanttDataDto { Items = items };
    }
}
