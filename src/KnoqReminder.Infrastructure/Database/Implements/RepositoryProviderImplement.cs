using CommunityToolkit.Diagnostics;
using KnoqReminder.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KnoqReminder.Infrastructure.Database.Implements;

sealed class RepositoryProviderImplement(
    IDbContextFactory<AppDbContext>? appDbContextFactory
    ) : IRepositoryProvider
{
    public async ValueTask<TRepository> CreateRepositoryAsync<TRepository>(CancellationToken cancellationToken = default)
        where TRepository : class, IRepositoryBase
    {

    }
}
