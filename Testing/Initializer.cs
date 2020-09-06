using Sentry;
using Testing.Helpers;

namespace Testing
{
    public static class Initializer
    {
        public static void Init()
        {
            if (!SentryTracingSdk.IsEnabled())
            {
                var integration = new SentryTracingSdkIntegration();
                integration.Register(null, new SentryOptions());
                SentryTracingSdk.SetDsn(new Dsn(DsnHelper.ValidDsnWithoutSecret));
            }
        }
    }
}
