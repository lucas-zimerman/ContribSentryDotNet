using Sentry.Protocol;
using ContribSentry.Enums;
using ContribSentry.Internals;
using ContribSentry.Interface;
using ContribSentry.Extensibility;
using System;
using ContribSentry.Cache;
using Sentry;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ContribSentry
{
    public static class ContribSentrySdk
    {

        internal static ITransactionWorker TracingService = DisabledTracingService.Instance;
        internal static IContribSentrySessionService SessionService = DisabledSessionService.Instance;
        internal static IEventCache EventCache = DisabledDiskCache.Instance;
        internal static IEnvelopeCache EnvelopeCache = DisabledEnvelopeCache.Instance;
        internal static IContribSentryTransport Transport;
        internal static CacheFileWorker CacheFileWorker;
        internal static ContribSentryOptions Options;
        internal static Serializer @Serializer;

        public static bool IsEnabled => Options != null;

        public static bool IsSessionSdkEnabled => Options?.SessionEnabled ?? false;

        public static bool IsTracingSdkEnabled => Options?.TransactionEnabled ?? false;
        public static bool IsCacheEnabled => Options?.CacheEnabled ?? false;

        internal static void Init(ContribSentryOptions options)
        {
            if (options?.IsEnabled == true)
            {
                Options = options;
                @Serializer = new Serializer();
                Transport = new ContribSentryTransport(options);

                if (IsSessionSdkEnabled)
                {
                    Options.DiagnosticLogger?.Log(SentryLevel.Debug, "ContribSentry Initializing Session Service");
                    SessionService = Options.SessionService ?? new ContribSentrySessionService();
                    SessionService.Init(Options, Transport);
                    Options.DiagnosticLogger?.Log(SentryLevel.Debug, "ContribSentry Session Service Initialzed");
                }
                if (IsTracingSdkEnabled)
                {
                    Options.DiagnosticLogger?.Log(SentryLevel.Debug, "ContribSentry Initializing Tracing Service");
                    TracingService = Options.TracingService ?? new TransactionWorker();
                    TracingService.Init(Options);
                    Options.DiagnosticLogger?.Log(SentryLevel.Debug, "ContribSentry Tracing Service Initialzed");
                }
                if (IsCacheEnabled)
                {
                    Options.DiagnosticLogger?.Log(SentryLevel.Debug, "ContribSentry Initializing Cache Service");
                    EventCache = new DiskCache(Options);
                    EnvelopeCache = new EnvelopeCache(Options);
                    CacheFileWorker = new CacheFileWorker(Options);
                    if (!CacheFileWorker.StartWorker())
                    {
                        Options.DisableCache();
                        CacheFileWorker = null;
                    }
                    else
                        Options.DiagnosticLogger?.Log(SentryLevel.Debug, "ContribSentry Cache Service Initialzed");
                }
                Options.DiagnosticLogger?.Log(SentryLevel.Debug, "ContribSentry Initialized");

            }
        }

        /// <summary>
        /// Backup the current data (Clientside apps)
        /// </summary>
        public static void Sleep()
        {
            SessionService.CacheCurrentSesion();
            TracingService.Sleep();
        }

        /// <summary>
        /// Inform that your application was resumed (Clientside apps)
        /// </summary>
        public static void Resume()
        {
            SessionService.DeleteCachedCurrentSession();
            TracingService.Resume();
        }

        /// <summary>
        /// Clears the project Data.
        /// </summary>
        public static void Close()
        {
            if (IsEnabled)
            {
                SessionService.Close();
                TracingService.Close();
            }

            SessionService = DisabledSessionService.Instance;
            TracingService = DisabledTracingService.Instance;
            EventCache = DisabledDiskCache.Instance;
            EnvelopeCache = DisabledEnvelopeCache.Instance;
            Options.DiagnosticLogger?.Log(SentryLevel.Debug, $"ContribSentry Closed");
            CacheFileWorker = null;
            Transport = null;
            Options = null;
        }

        #region Session
        /// <summary>
        /// Start new Session for the given user, if the user is not set, it'll use the DistinctId to distinct the user.
        /// </summary>
        public static void StartSession(User user = null)
        {
            if (IsEnabled)
                SessionService.StartSession(user, Options.DistinctId, Options.Environment, Options.Release);
        }

        /// <summary>
        /// End a session and send to Sentry
        /// </summary>
        public static void EndSession()
        {
            SessionService.EndSession();
        }

        #endregion
        #region Transaction
        /// <summary>
        /// Get the current transaction in the current context.
        /// </summary>
        /// <returns></returns>
        public static ISentryTracing GetCurrentTransaction()
            => TracingService.GetCurrentTransaction();

        public static ISpanBase GetCurrentTracingSpan()
            => TracingService.GetCurrentTracingSpan();

        /// <summary>
        /// Create a new Transaction with the given name.
        /// </summary>
        /// <param name="name">the transaction name.</param>
        /// <returns></returns>
        public static void StartTransaction(string name, Action<ISentryTracing> action, [CallerMemberName] string method = "")
            => TracingService.StartTransaction(name, method, action);

        /// <summary>
        /// start a span in the current active transaction
        /// </summary>
        /// <param name="url">the http request url.</param>
        /// <param name="requestType">the request type.</param>
        /// <returns></returns>
        public static Task StartChild(string url, ESpanRequest requestType, Action<ISpanBase> action = null)
            => TracingService.StartChild(url, requestType, action);

        /// <summary>
        /// start a span in the current active transaction.
        /// </summary>
        /// <param name="description">the description of the span, like a Sql Query or another data.</param>
        /// <param name="op">the method name.</param>
        /// <returns></returns>
        public static Task StartChild(string description, [CallerMemberName] string op = "", Action<ISpanBase> action = null)
            => TracingService.StartChild(description, op, action);

        public static Task StartChild(string description, [CallerMemberName] string op = "", Func<ISpanBase, Task> func = null)
            => TracingService.StartChild(description, op, func);

        internal static void CaptureTransaction(SentryTracing tracing, Exception ex)
            => TracingService.CaptureTransaction(tracing, ex);
        #endregion
    }
}