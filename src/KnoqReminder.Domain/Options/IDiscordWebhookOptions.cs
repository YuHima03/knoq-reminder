using System;
using System.Collections.Generic;
using System.Text;

namespace KnoqReminder.Domain.Options;

public interface IDiscordWebhookOptions
{
    Uri? WebhookBaseUrl { get; }
}
