using CMDocumentRepository.Domain.Entities;

namespace CMDocumentRepository.Domain.Interfaces;

public interface IDocumentTypeRepository : IRepository<DocumentType>
{
    Task<DocumentType?> GetByCodeAsync(string code);
    Task<bool> CodeExistsAsync(string code);
}
