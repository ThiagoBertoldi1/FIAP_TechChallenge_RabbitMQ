using Moq;
using Newtonsoft.Json;
using Queue.Domain.Entities;
using Queue.Insert.Services;
using Queue.Repository.Interfaces;

namespace Queue.Tests;

public class QueueInsertTests
{
    [Fact]
    public async Task ProcessMessageAsync_ValidMessage_CallsInsert()
    {
        var contact = new Contact { Id = Guid.NewGuid(), Name = "Teste" };
        var message = JsonConvert.SerializeObject(contact);

        var repoMock = new Mock<IRepositoryBase>();
        repoMock.Setup(r => r.Insert(It.IsAny<Contact>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

        var processor = new InsertProcessor(repoMock.Object);

        await processor.ProcessMessageAsync(message, CancellationToken.None);

        repoMock.Verify(r => r.Insert(It.Is<Contact>(c => c.Name == "Teste"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessMessageAsync_InsertFails_ThrowsException()
    {
        var contact = new Contact { Id = Guid.NewGuid(), Name = "Teste" };
        var message = JsonConvert.SerializeObject(contact);

        var repoMock = new Mock<IRepositoryBase>();
        repoMock.Setup(r => r.Insert(It.IsAny<Contact>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

        var processor = new InsertProcessor(repoMock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => processor.ProcessMessageAsync(message, CancellationToken.None));
    }
}