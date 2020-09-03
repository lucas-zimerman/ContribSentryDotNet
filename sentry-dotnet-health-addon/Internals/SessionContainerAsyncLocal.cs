using System.Threading;

namespace sentry_dotnet_health_addon.Internals
{
    internal class SessionContainerAsyncLocal : ISessionContainer
    {
        internal AsyncLocal<Session> Sessions = new AsyncLocal<Session>();
        public void CreateNewSession(Session session)
        {
            Sessions.Value = session;
        }

        public Session GetCurrent()
        {
            return Sessions.Value;
        }

        public void Clear()
        {
            //TODO: Clear AsyncLocal?
        }
    }
}
