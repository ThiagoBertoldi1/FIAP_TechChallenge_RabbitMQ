using Moq;
using Newtonsoft.Json;
using Queue.Delete.Services;
using Queue.Domain.Entities;
using Queue.Repository.Interfaces;

namespace Queue.Tests;

public class QueueDeleteTests
{
    [Fact]
    public async Task ProcessMessageAsync_ValidMessage_CallsDelete()
    {
        var contact = new Contact { Id = Guid.NewGuid() };
        var message = JsonConvert.SerializeObject(contact);

        var repoMock = new Mock<IRepositoryBase>();
        repoMock.Setup(r => r.Delete<Contact>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

        var processor = new DeleteProcessor(repoMock.Object);

        await processor.ProcessMessageAsync(message, CancellationToken.None);

        repoMock.Verify(r => r.Delete<Contact>(It.Is<Guid>(d => d == contact.Id), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessMessageAsync_DeleteFails_ThrowsException()
    {
        var contact = new Contact { Id = Guid.NewGuid() };
        var message = JsonConvert.SerializeObject(contact);

        var repoMock = new Mock<IRepositoryBase>();
        repoMock.Setup(r => r.Delete<Contact>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

        var processor = new DeleteProcessor(repoMock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => processor.ProcessMessageAsync(message, CancellationToken.None));
    }
}
