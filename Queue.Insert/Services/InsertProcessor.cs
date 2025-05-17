using Newtonsoft.Json;
using Queue.Domain.Entities;
using Queue.Insert.Interfaces;
using Queue.Repository.Interfaces;

namespace Queue.Insert.Services;

public class InsertProcessor(IRepositoryBase repository) : IMessageProcessor
{
    private readonly IRepositoryBase _repository = repository;

    public async Task ProcessMessageAsync(string message, CancellationToken cancellationToken)
    {
        var entity = JsonConvert.DeserializeObject<Contact>(message);
        if (entity is not null)
        {
            var result = await _repository.Insert(entity, cancellationToken);
            if (!result)
            {
                throw new InvalidOperationException("Não foi possível inserir o registro");
            }
        }
    }
}
