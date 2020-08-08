using Sentry;
using System.Collections.Generic;
using System.Linq;

namespace sentry_dotnet_transaction_addon
{
    public static class SentryTracingSDK
    {
        internal static List<KeyValuePair<string, SentryTracing>> _transactionStorage = new List<KeyValuePair<string, SentryTracing>>();

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

        public static SentryTracing StartTransaction(string name)
        {
            var tracing = new SentryTracing(name);
            _transactionStorage.Add(new KeyValuePair<string, SentryTracing>(name, tracing));
            return tracing;
        }

        public static SentryTracing RetreiveTransactionById(string id)
        {
            return _transactionStorage.FirstOrDefault(p => p.Value.Id == id).Value;//<- it could go kaboom
        }

        public static SentryTracing RetreiveTransactionByName(string name)
        {
            return _transactionStorage.FirstOrDefault(p => p.Key == name).Value;//<- it could go kaboom
        }

        internal static void DisposeTracingEvent(SentryTracing tracing)
        {
            _transactionStorage.Remove(_transactionStorage.First(p => p.Key == tracing.Transaction && p.Value.Equals(tracing)));
        }
    }
}
