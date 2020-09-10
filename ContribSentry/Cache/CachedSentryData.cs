using Sentry.Protocol;

namespace ContribSentry.Cache
{
    public class CachedSentryData
    {
        public SentryId EventId { get; private set; }

        public byte[] Data { get; private set; }
        public CachedSentryData(SentryId id, byte[] data)
        {
            EventId = id;
            Data = data;
        }
    }
}
