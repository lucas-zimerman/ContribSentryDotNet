using Sentry;
using sentry_dotnet_transaction_addon.Enums;
using sentry_dotnet_transaction_addon.Extensibility;
using sentry_dotnet_transaction_addon.Interface;
using sentry_dotnet_transaction_addon.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace sentry_dotnet_transaction_addon
{
    public static class SentryTracingSdk
    {
        internal static List<KeyValuePair<int?, SentryTracing>> _transactionStorage = new List<KeyValuePair<int?, SentryTracing>>();
        internal static ThreadTracking Tracker;

        internal static SentryTracingOptions TracingOptions { get; set; }

        /// <summary>
        /// Change the Dsn for SentryTracingSdk, it's optional since the Dsn is taken from SentrySdk.
        /// </summary>
        public static void SetDsn(Dsn dsn)
        {
            if (IsEnabled())
            {
                TracingOptions.Dsn = dsn;
            }
        }

        /// <summary>
        /// Initialize the Tracing Sdk.
        /// </summary>
        internal static void Init(SentryTracingOptions options)
        {
            TracingOptions = options;
            Tracker = new ThreadTracking();
        }

        public static bool IsEnabled() => TracingOptions != null;

        public static void Close()
        {
            _transactionStorage = null;
            TracingOptions = null;
            Tracker = null;
        }

        public static ISentryTracing RetreiveTransactionById(string id)
        {
            lock (_transactionStorage)
            {
                return _transactionStorage.FirstOrDefault(p => p.Value.Trace.TraceId == id).Value ?? (ISentryTracing)DisabledTracing.Instance;
            }
        }

        public static ISentryTracing RetreiveTransactionByName(string name)
        {
            lock (_transactionStorage)
            {
                return _transactionStorage.FirstOrDefault(p => p.Value.Transaction == name).Value ?? (ISentryTracing)DisabledTracing.Instance;
            }
        }

        /// <summary>
        /// Gets the Last Transaction from the current Task
        /// </summary>
        /// <returns></returns>
        public static ISentryTracing GetCurrentTransaction()
        {
            lock (_transactionStorage)
            {
                if (!IsEnabled() || _transactionStorage.Count() == 0 || !Tracker.Created)
                    return DisabledTracing.Instance;
                var id = Tracker.Id;
                var keyPair = _transactionStorage.LastOrDefault(p => p.Key == id);
                return keyPair.Value ?? (ISentryTracing)DisabledTracing.Instance;
            }
        }

        public static ISpanBase GetCurrentTracingSpan()
        {
            return GetCurrentTransaction().GetCurrentSpan();
        }

        public static ISentryTracing StartTransaction(string name)
        {
            var id = Tracker.StartUnsafeTrackingId();
            var tracing = new SentryTracing(name, id);
            lock (_transactionStorage)
            {
                _transactionStorage.Add(new KeyValuePair<int?, SentryTracing>(id, tracing));
            }
            return tracing;
        }

        public static ISpanBase StartChild(string url, ESpanRequest requestType)
        {
            var transaction = GetCurrentTransaction();
            var span = transaction.GetCurrentSpan();
            if(span is DisabledSpan)
            {
                return transaction.StartChild(url, requestType);
            }
            return span.StartChild(url, requestType);
        }

        public static ISpanBase StartChild(string description, string op = null)
        {
            var transaction = GetCurrentTransaction();
            var span = transaction.GetCurrentSpan();
            if (span is DisabledSpan)
            {
                return transaction.StartChild(description, op);
            }
            return span.StartChild(description, op);
        }

        internal static void DisposeTracingEvent(SentryTracing tracing)
        {
            lock (_transactionStorage)
            {
                _transactionStorage.Remove(_transactionStorage.First(p => p.Value.Equals(tracing)));
            }
        }
    }
}
