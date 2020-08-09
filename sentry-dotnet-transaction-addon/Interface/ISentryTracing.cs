using sentry_dotnet_transaction_addon.Enums;
using sentry_dotnet_transaction_addon.Interface;
using System;
using System.Collections.Generic;

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
        void Finish();
    }
}
