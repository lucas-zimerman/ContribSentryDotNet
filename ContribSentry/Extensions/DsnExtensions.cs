using Sentry;

namespace ContribSentry.Extensions
{
    internal static class DsnExtensions
    {
        internal static string GetEnvelopeUrl(this Dsn dsn)
        {
            return $"{dsn.SentryUri.Scheme}://{dsn.SentryUri.Host}/api/{dsn.ProjectId}/envelope/?sentry_key={dsn.PublicKey}&sentry_version=7";
        }

        internal static string GetEventUrl(this Dsn dsn)
        {
            return $"{dsn.SentryUri.Scheme}://{dsn.SentryUri.Host}/api/{dsn.ProjectId}/store/?sentry_key={dsn.PublicKey}";
        }
    }
}
