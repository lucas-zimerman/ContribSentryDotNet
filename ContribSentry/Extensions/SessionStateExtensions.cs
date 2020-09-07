using ContribSentry.Enums;

namespace ContribSentry.Extensions
{
    public static class SessionStateExtensions
    {
        public static string ConvertString(this ESessionState state)
        {
            if (state == ESessionState.Ok)
                return "ok";
            if (state == ESessionState.Exited)
                return "exited";
            if (state == ESessionState.Crashed)
                return "crashed";
            if( state == ESessionState.Abnormal)
                return "abnormal";
            return null;
        }
    }
}
