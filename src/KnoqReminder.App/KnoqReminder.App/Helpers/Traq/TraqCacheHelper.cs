using Microsoft.Extensions.Caching.Memory;
using Microsoft.Kiota.Abstractions;

namespace KnoqReminder.App.Helpers.Traq;

static class TraqCacheHelper
{
    const string UserCacheKey = "Traq:User";
    static readonly MemoryCacheEntryOptions UserCacheEntryOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3)
    };

    static string GetKey<T>(string key, T index) where T : ISpanFormattable
    {
        return $"{key}[{index}]";
    }

    public static async ValueTask<global::Traq.Models.UserDetail?> GetCachedWithLogOnFailureAsync(
        this global::Traq.Users.Item.WithUserItemRequestBuilder builder,
        IMemoryCache cache,
        ILoggerFactory loggerFactory,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default
        )
    {
        var userId = builder.ToGetRequestInformation(requestConfiguration).PathParameters["userId"];
        if (userId is not Guid uid)
        {
            return null;
        }
        return await cache.GetOrCreateAsync(GetKey(UserCacheKey, uid), async entry =>
        {
            entry.SetOptions(UserCacheEntryOptions);
            return await builder.GetWithLogOnFailureAsync(loggerFactory, requestConfiguration, cancellationToken).ConfigureAwait(false);
        });
    }
}
