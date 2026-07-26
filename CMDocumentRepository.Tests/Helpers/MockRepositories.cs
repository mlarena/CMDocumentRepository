using CMDocumentRepository.Domain.Common;
using CMDocumentRepository.Domain.Interfaces;
using Moq;

namespace CMDocumentRepository.Tests.Helpers;

public static class MockRepositories
{
    public static Mock<IRepository<T>> CreateMockRepository<T>(List<T>? initialData = null) where T : BaseEntity
    {
        var data = initialData ?? new List<T>();
        var mock = new Mock<IRepository<T>>();

        mock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(data);

        mock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => data.FirstOrDefault(x => x.Id == id));

        mock.Setup(r => r.ExistsAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => data.Any(x => x.Id == id));

        mock.Setup(r => r.AddAsync(It.IsAny<T>()))
            .Callback<T>(item => data.Add(item))
            .ReturnsAsync((T item) => item);

        mock.Setup(r => r.UpdateAsync(It.IsAny<T>()))
            .Callback<T>(item =>
            {
                var index = data.FindIndex(x => x.Id == item.Id);
                if (index >= 0) data[index] = item;
            })
            .Returns(Task.CompletedTask);

        mock.Setup(r => r.DeleteAsync(It.IsAny<T>()))
            .Callback<T>(item => data.Remove(item))
            .Returns(Task.CompletedTask);

        return mock;
    }
}
