using ContribSentry.Enums;
using ContribSentry.Extensibility;
using ContribSentry.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContribSentry.Internals
{
    internal class ContribSentryTracingService : IContribSentryTracingService
    {
        internal List<KeyValuePair<int?, SentryTracing>> _transactionStorage = new List<KeyValuePair<int?, SentryTracing>>();

        internal ITracingContextTrackingId Tracker;

        internal ContribSentryTracingService(ITracingContextTrackingId tracker)
        {
            Tracker = tracker ?? new ThreadTracking();
        }

        public void Init(ContribSentryOptions options)
        {
        }

        public void Close()
        {
            _transactionStorage = null;
            Tracker = null;
        }

        public ISentryTracing RetreiveTransactionById(string id)
        {
            lock (_transactionStorage)
            {
                return _transactionStorage.FirstOrDefault(p => p.Value.Trace.TraceId == id).Value ?? (ISentryTracing)DisabledTracing.Instance;
            }
        }

        public ISentryTracing RetreiveTransactionByName(string name)
        {
            lock (_transactionStorage)
            {
                return _transactionStorage.FirstOrDefault(p => p.Value.Transaction == name).Value ?? (ISentryTracing)DisabledTracing.Instance;
            }
        }
        public ISentryTracing GetCurrentTransaction()
        {
            lock (_transactionStorage)
            {
                if (_transactionStorage.Count() == 0)
                    return DisabledTracing.Instance;
                var id = Tracker.GetId();
                var keyPair = _transactionStorage.LastOrDefault(p => p.Key == id);
                return keyPair.Value ?? (ISentryTracing)DisabledTracing.Instance;
            }
        }

        public ISpanBase GetCurrentTracingSpan()
        {
            return GetCurrentTransaction().GetCurrentSpan();
        }
        public void DisposeTracingEvent(SentryTracing tracing)
        {
            lock (_transactionStorage)
            {
                _transactionStorage.Remove(_transactionStorage.First(p => p.Value.Equals(tracing)));
            }
        }

        public ISentryTracing StartTransaction(string name)
        {
            var id = Tracker.ReserveNewId();
            var tracing = new SentryTracing(name, id);
            Tracker.AssociateId(id);
            lock (_transactionStorage)
            {
                _transactionStorage.Add(new KeyValuePair<int?, SentryTracing>(id, tracing));
            }
            return tracing;
        }


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

        public Task StartCallbackTrackingIdAsync(Func<Task> test, int? unsafeId)
        {

            return Tracker.WithIsolatedTracing(test, unsafeId.Value);
        }
    }
}
