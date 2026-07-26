using CMDocumentRepository.Domain.Entities;
using CMDocumentRepository.Domain.Enums;

namespace CMDocumentRepository.Domain.Interfaces;

public interface ITaskRepository : IRepository<AppTask>
{
    Task<IEnumerable<AppTask>> GetByStatusAsync(AppTaskStatus status);
    Task<IEnumerable<AppTask>> GetByAssigneeAsync(Guid userId);
    Task<IEnumerable<AppTask>> GetByCreatorAsync(Guid userId);
    Task<IEnumerable<AppTask>> GetByPriorityAsync(TaskPriority priority);
    Task UpdateOrderAsync(Guid taskId, int newOrder, AppTaskStatus newStatus);
}
