using CMDocumentRepository.Domain.Entities;
using CMDocumentRepository.Domain.Enums;
using CMDocumentRepository.Domain.Interfaces;
using CMDocumentRepository.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CMDocumentRepository.Infrastructure.Repositories;

public class DocumentRepository : Repository<Document>, IDocumentRepository
{
    public DocumentRepository(AppDbContext context) : base(context) { }

    public async Task<Document?> GetByNumberAsync(string documentNumber)
    {
        return await _dbSet.FirstOrDefaultAsync(d => d.DocumentNumber == documentNumber);
    }

    public async Task<IEnumerable<Document>> GetByStatusAsync(DocumentStatus status)
    {
        return await _dbSet
            .Where(d => d.Status == status && !d.IsDeleted)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetByCategoryAsync(Guid categoryId)
    {
        return await _dbSet
            .Where(d => d.CategoryId == categoryId && !d.IsDeleted)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetByTypeAsync(Guid documentTypeId)
    {
        return await _dbSet
            .Where(d => d.DocumentTypeId == documentTypeId && !d.IsDeleted)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetByCreatorAsync(Guid userId)
    {
        return await _dbSet
            .Where(d => d.CreatedBy == userId && !d.IsDeleted)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Document>> SearchAsync(
        string keyword,
        DocumentStatus? status,
        Guid? categoryId,
        Guid? documentTypeId,
        DateTime? dateFrom,
        DateTime? dateTo)
    {
        var query = _dbSet.Where(d => !d.IsDeleted).AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(d =>
                d.Title.Contains(keyword) ||
                d.Description!.Contains(keyword) ||
                d.DocumentNumber.Contains(keyword));
        }

        if (status.HasValue)
            query = query.Where(d => d.Status == status.Value);

        if (categoryId.HasValue)
            query = query.Where(d => d.CategoryId == categoryId.Value);

        if (documentTypeId.HasValue)
            query = query.Where(d => d.DocumentTypeId == documentTypeId.Value);

        if (dateFrom.HasValue)
            query = query.Where(d => d.CreatedAt >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(d => d.CreatedAt <= dateTo.Value);

        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<string> GetNextDocumentNumberAsync(string prefix, int year)
    {
        var lastNumber = await _dbSet
            .Where(d => d.DocumentNumber.StartsWith($"{prefix}-{year}"))
            .OrderByDescending(d => d.DocumentNumber)
            .Select(d => d.DocumentNumber)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(lastNumber))
        {
            return $"{prefix}-{year}-001";
        }

        var parts = lastNumber.Split('-');
        if (parts.Length == 3 && int.TryParse(parts[2], out var number))
        {
            return $"{prefix}-{year}-{(number + 1):D3}";
        }

        return $"{prefix}-{year}-001";
    }
}
