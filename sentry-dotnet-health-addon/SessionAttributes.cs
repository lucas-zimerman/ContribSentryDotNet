using Newtonsoft.Json;

namespace sentry_dotnet_health_addon
{
    public class SessionAttributes
    {
        /** the user's ip address */
        [JsonProperty("ip_address")]
        public string IpAddress { get; private set; }

        /** the user Agent */
        [JsonProperty("user_agent")]
        internal string UserAgent { get; set; }

        /** the environment */
        [JsonProperty("environment")]
        public string Environment { get; private set; }

        /** the App's release */
        [JsonProperty("release")]
        public string Release { get; private set; }

        public SessionAttributes(string ipAddress, string userAgent, string environment, string release)
        {
            IpAddress = ipAddress;
            UserAgent = userAgent;
            Environment = environment;
            Release = release;
        }
    }
}
