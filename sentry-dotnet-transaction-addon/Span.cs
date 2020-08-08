using Newtonsoft.Json;
using System;
using sentry_dotnet_transaction_addon.Extensions;

namespace sentry_dotnet_transaction_addon
{
    public class Span
    {
        #region Properties
        [JsonProperty("description")]
        public string Description;

        [JsonProperty("op")]
        public string Op;

        [JsonProperty("span_id")]
        public string SpanId;

        [JsonProperty("parent_span_id")]
        public string ParentSpanId;

        [JsonProperty("start_timestamp")]
        public DateTimeOffset StartTimestamp;

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp;

        [JsonProperty("trace_id")]
        public string TraceId;
        #endregion
        public Span(string description, string op = null)
        {
            StartTimestamp = DateTimeOffset.Now;
            Description = description;
            Op = op ?? description;
            SpanId = Guid.NewGuid().LimitLength();
        }

        public void Finish()
        {
            Timestamp = DateTimeOffset.Now;
        }
    }
}
