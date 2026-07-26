using CMDocumentRepository.Domain.Entities;

namespace CMDocumentRepository.Domain.Interfaces;

public interface ISystemSettingRepository : IRepository<SystemSetting>
{
    Task<SystemSetting?> GetByKeyAsync(string key);
    Task<bool> KeyExistsAsync(string key);
}
