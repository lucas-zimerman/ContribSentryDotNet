using ContribSentry.Cache;
using ContribSentry.Interface;
using ContribSentry.Internals;
using System;
using System.Collections.Generic;

namespace ContribSentry.Extensibility
{
    public class DisabledEnvelopeCache : IEnvelopeCache
    {
        public static DisabledEnvelopeCache Instance => new DisabledEnvelopeCache();
        public void Discard(SentryEnvelope envelope, Guid? tempId) { }

        public void Discard(CachedSentryData envelope) { }

        public List<CachedSentryData> Iterator() => new List<CachedSentryData>();

        public void Store(CachedSentryData envelope) { }
    }
}