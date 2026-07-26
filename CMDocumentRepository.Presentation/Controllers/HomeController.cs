using CMDocumentRepository.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CMDocumentRepository.Presentation.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IMediator _mediator;

    public HomeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        if (userId.HasValue)
        {
            var documents = await _mediator.Send(new GetMyDocumentsQuery { UserId = userId.Value });
            var tasks = await _mediator.Send(new GetMyTasksQuery { UserId = userId.Value });
            var approvals = await _mediator.Send(new GetPendingApprovalsQuery { ApproverId = userId.Value });

            ViewBag.MyDocuments = documents.Count;
            ViewBag.MyTasks = tasks.Count;
            ViewBag.PendingApprovals = approvals.Count;
        }

        return View();
    }

    private Guid? GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? Guid.Parse(claim.Value) : null;
    }
}
