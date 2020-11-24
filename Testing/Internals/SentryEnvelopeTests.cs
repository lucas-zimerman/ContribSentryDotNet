using ContribSentry.Internals;
using Sentry.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ContribSentry.TracingTest.Internals
{
    public class SentryEnvelopeTests
    {
        [Fact]
        public void NewEnvelope_FromTracing_Has_EventId_Set()
        {
            var serializer = new Serializer();
            var sdk = new SdkVersion();
            var tracing = new SentryTracingEvent(new SentryTracing(null,0),false);
            var envelope = SentryEnvelope.FromTracing(tracing,sdk,serializer);
            Assert.Equal(tracing.EventId, envelope.Header.EventId);
            Assert.NotEqual(SentryId.Empty, envelope.Header.EventId);
        }
    }
}
