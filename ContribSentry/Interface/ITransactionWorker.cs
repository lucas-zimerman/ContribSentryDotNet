using ContribSentry.Enums;
using ContribSentry.Internals;
using System;
using System.Threading.Tasks;

namespace ContribSentry.Interface
{
    public interface ITransactionWorker
    {
        void Init(ContribSentryOptions options);
        void Close();
        ISentryTracing GetCurrentTransaction();
        ISpanBase GetCurrentTracingSpan();
        Task StartTransaction(string name, string method, Action<ISentryTracing> action);
        Task StartChild(string url, ESpanRequest requestType, Action<ISpanBase> action);
        Task StartChild(string description, string op, Action<ISpanBase> action);
        void CaptureTransaction(SentryTracing tracing, Exception ex);
    }
}
