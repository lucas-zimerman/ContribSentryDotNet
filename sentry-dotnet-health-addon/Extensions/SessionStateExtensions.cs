using sentry_dotnet_health_addon.Enums;

namespace sentry_dotnet_health_addon.Extensions
{
    public static class SessionStateExtensions
    {
        public static string ConvertString(this SessionState state)
        {
            if (state == SessionState.Ok)
                return "ok";
            if (state == SessionState.Exited)
                return "exited";
            if (state == SessionState.Crashed)
                return "crashed";
            if( state == SessionState.Abnormal)
                return "abnormal";
            return null;
        }
    }
}
