using CommunityToolkit.Diagnostics;
using KnoqReminder.Domain.Repositories;

namespace KnoqReminder.Tests.Domain.Repositories;

sealed class RepositoryProviderMock : IRepositoryProvider
{
    readonly RepositoryMock _repo = new();

    async ValueTask<TRepository> IRepositoryProvider.CreateRepositoryAsync<TRepository>(CancellationToken cancellationToken)
    {
        return (_repo as TRepository) ?? ThrowHelper.ThrowNotSupportedException<TRepository>();
    }
}
