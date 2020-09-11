using ContribSentry.Cache;
using ContribSentry.Interface;
using ContribSentry.Transport;
using Sentry.Protocol;
using System.IO;
using System.Threading.Tasks;

namespace ContribSentry.Internals
{
    internal class EndConsumerService : IEndConsumerService
    {
        public void CaptureSession(ISession session)
        {
            ContribSentrySdk.Options.DiagnosticLogger?.Log(SentryLevel.Debug, $"ContribSentry Capturing Session And Caching it");

            var envelope = SentryEnvelope.FromSession(session,
                ContribSentrySdk.Options.ContribSdk,
                ContribSentrySdk.@Serializer);

            Task.Run(async () => {
                var data = GetCacheFromEnvelope(envelope);
                ContribSentrySdk.EnvelopeCache.Store(data);
                var sent = await HttpTransport.Send(data);
                if (sent)
                {
                    ContribSentrySdk.EnvelopeCache.Discard(data);
                }
            });
        }

        public void CaptureTracing(SentryTracingEvent tracing)
        {
            ContribSentrySdk.Options.DiagnosticLogger?.Log(SentryLevel.Debug, $"ContribSentry Capturing Tracing {tracing.EventId} and Caching it.");

            var envelope = SentryEnvelope.FromTracing(tracing,
                ContribSentrySdk.Options.ContribSdk,
                ContribSentrySdk.@Serializer);

            Task.Run(async () => {
                var data = GetCacheFromEnvelope(envelope);
                ContribSentrySdk.EnvelopeCache.Store(data);
                var sent = await HttpTransport.Send(data);
                if (sent)
                {
                    ContribSentrySdk.EnvelopeCache.Discard(data);
                }
            });
        }

        public async Task<bool> CaptureCachedEnvelope(CachedSentryData envelope)
        {
            return await HttpTransport.Send(envelope);
        }

        private CachedSentryData GetCacheFromEnvelope(SentryEnvelope envelope)
        {
            var memoryStream = new MemoryStream();
            ContribSentrySdk.@Serializer.Serialize(envelope, memoryStream);
            var cache = new CachedSentryData(envelope.Header.EventId,memoryStream.ToArray(), envelope.Items[0].Type.Type);
            memoryStream.Close();
            return cache;
        }
    }
}
