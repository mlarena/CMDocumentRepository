using CMDocumentRepository.Domain.Entities;
using CMDocumentRepository.Domain.Interfaces;
using CMDocumentRepository.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CMDocumentRepository.Infrastructure.Repositories;

public class DocumentTypeRepository : Repository<DocumentType>, IDocumentTypeRepository
{
    public DocumentTypeRepository(AppDbContext context) : base(context) { }

    public async Task<DocumentType?> GetByCodeAsync(string code)
    {
        return await _dbSet.FirstOrDefaultAsync(dt => dt.Code == code);
    }

    public async Task<bool> CodeExistsAsync(string code)
    {
        return await _dbSet.AnyAsync(dt => dt.Code == code);
    }
}
