using System;
using System.Collections.Generic;

namespace sentry_dotnet_transaction_addon
{
    public interface ITransactionEvent
    {
        List<Span> Spans { get; }

        DateTimeOffset StartTimestamp { get; }
    }
}
