using ContribSentry.Cache;
using ContribSentry.Enums;
using ContribSentry.Interface;
using Sentry.Extensibility;
using Sentry.Protocol;
using System;

namespace ContribSentry.Internals
{
    internal class ContribSentrySessionService : IContribSentrySessionService
    {
        internal ISessionContainer HealthContainer;

        internal IEndConsumerService EndConsumer;

        internal DistinctiveId IdHandler;

        private bool _globalSessionMode;

        public void Init(ContribSentryOptions options, IEndConsumerService endConsumer)
        {
            _globalSessionMode = options.GlobalSessionMode;
            if (_globalSessionMode)
                HealthContainer = new SessionContainerGlobal();
            else
                HealthContainer = new SessionContainerAsyncLocal();

            IdHandler = new DistinctiveId();

            EndConsumer = endConsumer;
        }

        public void Close()
        {
            EndSession();

            HealthContainer = null;
            IdHandler = null;
        }

        public void StartSession(User user, string distinctId, string environment, string release)
        {
            HealthContainer.CreateNewSession(new Session(ResolveDistinctId(user, distinctId), user, environment, release));
        }

        private string ResolveDistinctId(User user, string distinctId)
        {
            if (distinctId != null)
                return IdHandler.GetDistinctiveId(distinctId);
            return IdHandler.GetDistinctiveId(user);
        }

        public void EndSession()
        {
            var session = HealthContainer.GetCurrent();
            if (session == null || session is DisabledHub || session.Status == ESessionState.Exited)
                return;
            session.End(DateTime.UtcNow);
            EndConsumer.CaptureSession(session);
            HealthContainer.Clear();
        }

        public ISession GetCurrent() => HealthContainer.GetCurrent();

        public void CacheCurrentSesion() 
        {
            if (_globalSessionMode)
            {
                if(GetCurrent() is Session session)
                {
                    var cached = session.Clone();
                    cached.End();
                    EndConsumer.CacheCurrentSession(cached);
                }
            }
        }

        public void DeleteCachedCurrentSession() 
        {
            if (_globalSessionMode)
            {
                ContribSentrySdk.EnvelopeCache.Discard(new CachedSentryData(SentryId.Empty, null, ESentryType.CurrentSession));
            }
        }

    }
}
