using CMDocumentRepository.Domain.Entities;
using CMDocumentRepository.Domain.Enums;
using CMDocumentRepository.Domain.Interfaces;
using CMDocumentRepository.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CMDocumentRepository.Infrastructure.Repositories;

public class TaskRepository : Repository<AppTask>, ITaskRepository
{
    public TaskRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<AppTask>> GetByStatusAsync(AppTaskStatus status)
    {
        return await _dbSet
            .Where(t => t.Status == status)
            .OrderBy(t => t.OrderNumber)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<AppTask>> GetByAssigneeAsync(Guid userId)
    {
        return await _dbSet
            .Where(t => t.AssigneeId == userId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<AppTask>> GetByCreatorAsync(Guid userId)
    {
        return await _dbSet
            .Where(t => t.CreatedBy == userId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<AppTask>> GetByPriorityAsync(TaskPriority priority)
    {
        return await _dbSet
            .Where(t => t.Priority == priority)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task UpdateOrderAsync(Guid taskId, int newOrder, AppTaskStatus newStatus)
    {
        var task = await _dbSet.FindAsync(taskId);
        if (task != null)
        {
            task.OrderNumber = newOrder;
            task.Status = newStatus;
            task.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
