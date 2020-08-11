using Sentry;
using sentry_dotnet_transaction_addon.Enums;
using sentry_dotnet_transaction_addon.Extensibility;
using sentry_dotnet_transaction_addon.Interface;
using sentry_dotnet_transaction_addon.Internals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sentry_dotnet_transaction_addon
{
    public class SentryTracing : ISentryTracing
    {
        public List<ISpanBase> Spans { get; private set; }
        public DateTimeOffset StartTimestamp { get; private set; }

        internal Trace Trace { get; private set; }
        public string Transaction { get; private set; }
        public SentryTracing(string name)
        {
            Trace = new Trace();
            Transaction = name;
            StartTimestamp = DateTimeOffset.Now;
            Spans = new List<ISpanBase>();
        }

        public ISpanBase GetSpan(string op)
        {
            return Spans.FirstOrDefault(s => s.Op == op);
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
            span.Spans = Spans;
            Spans.Add(span);
            return span;
        }

        public void Finish()
        {
            if (SentryTracingSdk.IsEnabled() && new Random().NextDouble() <= SentryTracingSdk.TracingOptions.TracesSampleRate)
            {
                var @event = new SentryTracingEvent(this);
                if (SentryTracingSdk.TracingOptions.RegisterTracingBreadcrmub)
                {
                    SentrySdk.AddBreadcrumb(@event.EventId.ToString(), "sentry.transaction");
                }
                SentrySdk.WithScope(scope =>
                {
                    SentrySdk.CaptureEvent(@event);
                });
            }
            SentryTracingSdk.DisposeTracingEvent(this);
        }

        public void Dispose()
        {
            Finish();
        }
    }
}
