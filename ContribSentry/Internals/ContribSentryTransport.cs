using ContribSentry.Cache;
using ContribSentry.Enums;
using ContribSentry.Interface;
using ContribSentry.Transport;
using Sentry;
using Sentry.Protocol;
using System.IO;
using System.Threading.Tasks;

namespace ContribSentry.Internals
{
    internal class ContribSentryTransport : IContribSentryTransport
    {
        private ContribSentryOptions _options;
        public ContribSentryTransport(ContribSentryOptions options)
        {
            _options = options;
        }

        public void CaptureSession(ISession session)
        {
            _options.DiagnosticLogger?.Log(SentryLevel.Debug, $"ContribSentry Capturing Session And Caching it");

            var envelope = SentryEnvelope.FromSession(session,
                _options.ContribSdk,
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

        public void CacheCurrentSession(ISession session)
        {
            _options.DiagnosticLogger?.Log(SentryLevel.Debug, "ContribSentry Caching current Session");
            var data = GetCacheFromCurrentSession(session);
            ContribSentrySdk.EnvelopeCache.Store(data);
        }

        public void CaptureTracing(SentryTracingEvent tracing)
        {
            _options.DiagnosticLogger?.Log(SentryLevel.Debug, "ContribSentry Capturing Tracing {0} and Caching it.", args: new object[] { tracing.EventId });

            var envelope = SentryEnvelope.FromTracing(tracing,
                _options.ContribSdk,
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

        public async Task<bool> CaptureCachedEnvelope(CachedSentryData cachedData)
        {
            if(cachedData.Type == ESentryType.CurrentSession)
            {
                var envelope = SentryEnvelope.FromSession(cachedData.Data,
                    _options.ContribSdk);
                var cachedEnvelope = GetCacheFromEnvelope(envelope);

                return await HttpTransport.Send(cachedEnvelope);
            }
            return await HttpTransport.Send(cachedData);
        }

        private CachedSentryData GetCacheFromEnvelope(SentryEnvelope envelope)
        {
            var memoryStream = new MemoryStream();
            ContribSentrySdk.@Serializer.Serialize(envelope, memoryStream);
            var cache = new CachedSentryData(envelope.Header.EventId,memoryStream.ToArray(), envelope.Items[0].Type.Type);
            memoryStream.Close();
            return cache;
        }

        private CachedSentryData GetCacheFromCurrentSession(ISession session)
        {
            var memoryStream = new MemoryStream();
            ContribSentrySdk.@Serializer.Serialize(session, memoryStream);
            var cache = new CachedSentryData(SentryId.Empty, memoryStream.ToArray(), ESentryType.CurrentSession);
            memoryStream.Close();
            return cache;
        }

    }
}
