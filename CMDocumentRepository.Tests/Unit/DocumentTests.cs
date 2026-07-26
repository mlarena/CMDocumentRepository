using CMDocumentRepository.Application.Commands;
using CMDocumentRepository.Domain.Entities;
using CMDocumentRepository.Domain.Enums;
using CMDocumentRepository.Domain.Interfaces;
using CMDocumentRepository.Tests.Helpers;
using Moq;
using Xunit;

namespace CMDocumentRepository.Tests.Unit;

public class CreateDocumentCommandHandlerTests
{
    private readonly Mock<IDocumentRepository> _documentRepositoryMock;
    private readonly Mock<IDocumentTypeRepository> _typeRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<INumberingService> _numberingServiceMock;
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly CreateDocumentCommandHandler _handler;

    public CreateDocumentCommandHandlerTests()
    {
        _documentRepositoryMock = new Mock<IDocumentRepository>();
        _typeRepositoryMock = new Mock<IDocumentTypeRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _numberingServiceMock = new Mock<INumberingService>();
        _fileServiceMock = new Mock<IFileService>();

        _handler = new CreateDocumentCommandHandler(
            _documentRepositoryMock.Object,
            _typeRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _userRepositoryMock.Object,
            _numberingServiceMock.Object,
            _fileServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsDocumentDto()
    {
        var documentType = TestData.CreateTestDocumentType();
        var category = TestData.CreateTestCategory();
        var user = TestData.CreateTestUser();

        _typeRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(documentType);
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(category);
        _userRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(user);
        _numberingServiceMock.Setup(r => r.GenerateDocumentNumberAsync(It.IsAny<string>()))
            .ReturnsAsync("ТЕСТ-2026-001");
        _documentRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Document>()))
            .ReturnsAsync((Document d) => d);

        var command = new CreateDocumentCommand
        {
            Title = "Новый документ",
            CategoryId = category.Id,
            DocumentTypeId = documentType.Id,
            CreatedBy = user.Id
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Новый документ", result.Title);
        Assert.Equal("ТЕСТ-2026-001", result.DocumentNumber);
        Assert.Equal(DocumentStatus.Draft, result.Status);
    }

    [Fact]
    public async Task Handle_InvalidDocumentType_ThrowsException()
    {
        _typeRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((DocumentType?)null);

        var command = new CreateDocumentCommand
        {
            Title = "Документ",
            DocumentTypeId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            CreatedBy = Guid.NewGuid()
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_InvalidCategory_ThrowsException()
    {
        var documentType = TestData.CreateTestDocumentType();
        _typeRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(documentType);
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Category?)null);

        var command = new CreateDocumentCommand
        {
            Title = "Документ",
            DocumentTypeId = documentType.Id,
            CategoryId = Guid.NewGuid(),
            CreatedBy = Guid.NewGuid()
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));
    }
}

public class DeleteDocumentCommandHandlerTests
{
    private readonly Mock<IDocumentRepository> _documentRepositoryMock;
    private readonly DeleteDocumentCommandHandler _handler;

    public DeleteDocumentCommandHandlerTests()
    {
        _documentRepositoryMock = new Mock<IDocumentRepository>();
        _handler = new DeleteDocumentCommandHandler(_documentRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingDocument_SetsIsDeleted()
    {
        var document = TestData.CreateTestDocument();
        _documentRepositoryMock.Setup(r => r.GetByIdAsync(document.Id))
            .ReturnsAsync(document);
        _documentRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Document>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(
            new DeleteDocumentCommand { Id = document.Id, DeletedBy = Guid.NewGuid() },
            CancellationToken.None);

        Assert.True(result);
        Assert.True(document.IsDeleted);
        Assert.NotNull(document.DeletedAt);
    }

    [Fact]
    public async Task Handle_NonExistingDocument_ReturnsFalse()
    {
        _documentRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Document?)null);

        var result = await _handler.Handle(
            new DeleteDocumentCommand { Id = Guid.NewGuid(), DeletedBy = Guid.NewGuid() },
            CancellationToken.None);

        Assert.False(result);
    }
}

public class RestoreDocumentCommandHandlerTests
{
    private readonly Mock<IDocumentRepository> _documentRepositoryMock;
    private readonly RestoreDocumentCommandHandler _handler;

    public RestoreDocumentCommandHandlerTests()
    {
        _documentRepositoryMock = new Mock<IDocumentRepository>();
        _handler = new RestoreDocumentCommandHandler(_documentRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_DeletedDocument_RestoresIt()
    {
        var document = TestData.CreateTestDocument();
        document.IsDeleted = true;
        document.DeletedAt = DateTime.UtcNow;

        _documentRepositoryMock.Setup(r => r.GetByIdAsync(document.Id))
            .ReturnsAsync(document);
        _documentRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Document>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(
            new RestoreDocumentCommand { Id = document.Id }, CancellationToken.None);

        Assert.True(result);
        Assert.False(document.IsDeleted);
        Assert.Null(document.DeletedAt);
    }
}
