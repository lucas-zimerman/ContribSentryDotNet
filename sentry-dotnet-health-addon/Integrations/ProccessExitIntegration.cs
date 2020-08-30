using System;
using System.Collections.Generic;
using System.Text;

namespace sentry_dotnet_health_addon.Integrations
{
    internal class ProccessExitIntegration : IDisposable
    {
        internal ProccessExitIntegration()
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        }

        public void Dispose()
        {
            AppDomain.CurrentDomain.ProcessExit -= CurrentDomain_ProcessExit;            
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
