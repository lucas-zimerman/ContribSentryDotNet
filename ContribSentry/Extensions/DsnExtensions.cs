using ContribSentry.Internals;
using Sentry;

namespace ContribSentry.Extensions
{
    internal static class DsnExtensions
    {
        internal static string GetEnvelopeUrl(this ContribDsn dsn)
        {
            return $"{dsn.GetEnvelopeEndpointUri()}/?sentry_key={dsn.PublicKey}&sentry_version=7";
        }

        internal static string GetEventUrl(this ContribDsn dsn)
        {
            return $"{dsn.GetStoreEndpointUri()}/?sentry_key={dsn.PublicKey}";
        }
    }
}
