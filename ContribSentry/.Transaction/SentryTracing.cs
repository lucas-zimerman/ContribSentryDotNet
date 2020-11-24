using ContribSentry.Enums;
using ContribSentry.Extensibility;
using ContribSentry.Interface;
using ContribSentry.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;

namespace ContribSentry
{
    public class SentryTracing : ISentryTracing
    {
        public List<ISpanBase> Spans { get; private set; }
        public DateTimeOffset StartTimestamp { get; private set; }
        internal Trace Trace { get; private set; }
        public string Transaction { get; private set; }
        public bool HasError { get; set; }
        public ConcurrentDictionary<string, object> Extra { get; set; }

        public SentryTracing(string name, string op)
        {
            Trace = new Trace(op);
            Transaction = name;
            StartTimestamp = DateTimeOffset.UtcNow;
            Spans = new List<ISpanBase>();
            Extra = new ConcurrentDictionary<string, object>();
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
            => ContribSentrySdk.CaptureTransaction(this, null);

        public void Finish(Exception ex)
            => ContribSentrySdk.CaptureTransaction(this, ex);
    }
}