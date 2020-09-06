using Sentry;
using Sentry.Extensibility;
using Sentry.Protocol;
using ContribSentry.Enums;
using System;
using System.Linq;

namespace ContribSentry.Internals.EventProcessor
{
    internal class SentrySessionEventProcessor : ISentryEventProcessor
    {
        public SentryEvent Process(SentryEvent @event)
        {
            if (@event.Level == SentryLevel.Error ||
                @event.Level == SentryLevel.Fatal)
            {
                var session = ContribSentrySdk.SessionService.GetCurrent();
                session?.RegisterError();
                if (session != null && @event.SentryExceptions.Any(e => e.Mechanism?.Handled == false))
                {
                    //crash, must close the session
                    session.End(DateTime.Now);
                    session.Status = ESessionState.Crashed;
                    ContribSentrySdk.EndConsumer.CaptureSession(session);
                }
            }
            return @event;
        }
    }
}