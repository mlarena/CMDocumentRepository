namespace CMDocumentRepository.Domain.Interfaces;

public interface INumberingService
{
    Task<string> GenerateDocumentNumberAsync(string typeCode);
}
