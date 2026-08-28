using CMDocumentRepository.Application.Commands;
using CMDocumentRepository.Application.DTOs;
using CMDocumentRepository.Application.Queries;
using CMDocumentRepository.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMDocumentRepository.Presentation.Controllers;

[Authorize]
public class UserController : Controller
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index(UserRole? role, bool? isActive)
    {
        var users = await _mediator.Send(new GetAllUsersQuery { Role = role, IsActive = isActive });
        return View(users);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        // Обычные пользователи видят только свой профиль
        var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (currentUserId != null && !User.IsInRole("SuperAdmin") && !User.IsInRole("Admin"))
        {
            if (id != Guid.Parse(currentUserId))
                return Forbid();
        }

        var user = await _mediator.Send(new GetUserByIdQuery { Id = id });
        if (user == null) return NotFound();
        return View(user);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserDto model)
    {
        if (!ModelState.IsValid) return View(model);

        try
        {
            var command = new CreateUserCommand
            {
                UserName = model.UserName,
                Email = model.Email,
                Password = model.Password,
                FirstName = model.FirstName,
                LastName = model.LastName,
                MiddleName = model.MiddleName,
                Role = model.Role
            };

            var user = await _mediator.Send(command);
            return RedirectToAction(nameof(Details), new { id = user.Id });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var user = await _mediator.Send(new GetUserByIdQuery { Id = id });
        if (user == null) return NotFound();

        var model = new UpdateUserDto
        {
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            MiddleName = user.MiddleName,
            Role = user.Role,
            IsActive = user.IsActive
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateUserDto model)
    {
        if (!ModelState.IsValid) return View(model);

        var command = new UpdateUserCommand
        {
            Id = id,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            MiddleName = model.MiddleName,
            Role = model.Role,
            IsActive = model.IsActive
        };

        var user = await _mediator.Send(command);
        return RedirectToAction(nameof(Details), new { id = user.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteUserCommand { Id = id });
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleLock(Guid id, bool lockUser, int? lockMinutes)
    {
        await _mediator.Send(new ToggleUserLockCommand { Id = id, Lock = lockUser, LockMinutes = lockMinutes });
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(Guid id, string newPassword)
    {
        await _mediator.Send(new ResetPasswordCommand { UserId = id, NewPassword = newPassword });
        TempData["Success"] = "Пароль успешно сброшен";
        return RedirectToAction(nameof(Details), new { id });
    }
}
