using ContribSentry.Enums;
using ContribSentry.Internals;
using System;
using System.Threading.Tasks;

namespace ContribSentry.Interface
{
    public interface IContribSentryTracingService
    {
        void Init(ContribSentryOptions options);
        void Close();
        ISentryTracing RetreiveTransactionById(string id);
        ISentryTracing RetreiveTransactionByName(string name);
        ISentryTracing GetCurrentTransaction();
        ISpanBase GetCurrentTracingSpan();
        ISentryTracing StartTransaction(string name);
        ISpanBase StartChild(string url, ESpanRequest requestType);
        ISpanBase StartChild(string description, string op);
        void DisposeTracingEvent(SentryTracing tracing);
        Task StartCallbackTrackingIdAsync(Func<Task> test, int? unsafeId);
    }
}
