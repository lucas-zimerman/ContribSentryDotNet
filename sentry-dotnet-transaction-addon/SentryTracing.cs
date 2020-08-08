using Sentry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sentry_dotnet_transaction_addon
{
    public class SentryTracing : ITransactionEvent, IDisposable
    {
        public List<Span> Spans { get; private set; }
        public DateTimeOffset StartTimestamp { get; private set; }

        internal Trace Trace { get; private set; }
        public string Transaction { get; private set; }
        public SentryTracing(string name)
        {
            Trace = new Trace();
            Transaction = name;
            StartTimestamp = DateTimeOffset.Now;
            Spans = new List<Span>();
        }

        public string Id => Guid.NewGuid().ToString();

        public Span GetSpan(string op)
        {
            return Spans.FirstOrDefault(s => s.Op == op);
        }

        public Span StartChild(string description, string op = null)
        {
            var span = new Span(description, op);
            SetTraceToSpan(span);
            Spans.Add(span);
            return span;
        }

        private void SetTraceToSpan(Span span)
        {
            span.TraceId = Trace.TraceId;
            span.ParentSpanId = Trace.SpanId;
        }

        public void Finish()
        {
            if (SentryTracingSDK.IsEnabled() && new Random().NextDouble() <= SentryTracingSDK.TracingOptions.TracesSampleRate)
            {
                var @event = new SentryTracingEvent(this);
                if (SentryTracingSDK.TracingOptions.RegisterTracingBreadcrmub)
                {
                    SentrySdk.AddBreadcrumb(@event.EventId.ToString(), "sentry.transaction");
                }
                SentrySdk.WithScope(scope =>
                {
                    SentrySdk.CaptureEvent(@event);
                });
            }
            SentryTracingSDK.DisposeTracingEvent(this);
        }

        public void Dispose()
        {
            Finish();
        }
    }
}
