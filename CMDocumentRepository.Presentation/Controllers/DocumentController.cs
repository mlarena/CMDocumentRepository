using CMDocumentRepository.Application.Commands;
using CMDocumentRepository.Application.DTOs;
using CMDocumentRepository.Application.Queries;
using CMDocumentRepository.Domain.Enums;
using CMDocumentRepository.Domain.Interfaces;
using CMDocumentRepository.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CMDocumentRepository.Presentation.Controllers;

[Authorize]
public class DocumentController : Controller
{
    private readonly IMediator _mediator;
    private readonly IExportService _exportService;
    private readonly IFileService _fileService;

    public DocumentController(IMediator mediator, IExportService exportService, IFileService fileService)
    {
        _mediator = mediator;
        _exportService = exportService;
        _fileService = fileService;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, DocumentStatus? status = null, Guid? categoryId = null, Guid? documentTypeId = null, string? keyword = null)
    {
        var result = await _mediator.Send(new GetPagedDocumentsQuery
        {
            PageNumber = page,
            PageSize = pageSize,
            Keyword = keyword,
            Status = status,
            CategoryId = categoryId,
            DocumentTypeId = documentTypeId
        });

        ViewBag.DocumentTypes = await _mediator.Send(new GetAllDocumentTypesQuery());
        ViewBag.Categories = await _mediator.Send(new GetAllCategoriesQuery());
        ViewBag.CurrentStatus = status;
        ViewBag.CurrentCategoryId = categoryId;
        ViewBag.CurrentDocumentTypeId = documentTypeId;
        ViewBag.CurrentKeyword = keyword;

        return View(result);
    }

