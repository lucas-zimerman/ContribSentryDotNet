using Sentry;
using ContribSentry.Enums;
using ContribSentry.Extensibility;
using ContribSentry.Interface;
using ContribSentry.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace ContribSentry
{
    public class SentryTracing : ISentryTracing
    {
        public List<ISpanBase> Spans { get; private set; }
        public DateTimeOffset StartTimestamp { get; private set; }
        internal Trace Trace { get; private set; }
        public string Transaction { get; private set; }
        public SentryTracing(string name, string op)
        {
            Trace = new Trace(op);
            Transaction = name;
            StartTimestamp = DateTimeOffset.UtcNow;
            Spans = new List<ISpanBase>();
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
            var @event = new SentryTracingEvent(this);
            if (ContribSentrySdk.Options.RegisterTracingBreadcrumb)
            {
                SentrySdk.AddBreadcrumb(@event.EventId.ToString(), "sentry.transaction");
            }
            SentrySdk.WithScope(scope =>
            {
                SentrySdk.CaptureEvent(@event);
            });
        }

        public void Finish(Exception ex)
        {

        }
    }
}