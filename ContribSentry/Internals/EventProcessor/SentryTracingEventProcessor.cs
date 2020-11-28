using Sentry;
using System;

namespace ContribSentry.Internals.EventProcessor
{
    internal class SentryTracingEventProcessor
    {
        private ContribSentryOptions _options = null;
        private Random _random = new Random();
        internal SentryTracingEventProcessor(ContribSentryOptions options)
        {
            _options = options;
        }

        internal SentryTracing Process(SentryTracing tracing, Scope scope)
        {
            if (_random.NextDouble() <= _options.TracesSampleRate)
            {
                tracing.Level = tracing.HasError ? SentryLevel.Error : SentryLevel.Info;
                tracing.Contexts = scope.Contexts;
                tracing.Contexts.AddOrUpdate("trace", tracing.Trace, (id, trace) => trace);
                tracing.Breadcrumbs = scope.Breadcrumbs;
                tracing.Environment = scope.Environment;
                tracing.Sdk = scope.Sdk;
                tracing.User = scope.User;
                return tracing;
            }
            _options.DiagnosticLogger?.Log(SentryLevel.Debug, "ContribSentry Tracing {0} discarted due to sampling.", args: new object[1] { tracing.EventId });
            return null;
        }
    }
}