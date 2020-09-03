using sentry_dotnet_transaction_addon.Enums;
using sentry_dotnet_transaction_addon.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sentry_dotnet_transaction_addon.Internals
{
    public interface ISentryTracing : IDisposable
    {
        List<ISpanBase> Spans { get; }

        DateTimeOffset StartTimestamp { get; }
        ISpanBase GetSpan(string op);

        ISpanBase GetCurrentSpan();

        ISpanBase StartChild(string description, string op = null);
        ISpanBase StartChild(string url, ESpanRequest requestType);

        /// <summary>
        /// Invoke the user code on an isolated environment so that you can interact with Tracing from anywhere. 
        /// </summary>
        /// <returns>A task where the user code is running.</returns>
        Task IsolateTracking(Func<Task> trackedCode);
        void Finish();
    }
}
