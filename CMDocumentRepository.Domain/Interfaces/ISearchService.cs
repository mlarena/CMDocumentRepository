using CMDocumentRepository.Domain.Entities;
using CMDocumentRepository.Domain.Enums;

namespace CMDocumentRepository.Domain.Interfaces;

public interface ISearchService
{
    Task<IEnumerable<Document>> FullTextSearchAsync(
        string query,
        DocumentStatus? status = null,
        Guid? categoryId = null,
        Guid? documentTypeId = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        int limit = 100);
}