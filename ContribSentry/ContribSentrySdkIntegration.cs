using ContribSentry.Internals.EventProcessor;
using Sentry;
using Sentry.Integrations;

namespace ContribSentry
{
    public class ContribSentrySdkIntegration : ISdkIntegration
    {
        internal ContribSentryOptions _options;
        public ContribSentrySdkIntegration()
        {
            _options = new ContribSentryOptions();
        }

        public ContribSentrySdkIntegration(ContribSentryOptions options)
        {
            _options = options;
        }

        public void Register(IHub hub, SentryOptions options)
        {
            _options.Dsn = options.Dsn;
            _options.Environment = options.Environment;
            _options.Release = options.Release;            
            ContribSentrySdk.Init(_options);
            _options = null;
            if (_options.SessionEnabled) 
                options.AddEventProcessor(new SentrySessionEventProcessor());
            if(_options.TransactionEnabled)
                options.AddEventProcessor(new SentryTracingEventProcessor());
        }
    }
}
