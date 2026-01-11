using Knoq;
using KnoqReminder.App.Helpers.Knoq;
using KnoqReminder.Domain.Services.Events;
using KnoqReminder.Utilities.Collections;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;

namespace KnoqReminder.App.Services.Events;

sealed class EventProvider(
    IMemoryCache cache,
    KnoqApiClient knoq,
    ILoggerFactory loggerFactory
    )
    : IEventProvider
{
    public async ValueTask<ScheduledEvent[]> GetEventsByStartTimeAsync(DateTimeOffset startTimeFrom, DateTimeOffset startTimeTo, CancellationToken cancellationToken = default)
    {
        var knoqEvents = await knoq.Events.GetAsync(
            requestConfiguration: config =>
            {
                config.QueryParameters.DateBegin = startTimeFrom.ToUniversalTime().ToString("O");
                config.QueryParameters.DateEnd = startTimeTo.ToUniversalTime().ToString("O");
            },
            cancellationToken: cancellationToken) ?? [];

        // Note: API は指定された期間内と開催時間が重複するイベントを返すため、開始時間で改めてフィルタリングする必要がある.
        knoqEvents.RemoveAllUnstable(e => DateTimeOffset.TryParse(e.TimeStart, null, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var dtStart) && startTimeFrom <= dtStart);
        if (knoqEvents is [])
        {
            return [];
        }
        knoqEvents.Sort((x, y) => x.TimeStart!.CompareTo(y.TimeStart!));

        return await knoqEvents
            .Select(x => x.EventId.GetValueOrDefault())
            .Where(x => x != Guid.Empty)
            .ToAsyncEnumerable()
            .Select(async (eid, ct) =>
            {
                var eventDetail = await knoq.Events[eid].TryGetCachedAsync(cache, loggerFactory, cancellationToken: ct).ConfigureAwait(false);
                return eventDetail?.ToScheduledEvent()!;
            })
            .Where(x => x is not null)
            .ToArrayAsync(cancellationToken);
    }
}

file static class KnoqEventExtension
{
    public static ScheduledEvent ToScheduledEvent(this Knoq.Models.ResponseEventDetail @event)
    {
        return new ScheduledEvent(
            @event.EventId.GetValueOrDefault(),
            @event.Name ?? "",
            @event.Place ?? "",
            (@event.Group?.GroupId).GetValueOrDefault(),
            @event.Description ?? "",
            @event.Open.GetValueOrDefault(),
            @event.Attendees?
                .Select(a => KeyValuePair.Create(
                    a.UserId.GetValueOrDefault(),
                    a.Schedule switch
                    {
                        Knoq.Models.ResponseEventDetail_attendees_schedule.Pending => EventAttendanceStatus.Pending,
                        Knoq.Models.ResponseEventDetail_attendees_schedule.Absent => EventAttendanceStatus.Absent,
                        Knoq.Models.ResponseEventDetail_attendees_schedule.Attendance => EventAttendanceStatus.Attending,
                        _ => EventAttendanceStatus.Unknown
                    }))
                .ToDictionary() ?? [],
            ParseToDateTimeOffsetOrDefault(@event.TimeStart),
            ParseToDateTimeOffsetOrDefault(@event.TimeEnd));
    }

    static DateTimeOffset ParseToDateTimeOffsetOrDefault(string? s)
    {
        return DateTimeOffset.TryParse(s, out var dt) ? dt : default;
    }
}
