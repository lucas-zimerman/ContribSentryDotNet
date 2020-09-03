using Newtonsoft.Json;
using Sentry.Protocol;
using System;

namespace sentry_dotnet_health_addon.Internals
{
    public class SentryEnvelopeHeader
    {
        /// <summary>
        ///Event Id must be set if the envelope holds an event, or an item that is related to the event.<br/>
        /// (e.g: attachments, user feedback)
        /// </summary>
        [JsonProperty("eventId")]
        public SentryId EventId { get; private set; }

        [JsonProperty("sdkVersion")]
        public SdkVersion SdkVersion { get; private set; }
        
        [JsonProperty("sentAt")]
        public string SentAt { get; private set; }

        internal SentryEnvelopeHeader(SentryId eventId, SdkVersion version)
        {
            EventId = eventId;
            SdkVersion = version;
            SentAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:MM:ss.ffZ");
        }
    }
}
