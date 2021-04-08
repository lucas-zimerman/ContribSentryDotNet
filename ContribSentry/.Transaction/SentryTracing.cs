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
using System.Collections.Concurrent;

namespace ContribSentry
{
    public class SentryTracing : ISentryTracing
    {
        internal static string TracingEventMessageKey => "@ContribSentryTransaction@";

        public List<ISpanBase> Spans { get; private set; }
        public DateTimeOffset StartTimestamp { get; private set; }

        [JsonIgnore]
        public int TrackerId { get; private set; }

        internal Trace Trace { get; private set; }
        public string Transaction { get; private set; }

        public bool HasError { get; set; }

        public ConcurrentDictionary<string, object> Extra { get; set; }

        public SentryTracing(string name, int trackerId)
        {
            Trace = new Trace();
            TrackerId = trackerId;
            Transaction = name;
            StartTimestamp = DateTimeOffset.UtcNow;
            Spans = new List<ISpanBase>();
            Extra = new ConcurrentDictionary<string, object>();
        }

        public ISpanBase GetSpan(string op)
            => Spans.FirstOrDefault(s => s.Op == op) ?? DisabledSpan.Instance;

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
            if (ContribSentrySdk.IsTracingSdkEnabled && new Random().NextDouble() <= ContribSentrySdk.Options.TracesSampleRate)
            {
                var hasError = Spans.Any(p => p.Error);
                Trace.SetStatus(Spans.LastOrDefault()?.Status);

                var @dumpEvent = new SentryEvent()
                {
                    Message = TracingEventMessageKey
                };

                var @tracing = new SentryTracingEvent(this, hasError)
                {
                    EventId = dumpEvent.EventId
                };
                if (ContribSentrySdk.Options.RegisterTracingBreadcrumb)
                {
                    SentrySdk.AddBreadcrumb(@tracing.EventId.ToString(), "sentry.transaction");
                }
                SentrySdk.WithScope(scope =>
                {

                    dumpEvent.Contexts[TracingEventMessageKey] = @tracing;
                    SentrySdk.CaptureEvent(@dumpEvent);
                });
            }
            ContribSentrySdk.TracingService.DisposeTracingEvent(this);
        }

        public void Dispose()
        {
            Finish();
        }

        public Task IsolateTracking(Func<Task> trackedCode)
        {
            return ContribSentrySdk.TracingService.StartCallbackTrackingIdAsync(trackedCode, TrackerId);
        }
    }
}