using ContribSentry.Interface;
using ContribSentry.Internals;
using Sentry;
using Sentry.Extensibility;
using System;

namespace ContribSentry
{
    public class ContribSentryOptions
    {
        internal static readonly double TracesSampleRateDefault = 1.0;

        internal static readonly bool RegisterTracingBreadcrumbDefault = true;

        internal static readonly int CacheDirSizeDefault = 100;

        public ContribSentryOptions(bool transactionEnable = true, bool sessionEnable = true, bool cacheEnable = false)
        {
            TransactionEnabled = transactionEnable;
            if (TransactionEnabled)
            {
                TracesSampleRate = TracesSampleRateDefault;
                RegisterTracingBreadcrumb = RegisterTracingBreadcrumbDefault;
            }
            SessionEnabled = sessionEnable;

            CacheEnabled = cacheEnable;
            EventCacheEnabled = cacheEnable;
            CacheDirSize = CacheDirSizeDefault;

            ContribSdk = new SdkVersion() { Name = "ContribSentry", Version = "4.0.0" };
        }

        internal void ConsumeSentryOptions(SentryOptions options)
        {
            Dsn = ContribDsn.Parse(options.Dsn);
            Environment = options.Environment;
            Release = options.Release;
            BeforeSend = options.BeforeSend;
            DiagnosticLogger = options.DiagnosticLogger;

        }

        internal void DisableCache()
        {
            CacheEnabled = false;
        }

        internal IDiagnosticLogger DiagnosticLogger { get; private set; }
        internal ContribDsn Dsn { get; set; }
        internal string Environment { get; set; }
        internal string Release { get; set; }
        internal bool IsEnabled => SessionEnabled || TransactionEnabled;
        
        public bool TransactionEnabled { get; private set; }
        public bool SessionEnabled { get; private set; }
        public bool CacheEnabled { get; private set; }

        internal bool EventCacheEnabled { get; private set; }
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
        public bool RegisterTracingBreadcrumb { get; set; }

        public string CacheDirPath { get; set; }

        public int CacheDirSize { get; set; }

        internal Func<bool> HasInternet { get; private set; }

        internal SdkVersion ContribSdk { get; set; }

        public IContribSentryTracingService TracingService { get; private set; }
        public IContribSentrySessionService SessionService { get; private set; }

        public IEnvelopeCache EnvelopeCache { get; private set; }

        public IEventCache EventCache { get; private set; }

        /// <summary>
        /// Disable the caching of Sentry events from ContribSentry.
        /// </summary>
        public void DisableEventCaching()
        {
            EventCacheEnabled = false;
        }

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

        /// <summary>
        /// Used to know if ContribSentry should cache or not SentryEvents since<br/>.
        /// it act as a middleware.
        /// </summary>
        /// <param name="hasInternet">the call that should return if the application has internet or not.</param>
        public void SetHasInternetCallback(Func<bool> hasInternet)
        {
            HasInternet = hasInternet;
        }

        /// <summary>
        /// Tries to Inject a custom TracingContext that another project requested.
        /// </summary>
        internal ITracingContextTrackingId TrackingIdMethod { get; set; }

        /// <summary>
        ///     A callback to invoke before sending an event to Sentry
        /// </summary>
        /// <Remarks>
        ///     The return of this event will be sent to Sentry. This allows the application
        ///     a chance to inspect and/or modify the event before it's sent. If the event should
        ///     not be sent at all, return null from the callback.
        /// </Remarks>
        internal Func<SentryEvent, SentryEvent> BeforeSend { get; set; }

    }
}
