using Newtonsoft.Json;

namespace sentry_dotnet_transaction_addon.Internals
{
    internal class EventIdSentData
    {
        [JsonProperty("event_id")]
        public string EventId { get; set; }
        [JsonProperty("sent_at")]
        public string SentAt { get; set; }
    }
}
