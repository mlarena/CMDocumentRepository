using CMDocumentRepository.Application.Commands;
using FluentValidation;

namespace CMDocumentRepository.Application.Validators;

public class CreateDocumentCommandValidator : AbstractValidator<CreateDocumentCommand>
{
    public CreateDocumentCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Название документа обязательно")
            .MaximumLength(500).WithMessage("Название не должно превышать 500 символов");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Описание не должно превышать 2000 символов");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Категория обязательна");

        RuleFor(x => x.DocumentTypeId)
            .NotEmpty().WithMessage("Тип документа обязателен");

        RuleFor(x => x.CreatedBy)
            .NotEmpty().WithMessage("Автор обязателен");

        RuleFor(x => x.ValidUntil)
            .GreaterThan(x => x.ValidFrom)
            .When(x => x.ValidFrom.HasValue && x.ValidUntil.HasValue)
            .WithMessage("Дата окончания должна быть позже даты начала");
    }
}

public class UpdateDocumentCommandValidator : AbstractValidator<UpdateDocumentCommand>
{
    public UpdateDocumentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID документа обязателен");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Название документа обязательно")
            .MaximumLength(500).WithMessage("Название не должно превышать 500 символов");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Категория обязательна");

        RuleFor(x => x.DocumentTypeId)
            .NotEmpty().WithMessage("Тип документа обязателен");

        RuleFor(x => x.UpdatedBy)
            .NotEmpty().WithMessage("Редактор обязателен");
    }
}

public class DeleteDocumentCommandValidator : AbstractValidator<DeleteDocumentCommand>
{
    public DeleteDocumentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID документа обязателен");

        RuleFor(x => x.DeletedBy)
            .NotEmpty().WithMessage("ID удаляющего обязателен");
    }
}
