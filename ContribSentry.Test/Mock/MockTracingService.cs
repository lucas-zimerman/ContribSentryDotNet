using ContribSentry.Enums;
using ContribSentry.Extensibility;
using ContribSentry.Interface;
using ContribSentry.Internals;
using System;
using System.Threading.Tasks;

namespace ContribSentry.Test.Mock
{
    public class MockTracingService : IContribSentryTracingService
    {
        public void Close() { }

        public void DisposeTracingEvent(SentryTracing tracing) { }

        public ISpanBase GetCurrentTracingSpan() => DisabledSpan.Instance;

        public ISentryTracing GetCurrentTransaction() => DisabledTracing.Instance;

        public void Init(ContribSentryOptions options) { }

        public ISentryTracing RetreiveTransactionById(string id) => DisabledTracing.Instance;

        public ISentryTracing RetreiveTransactionByName(string name) => DisabledTracing.Instance;

        public Task StartCallbackTrackingIdAsync(Func<Task> test, int? unsafeId) => new Task(() => { });

        public ISpanBase StartChild(string url, ESpanRequest requestType) => DisabledSpan.Instance;

        public ISpanBase StartChild(string description, string op) => DisabledSpan.Instance;

        public ISentryTracing StartTransaction(string name) => DisabledTracing.Instance;
    }
}
