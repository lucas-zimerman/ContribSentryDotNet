using Newtonsoft.Json;
using Sentry.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace sentry_dotnet_health_addon.Internals
{
    internal class SentryEnvelopeHeader
    {
        [JsonProperty("eventId")]
        private SentryId _eventId;

        [JsonProperty("sdkVersion")]
        private SdkVersion _sdkVersion;
        
        [JsonProperty("SentAt")]
        private string _sentAt;
        

        internal SentryEnvelopeHeader()
        {
            _eventId = SentryId.Empty;
            _sdkVersion = new SdkVersion() { Name = "LucasSdk", Version = "1.0.0" }; 
            _sentAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:MM:ss.ffZ");
        }
    }
}
