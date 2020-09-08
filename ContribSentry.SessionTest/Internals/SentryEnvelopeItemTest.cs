using ContribSentry.Enums;
using ContribSentry.Internals;
using Sentry.Protocol;
using Xunit;

namespace ContribSentry.SessionTest.Internals
{
    public class SentryEnvelopeItemTest
    {
        [Fact]
        public void New_Session_Creates_Envelope_Item_With_Session_Type()
        {
            var session = new Session("2", new User() { Id = "1" }, "e", "r");
            var serializer = new Serializer();
            var item = SentryEnvelopeItem.FromSession(session, serializer);
            Assert.Equal(ESentryType.Session, item.Type.Type);
            Assert.NotNull(item.Data);
        }
    }
}
