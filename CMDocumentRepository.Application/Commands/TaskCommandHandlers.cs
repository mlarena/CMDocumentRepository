using CMDocumentRepository.Application.DTOs;
using CMDocumentRepository.Domain.Entities;
using CMDocumentRepository.Domain.Enums;
using CMDocumentRepository.Domain.Interfaces;
using MediatR;

namespace CMDocumentRepository.Application.Commands;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskDto>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;

    public CreateTaskCommandHandler(ITaskRepository taskRepository, IUserRepository userRepository)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
    }

    public async Task<TaskDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var creator = await _userRepository.GetByIdAsync(request.CreatedBy)
            ?? throw new KeyNotFoundException("Создатель не найден");

        var task = new AppTask
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            Status = AppTaskStatus.Backlog,
            AssigneeId = request.AssigneeId,
            CreatedBy = request.CreatedBy,
            DueDate = request.DueDate,
            OrderNumber = 0,
            CreatedAt = DateTime.UtcNow
        };

        await _taskRepository.AddAsync(task);

        string? assigneeName = null;
        if (request.AssigneeId.HasValue)
        {
            var assignee = await _userRepository.GetByIdAsync(request.AssigneeId.Value);
            if (assignee != null)
                assigneeName = $"{assignee.LastName} {assignee.FirstName}";
        }

        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Priority = task.Priority,
            Status = task.Status,
            AssigneeId = task.AssigneeId,
            AssigneeName = assigneeName,
            CreatedBy = task.CreatedBy,
            CreatorName = $"{creator.LastName} {creator.FirstName}",
            CreatedAt = task.CreatedAt,
            DueDate = task.DueDate,
            OrderNumber = task.OrderNumber
        };
    }
}

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, TaskDto>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;

    public UpdateTaskCommandHandler(ITaskRepository taskRepository, IUserRepository userRepository)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
    }

    public async Task<TaskDto> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException("Задача не найдена");

        task.Title = request.Title;
        task.Description = request.Description;
        task.Priority = request.Priority;
        task.Status = request.Status;
        task.AssigneeId = request.AssigneeId;
        task.DueDate = request.DueDate;

        await _taskRepository.UpdateAsync(task);

        var creator = await _userRepository.GetByIdAsync(task.CreatedBy);
        string? assigneeName = null;
        if (task.AssigneeId.HasValue)
        {
            var assignee = await _userRepository.GetByIdAsync(task.AssigneeId.Value);
            if (assignee != null)
                assigneeName = $"{assignee.LastName} {assignee.FirstName}";
        }

        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Priority = task.Priority,
            Status = task.Status,
            AssigneeId = task.AssigneeId,
            AssigneeName = assigneeName,
            CreatedBy = task.CreatedBy,
            CreatorName = creator != null ? $"{creator.LastName} {creator.FirstName}" : string.Empty,
            CreatedAt = task.CreatedAt,
            DueDate = task.DueDate,
            OrderNumber = task.OrderNumber
        };
    }
}

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, bool>
{
    private readonly ITaskRepository _taskRepository;

    public DeleteTaskCommandHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<bool> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.Id);
        if (task == null) return false;

        await _taskRepository.DeleteAsync(task);
        return true;
    }
}

public class MoveTaskCommandHandler : IRequestHandler<MoveTaskCommand, bool>
{
    private readonly ITaskRepository _taskRepository;

    public MoveTaskCommandHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<bool> Handle(MoveTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.TaskId);
        if (task == null) return false;

        await _taskRepository.UpdateOrderAsync(request.TaskId, request.NewOrder, request.NewStatus);
        return true;
    }
}
