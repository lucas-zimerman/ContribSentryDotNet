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
            _options.ConsumeSentryOptions(options);
            ContribSentrySdk.Init(_options);

            if (_options.SessionEnabled) 
                options.AddEventProcessor(new SentrySessionEventProcessor());
            if (_options.CacheEnabled)
                options.AddEventProcessor(new SentryOfflineEventProcessor());
            _options = null;
        }
    }
}
