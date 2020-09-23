using Newtonsoft.Json;
using ContribSentry.Enums;

namespace ContribSentry.Internals
{
    public class SentryItemType
    {
        [JsonConverter(typeof(SentryTypeJsonConverter))]
        [JsonProperty("type")]
        public ESentryType Type { get; private set; }

        public SentryItemType(ESentryType type)
        {
            Type = type;
        }
    }
}