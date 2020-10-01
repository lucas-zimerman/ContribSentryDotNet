using ContribSentry.Extensibility;
using ContribSentry.Interface;
using Sentry.Protocol;

namespace ContribSentry.Test.Mock
{
    public class MockSessionService : IContribSentrySessionService
    {

        public void Init(ContribSentryOptions options, IEndConsumerService endConsumer) { }
        public void Close() { }
        public ISession GetCurrent() => DisabledSession.Instance;
        public void StartSession(User user, string distinctId, string environment, string release) { }
        public void EndSession() { }
        public void DeleteCachedCurrentSession() { }
        public void CacheCurrentSesion() { }
    }
}
