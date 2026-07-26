using CMDocumentRepository.Domain.Entities;
using CMDocumentRepository.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CMDocumentRepository.Infrastructure.Data;

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext context, ILogger? logger = null)
    {
        try
        {
            await context.Database.EnsureCreatedAsync();
            logger?.LogInformation("Database ensured");
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Database error");
            throw;
        }

        if (!await context.Users.AnyAsync())
        {
            var superAdmin = new User
            {
                Id = Guid.NewGuid(),
                UserName = "su",
                Email = "admin@system.local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234567890"),
                FirstName = "Супер",
                LastName = "Администратор",
                Role = UserRole.SuperAdmin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(superAdmin);
            logger?.LogInformation("Created superadmin user");
        }

        if (!await context.DocumentTypes.AnyAsync())
        {
            var documentTypes = new List<DocumentType>
            {
                new() { Id = Guid.NewGuid(), Name = "Договор", Code = "ДОГ", IsSystem = true, CreatedAt = DateTime.UtcNow },
                new() { Id = Guid.NewGuid(), Name = "Приказ", Code = "ПРИК", IsSystem = true, CreatedAt = DateTime.UtcNow },
                new() { Id = Guid.NewGuid(), Name = "Акт", Code = "АКТ", IsSystem = true, CreatedAt = DateTime.UtcNow },
                new() { Id = Guid.NewGuid(), Name = "Счёт", Code = "СЧЁТ", IsSystem = true, CreatedAt = DateTime.UtcNow },
                new() { Id = Guid.NewGuid(), Name = "Накладная", Code = "НАКЛ", IsSystem = true, CreatedAt = DateTime.UtcNow },
                new() { Id = Guid.NewGuid(), Name = "Техническая документация", Code = "ТЕХД", IsSystem = true, CreatedAt = DateTime.UtcNow },
                new() { Id = Guid.NewGuid(), Name = "Протокол", Code = "ПРОТ", IsSystem = true, CreatedAt = DateTime.UtcNow },
                new() { Id = Guid.NewGuid(), Name = "Отчёт", Code = "ОТЧЁТ", IsSystem = true, CreatedAt = DateTime.UtcNow }
            };

            context.DocumentTypes.AddRange(documentTypes);
            logger?.LogInformation("Created document types");
        }

        if (!await context.Categories.AnyAsync())
        {
            var categories = new List<Category>
            {
                new() { Id = Guid.NewGuid(), Name = "Финансовые", Code = "ФИН", CreatedAt = DateTime.UtcNow },
                new() { Id = Guid.NewGuid(), Name = "Юридические", Code = "ЮРИ", CreatedAt = DateTime.UtcNow },
                new() { Id = Guid.NewGuid(), Name = "Кадровые", Code = "КАДР", CreatedAt = DateTime.UtcNow },
                new() { Id = Guid.NewGuid(), Name = "Технические", Code = "ТЕХН", CreatedAt = DateTime.UtcNow },
                new() { Id = Guid.NewGuid(), Name = "Административные", Code = "АДМ", CreatedAt = DateTime.UtcNow },
                new() { Id = Guid.NewGuid(), Name = "Маркетинговые", Code = "МАРК", CreatedAt = DateTime.UtcNow }
            };

            context.Categories.AddRange(categories);
            logger?.LogInformation("Created categories");
        }

        await context.SaveChangesAsync();
        logger?.LogInformation("Seed data saved");
    }
}
