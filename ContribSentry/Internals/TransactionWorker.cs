using ContribSentry.Enums;
using ContribSentry.Extensibility;
using ContribSentry.Interface;
using Sentry;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ContribSentry.Internals
{
    internal class TransactionWorker : ITransactionWorker
    {
        internal AsyncLocal<SentryTracing> TracingContext = new AsyncLocal<SentryTracing>();

        private ContribSentryOptions _options;

        public void Init(ContribSentryOptions options) 
        {
            _options = options;
        }

        public void Close() { }

        public ISentryTracing GetCurrentTransaction()
            => TracingContext.Value ?? (ISentryTracing)DisabledTracing.Instance;

        public ISpanBase GetCurrentTracingSpan()
            => GetCurrentTransaction().GetCurrentSpan();

        public void StartTransaction(string name, string method, Action<ISentryTracing> action)
            => Task.Run(() =>
            {
                ISentryTracing tracing = DisabledTracing.Instance;
                if(TracingContext.Value is null)
                {
                    TracingContext.Value = new SentryTracing(name, method);
                    tracing = TracingContext.Value;
                }
                try
                {
                    action(tracing);
                }
                catch(Exception ex)
                {
                    tracing.Finish();
                    throw;
                }

            }).GetAwaiter();


        public ISpanBase StartChild(string url, ESpanRequest requestType)
        {
            var transaction = GetCurrentTransaction();
            var span = transaction.GetCurrentSpan();
            if (span is DisabledSpan)
            {
                return transaction.StartChild(url, requestType);
            }
            return span.StartChild(url, requestType);
        }

        public ISpanBase StartChild(string description, string op = null)
        {
            var transaction = GetCurrentTransaction();
            var span = transaction.GetCurrentSpan();
            if (span is DisabledSpan)
            {
                return transaction.StartChild(description, op);
            }
            return span.StartChild(description, op);
        }

        public void CaptureTransaction(SentryTracing tracing, Exception ex)
        {
            var hasError = ex != null;
            if (!hasError)
            {
                hasError = tracing.Spans.Any(p => p.Error);
                tracing.Trace.SetStatus(tracing.Spans.LastOrDefault()?.Status);
            }
            else
            {
                tracing.Trace.SetStatus(ex.GetType().ToString());
            }

            var @event = new SentryTracingEvent(tracing, hasError);
            if (_options.RegisterTracingBreadcrumb)
            {
                SentrySdk.AddBreadcrumb(@event.EventId.ToString(), "sentry.transaction");
            }
            SentrySdk.WithScope(scope =>
            {
                SentrySdk.CaptureEvent(@event);
            });
        }
    }
}
