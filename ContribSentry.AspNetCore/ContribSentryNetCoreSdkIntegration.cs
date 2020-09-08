using ContribSentry.AspNetCore.Internals;
using ContribSentry.Extensibility;
using Sentry;
using Sentry.Integrations;

namespace ContribSentry.AspNetCore
{
    public class ContribSentryNetCoreSdkIntegration : ISdkIntegration
    {
        internal ContribSentryOptions _options;
        public ContribSentryNetCoreSdkIntegration()
        {
            _options = new ContribSentryOptions(sessionEnable:false);
        }

        public ContribSentryNetCoreSdkIntegration(ContribSentryOptions options)
        {
            _options = new ContribSentryOptions(options.TransactionEnabled, false)
            {
                DistinctId = options.DistinctId,
                RegisterTracingBreadcrmub = options.RegisterTracingBreadcrmub,
                TracesSampleRate = options.TracesSampleRate,
            };
            _options.SetTracingService(options.TracingService);
        }

        public void Register(IHub hub, SentryOptions options)
        {
            _options.TrackingIdMethod = new HttpContextTracking();
            var integration = new ContribSentrySdkIntegration(_options);
            integration.Register(hub, options);
        }
    }
}
