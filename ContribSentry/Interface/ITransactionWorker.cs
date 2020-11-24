using ContribSentry.Enums;
using ContribSentry.Internals;
using System;

namespace ContribSentry.Interface
{
    public interface ITransactionWorker
    {
        void Init(ContribSentryOptions options);
        void Close();
        ISentryTracing GetCurrentTransaction();
        ISpanBase GetCurrentTracingSpan();
        void StartTransaction(string name, string method, Action<ISentryTracing> action);
        ISpanBase StartChild(string url, ESpanRequest requestType);
        ISpanBase StartChild(string description, string op);
        void CaptureTransaction(SentryTracing tracing, Exception ex);
    }
}
