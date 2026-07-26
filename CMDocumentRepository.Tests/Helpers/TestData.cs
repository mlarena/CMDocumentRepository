using CMDocumentRepository.Domain.Entities;
using CMDocumentRepository.Domain.Enums;

namespace CMDocumentRepository.Tests.Helpers;

public static class TestData
{
    public static User CreateTestUser(
        string userName = "testuser",
        string email = "test@example.com",
        UserRole role = UserRole.User)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            UserName = userName,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            FirstName = "Тест",
            LastName = "Пользователь",
            Role = role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Document CreateTestDocument(Guid? createdBy = null)
    {
        return new Document
        {
            Id = Guid.NewGuid(),
            DocumentNumber = "ТЕСТ-2026-001",
            Title = "Тестовый документ",
            Description = "Описание тестового документа",
            CategoryId = Guid.NewGuid(),
            DocumentTypeId = Guid.NewGuid(),
            Version = 1.0m,
            Status = DocumentStatus.Draft,
            CreatedBy = createdBy ?? Guid.NewGuid(),
            FilePath = "test.pdf",
            FileSize = 1024,
            FileExtension = ".pdf",
            MimeType = "application/pdf",
            CreatedAt = DateTime.UtcNow
        };
    }

    public static DocumentType CreateTestDocumentType()
    {
        return new DocumentType
        {
            Id = Guid.NewGuid(),
            Name = "Тестовый тип",
            Code = "ТЕСТ",
            Description = "Описание",
            IsSystem = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Category CreateTestCategory()
    {
        return new Category
        {
            Id = Guid.NewGuid(),
            Name = "Тестовая категория",
            Code = "ТЕСТКАТ",
            Description = "Описание",
            CreatedAt = DateTime.UtcNow
        };
    }

    public static AppTask CreateTestTask(Guid? createdBy = null)
    {
        return new AppTask
        {
            Id = Guid.NewGuid(),
            Title = "Тестовая задача",
            Description = "Описание задачи",
            Priority = TaskPriority.Medium,
            Status = AppTaskStatus.Backlog,
            CreatedBy = createdBy ?? Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Approval CreateTestApproval(Guid documentId, Guid approverId)
    {
        return new Approval
        {
            Id = Guid.NewGuid(),
            DocumentId = documentId,
            ApproverId = approverId,
            Status = ApprovalStatus.Pending,
            OrderNumber = 1,
            CreatedAt = DateTime.UtcNow
        };
    }
}
