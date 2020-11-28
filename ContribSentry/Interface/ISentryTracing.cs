using ContribSentry.Enums;
using ContribSentry.Interface;
using Sentry.Protocol;
using System;
using System.Collections.Generic;

namespace ContribSentry.Internals
{
    public interface ISentryTracing : IScope
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
