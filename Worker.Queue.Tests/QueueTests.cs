using Moq;
using Queue.Domain.Entities;
using Queue.Repository.Interfaces;

namespace Worker.Queue.Tests;

public class QueueTests
{
    private readonly Mock<IRepositoryBase> _repository = new();

    public static IEnumerable<object[]> InsertContacts =>
        [
            [new Contact { Name = "Pessoa4", Email = "pessoa1@gmail.com", PhoneNumber = 47991444009, District = "SC", Region = "Sul" }],
            [new Contact { Name = "Pessoa2", Email = "pessoa2@gmail.com", PhoneNumber = 47991666009, District = "AP", Region = "Sudeste" }],
            [new Contact { Name = "Pessoa3", Email = "pessoa3@gmail.com", PhoneNumber = 47321330009, District = "PO", Region = "Centro-Oeste" }]
        ];
    [Theory]
    [MemberData(nameof(InsertContacts))]
    public async void Contact_ShouldBeInserted(Contact contact)
    {
        _repository.Setup(x => x.Insert(contact, CancellationToken.None)).ReturnsAsync(true);

        var inserted = await _repository.Object.Insert(contact, CancellationToken.None);

        _repository.Verify(r => r.Insert(contact, CancellationToken.None), Times.Once);
        Assert.True(inserted);
    }

    public static IEnumerable<object[]> UpdateContacts =>
        [
            [new Contact { Id = Guid.NewGuid(), Name = "Pessoa1", Email = "pessoa1@gmail.com", PhoneNumber = 47991444009, District = "SC", Region = "Sul" }],
            [new Contact { Id = Guid.NewGuid(), Name = "Pessoa2", Email = "pessoa2@gmail.com", PhoneNumber = 47991666009, District = "AP", Region = "Sudeste" }],
            [new Contact { Id = Guid.NewGuid(), Name = "Pessoa3", Email = "pessoa3@gmail.com", PhoneNumber = 47321330009, District = "PO", Region = "Centro-Oeste" }]
        ];
    [Theory]
    [MemberData(nameof(UpdateContacts))]
    public async void Contact_ShouldBeUpdated(Contact contact)
    {
        _repository.Setup(x => x.Update(contact, CancellationToken.None)).ReturnsAsync(true);

        var response = await _repository.Object.Update(contact, CancellationToken.None);

        _repository.Verify(r => r.Update(contact, CancellationToken.None), Times.Once);
        Assert.True(response);
    }

    public static IEnumerable<object[]> DeleteContacts =>
        [
            [ Guid.NewGuid() ],
            [ Guid.NewGuid() ],
            [ Guid.NewGuid() ]
        ];
    [Theory]
    [MemberData(nameof(DeleteContacts))]
    public async void Contact_ShouldBeDeleted(Guid id)
    {
        _repository.Setup(x => x.Delete<Contact>(id, CancellationToken.None)).ReturnsAsync(true);

        var response = await _repository.Object.Delete<Contact>(id, CancellationToken.None);

        _repository.Verify(x => x.Delete<Contact>(id, CancellationToken.None), Times.Once);
        Assert.True(response);
    }
}