using System.Text.Json;
using CMDocumentRepository.Domain.Common;

namespace CMDocumentRepository.Domain.Entities;

public class AuditLog : BaseEntity
{
    public Guid UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public JsonDocument? OldValues { get; set; }
    public JsonDocument? NewValues { get; set; }

    public User User { get; set; } = null!;
}
