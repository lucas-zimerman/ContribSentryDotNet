using ContribSentry.Transaction;
using Sentry;
using Sentry.Extensibility;

namespace ContribSentry.Internals.EventProcessor
{
    internal class SentryTracingEventProcessor : ISentryEventProcessor
    {
        public SentryEvent Process(SentryEvent @event)
        {
            if (@event.Message?.Message == SentryTracing.TracingEventMessageKey &&
                @event.Contexts[SentryTracing.TracingEventMessageKey] is SentryTracingEvent tracing)
            {
                tracing.SetSentryEvent(@event);
                ContribSentrySdk.EndConsumer.CaptureTracing(tracing);
                return null;
            }
            return @event;
        }
    }
}