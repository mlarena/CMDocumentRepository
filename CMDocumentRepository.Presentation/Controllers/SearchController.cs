using CMDocumentRepository.Application.Queries;
using CMDocumentRepository.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMDocumentRepository.Presentation.Controllers;

[Authorize]
public class SearchController : Controller
{
    private readonly IMediator _mediator;

    public SearchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Results(string? keyword, DocumentStatus? status, Guid? categoryId, Guid? documentTypeId, DateTime? dateFrom, DateTime? dateTo)
    {
        var documents = await _mediator.Send(new SearchDocumentsQuery
        {
            Keyword = keyword,
            Status = status,
            CategoryId = categoryId,
            DocumentTypeId = documentTypeId,
            DateFrom = dateFrom,
            DateTo = dateTo
        });

        ViewBag.Keyword = keyword;
        ViewBag.Status = status;
        ViewBag.CategoryId = categoryId;
        ViewBag.DocumentTypeId = documentTypeId;
        ViewBag.DateFrom = dateFrom;
        ViewBag.DateTo = dateTo;

        return View(documents);
    }
}
