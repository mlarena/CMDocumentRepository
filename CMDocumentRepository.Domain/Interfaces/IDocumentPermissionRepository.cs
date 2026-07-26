using CMDocumentRepository.Domain.Entities;

namespace CMDocumentRepository.Domain.Interfaces;

public interface IDocumentPermissionRepository : IRepository<DocumentPermission>
{
    Task<IEnumerable<DocumentPermission>> GetByDocumentIdAsync(Guid documentId);
    Task<IEnumerable<DocumentPermission>> GetByUserIdAsync(Guid userId);
    Task<DocumentPermission?> GetByDocumentAndUserAsync(Guid documentId, Guid userId);
    Task<bool> HasPermissionAsync(Guid documentId, Guid userId, string permissionType);
}
