using ContribSentry.Cache;
using System.Collections.Generic;

namespace ContribSentry.Interface
{
    public interface IEnvelopeCache
    {
        void Store(CachedSentryData envelope);
        void Discard(CachedSentryData envelope);
        List<CachedSentryData> Iterator();
    }
}
