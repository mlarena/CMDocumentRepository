using CMDocumentRepository.Application.DTOs;
using MediatR;

namespace CMDocumentRepository.Application.Commands;

public record CreateUserCommand : IRequest<UserDto>
{
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? MiddleName { get; init; }
    public Domain.Enums.UserRole Role { get; init; } = Domain.Enums.UserRole.User;
}

public record UpdateUserCommand : IRequest<UserDto>
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? MiddleName { get; init; }
    public Domain.Enums.UserRole Role { get; init; }
    public bool IsActive { get; init; }
}

public record DeleteUserCommand : IRequest<bool>
{
    public Guid Id { get; init; }
}

public record ToggleUserLockCommand : IRequest<bool>
{
    public Guid Id { get; init; }
    public bool Lock { get; init; }
    public int? LockMinutes { get; init; }
}
