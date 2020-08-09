using sentry_dotnet_transaction_addon;
using sentry_dotnet_transaction_addon.Enums;
using sentry_dotnet_transaction_addon.Extensibility;
using sentry_dotnet_transaction_addon.Interface;
using sentry_dotnet_transaction_addon.Internals;
using System.Threading.Tasks;
using Testing.Helpers;
using Xunit;

namespace Testing
{
    public class StartChildTest
    {
        public ISpanBase AddSpan(string name) => SentryTracingSdk.StartChild(name);

        [Fact]
        public void SentryTracingSdkStartChildConcurrenceFindsCorrectParents()
        {
            SentryTracingSdk.Init(DsnHelper.ValidDsnWithoutSecret);
            ISentryTracing a = null, b = null, c = null;
            ISpanBase sa = null, sb = null, sc = null;
            Task []tasks = new Task[3];
            tasks[0] = new Task(() =>
            {
                a = SentryTracingSdk.StartTransaction("a");
                sa = AddSpan("a");
                sa.Finish();
            });
            tasks[1] = new Task(() =>
            {
                b = SentryTracingSdk.StartTransaction("b");
                sb = AddSpan("b");
                sb.Finish();
            });
            tasks[2] = new Task(() =>
            {
                c = SentryTracingSdk.StartTransaction("c");
                sc = AddSpan("c");
                sc.Finish();
            });

            tasks[1].Start();
            tasks[0].Start();
            tasks[2].Start();

            Task.WaitAll(tasks);

            //Check if all Tracings have only one Span
            Assert.True(a.Spans.Count.Equals(1));
            Assert.True(b.Spans.Count.Equals(1));
            Assert.True(c.Spans.Count.Equals(1));

            //Check if all Tracing have the correct Spans
            Assert.Equal(sa, a.Spans[0]);
            Assert.Equal(sb, b.Spans[0]);
            Assert.Equal(sc, c.Spans[0]);
            a.Finish();
            b.Finish();
            c.Finish();
        }

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
            SentryTracingSdk.Init(DsnHelper.ValidDsnWithoutSecret);
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
            SentryTracingSdk.Init(DsnHelper.ValidDsnWithoutSecret);
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
            SentryTracingSdk.Init(DsnHelper.ValidDsnWithoutSecret);
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
            SentryTracingSdk.Init(DsnHelper.ValidDsnWithoutSecret);
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
