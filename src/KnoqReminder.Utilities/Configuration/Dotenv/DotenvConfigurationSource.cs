using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Ini;

namespace KnoqReminder.Utilities.Configuration.Dotenv;

sealed class DotenvConfigurationSource : IniStreamConfigurationSource
{
    public override IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new DotenvConfigurationProvider(this);
    }
}
