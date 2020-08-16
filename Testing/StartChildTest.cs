using sentry_dotnet_transaction_addon;
using sentry_dotnet_transaction_addon.Enums;
using sentry_dotnet_transaction_addon.Extensibility;
using sentry_dotnet_transaction_addon.Interface;
using sentry_dotnet_transaction_addon.Internals;
using System.Threading.Tasks;
using Xunit;

namespace Testing
{
    public class StartChildTest
    {


        public StartChildTest()
        {
            Initializer.Init();
        }

        public ISpanBase AddSpan(string name) => SentryTracingSdk.StartChild(name);

        [Fact]
        public void SentryTracingSdkStartChildWithoutTransactionReturnDisabledSpan()
        {
            new Task(() =>
            {
                Assert.IsType<DisabledSpan>(AddSpan("_"));
            }).Start();
        }

        [Fact]
        public void SentryTracingSdkStartChildCanDispose()
        {
            ISentryTracing transaction = SentryTracingSdk.StartTransaction("tran");
            using (var span = transaction.StartChild("span"))
            {
                Task.Delay(100).Wait();
            }
            Assert.NotEqual(transaction.Spans[0].StartTimestamp, transaction.Spans[0].Timestamp);
            transaction.Finish();
        }

        [Fact]
        public void SentryTracingSdkStartSubChild()
        {
            ISentryTracing transaction = SentryTracingSdk.StartTransaction("tran");
            using (var span = transaction.StartChild("span"))
            {
                Task.Delay(100).Wait();

                using (var subSpan = span.StartChild("subspan"))
                {
                    Task.Delay(100).Wait();
                }
            }
            Assert.NotEqual(transaction.Spans[0].StartTimestamp, transaction.Spans[0].Timestamp);
            Assert.NotEqual(transaction.Spans[1].StartTimestamp, transaction.Spans[1].Timestamp);
            Assert.Equal(transaction.Spans[0].SpanId, transaction.Spans[1].ParentSpanId);
            transaction.Finish();
        }

        [Fact]
        public void SentryTracingSdkStartRequestChildOkStatus()
        {
            ISentryTracing transaction = SentryTracingSdk.StartTransaction("tran");
            using (var span = transaction.StartChild("span"))
            {
                Task.Delay(100).Wait();

                using (var subSpan = span.StartChild("subspan", ESpanRequest.Post))
                {
                    Task.Delay(100).Wait();
                    subSpan.Finish(200);
                }
            }
            Assert.NotEqual(transaction.Spans[0].StartTimestamp, transaction.Spans[0].Timestamp);
            Assert.NotEqual(transaction.Spans[1].StartTimestamp, transaction.Spans[1].Timestamp);
            Assert.Equal(transaction.Spans[0].SpanId, transaction.Spans[1].ParentSpanId);
            var statusSpan = transaction.Spans[1] as Span;
            Assert.Equal("ok", statusSpan.Status);
            transaction.Finish();
        }

        [Fact]
        public void SentryTracingSdkStartRequestChildErrorStatus()
        {
            ISentryTracing transaction = SentryTracingSdk.StartTransaction("tran");
            using (var span = transaction.StartChild("span"))
            {
                Task.Delay(100).Wait();

                using (var subSpan = span.StartChild("subspan", ESpanRequest.Post))
                {
                    Task.Delay(100).Wait();
                    subSpan.Finish(500);
                }
            }
            Assert.NotEqual(transaction.Spans[0].StartTimestamp, transaction.Spans[0].Timestamp);
            Assert.NotEqual(transaction.Spans[1].StartTimestamp, transaction.Spans[1].Timestamp);
            Assert.Equal(transaction.Spans[0].SpanId, transaction.Spans[1].ParentSpanId);
            var statusSpan = transaction.Spans[1] as Span;
            Assert.Equal("internal_error", statusSpan.Status);
            transaction.Finish();
        }

    }
}
