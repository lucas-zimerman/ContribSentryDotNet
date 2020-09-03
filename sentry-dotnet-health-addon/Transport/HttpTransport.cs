using sentry_dotnet_health_addon.Extensions;
using sentry_dotnet_health_addon.Internals;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace sentry_dotnet_health_addon.Transport
{
    public static class HttpTransport
    {
        internal static HttpClient Client = new HttpClient();
        public static async Task Send(SentryEnvelope envelope, Serializer serializer)
        {
            var memoryStream = new MemoryStream();
            serializer.Serialize(envelope, memoryStream);
            var content = new ByteArrayContent(memoryStream.ToArray());
            memoryStream.Close();
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-sentry-envelope");
            var url = SentrySessionSdk.Options.Dsn.GetTracingUrl();
            await Client.PostAsync(url, content);
        }
    }
}