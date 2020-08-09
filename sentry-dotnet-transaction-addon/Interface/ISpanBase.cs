using sentry_dotnet_transaction_addon.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace sentry_dotnet_transaction_addon.Interface
{
    public interface ISpanBase : IDisposable
    {
        string Description { get; }
        string Op { get; }
        string SpanId { get; }
        string ParentSpanId { get; }
        DateTimeOffset? StartTimestamp { get; }
        DateTimeOffset? Timestamp { get; }
        string TraceId { get; }

        void Finish();
        void Finish(int? httpStatus);
        void GetParentSpans(List<ISpanBase> spans);
        ISpanBase StartChild(string url, ESpanRequest requestType);
        ISpanBase StartChild(string description, string op = null);
    }
}
