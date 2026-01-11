using System.Net;
using KnoqReminder.Utilities;
using KnoqReminder.Utilities.Collections;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Kiota.Abstractions;

namespace KnoqReminder.App.Helpers.Knoq;

static class EventDetailHelper
{
    public static readonly KeyedMemoryCacheEntryOptions EventDetailCacheOptions = new()
    {
        Key = "Knoq:EventDetail",
        Options = new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3) }
    };

    static ILogger CreateLogger(ILoggerFactory factory)
    {
        return factory.CreateLogger(typeof(EventDetailHelper));
    }

    public static async ValueTask<global::Knoq.Models.ResponseEventDetail?> TryGetAsync(
        this global::Knoq.Events.Item.WithEventItemRequestBuilder builder,
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
            if (pathParams.TryFindValue("eventId", StringComparison.InvariantCultureIgnoreCase, out var eventId))
            {
                logger.LogError("Event not found: {eventId}", eventId);
            }
            else
            {
                logger.LogError(ex, "Failed to get a knoQ event.");
            }
        }
        catch (ApiException ex)
        {
            CreateLogger(loggerFactory).LogError(ex, "Failed to get a knoQ event.");
        }
        return null;
    }

    public static async ValueTask<global::Knoq.Models.ResponseEventDetail?> TryGetCachedAsync(
        this global::Knoq.Events.Item.WithEventItemRequestBuilder builder,
        IMemoryCache cache,
        ILoggerFactory loggerFactory,
        Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = null,
        CancellationToken cancellationToken = default)
    {
        var pathParams = builder.ToGetRequestInformation(requestConfiguration).PathParameters;
        if (pathParams.TryFindValue("eventId", StringComparison.InvariantCultureIgnoreCase, out var eidObj) && eidObj is Guid eid)
        {
            return await cache.GetOrCreateAsync(EventDetailCacheOptions.GetMemoryCacheKey(eid), async entry =>
            {
                entry.SetOptions(EventDetailCacheOptions.Options);
                return await builder.TryGetAsync(loggerFactory, requestConfiguration, cancellationToken).ConfigureAwait(false);
            });
        }
        return null;
    }
}
