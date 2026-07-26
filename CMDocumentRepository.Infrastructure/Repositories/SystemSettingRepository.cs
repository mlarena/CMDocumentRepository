using CMDocumentRepository.Domain.Entities;
using CMDocumentRepository.Domain.Interfaces;
using CMDocumentRepository.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CMDocumentRepository.Infrastructure.Repositories;

public class SystemSettingRepository : Repository<SystemSetting>, ISystemSettingRepository
{
    public SystemSettingRepository(AppDbContext context) : base(context) { }

    public async Task<SystemSetting?> GetByKeyAsync(string key)
    {
        return await _dbSet.FirstOrDefaultAsync(ss => ss.Key == key);
    }

    public async Task<bool> KeyExistsAsync(string key)
    {
        return await _dbSet.AnyAsync(ss => ss.Key == key);
    }
}
