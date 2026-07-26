using CMDocumentRepository.Application.DTOs;
using CMDocumentRepository.Domain.Entities;
using CMDocumentRepository.Domain.Interfaces;
using MediatR;

namespace CMDocumentRepository.Application.Commands;

public class CreateDocumentTypeCommandHandler : IRequestHandler<CreateDocumentTypeCommand, DocumentTypeDto>
{
    private readonly IDocumentTypeRepository _repository;

    public CreateDocumentTypeCommandHandler(IDocumentTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<DocumentTypeDto> Handle(CreateDocumentTypeCommand request, CancellationToken cancellationToken)
    {
        if (await _repository.CodeExistsAsync(request.Code))
            throw new InvalidOperationException("Тип документа с таким кодом уже существует");

        var entity = new DocumentType
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Code = request.Code,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(entity);

        return new DocumentTypeDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Code = entity.Code,
            Description = entity.Description,
            IsSystem = entity.IsSystem
        };
    }
}

public class UpdateDocumentTypeCommandHandler : IRequestHandler<UpdateDocumentTypeCommand, DocumentTypeDto>
{
    private readonly IDocumentTypeRepository _repository;

    public UpdateDocumentTypeCommandHandler(IDocumentTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<DocumentTypeDto> Handle(UpdateDocumentTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException("Тип документа не найден");

        entity.Name = request.Name;
        entity.Description = request.Description;
        await _repository.UpdateAsync(entity);

        return new DocumentTypeDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Code = entity.Code,
            Description = entity.Description,
            IsSystem = entity.IsSystem
        };
    }
}

public class DeleteDocumentTypeCommandHandler : IRequestHandler<DeleteDocumentTypeCommand, bool>
{
    private readonly IDocumentTypeRepository _repository;

    public DeleteDocumentTypeCommandHandler(IDocumentTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(DeleteDocumentTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id);
        if (entity == null) return false;

        if (entity.IsSystem)
            throw new InvalidOperationException("Невозможно удалить системный тип документа");

        await _repository.DeleteAsync(entity);
        return true;
    }
}

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _repository;

    public CreateCategoryCommandHandler(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (await _repository.CodeExistsAsync(request.Code))
            throw new InvalidOperationException("Категория с таким кодом уже существует");

        var entity = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Code = request.Code,
            Description = request.Description,
            ParentCategoryId = request.ParentCategoryId,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(entity);

        return new CategoryDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Code = entity.Code,
            Description = entity.Description,
            ParentCategoryId = entity.ParentCategoryId
        };
    }
}

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _repository;

    public UpdateCategoryCommandHandler(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException("Категория не найдена");

        entity.Name = request.Name;
        entity.Description = request.Description;
        await _repository.UpdateAsync(entity);

        return new CategoryDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Code = entity.Code,
            Description = entity.Description,
            ParentCategoryId = entity.ParentCategoryId
        };
    }
}

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
{
    private readonly ICategoryRepository _repository;

    public DeleteCategoryCommandHandler(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id);
        if (entity == null) return false;

        var subCategories = await _repository.GetSubCategoriesAsync(request.Id);
        if (subCategories.Any())
            throw new InvalidOperationException("Невозможно удалить категорию с подкатегориями");

        await _repository.DeleteAsync(entity);
        return true;
    }
}

public class SetDocumentPermissionCommandHandler : IRequestHandler<SetDocumentPermissionCommand, bool>
{
    private readonly IDocumentPermissionRepository _permissionRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IUserRepository _userRepository;

    public SetDocumentPermissionCommandHandler(
        IDocumentPermissionRepository permissionRepository,
        IDocumentRepository documentRepository,
        IUserRepository userRepository)
    {
        _permissionRepository = permissionRepository;
        _documentRepository = documentRepository;
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(SetDocumentPermissionCommand request, CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(request.DocumentId)
            ?? throw new KeyNotFoundException("Документ не найден");

        var user = await _userRepository.GetByIdAsync(request.UserId)
            ?? throw new KeyNotFoundException("Пользователь не найден");

        var existing = await _permissionRepository.GetByDocumentAndUserAsync(request.DocumentId, request.UserId);

        if (existing != null)
        {
            existing.CanRead = request.CanRead;
            existing.CanEdit = request.CanEdit;
            existing.CanApprove = request.CanApprove;
            existing.CanDelete = request.CanDelete;
            await _permissionRepository.UpdateAsync(existing);
        }
        else
        {
            var permission = new DocumentPermission
            {
                Id = Guid.NewGuid(),
                DocumentId = request.DocumentId,
                UserId = request.UserId,
                CanRead = request.CanRead,
                CanEdit = request.CanEdit,
                CanApprove = request.CanApprove,
                CanDelete = request.CanDelete,
                CreatedAt = DateTime.UtcNow
            };

            await _permissionRepository.AddAsync(permission);
        }

        return true;
    }
}

public class RemoveDocumentPermissionCommandHandler : IRequestHandler<RemoveDocumentPermissionCommand, bool>
{
    private readonly IDocumentPermissionRepository _permissionRepository;

    public RemoveDocumentPermissionCommandHandler(IDocumentPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task<bool> Handle(RemoveDocumentPermissionCommand request, CancellationToken cancellationToken)
    {
        var permission = await _permissionRepository.GetByDocumentAndUserAsync(request.DocumentId, request.UserId);
        if (permission == null) return false;

        await _permissionRepository.DeleteAsync(permission);
        return true;
    }
}
