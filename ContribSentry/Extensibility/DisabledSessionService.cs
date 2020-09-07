using ContribSentry.Interface;
using Sentry.Protocol;

namespace ContribSentry.Extensibility
{
    internal class DisabledSessionService : IContribSentrySessionService
    {
        internal static DisabledSessionService Instance = new DisabledSessionService();
        public void Close() { }

        public void EndSession() { }

        public ISession GetCurrent() => DisabledSession.Instance;

        public void Init(ContribSentryOptions options, IEndConsumerService endConsumer) { }

        public void StartSession(User user, string distinctId, string environment, string release) { }
    }
}
