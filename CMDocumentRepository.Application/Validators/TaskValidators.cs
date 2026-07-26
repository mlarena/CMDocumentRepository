using CMDocumentRepository.Application.Commands;
using FluentValidation;

namespace CMDocumentRepository.Application.Validators;

public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Название задачи обязательно")
            .MaximumLength(500).WithMessage("Название не должно превышать 500 символов");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Описание не должно превышать 2000 символов");

        RuleFor(x => x.CreatedBy)
            .NotEmpty().WithMessage("Создатель обязателен");
    }
}

public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID задачи обязателен");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Название задачи обязательно")
            .MaximumLength(500).WithMessage("Название не должно превышать 500 символов");

        RuleFor(x => x.UpdatedBy)
            .NotEmpty().WithMessage("Редактор обязателен");
    }
}

public class SendForApprovalCommandValidator : AbstractValidator<SendForApprovalCommand>
{
    public SendForApprovalCommandValidator()
    {
        RuleFor(x => x.DocumentId)
            .NotEmpty().WithMessage("ID документа обязателен");

        RuleFor(x => x.ApproverIds)
            .NotEmpty().WithMessage("Необходимо указать хотя бы одного согласующего");

        RuleFor(x => x.SentBy)
            .NotEmpty().WithMessage("ID отправителя обязателен");
    }
}

public class CreateDocumentTypeCommandValidator : AbstractValidator<CreateDocumentTypeCommand>
{
    public CreateDocumentTypeCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Название типа документа обязательно")
            .MaximumLength(100).WithMessage("Название не должно превышать 100 символов");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Код обязателен")
            .MaximumLength(50).WithMessage("Код не должен превышать 50 символов")
            .Matches("^[A-Za-z0-9_-]+$").WithMessage("Код может содержать только буквы, цифры, дефис и подчёркивание");
    }
}

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Название категории обязательно")
            .MaximumLength(100).WithMessage("Название не должно превышать 100 символов");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Код обязателен")
            .MaximumLength(50).WithMessage("Код не должен превышать 50 символов")
            .Matches("^[A-Za-z0-9_-]+$").WithMessage("Код может содержать только буквы, цифры, дефис и подчёркивание");
    }
}

public class SetDocumentPermissionCommandValidator : AbstractValidator<SetDocumentPermissionCommand>
{
    public SetDocumentPermissionCommandValidator()
    {
        RuleFor(x => x.DocumentId)
            .NotEmpty().WithMessage("ID документа обязателен");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("ID пользователя обязателен");
    }
}
