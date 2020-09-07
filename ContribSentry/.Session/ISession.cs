using ContribSentry.Enums;
using System;

namespace ContribSentry
{
    public interface ISession
    {
        DateTime? Started { get; }
        DateTime? Timestamp { get; }
        int? ErrorCount { get; }
        string DistinctId { get; }
        Guid SessionId { get; }
        bool? Init { get; }
        ESessionState Status { get; set; }
        long? Sequence { get; }
        long? Duration { get; }
        SessionAttributes Attributes { get; }
        void End(DateTime? timestamp);
        void RegisterError();
    }
}
