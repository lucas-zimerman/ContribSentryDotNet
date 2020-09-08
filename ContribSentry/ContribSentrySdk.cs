using Sentry.Protocol;
using ContribSentry.Enums;
using ContribSentry.Internals;
using ContribSentry.Interface;
using ContribSentry.Extensibility;
using System;

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
            EndConsumer = null;
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
        [Obsolete]
        public static ISentryTracing RetreiveTransactionById(string id)
        {
            return TracingService.RetreiveTransactionById(id);
        }
        /// <summary>
        /// Return an active transaction (if found) where the name matches the parameter name.
        /// </summary>
        /// <param name="name">the transaction name.</param>
        /// <returns></returns>
        public static ISentryTracing RetreiveTransactionByName(string name)
        {
            return TracingService.RetreiveTransactionByName(name);
        }

        /// <summary>
        /// Get the current transaction in the current context.
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

        /// <summary>
        /// Create a new Transaction with the given name.
        /// </summary>
        /// <param name="name">the transaction name.</param>
        /// <returns></returns>
        public static ISentryTracing StartTransaction(string name)
        {
            return TracingService.StartTransaction(name);
        }

        /// <summary>
        /// start a span in the current active transaction
        /// </summary>
        /// <param name="url">the http request url.</param>
        /// <param name="requestType">the request type.</param>
        /// <returns></returns>
        public static ISpanBase StartChild(string url, ESpanRequest requestType)
        {
            return TracingService.StartChild(url, requestType);
        }

        /// <summary>
        /// start a span in the current active transaction.
        /// </summary>
        /// <param name="description">the description of the span, like a Sql Query or another data.</param>
        /// <param name="op">Normally the name of the action or function</param>
        /// <returns></returns>
        public static ISpanBase StartChild(string description, string op = null)
        {
            return TracingService.StartChild(description, op);
        }
        #endregion
    }
}