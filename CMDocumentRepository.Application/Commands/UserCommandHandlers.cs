using CMDocumentRepository.Application.DTOs;
using CMDocumentRepository.Domain.Entities;
using CMDocumentRepository.Domain.Enums;
using CMDocumentRepository.Domain.Interfaces;
using MediatR;

namespace CMDocumentRepository.Application.Commands;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.UserNameExistsAsync(request.UserName))
            throw new InvalidOperationException("Пользователь с таким именем уже существует");

        if (await _userRepository.EmailExistsAsync(request.Email))
            throw new InvalidOperationException("Пользователь с таким email уже существует");

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = request.UserName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            MiddleName = request.MiddleName,
            Role = request.Role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            MiddleName = user.MiddleName,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException("Пользователь не найден");

        user.Email = request.Email;
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.MiddleName = request.MiddleName;
        user.Role = request.Role;
        user.IsActive = request.IsActive;

        await _userRepository.UpdateAsync(user);

        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            MiddleName = user.MiddleName,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);
        if (user == null) return false;

        if (user.Role == UserRole.SuperAdmin)
            throw new InvalidOperationException("Невозможно удалить суперпользователя");

        await _userRepository.DeleteAsync(user);
        return true;
    }
}

public class ToggleUserLockCommandHandler : IRequestHandler<ToggleUserLockCommand, bool>
{
    private readonly IUserRepository _userRepository;

    public ToggleUserLockCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(ToggleUserLockCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);
        if (user == null) return false;

        if (user.Role == UserRole.SuperAdmin)
            throw new InvalidOperationException("Невозможно заблокировать суперпользователя");

        user.IsLocked = request.Lock;
        user.LockedUntil = request.Lock ? DateTime.UtcNow.AddMinutes(request.LockMinutes ?? 15) : null;

        await _userRepository.UpdateAsync(user);
        return true;
    }
}
