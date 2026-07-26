using System.Text.RegularExpressions;
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

        // Формируем PostgreSQL tsquery из пользовательского ввода
        var tsQuery = BuildTsQuery(query);

        var sql = @"
            SELECT d.""Id"", d.""DocumentNumber"", d.""Title"", d.""Description"",
                   d.""CategoryId"", d.""DocumentTypeId"", d.""Version"", d.""Status"",
                   d.""CreatedBy"", d.""UpdatedBy"", d.""ApprovedBy"", d.""ApprovedAt"",
                   d.""ValidFrom"", d.""ValidUntil"", d.""FilePath"", d.""FileSize"",
                   d.""FileExtension"", d.""MimeType"", d.""IsDeleted"", d.""DeletedAt"",
                   d.""DeletedBy"", d.""Metadata"", d.""CreatedAt"", d.""UpdatedAt""
            FROM ""Documents"" d
            WHERE d.""IsDeleted"" = false
              AND d.""SearchVector"" @@ to_tsquery('russian', {0})";

        var parameters = new List<object> { tsQuery };
        var paramIndex = 1;

        if (status.HasValue)
        {
            sql += $@" AND d.""Status"" = {{{paramIndex}}}";
            parameters.Add(status.Value.ToString());
            paramIndex++;
        }

        if (categoryId.HasValue)
        {
            sql += $@" AND d.""CategoryId"" = {{{paramIndex}}}";
            parameters.Add(categoryId.Value);
            paramIndex++;
        }

        if (documentTypeId.HasValue)
        {
            sql += $@" AND d.""DocumentTypeId"" = {{{paramIndex}}}";
            parameters.Add(documentTypeId.Value);
            paramIndex++;
        }

        if (dateFrom.HasValue)
        {
            sql += $@" AND d.""CreatedAt"" >= {{{paramIndex}}}";
            parameters.Add(dateFrom.Value);
            paramIndex++;
        }

        if (dateTo.HasValue)
        {
            sql += $@" AND d.""CreatedAt"" <= {{{paramIndex}}}";
            parameters.Add(dateTo.Value);
            paramIndex++;
        }

        sql += $@"
            ORDER BY ts_rank(d.""SearchVector"", to_tsquery('russian', {0})) DESC
            LIMIT {limit}";

        return await _context.Documents
            .FromSqlRaw(sql, parameters.ToArray())
            .AsNoTracking()
            .ToListAsync();
    }

    private static string BuildTsQuery(string input)
    {
        // Убираем спецсимволы, разбиваем на слова, соединяем через &
        var cleaned = Regex.Replace(input, @"[^\w\s]", " ");
        var words = cleaned.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (words.Length == 0)
            return "";

        // Каждое слово получает суффикс :* для prefix matching
        return string.Join(" & ", words.Select(w => w.Trim() + ":*"));
    }
}