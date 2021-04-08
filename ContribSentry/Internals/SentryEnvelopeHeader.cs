using Newtonsoft.Json;
using Sentry;
using System;

namespace ContribSentry.Internals
{
    public class SentryEnvelopeHeader
    {
        /// <summary>
        ///Event Id must be set if the envelope holds an event, or an item that is related to the event.<br/>
        /// (e.g: attachments, user feedback)
        /// </summary>
        [JsonProperty("event_id")]
        private string _eventIdString => EventId.Equals(SentryId.Empty) ? null : EventId.ToString();

        [JsonIgnore]
        public SentryId EventId { get; private set; }

        [JsonProperty("sdk")]
        public SdkVersion SdkVersion { get; private set; }
        
        [JsonProperty("sent_at")]
        public string SentAt { get; private set; }

        internal SentryEnvelopeHeader(SentryId eventId, SdkVersion version)
        {
            EventId = eventId;
            SdkVersion = version;
            SentAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:MM:ss.ffZ");
        }
    }
}
