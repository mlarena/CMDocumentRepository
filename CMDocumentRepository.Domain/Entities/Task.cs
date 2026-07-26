using CMDocumentRepository.Domain.Common;
using CMDocumentRepository.Domain.Enums;

namespace CMDocumentRepository.Domain.Entities;

public class AppTask : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public AppTaskStatus Status { get; set; } = AppTaskStatus.Backlog;
    public Guid? AssigneeId { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? DueDate { get; set; }
    public int OrderNumber { get; set; }

    public User? Assignee { get; set; }
    public User Creator { get; set; } = null!;
}
