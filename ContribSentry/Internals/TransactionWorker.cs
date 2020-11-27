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
        internal AsyncLocal<SentryTracing> CurrentTransaction = new AsyncLocal<SentryTracing>();
        internal AsyncLocal<ISpanBase> CurrentSpan = new AsyncLocal<ISpanBase>();
        private ContribSentryOptions _options;

        public void Init(ContribSentryOptions options) 
        {
            _options = options;
        }

        public void Close() { }

        public ISentryTracing GetCurrentTransaction()
            => CurrentTransaction.Value ?? (ISentryTracing)DisabledTracing.Instance;

        public ISpanBase GetCurrentTracingSpan()
            => CurrentSpan.Value ?? (ISpanBase)DisabledSession.Instance;

        public Task StartTransaction(string name, string method, Action<ISentryTracing> action)
            => Task.Run(() =>
            {
                ISentryTracing tracing = DisabledTracing.Instance;
                if(CurrentTransaction.Value is null)
                {
                    CurrentTransaction.Value = new SentryTracing(name, method);
                    tracing = CurrentTransaction.Value;
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

            });


        public Task StartChild(string url, ESpanRequest requestType, Action<ISpanBase> action)
            => Task.Run(() =>
            {
                if (CurrentSpan.Value is null)
                {
                    var transaction = GetCurrentTransaction();
                    CurrentSpan.Value = transaction.StartChild(url, requestType);
                }
                else
                {
                    CurrentSpan.Value = CurrentSpan.Value.StartChild(url, requestType);
                }
                try
                {
                    action.Invoke(CurrentSpan.Value);
                    CurrentSpan.Value.Finish();
                }
                catch (Exception ex)
                {
                            //                                   CurrentSpan.Value.Finish();
                        }
            });

        public Task StartChild(string description, string op, Action<ISpanBase> action)
            => Task.Run(() =>
                           {
                               if (CurrentSpan.Value is null)
                               {
                                   var transaction = GetCurrentTransaction();
                                   CurrentSpan.Value = transaction.StartChild(description, op);
                               }
                               else
                               {
                                   CurrentSpan.Value = CurrentSpan.Value.StartChild(description, op);
                               }
                               try
                               {
                                   action.Invoke(CurrentSpan.Value);
                                   CurrentSpan.Value.Finish();
                               }
                               catch (Exception ex)
                               {
//                                   CurrentSpan.Value.Finish();
                               }
                           });

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
