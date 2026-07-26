using CMDocumentRepository.Application.Commands;
using CMDocumentRepository.Application.DTOs;
using CMDocumentRepository.Application.Queries;
using CMDocumentRepository.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CMDocumentRepository.Presentation.Controllers;

[Authorize]
public class DocumentController : Controller
{
    private readonly IMediator _mediator;

    public DocumentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index(DocumentStatus? status, Guid? categoryId, Guid? documentTypeId)
    {
        var documents = await _mediator.Send(new GetAllDocumentsQuery
        {
            Status = status,
            CategoryId = categoryId,
            DocumentTypeId = documentTypeId
        });
        return View(documents);
    }

    public async Task<IActionResult> MyDocuments()
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        var documents = await _mediator.Send(new GetMyDocumentsQuery { UserId = userId.Value });
        return View("Index", documents);
    }

    public async Task<IActionResult> ForApproval()
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        var documents = await _mediator.Send(new GetDocumentsForApprovalQuery { UserId = userId.Value });
        return View("Index", documents);
    }

    public async Task<IActionResult> Trash()
    {
        var documents = await _mediator.Send(new GetDeletedDocumentsQuery());
        return View("Index", documents);
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

        var command = new CreateDocumentCommand
        {
            Title = model.Title,
            Description = model.Description,
            CategoryId = model.CategoryId,
            DocumentTypeId = model.DocumentTypeId,
            ValidFrom = model.ValidFrom,
            ValidUntil = model.ValidUntil,
            File = model.File,
            FileName = model.FileName,
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

        var command = new UpdateDocumentCommand
        {
            Id = id,
            Title = model.Title,
            Description = model.Description,
            CategoryId = model.CategoryId,
            DocumentTypeId = model.DocumentTypeId,
            ValidFrom = model.ValidFrom,
            ValidUntil = model.ValidUntil,
            File = model.File,
            FileName = model.FileName,
            ChangeComment = model.ChangeComment,
            UpdatedBy = userId.Value
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

    private Guid? GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? Guid.Parse(claim.Value) : null;
    }
}
