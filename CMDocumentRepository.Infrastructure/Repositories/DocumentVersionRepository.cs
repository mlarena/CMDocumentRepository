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
        return await _context.DocumentVersions
            .Where(dv => dv.DocumentId == documentId)
            .OrderByDescending(dv => dv.VersionNumber)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<DocumentVersion?> GetLatestAsync(Guid documentId)
    {
        return await _context.DocumentVersions
            .Where(v => v.DocumentId == documentId)
            .OrderByDescending(v => v.VersionNumber)
            .FirstOrDefaultAsync();
    }

    public async Task<DocumentVersion?> GetByVersionNumberAsync(Guid documentId, decimal versionNumber)
    {
        return await _context.DocumentVersions
            .FirstOrDefaultAsync(v => v.DocumentId == documentId && v.VersionNumber == versionNumber);
    }

    public async Task DeleteAsync(DocumentVersion version)
    {
        _context.DocumentVersions.Remove(version);
        await _context.SaveChangesAsync();
    }
}
