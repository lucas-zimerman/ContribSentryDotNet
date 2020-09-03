using Sentry;
using Sentry.Integrations;

namespace sentry_dotnet_health_addon
{
    public class SentrySessionSdkIntegration : ISdkIntegration
    {
        internal SentrySessionOptions _options;
        public SentrySessionSdkIntegration()
        {
            _options = new SentrySessionOptions();
        }

        public SentrySessionSdkIntegration(SentrySessionOptions options)
        {
            _options = options;
        }

        public void Register(IHub hub, SentryOptions options)
        {
            _options.Dsn = options.Dsn;
            _options.Environment = options.Environment;
            _options.Release = options.Release;            
            SentrySessionSdk.Init(_options);
            _options = null;
            options.AddEventProcessor(new SentryHealthEventProcessor());
        }
    }
}
