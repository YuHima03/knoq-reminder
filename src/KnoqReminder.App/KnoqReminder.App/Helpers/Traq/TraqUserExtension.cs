using System.Net;
using KnoqReminder.Utilities;
using KnoqReminder.Utilities.Collections;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Kiota.Abstractions;

namespace KnoqReminder.App.Helpers.Traq;

static class TraqUserExtension
{
    public static readonly KeyedMemoryCacheEntryOptions UserCacheOptions = new()
    {
        Key = "Traq:User",
        Options = new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3) }
    };

    static ILogger CreateLogger(ILoggerFactory factory)
    {
        return factory.CreateLogger(typeof(TraqUserExtension));
    }

    public static async ValueTask<global::Traq.Models.UserDetail?> TryGetAsync(
        this global::Traq.Users.Item.WithUserItemRequestBuilder builder,
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
            if (pathParams.TryFindValue("userId", StringComparison.InvariantCultureIgnoreCase, out var uidObj) && uidObj is Guid uid)
            {
                logger.LogError_FailedToGetUser_UserNotFound(uid);
            }
            else
            {
                logger.LogError_FailedToGetUser(ex);
            }
        }
        catch (ApiException ex)
        {
            CreateLogger(loggerFactory).LogError_FailedToGetUser(ex);
        }
        return null;
    }

    public static async ValueTask<global::Traq.Models.UserDetail?> TryGetCachedAsync(
        this global::Traq.Users.Item.WithUserItemRequestBuilder builder,
        IMemoryCache cache,
        ILoggerFactory loggerFactory,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var pathParams = builder.ToGetRequestInformation(requestConfiguration).PathParameters;
        if (pathParams.TryFindValue("userId", StringComparison.InvariantCultureIgnoreCase, out var uidObj) && uidObj is Guid uid)
        {
            return await cache.GetOrCreateAsync(UserCacheOptions.GetMemoryCacheKey(uid), async entry =>
            {
                entry.SetOptions(UserCacheOptions.Options);
                return await builder.TryGetAsync(loggerFactory, requestConfiguration, cancellationToken).ConfigureAwait(false);
            });
        }
        return null;
    }
}

static partial class MessageLogger
{
    /// <summary>
    /// User not found: {<paramref name="userId"/>}
    /// </summary>
    [LoggerMessage(Level = LogLevel.Error, Message = "User not found: {userId}")]
    public static partial void LogError_FailedToGetUser_UserNotFound(this ILogger logger, Guid userId);

    /// <summary>
    /// Failed to get a traQ user.
    /// </summary>
    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to get a traQ user.")]
    public static partial void LogError_FailedToGetUser(this ILogger logger, Exception exception);
}
