using ContribSentry.Interface;
using ContribSentry.Transport;
using System.Threading.Tasks;

namespace ContribSentry.Internals
{
    internal class EndConsumerService : IEndConsumerService
    {
        public void CaptureSession(ISession session)
        {
            var envelope = SentryEnvelope.FromSession(session,
                ContribSentrySdk.Options.ContribSdk,
                ContribSentrySdk.@Serializer);
            Task.Run(async () => { await HttpTransport.Send(envelope, ContribSentrySdk.@Serializer); });
        }

        public void CaptureTracing(SentryTracingEvent tracing)
        {
            var envelope = SentryEnvelope.FromTracing(tracing,
                ContribSentrySdk.Options.ContribSdk,
                ContribSentrySdk.@Serializer);
            Task.Run(async () => { await HttpTransport.Send(envelope, ContribSentrySdk.@Serializer); });
        }
    }
}
