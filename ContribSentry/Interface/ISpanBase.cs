using ContribSentry.Enums;
using System;
using System.Collections.Generic;

namespace ContribSentry.Interface
{
    public interface ISpanBase
    {
        string Description { get; }
        string Op { get; }
        string SpanId { get; }
        string ParentSpanId { get; }
        DateTimeOffset? StartTimestamp { get; }
        DateTimeOffset? Timestamp { get; }
        string TraceId { get; }
        bool Error { get; }
        string Status { get; }


        void Finish();
        void Finish(int? httpStatus);
        void Finish(Exception exception);   
        void GetParentSpans(List<ISpanBase> spans);
        ISpanBase StartChild(string url, ESpanRequest requestType);
        ISpanBase StartChild(string description, string op = null);
    }
}
