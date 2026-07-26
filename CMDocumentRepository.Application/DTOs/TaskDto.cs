using CMDocumentRepository.Domain.Enums;

namespace CMDocumentRepository.Application.DTOs;

public record TaskDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public TaskPriority Priority { get; init; }
    public AppTaskStatus Status { get; init; }
    public Guid? AssigneeId { get; init; }
    public string? AssigneeName { get; init; }
    public Guid CreatedBy { get; init; }
    public string CreatorName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? DueDate { get; init; }
    public int OrderNumber { get; init; }
}

public record CreateTaskDto
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public TaskPriority Priority { get; init; } = TaskPriority.Medium;
    public Guid? AssigneeId { get; init; }
    public DateTime? DueDate { get; init; }
}

public record UpdateTaskDto
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public TaskPriority Priority { get; init; }
    public AppTaskStatus Status { get; init; }
    public Guid? AssigneeId { get; init; }
    public DateTime? DueDate { get; init; }
}

public record MoveTaskDto
{
    public Guid TaskId { get; init; }
    public int NewOrder { get; init; }
    public AppTaskStatus NewStatus { get; init; }
}
