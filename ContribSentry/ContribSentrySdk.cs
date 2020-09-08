using Sentry.Protocol;
using ContribSentry.Enums;
using ContribSentry.Internals;
using ContribSentry.Interface;
using ContribSentry.Extensibility;

namespace ContribSentry
{
    public static class ContribSentrySdk
    {

        internal static IContribSentryTracingService TracingService = DisabledTracingService.Instance;
        internal static IContribSentrySessionService SessionService = DisabledSessionService.Instance;
        internal static IEndConsumerService EndConsumer;
        internal static ContribSentryOptions Options;
        
        internal static Serializer @Serializer;

        public static bool IsEnabled => Options != null;

        public static bool IsSessionSdkEnabled => Options?.SessionEnabled ?? false;

        public static bool IsTracingSdkEnabled => Options?.TransactionEnabled ?? false;

        internal static void Init(ContribSentryOptions options)
        {
            if (options?.IsEnabled == true)
            {
                Options = options;
                @Serializer = new Serializer();
                EndConsumer = new EndConsumerService();

                if (IsSessionSdkEnabled)
                {
                    SessionService = Options.SessionService ?? new ContribSentrySessionService();
                    SessionService.Init(Options, EndConsumer);
                }
                if (IsTracingSdkEnabled)
                {
                    TracingService = Options.TracingService ?? new ContribSentryTracingService(Options.TrackingIdMethod);
                    TracingService.Init(Options);
                }
            }
        }

        public static void Close()
        {
            if (IsEnabled)
            {
                SessionService.Close();
                TracingService.Close();
            }

            SessionService = DisabledSessionService.Instance;
            TracingService = DisabledTracingService.Instance;
            EndConsumer = null;
            Options = null;
        }
        
        #region Session
        public static void StartSession(User user)
        {
            if (IsEnabled)
                SessionService.StartSession(user, Options.DistinctId, Options.Environment, Options.Release);
        }

        public static void EndSession()
        {
            SessionService.EndSession();
        }

        #endregion
        #region Transaction
        public static ISentryTracing RetreiveTransactionById(string id)
        {
            return TracingService.RetreiveTransactionById(id);
        }

        public static ISentryTracing RetreiveTransactionByName(string name)
        {
            return TracingService.RetreiveTransactionByName(name);
        }

        /// <summary>
        /// Gets the Last Transaction from the current Task
        /// </summary>
        /// <returns></returns>
        public static ISentryTracing GetCurrentTransaction()
        {
            return TracingService.GetCurrentTransaction();
        }

        public static ISpanBase GetCurrentTracingSpan()
        {
            return TracingService.GetCurrentTracingSpan();
        }

        public static ISentryTracing StartTransaction(string name)
        {
            return TracingService.StartTransaction(name);
        }

        public static ISpanBase StartChild(string url, ESpanRequest requestType)
        {
            return TracingService.StartChild(url, requestType);
        }

        public static ISpanBase StartChild(string description, string op = null)
        {
            return TracingService.StartChild(description, op);
        }
        #endregion
    }
}