using ContribSentry.Enums;
using Sentry;

namespace ContribSentry.Cache
{
    public class CachedSentryData
    {
        public SentryId EventId { get; set; }

        public ESentryType Type { get; private set; }

        public byte[] Data { get; private set; }
        public CachedSentryData(SentryId id, byte[] data, ESentryType type)
        {
            EventId = id;
            Data = data;
            Type = type;
        }
    }
}
