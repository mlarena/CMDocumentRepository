using CMDocumentRepository.Domain.Entities;
using CMDocumentRepository.Domain.Interfaces;
using CMDocumentRepository.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CMDocumentRepository.Infrastructure.Repositories;

public class DocumentVersionRepository : Repository<DocumentVersion>, IDocumentVersionRepository
{
    public DocumentVersionRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<DocumentVersion>> GetByDocumentIdAsync(Guid documentId)
    {
        return await _dbSet
            .Where(dv => dv.DocumentId == documentId)
            .OrderByDescending(dv => dv.VersionNumber)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<DocumentVersion?> GetLatestAsync(Guid documentId)
    {
        return await _dbSet
            .Where(dv => dv.DocumentId == documentId)
            .OrderByDescending(dv => dv.VersionNumber)
            .FirstOrDefaultAsync();
    }

    public async Task<DocumentVersion?> GetByVersionNumberAsync(Guid documentId, decimal versionNumber)
    {
        return await _dbSet
            .FirstOrDefaultAsync(dv => dv.DocumentId == documentId && dv.VersionNumber == versionNumber);
    }
}
