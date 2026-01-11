using System.Net;
using KnoqReminder.Utilities;
using KnoqReminder.Utilities.Collections;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Kiota.Abstractions;

namespace KnoqReminder.App.Helpers.Traq;

static class TraqUserGroupExtension
{
    public static readonly KeyedMemoryCacheEntryOptions UserGroupCacheOptions = new()
    {
        Key = "Traq:UserGroup",
        Options = new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3) }
    };

    static ILogger CreateLogger(ILoggerFactory factory)
    {
        return factory.CreateLogger(typeof(TraqUserGroupExtension));
    }

    public static async ValueTask<global::Traq.Models.UserGroup?> TryGetAsync(
        this global::Traq.Groups.Item.WithGroupItemRequestBuilder builder,
        ILoggerFactory loggerFactory,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await builder.GetAsync(requestConfiguration, cancellationToken);
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.NotFound)
        {
            var logger = CreateLogger(loggerFactory);
            var pathParams = builder.ToGetRequestInformation(requestConfiguration).PathParameters;
            if (pathParams.TryFindValue("groupId", StringComparison.InvariantCultureIgnoreCase, out var groupId))
            {
                logger.LogError_FailedToGetGroup_GroupNotFound(groupId);
            }
            else
            {
                logger.LogError_FailedToGetGroup(ex);
            }
        }
        catch (ApiException ex)
        {
            CreateLogger(loggerFactory).LogError_FailedToGetGroup(ex);
        }
        return null;
    }

    public static async ValueTask<global::Traq.Models.UserGroup?> TryGetCachedAsync(
        this global::Traq.Groups.Item.WithGroupItemRequestBuilder builder,
        IMemoryCache cache,
        ILoggerFactory loggerFactory,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var pathParams = builder.ToGetRequestInformation(requestConfiguration).PathParameters;
        if (pathParams.TryFindValue("groupId", StringComparison.InvariantCultureIgnoreCase, out var gidObj) && gidObj is Guid gid)
        {
            return await cache.GetOrCreateAsync(UserGroupCacheOptions.GetMemoryCacheKey(gid), async entry =>
            {
                entry.SetOptions(UserGroupCacheOptions.Options);
                return await builder.TryGetAsync(loggerFactory, requestConfiguration, cancellationToken).ConfigureAwait(false);
            });
        }
        return null;
    }
}

static partial class MessageLogger
{
    [LoggerMessage(Level = LogLevel.Error, Message = "Group not found: {groupId}")]
    public static partial void LogError_FailedToGetGroup_GroupNotFound(this ILogger logger, object? groupId);

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to get group.")]
    public static partial void LogError_FailedToGetGroup(this ILogger logger, Exception exception);
}
