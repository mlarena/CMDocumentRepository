using CMDocumentRepository.Domain.Entities;

namespace CMDocumentRepository.Domain.Interfaces;

public interface IDocumentVersionRepository
{
    Task<DocumentVersion> AddAsync(DocumentVersion version);
    Task<DocumentVersion?> GetByIdAsync(Guid id);
    Task<IEnumerable<DocumentVersion>> GetByDocumentIdAsync(Guid documentId);
    Task<DocumentVersion?> GetLatestAsync(Guid documentId);
    Task<DocumentVersion?> GetByVersionNumberAsync(Guid documentId, decimal versionNumber);
    Task DeleteAsync(DocumentVersion version);
}
