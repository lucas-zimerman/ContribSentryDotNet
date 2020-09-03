using Newtonsoft.Json;
using sentry_dotnet_health_addon.Enums;
using sentry_dotnet_health_addon.Extensions;

namespace sentry_dotnet_health_addon.Internals
{
    public class SentryItemType
    {
        [JsonProperty("type")]
        private string _type => Type.ConvertString();

        [JsonIgnore]
        public ESentryType Type { get; private set; }

        public SentryItemType(ESentryType type)
        {
            Type = type;
        }
    }
}
