using System;
using System.Collections.Generic;
using System.Text;

namespace sentry_dotnet_health_addon.Internals
{
    internal interface IHealthContainer
    {
        Session GetCurrent();

        void CreateNewSession(Session session);
    }
}
