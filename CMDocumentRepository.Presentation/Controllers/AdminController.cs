using CMDocumentRepository.Application.Commands;
using CMDocumentRepository.Application.DTOs;
using CMDocumentRepository.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMDocumentRepository.Presentation.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class AdminController : Controller
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index()
    {
        var stats = await _mediator.Send(new GetDashboardStatsQuery());
        return View(stats);
    }

    public async Task<IActionResult> DocumentTypes()
    {
        var types = await _mediator.Send(new GetAllDocumentTypesQuery());
        return View(types);
    }

    [HttpGet]
    public IActionResult CreateDocumentType()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateDocumentType(CreateDocumentTypeDto model)
    {
        if (!ModelState.IsValid) return View(model);

        try
        {
            var command = new CreateDocumentTypeCommand
            {
                Name = model.Name,
                Code = model.Code,
                Description = model.Description
            };

            var type = await _mediator.Send(command);
            return RedirectToAction(nameof(DocumentTypes));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteDocumentType(Guid id)
    {
        await _mediator.Send(new DeleteDocumentTypeCommand { Id = id });
        return RedirectToAction(nameof(DocumentTypes));
    }

    public async Task<IActionResult> Categories()
    {
        var categories = await _mediator.Send(new GetAllCategoriesQuery());
        return View(categories);
    }

    [HttpGet]
    public IActionResult CreateCategory()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(CreateCategoryDto model)
    {
        if (!ModelState.IsValid) return View(model);

        try
        {
            var command = new CreateCategoryCommand
            {
                Name = model.Name,
                Code = model.Code,
                Description = model.Description,
                ParentCategoryId = model.ParentCategoryId
            };

            var category = await _mediator.Send(command);
            return RedirectToAction(nameof(Categories));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        await _mediator.Send(new DeleteCategoryCommand { Id = id });
        return RedirectToAction(nameof(Categories));
    }

    public async Task<IActionResult> AuditLogs()
    {
        return View();
    }
}
