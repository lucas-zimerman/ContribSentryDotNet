using Sentry;
using Sentry.Extensibility;
using System;

namespace ContribSentry.Internals.EventProcessor
{
    internal class SentryTracingEventProcessor : ISentryEventProcessor
    {
        private ContribSentryOptions _options = null;
        private Random _random = new Random();
        internal SentryTracingEventProcessor(ContribSentryOptions options)
        {
            _options = options;
        }

        public SentryEvent Process(SentryEvent @event)
        {
            if (@event is SentryTracingEvent performanceEvent)
            {
                if (_random.NextDouble() <= _options.TracesSampleRate)
                {
                    ContribSentrySdk.Transport.CaptureTracing(performanceEvent);
                }
                else
                {
                    _options.DiagnosticLogger?.Log(SentryLevel.Debug, "ContribSentry Tracing {0} discarted due to sampling.", args: new object[1] { performanceEvent.EventId });
                }
                return null;
            }
            return @event;
        }
    }
}