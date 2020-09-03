using sentry_dotnet_health_addon.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace sentry_dotnet_health_addon.Extensions
{
    internal static class ESentryTypeExtensions
    {
        public static string ConvertString(this ESentryType type)
        {
            if (type == ESentryType.Attachment)
                return "attachment";
            if (type == ESentryType.Event)
                return "event";
            if (type == ESentryType.Session)
                return "session";
            if (type == ESentryType.Transaction)
                return "transaction";
            return "unknown";
        }
    }
}
