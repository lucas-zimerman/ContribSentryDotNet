using Newtonsoft.Json;
using sentry_dotnet_transaction_addon.Extensions;
using System;
using System.Diagnostics;

namespace sentry_dotnet_transaction_addon
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

        #endregion
        public Trace()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(5);

            Op = sf.GetMethod().Name;
            TraceId = Guid.NewGuid().LimitLength(100);
            SpanId = Guid.NewGuid().LimitLength();

        }
    }
}
