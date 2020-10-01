using Sentry;
using Sentry.Extensibility;
using Sentry.Protocol;

namespace ContribSentry.Internals.EventProcessor
{
    internal class SentryOfflineEventProcessor : ISentryEventProcessor
    {
        public SentryEvent Process(SentryEvent @event)
        {
            if (ContribSentrySdk.Options.HasInternet != null)
            {
                @event.Contexts.Device.IsOnline = ContribSentrySdk.Options.HasInternet();
                if (@event.Contexts.Device.IsOnline == false)
                {
                    if (ContribSentrySdk.Options.BeforeSend != null)
                        @event = ContribSentrySdk.Options.BeforeSend(@event);

                    if (@event != null)
                    {
                        ContribSentrySdk.EventCache.Store(@event);
                        ContribSentrySdk.Options.DiagnosticLogger?.Log(SentryLevel.Debug, $"ContribSentry Event {@event.EventId} was Cached due to lack of Internet.");
                    }
                    return null;
                }
            }
            return @event;
        }
    }
}
