using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnoqReminder.Domain.Options;

public interface ITraqApiClientOptions
{
    string BaseUrl { get; }

    string AccessToken { get; }
}
