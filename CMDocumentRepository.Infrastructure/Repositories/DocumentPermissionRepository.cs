using CMDocumentRepository.Domain.Entities;
using CMDocumentRepository.Domain.Interfaces;
using CMDocumentRepository.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CMDocumentRepository.Infrastructure.Repositories;

public class DocumentPermissionRepository : Repository<DocumentPermission>, IDocumentPermissionRepository
{
    public DocumentPermissionRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<DocumentPermission>> GetByDocumentIdAsync(Guid documentId)
    {
        return await _dbSet
            .Where(dp => dp.DocumentId == documentId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<DocumentPermission>> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Where(dp => dp.UserId == userId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<DocumentPermission?> GetByDocumentAndUserAsync(Guid documentId, Guid userId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(dp => dp.DocumentId == documentId && dp.UserId == userId);
    }

    public async Task<bool> HasPermissionAsync(Guid documentId, Guid userId, string permissionType)
    {
        var permission = await _dbSet
            .FirstOrDefaultAsync(dp => dp.DocumentId == documentId && dp.UserId == userId);

        if (permission == null) return false;

        return permissionType switch
        {
            "Read" => permission.CanRead,
            "Edit" => permission.CanEdit,
            "Approve" => permission.CanApprove,
            "Delete" => permission.CanDelete,
            _ => false
        };
    }
}
