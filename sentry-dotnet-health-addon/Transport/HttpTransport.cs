using Newtonsoft.Json;
using sentry_dotnet_health_addon.Extensibility;
using sentry_dotnet_health_addon.Extensions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace sentry_dotnet_health_addon.Transport
{
    public static class HttpTransport
    {
        public static async Task SendEnvelope(ISession session)
        {
            if (session is DisabledSession) return;
            var json = JsonConvert.SerializeObject(session, new JsonSerializerSettings() 
            { 
                DateFormatHandling = DateFormatHandling.IsoDateFormat, 
                DateTimeZoneHandling =  DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore                
            });
            await Task.Run(async () =>
            {
                try
                {
                    using (var client = new HttpClient())
                    {

                        var header = "{}";
                        //var header = JsonConvert.SerializeObject(new SentryEnvelopeHeader());

                        var @type = "{\"type\":\"session\"}";
                        var content = new StringContent(header + '\n' + @type + '\n' + json);
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/x-sentry-envelope");
                        var url = SentrySessionSdk.Options.Dsn.GetTracingUrl();
                        var @return = await client.PostAsync(url, content);
                        var response = await @return.Content.ReadAsStringAsync();
                        var sent = content.ReadAsStringAsync();
                    }
                }
                finally { }
            });

        }
    }
}
