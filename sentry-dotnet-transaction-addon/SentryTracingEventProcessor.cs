using Sentry;
using Sentry.Extensibility;

namespace sentry_dotnet_transaction_addon
{
    internal class SentryTracingEventProcessor : ISentryEventProcessor
    {
        public SentryEvent Process(SentryEvent @event)
        {
            if (@event is SentryTracingEvent performanceEvent)
            {
                performanceEvent.SendTransaction();
                return null;
            }
            return @event;
        }
    }
}