    public async Task<IActionResult> MyDocuments()
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        var documents = await _mediator.Send(new GetMyDocumentsQuery { UserId = userId.Value });
        var result = PagedResult<DocumentDto>.Create(documents, documents.Count, 1, documents.Count);
        return View("Index", result);
    }

    public async Task<IActionResult> ForApproval()
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        var documents = await _mediator.Send(new GetDocumentsForApprovalQuery { UserId = userId.Value });
        var result = PagedResult<DocumentDto>.Create(documents, documents.Count, 1, documents.Count);
        return View("Index", result);
    }

    public async Task<IActionResult> Trash()
    {
        var documents = await _mediator.Send(new GetDeletedDocumentsQuery());
        var result = PagedResult<DocumentDto>.Create(documents, documents.Count, 1, documents.Count);
        return View("Index", result);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var document = await _mediator.Send(new GetDocumentByIdQuery { Id = id });
        if (document == null) return NotFound();

        var versions = await _mediator.Send(new GetDocumentVersionsQuery { DocumentId = id });
        ViewBag.Versions = versions;

        return View(document);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.DocumentTypes = await _mediator.Send(new GetAllDocumentTypesQuery());
        ViewBag.Categories = await _mediator.Send(new GetAllCategoriesQuery());
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateDocumentDto model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.DocumentTypes = await _mediator.Send(new GetAllDocumentTypesQuery());
            ViewBag.Categories = await _mediator.Send(new GetAllCategoriesQuery());
            return View(model);
        }

        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        Stream? fileStream = null;
        var fileName = model.FileName;
        var uploadedFile = Request.Form.Files.GetFile("File");
        if (uploadedFile != null && uploadedFile.Length > 0)
        {
            fileStream = uploadedFile.OpenReadStream();
            fileName = uploadedFile.FileName;
        }

        var command = new CreateDocumentCommand
        {
            Title = model.Title,
            Description = model.Description,
            CategoryId = model.CategoryId,
            DocumentTypeId = model.DocumentTypeId,
            ValidFrom = model.ValidFrom,
            ValidUntil = model.ValidUntil,
            File = fileStream,
            FileName = fileName,
            CreatedBy = userId.Value
        };

        var document = await _mediator.Send(command);
        return RedirectToAction(nameof(Details), new { id = document.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var document = await _mediator.Send(new GetDocumentByIdQuery { Id = id });
        if (document == null) return NotFound();

        ViewBag.DocumentTypes = await _mediator.Send(new GetAllDocumentTypesQuery());
        ViewBag.Categories = await _mediator.Send(new GetAllCategoriesQuery());

        var model = new UpdateDocumentDto
        {
            Title = document.Title,
            Description = document.Description,
            CategoryId = document.CategoryId,
            DocumentTypeId = document.DocumentTypeId,
            ValidFrom = document.ValidFrom,
            ValidUntil = document.ValidUntil
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateDocumentDto model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.DocumentTypes = await _mediator.Send(new GetAllDocumentTypesQuery());
            ViewBag.Categories = await _mediator.Send(new GetAllCategoriesQuery());
            return View(model);
        }

        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        Stream? fileStream = null;
        var fileName = model.FileName;
        var uploadedFile = Request.Form.Files.GetFile("File");
        if (uploadedFile != null && uploadedFile.Length > 0)
        {
            fileStream = uploadedFile.OpenReadStream();
            fileName = uploadedFile.FileName;
        }

        var command = new UpdateDocumentCommand
        {
            Id = id,
            Title = model.Title,
            Description = model.Description,
            CategoryId = model.CategoryId,
            DocumentTypeId = model.DocumentTypeId,
            ValidFrom = model.ValidFrom,
            ValidUntil = model.ValidUntil,
            File = fileStream,
            FileName = fileName,
            ChangeComment = model.ChangeComment,
            UpdatedBy = userId.Value,
            IsMajorVersion = model.IsMajorVersion
        };

        var document = await _mediator.Send(command);
        return RedirectToAction(nameof(Details), new { id = document.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        await _mediator.Send(new DeleteDocumentCommand { Id = id, DeletedBy = userId.Value });
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(Guid id)
    {
        await _mediator.Send(new RestoreDocumentCommand { Id = id });
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Download(Guid id)
    {
        var document = await _mediator.Send(new GetDocumentByIdQuery { Id = id });
        if (document == null || string.IsNullOrEmpty(document.FilePath))
            return NotFound();

        var stream = await _fileService.GetFileStreamAsync(document.FilePath);
        var storedFileName = Path.GetFileName(document.FilePath);
        // Убираем префикс GUID_, чтобы вернуть оригинальное имя файла
        var underscoreIdx = storedFileName.IndexOf('_');
        var downloadName = underscoreIdx >= 0 ? storedFileName[(underscoreIdx + 1)..] : storedFileName;

        return File(stream, "application/octet-stream", downloadName);
    }

    [HttpGet]
    public async Task<IActionResult> DownloadVersion(Guid documentId, decimal versionNumber)
    {
        var versions = await _mediator.Send(new GetDocumentVersionsQuery { DocumentId = documentId });
        var version = versions.FirstOrDefault(v => v.VersionNumber == versionNumber);
        if (version == null || string.IsNullOrEmpty(version.FilePath))
            return NotFound();

        var stream = await _fileService.GetFileStreamAsync(version.FilePath);
        var storedFileName = Path.GetFileName(version.FilePath);
        // Убираем префикс vX_Y_ из имени файловой версии
        var underscoreIdx = storedFileName.IndexOf('_');
        var downloadName = underscoreIdx >= 0 ? storedFileName[(underscoreIdx + 1)..] : storedFileName;
        underscoreIdx = downloadName.IndexOf('_');
        if (underscoreIdx >= 0) downloadName = downloadName[(underscoreIdx + 1)..];

        return File(stream, "application/octet-stream", downloadName);
    }

    public async Task<IActionResult> Gantt()
    {
        var data = await _mediator.Send(new GetGanttDataQuery());
        return View(data);
    }

    public async Task<IActionResult> Export(DocumentStatus? status, Guid? categoryId, Guid? documentTypeId, string? keyword)
    {
        var result = await _mediator.Send(new GetPagedDocumentsQuery
        {
            PageNumber = 1,
            PageSize = 1000,
            Keyword = keyword,
            Status = status,
            CategoryId = categoryId,
            DocumentTypeId = documentTypeId
        });

        var excelBytes = _exportService.ExportDocumentsToExcel(result.Items);
        return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"documents_{DateTime.Now:yyyyMMdd}.xlsx");
    }

    private Guid? GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? Guid.Parse(claim.Value) : null;
    }
}
