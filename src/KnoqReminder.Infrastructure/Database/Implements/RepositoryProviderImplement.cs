using CommunityToolkit.Diagnostics;
using KnoqReminder.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KnoqReminder.Infrastructure.Database.Implements;

sealed class RepositoryProviderImplement(
    IDbContextFactory<AppDbContext> appDbContextFactory
    ) : IRepositoryProvider
{
    public async ValueTask<TRepository> CreateRepositoryAsync<TRepository>(CancellationToken cancellationToken = default)
        where TRepository : class, IRepositoryBase
    {
        AppDbContext? ctx = null;
        try
        {
            ctx = await appDbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
            if (ctx is not TRepository repository)
            {
                return ThrowHelper.ThrowInvalidOperationException<TRepository>($"The requested repository type '{typeof(TRepository).FullName}' is not supported.");
            }
            ctx = null; // Prevent disposal in finally block
            return repository;
        }
        finally
        {
            if (ctx is not null)
            {
                await ctx.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}
