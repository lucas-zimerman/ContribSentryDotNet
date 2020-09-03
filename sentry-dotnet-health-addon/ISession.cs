using sentry_dotnet_health_addon.Enums;
using System;

namespace sentry_dotnet_health_addon
{
    public interface ISession
    {
        DateTime? Started { get; }
        DateTime? Timestamp { get; }
        int? ErrorCount { get; }
        string DistinctId { get; }
        Guid SessionId { get; }
        bool? Init { get; }
        SessionState Status { get; set; }
        long? Sequence { get; }
        long? Duration { get; }
        SessionAttributes Attributes { get; }
        void End(DateTime? timestamp);
        void RegisterError();
    }
}
