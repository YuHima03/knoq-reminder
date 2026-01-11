using KnoqReminder.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Kiota.Abstractions;

namespace KnoqReminder.App.Helpers.Traq;

static class TraqGroupListExtension
{
    public static readonly KeyedMemoryCacheEntryOptions GroupListCacheOptions = new()
    {
        Key = "Traq:GroupList",
        Options = new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3) }
    };

    static ILogger CreateLogger(ILoggerFactory factory)
    {
        return factory.CreateLogger(typeof(TraqGroupListExtension));
    }

    public static async ValueTask<global::Traq.Models.UserGroup[]?> TryGetAsync(
        this global::Traq.Groups.GroupsRequestBuilder builder,
        ILoggerFactory loggerFactory,
        Action<RequestConfiguration<DefaultQueryParameters>>? request = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return (await builder.GetAsync(request, cancellationToken))?.ToArray();
        }
        catch (Exception ex)
        {
            CreateLogger(loggerFactory).LogError_FailedToGetGroupList(ex);
        }
        return null;
    }

    public static async ValueTask<global::Traq.Models.UserGroup[]?> TryGetCachedAsync(
        this global::Traq.Groups.GroupsRequestBuilder builder,
        IMemoryCache cache,
        ILoggerFactory loggerFactory,
        Action<RequestConfiguration<DefaultQueryParameters>>? request = null,
        CancellationToken cancellationToken = default)
    {
        return await cache.GetOrCreateAsync(GroupListCacheOptions.Key, async entry =>
        {
            entry.SetOptions(GroupListCacheOptions.Options);
            return await builder.TryGetAsync(loggerFactory, request, cancellationToken);
        });
    }
}

static partial class MessageLogger
{
    /// <summary>
    /// Failed to get group list.
    /// </summary>
    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to get group list.")]
    public static partial void LogError_FailedToGetGroupList(this ILogger logger, Exception ex);
}
