using System.Text;
using KnoqReminder.Domain.Services.DiscordWebhook;
using KnoqReminder.Domain.Services.Localization;
using KnoqReminder.Domain.Services.Reminder;
using KnoqReminder.Domain.Services.Urls;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.ObjectPool;
using Traq;

namespace KnoqReminder.App.Services.Reminder;

sealed partial class ReminderPublisher(
    IDiscordWebhookPublisher discordWebhookPublisher,
    IKnoqUrlProvider knoqUrlProvider,
    ILocalTimeProvider localTimeProvider,
    TraqApiClient traq,
    IMemoryCache cache,
    ObjectPool<Traq.Models.PostMessageRequest> postMessageRequestPool,
    ObjectPool<StringBuilder> stringBuilderPool,
    ILogger<ReminderPublisher> logger,
    ILoggerFactory loggerFactory
    )
    : IReminderPublisher;
