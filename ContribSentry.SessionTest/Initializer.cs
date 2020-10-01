using System;
using System.Collections.Generic;
using System.Text;
using Sentry;
using ContribSentry;
using SessionSdk.Helpers;

namespace SessionSdk.Test
{
    public static class Initializer
    {
        public const string TestRelease = "--1";
        public const string TestEnvironment = "test";
        public static void Init()
        {
            if (!ContribSentrySdk.IsEnabled)
            {
                var integration = new ContribSentrySdkIntegration(new ContribSentryOptions() { GlobalSessionMode = true });
                integration.Register(null, new SentryOptions() { Release = TestRelease, Environment = TestEnvironment, Dsn = new Dsn(Helpers.DsnHelper.ValidDsnWithoutSecret) });
            }
        }
    }
}