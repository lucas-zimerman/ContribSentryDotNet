using Sentry;
using Sentry.Extensibility;

namespace ContribSentry.Internals.EventProcessor
{
    internal class SentryTracingEventProcessor : ISentryEventProcessor
    {
        public SentryEvent Process(SentryEvent @event)
        {
            if (@event is SentryTracingEvent performanceEvent)
            {

                ContribSentrySdk.EndConsumer.CaptureTracing(performanceEvent);
                return null;
            }
            return @event;
        }
    }
}