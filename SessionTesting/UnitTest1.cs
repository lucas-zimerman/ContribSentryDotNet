using Sentry;
using Sentry.Protocol;
using sentry_dotnet_health_addon;
using sentry_dotnet_health_addon.Transport;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SessionTesting
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            /*
            var opt = new SentryOptions()
            {
                Dsn = new Dsn("https://56872a996f48477fa79641d7def05cd1@o201058.ingest.sentry.io/5327152"),
                Release = "1.1",
                Environment = "test"
                
            };
            SentrySdk.Init(opt);
            SentrySdk.ConfigureScope(scope =>
            {
                scope.User = new User() { Id = "123" };
            });

            var session = new ISession(Guid.NewGuid().ToString(), new User() { Id = "123" }, "test", "1.1");
            Thread.Sleep(2000);
//            SentrySdk.CaptureMessage("Hi");
            Thread.Sleep(2000);
            session.End(DateTime.Now);
            await HttpTransport.SendEnvelope(session);
            */
        }
    }
}
