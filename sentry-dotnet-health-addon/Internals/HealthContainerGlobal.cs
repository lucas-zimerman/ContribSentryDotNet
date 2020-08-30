namespace sentry_dotnet_health_addon.Internals
{
    internal class HealthContainerGlobal : IHealthContainer
    {
        internal Session Session;

        public void CreateNewSession(Session session)
        {
            Session = session;
        }

        public Session GetCurrent()
        {
            return Session;
        }
    }
}
