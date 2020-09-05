using Newtonsoft.Json;
using ContribSentry.Enums;
using ContribSentry.Extensions;

namespace ContribSentry.Internals
{
    public class SentryItemType
    {
        /// <summary>
        /// Type is only used for the json serialization.
        /// </summary>
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
