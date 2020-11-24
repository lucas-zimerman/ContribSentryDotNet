using ContribSentry.Cache;
using System.Threading.Tasks;

namespace ContribSentry.Interface
{
    /// <summary>
    /// Consumes the given objects and send to Sentry or Cache it.
    /// </summary>
    public interface IContribSentryTransport
    {
        void CaptureTracing(SentryTracingEvent tracing);
        void CaptureSession(ISession session);
        void CacheCurrentSession(ISession session);
        Task<bool> CaptureCachedEnvelope(CachedSentryData envelope);
    }
}