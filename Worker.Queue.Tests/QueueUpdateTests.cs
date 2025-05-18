using Moq;
using Newtonsoft.Json;
using Queue.Domain.Entities;
using Queue.Repository.Interfaces;
using Queue.Update.Services;

namespace Queue.Tests;

public class QueueUpdateTests
{
    [Fact]
    public async Task ProcessMessageAsync_ValidMessage_CallsUpdate()
    {
        var contact = new Contact { Id = Guid.NewGuid(), Name = "Teste" };
        var message = JsonConvert.SerializeObject(contact);

        var repoMock = new Mock<IRepositoryBase>();
        repoMock.Setup(r => r.Update(It.IsAny<Contact>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

        var processor = new UpdateProcessor(repoMock.Object);

        await processor.ProcessMessageAsync(message, CancellationToken.None);

        repoMock.Verify(r => r.Update(It.Is<Contact>(c => c.Name == "Teste"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessMessageAsync_UpdateFails_ThrowsException()
    {
        var contact = new Contact { Id = Guid.NewGuid(), Name = "Teste" };
        var message = JsonConvert.SerializeObject(contact);

        var repoMock = new Mock<IRepositoryBase>();
        repoMock.Setup(r => r.Update(It.IsAny<Contact>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

        var processor = new UpdateProcessor(repoMock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => processor.ProcessMessageAsync(message, CancellationToken.None));
    }
}
