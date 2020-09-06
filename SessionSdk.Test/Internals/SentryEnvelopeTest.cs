using ContribSentry.Enums;
using ContribSentry.Internals;
using Sentry.Protocol;
using Xunit;

namespace ContribSentry.SessionTest.Internals
{
    public class SentryEnvelopeTest
    {
        [Fact]
        public void New_Session_Creates_Envelope_With_EventyId_Empty_And_EnvelopeItem_As_Session()
        {
            var session = new Session("2", new User() { Id = "1" }, "e", "r");
            var sdk = new SdkVersion() { Name = "a", Version = "c" };
            var serializer = new Serializer();
            var envelope = SentryEnvelope.FromSession(session, sdk, serializer);
            Assert.Equal(SentryId.Empty, envelope.Header.EventId);
            Assert.Equal(sdk, envelope.Header.SdkVersion);
            Assert.Equal(ESentryType.Session , envelope.Items[0].Type.Type);
            Assert.NotNull(envelope.Items[0].Data);
        }
    }
}
