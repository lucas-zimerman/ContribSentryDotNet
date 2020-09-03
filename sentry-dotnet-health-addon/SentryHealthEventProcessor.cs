using Sentry;
using Sentry.Extensibility;
using Sentry.Protocol;
using sentry_dotnet_health_addon.Enums;
using System;
using System.Linq;

namespace sentry_dotnet_health_addon
{
    internal class SentryHealthEventProcessor : ISentryEventProcessor
    {
        public SentryEvent Process(SentryEvent @event)
        {
            if (@event.Level == SentryLevel.Error ||
                @event.Level == SentryLevel.Fatal)
            {
                var session = SentrySessionSdk.GetCurrent();
                session?.RegisterError();
                if (session != null && @event.SentryExceptions.Any(e => e.Mechanism?.Handled == false))
                {
                    //crash, must close the session
                    session.End(DateTime.Now);
                    session.Status = SessionState.Crashed;
                    SentrySessionSdk.CaptureSession(session);
                }
            }
            return @event;
        }
    }
}