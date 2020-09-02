using Sentry.Extensibility;
using Sentry.Protocol;
using sentry_dotnet_health_addon.Enums;
using sentry_dotnet_health_addon.Internals;
using sentry_dotnet_health_addon.Transport;
using System;

namespace sentry_dotnet_health_addon
{
    public static class SentrySessionSdk
    {
        internal static ISessionContainer HealthContainer;

        internal static SentrySessionOptions Options;

        internal static DistinctiveId IdHandler;

        internal static Serializer @Serializer;

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
                HealthContainer = new SessionContainerGlobal();
            else
                HealthContainer = new SessionContainerAsyncLocal();
            Options = options;
            IdHandler = new DistinctiveId();
            @Serializer = new Serializer();
        }

        public static void Close()
        {
            if (IsEnabled)
            {
                EndSession();
            }
            HealthContainer = null;
            Options = null;
            IdHandler = null;
            @Serializer = null;
        }

        public static void StartSession(User user)
        {
            HealthContainer.CreateNewSession(new Session(ResolveDistinctId(user), user, Options.Environment, Options.Release));
        }

        public static void EndSession()
        {
            var session = HealthContainer.GetCurrent();
            if (session == null || session is DisabledHub || session.Status == SessionState.Exited)
                return;
            session.End(DateTime.Now);
            CaptureSession(session);
        }

        internal static void CaptureSession(ISession session)
        {
            var envelope = SentryEnvelope.FromSession(session,
                new SdkVersion() { Name = "LucasSdk", Version = "1.0.0" },
                @Serializer);
            //Todo: SOLVE THIS!!!
            _ = HttpTransport.Send(envelope, @Serializer);

        }
        private static string ResolveDistinctId(User user)
        {
            if (Options?.DistinctId != null)
                return IdHandler.GetDistinctiveId(Options.DistinctId);
            return IdHandler.GetDistinctiveId(user);
        }
    }
}
