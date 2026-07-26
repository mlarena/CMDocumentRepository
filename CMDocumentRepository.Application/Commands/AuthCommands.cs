using CMDocumentRepository.Application.DTOs;
using MediatR;

namespace CMDocumentRepository.Application.Commands;

public record LoginCommand : IRequest<AuthResponseDto>
{
    public string UserName { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public record RefreshTokenCommand : IRequest<AuthResponseDto>
{
    public string RefreshToken { get; init; } = string.Empty;
}

public record LogoutCommand : IRequest<bool>
{
    public Guid UserId { get; init; }
}

public record ChangePasswordCommand : IRequest<bool>
{
    public Guid UserId { get; init; }
    public string OldPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
}

public record ResetPasswordCommand : IRequest<bool>
{
    public Guid UserId { get; init; }
    public string NewPassword { get; init; } = string.Empty;
}
