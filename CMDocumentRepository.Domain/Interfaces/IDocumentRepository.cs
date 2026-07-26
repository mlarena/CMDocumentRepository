using CMDocumentRepository.Domain.Entities;
using CMDocumentRepository.Domain.Enums;

namespace CMDocumentRepository.Domain.Interfaces;

public interface IDocumentRepository : IRepository<Document>
{
    Task<Document?> GetByNumberAsync(string documentNumber);
    Task<IEnumerable<Document>> GetByStatusAsync(DocumentStatus status);
    Task<IEnumerable<Document>> GetByCategoryAsync(Guid categoryId);
    Task<IEnumerable<Document>> GetByTypeAsync(Guid documentTypeId);
    Task<IEnumerable<Document>> GetByCreatorAsync(Guid userId);
    Task<IEnumerable<Document>> SearchAsync(string keyword, DocumentStatus? status, Guid? categoryId, Guid? documentTypeId, DateTime? dateFrom, DateTime? dateTo);
    Task<string> GetNextDocumentNumberAsync(string prefix, int year);
}
