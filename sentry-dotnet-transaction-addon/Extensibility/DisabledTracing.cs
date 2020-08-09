using sentry_dotnet_transaction_addon.Enums;
using sentry_dotnet_transaction_addon.Interface;
using sentry_dotnet_transaction_addon.Internals;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace sentry_dotnet_transaction_addon.Extensibility
{
    public class DisabledTracing : ISentryTracing
    {
        public static DisabledTracing Instance = new DisabledTracing();
        public List<ISpanBase> Spans => null;

        public DateTimeOffset StartTimestamp { get; }

        public void Dispose() { }
        public void Finish() { }

        public ISpanBase GetCurrentSpan() => DisabledSpan.Instance;

        public ISpanBase GetSpan(string op) => DisabledSpan.Instance;

        public ISpanBase StartChild(string description, string op = null) => null;

        public ISpanBase StartChild(string url, ESpanRequest requestType) => DisabledSpan.Instance;
    }
}
