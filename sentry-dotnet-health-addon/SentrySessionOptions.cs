using Sentry;

namespace sentry_dotnet_health_addon
{
    public class SentrySessionOptions
    {
        internal Dsn Dsn { get; set; }
        internal string Environment { get; set; }
        internal string Release { get; set; }

        /// <summary>
        /// True for single user applications like Apps, Otherwise, False.
        /// </summary>
        public bool GlobalHubMode { get; set; }

        /// <summary>
        /// The Device Id or the unique id that represents an user.
        /// </summary>
        public string DistinctId { get; set; }
    }
}
