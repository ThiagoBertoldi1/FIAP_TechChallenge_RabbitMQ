namespace Queue.Repository.Interfaces;
public interface IRepositoryBase
{
    Task<bool> Insert<T>(T entity, CancellationToken cancellationToken);
    Task<bool> Update<T>(T entity, CancellationToken cancellationToken);
    Task<bool> Delete<T>(Guid id, CancellationToken cancellationToken);
}
