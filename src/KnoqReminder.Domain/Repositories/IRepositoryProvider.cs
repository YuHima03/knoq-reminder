namespace KnoqReminder.Domain.Repositories;

public interface IRepositoryProvider
{
    ValueTask<TRepository> CreateRepositoryAsync<TRepository>(CancellationToken cancellationToken = default)
        where TRepository : class, IRepositoryBase;
}
