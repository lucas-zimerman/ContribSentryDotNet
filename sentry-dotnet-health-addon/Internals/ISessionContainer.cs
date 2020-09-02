
namespace sentry_dotnet_health_addon.Internals
{
    internal interface ISessionContainer
    {
        Session GetCurrent();

        void CreateNewSession(Session session);

        void Clear();
    }
}
