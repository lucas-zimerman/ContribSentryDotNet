using System;
using System.Collections.Generic;
using System.Text;
using Sentry;
using sentry_dotnet_health_addon;
using SessionSdk.Helpers;

namespace SessionSdk.Test
{
    public static class Initializer
    {
        public const string TestRelease = "--1";
        public const string TestEnvironment = "test";
        public static void Init()
        {
            if (!SentrySessionSdk.IsEnabled)
            {
                var integration = new SentrySessionSdkIntegration(new SentrySessionOptions() { GlobalHubMode = true });
                integration.Register(null, new SentryOptions() { Release = TestRelease, Environment = TestEnvironment, Dsn = new Dsn(DsnHelper.ValidDsnWithoutSecret) });
            }
        }
    }
}