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
public class TaskController : Controller
{
    private readonly IMediator _mediator;

    public TaskController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index(AppTaskStatus? status, Guid? assigneeId, TaskPriority? priority)
    {
        var tasks = await _mediator.Send(new GetAllTasksQuery
        {
            Status = status,
            AssigneeId = assigneeId,
            Priority = priority
        });
        return View(tasks);
    }

    public async Task<IActionResult> Kanban()
    {
        var allTasks = await _mediator.Send(new GetAllTasksQuery());
        return View(allTasks);
    }

    [HttpPost]
    public async Task<IActionResult> Move([FromBody] MoveTaskDto model)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return Unauthorized();

        await _mediator.Send(new MoveTaskCommand
        {
            TaskId = model.TaskId,
            NewOrder = model.NewOrder,
            NewStatus = model.NewStatus,
            MovedBy = userId.Value
        });

        return Ok();
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var task = await _mediator.Send(new GetTaskByIdQuery { Id = id });
        if (task == null) return NotFound();
        return View(task);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTaskDto model)
    {
        if (!ModelState.IsValid) return View(model);

        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        var command = new CreateTaskCommand
        {
            Title = model.Title,
            Description = model.Description,
            Priority = model.Priority,
            AssigneeId = model.AssigneeId,
            DueDate = model.DueDate,
            CreatedBy = userId.Value
        };

        var task = await _mediator.Send(command);
        return RedirectToAction(nameof(Details), new { id = task.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var task = await _mediator.Send(new GetTaskByIdQuery { Id = id });
        if (task == null) return NotFound();

        var model = new UpdateTaskDto
        {
            Title = task.Title,
            Description = task.Description,
            Priority = task.Priority,
            Status = task.Status,
            AssigneeId = task.AssigneeId,
            DueDate = task.DueDate
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateTaskDto model)
    {
        if (!ModelState.IsValid) return View(model);

        var userId = GetUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Account");

        var command = new UpdateTaskCommand
        {
            Id = id,
            Title = model.Title,
            Description = model.Description,
            Priority = model.Priority,
            Status = model.Status,
            AssigneeId = model.AssigneeId,
            DueDate = model.DueDate,
            UpdatedBy = userId.Value
        };

        var task = await _mediator.Send(command);
        return RedirectToAction(nameof(Details), new { id = task.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteTaskCommand { Id = id });
        return RedirectToAction(nameof(Index));
    }

    private Guid? GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? Guid.Parse(claim.Value) : null;
    }
}
