using CMDocumentRepository.Application.Commands;
using FluentValidation;

namespace CMDocumentRepository.Application.Validators;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Имя пользователя обязательно")
            .MaximumLength(100).WithMessage("Имя пользователя не должно превышать 100 символов");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обязателен")
            .MinimumLength(6).WithMessage("Пароль должен содержать минимум 6 символов");
    }
}

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Имя пользователя обязательно")
            .MaximumLength(100).WithMessage("Имя пользователя не должно превышать 100 символов")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("Имя пользователя может содержать только буквы, цифры и подчёркивание");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обязателен")
            .EmailAddress().WithMessage("Некорректный формат email")
            .MaximumLength(255).WithMessage("Email не должен превышать 255 символов");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обязателен")
            .MinimumLength(6).WithMessage("Пароль должен содержать минимум 6 символов");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Имя обязательно")
            .MaximumLength(100).WithMessage("Имя не должно превышать 100 символов");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Фамилия обязательна")
            .MaximumLength(100).WithMessage("Фамилия не должна превышать 100 символов");
    }
}

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID пользователя обязателен");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обязателен")
            .EmailAddress().WithMessage("Некорректный формат email");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Имя обязательно");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Фамилия обязательна");
    }
}

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("ID пользователя обязателен");

        RuleFor(x => x.OldPassword)
            .NotEmpty().WithMessage("Текущий пароль обязателен");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Новый пароль обязателен")
            .MinimumLength(6).WithMessage("Новый пароль должен содержать минимум 6 символов")
            .NotEqual(x => x.OldPassword).WithMessage("Новый пароль должен отличаться от текущего");
    }
}

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("ID пользователя обязателен");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Новый пароль обязателен")
            .MinimumLength(6).WithMessage("Новый пароль должен содержать минимум 6 символов");
    }
}
