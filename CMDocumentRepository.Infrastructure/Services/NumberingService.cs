using CMDocumentRepository.Domain.Interfaces;
using CMDocumentRepository.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CMDocumentRepository.Infrastructure.Services;

public class NumberingService : INumberingService
{
    private readonly AppDbContext _context;

    public NumberingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string> GenerateDocumentNumberAsync(string prefix)
    {
        var year = DateTime.UtcNow.Year;
        var lastNumber = await _context.Documents
            .Where(d => d.DocumentNumber.StartsWith($"{prefix}-{year}"))
            .OrderByDescending(d => d.DocumentNumber)
            .Select(d => d.DocumentNumber)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(lastNumber))
        {
            return $"{prefix}-{year}-001";
        }

        var parts = lastNumber.Split('-');
        if (parts.Length == 3 && int.TryParse(parts[2], out var number))
        {
            return $"{prefix}-{year}-{(number + 1):D3}";
        }

        return $"{prefix}-{year}-001";
    }
}
