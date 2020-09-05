using ContribSentry.Enums;
using ContribSentry.Interface;
using System;
using System.Collections.Generic;

namespace ContribSentry.Extensibility
{
    public partial class DisabledSpan : ISpanBase
    {

        internal static DisabledSpan Instance = new DisabledSpan(null, null);

        public DisabledSpan(string description, string op = null) { }

        public string Description => null;
        public string Op => null;
        public string SpanId => null;
        public string ParentSpanId => null;
        public DateTimeOffset? StartTimestamp => null;
        public DateTimeOffset? Timestamp => null;
        public string TraceId => null;

        public void Dispose() { }

        public void Finish() { }
        public void Finish(int? httpStatus) { }
        public void GetParentSpans(List<ISpanBase> spans) { }

        public ISpanBase StartChild(string description, string op = null) => Instance;
        public ISpanBase StartChild(string url, ESpanRequest requestType) => Instance;
    }
}
