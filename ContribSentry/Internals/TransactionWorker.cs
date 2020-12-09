using ContribSentry.Enums;
using ContribSentry.Extensibility;
using ContribSentry.Extensions;
using ContribSentry.Interface;
using ContribSentry.Internals.EventProcessor;
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
        private SentryTracingEventProcessor _tracingEventProcessor;
        private ContribSentryOptions _options;
        private AsyncLocal<ISpanBase> _sleepSpan = new AsyncLocal<ISpanBase>();

        public void Init(ContribSentryOptions options)
        {
            _options = options;
            _tracingEventProcessor = new SentryTracingEventProcessor(options);
        }

        public void Close() { }

        public ISentryTracing GetCurrentTransaction()
            => CurrentTransaction.Value ?? (ISentryTracing)DisabledTracing.Instance;

        public ISpanBase GetCurrentTracingSpan()
            => CurrentSpan.Value ?? (ISpanBase)DisabledSession.Instance;

        public Task StartTransaction(string name, string method, Action<ISentryTracing> action)
            => Task.Run(async () =>
            {
                ISentryTracing tracing = DisabledTracing.Instance;
                if (CurrentTransaction.Value is null)
                {
                    CurrentTransaction.Value = new SentryTracing(name, method);
                    tracing = CurrentTransaction.Value;
                }
                try
                {
                    action(tracing);
                    tracing.Finish();
                }
                catch (Exception ex)
                {
                    tracing.Finish(ex);
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
                    action(CurrentSpan.Value);
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
                                   action(CurrentSpan.Value);
                                   CurrentSpan.Value.Finish();
                               }
                               catch (Exception ex)
                               {
                                   CurrentSpan.Value.Finish(ex);
                               }
                           });

        public Task StartChild(string description, string op, Func<ISpanBase, Task> func)
            => Task.Run(async () =>
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
                    await func(CurrentSpan.Value);
                    CurrentSpan.Value.Finish();
                }
                catch (Exception ex)
                {
                    CurrentSpan.Value.Finish(ex);
                }
            });

        /// <summary>
        /// Captures the Transaction that was completed.
        /// </summary>
        /// <param name="tracing">The transaction.</param>
        /// <param name="ex">The exception.</param>
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
                tracing.Trace.SetStatus(SpanStatus.FromException(ex).ToString());
            }

            if (_options.RegisterTracingBreadcrumb)
            {
                SentrySdk.AddBreadcrumb(tracing.EventId.ToString(), "sentry.transaction");
            }
            SentrySdk.WithScope(scope =>
            {
                tracing = _tracingEventProcessor.Process(tracing, scope);
                if (tracing != null)
                {
                    ContribSentrySdk.Transport.CaptureTracing(tracing);
                }
            });
        }

        public void Sleep()
        {
            if (CurrentSpan.Value is null)
            {
                var transaction = GetCurrentTransaction();
                _sleepSpan.Value = transaction.StartChild("Sleep", "ContribSentrySDK");
            }
            else
            {
                _sleepSpan.Value = CurrentSpan.Value.StartChild("Sleep", "ContribSentrySDK");
            }
        }

        public void Resume()
        {
            if(_sleepSpan.Value is Span span)
            {
                span.Finish("idle");
            }
        }
    }
}
