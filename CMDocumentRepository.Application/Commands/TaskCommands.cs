using CMDocumentRepository.Application.DTOs;
using MediatR;

namespace CMDocumentRepository.Application.Commands;

public record CreateTaskCommand : IRequest<TaskDto>
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public CMDocumentRepository.Domain.Enums.TaskPriority Priority { get; init; } = CMDocumentRepository.Domain.Enums.TaskPriority.Medium;
    public Guid? AssigneeId { get; init; }
    public DateTime? DueDate { get; init; }
    public Guid CreatedBy { get; init; }
}

public record UpdateTaskCommand : IRequest<TaskDto>
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public CMDocumentRepository.Domain.Enums.TaskPriority Priority { get; init; }
    public CMDocumentRepository.Domain.Enums.AppTaskStatus Status { get; init; }
    public Guid? AssigneeId { get; init; }
    public DateTime? DueDate { get; init; }
    public Guid UpdatedBy { get; init; }
}

public record DeleteTaskCommand : IRequest<bool>
{
    public Guid Id { get; init; }
}

public record MoveTaskCommand : IRequest<bool>
{
    public Guid TaskId { get; init; }
    public int NewOrder { get; init; }
    public CMDocumentRepository.Domain.Enums.AppTaskStatus NewStatus { get; init; }
    public Guid MovedBy { get; init; }
}
