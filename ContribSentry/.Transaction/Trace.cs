using Newtonsoft.Json;
using ContribSentry.Extensions;
using System;

namespace ContribSentry
{
    internal class Trace
    {
        #region Properties
        [JsonProperty("op")]
        public string Op { get; private set; }

        [JsonProperty("span_id")]
        public string SpanId { get; private set; }

        [JsonProperty("trace_id")]
        public string TraceId { get; private set; }

        [JsonProperty("status")]
        public static string Status { get; private set; }

        public void SetStatus(string status) => Status = status;
        #endregion
        public Trace(string op)
        {
            Op = op;
            TraceId = Guid.NewGuid().LimitLength(100);
            SpanId = Guid.NewGuid().LimitLength();
        }
    }
}
