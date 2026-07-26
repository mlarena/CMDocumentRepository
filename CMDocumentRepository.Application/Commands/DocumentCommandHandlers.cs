using CMDocumentRepository.Application.DTOs;
using CMDocumentRepository.Domain.Entities;
using CMDocumentRepository.Domain.Enums;
using CMDocumentRepository.Domain.Interfaces;
using MediatR;

namespace CMDocumentRepository.Application.Commands;

public class CreateDocumentCommandHandler : IRequestHandler<CreateDocumentCommand, DocumentDto>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IDocumentTypeRepository _documentTypeRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly INumberingService _numberingService;
    private readonly IFileService _fileService;

    public CreateDocumentCommandHandler(
        IDocumentRepository documentRepository,
        IDocumentTypeRepository documentTypeRepository,
        ICategoryRepository categoryRepository,
        IUserRepository userRepository,
        INumberingService numberingService,
        IFileService fileService)
    {
        _documentRepository = documentRepository;
        _documentTypeRepository = documentTypeRepository;
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _numberingService = numberingService;
        _fileService = fileService;
    }

    public async Task<DocumentDto> Handle(CreateDocumentCommand request, CancellationToken cancellationToken)
    {
        var documentType = await _documentTypeRepository.GetByIdAsync(request.DocumentTypeId)
            ?? throw new KeyNotFoundException("Тип документа не найден");

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId)
            ?? throw new KeyNotFoundException("Категория не найдена");

        var creator = await _userRepository.GetByIdAsync(request.CreatedBy)
            ?? throw new KeyNotFoundException("Пользователь не найден");

        var documentNumber = await _numberingService.GenerateDocumentNumberAsync(documentType.Code);

        var filePath = string.Empty;
        var fileSize = 0L;
        var fileExtension = string.Empty;
        var mimeType = string.Empty;

        if (request.File != null && !string.IsNullOrEmpty(request.FileName))
        {
            fileExtension = Path.GetExtension(request.FileName).ToLowerInvariant();
            if (!_fileService.IsAllowedExtension(fileExtension))
                throw new InvalidOperationException($"Формат файла {fileExtension} не поддерживается");

            filePath = await _fileService.SaveFileAsync(request.File, request.FileName, Guid.NewGuid());
            fileSize = request.File.Length;
            mimeType = GetMimeType(fileExtension);
        }

        var document = new Document
        {
            Id = Guid.NewGuid(),
            DocumentNumber = documentNumber,
            Title = request.Title,
            Description = request.Description,
            CategoryId = request.CategoryId,
            DocumentTypeId = request.DocumentTypeId,
            Version = 1.0m,
            Status = DocumentStatus.Draft,
            CreatedBy = request.CreatedBy,
            ValidFrom = request.ValidFrom,
            ValidUntil = request.ValidUntil,
            FilePath = filePath,
            FileSize = fileSize,
            FileExtension = fileExtension,
            MimeType = mimeType,
            CreatedAt = DateTime.UtcNow
        };

        await _documentRepository.AddAsync(document);

        return new DocumentDto
        {
            Id = document.Id,
            DocumentNumber = document.DocumentNumber,
            Title = document.Title,
            Description = document.Description,
            CategoryId = document.CategoryId,
            CategoryName = category.Name,
            DocumentTypeId = document.DocumentTypeId,
            DocumentTypeName = documentType.Name,
            Version = document.Version,
            Status = document.Status,
            CreatedBy = document.CreatedBy,
            CreatorName = $"{creator.LastName} {creator.FirstName}",
            CreatedAt = document.CreatedAt,
            ValidFrom = document.ValidFrom,
            ValidUntil = document.ValidUntil,
            FilePath = document.FilePath,
            FileSize = document.FileSize,
            FileExtension = document.FileExtension
        };
    }

    private static string GetMimeType(string extension)
    {
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".txt" => "text/plain",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => "application/octet-stream"
        };
    }
}

