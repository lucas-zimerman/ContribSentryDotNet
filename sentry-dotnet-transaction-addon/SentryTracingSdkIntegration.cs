using Sentry;
using Sentry.Integrations;


namespace sentry_dotnet_transaction_addon
{
    public class SentryTracingSdkIntegration : ISdkIntegration
    {
        internal SentryTracingOptions _options;
        public SentryTracingSdkIntegration()
        {
            _options = new SentryTracingOptions();
        }

        public SentryTracingSdkIntegration(SentryTracingOptions options)
        {
            _options = options;
        }

        public void Register(IHub hub, SentryOptions options)
        {
            _options.Dsn = options.Dsn;
            SentryTracingSdk.Init(_options);
            _options = null;
            options.AddEventProcessor(new SentryTracingEventProcessor());
        }
    }
}
