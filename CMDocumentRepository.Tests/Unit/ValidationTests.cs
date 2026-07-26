using CMDocumentRepository.Application.Commands;
using CMDocumentRepository.Application.Validators;
using Xunit;

namespace CMDocumentRepository.Tests.Unit;

public class CreateUserCommandValidatorTests
{
    private readonly CreateUserCommandValidator _validator;

    public CreateUserCommandValidatorTests()
    {
        _validator = new CreateUserCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_NoErrors()
    {
        var command = new CreateUserCommand
        {
            UserName = "validuser",
            Email = "valid@example.com",
            Password = "password123",
            FirstName = "Иван",
            LastName = "Иванов"
        };

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_EmptyUserName_HasError()
    {
        var command = new CreateUserCommand
        {
            UserName = "",
            Email = "valid@example.com",
            Password = "password123",
            FirstName = "Иван",
            LastName = "Иванов"
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "UserName");
    }

    [Fact]
    public void Validate_InvalidEmail_HasError()
    {
        var command = new CreateUserCommand
        {
            UserName = "validuser",
            Email = "notanemail",
            Password = "password123",
            FirstName = "Иван",
            LastName = "Иванов"
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }

    [Fact]
    public void Validate_ShortPassword_HasError()
    {
        var command = new CreateUserCommand
        {
            UserName = "validuser",
            Email = "valid@example.com",
            Password = "12345",
            FirstName = "Иван",
            LastName = "Иванов"
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Password");
    }

    [Fact]
    public void Validate_EmptyFirstName_HasError()
    {
        var command = new CreateUserCommand
        {
            UserName = "validuser",
            Email = "valid@example.com",
            Password = "password123",
            FirstName = "",
            LastName = "Иванов"
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "FirstName");
    }
}

public class CreateDocumentCommandValidatorTests
{
    private readonly CreateDocumentCommandValidator _validator;

    public CreateDocumentCommandValidatorTests()
    {
        _validator = new CreateDocumentCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_NoErrors()
    {
        var command = new CreateDocumentCommand
        {
            Title = "Тестовый документ",
            CategoryId = Guid.NewGuid(),
            DocumentTypeId = Guid.NewGuid(),
            CreatedBy = Guid.NewGuid()
        };

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_EmptyTitle_HasError()
    {
        var command = new CreateDocumentCommand
        {
            Title = "",
            CategoryId = Guid.NewGuid(),
            DocumentTypeId = Guid.NewGuid(),
            CreatedBy = Guid.NewGuid()
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Title");
    }

    [Fact]
    public void Validate_EmptyCategoryId_HasError()
    {
        var command = new CreateDocumentCommand
        {
            Title = "Документ",
            CategoryId = Guid.Empty,
            DocumentTypeId = Guid.NewGuid(),
            CreatedBy = Guid.NewGuid()
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "CategoryId");
    }
}

public class CreateTaskCommandValidatorTests
{
    private readonly CreateTaskCommandValidator _validator;

    public CreateTaskCommandValidatorTests()
    {
        _validator = new CreateTaskCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_NoErrors()
    {
        var command = new CreateTaskCommand
        {
            Title = "Тестовая задача",
            CreatedBy = Guid.NewGuid()
        };

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_EmptyTitle_HasError()
    {
        var command = new CreateTaskCommand
        {
            Title = "",
            CreatedBy = Guid.NewGuid()
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Title");
    }

    [Fact]
    public void Validate_EmptyCreator_HasError()
    {
        var command = new CreateTaskCommand
        {
            Title = "Задача",
            CreatedBy = Guid.Empty
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "CreatedBy");
    }
}
