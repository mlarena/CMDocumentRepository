using CMDocumentRepository.Domain.Entities;
using CMDocumentRepository.Domain.Enums;

namespace CMDocumentRepository.Domain.Interfaces;

public interface IApprovalRepository : IRepository<Approval>
{
    Task<IEnumerable<Approval>> GetByDocumentIdAsync(Guid documentId);
    Task<IEnumerable<Approval>> GetByApproverIdAsync(Guid approverId);
    Task<Approval?> GetByDocumentAndApproverAsync(Guid documentId, Guid approverId);
    Task<IEnumerable<Approval>> GetPendingByApproverAsync(Guid approverId);
    Task<bool> AllApprovedAsync(Guid documentId);
}
