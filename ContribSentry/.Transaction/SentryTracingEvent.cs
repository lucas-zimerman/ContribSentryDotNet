using Newtonsoft.Json;
using Sentry;
using ContribSentry.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using Sentry.Protocol;

namespace ContribSentry
{
    public class SentryTracingEvent : SentryEvent
    {
        [JsonProperty("type")]
        public string Type { get; private set; }

        [JsonProperty("spans")]
        public List<ISpanBase> Spans { get; private set; }

        [JsonProperty("start_timestamp")]
        public DateTimeOffset StartTimestamp { get; private set; }

        internal SentryTracingEvent(SentryTracing transactionEvent, bool hasError)
        {
            Transaction = transactionEvent.Transaction;
            Type = "transaction";
            Level = hasError ? SentryLevel.Error : SentryLevel.Info;
            Contexts.AddOrUpdate("trace", transactionEvent.Trace, (id, trace) => trace);
            Spans = transactionEvent.Spans;
            StartTimestamp = transactionEvent.StartTimestamp;
            this.SetExtras(transactionEvent.Extra);
        }
    }
}