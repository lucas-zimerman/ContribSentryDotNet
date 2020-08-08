using Sentry;

namespace sentry_dotnet_transaction_addon
{
    public class SentryTracingOptions
    {
        public SentryTracingOptions(Dsn dsn, double tracesSampleRate = 1.0, bool registerTracingBreadcrumb = true)
        {
            Dsn = dsn;
            TracesSampleRate = tracesSampleRate;
            RegisterTracingBreadcrmub = registerTracingBreadcrumb;
        }

        public Dsn Dsn { get; set; }
        /// <summary>
        /// the rate of sending events where
        /// <para> 1.0 you always send a performance Event.</para> 
        /// <para> 0 You'll never send events.</para> 
        /// </summary>
        public double TracesSampleRate { get; set; }

        /// <summary>
        /// When set to true, it'll always register a breadcrumb with
        /// <br>the given tracing event Id if the event is sent to Sentry</br>
        /// </summary>
        public bool RegisterTracingBreadcrmub { get; set; }

    }
}
