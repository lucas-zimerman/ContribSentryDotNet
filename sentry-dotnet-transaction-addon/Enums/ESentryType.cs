using System;
using System.Collections.Generic;
using System.Text;

namespace sentry_dotnet_transaction_addon.Enums
{
    internal enum ESentryType
    {
        Session,
        Event,
        Attachment,
        Transaction,
        Unknown
    }
}
