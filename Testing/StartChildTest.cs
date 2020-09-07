using ContribSentry.Enums;
using ContribSentry.Extensibility;
using ContribSentry.Interface;
using ContribSentry.Internals;
using System.Threading.Tasks;
using Xunit;

namespace ContribSentry.TracingTest
{
    public class StartChildTest
    {
        public IContribSentryTracingService Service;

        private void SetupService()
        {
            Service = new ContribSentryTracingService();
            Service.Init(null);
        }


        public ISpanBase AddSpan(string name) => Service.StartChild(name, null);

        [Fact]
        public void SentryTracingSdkStartChildWithoutTransactionReturnDisabledSpan()
        {
            new Task(() =>
            {
                try
                {
                    SetupService();
                    Assert.IsType<DisabledSpan>(AddSpan("_"));
                }
                finally
                {
                    Service = null;
                }
            }).Start();
        }

        [Fact]
        public void SentryTracingSdkStartChildCanDispose()
        {
            try
            {
                SetupService();
                var transaction = Service.StartTransaction("tran");
                using (var span = transaction.StartChild("span"))
                {
                    Task.Delay(100).Wait();
                }
                Assert.NotEqual(transaction.Spans[0].StartTimestamp, transaction.Spans[0].Timestamp);
                transaction.Finish();
            }
            finally
            {
                Service = null;
            }
        }

        [Fact]
        public void SentryTracingSdkStartSubChild()
        {
            try
            {
                SetupService();
                var transaction = Service.StartTransaction("tran");
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
            finally
            {
                Service = null;
            }
        }

        [Fact]
        public void SentryTracingSdkStartRequestChildOkStatus()
        {
            try
            {
                SetupService();
                var transaction = Service.StartTransaction("tran");
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
            finally
            {
                Service = null;
            }
        }

        [Fact]
        public void SentryTracingSdkStartRequestChildErrorStatus()
        {
            try
            {
                SetupService();
                var transaction = Service.StartTransaction("tran");
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
            finally
            {
                Service = null;
            }
        }

    }
}
