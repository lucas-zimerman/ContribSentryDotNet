using ContribSentry.Internals;

namespace ContribSentry.Interface
{
    interface IEnvelopeCache
    {
        void Store(SentryEnvelope envelope);
        void Discard(SentryEnvelope envelope);
    }
}
