using Sentry;
using Sentry.Protocol;
using sentry_dotnet_health_addon.Integrations;
using sentry_dotnet_health_addon.Internals;
using sentry_dotnet_health_addon.Transport;
using System;
using System.Collections.Generic;
using System.Text;

namespace sentry_dotnet_health_addon
{
    public static class SentrySessionSdk
    {
        internal static IHealthContainer HealthContainer;

        internal static SentrySessionOptions Options;

        internal static DistinctiveId IdHandler;

        internal static ProccessExitIntegration Integration;

        public static bool IsEnabled => Options != null;

        public static ISession GetCurrent()
        {
            if (!IsEnabled)
                return null;
            return HealthContainer.GetCurrent();
        }

        internal static void Init(SentrySessionOptions options)
        {
            if (options.GlobalHubMode)
                HealthContainer = new HealthContainerGlobal();
            else
                HealthContainer = new HealthContainerAsyncLocal();
            Options = options;
            IdHandler = new DistinctiveId();
            Integration = new ProccessExitIntegration();
        }

        public static void Close()
        {
            HealthContainer = null;
            Options = null;
            IdHandler = null;
            Integration.Dispose();
        }

        public static void StartSession(User user)
        {
            HealthContainer.CreateNewSession(new Session(ResolveDistinctId(user), user, Options.Environment, Options.Release));
        }

        public static void EndSession()
        {
            var session = HealthContainer.GetCurrent();
            session.End(DateTime.Now);
            _ = HttpTransport.SendEnvelope(session);

        }

        private static string ResolveDistinctId(User user)
        {
            if (Options?.DistinctId != null)
                return IdHandler.GetDistinctiveId(Options.DistinctId);
            return IdHandler.GetDistinctiveId(user);
        }
    }
}
