using Newtonsoft.Json;
using Queue.Domain.Entities;
using Queue.Repository.Interfaces;
using Queue.Update.Interfaces;

namespace Queue.Update.Services;
public class UpdateProcessor(IRepositoryBase repositoryBase) : IMessageProcessor
{
    private readonly IRepositoryBase _repository = repositoryBase;

    public async Task ProcessMessageAsync(string message, CancellationToken cancellationToken)
    {
        var entity = JsonConvert.DeserializeObject<Contact>(message);
        if (entity is not null)
        {
            var updated = await _repository.Update(entity, cancellationToken);
            if (!updated)
            {
                throw new InvalidOperationException("Não foi possível atualizar o registro");
            }
        }
    }
}