public class UpdateDocumentCommandHandler : IRequestHandler<UpdateDocumentCommand, DocumentDto>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IDocumentVersionRepository _versionRepository;
    private readonly IDocumentTypeRepository _documentTypeRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IFileService _fileService;

    public UpdateDocumentCommandHandler(
        IDocumentRepository documentRepository,
        IDocumentVersionRepository versionRepository,
        IDocumentTypeRepository documentTypeRepository,
        ICategoryRepository categoryRepository,
        IUserRepository userRepository,
        IFileService fileService)
    {
        _documentRepository = documentRepository;
        _versionRepository = versionRepository;
        _documentTypeRepository = documentTypeRepository;
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _fileService = fileService;
    }

    public async Task<DocumentDto> Handle(UpdateDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException("Документ не найден");

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId)
            ?? throw new KeyNotFoundException("Категория не найдена");

        var documentType = await _documentTypeRepository.GetByIdAsync(request.DocumentTypeId)
            ?? throw new KeyNotFoundException("Тип документа не найден");

        document.Title = request.Title;
        document.Description = request.Description;
        document.CategoryId = request.CategoryId;
        document.DocumentTypeId = request.DocumentTypeId;
        document.ValidFrom = request.ValidFrom;
        document.ValidUntil = request.ValidUntil;
        document.UpdatedBy = request.UpdatedBy;

        if (request.File != null && !string.IsNullOrEmpty(request.FileName))
        {
            var fileExtension = Path.GetExtension(request.FileName).ToLowerInvariant();
            if (!_fileService.IsAllowedExtension(fileExtension))
                throw new InvalidOperationException($"Формат файла {fileExtension} не поддерживается");

            var latestVersion = await _versionRepository.GetLatestAsync(document.Id);
            var newVersionNumber = latestVersion != null
                ? Math.Floor(latestVersion.VersionNumber) + 0.1m
                : 1.1m;

            var versionPath = await _fileService.SaveFileVersionAsync(
                request.File, request.FileName, document.Id, newVersionNumber);

            var version = new DocumentVersion
            {
                Id = Guid.NewGuid(),
                DocumentId = document.Id,
                VersionNumber = newVersionNumber,
                FilePath = versionPath,
                FileSize = request.File.Length,
                CreatedBy = request.UpdatedBy,
                ChangeComment = request.ChangeComment,
                CreatedAt = DateTime.UtcNow
            };

            await _versionRepository.AddAsync(version);

            document.FilePath = await _fileService.SaveFileAsync(request.File, request.FileName, document.Id);
            document.FileSize = request.File.Length;
            document.FileExtension = fileExtension;
            document.Version = newVersionNumber;
        }

        await _documentRepository.UpdateAsync(document);

        return new DocumentDto
        {
            Id = document.Id,
            DocumentNumber = document.DocumentNumber,
            Title = document.Title,
            Description = document.Description,
            CategoryId = document.CategoryId,
            CategoryName = category.Name,
            DocumentTypeId = document.DocumentTypeId,
            DocumentTypeName = documentType.Name,
            Version = document.Version,
            Status = document.Status,
            CreatedBy = document.CreatedBy,
            CreatedAt = document.CreatedAt,
            UpdatedAt = document.UpdatedAt,
            ValidFrom = document.ValidFrom,
            ValidUntil = document.ValidUntil,
            FilePath = document.FilePath,
            FileSize = document.FileSize,
            FileExtension = document.FileExtension
        };
    }
}

public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, bool>
{
    private readonly IDocumentRepository _documentRepository;

    public DeleteDocumentCommandHandler(IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<bool> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(request.Id);
        if (document == null) return false;

        document.IsDeleted = true;
        document.DeletedAt = DateTime.UtcNow;
        document.DeletedBy = request.DeletedBy;

        await _documentRepository.UpdateAsync(document);
        return true;
    }
}

public class RestoreDocumentCommandHandler : IRequestHandler<RestoreDocumentCommand, bool>
{
    private readonly IDocumentRepository _documentRepository;

    public RestoreDocumentCommandHandler(IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<bool> Handle(RestoreDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(request.Id);
        if (document == null || !document.IsDeleted) return false;

        document.IsDeleted = false;
        document.DeletedAt = null;
        document.DeletedBy = null;

        await _documentRepository.UpdateAsync(document);
        return true;
    }
}

public class PermanentDeleteDocumentCommandHandler : IRequestHandler<PermanentDeleteDocumentCommand, bool>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IFileService _fileService;

    public PermanentDeleteDocumentCommandHandler(IDocumentRepository documentRepository, IFileService fileService)
    {
        _documentRepository = documentRepository;
        _fileService = fileService;
    }

    public async Task<bool> Handle(PermanentDeleteDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(request.Id);
        if (document == null) return false;

        if (!string.IsNullOrEmpty(document.FilePath) && await _fileService.FileExistsAsync(document.FilePath))
        {
            await _fileService.DeleteFileAsync(document.FilePath);
        }

        await _documentRepository.DeleteAsync(document);
        return true;
    }
}

public class SetDocumentStatusCommandHandler : IRequestHandler<SetDocumentStatusCommand, bool>
{
    private readonly IDocumentRepository _documentRepository;

    public SetDocumentStatusCommandHandler(IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<bool> Handle(SetDocumentStatusCommand request, CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(request.Id);
        if (document == null) return false;

        document.Status = request.Status;
        if (request.Status == DocumentStatus.Approved && request.ApprovedBy.HasValue)
        {
            document.ApprovedBy = request.ApprovedBy;
            document.ApprovedAt = DateTime.UtcNow;
        }

        await _documentRepository.UpdateAsync(document);
        return true;
    }
}
