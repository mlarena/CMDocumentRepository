using CMDocumentRepository.Domain.Entities;
using CMDocumentRepository.Domain.Enums;
using CMDocumentRepository.Domain.Interfaces;
using CMDocumentRepository.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CMDocumentRepository.Infrastructure.Repositories;

public class ApprovalRepository : Repository<Approval>, IApprovalRepository
{
    public ApprovalRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Approval>> GetByDocumentIdAsync(Guid documentId)
    {
        return await _dbSet
            .Where(a => a.DocumentId == documentId)
            .OrderBy(a => a.OrderNumber)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Approval>> GetByApproverIdAsync(Guid approverId)
    {
        return await _dbSet
            .Where(a => a.ApproverId == approverId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Approval?> GetByDocumentAndApproverAsync(Guid documentId, Guid approverId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(a => a.DocumentId == documentId && a.ApproverId == approverId);
    }

    public async Task<IEnumerable<Approval>> GetPendingByApproverAsync(Guid approverId)
    {
        return await _dbSet
            .Where(a => a.ApproverId == approverId && a.Status == ApprovalStatus.Pending)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> AllApprovedAsync(Guid documentId)
    {
        return await _dbSet
            .Where(a => a.DocumentId == documentId)
            .AllAsync(a => a.Status == ApprovalStatus.Approved);
    }
}
