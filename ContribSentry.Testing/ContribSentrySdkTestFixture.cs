using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Sentry;

namespace ContribSentry.Testing
{
    public abstract class ContribSentrySdkTestFixture : IDisposable
    {
        public TestServer TestServer { get; set; }

        public HttpClient HttpClient { get; set; }
        public IServiceProvider ServiceProvider { get; set; }

        public Action<IServiceCollection> ConfigureServices { get; set; }
        public Action<IApplicationBuilder> ConfigureApp { get; set; }
        public Action<IWebHostBuilder> ConfigureWehHost { get; set; }

        protected virtual void Build(Action<HttpContext> externalMiddleware = null)
        {
            var builder = new WebHostBuilder();

            _ = builder.ConfigureServices(s =>
            {
                ConfigureServices?.Invoke(s);
            });
            _ = builder.Configure(app =>
            {
                ConfigureApp?.Invoke(app);
                _ = app.Use(async (context, next) =>
                {
                    externalMiddleware?.Invoke(context);
                });
            });

            ConfigureWehHost?.Invoke(builder);
            ConfigureBuilder(builder);

            TestServer = new TestServer(builder);
            HttpClient = TestServer.CreateClient();
            ServiceProvider = TestServer.Host.Services;
        }

        protected virtual void ConfigureBuilder(WebHostBuilder builder)
        {

        }

        public void Dispose()
        {
            SentrySdk.Close();
        }
    }
}
