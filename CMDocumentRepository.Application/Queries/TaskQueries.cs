using CMDocumentRepository.Application.DTOs;
using CMDocumentRepository.Domain.Enums;
using MediatR;

namespace CMDocumentRepository.Application.Queries;

public record GetTaskByIdQuery : IRequest<TaskDto?>
{
    public Guid Id { get; init; }
}

public record GetAllTasksQuery : IRequest<List<TaskDto>>
{
    public AppTaskStatus? Status { get; init; }
    public Guid? AssigneeId { get; init; }
    public TaskPriority? Priority { get; init; }
}

public record GetTasksByStatusQuery : IRequest<List<TaskDto>>
{
    public AppTaskStatus Status { get; init; }
}

public record GetMyTasksQuery : IRequest<List<TaskDto>>
{
    public Guid UserId { get; init; }
}
