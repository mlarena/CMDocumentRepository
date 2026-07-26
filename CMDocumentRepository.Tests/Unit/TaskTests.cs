using CMDocumentRepository.Application.Commands;
using CMDocumentRepository.Domain.Entities;
using CMDocumentRepository.Domain.Enums;
using CMDocumentRepository.Domain.Interfaces;
using CMDocumentRepository.Tests.Helpers;
using Moq;
using Xunit;

namespace CMDocumentRepository.Tests.Unit;

public class CreateTaskCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly CreateTaskCommandHandler _handler;

    public CreateTaskCommandHandlerTests()
    {
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new CreateTaskCommandHandler(_taskRepositoryMock.Object, _userRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsTaskDto()
    {
        var user = TestData.CreateTestUser();
        _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id))
            .ReturnsAsync(user);
        _taskRepositoryMock.Setup(r => r.AddAsync(It.IsAny<AppTask>()))
            .ReturnsAsync((AppTask t) => t);

        var command = new CreateTaskCommand
        {
            Title = "Новая задача",
            Priority = TaskPriority.High,
            CreatedBy = user.Id
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Новая задача", result.Title);
        Assert.Equal(TaskPriority.High, result.Priority);
        Assert.Equal(AppTaskStatus.Backlog, result.Status);
    }

    [Fact]
    public async Task Handle_InvalidCreator_ThrowsException()
    {
        _userRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((User?)null);

        var command = new CreateTaskCommand
        {
            Title = "Задача",
            CreatedBy = Guid.NewGuid()
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));
    }
}

public class MoveTaskCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly MoveTaskCommandHandler _handler;

    public MoveTaskCommandHandlerTests()
    {
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _handler = new MoveTaskCommandHandler(_taskRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingTask_ReturnsTrue()
    {
        var task = TestData.CreateTestTask();
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(task.Id))
            .ReturnsAsync(task);
        _taskRepositoryMock.Setup(r => r.UpdateOrderAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<AppTaskStatus>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(
            new MoveTaskCommand
            {
                TaskId = task.Id,
                NewOrder = 1,
                NewStatus = AppTaskStatus.InProgress,
                MovedBy = Guid.NewGuid()
            }, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_NonExistingTask_ReturnsFalse()
    {
        _taskRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((AppTask?)null);

        var result = await _handler.Handle(
            new MoveTaskCommand
            {
                TaskId = Guid.NewGuid(),
                NewOrder = 1,
                NewStatus = AppTaskStatus.InProgress,
                MovedBy = Guid.NewGuid()
            }, CancellationToken.None);

        Assert.False(result);
    }
}
