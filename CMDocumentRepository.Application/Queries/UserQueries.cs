using CMDocumentRepository.Application.DTOs;
using CMDocumentRepository.Domain.Enums;
using MediatR;

namespace CMDocumentRepository.Application.Queries;

public record GetUserByIdQuery : IRequest<UserDto?>
{
    public Guid Id { get; init; }
}

public record GetAllUsersQuery : IRequest<List<UserDto>>
{
    public UserRole? Role { get; init; }
    public bool? IsActive { get; init; }
}

public record GetUserByUserNameQuery : IRequest<UserDto?>
{
    public string UserName { get; init; } = string.Empty;
}
