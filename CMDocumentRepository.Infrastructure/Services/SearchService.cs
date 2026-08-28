using CMDocumentRepository.Domain.Entities;
using CMDocumentRepository.Domain.Enums;
using CMDocumentRepository.Domain.Interfaces;
using CMDocumentRepository.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CMDocumentRepository.Infrastructure.Services;

public class SearchService : ISearchService
{
    private readonly AppDbContext _context;

    public SearchService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Document>> FullTextSearchAsync(
        string query,
        DocumentStatus? status = null,
        Guid? categoryId = null,
        Guid? documentTypeId = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        int limit = 100)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Enumerable.Empty<Document>();

        var searchQuery = query.ToLowerInvariant();

        var docs = _context.Documents.AsNoTracking()
            .Where(d => d.IsDeleted == false)
            .Where(d => d.Title.ToLower().Contains(searchQuery)
                     || d.Description.ToLower().Contains(searchQuery)
                     || d.FileName.ToLower().Contains(searchQuery))
            .AsQueryable();

        if (status.HasValue)
            docs = docs.Where(d => d.Status == status.Value);

        if (categoryId.HasValue)
            docs = docs.Where(d => d.CategoryId == categoryId.Value);

        if (documentTypeId.HasValue)
            docs = docs.Where(d => d.DocumentTypeId == documentTypeId.Value);

        if (dateFrom.HasValue)
            docs = docs.Where(d => d.CreatedAt >= dateFrom.Value);

        if (dateTo.HasValue)
            docs = docs.Where(d => d.CreatedAt <= dateTo.Value);

        return await docs
            .OrderByDescending(d => d.Title.ToLower().IndexOf(searchQuery))
            .Take(limit)
            .ToListAsync();
    }
}