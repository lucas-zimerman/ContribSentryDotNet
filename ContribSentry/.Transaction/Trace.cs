using Newtonsoft.Json;
using ContribSentry.Extensions;
using System;
using System.Diagnostics;

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
        public Trace()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(5) ?? st.GetFrame(4) ?? st.GetFrame(3) ?? st.GetFrame(2) ?? st.GetFrame(1);

            Op = sf?.GetMethod().Name;
            TraceId = Guid.NewGuid().LimitLength(100);
            SpanId = Guid.NewGuid().LimitLength();

        }
    }
}
