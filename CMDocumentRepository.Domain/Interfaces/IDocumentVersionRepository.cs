using CMDocumentRepository.Domain.Entities;

namespace CMDocumentRepository.Domain.Interfaces;

public interface IDocumentVersionRepository : IRepository<DocumentVersion>
{
    Task<IEnumerable<DocumentVersion>> GetByDocumentIdAsync(Guid documentId);
    Task<DocumentVersion?> GetLatestAsync(Guid documentId);
    Task<DocumentVersion?> GetByVersionNumberAsync(Guid documentId, decimal versionNumber);
}
