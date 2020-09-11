using ContribSentry.Cache;
using ContribSentry.Interface;
using Sentry;
using System.Collections.Generic;

namespace ContribSentry.Extensibility
{
    public class DisabledDiskCache : IEventCache
    {
        public static DisabledDiskCache Instance = new DisabledDiskCache();
        public void Discard(SentryEvent @event) { }

        public void Discard(CachedSentryData @event) { }

        public List<CachedSentryData> Iterator() => new List<CachedSentryData>();

        public void Store(SentryEvent @event) { }
    }
}
