using ContribSentry.Enums;
using ContribSentry.Extensibility;
using ContribSentry.Interface;
using ContribSentry.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using Sentry.Protocol;
using Sentry;
using Newtonsoft.Json;

namespace ContribSentry
{
    public class SentryTracing : ISentryTracing
    {
        #region SentryEventSpecificData
        [JsonProperty("event_id")]
        public SentryId EventId { get; private set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; private set; }
        public IScopeOptions ScopeOptions { get; }
        [JsonProperty("level")]
        public SentryLevel? Level { get; set; }

        [JsonProperty("request")]
        public Request Request { get; set; }

        [JsonProperty("contexts")]
        public Contexts Contexts { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("environment")]
        public string Environment { get; set; }

        [JsonProperty("sdk")]
        public SdkVersion Sdk { get; internal set; }

        [JsonProperty("fingerprint")]
        public IEnumerable<string> Fingerprint { get; set; }

        [JsonProperty("breadcrumbs")]
        public IEnumerable<Breadcrumb> Breadcrumbs { get; internal set; }

        [JsonProperty("extra")]
        IReadOnlyDictionary<string, object> IScope.Extra { get; }

        [JsonProperty("tags")]
        public IReadOnlyDictionary<string, string> Tags { get; internal set; }

        #endregion
        [JsonProperty("type")]
        public string Type => "transaction";

        [JsonProperty("spans")]
        public List<ISpanBase> Spans { get; private set; }

        [JsonProperty("start_timestamp")]
        public DateTimeOffset StartTimestamp { get; private set; }

        [JsonIgnore]
        internal Trace Trace { get; private set; }

        [JsonProperty("transaction")]
        public string Transaction { get; set; }

        [JsonIgnore]
        public bool HasError { get; set; }

        public SentryTracing(string name, string op)
        {
            EventId = new SentryId();
            Trace = new Trace(op);
            Transaction = name;
            StartTimestamp = DateTimeOffset.UtcNow;
            Spans = new List<ISpanBase>();
            Contexts = new Contexts();
            Tags = new Dictionary<string, string>();
        }

        public ISpanBase GetCurrentSpan()
        {
            return Spans.LastOrDefault(s => s.Timestamp == s.StartTimestamp && s.ParentSpanId == Trace.SpanId) ?? DisabledSpan.Instance;
        }

        public ISpanBase StartChild(string description, string op = null)
        {
            var span = new Span(Trace.TraceId, Trace.SpanId,  description, op);
            span.GetParentSpans(Spans);
            Spans.Add(span);
            return span;
        }
        public ISpanBase StartChild(string url, ESpanRequest requestType)
        {
            var span = new Span(Trace.TraceId, Trace.SpanId, url, requestType);
            span.GetParentSpans(Spans);
            Spans.Add(span);
            return span;
        }

        public void Finish()
        {
            Timestamp = DateTimeOffset.UtcNow;
            ContribSentrySdk.CaptureTransaction(this, null);
        }

        public void Finish(Exception ex)
        {
            Timestamp = DateTimeOffset.UtcNow;
            ContribSentrySdk.CaptureTransaction(this, ex);
        }
    }
}