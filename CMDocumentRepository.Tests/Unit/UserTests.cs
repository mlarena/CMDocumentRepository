using CMDocumentRepository.Application.Commands;
using CMDocumentRepository.Domain.Entities;
using CMDocumentRepository.Domain.Enums;
using CMDocumentRepository.Domain.Interfaces;
using CMDocumentRepository.Tests.Helpers;
using Moq;
using Xunit;

namespace CMDocumentRepository.Tests.Unit;

public class CreateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new CreateUserCommandHandler(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsUserDto()
    {
        var command = new CreateUserCommand
        {
            UserName = "newuser",
            Email = "new@example.com",
            Password = "password123",
            FirstName = "Новый",
            LastName = "Пользователь",
            Role = UserRole.User
        };

        _userRepositoryMock.Setup(r => r.UserNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);
        _userRepositoryMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);
        _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("newuser", result.UserName);
        Assert.Equal("new@example.com", result.Email);
        Assert.Equal(UserRole.User, result.Role);
    }

    [Fact]
    public async Task Handle_DuplicateUserName_ThrowsException()
    {
        var command = new CreateUserCommand
        {
            UserName = "existing",
            Email = "new@example.com",
            Password = "password123",
            FirstName = "Новый",
            LastName = "Пользователь"
        };

        _userRepositoryMock.Setup(r => r.UserNameExistsAsync("existing"))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ThrowsException()
    {
        var command = new CreateUserCommand
        {
            UserName = "newuser",
            Email = "existing@example.com",
            Password = "password123",
            FirstName = "Новый",
            LastName = "Пользователь"
        };

        _userRepositoryMock.Setup(r => r.UserNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);
        _userRepositoryMock.Setup(r => r.EmailExistsAsync("existing@example.com"))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));
    }
}

public class DeleteUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly DeleteUserCommandHandler _handler;

    public DeleteUserCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new DeleteUserCommandHandler(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingUser_ReturnsTrue()
    {
        var user = TestData.CreateTestUser();
        _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id))
            .ReturnsAsync(user);
        _userRepositoryMock.Setup(r => r.DeleteAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(
            new DeleteUserCommand { Id = user.Id }, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_NonExistingUser_ReturnsFalse()
    {
        _userRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((User?)null);

        var result = await _handler.Handle(
            new DeleteUserCommand { Id = Guid.NewGuid() }, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task Handle_SuperAdmin_ThrowsException()
    {
        var superAdmin = TestData.CreateTestUser(role: UserRole.SuperAdmin);
        _userRepositoryMock.Setup(r => r.GetByIdAsync(superAdmin.Id))
            .ReturnsAsync(superAdmin);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(
                new DeleteUserCommand { Id = superAdmin.Id }, CancellationToken.None));
    }
}
