using ContribSentry.Interface;
using Sentry;
using Sentry.Protocol;

namespace ContribSentry
{
    public class ContribSentryOptions
    {
        internal static readonly double TracesSampleRateDefault = 1.0;

        internal static readonly bool RegisterTracingBreadcrumb = true;

        public ContribSentryOptions(bool transactionEnable = true, bool sessionEnable = true)
        {
            TransactionEnabled = transactionEnable;
            if (TransactionEnabled)
            {
                TracesSampleRate = TracesSampleRateDefault;
                RegisterTracingBreadcrmub = RegisterTracingBreadcrumb;
            }
            SessionEnabled = sessionEnable;

            ContribSdk = new SdkVersion() { Name = "ContribSentry", Version = "3.0.0" };
        }

        internal Dsn Dsn { get; set; }
        internal string Environment { get; set; }
        internal string Release { get; set; }
        internal bool IsEnabled => SessionEnabled || TransactionEnabled;
        
        public bool TransactionEnabled { get; private set; }
        public bool SessionEnabled { get; private set; }

        /// <summary>
        /// True for single user applications like Apps, Otherwise, False.
        /// </summary>
        public bool GlobalSessionMode { get; set; }

        /// <summary>
        /// The Device Id or the unique id that represents an user.
        /// </summary>
        public string DistinctId { get; set; }

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

        internal SdkVersion ContribSdk { get; set; }

        public IContribSentryTracingService TracingService { get; private set; }
        public IContribSentrySessionService SessionService { get; private set; }

        /// <summary>
        /// Allows to Inject a custom Tracing Service, must be set before SentrySdk.Init<br/>
        /// is called.
        /// </summary>
        /// <param name="service">The Tracing service.</param>
        public void SetTracingService(IContribSentryTracingService service)
        {
            TracingService = service;
        }
        /// <summary>
        /// Allows to Inject a custom Session Service, must be set before SentrySdk.Init<br/>
        /// is called.
        /// </summary>
        /// <param name="service">The Session service.</param>
        public void SetSessionService(IContribSentrySessionService service)
        {
            SessionService = service;
        }
    }
}
