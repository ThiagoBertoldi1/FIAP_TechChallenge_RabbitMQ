using Newtonsoft.Json;
using Queue.Domain.Entities;
using Queue.Domain.Interfaces;
using Queue.Repository.Interfaces;

namespace Queue.Delete.Services;

public class DeleteProcessor(IRepositoryBase repository) : IMessageProcessor
{
    private readonly IRepositoryBase _repository = repository;

    public async Task ProcessMessageAsync(string message, CancellationToken cancellationToken)
    {
        if (Guid.TryParse(JsonConvert.DeserializeObject<Contact>(message)?.Id.ToString(), out var idDelete) &&
            idDelete != Guid.Empty)
        {
            var result = await _repository.Delete<Contact>(idDelete, cancellationToken);
            if (!result)
            {
                throw new InvalidOperationException("Não foi possível inserir o registro");
            }
        }
    }
}
