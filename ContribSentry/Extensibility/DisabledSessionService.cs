using ContribSentry.Interface;
using Sentry;
using Sentry.Protocol;

namespace ContribSentry.Extensibility
{
    internal class DisabledSessionService : IContribSentrySessionService
    {
        internal static DisabledSessionService Instance = new DisabledSessionService();

        public void Init(ContribSentryOptions options, IEndConsumerService endConsumer) { }

        public void Close() { }

        public ISession GetCurrent() => DisabledSession.Instance;

        public void StartSession(User user, string distinctId, string environment, string release) { }

        public void EndSession() { }

        public void CacheCurrentSesion() { }

        public void DeleteCachedCurrentSession() { }

    }
}
