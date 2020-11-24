using ContribSentry.Enums;
using ContribSentry.Interface;
using System;
using System.Collections.Generic;

namespace ContribSentry.Internals
{
    public interface ISentryTracing
    {
        List<ISpanBase> Spans { get; }
        DateTimeOffset StartTimestamp { get; }
        ISpanBase GetCurrentSpan();

        ISpanBase StartChild(string description, string op = null);
        ISpanBase StartChild(string url, ESpanRequest requestType);
        void Finish();
        void Finish(Exception ex);
    }
}
