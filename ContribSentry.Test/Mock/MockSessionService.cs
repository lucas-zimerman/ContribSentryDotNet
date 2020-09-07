using ContribSentry.Extensibility;
using ContribSentry.Interface;
using Sentry.Protocol;

namespace ContribSentry.Test.Mock
{
    public class MockSessionService : IContribSentrySessionService
    {
        public void Close() { }

        public void EndSession() { }
        public ISession GetCurrent() => DisabledSession.Instance;

        public void Init(ContribSentryOptions options, IEndConsumerService endConsumer) { }

        public void StartSession(User user, string distinctId, string environment, string release) { }
    }
}
