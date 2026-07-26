using CMDocumentRepository.Application.Commands;
using CMDocumentRepository.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CMDocumentRepository.Presentation.Controllers;

[Authorize]
public class ApprovalController : Controller
{
    private readonly IMediator _mediator;

    public ApprovalController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        var approvals = await _mediator.Send(new GetPendingApprovalsQuery { ApproverId = userId.Value });
        return View(approvals);
    }

    public async Task<IActionResult> History()
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        var approvals = await _mediator.Send(new GetMyApprovalsQuery { ApproverId = userId.Value });
        return View("Index", approvals);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(Guid approvalId, string? comment)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        await _mediator.Send(new ApproveDocumentCommand
        {
            ApprovalId = approvalId,
            Comment = comment,
            ApproverId = userId.Value
        });

        TempData["Success"] = "Документ согласован";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(Guid approvalId, string comment)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        await _mediator.Send(new RejectDocumentCommand
        {
            ApprovalId = approvalId,
            Comment = comment,
            ApproverId = userId.Value
        });

        TempData["Success"] = "Документ отклонён";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RequestRework(Guid approvalId, string? comment)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        await _mediator.Send(new RequestReworkCommand
        {
            ApprovalId = approvalId,
            Comment = comment,
            ApproverId = userId.Value
        });

        TempData["Success"] = "Запрошена доработка";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendForApproval(Guid documentId, List<Guid> approverIds)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        await _mediator.Send(new SendForApprovalCommand
        {
            DocumentId = documentId,
            ApproverIds = approverIds,
            SentBy = userId.Value
        });

        TempData["Success"] = "Документ отправлен на согласование";
        return RedirectToAction("Details", "Document", new { id = documentId });
    }

    private Guid? GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? Guid.Parse(claim.Value) : null;
    }
}
