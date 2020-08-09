using Sentry;
using sentry_dotnet_transaction_addon.Enums;
using sentry_dotnet_transaction_addon.Extensibility;
using sentry_dotnet_transaction_addon.Interface;
using sentry_dotnet_transaction_addon.Internals;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sentry_dotnet_transaction_addon
{
    public static class SentryTracingSdk
    {
        internal static List<KeyValuePair<int?, SentryTracing>> _transactionStorage = new List<KeyValuePair<int?, SentryTracing>>();

        internal static SentryTracingOptions TracingOptions { get; set; }

        /// <summary>
        /// Initialize the Tracing Sdk.
        /// </summary>
        public static void Init(string dsn)
        {
            Init(new SentryTracingOptions(new Dsn(dsn)));
        }

        /// <summary>
        /// Initialize the Tracing Sdk.
        /// </summary>
        public static void Init(Dsn dsn)
        {
            Init(new SentryTracingOptions(dsn));
        }

        /// <summary>
        /// Initialize the Tracing Sdk.
        /// </summary>
        public static void Init(SentryTracingOptions options)
        {
            TracingOptions = options;
        }

        public static bool IsEnabled() => TracingOptions != null;

        public static void Close()
        {
            _transactionStorage.Clear();
            _transactionStorage = null;
            TracingOptions = null;
        }

        public static ISentryTracing StartTransaction(string name)
        {
            var tracing = new SentryTracing(name);
            _transactionStorage.Add(new KeyValuePair<int?, SentryTracing>(Task.CurrentId, tracing));
            return tracing;
        }

        public static ISentryTracing RetreiveTransactionById(string id)
        {
            return _transactionStorage.FirstOrDefault(p => p.Value.Trace.TraceId == id).Value ?? (ISentryTracing)DisabledTracing.Instance;
        }

        public static ISentryTracing RetreiveTransactionByName(string name)
        {
            return _transactionStorage.FirstOrDefault(p => p.Value.Transaction == name).Value ?? (ISentryTracing)DisabledTracing.Instance;
        }

        /// <summary>
        /// Gets the Last Transaction from the current Task
        /// </summary>
        /// <returns></returns>
        public static ISentryTracing GetCurrentTransaction()
        {
            if (!IsEnabled() || _transactionStorage.Count() == 0)
                return DisabledTracing.Instance;
            var keyPair = _transactionStorage.LastOrDefault(p => p.Key == Task.CurrentId);
            return keyPair.Value ?? (ISentryTracing)DisabledTracing.Instance;
        }

        public static ISpanBase GetCurrentTracingSpan()
        {
            return GetCurrentTransaction().GetCurrentSpan();
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
            _transactionStorage.Remove(_transactionStorage.First(p => p.Value.Equals(tracing)));
        }
    }
}
